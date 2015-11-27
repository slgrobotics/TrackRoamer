//------------------------------------------------------------------------------
//  <copyright file="DepthCamSensorTypes.cs" company="Microsoft Corporation">
//      Copyright (C) Microsoft Corporation.  All rights reserved.
//  </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    using System.Windows;

    using kinect = Microsoft.Kinect;

    /// <summary>
    /// Joint information used in visualizing the skeletons
    /// </summary>
    public class VisualizableJoint
    {
        /// <summary>
        /// Gets or sets the tracking state. Is one of the following - Tracked, Inferred, Not Tracked
        /// </summary>
        public kinect.JointTrackingState TrackingState { get; set; }

        public bool IsJointOfInterest { get; set; }

        public kinect.JointType jointType;

        /// <summary>
        /// Visualizable coordinates of the joint, 2D in the window
        /// </summary>
        public Point JointCoordinates = new Point();

        // original 3D data for driving and targeting.
        // Kinect Z goes straight forward, X - to the left side, Y - up:
        public double X { get; set; }      // meters, relative to Kinect camera
        public double Y { get; set; }
        public double Z { get; set; }

        // Warning: ComputePanTilt() can set Pan or Tilt to NaN:
        public double Pan { get; set; }                 // relative bearing from the robot's point of view
        public double Tilt { get; set; }

        public VisualizableJoint(kinect.JointType jt)
        {
            this.TrackingState = kinect.JointTrackingState.NotTracked;
            this.jointType = jt;
            this.IsJointOfInterest = false;
        }

        /// <summary>
        /// Warning: ComputePanTilt() can set Pan or Tilt to NaN
        /// </summary>
        public void ComputePanTilt()
        {
            Pan = -toDegrees(Math.Atan2(X, Z));
            Tilt = toDegrees(Math.Atan2(Y, Z));
            //Console.WriteLine("**********************************************************************    X=" + X + "   Y=" + Y + "   Z=" + Z + "   Pan=" + Pan + "   Tilt=" + Tilt);
        }

        public double toDegrees(double radians)
        {
            return radians * 180.0d / Math.PI;
        }
    }

    /// <summary>
    /// SkeletonPose, as detected, in the order of detection priority
    /// see http://www.rit.edu/innovationcenter/kinectatrit/glossary-common-gestures
    /// </summary>
    public enum SkeletonPose
    {
        NotDetected,
        None,
        HandsUp,
        LeftHandUp,
        RightHandUp,
        BothArmsForward,
        LeftArmForward,
        RightArmForward,
        BothArmsOut,
        LeftArmOut,
        RightArmOut,
        ArmsCrossed,
        LeftHandPointingRight,
        RightHandPointingLeft
    }

    /// <summary>
    /// Joint coordinates that will be used by UI to draw the seletons
    /// </summary>
    public class VisualizableSkeletonInformation
    {
        /// <summary>
        /// Gets or sets the skeleton quality.
		/// We'll parse out Skeletal information and put it here in a 
        /// format that can be plopped into UI as is
        /// </summary>
        public string SkeletonQuality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this skeleton is tracked by Kinect
        /// </summary>
        public bool IsSkeletonActive { get; set; }

        /// <summary>
        /// Indicates the main target, usually the closest skeleton to the camera
        /// </summary>
        public bool IsMainSkeleton { get; set; }

        /// <summary>
        /// a number maintained by Kinect. See  http://msdn.microsoft.com/en-us/library/jj131025.aspx#Active_User_Tracking
        /// </summary>
        public int TrackingId { get; set; }

        /// <summary>
        /// index in FrameProcessor preallocated array
        /// </summary>
        public int Index { get; set; }

        public SkeletonPose SkeletonPose { get; set; }

        public double? SkeletonSizeMeters { get; set; }      // distance between head and feet

        public double DistanceMeters { get; set; }      // distance to jointOfInterest (usually Spine)

        /// <summary>
        /// fills this.SkeletonPose - may leave it as NotDetected or fill None
        /// </summary>
        /// <param name="skel">Skeleton</param>
        public void detectSkeletonPose(kinect.Skeleton skel)
        {
            // We come here only when skeleton and its Head is tracked. For pose detection spine and both wrists must be tracked too. 
            if (skel.Joints[kinect.JointType.WristLeft].TrackingState == kinect.JointTrackingState.Tracked
                && skel.Joints[kinect.JointType.WristRight].TrackingState == kinect.JointTrackingState.Tracked
                && skel.Joints[kinect.JointType.Spine].TrackingState == kinect.JointTrackingState.Tracked)
            {
                double sizeFactor = SkeletonSizeMeters.HasValue ? SkeletonSizeMeters.Value / 1.7d : 1.0d;   // account for persons with height different from my 1.7 meters

                // coordinates in meters:
                //Tracer.Trace("Hands: " + skel.Joints[kinect.JointType.WristLeft].Position.X + "   " + skel.Joints[kinect.JointType.WristRight].Position.X
                //     + "   -   " + skel.Joints[kinect.JointType.WristLeft].Position.Y + "   " + skel.Joints[kinect.JointType.WristRight].Position.Y
                //     + "   -   " + skel.Joints[kinect.JointType.WristLeft].Position.Z + "   " + skel.Joints[kinect.JointType.WristRight].Position.Z);

                bool leftHandUp =  skel.Joints[kinect.JointType.WristLeft].Position.Y > skel.Joints[kinect.JointType.Head].Position.Y;

                bool rightHandUp =  skel.Joints[kinect.JointType.WristRight].Position.Y > skel.Joints[kinect.JointType.Head].Position.Y;

                if (leftHandUp && rightHandUp)
                {
                    this.SkeletonPose = SkeletonPose.HandsUp;
                }
                else if (leftHandUp)
                {
                    this.SkeletonPose = SkeletonPose.LeftHandUp;
                }
                else if (rightHandUp)
                {
                    this.SkeletonPose = SkeletonPose.RightHandUp;
                }
                else
                {
                    bool leftHandForward = skel.Joints[kinect.JointType.WristLeft].Position.Z < skel.Joints[kinect.JointType.Spine].Position.Z - 0.4f * sizeFactor;

                    bool leftHandToTheSide = skel.Joints[kinect.JointType.WristLeft].Position.X < skel.Joints[kinect.JointType.Spine].Position.X - 0.6f * sizeFactor;   // meters

                    bool leftHandPointingRight = skel.Joints[kinect.JointType.WristLeft].Position.X > skel.Joints[kinect.JointType.Spine].Position.X + 0.2f * sizeFactor;   // meters


                    bool rightHandForward = skel.Joints[kinect.JointType.WristRight].Position.Z < skel.Joints[kinect.JointType.Spine].Position.Z - 0.4f * sizeFactor;

                    bool rightHandToTheSide = skel.Joints[kinect.JointType.WristRight].Position.X > skel.Joints[kinect.JointType.Spine].Position.X + 0.6f * sizeFactor;

                    bool rightHandPointingLeft = skel.Joints[kinect.JointType.WristRight].Position.X < skel.Joints[kinect.JointType.Spine].Position.X - 0.2f * sizeFactor;


                    bool HandsCrossed = skel.Joints[kinect.JointType.WristRight].Position.X < skel.Joints[kinect.JointType.WristLeft].Position.X - 0.2f * sizeFactor; // criteria looser than leftHandPointingRight && rightHandPointingLeft


                    if (leftHandForward && rightHandForward)
                    {
                        this.SkeletonPose = SkeletonPose.BothArmsForward;
                    }
                    else if (leftHandForward)
                    {
                        this.SkeletonPose = SkeletonPose.LeftArmForward;
                    }
                    else if (rightHandForward)
                    {
                        this.SkeletonPose = SkeletonPose.RightArmForward;
                    }
                    else if (leftHandToTheSide && rightHandToTheSide)
                    {
                        this.SkeletonPose = SkeletonPose.BothArmsOut;
                    }
                    else if (leftHandToTheSide)
                    {
                        this.SkeletonPose = SkeletonPose.LeftArmOut;
                    }
                    else if (rightHandToTheSide)
                    {
                        this.SkeletonPose = SkeletonPose.RightArmOut;
                    }
                    else if (HandsCrossed)
                    {
                        this.SkeletonPose = SkeletonPose.ArmsCrossed;
                    }
                    else if (leftHandPointingRight)
                    {
                        this.SkeletonPose = SkeletonPose.LeftHandPointingRight;
                    }
                    else if (rightHandPointingLeft)
                    {
                        this.SkeletonPose = SkeletonPose.RightHandPointingLeft;
                    }
                    else
                    {
                        this.SkeletonPose = SkeletonPose.None;
                    }
                }
            }
        }

        public VisualizableSkeletonInformation(int index)
        {
            Index = index;
            SkeletonPose = SkeletonPose.NotDetected;
            SkeletonSizeMeters = null;
            IsSkeletonActive = false;
            IsMainSkeleton = false;
            DistanceMeters = double.MaxValue;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}{2}{3}{4}\r\n{5:#.00}m away",
                Index,
                TrackingId,
                IsMainSkeleton ? "*" : string.Empty,
                SkeletonSizeMeters.HasValue ? string.Format("{0:#.00}m tall", SkeletonSizeMeters.Value) : string.Empty,
                this.SkeletonPose == TrackRoamerBehaviors.SkeletonPose.None ? string.Empty : (" " + this.SkeletonPose),
                DistanceMeters);
        }

        /// <summary>
        /// Pre-allocated joint points
        /// </summary>
        public Dictionary<kinect.JointType, VisualizableJoint> JointPoints = new Dictionary<kinect.JointType, VisualizableJoint>() 
        { 
            { kinect.JointType.AnkleLeft, new VisualizableJoint(kinect.JointType.AnkleLeft) },
            { kinect.JointType.AnkleRight, new VisualizableJoint(kinect.JointType.AnkleRight) },
            { kinect.JointType.ElbowLeft, new VisualizableJoint(kinect.JointType.ElbowLeft) },
            { kinect.JointType.ElbowRight, new VisualizableJoint(kinect.JointType.ElbowRight) },
            { kinect.JointType.FootLeft, new VisualizableJoint(kinect.JointType.FootLeft) },
            { kinect.JointType.FootRight, new VisualizableJoint(kinect.JointType.FootRight) },
            { kinect.JointType.HandLeft, new VisualizableJoint(kinect.JointType.HandLeft) },
            { kinect.JointType.HandRight, new VisualizableJoint(kinect.JointType.HandRight) },
            { kinect.JointType.Head, new VisualizableJoint(kinect.JointType.Head) },
            { kinect.JointType.HipCenter, new VisualizableJoint(kinect.JointType.HipCenter) },
            { kinect.JointType.HipLeft, new VisualizableJoint(kinect.JointType.HipLeft) },
            { kinect.JointType.HipRight, new VisualizableJoint(kinect.JointType.HipRight) },
            { kinect.JointType.KneeLeft, new VisualizableJoint(kinect.JointType.KneeLeft) },
            { kinect.JointType.KneeRight, new VisualizableJoint(kinect.JointType.KneeRight) },
            { kinect.JointType.ShoulderCenter, new VisualizableJoint(kinect.JointType.ShoulderCenter) },
            { kinect.JointType.ShoulderLeft, new VisualizableJoint(kinect.JointType.ShoulderLeft) },
            { kinect.JointType.ShoulderRight, new VisualizableJoint(kinect.JointType.ShoulderRight) },
            { kinect.JointType.Spine, new VisualizableJoint(kinect.JointType.Spine) },
            { kinect.JointType.WristLeft, new VisualizableJoint(kinect.JointType.WristLeft) },
            { kinect.JointType.WristRight, new VisualizableJoint(kinect.JointType.WristRight) }
        };
    }
}
