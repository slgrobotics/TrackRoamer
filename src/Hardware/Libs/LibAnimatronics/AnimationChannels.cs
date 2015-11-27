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

namespace Trackroamer.Library.LibAnimatronics
{
    public enum AnimationChannels
    {
        ANIMATIONS      = 0,    // clear animations and set default animations
        PAN             = 1,    // direct control
        TILT            = 2,    // direct control
        JAW             = 3,    // direct control

        ANIM_TONGUE_L   = 10,
        ANIM_TONGUE_R   = 11,
        ANIM_EYE_RED_L  = 12,
        ANIM_EYE_RED_LM = 13,
        ANIM_EYE_RED_RM = 14,
        ANIM_EYE_RED_R  = 15,
        ANIM_EYE_WHITE  = 16,

        ANIM_SERVO_PAN  = 20,
        ANIM_SERVO_TILT = 21,
        ANIM_SERVO_JAW  = 22,
    }
}
