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
using System.Threading.Tasks;

namespace TrackRoamer.Robotics.LibBehavior
{
    /// <summary>
    /// Helper class to compute exponential moving average
    /// </summary>
    public class Ema
    {
        // variables :
        private double? valuePrev = null;
        private int emaPeriod;
        private double multiplier;

        /// <summary>
        /// Constructor. Good value for period is 5...30
        /// </summary>
        /// <param name="period"></param>
        public Ema(int period)
        {
            emaPeriod = period;
            multiplier = 2.0d / (1.0d + (double)emaPeriod);
        }

        /// <summary>
        /// Call Compute() every time you have a measurement. Returns smoothened value.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public double Compute(double val)
        {
            double? valEma = !valuePrev.HasGoodValue() ? val : ((val - valuePrev) * multiplier + valuePrev);
            valuePrev = valEma;
            return valEma.GetValueOrDefault();
        }
    }
}

public static class GoodValueHelper
{
    public static bool HasGoodValue(this double? val)
    {
        return val.HasValue && !double.IsNaN(val.Value) && !double.IsInfinity(val.Value);
    }
}

