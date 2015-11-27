
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

using dssp = Microsoft.Dss.ServiceModel.Dssp;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.LibBehavior;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        private int DecisionMainLoopWaitIntervalMs = 100;     // time to wait in the main loop to keep it from taking all CPU time.

        protected DateTime lastDeepThinking = DateTime.MinValue;

        /// <summary>
        /// Initialize Decision Main Loop
        /// </summary>
        /// <returns>Iterator</returns>
        private IEnumerator<ITask> InitializeDecisionMainLoop()
        {
            SpawnIterator(this.DecisionMainLoop);

            yield break;
        }

        #region DecisionMainLoop()

        /// <summary>
        /// Decision Main Loop
        /// </summary>
        /// <returns>A standard CCR iterator.</returns>
        private IEnumerator<ITask> DecisionMainLoop()
        {
            while (true)
            {
                //Tracer.Trace("...thinking deep...");

                lastDeepThinking = DateTime.Now;

                // Perform SLAM computations:

                Slam();

                // interact with humans
                Interaction();

                // now actually make a decision and execute it:

                Strategy();     // see what moves are appropriate for current task and situation

                setGuiCurrentTactics(_mapperVicinity.robotState.robotTacticsType);      // display which tactics is selected by Strategy, put it in the combo box in the Mapping window

                Tactics();      // execute the moves, if not restricted by the CollisionState

                AdjustKinectTilt();

                // poll N times a sec
                yield return TimeoutPort(DecisionMainLoopWaitIntervalMs).Receive();
            }
        }

        #endregion // DecisionMainLoop()
    }
}
