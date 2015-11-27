using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
#if OBSOLETE_PLANANDAVOID_LOGIC

        private const double TurningWaitInterval = 1.0d;              // wait for so many seconds after a turn before accepting a laser frame (should be close to full sweep cycle of the laser) 

        #region PlanAndAvoid tactics

        private void TacticsPlanAndAvoid()
        {
            if (_currentRoutePlan.isGoodPlan)
            {
                // we have a good plan, try executing it:
                string sTurn = "no turn";

                double? bestHeadingRelative = _currentRoutePlan.bestHeading - _mapperVicinity.robotDirection.heading;

                if (bestHeadingRelative.HasValue && bestHeadingRelative != 0.0d)
                {
                    sTurn = "turn " + (bestHeadingRelative > 0.0d ? "right " : "left ") + bestHeadingRelative + " degrees";
                }

                if (_currentRoutePlan.legMeters.HasValue)
                {
                    sPlan = "plan: " + sTurn + " and go up to " + Math.Round((double)_currentRoutePlan.legMeters, 2) + " meters (obstacle at " + Math.Round((double)_currentRoutePlan.closestObstacleAlongBestPathMeters, 2) + " meters";
                }
                else
                {
                    sPlan = "plan: steep " + sTurn + " only (obstacle at " + Math.Round((double)_currentRoutePlan.closestObstacleAlongBestPathMeters, 2) + " meters)";
                }

                Tracer.Trace(sPlan);

                // AvoidCollision and EnterOpenSpace have precedence over
                // all other state transitions and are thus handled first.
                bool canMove = PerformAvoidCollision(_currentRoutePlan);   // may result in state set to MovingState.Unable

                // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
                // we must use them while executing the plan.

                // good place to analyze the plan against constraints and set MovingState.Unable
                RoutePlan bestPlan = critiquePlanChain(_currentRoutePlan);

                if (bestPlan == null)
                {
                    _state.MovingState = MovingState.Unable;    // no good plan in the chain
                }

                if (_state.MovingState != MovingState.Unknown && _state.MovingState != MovingState.Unable)
                {
                    if (!_currentRoutePlan.legMeters.HasValue)
                    {
                        // plan requires a steep turn towards the goal:
                        TurnToGoal(_currentRoutePlan);
                    }
                    else
                    {
                        // plan suggests moving and adjusting heading and speed:
                        EnterOpenSpace(_currentRoutePlan);

                        UpdateMovingState(_currentRoutePlan);
                    }
                }
                else
                {
                    // state - Unknown or Unable; will likely start mapping
                    UpdateMovingState(_currentRoutePlan);
                }
            }
            else
            {
                // the plan is no good. Just avoid collision for now:
                Tracer.Trace(sPlan);
                bool canMove = PerformAvoidCollision(null);   // may result in state set to MovingState.Unable (if mustStop)
            }
        }

        #endregion // PlanAndAvoid tactics

        #region Decision Making Elements

        /// <summary>
        /// just turn where the plan wants you to; we are calling with good plan (heading) and no legMeters. 
        /// </summary>
        /// <param name="plan"></param>
        private void TurnToGoal(RoutePlan plan)
        {
            // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            // we must use them while executing the plan.

            _state.NewHeading = (int)Math.Round((double)plan.bestHeadingRelative(_mapperVicinity));

            setMovingStateDetail("TurnToGoal to NewHeading=" + _state.NewHeading);

            AdjustHeading();
        }

        /// <summary>
        /// If the robot is mapping and there is sufficient open space directly ahead, enter this space.
        /// </summary>
        /// <param name="distance"></param>
        private void EnterOpenSpace(RoutePlan plan)
        {
            if (_state.IsMapping && plan != null && plan.isGoodPlan)
            {
                // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
                // we must use them while executing the plan.

                int? distance = (int)((double)plan.closestObstacleAlongBestPathMeters * 1000.0d);

                if (distance > SafeDistanceMm)
                {
                    // We are mapping but can see plenty of free space ahead.
                    // The robot should go into this space.

                    LogInfo("TrackRoamerBehaviorsService: EnterOpenSpace() - while mapping, distance greater than SafeDistance=" + SafeDistanceMm);

                    string message = "open space."; // "while mapping - detected open space; entering...";

                    Talker.Say(5, message);
                    LogHistory(5, message);
                    setMovingStateDetail(message);

                    StopTurning();
                    _state.MovingState = MovingState.FreeForwards;
                    _state.Countdown = 4;
                }
            }
        }

        /// <summary>
        /// Transitions to the most appropriate state.
        /// </summary>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void UpdateMovingState(RoutePlan plan)
        {
            LogInfo("TrackRoamerBehaviorsService:UpdateMovingState()   Countdown=" + _state.Countdown + "   MovingState=" + _state.MovingState);

            // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            // we must use them while executing the plan.

            //if (_state.Countdown > 0)
            //{
            //    _state.Countdown--;
            //    //_state.Countdown = 0;
            //}
            //else

            // based on MetaState, go to the MetaState...() method - which may change the MovingState appropriately
            if (_state.IsUnknown || _state.MovingState == MovingState.Unable)
            {
                MetaStateStartMapping(plan);
            }
            else if (_state.IsMoving)
            {
                MetaStateMove(plan);
            }
            else if (_state.IsMapping)
            {
                MetaStateMap(plan);
            }
        }

        /// <summary>
        /// Implements the "Moving" meta state, basically any transition and not in-place sequence.
        /// </summary>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void MetaStateMove(RoutePlan plan)
        {
            LogInfo("TrackRoamerBehaviorsService: MetaStateMove()");

            Talker.Say(4, "Move - " + _state.MovingState);

            // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            // we must use them while executing the plan.

            switch (_state.MovingState)
            {
                case MovingState.AdjustHeading:
                    setMovingStateDetail("MetaStateMove - AdjustHeading to NewHeading=" + _state.NewHeading);
                    AdjustHeading();
                    break;
                case MovingState.FreeForwards:
                    setMovingStateDetail("MetaStateMove - FreeForwards");
                    AdjustDirectionAndVelocity(plan);
                    break;
                case MovingState.InTransition:
                case MovingState.Unknown:
                    // inTransition means "while executing TurnAndMoveForward() command" (see base).
                    // TurnAndMoveForward() may for a moment leave the state in "Unknown", do not treat it as termination.
                    // this allows current transition to complete, without exiting the "Moving" metastate
                    break;
                case MovingState.BumpedBackingUp:
                    setMovingStateDetail("MetaStateMove - BumpedBackingUp");
                    break;
                default:
                    LogError("TrackRoamerBehaviorsService:MetaStateMove() called in illegal state - " + _state.MovingState);
                    break;
            }
        }

        /// <summary>
        /// Implements the "Mapping" meta state.
        /// </summary>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void MetaStateMap(RoutePlan plan)
        {
            LogInfo("TrackRoamerBehaviorsService: MetaStateMap()  MovingState=" + _state.MovingState);

            // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            // we must use them while executing the plan.

            switch (_state.MovingState)
            {
                case MovingState.RandomTurn:
                    LogInfo("MovingState.RandomTurn");
                    setMovingStateDetail("MetaStateMap - performing RandomTurn");
                    RandomTurn();
                    break;

                case MovingState.Recovering:
                    LogInfo("MovingState.Recovering");
                    setMovingStateDetail("MetaStateMap - Recovering");
                    Recover();
                    break;

                /*
				case MovingState.MapSurroundings:
					_state.Mapped = true;
                    LogInfo("MovingState.MapSurroundings:  Turning 180 deg to map");

                    setMovingStateDetail("MetaStateMap - turning 180 deg to map");

                    SpawnIterator<TurnAndMoveParameters, Handler>(
                        new TurnAndMoveParameters() {
                            speed = (int)Math.Round(MaximumBackwardVelocity),
                            rotatePower = ModerateTurnPower,
                            rotateAngle = 180,
                            desiredMovingState = MovingState.MapSouth
                        },
                        delegate() {
                            _state.Countdown = 15;
                        },
                        TurnAndMoveForward);
                    
					break;

				case MovingState.MapSouth:
                    LogInfo("MovingState.MapSouth:  mapping the View South");

                    Talker.Say(5, "mapping South");

                    setMovingStateDetail("MetaStateMap - mapping South");

                    // _state.South = _laserData;   // needed for FindBestComposite()

                    SpawnIterator<TurnAndMoveParameters, Handler>(
                        new TurnAndMoveParameters() {
                            speed = (int)Math.Round(MaximumBackwardVelocity),
                            rotatePower = ModerateTurnPower,
                            rotateAngle = 180,
                            desiredMovingState = MovingState.MapNorth
                        },
                        delegate() {
                            _state.Countdown = 15;
                        },
                        TurnAndMoveForward);

                        break;

				case MovingState.MapNorth:
                    LogInfo("MovingState.MapNorth:  mapping the View North");
                    
                    Talker.Say(5, "mapping North");

                    //_state.NewHeading = FindBestComposite(_state.South, _laserData);

                    _state.NewHeading = (int)plan.bestHeadingRelative(_mapperVicinity);

                    LogInfo("Composite Map suggests turn to heading: " + _state.NewHeading);

                    setMovingStateDetail("MetaStateMap - mapping North - Composite Map suggests turn: " + _state.NewHeading);

					_state.South = null;
                    _state.MovingState = MovingState.AdjustHeading;
					break;
                */

                case MovingState.InTransition:
                    break;

                default:
                    LogError("TrackRoamerBehaviorsService:MetaStateMap() called in illegal state  - " + _state.MovingState);
                    break;
            }
        }

        /// <summary>
        /// Adjusts the velocity based on environment.
        /// </summary>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void AdjustDirectionAndVelocity(RoutePlan plan)
        {
            LogInfo("TrackRoamerBehaviorsService: AdjustDirectionAndVelocity()");

            // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            // we must use them while executing the plan.

            int legDistanceMm = _state.collisionState.canMoveForwardDistanceMm;

            if (plan.legMeters.HasValue)
            {
                legDistanceMm = Math.Min(legDistanceMm, (int)((double)plan.legMeters * 1000.0d));
            }

            // we come here when _state.MovingState == MovingState.FreeForwards

            _state.Mapped = false;

            //int bestHeading = FindBestHeading(_laserData, 0, (int)Math.Round(_state.Velocity * 1000.0d / 10.0d), CorridorWidthMoving, "FreeForwards - Adjust");

            _state.NewHeading = (int)plan.bestHeadingRelative(_mapperVicinity);    // right - positive, left - negative

            LogInfo("     bestHeading=" + _state.NewHeading + currentCompass);

            if (legDistanceMm > FreeDistanceMm)
            {
                SpawnIterator<TurnAndMoveParameters, Handler>(
                    new TurnAndMoveParameters()
                    {
                        rotateAngle = (int)(((double)_state.NewHeading) * turnHeadingFactor),
                        rotatePower = MaximumTurnPower,
                        speed = (int)Math.Round(Math.Min(_state.collisionState.canMoveForwardSpeedMms, MaximumForwardVelocityMmSec)),
                        desiredMovingState = MovingState.FreeForwards
                    },
                    delegate()
                    {
                    },
                    TurnAndMoveForward);
            }
            else if (legDistanceMm > AwareOfObstacleDistanceMm)
            {
                SpawnIterator<TurnAndMoveParameters, Handler>(
                    new TurnAndMoveParameters()
                    {
                        rotateAngle = (int)(((double)_state.NewHeading) * turnHeadingFactor),
                        rotatePower = ModerateTurnPower,
                        speed = (int)Math.Round(Math.Min(_state.collisionState.canMoveForwardSpeedMms, ModerateForwardVelocityMmSec)),
                        desiredMovingState = MovingState.FreeForwards
                    },
                    delegate()
                    {
                    },
                    TurnAndMoveForward);
            }
            else
            {
                SpawnIterator<TurnAndMoveParameters, Handler>(
                    new TurnAndMoveParameters()
                    {
                        rotateAngle = (int)(((double)_state.NewHeading) * turnHeadingFactor),
                        rotatePower = MinimumTurnPower,
                        speed = (int)Math.Round(Math.Min(_state.collisionState.canMoveForwardSpeedMms, MinimumForwardVelocityMmSec)),
                        desiredMovingState = MovingState.FreeForwards
                    },
                    delegate()
                    {
                        _state.Countdown = Math.Abs(_state.NewHeading / 10);
                    },
                    TurnAndMoveForward);
            }

            LogInfo("     new Countdown=" + _state.Countdown);
        }

        /// <summary>
        /// Implements the "AdjustHeading" state. We are moving already, just need to adjust heading and keep moving.
        /// </summary>
        private void AdjustHeading()
        {
            LogInfo("TrackRoamerBehaviorsService: AdjustHeading()     turning to NewHeading=" + _state.NewHeading + currentCompass);

            _state.MovingState = MovingState.InTransition;
            lastInTransitionStarted = DateTime.Now;

            SpawnIterator<TurnAndMoveParameters, Handler>(
                new TurnAndMoveParameters()
                {
                    rotateAngle = (int)(((double)_state.NewHeading) * turnHeadingFactor),
                    rotatePower = ModerateTurnPower,
                    speed = (int)Math.Round(ModerateForwardVelocityMmSec),
                    desiredMovingState = MovingState.FreeForwards
                },
                delegate()
                {
                    _state.Countdown = Math.Abs(_state.NewHeading / 10);
                },
                TurnAndMoveForward);
        }

        /// <summary>
        /// Implements the "RandomTurn" state.
        /// </summary>
        private void RandomTurn()
        {
            LogInfo("TrackRoamerBehaviorsService: RandomTurn()");

            _state.NewHeading = new Random().Next(-115, 115);
            LogInfo("     start turning (random) to NewHeading=" + _state.NewHeading + currentCompass);

            SpawnIterator<TurnAndMoveParameters, Handler>(
                new TurnAndMoveParameters()
                {
                    rotateAngle = _state.NewHeading,
                    rotatePower = ModerateTurnPower,
                    desiredMovingState = MovingState.Unknown
                },
                delegate()
                {
                    _state.Countdown = 2 + Math.Abs(_state.NewHeading / 10);
                },
                TurnAndMoveForward);
        }

        /// <summary>
        /// Transitions to "Mapping" meta state or "AdjustHeading" state depending on
        /// environment.
        /// </summary>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void MetaStateStartMapping(RoutePlan plan)
        {
            int distance = 0;

            if (plan.legMeters.HasValue)
            {
                distance = (int)((double)plan.legMeters * 1000.0d);
            }

            LogInfo("TrackRoamerBehaviorsService: MetaStateStartMapping(distance=" + distance + ")   ObstacleDistance=" + ObstacleDistanceMm);

            Talker.Say(5, "Mapping");

            markNorthBitmap("Start Mapping");

            StopMoving();

            // at this point constraints and allowable speeds and distances are known and stored in _state.collisionState
            // we must use them while executing the plan.

            if (distance < ObstacleDistanceMm)
            {
                LogInfo("    distance < ObstacleDistance (not finding exit)   _state.Mapped=" + _state.Mapped);

                _state.MovingState = MovingState.Recovering;
                LogInfo("    MovingState Recovering (not finding exit)");
                //Talker.Say(5, "Recovering");
                setMovingStateDetail("Recovering (not finding exit)");

                /*
				if (_state.Mapped)
				{
					// We have been mapping before but do not seem to
					// have found anything.
                    _state.MovingState = MovingState.RandomTurn;
                    LogInfo("    requesting RandomTurn");
                    Talker.Say(5, "random turn");
                    setMovingStateDetail("Start Mapping (not finding exit) - requesting RandomTurn");
                }
				else
				{
                    _state.MovingState = MovingState.MapSurroundings;
                    LogInfo("    requesting MapSurroundings");
                    Talker.Say(5, "map surroundings");
                    setMovingStateDetail("Start Mapping (not finding exit) - requesting MapSurroundings");
                }
                 * */
            }
            else
            {
                LogInfo("    distance > ObstacleDistance (exit route found)   CorridorWidthMapping=" + CorridorWidthMappingMm);
                //Talker.Say(5, "exit route found");

                int legMm = Math.Min(ObstacleDistanceMm, distance);

                _state.NewHeading = (int)plan.bestHeadingRelative(_mapperVicinity);      // right - positive, left - negative

                LogInfo("Leg: " + legMm + " mm   NewHeading=" + _state.NewHeading);

                setMovingStateDetail("Start Mapping (exit route found) NewHeading=" + _state.NewHeading + "Leg: " + legMm + " mm");

                _state.MovingState = MovingState.AdjustHeading;
                _state.Countdown = legMm / 50 + Math.Abs(_state.NewHeading / 10);

                LogInfo("     new Countdown=" + _state.Countdown);
            }
        }
        #endregion // Decision Making Elements
#endif // OBSOLETE_PLANANDAVOID_LOGIC

#if OBSOLETE_LASER_LOGIC

        #region Laser Specific decision helpers

        /// <summary>
		/// Respresent a laser range finder reading
		/// </summary>
		internal class RangeData
		{
			/// <summary>
			/// Creates a new instance.
			/// </summary>
			/// <param name="distance">measured distance, mm</param>
			/// <param name="heading">heading in degrees</param>
			public RangeData(int distance, double heading)
			{
                _distance = distance;
				_heading = heading;
			}

			int _distance;
			double _heading;

            // computed values:
			public int DistanceAdjusted { get; set; }     // mm; taking into consideration the Corridor - basically how far we can go in this direction given our width
            public double Weight { get; set; }            // some product of distanceAdjusted and angle between current robot heading and this heading, and may be more

            // measured values:

			/// <summary>
			/// Gets the distance in milimeters.
			/// </summary>
			public int Distance
			{
				get { return _distance; }
			}

			/// <summary>
			/// Gets the heading in degrees.
			/// </summary>
			public double Heading
			{
				get { return _heading; }
			}

			/// <summary>
			/// Comparer to sort instances by distance in a list.
			/// </summary>
			/// <param name="first">first reading</param>
			/// <param name="second">second reading</param>
			/// <returns>a value les than 0 if  <paramref name="first"/> is closer than <paramref name="second"/>, 0 if both have the same distance, a value greater 0 otherwise</returns>
			static public int ByDistance(RangeData first, RangeData second)
			{
				return first._distance.CompareTo(second._distance);
			}
            
            static public int ByHeading(RangeData first, RangeData second)
            {
                // we compare both values based on how far they are from "straight ahead" course
                double dFirst = Math.Abs(first.Heading);
                double dSecond = Math.Abs(second.Heading);
                return dFirst.CompareTo(dSecond);
            }

            static public int ByWeight(RangeData first, RangeData second)
            {
                return first.Weight.CompareTo(second.Weight);
            }
        }

		/// <summary>
		/// Finds the best free corridor (maximum free space ahead) in a 360 degree scan.
		/// </summary>
		/// <param name="south">the backward half of the scan</param>
		/// <param name="north">the forward half of the scan</param>
		/// <returns>best heading in degrees (right turn - positive, left turn - negative)</returns>
		private int FindBestComposite(sicklrf.State south, sicklrf.State north)
		{
            try
            {
                // sanity check:
                LogInfo("FindBestComposite() south: " + south.DistanceMeasurements.Length + " points,  north: " + north.DistanceMeasurements.Length + " points");

                if (south.DistanceMeasurements.Length + north.DistanceMeasurements.Length < 360)
                {
                    LogError("FindBestComposite() - bad laser measurement");
                    return 0;
                }

                sicklrf.State composite = new sicklrf.State();

                composite.DistanceMeasurements = new int[361];

                for (int i = 0; i < composite.DistanceMeasurements.Length; i++)
                {
                    // the trick is to have halves of the South scan become sides of the full North scan,
                    // so that a 360 degrees composite has heading 0 in the middle.

                    if (i < 90)
                    {
                        composite.DistanceMeasurements[i] = south.DistanceMeasurements[i + 90];
                    }
                    else if (i < 270)
                    {
                        composite.DistanceMeasurements[i] = north.DistanceMeasurements[i - 90];
                    }
                    else // 270...359
                    {
                        composite.DistanceMeasurements[i] = south.DistanceMeasurements[i - 270];
                    }
                }

                composite.AngularResolution = 1.0;
                composite.AngularRange = 360;
                composite.Units = north.Units;

                return FindBestHeading(composite, 0, 0, CorridorWidthMoving, "Best Composite");
            }
            catch (Exception exc)
            {
                LogError(exc);
                return 0;
            }
		}

		/// <summary>
		/// Finds the best heading in a 180 degree laser scan
		/// </summary>
		/// <param name="dx">horizontal offset (always 0 here)</param>
		/// <param name="dy">vertical offset (based on velocity)</param>
        /// <param name="width">corridorWidth of corridor that must be free</param>
		/// <returns>best heading in degrees (right turn - positive, left turn - negative)</returns>
        private int FindBestHeading(int dx, int dy, int corridorWidth, string comment)
        {
            LogInfo("FindBestHeading()   dx=" + dx + " dy=" + dy + " width=" + corridorWidth);

            int count = _laserData.DistanceMeasurements.Length;   // can be 181 for laser scan, or 361 for north-south composite
            double span = toRadians(_laserData.AngularRange);     // radians

            bool isComposite = count > 200; // regular is 181, composite is 361

            List<RangeData> ranges = new List<RangeData>();
            int bestHeadingInt = 0;

            // draw as we need to display decision making process. We may get a composite (north-south scan, with robot front in the middle):
            Bitmap bmp = isComposite ? currentStatusGraphics.compositeBmp : currentStatusGraphics.northBmp;
            lock (bmp)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // massage the data - create a bubble of ranges, corrected for forward speed:
                    for (int i = 0; i < count; i++)
                    {
                        int range = _laserData.DistanceMeasurements[i];

                        double angle = span * i / count - span / 2.0d;     // radians; angle from the center

                        double x = range * Math.Sin(angle) - dx;
                        double y = range * Math.Cos(angle) - dy;    // account for some speed, make forward distance shorter

                        angle = Math.Atan2(-x, y);
                        range = (int)Math.Sqrt(x * x + y * y);

                        // with the "dy" speed correction the bubble in front of us is a bit thinner than on the sides.
                        // we don't care about objects too far away, and that also removes the speed-induced distortion on the open.
                        range = Math.Min(range, (int)(FreeDistance * 1.01d));

                        if (i % 10 == 0)
                        {
                            // keep in mind that for rayLine the negative is for the right:
                            rayLine(null, g, range, toDegrees(angle), Pens.Yellow, Brushes.Black);
                        }

                        ranges.Add(new RangeData(range, angle));
                    }

                    // compute distanceAdjusted for each range, so that we know where we can fit with our width:
                    for (int i = 0; i < ranges.Count; i++)
                    {
                        RangeData curr = ranges[i];

                        // for every range see how the robot size fits in that distance spot (with a given robot width):
                        double delta = Math.Atan2(corridorWidth, curr.Distance);
                        double currHeading = curr.Heading;
                        double low = currHeading - delta;
                        double high = currHeading + delta;
                        int distanceAdjusted = curr.Distance;

                        // all rays within our corridor - find minimum:
                        for (int j = 0; j < ranges.Count; j++)
                        {
                            if (i != j && ranges[j].Heading > low && ranges[j].Heading < high)
                            {
                                distanceAdjusted = Math.Min(distanceAdjusted, ranges[j].Distance);
                            }
                        }

                        curr.DistanceAdjusted = distanceAdjusted;

                        curr.Weight = (distanceAdjusted - SafeDistance) * Math.Abs(currHeading > 0 ? Math.PI / 2.0d - currHeading : currHeading + Math.PI / 2.0d);

                        if (i % 10 == 0)
                        {
                            // keep in mind that for rayLine the negative is for the right:
                            rayLine("" + Math.Round(curr.Weight / 100.0d), g, curr.DistanceAdjusted, toDegrees(currHeading), Pens.OrangeRed, Brushes.Black);
                        }
                    }

                    ranges.Sort(RangeData.ByWeight);
                    ranges.Reverse();

                    //// display all candidates left for the final choice (usually just 2-5 rays):
                    //for (int i = 0; i < ranges.Count; i++)
                    //{
                    //    RangeData range = ranges[i];

                    //    // keep in mind that for rayLine the negative is for the right:
                    //    rayLine(null, g, range.Distance, -toDegrees(range.Heading), Pens.OrangeRed, Brushes.Black);
                    //}

                    int bestDistance = ranges[0].DistanceAdjusted;
                    double bestHeading = ranges[0].Heading;
                    double bestHeadingDistance = bestDistance;

                    /*
                    Random rand = new Random();

                    bool byBestDistance = false;

                    if (ranges.Count > 1)
                    {
                        for (int i = 1; i < ranges.Count; i++)
                        {
                            if (ranges[i].Distance < bestDistance)
                            {
                                byBestDistance = true;  // we have a true winner
                                bestHeading = ranges[i-1].Heading;
                                bestHeadingDistance = ranges[i-1].Distance;
                                break;
                            }
                        }

                        if (!byBestDistance)
                        {
                            // Could not choose by best distance - we are on the open at least on one side; get creative.
                            // How about just keeping it the closest to the straight course?
                            ranges.Sort(RangeData.ByHeading);

                            bestHeading = ranges[0].Heading;
                            bestHeadingDistance = ranges[0].Distance;

                            //for (int i = 0; i < ranges.Count; i++)
                            //{
                            //    if (rand.Next(i + 1) == 0)
                            //    {
                            //        bestHeading = ranges[i].Heading;
                            //        bestHeadingDistance = ranges[i].Distance;
                            //    }
                            //}
                        }
                    }
                    */

                    // right turn - positive, left turn - negative, expressed in degrees:
                    bestHeadingInt = (int)Math.Round(toDegrees(bestHeading));

                    // now draw our decision; keep in mind that for rayLine the negative is for the right:

                    using (Pen dirPen = new Pen(Color.LimeGreen, 5.0f))
                    {
                        dirPen.EndCap = LineCap.ArrowAnchor;
                        rayLine("" + bestHeadingInt, g, bestHeadingDistance*1.1d, (double)bestHeadingInt, dirPen, Brushes.Green);
                    }

                    int xLbl = 10;
                    int yLbl = bmp.Height - 40;

                    g.DrawString(comment, StatusGraphics.fontBmpL, Brushes.Green, xLbl, yLbl);
                }
            }
            // end of drawing

            string bestHeadingSay = "straight";

            if (bestHeadingInt < 0)
            {
                bestHeadingSay = "left " + (-bestHeadingInt);
            }
            else if (bestHeadingInt > 0)
            {
                bestHeadingSay = "right " + bestHeadingInt;
            }

            Talker.Say(5, "best heading - " + bestHeadingSay);

            LogInfo("FindBestHeading() found best heading = " + bestHeadingInt + " degrees (" + bestHeadingSay + ")");

            return bestHeadingInt;
        }

        /// <summary>
		/// Finds closest obstacle in a corridor.
		/// </summary>
		/// <param name="width">corridor width</param>
		/// <param name="fov">field of view in degrees, on each side</param>
		/// <returns>distance to the closest obstacle</returns>
		private int FindNearestObstacleInCorridor(int width, int fieldOfView)
		{
			int index;
			int best = 8192;
			int count = _laserData.DistanceMeasurements.Length;
			double rangeLow = -_laserData.AngularRange / 2.0;
			double rangeHigh = _laserData.AngularRange / 2.0;
			double span = _laserData.AngularRange;

			for (index = 0; index < count; index++)
			{
				double angle = rangeLow + (span * index) / count;
				if (Math.Abs(angle) <= fieldOfView)
				{
					angle = angle * Math.PI / 180;

					int range = _laserData.DistanceMeasurements[index];
					int x = (int)(range * Math.Sin(angle));
					int y = (int)(range * Math.Cos(angle));

					if (Math.Abs(x) < width)
					{
						if (range < best)
						{
							best = range;
						}
					}
				}
			}

			return best;
		}
        #endregion

#endif // OBSOLETE_LASER_LOGIC

    }
}
