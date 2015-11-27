using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

using System.Net;
using System.Net.Mime;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using TrackRoamer.Robotics.Utility.LibPicSensors;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard
{
    // The ActivationSettings attribute with Sharing == false makes the runtime dedicate a dispatcher thread pool just for this service.
    // ExecutionUnitsPerDispatcher	- Indicates the number of execution units allocated to the dispatcher
    // ShareDispatcher	            - Inidicates whether multiple service instances can be pooled or not
    [ActivationSettings(ShareDispatcher = false, ExecutionUnitsPerDispatcher = 8)]
    [Contract(Contract.Identifier)]
    [DisplayName("(User) TrackRoamer Proximity Brick")]
    [Description("TrackRoamer Proximity Brick represents a PIC4550 based Proximity Board with sonars and IR sensors")]
    class TrackRoamerBrickProximityBoardService : DsspServiceBase
    {
        // USB Device ID for Proximity Module. Must match definitions in Microchip PIC microcontroller code (USB Device - HID - Proximity Module\Generic HID - Firmware\usb_descriptors.c lines 178, 179)
        private const Int32 vendorId = 0x0925;
        private const Int32 productId = 0x7001;

        private const int openDelayMs = 5000;

        /// <summary>
        /// Declare the service state and also the XSLT Transform for displaying it
        /// </summary>
        [ServiceState(StateTransform = "TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.TrackRoamerBrickProximityBoard.xslt")]
        TrackRoamerBrickProximityBoardState _state = new TrackRoamerBrickProximityBoardState() {
            ProductId = productId,
            VendorId = vendorId,
            AngularRange = 180,
            AngularResolution = ((double)180) / 26.0d,
            Description = "TrackRoamer Brick Proximity Board Service"
        };

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/trproxboard", AllowMultipleInstances = false)]
        TrackRoamerBrickProximityBoardOperations _mainPort = new TrackRoamerBrickProximityBoardOperations();

        ProximityBoardCcrServiceCommander _pbCommander;
        DispatcherQueue _pbCommanderTaskQueue = null;
        PBCommandPort _pbCommanderDataEventsPort = new PBCommandPort();      // data (Sonar, Direction, Accelerometer, Proximity) is posted to this port by ProximityBoardCcrServiceCommander

        DsspHttpUtilitiesPort _httpUtilities;


        /*
         * others can subscribe for the following types:
         * UpdateSonarData, UpdateDirectionData, UpdateAccelerometerData, UpdateProximityData, ResetType and whole TrackRoamerBrickProximityBoardState
         * 
         * example:
         * 
         *             Arbiter.Receive<trpb.UpdateSonarData>(true, _trpbNotify, trpbUpdateNotification)    <- put this in interleave
         * 
         *             Type[] notifyMeOf = new Type[] { typeof(trpb.UpdateSonarData) };
         *             _trpbPort.Subscribe(_trpbNotify, notifyMeOf);
         * 
         */
        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Service constructor
        /// </summary>
        public TrackRoamerBrickProximityBoardService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::TrackRoamerBrickProximityBoardService()");
        }

        /// <summary>
        /// Service start
        /// </summary>
        /// <summary>
        /// Send phase of construction.
        /// </summary>
        protected override void Start()
        {
            //if (_state == null)
            //{
            //    _state = new TrackRoamerBrickProximityBoardState();
            //}

            Tracer.Trace("TrackRoamerBrickProximityBoardService::Start()");

            LogInfo("TrackRoamerBrickProximityBoardService::Start");

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            //
            // Kick off the connection to the Proximity Board device.
            //
            SpawnIterator(0, StartProximityBoard);

            // This service does not use base.Start() because of the way that
            // the handlers are hooked up. Also, because of this, there are
            // explicit Get and HttpGet handlers instead of using the default ones.
            // Handlers that need write or Exclusive access to state go under
            // the Exclusive group. Handlers that need read or shared access, and can be
            // Concurrent to other readers, go to the Concurrent group.
            // Other internal ports can be included in interleave so you can coordinate
            // intermediate computation with top level handlers.
            Activate(
                Arbiter.Interleave(
                new TeardownReceiverGroup(
                    Arbiter.Receive<DsspDefaultDrop>(false, _mainPort, DropHandler)),
                new ExclusiveReceiverGroup(
                    Arbiter.Receive<Replace>(true, _mainPort, ReplaceHandler),
                    Arbiter.Receive<SonarData>(true, _pbCommanderDataEventsPort, SonarMeasurementHandler),
                    Arbiter.Receive<DirectionData>(true, _pbCommanderDataEventsPort, DirectionMeasurementHandler),
                    Arbiter.Receive<AccelerometerData>(true, _pbCommanderDataEventsPort, AccelerometerMeasurementHandler),
                    Arbiter.Receive<ProximityData>(true, _pbCommanderDataEventsPort, ProximityMeasurementHandler),
                    Arbiter.Receive<ParkingSensorData>(true, _pbCommanderDataEventsPort, ParkingSensorMeasurementHandler),
                    Arbiter.Receive<AnalogData>(true, _pbCommanderDataEventsPort, AnalogMeasurementHandler),
                    Arbiter.ReceiveWithIterator<ProximityBoardUsbDeviceAttached>(true, _pbCommanderDataEventsPort, UsbDeviceAttached),
                    Arbiter.ReceiveWithIterator<ProximityBoardUsbDeviceDetached>(true, _pbCommanderDataEventsPort, UsbDeviceDetached),
                    Arbiter.ReceiveWithIterator<Exception>(true, _pbCommanderDataEventsPort, ExceptionHandler)),
                new ConcurrentReceiverGroup(
                    Arbiter.Receive<DsspDefaultLookup>(true, _mainPort, DefaultLookupHandler),
                    Arbiter.ReceiveWithIterator<Subscribe>(true, _mainPort, SubscribeHandler),
                    Arbiter.Receive<Get>(true, _mainPort, GetHandler),
                    Arbiter.Receive<HttpGet>(true, _mainPort, HttpGetHandler),
                    Arbiter.Receive<Reset>(true, _mainPort, ResetHandler))
                )
            );

            DirectoryInsert();

            //base.Start();
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::SubscribeHandler() - subscribe request from '" + subscribe.Body.Subscriber + "' for " + subscribe.Body.TypeFilter.Length + " types.");

            foreach (string tf in subscribe.Body.TypeFilter)
            {
                Tracer.Trace("  =========== subscribe requested for type: " + tf);
            }

            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);

            yield break; 
        }

        #region Proximity Board initialization and board maintenance

        /// <summary>
        /// Start conversation with the TrackRoamer Proximity Board device.
        /// </summary>
        IEnumerator<ITask> StartProximityBoard(int timeout)
        {
            Tracer.Trace(string.Format("TrackRoamerBrickProximityBoardService::StartProximityBoard() timeout={0} ms", timeout));

            _state.LinkState = "Initializing";

            if (timeout > 0)
            {
                //
                // caller asked us to wait <timeout> milliseconds until we start.
                //

                yield return Arbiter.Receive(false, TimeoutPort(timeout),
                    delegate(DateTime dt)
                    {
                        LogInfo(string.Format("StartProximityBoard() - Done Waiting {0} ms", timeout));
                    }
                );
            }

            if (_pbCommanderTaskQueue == null)
            {
                //
                // The internal services run on their own dispatcher, we need to create that (once)
                //

                AllocateExecutionResource allocExecRes = new AllocateExecutionResource(0, "TrackRoamerProximityBoard");

                ResourceManagerPort.Post(allocExecRes);

                yield return Arbiter.Choice(
                    allocExecRes.Result,
                    delegate(ExecutionAllocationResult result)
                    {
                        _pbCommanderTaskQueue = result.TaskQueue;
                    },
                    delegate(Exception e)
                    {
                        LogError(e);
                    }
                );
            }

            _pbCommander = new ProximityBoardCcrServiceCommander(_pbCommanderTaskQueue ?? TaskQueue, _pbCommanderDataEventsPort, _state.VendorId, _state.ProductId);
            _pbCommander.Parent = ServiceInfo.Service;
            _pbCommander.Console = ConsoleOutputPort;
            _state.IsConnected = false;

            // Open is an empty operatiion, but we need to flush the internal queue - so let it be.

            FlushPortSet(_pbCommanderDataEventsPort);

            bool failed = false;

            yield return (
                Arbiter.Choice(
                    _pbCommander.Open(),
                    delegate(SuccessResult success)
                    {
                        _state.LinkState = "Initializing - link opened";

                        LogInfo("Opened link to Proximity Board");
                    },
                    delegate(Exception exception)
                    {
                        _state.LinkState = "Error Initializing - could not open link";

                        failed = true;
                        LogError(exception);
                    }
                )
            );

            if (failed)
            {
                yield break;
            }

            //
            // Set the servo sweep rate:
            //

            yield return (
                Arbiter.Choice(
                    _pbCommander.SetServoSweepRate(),
                    delegate(SuccessResult success)
                    {
                        _state.LinkState = "Servo Sweep Rate Set";
                        LogInfo(_state.LinkState);
                    },
                    delegate(Exception exception)
                    {
                        _state.LinkState = "Error Initializing - could not set Servo Sweep Rate";
                        failed = true;
                        LogError(exception);
                    }
                )
            );

            if (failed)
            {
                yield break;
            }

            //
            // start continuous measurements.
            //

            yield return (
                Arbiter.Choice(
                    _pbCommander.SetContinuous(),
                    delegate(SuccessResult success)
                    {
                        _state.LinkState = "Started Continuous Measurement";
                        _state.IsConnected = true;
                        LogInfo(_state.LinkState);
                    },
                    delegate(Exception failure)
                    {
                        _state.LinkState = "Error Initializing - could not start Continuous Measurement";
                        _pbCommanderDataEventsPort.Post(failure);
                        failed = true;
                    }
                )
            );

            if (failed)
            {
                yield break;
            }

            // somehow the board skips commands on startup, send it twice after a wait:

            yield return Arbiter.Receive(false, TimeoutPort(1000),
                delegate(DateTime dt)
                {
                }
            );

            //
            // start continuous measurements - try 2.
            //

            yield return (
                Arbiter.Choice(
                    _pbCommander.SetContinuous(),
                    delegate(SuccessResult success)
                    {
                        _state.LinkState = "Started Continuous Measurement";
                        _state.IsConnected = true;
                        LogInfo(_state.LinkState);
                    },
                    delegate(Exception failure)
                    {
                        _state.LinkState = "Error Initializing - could not start Continuous Measurement";
                        _pbCommanderDataEventsPort.Post(failure);
                        failed = true;
                    }
                )
            );

            if (failed)
            {
                yield break;
            }

        }

        private void FlushPortSet(IPortSet portSet)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::FlushPortSet()");

            // retrieve and discard all messages from all ports in a portset
            foreach (IPortReceive port in portSet.Ports)
            {
                while (port.Test() != null)
                    ;
            }
        }

        IEnumerator<ITask> UsbDeviceDetached(ProximityBoardUsbDeviceDetached linkDetached)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::UsbDeviceDetached(): " + linkDetached.Description);

            _submgrPort.Post(new submgr.Submit(new ResetType(), DsspActions.SubmitRequest));

            _state.Description = linkDetached.Description;
            _state.LinkState = "USB Device Detached - closing link to Proximity Board";
            _state.IsConnected = false;

            LogInfo("Closing link to Proximity Board");

            yield return
                Arbiter.Choice(
                    _pbCommander.Close(),
                    delegate(SuccessResult success)
                    {
                        _state.LinkState = "USB Device Detached - link to Proximity Board closed";
                    },
                    delegate(Exception except)
                    {
                        _state.LinkState = "USB Device Detached - Error closing link to Proximity Board";
                        LogError(except);
                    }
                );

            _state.LinkState = "USB Device Detached - Proximity Board link closed, waiting 5 seconds";
            LogInfo(_state.LinkState);
            _pbCommander = null;

            SpawnIterator(openDelayMs, StartProximityBoard);

            yield break;
        }

        IEnumerator<ITask> UsbDeviceAttached(ProximityBoardUsbDeviceAttached linkAttached)
        {
            bool failed = false;

            Tracer.Trace("TrackRoamerBrickProximityBoardService::UsbDeviceAttached(): " + linkAttached.Description);

            _state.Description = linkAttached.Description;
            _state.LinkState = "USB Device Attached: " + linkAttached.Description;
            LogInfo(_state.LinkState);

            //
            // the device has powered on. Find the HID.
            //

            yield return Arbiter.Choice(
                _pbCommander.TryFindTheHid(),
                delegate(SuccessResult success)
                {
                    _state.LinkState = "USB Device Attached - Proximity Module HID Found";
                    LogInfo(_state.LinkState);
                },
                delegate(Exception failure)
                {
                    _state.LinkState = "USB Device Attached - Error looking for Proximity Module HID";
                    _pbCommanderDataEventsPort.Post(failure);
                    failed = true;
                }
            );

            if (failed)
            {
                yield break;
            }

            //
            // Set the servo sweep rate:
            //

            yield return Arbiter.Choice(
                _pbCommander.SetServoSweepRate(),
                delegate(SuccessResult success)
                {
                    _state.LinkState = "USB Device Attached - Servo Sweep Rate Set";
                    LogInfo(_state.LinkState);
                },
                delegate(Exception failure)
                {
                    _state.LinkState = "USB Device Attached - Error - could not set Servo Sweep Rate";
                    _pbCommanderDataEventsPort.Post(failure);
                    failed = true;
                }
            );

            if (failed)
            {
                yield break;
            }

            //
            // start continuous measurements.
            //

            yield return Arbiter.Choice(
                _pbCommander.SetContinuous(),
                delegate(SuccessResult success)
                {
                    _state.LinkState = "USB Device Attached - started Continuous Measurement";
                    _state.IsConnected = true;
                    LogInfo(_state.LinkState);
                },
                delegate(Exception failure)
                {
                    _state.LinkState = "USB Device Attached - Error starting Continuous Measurement";
                    _pbCommanderDataEventsPort.Post(failure);
                    failed = true;
                }
            );

            /*
            if (failed)
            {
                yield break;
            }

            //
            // wait for confirm message that signals that the Proximity Board is now in continuous measurement mode.
            //

            yield return Arbiter.Choice(
                Arbiter.Receive<ProximityBoardConfirm>(false, _internalPort,
                    delegate(ProximityBoardConfirm confirm)
                    {
                        // received Confirm
                    }),
                Arbiter.Receive<DateTime>(false, TimeoutPort(10000),
                    delegate(DateTime time)
                    {
                        _internalPort.Post(new TimeoutException("Timeout waiting for ProximityBoardConfirm after setting continuous measurement mode"));
                    })
            );
            */

            yield break;
        }

        #endregion

        #region Measurement events handlers
        /// <summary>
        /// Handle new measurement data from the Proximity Board.
        /// </summary>
        /// <param name="measurement">Measurement Data</param>
        void SonarMeasurementHandler(SonarData measurement)
        {
            //Tracer.Trace("TrackRoamerBrickProximityBoardService::SonarMeasurementHandler()");

            try
            {
                _state.LastSampleTimestamp = new DateTime(measurement.TimeStamp);
                _state.MostRecentSonar = new SonarDataDssSerializable(measurement);
                _state.LinkState = "receiving Sonar Data";

                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

                UpdateSonarData usd = new UpdateSonarData();
                usd.Body = _state.MostRecentSonar;

                //Tracer.Trace(" ========= sending UpdateSonarData notification ==================");

                base.SendNotification(_submgrPort, usd);
            }
            catch (Exception e)
            {
                _state.LinkState = "Error while receiving Sonar Data";
                LogError(e);
            }
        }

        void DirectionMeasurementHandler(DirectionData measurement)
        {
            //Tracer.Trace("TrackRoamerBrickProximityBoardService::DirectionMeasurementHandler()");

            try
            {
                _state.LastSampleTimestamp = new DateTime(measurement.TimeStamp);
                _state.MostRecentDirection = new DirectionDataDssSerializable(measurement);
                _state.LinkState = "receiving Direction Data";

                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

                UpdateDirectionData usd = new UpdateDirectionData();
                usd.Body = _state.MostRecentDirection;

                base.SendNotification<UpdateDirectionData>(_submgrPort, usd);
            }
            catch (Exception e)
            {
                _state.LinkState = "Error while receiving Direction Data";
                LogError(e);
            }
        }

        void AccelerometerMeasurementHandler(AccelerometerData measurement)
        {
            //Tracer.Trace("TrackRoamerBrickProximityBoardService::AccelerometerMeasurementHandler()");

            try
            {
                _state.LastSampleTimestamp = new DateTime(measurement.TimeStamp);
                _state.MostRecentAccelerometer = new AccelerometerDataDssSerializable(measurement);
                _state.LinkState = "receiving Accelerometer Data";

                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

                UpdateAccelerometerData usd = new UpdateAccelerometerData();
                usd.Body = _state.MostRecentAccelerometer;

                base.SendNotification<UpdateAccelerometerData>(_submgrPort, usd);
            }
            catch (Exception e)
            {
                _state.LinkState = "Error while receiving Accelerometer Data";
                LogError(e);
            }
        }

        void ProximityMeasurementHandler(ProximityData measurement)
        {
            //Tracer.Trace("TrackRoamerBrickProximityBoardService::ProximityMeasurementHandler()");

            try
            {
                _state.LastSampleTimestamp = new DateTime(measurement.TimeStamp);
                _state.MostRecentProximity = new ProximityDataDssSerializable(measurement);
                _state.LinkState = "receiving Proximity Data";

                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

                UpdateProximityData usd = new UpdateProximityData();
                usd.Body = _state.MostRecentProximity;

                base.SendNotification<UpdateProximityData>(_submgrPort, usd);
            }
            catch (Exception e)
            {
                _state.LinkState = "Error while receiving Proximity Data";
                LogError(e);
            }
        }

        void ParkingSensorMeasurementHandler(ParkingSensorData measurement)
        {
            //Tracer.Trace("TrackRoamerBrickProximityBoardService::ParkingSensorMeasurementHandler()");

            try
            {
                _state.LastSampleTimestamp = new DateTime(measurement.TimeStamp);
                _state.MostRecentParkingSensor = new ParkingSensorDataDssSerializable(measurement);
                _state.LinkState = "receiving Parking Sensor Data";

                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

                UpdateParkingSensorData usd = new UpdateParkingSensorData();
                usd.Body = _state.MostRecentParkingSensor;

                base.SendNotification<UpdateParkingSensorData>(_submgrPort, usd);
            }
            catch (Exception e)
            {
                _state.LinkState = "Error while receiving Parking Sensor Data";
                LogError(e);
            }
        }

        void AnalogMeasurementHandler(AnalogData measurement)
        {
            //Tracer.Trace("TrackRoamerBrickProximityBoardService::PotMeasurementHandler() analogValue1=" + measurement.analogValue1);

            try
            {
                _state.MostRecentAnalogData = new AnalogDataDssSerializable() { analogValue1 = measurement.analogValue1, TimeStamp = new DateTime(measurement.TimeStamp) };
                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

                UpdateAnalogData usd = new UpdateAnalogData();
                usd.Body = _state.MostRecentAnalogData;

                base.SendNotification<UpdateAnalogData>(_submgrPort, usd);
            }
            catch (Exception e)
            {
                _state.LinkState = "Error while receiving POT Data";
                LogError(e);
            }
        }

        IEnumerator<ITask> ExceptionHandler(Exception exception)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::ExceptionHandler() exception=" + exception);

            LogError(exception);

            //BadPacketException bpe = exception as BadPacketException;

            //if (bpe != null && bpe.Count < 2)
            //{
            //    yield break;
            //}

            _submgrPort.Post(new submgr.Submit(new ResetType(), DsspActions.SubmitRequest));

            _state.LinkState = "Exception " + exception.Message + " - Closing link to Proximity Board";
            _state.IsConnected = false;

            LogInfo("Closing link to Proximity Board");
            yield return
                Arbiter.Choice(
                    _pbCommander.Close(),
                    delegate(SuccessResult success)
                    {
                    },
                    delegate(Exception except)
                    {
                        LogError(except);
                    }
                );

            _state.LinkState = "Exception " + exception.Message + " - Proximity Board link closed, waiting 5 seconds to reopen";
            LogInfo(_state.LinkState);
            _pbCommander = null;

            SpawnIterator(openDelayMs, StartProximityBoard);

            yield break;
        }

        #endregion

        #region DSSP operation handlers

        void GetHandler(Get get)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::GetHandler()");

            get.ResponsePort.Post(_state);
        }


        void ReplaceHandler(Replace replace)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::ReplaceHandler()");

            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
        }

        void DropHandler(DsspDefaultDrop drop)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::DropHandler()");

            _state.LinkState = "Dropping service - Closing link to Proximity Board";
            _state.IsConnected = false;

            try
            {
                if (_pbCommander != null)
                {
                    // release dispatcher queue resource
                    ResourceManagerPort.Post(new FreeExecutionResource(_pbCommander.TaskQueue));
                    _pbCommander.Close();
                    _pbCommander = null;
                    _state.IsConnected = false;
                }
            }
            finally
            {
                base.DefaultDropHandler(drop);
            }
        }

        void ResetHandler(Reset reset)
        {
            Tracer.Trace("TrackRoamerBrickProximityBoardService::ResetHandler()");

            _pbCommanderDataEventsPort.Post(new Exception("External Reset Requested"));
            reset.ResponsePort.Post(DefaultSubmitResponseType.Instance);
        }

        #endregion

        #region HttpGet Handlers

        static readonly string _root = "/trproxboard";

        static readonly string _raw1 = "raw";
        static readonly string _cylinder = "cylinder";
        static readonly string _top = "top";
        static readonly string _topw = "top/";

        // example: http://localhost:50000/trproxboard/cylinder

        // no [ServiceHandler] here because Start() has been overwritten
        private void HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;

            string path = request.Url.AbsolutePath;

            Stream image = null;
            bool isMyPath = false;

            //Tracer.Trace("GET: path='" + path + "'");

            if (path.StartsWith(_root))
            {
                path = path.Substring(_root.Length);
                isMyPath = true;
            }

            if (isMyPath)
            {
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1);
                }

                if (path == _cylinder)
                {
                    image = GenerateCylinder();
                }
                else if (path == _top)
                {
                    image = GenerateTop(600);
                }
                else if (path.StartsWith(_topw))
                {
                    int width;
                    string remain = path.Substring(_topw.Length);

                    if (int.TryParse(remain, out width))
                    {
                        image = GenerateTop(width);
                    }
                }
                else if (path == "" || path == _raw1)
                {
                    HttpResponseType rsp = new HttpResponseType(HttpStatusCode.OK, _state, base.StateTransformPath);
                    httpGet.ResponsePort.Post(rsp);
                    return;
                }
            }

            if (image != null)
            {
                SendJpeg(httpGet.Body.Context, image);
            }
            else
            {
                httpGet.ResponsePort.Post(Fault.FromCodeSubcodeReason(
                    W3C.Soap.FaultCodes.Receiver,
                    DsspFaultCodes.OperationFailed,
                    "Unable to generate Image for path '" + path + "'"));
            }
        }

        private void SendJpeg(HttpListenerContext context, Stream stream)
        {
            WriteResponseFromStream write = new WriteResponseFromStream(context, stream, MediaTypeNames.Image.Jpeg);

            _httpUtilities.Post(write);

            Activate(
                Arbiter.Choice(
                    write.ResultPort,
                    delegate(Stream res)
                    {
                        stream.Close();
                    },
                    delegate(Exception e)
                    {
                        stream.Close();
                        LogError(e);
                    }
                )
            );
        }

        #endregion

        #region Image generators for HttpGet

        private Stream GenerateCylinder()
        {
            SonarDataDssSerializable lsd = _state.MostRecentSonar;

            if (lsd == null)
            {
                return null;
            }

            int nRays = lsd.RangeMeters.Length;

            MemoryStream memory = null;
            int scalefactor = 23;       // 600 / 26 = 23.0769231  - to have width 600px when nRays = 26
            int bmpWidth = nRays * scalefactor;
            int nearRange = 300;
            int farRange = 2000;
            int bmpHeight = 100;
            int topTextHeight = 17; // leave top for the text
            Font font = new Font(FontFamily.GenericSansSerif, 10, GraphicsUnit.Pixel);

            using (Bitmap bmp = new Bitmap(bmpWidth, bmpHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.LightGray);

                    int half = (int)Math.Round(bmp.Height * 0.65d);
                    int middle = nRays / 2;
                    int rangeMax = 0;
                    int pointMax = -1;
                    Dictionary<int, string> labels = new Dictionary<int, string>();

                    for (int i=0; i < nRays ;i++)
                    {
                        int range = (int)(lsd.RangeMeters[i] * 1000.0d);

                        int x = i * scalefactor;

                        if (i == middle)
                        {
                            g.DrawLine(Pens.Gray, x, topTextHeight, x, bmpHeight);
                        }
                        if (range > 0 && range < 8192)
                        {
                            if (range > rangeMax)
                            {
                                rangeMax = range;
                                pointMax = x;
                            }

                            int h = bmp.Height * 300 / range;
                            if (h < 0)
                            {
                                h = 0;
                            }
                            Color col = ColorHelper.LinearColor(Color.OrangeRed, Color.LightGreen, nearRange, farRange, range);
                            //Color col = ColorHelper.LinearColor(Color.DarkBlue, Color.LightGray, 0, 8192, range);
                            g.DrawLine(new Pen(col, (float)scalefactor), bmp.Width - x, Math.Max(topTextHeight, half - h), bmp.Width - x, Math.Min(bmpHeight, half + h));
                            if (i > 0 && i % 2 == 0 && i < nRays - 1)
                            {
                                double roundRange = Math.Round(range / 1000.0d, 1); // meters
                                string str = "" + roundRange;
                                labels.Add(x, str);
                            }
                        }
                    }
                    foreach (int x in labels.Keys)
                    {
                        // draw labels (distance of every second ray in meters):
                        string str = labels[x];
                        g.DrawString(str, font, Brushes.Black, bmp.Width - x - 8, (int)(bmpHeight - topTextHeight * 2 + Math.Abs((double)middle - x / scalefactor) * 20 / middle) - 30);
                    }
                    if (pointMax > 0)
                    {
                        // draw a vertical green line where the distance reaches its max value:
                        double roundRangeMax = Math.Round(rangeMax / 1000.0d, 1); // meters
                        int lineWidth = 4;
                        g.DrawLine(new Pen(Color.DarkGreen, (float)lineWidth), bmp.Width - pointMax - lineWidth/2, half, bmp.Width - pointMax - lineWidth/2, bmpHeight);
                        g.DrawString("" + roundRangeMax + "m", font, Brushes.DarkGreen, bmp.Width - pointMax, bmpHeight - topTextHeight);
                    }

                    g.DrawString(
                        lsd.TimeStamp.ToString() + "       max: " + rangeMax + " red: <" + nearRange + " green: >" + farRange + " mm",
                        font, Brushes.Black, 0, 0
                    );
                }

                memory = new MemoryStream();
                bmp.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;
            }

            return memory;
        }

        internal class Lbl
        {
            public float lx = 0;
            public float ly = 0;
            public string label = "";
        }

        private Stream GenerateTop(int imageWidth)
        {
            SonarDataDssSerializable lsd = _state.MostRecentSonar;

            if (lsd == null)
            {
                return null;
            }

            int nRays = lsd.RangeMeters.Length;

            MemoryStream memory = null;
            Font font = new Font(FontFamily.GenericSansSerif, 10, GraphicsUnit.Pixel);

            // Ultrasonic sensor reaches to about 3.5 meters; we scale the height of our display to this range:
            double maxExpectedRange = 5000.0d;  // mm

            int imageHeight = imageWidth / 2;
            int extraHeight = 100;              // shows state of proximity sensors

            using (Bitmap bmp = new Bitmap(imageWidth, imageHeight + extraHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.LightGray);

                    double angularOffset = -90 + 180.0d / 2.0d;
                    double piBy180 = Math.PI / 180.0d;
                    double halfAngle = 1.0d / 2.0d;
                    double scale = imageHeight / maxExpectedRange;
                    double drangeMax = 0.0d;

                    GraphicsPath path = new GraphicsPath();

                    Dictionary<int, Lbl> labels = new Dictionary<int, Lbl>();

                    for (int pass = 0; pass != 2; pass++)
                    {
                        for (int i = 0; i < nRays; i++)
                        {
                            int range = (int)(lsd.RangeMeters[i] * 1000.0d);

                            if (range > 0 && range < 8192)
                            {
                                double angle = i * _state.AngularResolution - angularOffset;
                                double lowAngle = (angle - halfAngle) * piBy180;
                                double highAngle = (angle + halfAngle) * piBy180;

                                double drange = range * scale;

                                float lx = (float)(imageHeight + drange * Math.Cos(lowAngle));
                                float ly = (float)(imageHeight - drange * Math.Sin(lowAngle));
                                float hx = (float)(imageHeight + drange * Math.Cos(highAngle));
                                float hy = (float)(imageHeight - drange * Math.Sin(highAngle));

                                if (pass == 0)
                                {
                                    if (i == 0)
                                    {
                                        path.AddLine(imageHeight, imageHeight, lx, ly);
                                    }
                                    path.AddLine(lx, ly, hx, hy);

                                    drangeMax = Math.Max(drangeMax, drange);
                                }
                                else
                                {
                                    g.DrawLine(Pens.DarkBlue, lx, ly, hx, hy);

                                    if (i > 0 && i % 2 == 0 && i < nRays - 1)
                                    {
                                        float llx = (float)(imageHeight + drangeMax * 1.3f * Math.Cos(lowAngle));
                                        float lly = (float)(imageHeight - drangeMax * 1.3f * Math.Sin(lowAngle));
                                        double roundRange = Math.Round(range / 1000.0d, 1); // meters
                                        string str = "" + roundRange;
                                        labels.Add(i, new Lbl() { label = str, lx = llx, ly = lly });
                                    }
                                }
                            }
                        }

                        if (pass == 0)
                        {
                            g.FillPath(Brushes.White, path);
                        }
                    }

                    // now draw the robot. Trackroamer is 680 mm wide.
                    float botHalfWidth = (float)(680 / 2.0d * scale);
                    DrawHelper.drawRobotBoundaries(g, botHalfWidth, imageWidth / 2, imageHeight);

                    g.DrawString(lsd.TimeStamp.ToString(), font, Brushes.Black, 0, 0);

                    foreach (int x in labels.Keys)
                    {
                        Lbl lbl = labels[x];
                        g.DrawString(lbl.label, font, Brushes.Black, lbl.lx, lbl.ly);
                    }

                    // draw a 200x400 image of IR proximity sensors:

                    Rectangle drawRect = new Rectangle(imageWidth/2 - extraHeight, imageHeight - extraHeight * 2, extraHeight * 2, extraHeight * 4);

                    if (_state.MostRecentProximity != null)
                    {
                        DrawHelper.drawProximityVectors(g, drawRect, _state.MostRecentProximity.arrangedForDrawing, 1);
                    }

                    if (_state.MostRecentParkingSensor != null)
                    {
                        DrawHelper.drawProximityVectors(g, drawRect, _state.MostRecentParkingSensor.arrangedForDrawing, 2);
                    }
                }

                memory = new MemoryStream();
                bmp.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;

            }
            return memory;
        }

        #endregion
    }
}


