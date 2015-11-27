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

using Trackroamer.Library.LibAnimatronics;

namespace Trackroamer.Robotics.AnimatronicHead
{
    /// <summary>
    /// per-channel animation strips
    /// </summary>
    public class Animations : Dictionary<string, Animation>
    {
        public Animations()
        {
            // all named animations are single-channel strips.
            // they are designed for 10 seconds, and must be scaled to fit the purpose.

            #region Tongue Left

            this.Add("Tongue Left On", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Tongue Left Off", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

            this.Add("Tongue Left Cycle", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Tongue Left Cycle Weak", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 30, 5000 },
                    { -1, 10, 5000 }
                }
            ));

            this.Add("Tongue Left Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 30, 5000 }
                }
            ));

            this.Add("Tongue Left Blink", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 8000  },
                }
            ));

            this.Add("Tongue Left Blink Fast", new Animation(
                AnimationChannels.ANIM_TONGUE_L,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 0, 9000  },
                }
            ));

            #endregion // Tongue Left

            #region Tongue Right

            this.Add("Tongue Right On", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Tongue Right Off", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

            this.Add("Tongue Right Cycle", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Tongue Right Cycle Weak", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 30, 5000 },
                    { -1, 10, 5000 }
                }
            ));

            this.Add("Tongue Right Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 30, 5000 }
                }
            ));

            this.Add("Tongue Right Blink", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 8000  },
                }
            ));

            this.Add("Tongue Right Blink Fast", new Animation(
                AnimationChannels.ANIM_TONGUE_R,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 0, 9000  },
                }
            ));

            #endregion // Tongue Right

            #region Eye Red Left

            this.Add("Eye Red Left On", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Eye Red Left On Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 10000 }
                }
            ));

            this.Add("Eye Red Left Off", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

            this.Add("Eye Red Left Off Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 0, 10000 }
                }
            ));

            this.Add("Eye Red Left Cycle", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Eye Red Left Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 40, 5000 }
                }
            ));

            this.Add("Eye Red Left Blink", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 8000  },
                }
            ));

            this.Add("Eye Red Left Blink Twice", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 6000  },
                }
            ));

            this.Add("Eye Red Left Blink Fast", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 0, 9000  },
                }
            ));

            this.Add("Eye Red Left Blink2", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 50, 1000  },
                    { -1, 0, 2000  },
                    { -1, 0, 6000 }
                }
            ));

            this.Add("Eye Red Left Blink3", new Animation(
                AnimationChannels.ANIM_EYE_RED_L,
                new int[,] {
                    { -1, 0, 1000 },
                    { -1, 100, 500 },
                    { -1, 0, 1000  },
                    { -1, 0, 7500 }
                }
            ));

            #endregion // Eye Red Left

            #region Eye Red Leftmost

            this.Add("Eye Red Leftmost On", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Eye Red Leftmost On Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 10000 }
                }
            ));

           this.Add("Eye Red Leftmost Off", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

           this.Add("Eye Red Leftmost Off Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 0, 10000 }
                }
            ));

            this.Add("Eye Red Leftmost Cycle", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Eye Red Leftmost Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 40, 5000 }
                }
            ));

            this.Add("Eye Red Leftmost Blink", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 8000  },
                }
            ));

            this.Add("Eye Red Leftmost Blink Twice", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 6000  },
                }
            ));

            this.Add("Eye Red Leftmost Blink Fast", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 0, 9000  },
                }
            ));

            this.Add("Eye Red Leftmost Blink2", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 50, 1000  },
                    { -1, 0, 2000  },
                    { -1, 0, 6000 }
                }
            ));

            this.Add("Eye Red Leftmost Blink3", new Animation(
                AnimationChannels.ANIM_EYE_RED_LM,
                new int[,] {
                    { -1, 0, 1000 },
                    { -1, 100, 500 },
                    { -1, 0, 1000  },
                    { -1, 0, 7500 }
                }
            ));

            #endregion // Eye Red Leftmost

            #region Eye Red Rightmost

            this.Add("Eye Red Rightmost On", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Eye Red Rightmost On Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 10000 }
                }
            ));

            this.Add("Eye Red Rightmost Off", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

            this.Add("Eye Red Rightmost Off Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 0, 10000 }
                }
            ));

            this.Add("Eye Red Rightmost Cycle", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Eye Red Rightmost Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 40, 5000 }
                }
            ));

            this.Add("Eye Red Rightmost Blink", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 8000  },
                }
            ));

            this.Add("Eye Red Rightmost Blink Twice", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 6000  },
                }
            ));

            this.Add("Eye Red Rightmost Blink Fast", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 0, 9000  },
                }
            ));

            this.Add("Eye Red Rightmost Blink2", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 50, 1000  },
                    { -1, 0, 2000  },
                    { -1, 0, 6000 }
                }
            ));

            this.Add("Eye Red Rightmost Blink3", new Animation(
                AnimationChannels.ANIM_EYE_RED_RM,
                new int[,] {
                    { -1, 0, 1000 },
                    { -1, 100, 500 },
                    { -1, 0, 1000  },
                    { -1, 0, 7500 }
                }
            ));

            #endregion // Eye Red Rightmost

            #region Eye Red Right

            this.Add("Eye Red Right On", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Eye Red Right On Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 10000 }
                }
            ));

            this.Add("Eye Red Right Off", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

            this.Add("Eye Red Right Off Slow", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 0, 10000 }
                }
            ));

            this.Add("Eye Red Right Cycle", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Eye Red Right Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 40, 5000 }
                }
            ));

            this.Add("Eye Red Right Blink", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 8000  },
                }
            ));

            this.Add("Eye Red Right Blink Twice", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 100, 1000 },
                    { -1, 0, 1000 },
                    { -1, 0, 6000  },
                }
            ));

            this.Add("Eye Red Right Blink Fast", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 0, 9000  },
                }
            ));

            this.Add("Eye Red Right Blink2", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 50, 1000  },
                    { -1, 0, 2000  },
                    { -1, 0, 6000 }
                }
            ));

            this.Add("Eye Red Right Blink3", new Animation(
                AnimationChannels.ANIM_EYE_RED_R,
                new int[,] {
                    { -1, 0, 1000 },
                    { -1, 100, 500 },
                    { -1, 0, 1000  },
                    { -1, 0, 7500 }
                }
            ));

            #endregion // Eye Red Right

            #region Eyes White

            this.Add("Eyes White On", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 200 },
                    { -1, 100, 9800 },
                }
            ));

            this.Add("Eyes White On Slow", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 10000 }
                }
            ));

            this.Add("Eyes White Off", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 0, 200 },
                    { -1, 0, 9800 },
                }
            ));

            this.Add("Eyes White Off Slow", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 0, 10000 }
                }
            ));

            this.Add("Eyes White Cycle", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 0, 5000 }
                }
            ));

            this.Add("Eyes White Cycle Stay Half Lit", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 5000 },
                    { -1, 60, 5000 }
                }
            ));

            this.Add("Eyes White Blink", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 1000  },
                    { -1, 0, 1000 },
                    { -1, 0, 8000 }
                }
            ));

            this.Add("Eyes White Blink Fast", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 500  },
                    { -1, 0, 500 },
                    { -1, 0, 9000 }
                }
            ));

            this.Add("Eyes White Blink Twice", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 1000  },
                    { -1, 0, 1000 },
                    { -1, 100, 1000  },
                    { -1, 0, 1000 },
                    { -1, 0, 6000 },
                }
            ));

            this.Add("Eyes White Blink Twice Fast", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 100, 500  },
                    { -1, 0, 500 },
                    { -1, 100, 500  },
                    { -1, 0, 500 },
                    { -1, 0, 8000 },
                }
            ));

            this.Add("Eyes White Blink2", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { 0, 100, 500 },
                    { -1, 0, 500 },
                    { -1, 50, 1000  },
                    { -1, 0, 2000  },
                    { -1, 0, 6000 }
                }
            ));

            this.Add("Eyes White Blink3", new Animation(
                AnimationChannels.ANIM_EYE_WHITE,
                new int[,] {
                    { -1, 0, 1000 },
                    { -1, 100, 500 },
                    { -1, 0, 1000  },
                    { -1, 0, 7500 }
                }
            ));

            #endregion // Eyes White

            #region Servo Pan

            const int HEAD_LEFT         = 1000;
            const int HEAD_LEFT_SMALL   = 1300;
            const int HEAD_RIGHT        = 2000;
            const int HEAD_RIGHT_SMALL  = 1700;
            const int HEAD_STRAIGHT     = 1500;

            this.Add("Turn Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_STRAIGHT, 10000 }
                }
            ));

            this.Add("Turn Left", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT, 10000 }
                }
            ));

            this.Add("Turn Cycle Left Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT, 5000 },
                    { -1, HEAD_STRAIGHT, 5000 }
                }
            ));

            this.Add("Turn Cycle Left Small Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT_SMALL, 5000 },
                    { -1, HEAD_STRAIGHT, 5000 }
                }
            ));

            this.Add("Turn Cycle Left Small Short Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT_SMALL, 2000 },
                    { -1, HEAD_STRAIGHT, 2000 },
                    { -1, HEAD_STRAIGHT, 6000 }
                }
            ));

            this.Add("Turn Left Small", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT_SMALL, 10000 }
                }
            ));

            this.Add("Turn Right", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_RIGHT, 10000 }
                }
            ));

            this.Add("Turn Cycle Right Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_RIGHT, 5000 },
                    { -1, HEAD_STRAIGHT, 5000 }
                }
            ));

            this.Add("Turn Cycle Right Small Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_RIGHT_SMALL, 5000 },
                    { -1, HEAD_STRAIGHT, 5000 }
                }
            ));

            this.Add("Turn Cycle Right Small Fast Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_RIGHT_SMALL, 2000 },
                    { -1, HEAD_STRAIGHT, 2000 },
                    { -1, HEAD_STRAIGHT, 6000 }
                }
            ));

            this.Add("Turn Right Small", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_RIGHT_SMALL, 10000 }
                }
            ));

            this.Add("Turn Cycle Left Right Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT, 4000 },
                    { -1, HEAD_RIGHT, 4000 },
                    { -1, HEAD_STRAIGHT, 2000 }
                }
            ));

            this.Add("Turn Cycle Left Right Small Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT_SMALL, 4000 },
                    { -1, HEAD_RIGHT_SMALL, 4000 },
                    { -1, HEAD_STRAIGHT, 2000 }
                }
            ));

            this.Add("Turn Cycle Right Left Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_RIGHT, 4000 },
                    { -1, HEAD_LEFT, 4000 },
                    { -1, HEAD_STRAIGHT, 2000 }
                }
            ));

            this.Add("Turn Cycle Right Left Small Stay Straight", new Animation(
                AnimationChannels.ANIM_SERVO_PAN,
                new int[,] {
                    { -1, HEAD_LEFT_SMALL, 2500 },
                    { -1, HEAD_RIGHT_SMALL, 5000 },
                    { -1, HEAD_STRAIGHT, 2500 }
                }
            ));

            #endregion // Servo Pan

            #region Servo Tilt

            const int HEAD_DOWN         = 1800;
            const int HEAD_DOWN_SMALL   = 1650;
            const int HEAD_UP           = 1100;
            const int HEAD_UP_SMALL     = 1300;
            const int HEAD_LEVEL        = 1500;

            this.Add("Tilt Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_LEVEL, 10000 }
                }
            ));

            this.Add("Tilt Up", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_UP, 10000 }
                }
            ));

            this.Add("Tilt Cycle Up Stay Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_UP, 5000 },
                    { -1, HEAD_LEVEL, 5000 }
                }
            ));

            this.Add("Tilt Cycle Up Stay Up Small", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_UP, 5000 },
                    { -1, HEAD_UP_SMALL, 5000 }
                }
            ));

            this.Add("Tilt Cycle Up Small Stay Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_UP_SMALL, 5000 },
                    { -1, HEAD_LEVEL, 5000 }
                }
            ));

            this.Add("Tilt Cycle Up Small Fast Stay Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_UP_SMALL, 2000 },
                    { -1, HEAD_LEVEL, 2000 },
                    { -1, HEAD_LEVEL, 6000 }
                }
            ));

            this.Add("Tilt Up Small", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_UP_SMALL, 10000 }
                }
            ));

            this.Add("Tilt Down", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_DOWN, 10000 }
                }
            ));

            this.Add("Tilt Cycle Down Stay Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_DOWN, 5000 },
                    { -1, HEAD_LEVEL, 5000 }
                }
            ));

            this.Add("Tilt Cycle Down Stay Down Small", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_DOWN, 5000 },
                    { -1, HEAD_DOWN_SMALL, 5000 }
                }
            ));

            this.Add("Tilt Cycle Down Small Stay Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_DOWN_SMALL, 5000 },
                    { -1, HEAD_LEVEL, 5000 }
                }
            ));

            //this.Add("Tilt Cycle Down Small Fast Stay Level", new Animation(
            //    AnimationChannels.ANIM_SERVO_TILT,
            //    new int[,] {
            //        { -1, HEAD_DOWN_SMALL, 3000 },
            //        { -1, HEAD_LEVEL, 3000 },
            //        { -1, HEAD_LEVEL, 4000 }
            //    }
            //));

            this.Add("Tilt Cycle Down Small Fast Stay Level", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_DOWN_SMALL, 2000 },
                    { -1, HEAD_LEVEL, 2000 },
                    { -1, HEAD_LEVEL, 6000 }
                }
            ));

            this.Add("Tilt Down Small", new Animation(
                AnimationChannels.ANIM_SERVO_TILT,
                new int[,] {
                    { -1, HEAD_DOWN_SMALL, 10000 }
                }
            ));

            #endregion // Servo Tilt

            #region Servo Jaw

            const int JAW_OPEN = 1800;
            const int JAW_CLOSE = 1200;

            this.Add("Jaw Open", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_OPEN, 10000 }
                }
            ));

            this.Add("Jaw Close", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_CLOSE, 10000 }
                }
            ));

            this.Add("Jaw Cycle Once Stay Open", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_CLOSE, 5000 },
                    { -1, JAW_OPEN, 5000 }      // stay open
                }
            ));

            this.Add("Jaw Cycle Once Short Stay Open", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_CLOSE, 3000 },
                    { -1, JAW_OPEN, 3000 },
                    { -1, JAW_OPEN, 4000 }      // stay open
                }
            ));

            this.Add("Jaw Cycle Twice Stay Open", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_CLOSE, 2000 },
                    { -1, JAW_OPEN, 2000 },
                    { -1, JAW_CLOSE, 2000 },
                    { -1, JAW_OPEN, 4000 }      // stay open
                }
            ));

            this.Add("Jaw Cycle Once Stay Closed", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_OPEN, 5000 },
                    { -1, JAW_CLOSE, 5000 }      // stay closed
                }
            ));

            this.Add("Jaw Cycle Once Short Stay Closed", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_OPEN, 3000 },
                    { -1, JAW_CLOSE, 3000 },
                    { -1, JAW_CLOSE, 4000 }      // stay closed
                }
            ));

            this.Add("Jaw Cycle Twice Stay Closed", new Animation(
                AnimationChannels.ANIM_SERVO_JAW,
                new int[,] {
                    { -1, JAW_OPEN, 2000 },
                    { -1, JAW_CLOSE, 2000 },
                    { -1, JAW_OPEN, 2000 },
                    { -1, JAW_CLOSE, 4000 }      // stay closed
                }
            ));

            #endregion // Servo Jaw

        }

    }
}
