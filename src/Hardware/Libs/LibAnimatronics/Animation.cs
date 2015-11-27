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

using Trackroamer.Library.LibArduinoComm;

namespace Trackroamer.Library.LibAnimatronics
{
    public class Animation
    {
        private AnimationChannels _channel;
        private int[,] _commandValues;

        public Animation(AnimationChannels channel, int[,] commandValues)
        {
            _channel = channel;
            _commandValues = commandValues;
        }

        public ToArduino ToArduino(double scale = 1.0d, bool doRepeat = false)
        {
            int nFrames = _commandValues.Length / 3;

            ToArduino ret = new ToArduino
            {
                channel = (int)_channel,
                command = (int)AnimationCommands.SET_FRAMES + (nFrames << 8) + (doRepeat ? 0x80 : 0),
                commandValues = scaleValues(scale)
            };

            return ret;
        }

        private int[] scaleValues(double scale)
        {
            int[] ret = new int[_commandValues.Length];

            int k = 0;
            for (int i = 0; i <= _commandValues.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _commandValues.GetUpperBound(1); j++)
                {
                    if (j == 2)
                    {
                        ret[k++] = (int)(_commandValues[i, j] * scale);
                    }
                    else
                    {
                        ret[k++] = _commandValues[i, j];
                    }

                }
            }

            return ret;
        }

    }
}
