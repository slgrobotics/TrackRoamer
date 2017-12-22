//------------------------------------------------------------------------------
//  <copyright file="FramePreProcessor.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Microsoft.Ccr.Core;
    using Microsoft.Kinect;
    using Microsoft.Robotics;
    using ccr = Microsoft.Ccr.Core;
    using common = Microsoft.Robotics.Common;
    using kinect = Microsoft.Robotics.Services.Sensors.Kinect;
    using kinectProxy = Microsoft.Robotics.Services.Sensors.Kinect.Proxy;
    using pm = Microsoft.Robotics.PhysicalModel;

    /// <summary>
    /// Responsibility of this class is to take a raw frame and turn it into a format that can be consumed by UI w/o having to do any service calls in the process
    /// For Video - it means nothing to do (as RawFrame's planar image for RGB is directly consumable by UI)
    /// For Depth - it means turning Kinect Depth frame into a grayscale (with optional coloring of players)
    /// For Skeletons - it means pre-calculating all Joint positions and storing those in JointPoints container such that UI only needs to connect the dots
    /// </summary>
    public partial class FramePreProcessor
    {
        /// <summary>
        /// Blue byte offset in Color stream.
        /// </summary>
        private const int BlueIndex = 0;

        /// <summary>
        /// Green byte offset in Color stream.
        /// </summary>
        private const int GreenIndex = 1;

        /// <summary>
        /// Red byte offset in Color stream.
        /// </summary>
        private const int RedIndex = 2;

        /// <summary>
        /// Need to interact with main Kinect service for coordinate calculations of skeletal data
        /// </summary>
        private kinectProxy.KinectOperations kinectPort;

        private TrackRoamerBehaviorsState _state;

        private double KinectNykoGlassesFactor = 1.0d;

        // when considering skeletons, we use Head and one more joint, for which we calculate Pan/Tilt/DistanceMeters
        public JointType jointTypeOfInterest = JointType.Spine;

        /// <summary>
        /// We need to initialize Kinect port - since we'll be talking to the service
        /// </summary>
        /// <param name="kinectPort"></param>
        public FramePreProcessor(kinectProxy.KinectOperations kinectPort, TrackRoamerBehaviorsState state, bool useKinectNykoGlasses)
        {
            this.kinectPort = kinectPort;
            _state = state;

            if (useKinectNykoGlasses)
            {
                KinectNykoGlassesFactor = 1.25d;
            }
        }

        /// <summary>
        /// Cached raw frames as they were read from the Kinect Service
        /// </summary>
        public kinect.RawKinectFrames RawFrames;

        /// <summary>
        /// Processed depth image - ready to be consumed by the UI
        /// </summary>
        public byte[] DepthColordBytes;

        /// <summary>
        /// Joint coordinates that will be used by UI to draw the seletons. 7 skeleton structs are preallocated
        /// </summary>
        public List<VisualizableSkeletonInformation> AllSkeletons = null;

        private List<VisualizableSkeletonInformation> SkeletonsTemp = null;

        private void AllocateSkeletonsTemp()
        {
            SkeletonsTemp = new List<VisualizableSkeletonInformation>()
                                {
                                    new VisualizableSkeletonInformation(0),
                                    new VisualizableSkeletonInformation(1),
                                    new VisualizableSkeletonInformation(2),
                                    new VisualizableSkeletonInformation(3),
                                    new VisualizableSkeletonInformation(4),
                                    new VisualizableSkeletonInformation(5),
                                    new VisualizableSkeletonInformation(6)
                                };
        }

        /// <summary>
        /// Used to pass a point from JointToPointCoordinates() to ProcessSkeletons()
        /// </summary>
        private Point cachedJointPoint;

        /// <summary>
        /// Invoked right after a raw frame was obtained from Kinect service
        /// </summary>
        /// <param name="frames">Raw frame as recieved from Kinect sensor</param>
        public void SetRawFrame(kinect.RawKinectFrames frames)
        {
            this.RawFrames = frames;

            if (null == this.RawFrames.RawDepthFrameData)
            {
                // could be that depth frame was not requested
                return;
            }
        }

        /// <summary>
        /// Convert Kinect depth array to color bytes (grayscale with optional player coloring)
        /// </summary>
        /// <returns>Depth image array</returns>
        public byte[] GetDepthBytes()
        {
            int height = this.RawFrames.RawDepthFrameInfo.Height;
            int width = this.RawFrames.RawDepthFrameInfo.Width;

            short[] depthData = this.RawFrames.RawDepthFrameData;

            if (null == this.DepthColordBytes)
            {
                this.DepthColordBytes =
                    new byte[this.RawFrames.RawDepthFrameInfo.Height *
                        this.RawFrames.RawDepthFrameInfo.Width * 4];
            }

            int maxDistSquaredNormalized =
                ((int)KinectUI.MaxValidDepth *
                (int)KinectUI.MaxValidDepth) / 255;

            var depthIndex = 0;
            for (var y = 0; y < height; y++)
            {
                var heightOffset = y * width;

                for (var x = 0; x < width; x++)
                {
                    int index = 0;

                        index = (x + heightOffset) * 4;

                    var distance = this.GetDistanceWithPlayerIndex(depthData[depthIndex]);

                    // we square the distance to optimize for farther objects (i.e. can distingush 
                    // between objects at 2m and 2.1m quite easily
                    this.DepthColordBytes[index + BlueIndex] =
                        this.DepthColordBytes[index + GreenIndex] =
                        this.DepthColordBytes[index + RedIndex] = (byte)((distance * distance) / maxDistSquaredNormalized);

                    this.ColorPlayers(depthData[depthIndex], this.DepthColordBytes, index);

                    depthIndex++;
                }
            }

            return this.DepthColordBytes;
        }

        /// <summary>
        /// Use different colors for different players. 
        /// </summary>
        /// <param name="depthReadingToExamine">Depth reading to examine</param>
        /// <param name="colorFrame">Color image frame</param>
        /// <param name="index">Player index</param>
        internal void ColorPlayers(short depthReadingToExamine, byte[] colorFrame, int index)
        {
            int player = this.GetPlayerIndex(depthReadingToExamine);

            switch (player)
            {
                case 1:
                    colorFrame[index + GreenIndex] = 200;
                    break;
                case 2:
                    colorFrame[index + BlueIndex] = 200;
                    break;
                case 3:
                    colorFrame[index + RedIndex] = 200;
                    break;
                case 4:
                    colorFrame[index + GreenIndex] = 200;
                    colorFrame[index + RedIndex] = 200;
                    break;
                case 5:
                    colorFrame[index + BlueIndex] = 200;
                    colorFrame[index + RedIndex] = 200;
                    break;
                case 6:
                    colorFrame[index + BlueIndex] = 200;
                    colorFrame[index + GreenIndex] = 200;
                    break;
                case 7:
                    colorFrame[index + RedIndex] = 100;
                    break;
            }
        }

        /// <summary>
        /// Depth bytes to millimeter in 'PlayerIndex' format
        /// </summary>
        /// <param name="depth">Depth value to extract distance out of</param>        
        /// <returns>Distance in millimeter</returns>
        private int GetDistanceWithPlayerIndex(short depth)
        {
            int distance = (int)depth >> 3;
            return distance;
        }

        /// <summary>
        /// Self explanatory
        /// </summary>
        /// <param name="depth">Depth value to extract player index out of</param>
        /// <returns>0 = no player, 1 = 1st player, 2 = 2nd player... </returns>
        private int GetPlayerIndex(short depth)
        {
            return (int)depth & 7;
        }

        /// <summary>
        /// Convert raw skeletal structure into one we can visualize (with window coordinates) and process in Behavior.
        /// The result is a list of seven VisualizableSkeletonInformation objects, some having IsSkeletonActive=true and other properties filled.
        /// </summary>
        /// <returns>CCR Iterator</returns>
        public IEnumerator<ITask> ProcessSkeletons()
        {
            // while calculating the frame, we operate on a freshly allocated list of seven skeletons:
            AllocateSkeletonsTemp();

            int skeletonIndex = 0;
            int mainSkeletonIndex = -1;
            double minSkelDistance = double.MaxValue;

            foreach (Skeleton skel in this.RawFrames.RawSkeletonFrameData.SkeletonData)
            {
                VisualizableSkeletonInformation vsi = this.SkeletonsTemp[skeletonIndex];
                vsi.IsSkeletonActive = false;
                vsi.SkeletonQuality = string.Empty;
                vsi.SkeletonPose = SkeletonPose.None;

                // skeleton is tracked, head and jointTypeOfInterest clearly visible:
                if (SkeletonTrackingState.Tracked == skel.TrackingState
                    && skel.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked
                    && skel.Joints[jointTypeOfInterest].TrackingState == JointTrackingState.Tracked)
                {
                    vsi.IsSkeletonActive = true;
                    vsi.SkeletonQuality = skel.ClippedEdges.ToString();
                    vsi.TrackingId = skel.TrackingId;    // see http://msdn.microsoft.com/en-us/library/jj131025.aspx#Active_User_Tracking

                    if (skel.Joints[JointType.FootLeft].TrackingState == JointTrackingState.Tracked && skel.Joints[JointType.FootRight].TrackingState == JointTrackingState.Tracked)
                    {
                        vsi.SkeletonSizeMeters = skel.Joints[JointType.Head].Position.Y - (skel.Joints[JointType.FootLeft].Position.Y + skel.Joints[JointType.FootRight].Position.Y) / 2.0d + 0.08d; // plus head diameter
                    }

                    vsi.detectSkeletonPose(skel);   // fills vsi.SkeletonPose

                    // Populate joint poitns and compute Pan Tilt and DistanceMeters:
                    foreach (Joint joint in skel.Joints)
                    {
                        yield return new IterativeTask<Joint>(joint, this.JointToPointCoordinates);

                        vsi.JointPoints[joint.JointType].JointCoordinates = this.cachedJointPoint;
                        vsi.JointPoints[joint.JointType].TrackingState = joint.TrackingState;

                        VisualizableJoint vj = vsi.JointPoints[joint.JointType];

                        vj.JointCoordinates = this.cachedJointPoint;
                        vj.TrackingState = joint.TrackingState;

                        vj.X = skel.Joints[joint.JointType].Position.X / KinectNykoGlassesFactor;
                        vj.Y = skel.Joints[joint.JointType].Position.Y / KinectNykoGlassesFactor;
                        vj.Z = skel.Joints[joint.JointType].Position.Z / KinectNykoGlassesFactor;

                        vj.ComputePanTilt();

                        if (joint.JointType == jointTypeOfInterest)
                        {
                            vj.IsJointOfInterest = true;
                            vsi.DistanceMeters = vj.Z;

                            if (vsi.DistanceMeters < minSkelDistance)
                            {
                                minSkelDistance = vsi.DistanceMeters;
                                mainSkeletonIndex = skeletonIndex;
                            }
                        }
                    }

                    if (skeletonIndex < _state.HumanInteractionStates.Length)
                    {
                        HumanInteractionState his = _state.HumanInteractionStates[skeletonIndex];
                        VisualizableJoint jointOfInterest = vsi.JointPoints[jointTypeOfInterest];

                        his.IsTracked = true;
                        his.TrackingId = skel.TrackingId;
                        his.IsMain = false;
                        his.TimeStamp = DateTime.Now;
                        his.DirectionPan = jointOfInterest.Pan;
                        his.DirectionTilt = jointOfInterest.Tilt;
                        his.DistanceMeters = vsi.DistanceMeters;
                    }
                }
                else
                {
                    if (skeletonIndex < _state.HumanInteractionStates.Length)
                    {
                        HumanInteractionState his = _state.HumanInteractionStates[skeletonIndex];

                        his.IsTracked = false;
                        his.IsMain = false;
                    }
                }
                skeletonIndex++;
            }

            if (mainSkeletonIndex >= 0)
            {
                VisualizableSkeletonInformation vsi = this.SkeletonsTemp[mainSkeletonIndex];
                vsi.IsMainSkeleton = true;

                if (mainSkeletonIndex < _state.HumanInteractionStates.Length)
                {
                    HumanInteractionState his = _state.HumanInteractionStates[mainSkeletonIndex];

                    his.IsMain = true;
                }
            }

            // make the result available for outside consumption:
            AllSkeletons = SkeletonsTemp;

            yield break;
        }

        /// <summary>
        /// The skeleton data, the color image data, and the depth data are based on different 
        /// coordinate systems. To show consistent images from all three streams in the sample’s 
        /// display window, we need to convert coordinates in skeleton space to image space by 
        /// following steps
        /// </summary>
        /// <param name="joint">Joint to get coordinates for</param>
        /// <returns>CCR Iterator</returns>
        private IEnumerator<ITask> JointToPointCoordinates(Joint joint)
        {
            int colorX = 0;
            int colorY = 0;            

            kinectProxy.SkeletonToColorImageRequest request = new kinectProxy.SkeletonToColorImageRequest();
            request.SkeletonVector = joint.Position;

            yield return this.kinectPort.SkeletonToColorImage(request).Choice(
                    SkeletonToColorImageResponse =>
                    {
                        colorX = SkeletonToColorImageResponse.X;
                        colorY = SkeletonToColorImageResponse.Y;
                    },
                    failure =>
                    {
                        // high freq call - no logging
                    });

            // Clip the values
            colorX = Math.Min(colorX, KinectUI.ColorImageWidth);
            colorY = Math.Min(colorY, KinectUI.ColorImageHeight);

            // Scale the color image coordinates to the size of the skeleton display on the screen 
            this.cachedJointPoint = new Point(
                ((KinectUI.DisplayWindowWidth * colorX) / KinectUI.ColorImageWidth),
                ((KinectUI.DisplayWindowHeight * colorY) / KinectUI.ColorImageHeight));
        }
    }
}
