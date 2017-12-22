using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;
using Microsoft.Robotics.Common;
using rpm = Microsoft.Robotics.PhysicalModel;
using common = Microsoft.Robotics.Common;

using System.Windows;
using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;
using TrackRoamer.Robotics.Utility.LibPicSensors;
using libguiwpf = TrackRoamer.Robotics.LibGuiWpf;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using trdrive = TrackRoamer.Robotics.Services.TrackRoamerDrive.Proxy;
using encoder = Microsoft.Robotics.Services.Encoder.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using powerbrick = TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Proxy;
using proxibrick = TrackRoamer.Robotics.Services.TrackRoamerBrickProximityBoard.Proxy;
using gps = Microsoft.Robotics.Services.Sensors.Gps.Proxy;
using chrum6orientationsensor = TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Proxy;

using kinect = Microsoft.Robotics.Services.Sensors.Kinect;
using kinectProxy = Microsoft.Robotics.Services.Sensors.Kinect.Proxy;
using nui = Microsoft.Kinect;
using depthcam = Microsoft.Robotics.Services.DepthCamSensor;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

/*
Note: MRDS 4 Kinect displays tons of "ImageFrame not disposed" messages in DebugView. To fix that:
 * 
 * file C:\Microsoft Robotics Dev Studio 4\samples\Sensors\Kinect\Kinect\Kinect.cs needs correction, and you have to
 *  rebuild projectC:\Microsoft Robotics Dev Studio 4\samples\Sensors\Kinect\Kinect\Kinect.csproj
 *  
 * Lines 509-553 - insert Dispose where it is needed:
 * 
 *          if (kinectFrame != null)
            {
                rawFrames.RawColorFrameInfo = new KinectFrameInfo(kinectFrame);
                rawFrames.RawColorFrameData = new byte[kinectFrame.PixelDataLength];
                kinectFrame.CopyPixelDataTo(rawFrames.RawColorFrameData);
                kinectFrame.Dispose();                                           <- this is missing in 3 places - Color, Depth and Skeleton frames
            }            
........
                kinectFrame.Dispose();
........
                skeletonFrame.Dispose();
*/

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        // see C:\Microsoft Robotics Dev Studio 4\samples\Sensors\Kinect\KinectUI

        private int KinectLoopWaitIntervalMs = 500;     // time to wait in the Kinect frames sampling loop when no sampling is enabled.

        private bool useKinectNykoGlasses = true;

        /// <summary>
        /// Used to guage frequency of reading the state (which is much lower than that of reading frames)
        /// </summary>
        private double lastStateReadTime = 0;

        /// <summary>
        /// We dont want to flood logs with same errors
        /// </summary>
        private bool atLeastOneFrameQueryFailed = false;

        /// <summary>
        /// We dont want to flood logs with same errors
        /// </summary>
        private bool atLeastOneTiltPollFailed = false;

        /// <summary>
        /// Those are used to set appropariate flags when querying Kinect frame
        /// </summary>
        public bool IncludeDepth { get; set; }

        public bool IncludeVideo { get; set; }

        public bool IncludeVideoProcessed { get; set; }

        public bool IncludeSkeletons { get; set; }

        /// <summary>
        /// Main UI window to do "Invokes" on when data is ready for visualization
        /// </summary>
        private KinectUI userInterface;

        /// <summary>
        /// Kinect partner service
        /// </summary>
        [Partner("Kinect", Contract = kinectProxy.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        private kinectProxy.KinectOperations kinectPort = new kinectProxy.KinectOperations();

        private FramePreProcessor frameProcessor;

        /// <summary>
        /// Initialize Kinect UI service
        /// </summary>
        /// <returns>Iterator</returns>
        private IEnumerator<ITask> InitializeKinectUI()
        {
            this.frameProcessor = new FramePreProcessor(this.kinectPort, this._state, this.useKinectNykoGlasses);

            var runWindow = this.wpfServicePort.RunWindow(() => new KinectUI(this));
            yield return (Choice)runWindow;

            var exception = (Exception)runWindow;
            if (exception != null)
            {
                LogError(exception);
                StartFailed();
                yield break;
            }

            // need double cast because WPF adapter doesn't know about derived window types
            this.userInterface = (Window)runWindow as KinectUI;

            yield return this.kinectPort.Get().Choice(
                kinectState =>
                {
                    this.UpdateState(kinectState);
                },
                failure =>
                {
                    LogError(failure);
                });

            yield return this.kinectPort.GetDepthProperties().Choice(
                GetDepthProperties =>
                {
                    KinectUI.MaxValidDepth = GetDepthProperties.MaxDepthValue;
                },
                failure =>
                {
                    LogError(failure);
                });

            SpawnIterator(this.ReadKinectLoop);
        }

        #region ReadKinectLoop()

        /// <summary>
        /// Main read loop
        /// Read raw frame from Kinect service, then process it asynchronously, then request UI update
        /// </summary>
        /// <returns>A standard CCR iterator.</returns>
        private IEnumerator<ITask> ReadKinectLoop()
        {
            // note: see frame rate at C:\Microsoft Robotics Dev Studio 4\projects\TrackRoamer\TrackRoamerServices\Config\TrackRoamer.TrackRoamerBot.Kinect.Config.xml
            while (true)
            {
                try
                {
                    kinectProxy.QueryRawFrameRequest frameRequest = new kinectProxy.QueryRawFrameRequest();
                    frameRequest.IncludeDepth = this.IncludeDepth;
                    frameRequest.IncludeVideo = this.IncludeVideo;
                    frameRequest.IncludeSkeletons = this.IncludeSkeletons;

                    if (!this.IncludeDepth && !this.IncludeVideo && !this.IncludeSkeletons)
                    {
                        // poll 2 times a sec if user for some reason deselected all image options (this would turn into a busy loop then)
                        yield return TimeoutPort(KinectLoopWaitIntervalMs).Receive();
                    }

                    kinect.RawKinectFrames rawFrames = null;

                    // poll depth camera
                    yield return this.kinectPort.QueryRawFrame(frameRequest).Choice(
                        rawFrameResponse =>
                        {
                            rawFrames = rawFrameResponse.RawFrames;
                        },
                        failure =>
                        {
                            if (!this.atLeastOneFrameQueryFailed)
                            {
                                this.atLeastOneFrameQueryFailed = true;
                                LogError(failure);
                            }
                        });

                    this.frameProcessor.currentPanKinect = _state.currentPanKinect;
                    this.frameProcessor.currentTiltKinect = _state.currentTiltKinect;
                    this.frameProcessor.SetRawFrame(rawFrames);

                    if (null != rawFrames.RawSkeletonFrameData)
                    {
                        yield return new IterativeTask(this.frameProcessor.ProcessSkeletons);

                        var tmpAllSkeletons = frameProcessor.AllSkeletons;  // get a snapshot of the pointer to allocated array, and then take sweet time processing it knowing it will not change

                        if (tmpAllSkeletons != null)
                        {
                            // tmpAllSkeletons is a list of seven skeletons, those good enough for processing have IsSkeletonActive true
                            var skels = from s in tmpAllSkeletons
                                        where s.IsSkeletonActive
                                        select s;

                            foreach (VisualizableSkeletonInformation skel in skels)
                            {
                                // Kinect Z goes straight forward, X - to the left side, Y - up
                                double kZ = skel.JointPoints[nui.JointType.Spine].Z;     // meters, relative to Kinect camera
                                double kX = skel.JointPoints[nui.JointType.Spine].X;

                                GeoPosition pos1 = (GeoPosition)_mapperVicinity.robotPosition.Clone();

                                double relBearing = Direction.to180fromRad(Math.Atan2(-kX, kZ));
                                double rangeMeters = Math.Sqrt(kZ * kZ + kX * kX);

                                pos1.translate(new Direction() { heading = _mapperVicinity.robotDirection.heading, bearingRelative = relBearing }, new Distance(rangeMeters));

                                DetectedHuman dhum1 = new DetectedHuman()
                                {
                                    geoPosition = pos1,
                                    firstSeen = DateTime.Now.Ticks,
                                    lastSeen = DateTime.Now.Ticks,
                                    detectorType = DetectorType.KINECT_SKELETON,
                                };

                                lock (_mapperVicinity)
                                {
                                    _mapperVicinity.Add(dhum1);
                                }
                            }
                        }
                    }

                    if (null != rawFrames.RawColorFrameData)
                    {
                        yield return new IterativeTask(this.frameProcessor.ProcessImageFrame);      // RGB Video
                    }

                    if (null != rawFrames.RawDepthFrameData)
                    {
                        yield return new IterativeTask(this.frameProcessor.ProcessDepthFrame);      // Depth information frame
                    }

                    this.UpdateUI(this.frameProcessor);

                    Decide(SensorEventSource.Kinect);

                    // poll state at low frequency to see if tilt has shifted (may happen on an actual robot due to shaking)
                    if (common.Utilities.ElapsedSecondsSinceStart - this.lastStateReadTime > 1)
                    {
                        yield return this.kinectPort.Get().Choice(
                            kinectState =>
                            {
                                this.UpdateState(kinectState);  // update value displayed in WPF window
                                _state.currentTiltKinect = kinectState.TiltDegrees;
                            },
                            failure =>
                            {
                                if (!this.atLeastOneTiltPollFailed)
                                {
                                    this.atLeastOneTiltPollFailed = true;
                                    LogError(failure);
                                }
                            });

                        this.lastStateReadTime = common.Utilities.ElapsedSecondsSinceStart;
                    }
                }
                finally
                {
                }
            }
        }

        #endregion // ReadKinectLoop()

        #region Updates to WPF window

        private void UpdateUI(FramePreProcessor framePreProcessor)
        {
            this.wpfServicePort.Invoke(() => this.userInterface.DrawFrame(framePreProcessor));
        }

        private void UpdateState(kinectProxy.KinectState kinectState)
        {
            this.wpfServicePort.Invoke(() => this.userInterface.UpdateState(kinectState));
        }

        #endregion // Updates to WPF window

        #region Setting Skeleton Smoothing on Kinect device

        /// <summary>
        /// Send a request to Kinect service to update smoothing parameters
        /// </summary>
        /// <param name="transformSmooth"></param>
        /// <param name="smoothing"></param>
        /// <param name="correction"></param>
        /// <param name="prediction"></param>
        /// <param name="jitterRadius"></param>
        /// <param name="maxDeviationRadius"></param>
        internal void UpdateSkeletalSmoothing(
            bool transformSmooth,
            float smoothing,
            float correction,
            float prediction,
            float jitterRadius,
            float maxDeviationRadius)
        {
            nui.TransformSmoothParameters newSmoothParams = new nui.TransformSmoothParameters();
            newSmoothParams.Correction = correction;
            newSmoothParams.JitterRadius = jitterRadius;
            newSmoothParams.MaxDeviationRadius = maxDeviationRadius;
            newSmoothParams.Prediction = prediction;
            newSmoothParams.Smoothing = smoothing;

            kinectProxy.UpdateSkeletalSmoothingRequest request = new kinectProxy.UpdateSkeletalSmoothingRequest();
            request.TransfrormSmooth = transformSmooth;
            request.SkeletalEngineTransformSmoothParameters = newSmoothParams;

            Activate(
                Arbiter.Choice(
                    this.kinectPort.UpdateSkeletalSmoothing(request),
                    success =>
                    {
                        // nothing to do
                    },
                    fault =>
                    {
                        // the fault handler is outside the WPF dispatcher
                        // to perfom any UI related operation we need to go through the WPF adapter

                        // show an error message
                        this.wpfServicePort.Invoke(() => this.userInterface.ShowFault(fault));
                    }));
        }

        #endregion // Setting Skeleton Smoothness on Kinect device

        #region InitializeDepthProcessing

        /*
        // see  C:\Microsoft Robotics Dev Studio 4\samples\Misc\ObstacleAvoidance\ObstacleAvoidanceDrive.cs

        /// <summary>
        /// Preferred depth cam image width
        /// </summary>
        private const int DefaultDepthCamImageWidth = 320;

        /// <summary>
        /// Preferred depth cam image height
        /// </summary>
        private const int DefaultDepthcamImageHeight = 240;

        /// <summary>
        /// Pre calculated depths to floor plane
        /// </summary>
        private short[] floorCeilingMinDepths;

        /// <summary>
        /// Pre calculated depths to ceiling plane
        /// </summary>
        private short[] floorCeilingMaxDepths;

        /// <summary>
        /// Default pose for depthcam sensor
        /// </summary>
        private rpm.Pose depthCameraDefaultPose = new rpm.Pose()
        {
            Orientation = new rpm.Quaternion(0, 0, 0, 1)
        };

        /// <summary>
        /// Inverse projection matrix for viewspace to pixelspace conversions
        /// </summary>
        private rpm.Matrix inverseProjectionMatrix;

        /// <summary>
        /// Initializes depth processing variables
        /// </summary>
        /// <param name="fov">Horizontal field of view</param>
        /// <param name="depthRange">Max depth range in meters</param>
        private void InitializeDepthProcessing(double fov, double depthRange)
        {
            if (this.floorCeilingMinDepths != null)
            {
                return;
            }

            this.inverseProjectionMatrix = MathUtilities.Invert(MathUtilities.ComputeProjectionMatrix(
                (float)fov,
                DefaultDepthCamImageWidth,
                DefaultDepthcamImageHeight,
                depthRange));

            // pre calculate pixel space thresholds for floor and ceiling removal. We allow for
            // a small (10cm) margin on floor detection, and we cut-off the ceiling 2m above the camera
            DepthImageUtilities.ComputeCeilingAndFloorDepthsInPixelSpace(
                cameraPose: this.depthCameraDefaultPose,
                floorThreshold: -0.1f,
                ceilingThreshold: 2f,
                invProjectionMatrix: this.inverseProjectionMatrix,
                floorCeilingMinDepths: ref this.floorCeilingMinDepths,
                floorCeilingMaxDepths: ref this.floorCeilingMaxDepths,
                width: DefaultDepthCamImageWidth,
                height: DefaultDepthcamImageHeight,
                depthRange: (float)kinect.KinectCameraConstants.MaximumRangeMeters);
        }
        */

        #endregion // InitializeDepthProcessing
    }
}
