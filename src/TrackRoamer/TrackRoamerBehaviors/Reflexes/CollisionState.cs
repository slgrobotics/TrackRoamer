using System;
using System.Collections.Generic;
using System.Linq;

using System.ComponentModel;
using Microsoft.Dss.Core.Attributes;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    [DataContract]
    public class CollisionState
    {
        [DataMember, Browsable(true)]
        public bool canMoveForward;
        [DataMember, Browsable(true)]
        public int canMoveForwardDistanceMm;
        [DataMember, Browsable(true)]
        public double canMoveForwardSpeedMms;

        [DataMember, Browsable(true)]
        public bool canMoveBackwards;
        [DataMember, Browsable(true)]
        public int canMoveBackwardsDistanceMm;
        [DataMember, Browsable(true)]
        public double canMoveBackwardsSpeedMms;

        [DataMember, Browsable(true)]
        public bool canTurnRight;
        [DataMember, Browsable(true)]
        public bool canTurnLeft;

        /// <summary>
        /// will be null if plan is null or has no leg planned or the leg is outside +-45 degrees sector forward 
        /// will be false if planned leg has obstacle closer than ObstacleDistanceMm (0.6m)
        ///         true if no close obstacles on the leg
        /// </summary>
        [DataMember, Browsable(true)]
        public bool? canMoveByPlan;

        [DataMember, Browsable(true)]
        public bool mustStop;
        [DataMember, Browsable(true)]
        public string message;

        public CollisionState()
        {
            initRestrictive();
        }

        public void initRestrictive()
        {
            canMoveForward = false;
            canMoveForwardDistanceMm = 0;
            canMoveForwardSpeedMms = 0.0d;
            canMoveBackwards = false;
            canMoveBackwardsDistanceMm = 0;
            canMoveBackwardsSpeedMms = 0.0d;
            canTurnRight = false;
            canTurnLeft = false;
            canMoveByPlan = null;
            mustStop = true;
            message = string.Empty;
        }

        public void initPermissive(int freeDistanceMm, double maximumForwardVelocityMmSec, double maximumBackwardVelocityMmSec)
        {
            canMoveForward = true;
            canMoveForwardDistanceMm = freeDistanceMm;
            canMoveForwardSpeedMms = maximumForwardVelocityMmSec;
            canMoveBackwards = true;
            canMoveBackwardsDistanceMm = freeDistanceMm;
            canMoveBackwardsSpeedMms = maximumBackwardVelocityMmSec;
            canTurnRight = true;
            canTurnLeft = true;
            canMoveByPlan = null;
            mustStop = false;
            message = string.Empty;
        }
    }
}
