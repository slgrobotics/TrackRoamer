//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
//using bumper = TrackRoamer.Robotics.Services.TrackRoamerServices.Bumper.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region  Bumper handlers

        /// <summary>
        /// Handles Replace notifications from the Bumper partner - updating state of the whole set of sensors
        /// </summary>
        /// <remarks>Posts a <typeparamref name="BumpersArrayUpdate"/> to itself.</remarks>
        /// <param name="replace">notification</param>
        void BumperReplaceNotification(bumper.Replace replace)
        {
            LogInfo("DriveBehaviorServiceBase: BumperReplaceNotification()");
            _mainPort.Post(new BumpersArrayUpdate(replace.Body));
        }

        /// <summary>
        /// Handles Update notification from the Bumper partner - updates a single sensor state (usually whiskers)
        /// </summary>
        /// <remarks>Posts a <typeparamref name="BumperUpdate"/> to itself.</remarks>
        /// <param name="update">notification</param>
        void BumperUpdateNotification(bumper.Update update)
        {
            LogInfo("DriveBehaviorServiceBase: BumperUpdateNotification()");
            _mainPort.Post(new BumperUpdate(update.Body));
        }

        /// <summary>
        /// Handles the <typeparamref name="BumpersArrayUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void BumpersArrayUpdateHandler(BumpersArrayUpdate update)
        {
            LogInfo("DriveBehaviorServiceBase: BumpersArrayUpdateHandler()");

            if ((_testBumpMode || _state.IsMoving) && BumpersPressed(update.Body))
            {
                Bumped(false, false, update.Body);
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Handles the <typeparamref name="BumperUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void BumperUpdateHandler(BumperUpdate update)
        {
            LogInfo("DriveBehaviorServiceBase: BumperUpdateHandler() HardwareIdentifier=" + update.Body.HardwareIdentifier);

            if (update.Body.HardwareIdentifier == 101)
            {
                _state.MostRecentWhiskerLeft = update.Body.Pressed;
            }

            if (update.Body.HardwareIdentifier == 201)
            {
                _state.MostRecentWhiskerRight = update.Body.Pressed;
            }

            if ((_testBumpMode || _state.IsMoving) && update.Body.Pressed)  // allow event to propagate while stationary, in test bump mode
            {
                //Talker.Say("sensor " + update.Body.HardwareIdentifier);

                Bumped(_state.MostRecentWhiskerLeft, _state.MostRecentWhiskerRight, null);
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Checks whether at least one of the contact sensors is pressed.
        /// </summary>
        /// <param name="bumpers"><code>true</code> if at least one bumper in <paramref name="bumpers"/> is pressed, otherwise <code>false</code></param>
        /// <returns></returns>
        private bool BumpersPressed(bumper.ContactSensorArrayState bumpers)
        {
            if (bumpers.Sensors == null)
            {
                return false;
            }
            foreach (bumper.ContactSensor s in bumpers.Sensors)
            {
                if (s.Pressed)
                {
                    return true;
                }
            }
            return false;
        }
        
        #endregion  // Bumper handlers
    }
}
