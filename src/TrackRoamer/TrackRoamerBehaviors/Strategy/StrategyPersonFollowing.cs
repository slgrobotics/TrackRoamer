//#define DEBUG_MOVEMENTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Mime;
using W3C.Soap;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using kinect = Microsoft.Kinect;
using animhead = TrackRoamer.Robotics.Services.AnimatedHeadService.Proxy;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region StrategyPersonFollowing()

        /// <summary>
        /// how far we want to be from the target
        /// </summary>
        private const double TargetDistanceToGoalMeters = 1.8d;      // must be less than FreeDistanceMm

        private const double smallMovementsAngleTreshold = 5.0d;

        private int skelsCountPrev = 0;
        private SkeletonPose skeletonPoseLast = SkeletonPose.None;
        private DateTime lastLostTargets = DateTime.Now;
        private DateTime lastHadSkeletons = DateTime.Now;
        private DateTime lastHadRedShirt = DateTime.Now;
        private DateTime lastAmazing = DateTime.Now;
        private DateTime lastGunsFiredOnRed = DateTime.Now;
        private DateTime lastTurnedKinectPlatform = DateTime.Now;
        private DateTime lastThanksForStoppingBy = DateTime.Now;
        private DateTime lastSayState = DateTime.Now;
        private bool hadATarget = false;
        private double targetPan = 0.0d;    // relative to robot
        private double targetTilt = 0.0d;

        private int lastWaitingForHumansAnnounced = 0;
        private int lastTargetPanSwitch = 0;
        private bool haveATargetNow = false;
        private int haveATargetNowState = 0;

        private double[] panKinectSearchAngles = new double[] { 0.0d, -35.0d, -70.0d, -35.0d, 0.0d, 35.0d, 70.0d, 35.0d };
        private int panKinectSearchIndex = 0;
        private int secondsSinceLostTargetLast = -1;

        private SkeletonPose shootingPose = SkeletonPose.HandsUp;
        private bool shotAtHuman = false;
        private DateTime lastShotAtHuman = DateTime.MinValue;
        private object shootingPoseLock = new object();

        private TalkerToHuman talkerToHuman;

        #region Skeleton pose helpers

        /// <summary>
        /// converts SkeletonPose enum into spoken words
        /// </summary>
        /// <param name="pose"></param>
        /// <returns></returns>
        private string SkeletonPoseToSpokenString(SkeletonPose pose)
        {
            // split CamelCase into words:
            string sPose = pose.ToString();
            Regex upperCaseRegex = new Regex(@"[A-Z]{1}[a-z]*");
            MatchCollection matches = upperCaseRegex.Matches(sPose);
            List<string> words = new List<string>();
            foreach (Match match in matches)
            {
                words.Add(match.Value);
            }
            return string.Join(" ", words.ToArray()); 
        }

        /// <summary>
        /// when changed, announce pose and react to it. Left and right are mirror in robot view.
        /// </summary>
        /// <param name="skeletonPose"></param>
        private void ReactOnSkeletonPose(SkeletonPose skeletonPose)
        {
            if (skeletonPose != skeletonPoseLast)
            {
                skeletonPoseLast = skeletonPose;

                switch (skeletonPose)
                {
                    case SkeletonPose.NotDetected:      // we don't come here with "NotDetected"
                    case SkeletonPose.None:
                    case SkeletonPose.BothArmsForward:  // special case - growling at human
                        break;

                    default:
                        talkerToHuman.Say(9, SkeletonPoseToSpokenString(skeletonPose));
                        break;
                }


                switch (skeletonPose)
                {
                    case SkeletonPose.NotDetected:  // we don't come here with "NotDetected"
                    default:
                        StartHeadAnimationCombo(HeadComboAnimations.Restpose);
                        AddHeadAnimationCombo(HeadComboAnimations.BlinkCycle);
                        HeadlightsOff();
                        break;

                    case SkeletonPose.ArmsCrossed:
                    case SkeletonPose.LeftHandPointingRight:
                    case SkeletonPose.LeftHandUp:
                    case SkeletonPose.RightHandPointingLeft:
                    case SkeletonPose.RightHandUp:
                        StartHeadAnimationCombo(HeadComboAnimations.Blink1, true, 0.5d);
                        break;

                    case SkeletonPose.None:
                        //talkerToHuman.Say(9, "At ease");
                        HeadlightsOff();
                        StartHeadAnimationCombo(HeadComboAnimations.Restpose);
                        AddHeadAnimationCombo(HeadComboAnimations.Acknowledge);
                        break;

                    case SkeletonPose.HandsUp:
                        //talkerToHuman.Say(9, "Hands Up");
                        StartHeadAnimationCombo(HeadComboAnimations.Angry);
                        break;

                    case SkeletonPose.BothArmsForward:
                        //talkerToHuman.Say(9, "Pointing At Me");
                        talkerToHuman.GrowlAtHuman();
                        break;

                    case SkeletonPose.LeftArmForward:
                        StartHeadAnimationCombo(HeadComboAnimations.Alert_lookright);
                        break;

                    case SkeletonPose.RightArmForward:
                        StartHeadAnimationCombo(HeadComboAnimations.Alert_lookleft);
                        break;

                    // react to headlights commanding pose gestures:
                    case SkeletonPose.BothArmsOut:
                        {
                            HeadlightsOn();
                            FollowDirectionTargetDistanceToGoalMeters += 0.3d;  // hands to the sides also means back up a bit
                            StartHeadAnimationCombo(HeadComboAnimations.Angry);
                        }
                        break;

                    case SkeletonPose.LeftArmOut:
                        {
                            HeadlightsOnOff(false, true);   // right light on
                            StartHeadAnimationCombo(HeadComboAnimations.Acknowledge);
                            AddHeadAnimationCombo(HeadComboAnimations.Turn_right);
                        }
                        break;

                    case SkeletonPose.RightArmOut:
                        {
                            HeadlightsOnOff(true, false);   // left light on
                            StartHeadAnimationCombo(HeadComboAnimations.Acknowledge);
                            AddHeadAnimationCombo(HeadComboAnimations.Turn_left);
                        }
                        break;
                }
            }
        }

        #endregion // Skeleton pose helpers

        private void StrategyPersonFollowingInit()
        {
            _soundsHelper.SetSoundSkin(SoundSkinType.Bullfight);
            _mapperVicinity.robotDirection.bearing = null;

            talkerToHuman = new TalkerToHuman(_soundsHelper, this);

            StartHeadAnimationCombo(HeadComboAnimations.Restpose);
            AddHeadAnimationCombo(HeadComboAnimations.BlinkCycle, true, 0.4d);
            HeadlightsOff();
        }

        private void StrategyPersonFollowing()
        {
            haveATargetNow = false;
            bool haveSkeleton = false;
            bool lostSkeletons = false;
            DateTime Now = DateTime.Now;
            //kinect.JointType targetJointType = kinect.JointType.HandLeft;
            kinect.JointType targetJointType = kinect.JointType.Spine;

            setCurrentGoalDistance(null);     // measured value, best case is distance to skeleton, can be null if we completely lost target, or assumed to be 5 meters for red shirt.
            FollowDirectionTargetDistanceToGoalMeters = TargetDistanceToGoalMeters;  // desired value. We want to stop at this distance to human and keep him in front of the robot.

            SetLightsTrackingSkeleton(false);
            SetLightsTrackingRedShirt(false);

            if (!_mapperVicinity.robotState.ignoreKinectSkeletons)
            {
                var tmpAllSkeletons = frameProcessor.AllSkeletons;  // get a snapshot of the pointer to allocated array, and then take sweet time processing it knowing it will not change

                var skels = from s in tmpAllSkeletons
                            where s.IsSkeletonActive && s.JointPoints[targetJointType].TrackingState == kinect.JointTrackingState.Tracked
                            orderby s.JointPoints[targetJointType].Z
                            select s;

                int skelsCount = skels.Count();

                if (skelsCount != skelsCountPrev)
                {
                    int deltaSkelsCount = skelsCount - skelsCountPrev;
                    skelsCountPrev = skelsCount;

                    //if (deltaSkelsCount < 0)
                    //{
                    //    if ((Now - lastAmazing).TotalSeconds > 10.0d)
                    //    {
                    //        lastAmazing = Now;
                    //        _soundsHelper.PlaySound("you were amazing", 0.5d);
                    //    }
                    //}
                    //else
                    //{
                    //    _soundsHelper.PlaySound("skeletons number changed", 0.2d);
                    //}
                    //talkerToHuman.ensureAnnouncementDelay();

                    if (skelsCount > 0)
                    {
                        frameProcessor.doSaveOneImage = _mapperVicinity.robotState.doPhotos;  // snap a picture

                        //_mainWindow.PlayRandomSound();
                        //talkerToHuman.Say(9, "" + skelsCount + " tasty human" + (skelsCount > 1 ? "s" : ""));
                        HeadlightsOff();
                    }
                    else
                    {
                        lostSkeletons = true;
                    }
                }

                if (skelsCount > 0)
                {
                    haveSkeleton = true;

                    #region Have a skeleton, follow it

                    lastHadSkeletons = Now;

                    // found the first skeleton; track it:
                    VisualizableSkeletonInformation vsi = skels.FirstOrDefault();

                    if (vsi == null)
                    {
                        // this really, really should not happen, especially now when we allocate frameProcessor.AllSkeletons for every frame.
                        Tracer.Error("StrategyPersonFollowing() vsi == null");
                        return;
                    }

                    VisualizableJoint targetJoint = vsi.JointPoints[targetJointType];
                    //bool isSkeletonActive = vsi.IsSkeletonActive;     always true
                    SkeletonPose skeletonPose = vsi.SkeletonPose;

                    // when changed, announce pose and react to it:
                    ReactOnSkeletonPose(skeletonPose);

                    // Warning: VisualizableJoint::ComputePanTilt() can set Pan or Tilt to NaN
                    if (targetJoint != null && !double.IsNaN(targetJoint.Pan) && !double.IsNaN(targetJoint.Tilt))
                    {
                        haveATargetNow = true;

                        SetLightsTrackingSkeleton(true);

                        double targetPanRelativeToRobot = _state.currentPanKinect + targetJoint.Pan;
                        double targetPanRelativeToHead = targetJoint.Pan;

                        //Tracer.Trace("==================  currentPanKinect=" + _state.currentPanKinect + "   targetJoint.Pan=" + targetJoint.Pan + "   targetPanRelativeToRobot=" + targetPanRelativeToRobot);

                        // guns rotate (pan) with Kinect, but tilt independently of Kinect. They are calibrated when Kinect tilt = 0
                        targetPan = targetPanRelativeToHead;
                        targetTilt = targetJoint.Tilt + _state.currentTiltKinect;

                        double kinectTurnEstimate = targetPanRelativeToRobot - _state.currentPanKinect;
                        bool shouldTurnKinect = Math.Abs(kinectTurnEstimate) > smallMovementsAngleTreshold;   // don't follow small movements

                        SetDesiredKinectPlatformPan(shouldTurnKinect ? (double?)targetPanRelativeToRobot : null);      // will be processed in computeAndExecuteKinectPlatformTurn() when head turn measurement comes.

                        setPanTilt(targetPan, targetTilt);

                        double distanceToHumanMeters = targetJoint.Z;   // actual distance from Kinect to human

                        bool tooCloseToHuman = distanceToHumanMeters < TargetDistanceToGoalMeters - 0.1d;      // cannot shoot, likely backing up
                        bool veryCloseToHuman = distanceToHumanMeters < TargetDistanceToGoalMeters + 0.1d;     // can talk to human, likely in the dead zone and not moving

                        #region Greet the Human

                        if (veryCloseToHuman && talkerToHuman.canTalk())
                        {
                            frameProcessor.doSaveOneImage = _mapperVicinity.robotState.doPhotos;  // snap a picture
                            talkerToHuman.TalkToHuman();
                        }

                        #endregion // Greet the Human

                        #region Shoot the Human

                        if (skeletonPose == shootingPose)
                        {
                            if (!tooCloseToHuman)
                            {
                                //lock (shootingPoseLock)
                                //{
                                if (!shotAtHuman && (Now - lastShotAtHuman).TotalSeconds > 2.0d)
                                {
                                    lastShotAtHuman = Now;
                                    shotAtHuman = true;
                                    talkerToHuman.Say(9, "good boy");
                                    SpawnIterator(ShootGunOnce);
                                }
                                //}
                            }
                        }
                        else
                        {
                            shotAtHuman = false;
                        }

                        #endregion // Shoot the Human

                        ComputeMovingVelocity(distanceToHumanMeters, targetPanRelativeToRobot, 0.25d, 10.0d);
                    }
                    // else
                    // {
                    //      // we have skeleton(s) but the target joint is not visible. What to do here?
                    // }
                    #endregion // Have a skeleton, follow it
                }
                else if ((Now - lastHadSkeletons).TotalSeconds < 1.0d)
                {
                    return; // may be just temporary loss of skeletons, wait a little before switching to red shirt
                }
            }   // end ignoreKinectSkeletons

            if (!_mapperVicinity.robotState.ignoreRedShirt && !haveSkeleton && frameProcessor.videoSurveillanceDecider != null)
            {
                #region Have a red shirt, follow it

                VideoSurveillanceTarget target = frameProcessor.videoSurveillanceDecider.mainColorTarget;

                if (target != null && (Now - target.TimeStamp).TotalSeconds < 0.5d)     // must also be recent
                {
                    lastHadRedShirt = Now;

                    haveATargetNow = true;

                    SetLightsTrackingRedShirt(true);

                    double targetPanRelativeToRobot = target.Pan;   // already adjusted for currentPanKinect

                    //Tracer.Trace("+++++++++++++++  currentPanKinect=" + _state.currentPanKinect + "   target.Pan=" + target.Pan + "   targetPanRelativeToRobot=" + targetPanRelativeToRobot);
                    //Tracer.Trace("   target.Pan=" + target.Pan + "   Tilt=" + target.Tilt);

                    // guns rotate (pan) with Kinect, but tilt independently of Kinect. They are calibrated when Kinect tilt = 0
                    targetPan = targetPanRelativeToRobot - _state.currentPanKinect;
                    targetTilt = target.Tilt; // currentTiltKinect already accounted for by VideoSurveillance
                    //Tracer.Trace("+++++++++++++++  currentTiltKinect=" + _state.currentTiltKinect + "   target.Tilt=" + target.Tilt + "   targetTilt=" + targetTilt);

                    //if((DateTime.Now - lastTurnedKinectPlatform).TotalSeconds > 1.0d)
                    {
                        lastTurnedKinectPlatform = DateTime.Now;
                        double kinectTurnEstimate = targetPan; // targetPanRelativeToRobot - _state.currentPanKinect;
                        bool shouldTurnKinect = Math.Abs(kinectTurnEstimate) > smallMovementsAngleTreshold;   // don't follow small movements

                        SetDesiredKinectPlatformPan(shouldTurnKinect ? (double?)targetPanRelativeToRobot : null);      // will be processed in computeAndExecuteKinectPlatformTurn() when head turn measurement comes.
                    }

                    //Tracer.Trace(string.Format("   targetPan={0:0.00}        Tilt={1:0.00}        PanKinect={2:0.00}", targetPan, targetTilt, _state.currentPanKinect));

                    setPanTilt(targetPan, targetTilt);

                    double bestKinectTilt = targetTilt;   // will be limited to +-27 degrees

                    SetDesiredKinectTilt(bestKinectTilt);

                    // choose robotTacticsType - current tactics is move towards human:

                    var mostRecentParkingSensor = _state.MostRecentParkingSensor;

                    double redShirtDistanceMetersEstimated = mostRecentParkingSensor == null ? TargetDistanceToGoalMeters : Math.Min(mostRecentParkingSensor.parkingSensorMetersLF, mostRecentParkingSensor.parkingSensorMetersRF);

                    //Tracer.Trace("redShirtDistanceEstimated = " + redShirtDistanceMetersEstimated);

                    ComputeMovingVelocity(redShirtDistanceMetersEstimated, targetPanRelativeToRobot, 0.35d, 10.0d);

                    if (_mapperVicinity.robotState.robotTacticsType == RobotTacticsType.None
                        && Math.Abs(redShirtDistanceMetersEstimated - TargetDistanceToGoalMeters) < 0.35d
                        && Math.Abs(targetPan) < 10.0d
                        && (DateTime.Now - lastGunsFiredOnRed).TotalSeconds > 5.0d)
                    {
                        lastGunsFiredOnRed = Now;
                        //talkerToHuman.Say(9, "red shirt");
                        SpawnIterator(ShootGunOnce);
                    }

                    if (!hadATarget || lostSkeletons)   // just acquired target, or lost all Skeletons
                    {
                        frameProcessor.doSaveOneImage = _mapperVicinity.robotState.doPhotos;  // snap a picture

                        //talkerToHuman.Say(9, "red shirt");
                        //nextAnnouncementDelay = _soundsHelper.Announce("$lady in red", nextAnnouncementDelayDefault, 0.05d);
                        //nextAnnouncementDelay = _soundsHelper.Announce("red shirt", nextAnnouncementDelayDefault, 0.05d);

                        talkerToHuman.rewindDialogue();
                    }
                }
                else
                {
                    if (target == null)
                    {
                        Tracer.Trace("-----------------  no main color target");
                    }
                    else
                    {
                        Tracer.Trace("-----------------  main color target too old at " + (Now - target.TimeStamp).TotalSeconds + " sec");
                    }
                }

                #endregion // Have a red shirt, follow it
            } // end ignoreRedShirt
            else if ((Now - lastHadRedShirt).TotalSeconds < 1.0d)
            {
                _mapperVicinity.robotDirection.bearing = null;  // indication for tactics to compute collisions and stop.

                return; // may be just temporary loss of red shirt, wait a little before switching to sound
            }
            else if(!_mapperVicinity.robotState.ignoreKinectSounds)
            {
                // we let voice recognizer have control for several seconds, if we can't track skeleton or red shirt anyway.
                if ((Now - lastVoiceLocalized).TotalSeconds > 5.0d)
                {
                    // choose robotTacticsType - current tactics is Stop:
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                }
            }

            if (!haveATargetNow)
            {
                // no target means stopping 
                PerformAvoidCollision(null, 1.0d);  // just in case
                setCurrentGoalDistance(null);
                _mapperVicinity.robotDirection.bearing = null;  // indication for tactics to compute collisions and stop.
                _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                StopMoving();
                _state.MovingState = MovingState.Unable;
                _state.Countdown = 0;   // 0 = immediate response
            }

            if (hadATarget && !haveATargetNow)
            {
                lastLostTargets = Now;
                secondsSinceLostTargetLast = -1;
                haveATargetNowState = 0;

                if ((Now - lastThanksForStoppingBy).TotalSeconds > 60.0d)
                {
                    lastThanksForStoppingBy = Now;
                    talkerToHuman.Say(9, "thanks for stopping by!");
                }
                //talkerToHuman.Say(9, "lost all humans");
                //string messageToSay = "$lost all humans";
                //nextAnnouncementDelay = _soundsHelper.Announce(messageToSay, nextAnnouncementDelayDefault, 0.1d);

                talkerToHuman.rewindDialogue();

                lastTargetPanSwitch = 0;
                StartHeadAnimationCombo(HeadComboAnimations.Restpose, false);
                AddHeadAnimationCombo(HeadComboAnimations.BlinkCycle, true, 0.4d);
            }

            hadATarget = haveATargetNow;    // set flag for the next cycle

            #region Target Lost Routine

            if (!haveATargetNow)
            {
                if (_mapperVicinity.robotState.doLostTargetRoutine)
                {
                    // after losing targets, rotate both directions for a while, and then stop and wait:

                    int secondsSinceLostTarget = (int)Math.Round((Now - lastLostTargets).TotalSeconds);

                    if (secondsSinceLostTarget != secondsSinceLostTargetLast)
                    {
                        // we come here once every second when the target is not in view.
                        secondsSinceLostTargetLast = secondsSinceLostTarget;

                        if (secondsSinceLostTarget <= 30)
                        {
                            HeadlightsOn();

                            double tmpPanKinect = 0.0d;

                            switch (secondsSinceLostTarget)
                            {
                                case 0:
                                case 1:
                                    // stop for now:
                                    setCurrentGoalDistance(null);
                                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                                    SetDesiredKinectTilt(3.0d);
                                    return;

                                case 2:
                                case 3:
                                case 4:
                                    if (haveATargetNowState != 1)
                                    {
                                        tmpPanKinect = 50.0d * Math.Sign(targetPan);
                                        Tracer.Trace("setPanTilt()  1  Kinect pan=" + tmpPanKinect);
                                        SetDesiredKinectPlatformPan(tmpPanKinect);
                                        setGunsParked();
                                        haveATargetNowState = 1;
                                        talkerToHuman.Say(9, "One");
                                    }
                                    break;

                                case 5:
                                case 6:
                                case 7:
                                    if (haveATargetNowState != 2)
                                    {
                                        Tracer.Trace("setPanKinect()  2  Kinect pan=0");
                                        SetDesiredKinectPlatformPan(0.0d);
                                        haveATargetNowState = 2;
                                        talkerToHuman.Say(9, "Two");
                                    }
                                    break;

                                case 8:
                                case 9:
                                case 10:
                                    if (haveATargetNowState != 3)
                                    {
                                        tmpPanKinect = -50.0d * Math.Sign(targetPan);
                                        Tracer.Trace("setPanKinect()  3  Kinect pan=" + tmpPanKinect);
                                        SetDesiredKinectPlatformPan(tmpPanKinect);
                                        haveATargetNowState = 3;
                                        talkerToHuman.Say(9, "Three");
                                    }
                                    break;

                                case 11:
                                case 12:
                                    if (haveATargetNowState != 4)
                                    {
                                        Tracer.Trace("setPanKinect()  4  Kinect pan=0");
                                        SetDesiredKinectPlatformPan(0.0d);
                                        haveATargetNowState = 4;
                                        talkerToHuman.Say(9, "Four");
                                    }
                                    break;
                            }

                            if (secondsSinceLostTarget > 12 && secondsSinceLostTarget % 6 == 0 && lastTargetPanSwitch != secondsSinceLostTarget)
                            {
                                lastTargetPanSwitch = secondsSinceLostTarget;
                                targetPan = -targetPan;     // switch rotation direction every 6 seconds

                                Tracer.Trace("setPanKinect()  5  Kinect pan=0");
                                talkerToHuman.Say(9, "Switch");
                                SetDesiredKinectPlatformPan(0.0d);
                            }

                            setCurrentGoalBearingRelativeToRobot(60.0d * Math.Sign(targetPan));   // keep in the same direction where the target last was, aiming at 60 degrees for a steep turn in place

                            // choose robotTacticsType - rotate towards where the target was last seen:
                            setCurrentGoalDistance(TargetDistanceToGoalMeters);
                            FollowDirectionMaxVelocityMmSec = MinimumForwardVelocityMmSec; // ;ModerateForwardVelocityMmSec
                            _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.FollowDirection;
                        }
                        else
                        {
                            // stop, sing a song and wait for a target to appear:
                            FollowDirectionMaxVelocityMmSec = 0.0d;
                            setCurrentGoalDistance(null);
                            _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                            haveATargetNowState = 0;

                            int lonelyPlayTime = 180;     // the song is 2:40 - give it 3 minutes to play

                            if (secondsSinceLostTarget % 20 == 0 && (lastWaitingForHumansAnnounced == 0 || lastWaitingForHumansAnnounced == secondsSinceLostTarget - lonelyPlayTime))
                            {
                                lastWaitingForHumansAnnounced = secondsSinceLostTarget;
                                //talkerToHuman.Say(9, "waiting for humans");
                                _soundsHelper.Announce("$lonely", 5.0d, 0.05d);     // play "I-am-Mr-Lonely.mp3" really quietly

                                talkerToHuman.rewindDialogue();
                                lastWaitingForHumansAnnounced = 0;

                                HeadlightsOff();
                            }

                            Tracer.Trace("secondsSinceLostTarget=" + secondsSinceLostTarget);

                            if (secondsSinceLostTarget % 10 == 0)
                            {
                                Tracer.Trace("setPanKinect()  5  Kinect pan=" + panKinectSearchAngles[panKinectSearchIndex]);

                                SetDesiredKinectTilt(3.0d);
                                SetDesiredKinectPlatformPan(panKinectSearchAngles[panKinectSearchIndex++]);
                                if (panKinectSearchIndex >= panKinectSearchAngles.Length)
                                {
                                    panKinectSearchIndex = 0;
                                }
                            }
                        }
                    }
                }
                else    // !doLostTargetRoutine
                {
                    // just assume safe position and wait till a new target appears in front of the camera:
                    HeadlightsOff();
                    if ((DateTime.Now - lastLostTargets).TotalSeconds > 3.0d)
                    {
                        SafePosture();
                    }
                    // stop for now:
                    setCurrentGoalDistance(null);
                    _mapperVicinity.robotDirection.bearing = null;
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                }
            }

            #endregion // Target Lost Routine
        }

        /// <summary>
        /// compute and set all FollowDirection parameters based on polar coordinates of target
        /// </summary>
        /// <param name="distanceToHumanMeters"></param>
        /// <param name="targetPanRelativeToRobot"></param>
        /// <param name="toleranceMeters">positive</param>
        /// <param name="toleranceDegrees">positive</param>
        private void ComputeMovingVelocity(double distanceToHumanMeters, double targetPanRelativeToRobot, double toleranceMeters, double toleranceDegrees)
        {
            //Tracer.Trace("++++++ ComputeMovingVelocity()  distanceToHumanMeters=" + distanceToHumanMeters + "   targetPanRelativeToRobot=" + targetPanRelativeToRobot);
            setCurrentGoalBearingRelativeToRobot(targetPanRelativeToRobot);

            double distanceToCoverMeters = distanceToHumanMeters - TargetDistanceToGoalMeters;

            // see if we are OK with just keeping the current position or heading:
            bool positionOk = Math.Abs(distanceToCoverMeters) < toleranceMeters;
            bool headingOk = Math.Abs(targetPanRelativeToRobot) < toleranceDegrees;
            bool intendToStay = positionOk && headingOk; // do not move

            if (intendToStay)
            {
                // within the margin from target and almost pointed to it.
                // this is dead zone - we don't want to jerk around the desired distance and small angle:
                SayState("keeping - target at " + Math.Round(targetPanRelativeToRobot));
                //setCurrentGoalDistance(TargetDistanceToGoalMeters);    // let PID think we've reached the target
                _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                return;
            }

            // we need to move - maybe turn in place. See if we can move at all.
            bool canMove = PerformAvoidCollision(null, positionOk ? null : (double?)Math.Sign(distanceToCoverMeters)); // make sure CollisionState is computed if we want to move. Use velocity +-1.0 to communicate direction.

            CollisionState collisionState = _state.collisionState;

            if (positionOk && !headingOk)   // just turn
            {
                FollowDirectionMaxVelocityMmSec = MinimumForwardVelocityMmSec;
                if (targetPanRelativeToRobot > 0.0d && collisionState.canTurnRight)
                {
                    SayState("Turn right in place " + Math.Round(targetPan));
                    FollowDirectionMaxVelocityMmSec = MaximumForwardVelocityMmSec;
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.FollowDirection;
                    setCurrentGoalDistance(distanceToHumanMeters);
                    return;
                }
                else if (targetPanRelativeToRobot < 0.0d && collisionState.canTurnLeft)
                {
                   SayState("Turn left in place " + Math.Round(-targetPan));
                   FollowDirectionMaxVelocityMmSec = MaximumForwardVelocityMmSec;
                   _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.FollowDirection;
                   setCurrentGoalDistance(distanceToHumanMeters);
                   return;
                }
                else
                {
                    SayState("Turn blocked");
                    _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                   return;
                }
            }

            if (!canMove)
            {
                SayState("movement blocked");
                StopMoving();
                _state.MovingState = MovingState.Unable;
                _state.Countdown = 0;   // 0 = immediate response
                _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.None;
                return;
            }

            if (distanceToCoverMeters >= 0.0d)
            {
                SayState(collisionState.canMoveForward ? ("Forward " + Math.Round(distanceToCoverMeters, 1)) : "Forward Blocked");
                FollowDirectionMaxVelocityMmSec = Math.Min(MaximumForwardVelocityMmSec, collisionState.canMoveForwardSpeedMms);
                //Tracer.Trace("++ fwd: FollowDirectionMaxVelocityMmSec = " + FollowDirectionMaxVelocityMmSec);
                _mapperVicinity.robotState.robotTacticsType = collisionState.canMoveForward ? RobotTacticsType.FollowDirection : RobotTacticsType.None;
                setCurrentGoalDistance(distanceToHumanMeters);
            }
            else if (distanceToCoverMeters <= 0.0d)
            {
                FollowDirectionMaxVelocityMmSec = Math.Min(MaximumBackwardVelocityMmSec, collisionState.canMoveBackwardsSpeedMms);
                SayState(collisionState.canMoveBackwards ? ("Backwards " + Math.Round(distanceToCoverMeters, 1)) : "Backwards Blocked");
                //SayState(collisionState.canMoveBackwards ? ("Backwards " + Math.Round(distanceToCoverMeters, 1) + " velocity " + FollowDirectionMaxVelocityMmSec + " human at " + Math.Round(distanceToHumanMeters, 1)) : "Backwards Blocked");
                _mapperVicinity.robotState.robotTacticsType = collisionState.canMoveBackwards ? RobotTacticsType.FollowDirection : RobotTacticsType.None;
                setCurrentGoalDistance(distanceToHumanMeters);
            }

            //Tracer.Trace("FollowDirectionMaxVelocityMmSec = " + FollowDirectionMaxVelocityMmSec);
        }

        #endregion // StrategyPersonFollowing()

        [Conditional("DEBUG_MOVEMENTS")]
        private void SayState(string whatToSay)
        {
            Tracer.Trace(whatToSay);
            if ((DateTime.Now - lastSayState).TotalSeconds > 3.0d)
            {
                lastSayState = DateTime.Now;
                talkerToHuman.Say(9, whatToSay);
            }
        }
    }
}
