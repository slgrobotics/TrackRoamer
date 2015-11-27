using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using sr = Microsoft.Robotics.Services.Sensors.Kinect.MicArraySpeechRecognizer.Proxy;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.LibMapping;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    /// <summary>
    /// SpeechRecognizerDictionaryItem helps connect voice command semantics from MicArraySpeechRecognizer.user.config.xml with the handler.
    /// </summary>
    internal class SpeechRecognizerDictionaryItem
    {
        public string voiceCommand;
        public string semantics;    // must be unoque across whole dictionary, action will be called based on this key
        public float minimumRequiredConfidence;
        public Handler<SpeechRecognizerDictionaryItem, double> handler;     // second argument is relative bearing degrees to the sound

        public SpeechRecognizerDictionaryItem(string vc, string sem, Handler<SpeechRecognizerDictionaryItem, double> h, float requiredConfidence = 0.5f)
        {
            voiceCommand = vc;
            semantics = sem;
            minimumRequiredConfidence = requiredConfidence;
            handler = h;
        }
    }

    partial class TrackRoamerBehaviorsService : DsspServiceBase
    {
        #region Initialize Speech Recognizer

        private List<SpeechRecognizerDictionaryItem> speechRecognizerDictionary = new List<SpeechRecognizerDictionaryItem>();

        private IEnumerator<ITask> InitializeSpeechRecognizer()
        {
            // Register handlers for notification from speech recognizer
            MainPortInterleave.CombineWith(new Interleave(
                new ExclusiveReceiverGroup(
                    Arbiter.Receive<sr.SpeechDetected>(true, this.speechRecoNotifyPort, this.SpeechDetectedHandler),
                    Arbiter.Receive<sr.SpeechRecognized>(true, this.speechRecoNotifyPort, this.SpeechRecognizedHandler),
                    Arbiter.Receive<sr.SpeechRecognitionRejected>(true, this.speechRecoNotifyPort, this.SpeechRecognitionRejectedHandler),
                    Arbiter.Receive<sr.BeamDirectionChanged>(true, this.speechRecoNotifyPort, this.BeamDirectionChangedHandler)),
                new ConcurrentReceiverGroup()));

            // subscribe to speech recognizer notifications:
            this.speechRecoPort.Subscribe(this.speechRecoNotifyPort);

            // prepare the voice command dictionary with actions. It has to use semantics that are defined in MicArraySpeechRecognizer.user.config.xml
            // the voice commands part can be left empty. You can have many items with the same semantics and handler:
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "stop", VoiceCommandStopHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "continue", VoiceCommandContinueHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "turn left", VoiceCommandTurnLeftHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "turn right", VoiceCommandTurnRightHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "back up", VoiceCommandBackUpHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "track humans", VoiceCommandTrackHumansHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "here I am", VoiceCommandHereIAmHandler));
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "shoot me", VoiceCommandShootMeHandler, 0.65f));      // higher confidence for shooting
            speechRecognizerDictionary.Add(new SpeechRecognizerDictionaryItem("", "your master", VoiceCommandYourMasterHandler));


            /*
            
            // You can set the dictionary only if it hasn't been set by existing C:\Microsoft Robotics Dev Studio 4\store\micarrayspeechrecognizer.user.config.xml
            // or by C:\Microsoft Robotics Dev Studio 4\projects\TrackRoamer\TrackRoamerServices\Config\MicArraySpeechRecognizer.user.config.xml  and manifest

            // Wait 2 seconds:
            yield return Timeout(2000);

           // provide Grammar in the form of dictionary:
            sr.SpeechRecognizerState srState = new sr.SpeechRecognizerState();
            srState.GrammarType = sr.GrammarType.DictionaryStyle;

            srState.DictionaryGrammar = new DssDictionary<string, string>();

            for (int i = 0; i < speechRecognizerDictionary.Count; i++)
            {
                // make sure the voiceCommand part is not empty - contains actual pronounced command:
                srState.DictionaryGrammar.Add(speechRecognizerDictionary[i].voiceCommand.Trim(), speechRecognizerDictionary[i].semantics.Trim());
            }

            //srState.DictionaryGrammar.Add("stop", "stop");

            // Post replace request to SpeechRecognizer service and check outcome
            sr.Replace replaceRequest = new sr.Replace(srState);
            this.speechRecoPort.Post(replaceRequest);

            yield return (Choice)replaceRequest.ResponsePort;
            Fault fault = (Fault)replaceRequest.ResponsePort;
            if (fault != null)
            {
                string message = "Failed to set Dictionary - in Initialize Speech Recognizer";
                LogError(message);
                Talker.Say(10, message);
                yield break;
            }
            */

            // Wait 2 seconds:
            yield return Timeout(2000);

            Talker.Say(10, "talk to me");
        }

        #endregion // Initialize Speech Recognizer

        #region Speech recognizer handlers

        private const double SpeechRecognizerTalkerBlackoutSec = 3.0d;

        /// <summary>
        /// Speech detected handler
        /// </summary>
        /// <param name="detected"></param>
        private void SpeechDetectedHandler(sr.SpeechDetected detected)
        {
            TimeSpan sinceTalk = DateTime.Now - Talker.lastSpoken;
            if (sinceTalk.TotalSeconds < SpeechRecognizerTalkerBlackoutSec)
            {
                Tracer.Trace("SpeechDetectedHandler in blackout at " + sinceTalk.TotalSeconds + " sec");
                return;
            }

            int angle = Direction.to180fromRad(-detected.Body.Angle);

            Tracer.Trace("speech detected at " + angle);

            // register the fact:
            _state.AnySpeechDirection = angle;
            _state.AnySpeechTimeStamp = DateTime.Now;

            //setPan(angle);
            //setTilt(0);

            //Talker.Say(10, "" + ((int)angle));
        }

        /// <summary>
        /// Speech recognized handler
        /// </summary>
        /// <param name="recognized"></param>
        private void SpeechRecognizedHandler(sr.SpeechRecognized recognized)
        {
            TimeSpan sinceTalk = DateTime.Now - Talker.lastSpoken;
            if (sinceTalk.TotalSeconds < SpeechRecognizerTalkerBlackoutSec)
            {
                Tracer.Trace("SpeechRecognizedHandler in blackout at " + sinceTalk.TotalSeconds + " sec");
                return;
            }

            int angle = Direction.to180fromRad(-recognized.Body.Angle);
            string commandText = recognized.Body.Text;
            string commandSemantics = recognized.Body.Semantics.ValueString;
            double confidence = Math.Round(recognized.Body.Confidence, 3);       // 0 to 1, usually around 0.97 for successfully recognized commands

            //Tracer.Trace("****************************************  SpeechRecognizedHandler  **************************************** ");
            //Tracer.Trace("speech '" + commandText + "'=>'" + commandSemantics + "' at " + angle + " degrees,  confidence " + confidence);


            // find the handler based on semantics:
            SpeechRecognizerDictionaryItem srdi = (from di in speechRecognizerDictionary
                                                   where di.semantics == commandSemantics
                                                   select di).FirstOrDefault();


            // usually Confidence is 0.95-0.99
            if (srdi != null && recognized.Body.Confidence > srdi.minimumRequiredConfidence)
            {
                VoiceCommandState state = _state.VoiceCommandState;
                state.TimeStamp = DateTime.Now;
                state.Text = commandText;
                state.Semantics = commandSemantics;
                state.ConfidencePercent = (int)Math.Round(confidence * 100.0f);
                state.Direction = angle;

                LogHistory(1, "speech '" + commandText + "'=>'" + commandSemantics + "' at " + angle + " deg,   " + confidence);

                // now call the handler:
                srdi.handler(srdi, angle);
            }
            else if (confidence > 0.5d)
            {
                LogHistory(3, "rejected '" + commandText + "'=>'" + commandSemantics + "' at " + angle + " deg,   " + confidence);

                Talker.Say(10, "Homm");     // not enough confidence
            }

            //setPan(angle);
            //setTilt(0);
        }

        /// <summary>
        /// Speech recognition rejected handler
        /// </summary>
        /// <param name="rejected"></param>
        private void SpeechRecognitionRejectedHandler(sr.SpeechRecognitionRejected rejected)
        {
            TimeSpan sinceTalk = DateTime.Now - Talker.lastSpoken;
            if (sinceTalk.TotalSeconds < SpeechRecognizerTalkerBlackoutSec)
            {
                Tracer.Trace("SpeechRecognitionRejectedHandler in blackout at " + sinceTalk.TotalSeconds + " sec");
                return;
            }

            int angle = Direction.to180fromRad(-rejected.Body.Angle);

            Tracer.Trace("speech not recognized at " + angle);

            //Talker.Say(10, "What?");
        }

        /// <summary>
        /// Mic array beam direction changed event handler
        /// </summary>
        /// <param name="beamDirectionChanged"></param>
        private void BeamDirectionChangedHandler(sr.BeamDirectionChanged beamDirectionChanged)
        {
            // KinectAudioSource.BeamAngle Property
            // Gets the beam angle (in degrees), which is the direction the audio beam is pointing.
            // The center value is zero, negative values are right of the Kinect device (left of user), and positive values are left of the Kinect device (right of user).

            int angle = Direction.to180fromRad(-beamDirectionChanged.Body.Angle);

            Tracer.Trace("beam direction changed at " + angle);

            // register the fact:
            _state.SoundBeamDirection = angle;
            _state.SoundBeamTimeStamp = DateTime.Now;

            //setPan(angle);
            //setTilt(0);
        }

        #endregion // Speech recognizer handlers

        #region Voice Commands handlers

        private void VoiceCommandStopHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            _mapperVicinity.robotState.robotStrategyType = RobotStrategyType.None;  // active Stop implied
            StopMoving();

            Talker.Say(10, "Stopped!");
        }

        RobotStrategyType robotStrategyTypePrevious = RobotStrategyType.None;

        private void VoiceCommandContinueHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            Talker.Say(10, "Strategy: " + robotStrategyTypePrevious);
            _mapperVicinity.robotState.robotStrategyType = robotStrategyTypePrevious;
        }

        private void VoiceCommandTurnLeftHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            robotStrategyTypePrevious = _mapperVicinity.robotState.robotStrategyType;
            _mapperVicinity.robotState.robotStrategyType = RobotStrategyType.InTransition;
            SafePosture();
            int rotateAngle = -30;
            TurnByAngle(rotateAngle, ModerateTurnPower, true);  // use the shoot-and-forget version.
        }

        private void VoiceCommandTurnRightHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            robotStrategyTypePrevious = _mapperVicinity.robotState.robotStrategyType;
            _mapperVicinity.robotState.robotStrategyType = RobotStrategyType.InTransition;
            SafePosture();
            int rotateAngle = 30;
            TurnByAngle(rotateAngle, ModerateTurnPower, true);  // use the shoot-and-forget version.
        }

        private void VoiceCommandBackUpHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            robotStrategyTypePrevious = _mapperVicinity.robotState.robotStrategyType;
            _mapperVicinity.robotState.robotStrategyType = RobotStrategyType.InTransition;
            SafePosture();
            Translate(-300, MaximumBackwardVelocityMmSec, true);  // use the shoot-and-forget version.
        }

        private void VoiceCommandTrackHumansHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            Talker.Say(10, "Game On!");

            robotStrategyTypePrevious = RobotStrategyType.None;
            _mapperVicinity.robotState.robotStrategyType = RobotStrategyType.PersonFollowing;
        }

        DateTime lastVoiceLocalized = DateTime.MinValue;

        private void VoiceCommandHereIAmHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            if (_mapperVicinity.robotState.robotStrategyType == RobotStrategyType.PersonFollowing && !haveATargetNow && !_mapperVicinity.robotState.ignoreKinectSounds)
            {
                double targetPanRelativeToRobot = _state.currentPanKinect + angle;

                //Talker.Say(10, "Sound at " + ((int)angle));
                Talker.Say(10, "Sound at " + ((int)targetPanRelativeToRobot));

                //Tracer.Trace("+++++++++++++++  currentPanKinect=" + _state.currentPanKinect + "   Sound angle=" + angle + "   targetPanRelativeToRobot=" + targetPanRelativeToRobot);

                //SetDesiredKinectPlatformPan(angle);

                setPanTilt(angle, 5.0d);
                lastVoiceLocalized = DateTime.Now;

                setCurrentGoalBearingRelativeToRobot(targetPanRelativeToRobot);

                // choose robotTacticsType - current tactics is move towards human:
                FollowDirectionMaxVelocityMmSec = ModerateForwardVelocityMmSec;
                _mapperVicinity.robotState.robotTacticsType = RobotTacticsType.FollowDirection;
            }
        }

        private void VoiceCommandShootMeHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            SpawnIterator(ShootGunOnce);
        }

        private void VoiceCommandYourMasterHandler(SpeechRecognizerDictionaryItem grammarItem, double angle)
        {
            Talker.Say(10, "Sir, Sergei Grich inn, Sir!");
        }

        #endregion // Voice Commands handlers

    }
}
