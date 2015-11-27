//-----------------------------------------------------------------------
//  This file is part of Microsoft Robotics Developer Studio Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: SickLRF.cs $ $Revision: 1 $
//-----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;


using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Services.Serializer;

using Microsoft.Dss.ServiceModel.Dssp;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using System.Net;
using System.Net.Mime;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using W3C.Soap;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    /// <summary>
    /// TrackRoamer Ultrasound Range Finder service.
    /// </summary>
    [Contract(Contract.Identifier)]
    [DisplayName("TrackRoamer Ultrasound Range Finder")]
    [Description("Provides access to a TrackRoamer Ultrasound Range Finder.")]
    [DssServiceDescription("http://msdn.microsoft.com/library/cc998493.aspx")]
    public class SickLRFService : DsspServiceBase
    {
        CommLink _link;
        LRFCommLinkPort _internalPort = new LRFCommLinkPort();

        [ServicePort("/sicklrf")]
        SickLRFOperations _mainPort = new SickLRFOperations();

        [ServiceState(StateTransform = "TrackRoamer.Robotics.Services.TrackRoamerUsrf.SickLRF.xslt")]
        [InitialStatePartner(Optional = true, ServiceUri = ServicePaths.Store + "/SickLRF.config.xml")]
        State _state = new State();

        // This is no longer used - Use base.StateTransformPath instead (see above)
        [EmbeddedResource("TrackRoamer.Robotics.Services.TrackRoamerUsrf.SickLRF.xslt")]
        string _transform = null;

        [Partner("SubMgr", Contract = submgr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.CreateAlways)]
        submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        DsspHttpUtilitiesPort _httpUtilities;

        DispatcherQueue _queue = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="creationPort">Passed to the base class for construction.</param>
        public SickLRFService(DsspServiceCreationPort creationPort) :
                base(creationPort)
        {
            Tracer.Trace("SickLRF::SickLRFService()");
        }

        /// <summary>
        /// Send phase of construction.
        /// </summary>
        protected override void Start()
        {
            if (_state == null)
            {
                _state = new State();
            }

            Tracer.Trace("SickLRF::Start()");

            LogInfo("Start");

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            //
            // Kick off the connection to the Laser Range Finder device.
            //
            SpawnIterator(0, _state.ComPort, StartLRF);

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
                    Arbiter.Receive<DsspDefaultDrop>(false, _mainPort, DropHandler)
                    ),
                new ExclusiveReceiverGroup(
                    Arbiter.Receive<Replace>(true, _mainPort, ReplaceHandler),
                    Arbiter.Receive<LinkMeasurement>(true, _internalPort, MeasurementHandler),
                    Arbiter.ReceiveWithIterator<LinkPowerOn>(true, _internalPort, PowerOn),
                    Arbiter.ReceiveWithIterator<Exception>(true, _internalPort, ExceptionHandler)),
                new ConcurrentReceiverGroup(
                    Arbiter.Receive<DsspDefaultLookup>(true, _mainPort, DefaultLookupHandler),
                    Arbiter.ReceiveWithIterator<Subscribe>(true, _mainPort, SubscribeHandler),
                    Arbiter.ReceiveWithIterator<ReliableSubscribe>(true, _mainPort, ReliableSubscribeHandler),
                    Arbiter.Receive<Get>(true, _mainPort, GetHandler),
                    Arbiter.Receive<HttpGet>(true, _mainPort, HttpGetHandler),
                    Arbiter.Receive<Reset>(true, _mainPort, ResetHandler))
                )
            );

            DirectoryInsert();
        }

        #region Initialization

        /// <summary>
        /// Start conversation with the SickLRF device.
        /// </summary>
        IEnumerator<ITask> StartLRF(int timeout, int comPort)
        {
            Tracer.Trace("SickLRF::StartLRF() comPort=" + comPort);

            if (timeout > 0)
            {
                //
                // caller asked us to wait <timeout> milliseconds until we start.
                //

                yield return Arbiter.Receive(false, TimeoutPort(timeout),
                    delegate(DateTime dt)
                    {
                        LogInfo("Done Waiting");
                    }
                );
            }

            if (_queue == null)
            {
                //
                // The internal services run on their own dispatcher, we need to create that (once)
                //

                AllocateExecutionResource allocExecRes = new AllocateExecutionResource(0, "SickLRF");

                ResourceManagerPort.Post(allocExecRes);

                yield return Arbiter.Choice(
                    allocExecRes.Result,
                    delegate(ExecutionAllocationResult result)
                    {
                        _queue = result.TaskQueue;
                    },
                    delegate(Exception e)
                    {
                        LogError(e);
                    }
                );
            }

            string comName;

            if (comPort <= 0)
            {
                //
                // We default to COM4, because
                // a) that was our previous behavior and
                // b) the hardware that we have uses COM4
                //
                comName = "COM4";
            }
            else
            {
                comName = "COM" + comPort;
            }

            _link = new CommLink(_queue ?? TaskQueue, comName, _internalPort);
            _link.Parent = ServiceInfo.Service;
            _link.Console = ConsoleOutputPort;

            FlushPortSet(_internalPort);
            yield return(
                Arbiter.Choice(
                    _link.Open(),
                    delegate(SuccessResult success)
                    {
                        LogInfo("Opened link to LRF");
                    },
                    delegate(Exception exception)
                    {
                        LogError(exception);
                    }
                )
            );
        }

        private void FlushPortSet(IPortSet portSet)
        {
            Tracer.Trace("SickLRF::FlushPortSet()");

            foreach (IPortReceive port in portSet.Ports)
            {
                while (port.Test() != null) ;
            }
        }

        IEnumerator<ITask> PowerOn(LinkPowerOn powerOn)
        {
            bool failed = false;

            Tracer.Trace("SickLRF::PowerOn()");

            _state.Description = powerOn.Description;
            _state.LinkState = "Power On: " + powerOn.Description;
            LogInfo(_state.LinkState);

            //
            // the device has powered on. Set the BaudRate to the highest supported.
            //

            yield return Arbiter.Choice(
                _link.SetDataRate(9600),
                delegate(SuccessResult success)
                {
                    _state.LinkState = "Baud Rate set to " + 9600;
                    LogInfo(_state.LinkState);
                },
                delegate(Exception failure)
                {
                    _internalPort.Post(failure);
                    failed = true;
                }
            );

            if (failed)
            {
                yield break;
            }

            Tracer.Trace("SickLRF::Opening the port");

            /*
            //
            // wait for confirm to indicate that the LRF has received the new baud rate and is
            // expecting the serial rate to change imminently.
            //

            yield return Arbiter.Choice(
                Arbiter.Receive<LinkConfirm>(false,_internalPort,
                    delegate(LinkConfirm confirm)
                    {
                        // the confirm indicates that the LRF has recieved the new baud rate
                    }),
                Arbiter.Receive<DateTime>(false, TimeoutPort(1000),
                    delegate(DateTime time)
                    {
                        _internalPort.Post(new TimeoutException("Timeout waiting for Confirm while setting data rate"));
                        failed = true;
                    })
            );

            if (failed)
            {
                yield break;
            }

            //
            // Set the serial rate to the rate requested above.
            //

            yield return Arbiter.Choice(
                _link.SetRate(),
                delegate(SuccessResult success)
                {
                    _state.LinkState = "Changed Rate to: " + _link.BaudRate;
                    LogInfo(_state.LinkState);
                },
                delegate(Exception failure)
                {
                    _internalPort.Post(failure);
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
                _link.SetContinuous(),
                delegate(SuccessResult success)
                {
                    _state.LinkState = "Starting Continuous Measurement";
                    LogInfo(_state.LinkState);
                },
                delegate(Exception failure)
                {
                    _internalPort.Post(failure);
                    failed = true;
                }
            );

            if (failed)
            {
                yield break;
            }

            //
            // wait for confirm message that signals that the LRF is now in continuous measurement mode.
            //

            yield return Arbiter.Choice(
                Arbiter.Receive<LinkConfirm>(false, _internalPort,
                    delegate(LinkConfirm confirm)
                    {
                        // received Confirm
                    }),
                Arbiter.Receive<DateTime>(false, TimeoutPort(1000),
                    delegate(DateTime time)
                    {
                        _internalPort.Post(new TimeoutException("Timeout waiting for Confirm after setting continuous measurement mode"));
                    })
            );

            yield break;
             * */
        }


        #endregion

        #region Laser Range Finder events
        /// <summary>
        /// Handle new measurement data from the LRF.
        /// </summary>
        /// <param name="measurement">Measurement Data</param>
        void MeasurementHandler(LinkMeasurement measurement)
        {
            //Tracer.Trace("SickLRF::MeasurementHandler()");

            try
            {
                //
                // The SickLRF typically reports on either a 180 degrees or 100 degrees
                // field of vision. From the number of readings we can calculate the
                // Angular Range and Resolution.
                //
                switch (measurement.Ranges.Length)
                {
                    case 181:
                        // we always get here:
                        _state.AngularRange = 180;
                        _state.AngularResolution = 1;
                        break;

                    case 361:
                        _state.AngularRange = 180;
                        _state.AngularResolution = 0.5;
                        break;

                    case 101:
                        _state.AngularRange = 100;
                        _state.AngularResolution = 1;
                        break;

                    case 201:
                        _state.AngularRange = 100;
                        _state.AngularResolution = 0.5;
                        break;

                    case 401:
                        _state.AngularRange = 100;
                        _state.AngularResolution = 0.25;
                        break;

                    default:
                        break;
                }
                _state.DistanceMeasurements = measurement.Ranges;
                _state.Units = measurement.Units;
                _state.TimeStamp = measurement.TimeStamp;
                _state.LinkState = "Measurement received";

                //
                // Inform subscribed services that the state has changed.
                //
                _subMgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));
            }
            catch (Exception e)
            {
                LogError(e);
            }

        }

        IEnumerator<ITask> ExceptionHandler(Exception exception)
        {
            Tracer.Trace("SickLRF::ExceptionHandler() exception=" + exception);

            LogError(exception);

            BadPacketException bpe = exception as BadPacketException;

            if (bpe != null && bpe.Count < 2)
            {
                yield break;
            }

            _subMgrPort.Post(new submgr.Submit(new ResetType(), DsspActions.SubmitRequest));

            LogInfo("Closing link to LRF");
            yield return
                Arbiter.Choice(
                    _link.Close(),
                    delegate(SuccessResult success)
                    {
                    },
                    delegate(Exception except)
                    {
                        LogError(except);
                    }
                );

            _state.LinkState = "LRF Link closed, waiting 5 seconds";
            LogInfo(_state.LinkState);
            _link = null;

            SpawnIterator(5000, _state.ComPort, StartLRF);

            yield break;
        }

        #endregion

        #region DSSP operation handlers

        void GetHandler(Get get)
        {
            Tracer.Trace("SickLRF::GetHandler()");

            get.ResponsePort.Post(_state);
        }


        void ReplaceHandler(Replace replace)
        {
            Tracer.Trace("SickLRF::ReplaceHandler()");

            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
        }

        IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            Tracer.Trace("SickLRF::SubscribeHandler()");

            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    if (_state != null &&
                        _state.DistanceMeasurements != null)
                    {
                        _subMgrPort.Post(new submgr.Submit(
                            subscribe.Body.Subscriber, DsspActions.ReplaceRequest, _state, null));
                    }
                },
                null
            );
        }

        IEnumerator<ITask> ReliableSubscribeHandler(ReliableSubscribe subscribe)
        {
            Tracer.Trace("SickLRF::ReliableSubscribeHandler()");

            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    if (_state != null &&
                        _state.DistanceMeasurements != null)
                    {
                        _subMgrPort.Post(new submgr.Submit(
                            subscribe.Body.Subscriber, DsspActions.ReplaceRequest, _state, null));
                    }
                },
                null
            );
        }

        void DropHandler(DsspDefaultDrop drop)
        {
            Tracer.Trace("SickLRF::DropHandler()");

            try
            {
                if (_link != null)
                {
                    // release dispatcher queue resource
                    ResourceManagerPort.Post(new FreeExecutionResource(_link.TaskQueue));
                    _link.Close();
                    _link = null;
                }
            }
            finally
            {
                base.DefaultDropHandler(drop);
            }
        }

        void ResetHandler(Reset reset)
        {
            Tracer.Trace("SickLRF::ResetHandler()");

            _internalPort.Post(new Exception("External Reset Requested"));
            reset.ResponsePort.Post(DefaultSubmitResponseType.Instance);
        }

        #endregion

        #region HttpGet Handlers

        static readonly string _root = "/sicklrf";
        static readonly string _cylinder = "/sicklrf/cylinder";
        static readonly string _top = "/sicklrf/top";
        static readonly string _topw = "/sicklrf/top/";


        void HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;

            Stream image = null;

            string path = request.Url.AbsolutePath;

            if (path == _cylinder)
            {
                image = GenerateCylinder();
            }
            else if (path == _top)
            {
                image = GenerateTop(400);
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
            else if (path == _root)
            {
                HttpResponseType rsp = new HttpResponseType(HttpStatusCode.OK,
                _state,
                //base.StateTransformPath,
                _transform);
                httpGet.ResponsePort.Post(rsp);
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
                    "Unable to generate Image"));
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

        #region Image generators

        private Stream GenerateCylinder()
        {
            if (_state.DistanceMeasurements == null)
            {
                return null;
            }

            MemoryStream memory = null;
            int scalefactor = 2;
            int bmpWidth = _state.DistanceMeasurements.Length * scalefactor;
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
                    int middle = _state.DistanceMeasurements.Length / 2;
                    int rangeMax = 0;
                    int pointMax = -1;
                    Dictionary<int, string> labels = new Dictionary<int, string>();

                    for (int i = 0; i < _state.DistanceMeasurements.Length; i++)
                    {
                        int range = _state.DistanceMeasurements[i];

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
                            Color col = LinearColor(Color.OrangeRed, Color.LightGreen, nearRange, farRange, range);
                            //Color col = LinearColor(Color.DarkBlue, Color.LightGray, 0, 8192, range);
                            g.DrawLine(new Pen(col, (float)scalefactor), bmp.Width - x, Math.Max(topTextHeight, half - h), bmp.Width - x, Math.Min(bmpHeight, half + h));
                            if (i > 0 && i % 20 == 0 && i < _state.DistanceMeasurements.Length - 10)
                            {
                                double roundRange = Math.Round(range / 1000.0d, 1); // meters
                                string str = "" + roundRange;
                                labels.Add(x, str);
                            }
                        }
                    }
                    foreach(int x in labels.Keys)
                    {
                        string str = labels[x];
                        g.DrawString(str, font, Brushes.Black, bmp.Width - x - 8, (int)(bmpHeight - topTextHeight * 2 + Math.Abs((double)middle - x/scalefactor) * 20 / middle));
                    }
                    if (pointMax > 0)
                    {
                        double roundRangeMax = Math.Round(rangeMax/1000.0d, 1); // meters
                        int shift = 3;  // account for the fact that we get chunks of approx 7 points for 26 scan stops
                        g.DrawLine(new Pen(Color.DarkGreen, (float)scalefactor), bmp.Width - pointMax - shift, half, bmp.Width - pointMax - shift, bmpHeight);
                        g.DrawString("" + roundRangeMax + "m", font, Brushes.DarkGreen, bmp.Width - pointMax, bmpHeight - topTextHeight);
                    }
                    g.DrawString(
                        _state.TimeStamp.ToString() + " max: " + rangeMax + " red: <" + nearRange + " green: >" + farRange,
                        font, Brushes.Black, 0, 0
                    );
                }

                memory = new MemoryStream();
                bmp.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;
            }

            return memory;
        }

        private Color LinearColor(Color nearColor, Color farColor, int nearLimit, int farLimit, int value)
        {
            if (value <= nearLimit)
            {
                return nearColor;
            }
            else if (value >= farLimit)
            {
                return farColor;
            }

            int span = farLimit - nearLimit;
            int pos = value - nearLimit;

            int r = (nearColor.R * (span - pos) + farColor.R * pos) / span;
            int g = (nearColor.G * (span - pos) + farColor.G * pos) / span;
            int b = (nearColor.B * (span - pos) + farColor.B * pos) / span;

            return Color.FromArgb(r, g, b);
        }

        internal class Lbl
        {
            public float lx;
            public float ly;
            public string label;
        }

        private Stream GenerateTop(int imageWidth)
        {
            if (_state.DistanceMeasurements == null)
            {
                return null;
            }

            MemoryStream memory = null;
            Font font = new Font(FontFamily.GenericSansSerif, 10, GraphicsUnit.Pixel);

            // Ultrasonic sensor reaches to about 3.5 meters; we scale the height of our display to this range:
            double maxExpectedRange = 5000.0d;  // mm

            int imageHeight = imageWidth / 2;
            using (Bitmap bmp = new Bitmap(imageWidth, imageHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.LightGray);

                    double angularOffset = -90 + _state.AngularRange / 2.0;
                    double piBy180 = Math.PI / 180.0;
                    double halfAngle = _state.AngularResolution / 2.0;
                    double scale = imageHeight / maxExpectedRange;
                    double drangeMax = 0.0d;

                    GraphicsPath path = new GraphicsPath();

                    Dictionary<int, Lbl> labels = new Dictionary<int, Lbl>();

                    for (int pass = 0; pass != 2; pass++)
                    {
                        for (int i = 0; i < _state.DistanceMeasurements.Length; i++)
                        {
                            int range = _state.DistanceMeasurements[i];
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

                                    if (i > 0 && i % 20 == 0 && i < _state.DistanceMeasurements.Length - 10)
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

                    float botWidth = (float)(190 * scale);
                    g.DrawLine(Pens.Red, imageHeight, imageHeight - botWidth, imageHeight, imageHeight);
                    g.DrawLine(Pens.Red, imageHeight - 3, imageHeight - botWidth, imageHeight + 3, imageHeight - botWidth);
                    g.DrawLine(Pens.Red, imageHeight - botWidth, imageHeight - 3, imageHeight - botWidth, imageHeight);
                    g.DrawLine(Pens.Red, imageHeight + botWidth, imageHeight - 3, imageHeight + botWidth, imageHeight);
                    g.DrawLine(Pens.Red, imageHeight - botWidth, imageHeight - 1, imageHeight + botWidth, imageHeight - 1);

                    g.DrawString(_state.TimeStamp.ToString(), font, Brushes.Black, 0, 0);

                    foreach (int x in labels.Keys)
                    {
                        Lbl lbl = labels[x];
                        g.DrawString(lbl.label, font, Brushes.Black, lbl.lx, lbl.ly);
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
