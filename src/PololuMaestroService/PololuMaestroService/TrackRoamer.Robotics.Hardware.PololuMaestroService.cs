using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using W3C.Soap;
using System.Net;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

// Pololu "Mini Maestro 12" is used here to control the servos. See PololuDLLs folder for dlls. Need to install driver too from Pololu.
using Pololu.Usc;
using Pololu.UsbWrapper;

namespace TrackRoamer.Robotics.Hardware.PololuMaestroService
{
	[Contract(Contract.Identifier)]
	[DisplayName("(User) Pololu Maestro Service")]
	[Description("Pololu Maestro Service service controls servos and reads analog signals")]
	class PololuMaestroService : DsspServiceBase
	{
        private const string _configFile = ServicePaths.Store + "/PololuMaestroService.user.config.xml";

        // we are using Pololu Maestro Mini 12 http://www.pololu.com/catalog/product/1352
        private const int CHANNELS_PER_DEVICE = 12;

		/// <summary>
		/// Service state
		/// </summary>
        [ServiceState(StateTransform = "TrackRoamer.Robotics.Hardware.PololuMaestroService.TrackRoamer.Robotics.Hardware.PololuMaestroService.xslt")]
        [InitialStatePartner(Optional = true, ServiceUri = _configFile)]
		private PololuMaestroServiceState _state = new PololuMaestroServiceState();

        /// <summary>
		/// Main service port
		/// </summary>
		[ServicePort("/PololuMaestroService", AllowMultipleInstances = false)]
		PololuMaestroServiceOperations _mainPort = new PololuMaestroServiceOperations();
		
		[SubscriptionManagerPartner]
		submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();
		
		/// <summary>
		/// Service constructor
		/// </summary>
		public PololuMaestroService(DsspServiceCreationPort creationPort)
			: base(creationPort)
		{
		}
		
		/// <summary>
		/// Service start
		/// </summary>
		protected override void Start()
		{
            try
            {
                bool stateChanged = false;

                if (_state == null)
                {
                    _state = new PololuMaestroServiceState();
                    stateChanged = true;
                }

                if (_state.SafePositions == null)
                {
                    _state.SafePositions = new List<SafePosition>();
                    //_state.SafePositions.Add(new SafePosition() { channel = 4, positionUs = 1000 });
                    stateChanged = true;
                }

                if(stateChanged)
                {
                    SaveState(_state);
                }

                SetSafePositions();
            }
            catch (Exception exception)
            {
                // Fatal exception during startup, shutdown service
                LogError(LogGroups.Activation, exception);
                DefaultDropHandler(new DsspDefaultDrop());
                return;
            }
			
			base.Start();
		}

        private void SetSafePositions()
        {
            Console.WriteLine("Set Safe Positions: " + _state.SafePositions.Count);

            foreach (SafePosition safePosition in _state.SafePositions)
            {
                Console.WriteLine("    channel: {0}    target: {1} us", safePosition.channel, safePosition.positionUs);

                TrySetTarget(safePosition.channel, (ushort)(safePosition.positionUs << 2));
            }
        }

        /// <summary>
        /// Pololu Maestro Command Handler
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> PololuMaestroCommandHandler(SendPololuMaestroCommand cmd)
        {
            //Console.WriteLine("PololuMaestroCommand: " + cmd.Body);

            TrySetTarget(cmd.Body.ChannelValues);

            cmd.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }


        #region Operation Handlers

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// Http Get Handler
        /// </summary>
        /// <param name="httpGet"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;

            HttpResponseType rsp = null;

            rsp = new HttpResponseType(HttpStatusCode.OK, _state, base.StateTransformPath);
            //rsp = new HttpResponseType(HttpStatusCode.OK, _state, _transformPololuMaestroData);

            httpGet.ResponsePort.Post(rsp);
            yield break;

        }

        /// <summary>
		/// Handles Subscribe messages
		/// </summary>
		/// <param name="subscribe">the subscribe request</param>
		[ServiceHandler]
		public void SubscribeHandler(Subscribe subscribe)
		{
			SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
		}

        /// <summary>
        /// Shut down the service, put everything in safe positions
        /// </summary>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
        {
            SetSafePositions();

            base.DefaultDropHandler(drop);
            yield break;
        }

        #endregion // Operation Handlers

        #region Servo controls

        /// <summary>
        /// Attempts to set the target (width of pulses sent) of a channel.
        /// </summary>
        /// <param name="channel">Channel number - from 0 to 23.</param>
        /// <param name="target">
        ///   Target, in units of quarter microseconds.  For typical servos,
        ///   6000 is neutral and the acceptable range is 4000-8000.
        ///   A good servo will take 880 to 2200 us (3520 to 8800 in quarters)
        /// </param>
        private void TrySetTarget(Byte channel, UInt16 target)
        {
            try
            {
                int index = channel / CHANNELS_PER_DEVICE;
                channel = (byte)(channel % CHANNELS_PER_DEVICE);

                using (Usc device = connectToDevice(index))  // Find a device and temporarily connect.
                {

                    //Console.WriteLine("    (s) device: {0}    channel: {1}    target: {2}", device.getSerialNumber(), channel, target);

                    device.setTarget(channel, target);

                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception);
            }
        }

        private void TrySetTarget(List<ChannelValuePair> channelValues)
        {
            try
            {
                var groupedByDevice = from a in channelValues
                         group a by a.Channel / CHANNELS_PER_DEVICE into g
                         select new { deviceIndex = g.Key, deviceChannelValues = g };

                foreach (var devGrp in groupedByDevice)
                {

                    using (Usc device = connectToDevice(devGrp.deviceIndex))  // Find a device and temporarily connect.
                    {
                        foreach (ChannelValuePair cvp in devGrp.deviceChannelValues)
                        {
                            byte channel = (byte)(cvp.Channel % CHANNELS_PER_DEVICE);
                            //Console.WriteLine("    (m) device: {0}    channel: {1}    target: {2}", device.getSerialNumber(), channel, cvp.Target);
                            device.setTarget(channel, cvp.Target);
                        }
                        // device.Dispose() is called automatically when the "using" block ends,
                        // allowing other functions and processes to use the device.
                    }
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception);
            }
        }

        private void TryGetTarget(Byte channel)
        {
            try
            {
                int index = channel / CHANNELS_PER_DEVICE;
                channel = (byte)(channel % CHANNELS_PER_DEVICE);

                using (Usc device = connectToDevice(index))  // Find a device and temporarily connect.
                {
                    ServoStatus[] servos;
                    device.getVariables(out servos);

                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception);
            }
        }

        // we assume that devices serial numbers are in increasing sequence:
        Comparison<DeviceListItem> deviceComparer = new Comparison<DeviceListItem>((a, b) => int.Parse(a.serialNumber) - int.Parse(b.serialNumber));

        /// <summary>
        /// Connects to a Maestro using native USB and returns the Usc object
        /// representing that connection.  When you are done with the
        /// connection, you should close it using the Dispose() method so that
        /// other processes or functions can connect to the device later.  The
        /// "using" statement can do this automatically for you.
        /// </summary>
        private Usc connectToDevice(int index)
        {
            // Get a list of all connected devices of this type.
            List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();
            connectedDevices.Sort(deviceComparer);

            // we must have all three devices online, or we will be commanding a wrong device.
            if (connectedDevices.Count() != 3)
            {
                throw new Exception("Not all Pololu devices are online - found " + connectedDevices.Count() + "  expected 3");
            }

            DeviceListItem dli = connectedDevices[index];

            //foreach (DeviceListItem dli in connectedDevices)
            {
                // If you have multiple devices connected and want to select a particular
                // device by serial number, you could simply add a line like this:
                //   if (dli.serialNumber != "00012345"){ continue; }

                Usc device = new Usc(dli); // Connect to the device.
                return device;             // Return the device.
            }
            throw new Exception("Could not find Pololu device.\r\nMake sure it's plugged into USB\r\nCheck your Device Manager.\r\nPololu Mini Maestro 12 needed.");
        }

        #endregion // Servo controls
	}
}


