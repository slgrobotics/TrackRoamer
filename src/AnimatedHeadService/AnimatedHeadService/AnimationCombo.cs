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

namespace TrackRoamer.Robotics.Services.AnimatedHeadService
{
    /// <summary>
    /// multi channel animations names database
    /// </summary>
    public class AnimationCombo : Dictionary<string, string[]>
    {
        public AnimationCombo()
        {
            // all named animation combos are multi-channel strips.
            // they are not limited to 10 seconds, and will be scaled to fit the purpose.

            // Character:  VanessaHB (Head Big) : Vanessa from Guile 3D Studio - 2006

            this.Add("Acknowledge", new string[] {              // 1 
                 "Tilt Cycle Down Small Fast Stay Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Once Stay Closed",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Acknowledge_return", new string[] {       // 2 
            //     "Eyes White Blink" 
            // });

            this.Add("Afraid", new string[] {                   // 3 
                 "Tilt Cycle Down Stay Down Small",
                 "Turn Cycle Left Small Stay Straight",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Once Stay Closed",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Afraid_return", new string[] {            // 4 
            //     "Eyes White Blink" 
            // });

            this.Add("Alert", new string[] {                    // 5 
                 "Eyes White Cycle Stay Half Lit",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            this.Add("Alert_lookleft", new string[] {           // 6 
                 "Eyes White Cycle Stay Half Lit",
                 "Turn Left Small",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Alert_lookleft_return", new string[] {    // 7 
            //     "Eyes White Blink" 
            // });

            this.Add("Alert_lookright", new string[] {          // 8 
                 "Eyes White Cycle Stay Half Lit",
                 "Turn Right Small",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Alert_lookright_return", new string[] {   // 9 
            //     "Eyes White Blink" 
            // });

            //this.Add("Alert_return", new string[] {             // 10 
            //     "Eyes White Blink" 
            // });

            //this.Add("Andromeda", new string[] {                // 11 
            //     "Eyes White Blink" 
            // });

            this.Add("Angry", new string[] {                    // 12 
                 "Eye Red Leftmost Blink",
                 "Eye Red Left Cycle Stay Half Lit",
                 "Eye Red Rightmost Blink",
                 "Eye Red Right Cycle Stay Half Lit",
                 "Turn Cycle Right Small Stay Straight",
                 "Tilt Cycle Up Small Fast Stay Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Once Stay Closed",
                 "Tongue Left Blink Fast",
                 "Tongue Right Blink Fast"
             });

            this.Add("Blink", new string[] {                    // 13 
                 "Eyes White Blink2" 
             });

            this.Add("Blink1", new string[] {                   // 14 
                 "Eyes White Blink3" 
             });

            this.Add("BlinkCycle", new string[] {                // 14 
                 "Eyes White Cycle Stay Half Lit", 
                 "Tongue Left Cycle Weak", 
                 "Tongue Right Cycle Weak" 
             });

            //this.Add("Brows_up", new string[] {                 // 15 
            //     "Eyes White Blink" 
            // });

            //this.Add("Cry", new string[] {                      // 16 
            //     "Eyes White Blink" 
            // });

            //this.Add("Cry_return", new string[] {               // 17 
            //     "Eyes White Blink" 
            // });

            //this.Add("Cyber", new string[] {                    // 18 
            //     "Eyes White Blink" 
            // });

            this.Add("Decline", new string[] {                  // 19 
                 "Eye Red Leftmost Blink2",
                 "Eye Red Rightmost Blink2",
                 "Eyes White Cycle Stay Half Lit",
                 "Tilt Level",
                 "Turn Cycle Left Right Small Stay Straight",
                 "Jaw Close"
             });

            //this.Add("Domagic1", new string[] {                 // 20 
            //     "Eyes White Blink" 
            // });

            //this.Add("Domagic2", new string[] {                 // 21 
            //     "Eyes White Blink" 
            // });

            //this.Add("Dontcare", new string[] {                 // 22 
            //     "Eyes White Blink" 
            // });

            //this.Add("Dontcare_return", new string[] {          // 23 
            //     "Eyes White Blink" 
            // });

            //this.Add("Getattention", new string[] {             // 24 
            //     "Eyes White Blink" 
            // });

            //this.Add("Hearing", new string[] {                  // 25 
            //     "Eyes White Blink" 
            // });

            //this.Add("Hearing_return", new string[] {           // 26 
            //     "Eyes White Blink" 
            // });

            //this.Add("Hey", new string[] {                      // 27 
            //     "Eyes White Blink" 
            // });

            //this.Add("Hide", new string[] {                     // 28 
            //     "Eyes White Blink" 
            // });

            //this.Add("Kiss", new string[] {                     // 29 
            //     "Eyes White Blink" 
            // });

            this.Add("Look_down", new string[] {                // 30 
                 "Eyes White Cycle Stay Half Lit",
                 "Tilt Down",
             });

            //this.Add("Look_down_return", new string[] {         // 31 
            //     "Eyes White Blink" 
            // });

            this.Add("Look_up", new string[] {                  // 32 
                 "Eyes White Cycle Stay Half Lit",
                 "Tilt Up",
             });

            //this.Add("Look_up_return", new string[] {           // 33 
            //     "Eyes White Blink" 
            // });

            this.Add("Lookaround", new string[] {               // 34 
                 "Tilt Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Turn Cycle Right Left Stay Straight",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            this.Add("Mad", new string[] {                      // 35 
                 "Eye Red Leftmost Blink",
                 "Eye Red Left Cycle Stay Half Lit",
                 "Eye Red Rightmost Blink",
                 "Eye Red Right Cycle Stay Half Lit",
                 "Turn Cycle Left Right Stay Straight",
                 "Tilt Cycle Up Stay Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Twice Stay Closed",
                 "Tongue Left Cycle Stay Half Lit",
                 "Tongue Right Cycle Stay Half Lit"
             });

            this.Add("Mad2", new string[] {                      // 35 
                 "Eye Red Leftmost Blink",
                 "Eye Red Left Cycle Stay Half Lit",
                 "Eye Red Rightmost Blink",
                 "Eye Red Right Cycle Stay Half Lit",
                 "Turn Cycle Right Left Stay Straight",
                 "Tilt Cycle Up Stay Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Twice Stay Closed",
                 "Tongue Left Cycle Stay Half Lit",
                 "Tongue Right Cycle Stay Half Lit"
             });

            //this.Add("Mad_return", new string[] {               // 36 
            //     "Eyes White Blink" 
            // });

            this.Add("Notgood", new string[] {                  // 37 
                 "Eye Red Leftmost Blink2",
                 "Eye Red Rightmost Blink2",
                 "Eyes White Cycle Stay Half Lit",
                 "Tilt Level",
                 "Turn Cycle Left Right Small Stay Straight",
                 "Jaw Close",
                 "Tongue Left Cycle Stay Half Lit",
                 "Tongue Right Cycle Stay Half Lit"
             });

            //this.Add("Notgood_return", new string[] {           // 38 
            //     "Eyes White Blink" 
            // });

            //this.Add("Notsure", new string[] {                  // 39 
            //     "Eyes White Blink" 
            // });

            //this.Add("Notsure_return", new string[] {           // 40 
            //     "Eyes White Blink" 
            // });

            //this.Add("Nude", new string[] {                     // 41 
            //     "Eyes White Blink" 
            // });

            this.Add("Ohno", new string[] {                     // 42 
                 "Eye Red Leftmost Blink2",
                 "Eye Red Rightmost Blink2",
                 "Eyes White Cycle Stay Half Lit",
                 "Tilt Level",
                 "Turn Cycle Left Right Stay Straight",
                 "Jaw Close",
                 "Tongue Left Cycle Stay Half Lit",
                 "Tongue Right Cycle Stay Half Lit"
             });

            //this.Add("Ohno_return", new string[] {              // 43 
            //     "Eyes White Blink" 
            // });

            //this.Add("Palmtop", new string[] {                  // 44 
            //     "Eyes White Blink" 
            // });

            this.Add("Pleased", new string[] {                  // 45 
                 "Tilt Cycle Down Small Fast Stay Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Once Stay Closed",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
            });

            //this.Add("Pleased_return", new string[] {           // 46 
            //     "Eyes White Blink" 
            // });

            //this.Add("Quiz", new string[] {                     // 47 
            //     "Eyes White Blink" 
            // });

            //this.Add("Read", new string[] {                     // 48 
            //     "Eyes White Blink" 
            // });

            //this.Add("Really", new string[] {                   // 49 
            //     "Eyes White Blink" 
            // });

            //this.Add("Really_return", new string[] {            // 50 
            //     "Eyes White Blink" 
            // });

            this.Add("Restpose", new string[] {                 // 51 
                "Eyes White Cycle Stay Half Lit",
                "Turn Straight",
                "Tilt Level",
                "Jaw Close",
            });

            this.Add("Sad", new string[] {                      // 52 
                "Eyes White Cycle Stay Half Lit",
                "Turn Left Small",
                "Tilt Down Small",
                "Jaw Close",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
            });

            //this.Add("Sad_return", new string[] {               // 53 
            //     "Eyes White Blink" 
            // });

            //this.Add("Shades", new string[] {                   // 54 
            //     "Eyes White Blink" 
            // });

            //this.Add("Shocked", new string[] {                  // 55 
            //     "Eyes White Blink" 
            // });

            //this.Add("Shocked_return", new string[] {           // 56 
            //     "Eyes White Blink" 
            // });

            //this.Add("Show", new string[] {                     // 57 
            //     "Eyes White Blink" 
            // });

            //this.Add("Showing", new string[] {                  // 58 
            //     "Eyes White Blink" 
            // });

            //this.Add("Skeleton", new string[] {                 // 59 
            //     "Eyes White Blink" 
            // });

            //this.Add("Skeleton_return", new string[] {          // 60 
            //     "Eyes White Blink" 
            // });

            //this.Add("Skeleton_talk", new string[] {            // 61 
            //     "Eyes White Blink" 
            // });

            this.Add("Smallturnleft", new string[] {            // 62 
                 "Turn Left Small" 
             });

            this.Add("Smallturnleftdow", new string[] {         // 63 
                 "Turn Left Small",
                 "Tilt Down Small"
             });

            this.Add("Smallturnrigh", new string[] {            // 64 
                 "Turn Right Small" 
             });

            this.Add("Smallturnrightdow", new string[] {        // 65 
                 "Turn Right Small",
                 "Tilt Down Small"
             });

            this.Add("Smile", new string[] {                    // 66 
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Open",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Smile_return", new string[] {             // 67 
            //     "Eyes White Blink" 
            // });

            //this.Add("Stickit", new string[] {                  // 68 
            //     "Eyes White Blink" 
            // });

            //this.Add("Sticky", new string[] {                   // 69 
            //     "Eyes White Blink" 
            // });

            //this.Add("Sticky2", new string[] {                  // 70 
            //     "Eyes White Blink" 
            // });

            this.Add("Surprised", new string[] {                // 71 
                 "Eyes White Blink2",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Surprised_return", new string[] {         // 72 
            //     "Eyes White Blink" 
            // });

            //this.Add("Terminator", new string[] {               // 73 
            //     "Eyes White Blink" 
            // });

            this.Add("Think", new string[] {                    // 74 
                 "Eyes White Cycle Stay Half Lit",
                 "Turn Right Small",
                 "Tilt Down Small",
                 "Jaw Close",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Think_return", new string[] {             // 75 
            //     "Eyes White Blink" 
            // });

            this.Add("Turn_left", new string[] {                // 76 
                 "Turn Left" 
             });

            //this.Add("Turn_left_return", new string[] {         // 77 
            //     "Eyes White Blink" 
            // });

            this.Add("Turn_left_smile", new string[] {          // 78 
                "Eyes White Cycle Stay Half Lit",
                "Turn Left",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
            });

            //this.Add("Turn_left_smile_return", new string[] {    // 79 
            //     "Eyes White Blink" 
            // });

            this.Add("Turn_right", new string[] {               // 80 
                 "Turn Right" 
             });

            //this.Add("Turn_right_return", new string[] {        // 81 
            //     "Eyes White Blink" 
            // });

            this.Add("Turn_right_smile", new string[] {         // 82 
                "Eyes White Cycle Stay Half Lit",
                "Turn Right",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Turn_right_smile_return", new string[] {  // 83 
            //     "Eyes White Blink" 
            // });

            this.Add("Upset", new string[] {                    // 84 
                "Eye Red Leftmost Blink2",
                "Eye Red Rightmost Blink2",
                "Eyes White Cycle Stay Half Lit",
                "Turn Right Small",
                "Tilt Down Small",
                "Jaw Cycle Once Stay Closed",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Upset_return", new string[] {             // 85 
            //     "Eyes White Blink" 
            // });

            //this.Add("Version", new string[] {                  // 86 
            //     "Eyes White Blink" 
            // });

            this.Add("What", new string[] {                     // 87 
                "Eyes White Cycle Stay Half Lit",
                "Turn Straight",
                "Tilt Cycle Up Stay Up Small",
                "Jaw Cycle Twice Stay Open",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("What_return", new string[] {              // 88 
            //     "Eyes White Blink" 
            // });

            this.Add("Wow", new string[] {                      // 89 
                "Eyes White Cycle Stay Half Lit",
                "Turn Straight",
                "Tilt Cycle Up Stay Up Small",
                "Jaw Cycle Twice Stay Open",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Wow_return", new string[] {               // 90 
            //     "Eyes White Blink" 
            // });

            this.Add("Yeah", new string[] {                     // 91 
                "Eye Red Leftmost Blink2",
                "Eye Red Rightmost Blink2",
                "Eyes White Cycle Stay Half Lit",
                "Turn Right Small",
                "Tilt Down Small",
                "Jaw Cycle Once Stay Closed",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
             });

            //this.Add("Yeah_return", new string[] {              // 92 
            //     "Eyes White Blink" 
            // });

            this.Add("Yell", new string[] {                     // 93 
                "Eyes White Cycle Stay Half Lit",
                "Turn Straight",
                "Tilt Cycle Up Stay Up Small",
                "Jaw Cycle Twice Stay Open",
                 "Tongue Left Cycle Stay Half Lit",
                 "Tongue Right Cycle Stay Half Lit"
             });

            this.Add("Talk", new string[] {                  // 45 
                 "Tilt Cycle Up Tiny Fast Stay Level",
                 "Eyes White Cycle Stay Half Lit",
                 "Jaw Cycle Twice Stay Closed",
                 "Tongue Left Cycle Weak",
                 "Tongue Right Cycle Weak"
            });

        }
    }
}
