//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
//#define TRACELOG

using System;
using System.Collections.Generic;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using ccrwpf = Microsoft.Ccr.Adapters.Wpf;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;
using libguiwpf = TrackRoamer.Robotics.LibGuiWpf;

using animhead = TrackRoamer.Robotics.Services.AnimatedHeadService.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    /// <summary>
    /// Valid Combo names
    /// </summary>
    public enum HeadComboAnimations
    {
        Acknowledge, Afraid, Alert, Alert_lookleft, Alert_lookright, Angry, Blink, Blink1, BlinkCycle, Decline, Look_down, Look_up, Lookaround,
        Mad, Mad2, Notgood, Ohno, Pleased, Restpose, Sad,
        Smallturnleft, Smallturnleftdow, Smallturnrigh, Smallturnrightdow,
        Smile, Surprised, Think, Turn_left, Turn_left_smile, Turn_right, Turn_right_smile, Upset, What, Wow, Yeah, Yell, Talk
    }

    partial class TrackRoamerBehaviorsService : DsspServiceBase, IAnimatedHead
    {
        #region TestAnimatedHead

        /// <summary>
        /// for a test, uncomment the call to this metod above and watch console
        /// </summary>
        /// <returns></returns>
        private IEnumerator<ITask> TestAnimatedHead()
        {
            Console.WriteLine("TestAnimatedHead - started");

            yield return Timeout(8000);

            while (KinectPlatformCalibrationInProgress)
            {
                yield return Timeout(2000);
            }

            ClearHeadAnimations();

            yield return Timeout(4000);

            StartHeadAnimationsDefault();

            yield return Timeout(3000);

            StartHeadAnimationCombo(HeadComboAnimations.Mad);

            yield return Timeout(4000);

            StartHeadAnimationCombo(HeadComboAnimations.Acknowledge);

            yield return Timeout(4000);

            ClearHeadAnimations();

            yield break;
        }

        public void ClearHeadAnimations()
        {
            Console.WriteLine("ClearHeadAnimations");

            animhead.ArduinoDeviceCommand cmd = new animhead.ArduinoDeviceCommand() { Command = animhead.AnimatedHeadCommands.ANIMATIONS_CLEAR };

            _animatedHeadCommandPort.Post(new animhead.SendArduinoDeviceCommand(cmd));
        }

        public void StartHeadAnimationsDefault()
        {
            Console.WriteLine("StartHeadAnimationsDefault");

            animhead.ArduinoDeviceCommand cmd = new animhead.ArduinoDeviceCommand() { Command = animhead.AnimatedHeadCommands.ANIMATIONS_DEFAULT };

            _animatedHeadCommandPort.Post(new animhead.SendArduinoDeviceCommand(cmd));
        }

        private DateTime lastHeadAnimationComboStarted = DateTime.Now;
        private HeadComboAnimations? lastHeadAnimationCombo = null;
        bool lastHeadAnimationComboRepeat = false;

        public void StartHeadAnimationCombo(HeadComboAnimations anim, bool repeat = false, double scale = 0.2d)
        {
            // we call StartHeadAnimationComboNow if:
            // - requested different animation
            // - stopping repeat of the same animation
            // - requesting the same animation after 3 seconds
            if (anim != lastHeadAnimationCombo || lastHeadAnimationComboRepeat && !repeat || !lastHeadAnimationComboRepeat && (DateTime.Now - lastHeadAnimationComboStarted).TotalSeconds > 3.0d)
            {
                Console.WriteLine("StartHeadAnimationCombo('" + anim + "')");

                StartHeadAnimationComboNow(anim, false, repeat, scale);
            }
        }

        public void AddHeadAnimationCombo(HeadComboAnimations anim, bool repeat = false, double scale = 0.2d)
        {
            if (anim != lastHeadAnimationCombo || lastHeadAnimationComboRepeat && !repeat || !lastHeadAnimationComboRepeat && (DateTime.Now - lastHeadAnimationComboStarted).TotalSeconds > 3.0d)
            {
                Console.WriteLine("AddHeadAnimationCombo('" + anim + "')");

                StartHeadAnimationComboNow(anim, true, repeat, scale);
            }
        }

        private void StartHeadAnimationComboNow(HeadComboAnimations anim, bool add, bool repeat, double scale)
        {
            string animName = (add ? "+" : string.Empty) + anim.ToString();

            animhead.ArduinoDeviceCommand cmd = new animhead.ArduinoDeviceCommand() { Command = animhead.AnimatedHeadCommands.SetAnimCombo, Args = animName, Scale = scale, doRepeat = repeat };

            _animatedHeadCommandPort.Post(new animhead.SendArduinoDeviceCommand(cmd));

            lastHeadAnimationComboStarted = DateTime.Now;
            lastHeadAnimationCombo = anim;
            lastHeadAnimationComboRepeat = repeat;
        }

        #endregion // TestAnimatedHead
    }
}
