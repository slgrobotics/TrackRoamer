/*
 * Copyright (c) 2013..., Sergei Grichine   http://trackroamer.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *    
 * this is a no-warranty no-liability permissive license - you do not have to publish your changes,
 * although doing so, donating and contributing is always appreciated
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibBehavior
{
    public class TrajectoryPredictor : List<TrajectoryPoint>
    {
        public int Limit = 5;

        private void Enqueue(TrajectoryPoint item)
        {
            while (this.Count >= this.Limit)
            {
                this.RemoveAt(this.Count-1);    // beginning of queue - oldest item
            }
            this.Insert(0, item);               // end of queue - most current item
        }

        /// <summary>
        /// compute where the trajectory will be in some time from now
        /// </summary>
        /// <param name="fromCurrent"></param>
        /// <returns></returns>
        public TrajectoryPoint predict(TrajectoryPoint currentPoint, TimeSpan fromCurrent)
        {
            TrajectoryPoint ret;

            ret = currentPoint;     // speeds/rates remain at 0
			// return ret;

            if (this.Count == 0 || this[0].tooOld)
            {
                ret = currentPoint;     // speeds/rates remain at 0
            }
            else
            {
                TrajectoryPoint prevPoint = this[0];
                double deltaSecs = (currentPoint.timestamp - prevPoint.timestamp).TotalSeconds;

                currentPoint.Xspeed = (currentPoint.X - prevPoint.X) / deltaSecs;
                currentPoint.Yspeed = (currentPoint.Y - prevPoint.Y) / deltaSecs;
                currentPoint.Zspeed = (currentPoint.Z - prevPoint.Z) / deltaSecs;

                currentPoint.panAngleRate = (currentPoint.panAngle - prevPoint.panAngle) / deltaSecs;
                currentPoint.tiltAngleRate = (currentPoint.tiltAngle - prevPoint.tiltAngle) / deltaSecs;

                double deltaSecsP = fromCurrent.TotalSeconds;

                ret = new TrajectoryPoint()
                    {
                        X = currentPoint.X + currentPoint.Xspeed * deltaSecsP,
                        Y = currentPoint.Y + currentPoint.Yspeed * deltaSecsP,
                        Z = currentPoint.Z + currentPoint.Zspeed * deltaSecsP,

                        panAngle = currentPoint.panAngle + currentPoint.panAngleRate * deltaSecsP,
                        tiltAngle = currentPoint.tiltAngle + currentPoint.tiltAngleRate * deltaSecsP
                    };
            }

            Enqueue(currentPoint);

            return ret;
        }
    }
}
