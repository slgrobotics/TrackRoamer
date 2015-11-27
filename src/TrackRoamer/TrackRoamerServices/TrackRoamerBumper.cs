using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using pxbumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

using TrackRoamer.Robotics.Utility.LibSystem;
using powerbrick = TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper
{
    
    [Contract(Contract.Identifier)]
    [AlternateContract(pxbumper.Contract.Identifier)]
    [DisplayName("(User) TrackRoamer Contact Sensor - Front Whiskers")]
    [Description("Provides access to the TrackRoamer array of whiskers sensors used as a bumper.\n(Uses Generic Contact Sensors contract.)")]
    public class BumperService : DsspServiceBase
    {
        private const string _configFile = ServicePaths.Store + "/TrackRoamer.Services.Bumper.config.xml";  // C:\Microsoft Robotics Dev Studio 4\projects\TrackRoamer\TrackRoamerServices\Config\TrackRoamer.TrackRoamerBot.Bumper.Config.xml

		[EmbeddedResource("TrackRoamer.Robotics.Services.TrackRoamerServices.TrackRoamerBumper.xslt")]
        string _transform = null;
        
        [InitialStatePartner(Optional = true, ServiceUri = _configFile)]
        private pxbumper.ContactSensorArrayState _state = new pxbumper.ContactSensorArrayState();

        private bool _subscribed = false;

		[ServicePort("/TrackRoamerBumper", AllowMultipleInstances = true)]
        private pxbumper.ContactSensorArrayOperations _mainPort = new pxbumper.ContactSensorArrayOperations();

        #region Powerbrick partner - for whiskers
        [Partner("TrackRoamerPowerBrick", Contract = powerbrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate, Optional = false)]
        protected powerbrick.TrackRoamerBrickPowerOperations _trackroamerbotPort = new powerbrick.TrackRoamerBrickPowerOperations();
        private Port<powerbrick.UpdateWhiskers> notificationPortWhiskers = new Port<powerbrick.UpdateWhiskers>();
        #endregion


        [Partner("SubMgr", Contract=submgr.Contract.Identifier, CreationPolicy=PartnerCreationPolicy.CreateAlways, Optional=false)]
        private submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();
        
        /// <summary>
        /// Cached version of the Interleave arbiter that protects concurrent access on Dssp handlers
		/// We use it to combine with an interleave that manages notifications from the TrackRoamerBot service
        /// </summary>
        //private Interleave _mainInterleave;

        public BumperService(DsspServiceCreationPort creationPort) : 
                base(creationPort)
        {
            LogInfo("TrackRoamerBumper:BumperService() -- port: " + creationPort.ToString());
        }

        protected override void Start()
        {
            //configure initial state
            if (_state == null)
            {
                LogInfo("TrackRoamerBumper:Start(): _state == null - initializing...");

                _state = new pxbumper.ContactSensorArrayState();
                _state.Sensors = new List<pxbumper.ContactSensor>();

                pxbumper.ContactSensor leftBumper = new pxbumper.ContactSensor();
                leftBumper.HardwareIdentifier = 101;
                leftBumper.Name = "Front Whisker Left";

                _state.Sensors.Add(leftBumper);

                pxbumper.ContactSensor rightBumper = new pxbumper.ContactSensor();
                rightBumper.HardwareIdentifier = 201;
                rightBumper.Name = "Front Whisker Right";

                _state.Sensors.Add(rightBumper);

                SaveState(_state);
            }
            else
            {
                LogInfo("TrackRoamerBumper:Start(): _state is supplied by file: " + _configFile);
            }

            base.Start();

            MainPortInterleave.CombineWith(
                new Interleave(
                    new TeardownReceiverGroup(),
                    new ExclusiveReceiverGroup(
                        Arbiter.Receive<powerbrick.UpdateWhiskers>(true, notificationPortWhiskers, WhiskersNotificationHandler)
                    ),
                    new ConcurrentReceiverGroup()
                )
            );

			// display HTTP service Uri
            LogInfo("TrackRoamerBumper:Start() Service URI=" + ServiceInfo.HttpUri());

            // Subscribe to the Hardware Controller for bumper notifications
			SubscribeToTrackRoamerBot();
        }

        /// <summary>
        /// Subscribe to Whiskers notifications
        /// </summary>
		private void SubscribeToTrackRoamerBot()
        {
            Type[] notifyMeOfDistance = new Type[] { typeof(powerbrick.UpdateWhiskers) };

            Tracer.Trace("TrackRoamerBumper: calling Subscribe() for UpdateWhiskers");

            // Subscribe to the TrackRoamerBot and wait for a response
            Activate(
                Arbiter.Choice(_trackroamerbotPort.Subscribe(notificationPortWhiskers, notifyMeOfDistance),
                    delegate(SubscribeResponseType Rsp)
                    {
                        // update our state with subscription status
                        _subscribed = true;
                        // Subscription was successful, update our state with subscription status:
                        LogInfo("TrackRoamerBumper: Subscription to Power Brick Service for UpdateWhiskers succeeded");
                    },
                    delegate(Fault F)
                    {
                        LogError("TrackRoamerBumper: Subscription to Power Brick Service for UpdateWhiskers failed");
                    }
                )
            );
        }

        /// <summary>
        /// receiving Whiskers notifications here
        /// </summary>
        /// <param name="notification"></param>
        public void WhiskersNotificationHandler(powerbrick.UpdateWhiskers notification)
        {
			/*
				HardwareIdentifier coding:
				  1st digit   1=Left 2=Right
				  2d  digit   0=Front 1=Rear
				  3d  digit   1=Whisker 2=IRBumper 3=StepSensor
			 */

            foreach (pxbumper.ContactSensor bumper in _state.Sensors)
            {
                bool changed = false;

				switch (bumper.HardwareIdentifier)
				{
					case 101:
						if (notification.Body.FrontWhiskerLeft != null && bumper.Pressed != notification.Body.FrontWhiskerLeft)
						{
							bumper.Pressed = (bool)notification.Body.FrontWhiskerLeft;
							changed = true;
						}
						break;

					case 201:
						if (notification.Body.FrontWhiskerRight != null && bumper.Pressed != notification.Body.FrontWhiskerRight)
						{
							bumper.Pressed = (bool)notification.Body.FrontWhiskerRight;
							changed = true;
						}
						break;
				}

				if (changed)
				{
					this.SendNotification<pxbumper.Update>(_subMgrPort, new pxbumper.Update(bumper));
				}
            }
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(pxbumper.Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            httpGet.ResponsePort.Post(new HttpResponseType(
                HttpStatusCode.OK,
                _state,
                _transform)
            );
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> UpdateHandler(pxbumper.Update updateBumper)
        {
            throw new InvalidOperationException("Track Roamer Bumper Sensors are not updateable");
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ReplaceHandler(pxbumper.Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SubscribeHandler(pxbumper.Subscribe subscribe)
        {
            LogInfo("TrackRoamerBumper received Subscription request from Subscriber=" + subscribe.Body.Subscriber);
            base.SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ReliableSubscribeHandler(pxbumper.ReliableSubscribe subscribe)
        {
            LogInfo("TrackRoamerBumper received Reliable Subscription request from Subscriber=" + subscribe.Body.Subscriber);
            base.SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
        {
            LogInfo("TrackRoamerBumper:DropHandler()");

            base.DefaultDropHandler(drop);

            yield break;
        }
    }
}
