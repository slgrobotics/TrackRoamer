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
        protected DateTime lastGoStraightToDirection = DateTime.MinValue;
        protected double goStraightToDirectionIntervalSec = 5.0d;

        #region GoStraightToDirection() tactics

        private void TacticsGoStraightToDirection()
        {
            Direction curDir = _mapperVicinity.robotDirection;  // also in _currentGoalBearing

            if (curDir != null && curDir.bearing.HasValue &&
                (DateTime.Now > lastGoStraightToDirection.AddSeconds(goStraightToDirectionIntervalSec)
                    || (curDir.bearingRelative.HasValue && Math.Abs(curDir.bearingRelative.Value) > 3.0d))
                )
            {
                // AvoidCollision and EnterOpenSpace have precedence over
                // all other state transitions and are thus handled first.
                bool canMove = PerformAvoidCollision(null);
                if (!canMove)
                {
                    StopMoving();
                    _state.MovingState = MovingState.Unable;
                    _state.Countdown = 0;   // 0 = immediate response
                }

                //LogHistory(10, "Go Straight To Direction:  bearing=" + curDir.bearing + "   heading=" + curDir.heading);

                //Tracer.Trace("*******************************************************************  Go Straight To Direction:  bearing=" + curDir.bearing + "   heading=" + curDir.heading);

                SpawnIterator<TurnAndMoveParameters, Handler>(
                    new TurnAndMoveParameters()
                    {
                        speed = (int)Math.Round(ModerateForwardVelocityMmSec),
                        rotateAngle = (int)curDir.turnRelative,
                        rotatePower = ModerateTurnPower,
                        desiredMovingState = MovingState.FreeForwards
                    },
                    delegate()
                    {
                    },
                    TurnAndMoveForward);

                lastGoStraightToDirection = DateTime.Now;
            }
        }

        #endregion // GoStraightToDirection() tactics
    }
}
