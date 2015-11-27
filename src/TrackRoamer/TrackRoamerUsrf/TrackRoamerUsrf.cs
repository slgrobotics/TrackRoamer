using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using System.Net;
using System.Net.Mime;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using submgr = Microsoft.Dss.Services.SubscriptionManager;
using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerUsrf
{
    // The ActivationSettings attribute with Sharing == false makes the runtime dedicate a dispatcher thread pool just for this service.
    // ExecutionUnitsPerDispatcher	- Indicates the number of execution units allocated to the dispatcher
    // ShareDispatcher	            - Inidicates whether multiple service instances can be pooled or not
    [ActivationSettings(ShareDispatcher = false, ExecutionUnitsPerDispatcher = 8)]
    [Contract(Contract.Identifier)]
    [DisplayName("(User) TrackRoamer Ultrasound Range Finder")]
    [Description("Provides access to a TrackRoamer Ultrasound Range Finder.")]
    [AlternateContract(sicklrf.Contract.Identifier)]
    class TrackRoamerUsrfService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        //[ServiceState(StateTransform = "Microsoft.Robotics.Services.Sensors.SickLRF.SickLRF.xslt")]
        [ServiceState(StateTransform = "TrackRoamer.Robotics.Services.TrackRoamerUsrf.TrackRoamerUsrf.xslt")]
        sicklrf.State _state = new sicklrf.State();

        bool doAveraging = true;
        bool doWeeding = true;

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/usrf", AllowMultipleInstances = false)]
        sicklrf.SickLRFOperations _mainPort = new sicklrf.SickLRFOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// TrackRoamerBrickProximityBoardService partner
        /// </summary>
        [Partner("TrackRoamerProximityBrick", Contract = proxibrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        proxibrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServicePort = new proxibrick.TrackRoamerBrickProximityBoardOperations();
        proxibrick.TrackRoamerBrickProximityBoardOperations _trackRoamerBrickProximityBoardServiceNotify = new proxibrick.TrackRoamerBrickProximityBoardOperations();

        DsspHttpUtilitiesPort _httpUtilities;

        /// <summary>
        /// Service constructor
        /// </summary>
        public TrackRoamerUsrfService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            Tracer.Trace("TrackRoamerUsrfService::TrackRoamerUsrfService()");
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            Tracer.Trace("TrackRoamerUsrfService::Start()");

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            base.Start();       // fire up MainPortInterleave; wireup [ServiceHandler] methods

            MainPortInterleave.CombineWith(new Interleave(
                new ExclusiveReceiverGroup(
                    Arbiter.ReceiveWithIterator<sicklrf.Subscribe>(true, _mainPort, SubscribeHandler),
                    Arbiter.ReceiveWithIterator<sicklrf.ReliableSubscribe>(true, _mainPort, ReliableSubscribeHandler),
                    Arbiter.Receive<sicklrf.Get>(true, _mainPort, GetHandler),
                    Arbiter.Receive<sicklrf.Reset>(true, _mainPort, ResetHandler),
                    Arbiter.Receive<proxibrick.UpdateSonarData>(true, _trackRoamerBrickProximityBoardServiceNotify, trpbUpdateSonarNotification)
                ),
                new ConcurrentReceiverGroup()
            ));

            Tracer.Trace("TrackRoamerUsrfService:  calling Subscribe() for UpdateSonarData");

            Type[] notifyMeOf = new Type[] { typeof(proxibrick.UpdateSonarData) };

            _trackRoamerBrickProximityBoardServicePort.Subscribe(_trackRoamerBrickProximityBoardServiceNotify, notifyMeOf);
        }

        /// <summary>
        /// convert sonar sweep into laser-like 180 degrees data
        /// </summary>
        /// <param name="update"></param>
        void trpbUpdateSonarNotification(proxibrick.UpdateSonarData update)
        {
            //LogInfo("TrackRoamerUsrfService: trpbUpdateNotification()");

            //Tracer.Trace("TrackRoamerUsrfService:  trpbUpdateNotification()");

            try
            {
                proxibrick.SonarDataDssSerializable  p = update.Body;

                int packetLength = p.RangeMeters.Length;      // must be 26 packets, each covering 7 degrees; 

                if (packetLength != 26)
                {
                    Tracer.Error("TrackRoamerUsrfService::trpbUpdateNotification()  not a standard measurement, angles.Count=" + packetLength + "  (expected 26) -- ignored");
                    return;
                }

                int[] intRanges = new int[packetLength];

                for(int i=0; i < packetLength ;i++)
                {
                    // range = (ushort)(i * 40);
                    ushort range = (ushort)Math.Round(p.RangeMeters[i] * 1000.0d);
                    if (range > 0x1FF7)
                    {
                        range = 0x2000; // limit to 8192 mm; actual range about 4 meters
                    }

                    intRanges[i] = (int)range;
                }

                if (doWeeding)
                {
                    if (intRanges[0] < (intRanges[1] + intRanges[2]) / 4)
                    {
                        intRanges[0] = (intRanges[1] + intRanges[2]) / 2;
                    }

                    if (intRanges[packetLength - 1] < (intRanges[packetLength - 2] + intRanges[packetLength - 3]) / 4)
                    {
                        intRanges[packetLength - 1] = (intRanges[packetLength - 2] + intRanges[packetLength - 3]) / 2;
                    }

                    for (int i = 1; i < packetLength-1; i++)
                    {
                        if (intRanges[i] < (intRanges[i - 1] + intRanges[i + 1]) * 3 / 8)
                        {
                            intRanges[i] = (intRanges[i - 1] + intRanges[i + 1]) / 2;
                        }
                    }
                }

                int angularRange = 180;
                int angularResolution = 1;

                int mesLength = angularRange + 1;       // 181

                int[] lsdRanges = new int[mesLength];      // millimeters, with 1 degree resolution

                int step = (int)Math.Round((double)mesLength / (double)packetLength);   // typically round(6.96) = 7

                // if we smooth the measurements, Decide() has better chance of sorting the values right. It does not like 7 degrees steps. 
                // we need these for exponential moving average:
                double emaPeriod = 4.0d;
                double emaMultiplier = 2.0d / (1.0d + emaPeriod);
                double? emaPrev = null;
                int iRange = 0;

                for (int i = 0; i < mesLength; i++)         // 0...181
                {
                    int angleIndex = Math.Min(i / step, packetLength-1);

                    iRange = intRanges[angleIndex];

                    if (doAveraging)
                    {
                        // calculate exponential moving average - smooth the curve a bit:
                        double? ema = !emaPrev.HasGoodValue() ? iRange : ((iRange - emaPrev) * emaMultiplier + emaPrev);
                        emaPrev = ema;
                        iRange = (int)Math.Round((double)ema);
                    }

                    //Tracer.Trace("&&&&&&&&&&&&&&&&&&&&&&&&&&&&   i=" + i + " range=" + range + " ema=" + iRange);

                    lsdRanges[i] = iRange;  // 5000; // milimeters
                }

                _state.AngularRange = angularRange;
                _state.AngularResolution = angularResolution;

                _state.DistanceMeasurements = lsdRanges;
                _state.Units = sicklrf.Units.Millimeters;
                _state.TimeStamp = p.TimeStamp;
                _state.LinkState = "Measurement received";

                //
                // Inform subscribed services that the state has changed.
                //
                _submgrPort.Post(new submgr.Submit(_state, DsspActions.ReplaceRequest));

            }
            catch (Exception exc)
            {
                LogError(exc);
            }
        }

        /// <summary>
        /// Handles Get requests
        /// </summary>
        /// <param name="get">request message</param>
        [ServiceHandler]
        public void GetHandler(sicklrf.Get get)
        {
            Tracer.Trace("TrackRoamerUsrfService::GetHandler()");

            get.ResponsePort.Post(_state);
        }

        /// <summary>
        /// Handles Replace requests
        /// </summary>
        /// <param name="replace">request message</param>
        [ServiceHandler]
        public void ReplaceHandler(sicklrf.Replace replace)
        {
            Tracer.Trace("TrackRoamerUsrfService::ReplaceHandler()");

            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
        }

        // ====================================================================================
        // as generated:
        //[ServiceHandler]
        //public void SubscribeHandler(sicklrf.Subscribe subscribe)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Handles Subscribe requests
        /// </summary>
        /// <param name="subscribe">request message</param>
        IEnumerator<ITask> SubscribeHandler(sicklrf.Subscribe subscribe)
        {
            Tracer.Trace("TrackRoamerUsrfService::SubscribeHandler()");

            yield return Arbiter.Choice(
                SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    if (_state != null &&
                        _state.DistanceMeasurements != null)
                    {
                        _submgrPort.Post(new submgr.Submit(
                            subscribe.Body.Subscriber, DsspActions.ReplaceRequest, _state, null));
                    }
                },
                null
            );
        }

        // ====================================================================================
        // as generated:
        //[ServiceHandler]
        //public void ReliableSubscribeHandler(sicklrf.ReliableSubscribe reliablesubscribe)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Handles ReliableSubscribe requests
        /// </summary>
        /// <param name="reliablesubscribe">request message</param>
        IEnumerator<ITask> ReliableSubscribeHandler(sicklrf.ReliableSubscribe reliablesubscribe)
        {
            Tracer.Trace("TrackRoamerUsrfService::ReliableSubscribeHandler()");

            yield return Arbiter.Choice(
                SubscribeHelper(_submgrPort, reliablesubscribe.Body, reliablesubscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    if (_state != null &&
                        _state.DistanceMeasurements != null)
                    {
                        _submgrPort.Post(new submgr.Submit(
                            reliablesubscribe.Body.Subscriber, DsspActions.ReplaceRequest, _state, null));
                    }
                },
                null
            );
        }

        /// <summary>
        /// Handles Reset requests
        /// </summary>
        /// <param name="reset">request message</param>
        [ServiceHandler]
        public void ResetHandler(sicklrf.Reset reset)
        {
            Tracer.Trace("TrackRoamerUsrfService::ResetHandler()");

            // TBD: send reset request to the board partner

            reset.ResponsePort.Post(DefaultSubmitResponseType.Instance);
        }

        #region HttpGet Handlers

        static readonly string _root = "/sicklrf";
        static readonly string _root1 = "/usrf/sicklrf";
        static readonly string _root2 = "/usrf";

        static readonly string _raw1 = "raw";
        static readonly string _cylinder = "cylinder";
        static readonly string _top = "top";
        static readonly string _topw = "top/";

        // example: http://localhost:50000/usrf/top

        /// <summary>
        /// Handles HttpGet requests
        /// </summary>
        /// <param name="httpget">request message</param>
        [ServiceHandler]
        public void HttpGetHandler(Microsoft.Dss.Core.DsspHttp.HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;

            Stream image = null;
            bool isMyPath = false;

            string path = request.Url.AbsolutePath;

            //Tracer.Trace("GET: path='" + path + "'");

            if (path.StartsWith(_root))
            {
                path = path.Substring(_root.Length);
                isMyPath = true;
            }
            else if (path.StartsWith(_root1))
            {
                path = path.Substring(_root1.Length);
                isMyPath = true;
            }
            else if (path.StartsWith(_root2))
            {
                path = path.Substring(_root2.Length);
                isMyPath = true;
            }

            if (isMyPath && path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            if (path == _cylinder)
            {
                image = GenerateCylinder();
            }
            else if (path == _top)
            {
                image = GenerateTop(540);
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
            if (_state.DistanceMeasurements == null)
            {
                return null;
            }

            MemoryStream memory = null;
            int scalefactor = 3;
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
                            Color col = ColorHelper.LinearColor(Color.OrangeRed, Color.LightGreen, nearRange, farRange, range);
                            //Color col = ColorHelper.LinearColor(Color.DarkBlue, Color.LightGray, 0, 8192, range);
                            g.DrawLine(new Pen(col, (float)scalefactor), bmp.Width - x, Math.Max(topTextHeight, half - h), bmp.Width - x, Math.Min(bmpHeight, half + h));
                            if (i > 0 && i % 20 == 0 && i < _state.DistanceMeasurements.Length - 10)
                            {
                                double roundRange = Math.Round(range / 1000.0d, 1); // meters
                                string str = "" + roundRange;
                                labels.Add(x, str);
                            }
                        }
                    }
                    foreach (int x in labels.Keys)
                    {
                        string str = labels[x];
                        g.DrawString(str, font, Brushes.Black, bmp.Width - x - 8, (int)(bmpHeight - topTextHeight * 2 + Math.Abs((double)middle - x / scalefactor) * 20 / middle));
                    }
                    if (pointMax > 0)
                    {
                        // draw a vertical green line where the distance reaches its max value:
                        double roundRangeMax = Math.Round(rangeMax / 1000.0d, 1); // meters
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

                    float botHalfWidth = (float)(680 / 2.0d * scale);
                    DrawHelper.drawRobotBoundaries(g, botHalfWidth, imageWidth / 2, imageHeight);

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


