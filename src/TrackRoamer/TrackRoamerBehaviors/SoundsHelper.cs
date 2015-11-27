using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using libguiwpf = TrackRoamer.Robotics.LibGuiWpf;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    public enum SoundSkinType
    {
        None,
        Bullfight
    }

    /// <summary>
    /// SoundSkin is a library of sounds.
    /// Key is a situation name specified by a Behavior
    /// Value is a sound file name
    /// </summary>
    public class SoundSkin : Dictionary<string, string>
    {
        public SoundSkinType soundSkinType;
        public string soundsBasePath;
    }

    /// <summary>
    /// SoundsHelper keeps a library of sounds and plays them based on a "skin" defined by Behavior.
    /// To play the sounds it uses MediaPlayer embedded into the MainWindow.SoundSkinFactory
    /// </summary>
    public class SoundsHelper
    {
        private libguiwpf.MainWindow _mainWindow = null;
        private SoundSkin soundSkin = null;
        private Random randomSound = new Random();

        public SoundsHelper(libguiwpf.MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void SetSoundSkin(SoundSkinType type)
        {
            soundSkin = SoundSkinFactory.Produce(type);     // can be null if type is .None
        }

        public void PlaySound(string key, double volume)
        {
            if (_mainWindow != null)
            {
                string filename = SoundSkinFactory.soundsFileDefault;      // default sound for skin .None and for all not found sounds
                string sbp = SoundSkinFactory.soundsBasePathDefault;

                if (soundSkin != null && soundSkin.ContainsKey(key))
                {
                    filename = soundSkin[key];
                    if (filename.IndexOf("|") != -1)
                    {
                        string[] tmp = filename.Split(new char[] { '|' });
                        int randomIndex = randomSound.Next(tmp.Length);
                        filename = tmp[randomIndex];
                    }
                    sbp = soundSkin.soundsBasePath;
                }
                else
                {
                    Tracer.Trace("PlaySound: key '" + key + "' not found " + (soundSkin == null ? " - null sound skin" : (" in sound skin " + soundSkin.soundSkinType)));
                }

                _mainWindow.PlaySound(filename, Math.Min(1.0d, Math.Max(0.02d, volume)), sbp);   // The media's volume represented on a linear scale between 0 and 1. The default is 0.5

                Talker.lastSpoken = DateTime.Now;   // activate speech recognizer blackout
            }
        }

        /// <summary>
        /// a helper to announce strings through talker or to play sounds through media player.
        /// messageToSay formats:
        ///     "something to say"      - will be read through speech synthesizer
        ///     "$wowcool"              - will be played via player (in current sound skin)
        ///     "something to say;2.3"  - will be read, and 2.2 returned, for example to set the delay before the next message   
        /// </summary>
        /// <param name="messageToSay"></param>
        /// <param name="returnedParameterDefault"></param>
        /// <param name="volume">The media's volume represented on a linear scale between 0 and 1. The default is 0.5</param>
        /// <returns></returns>
        public double Announce(string messageToSay, double returnedParameterDefault, double volume = 0.5d)
        {
            double returnedParameter = returnedParameterDefault;

            if (messageToSay.IndexOf(";") != -1)
            {
                string[] tmp = messageToSay.Split(new char[] { ';' });
                messageToSay = tmp[0];
                returnedParameter = double.Parse(tmp[1]);
            }

            if (messageToSay.StartsWith("$"))
            {
                PlaySound(messageToSay.Substring(1), volume);   // message should be matching one of the keys in the sound scheme
            }
            else
            {
                Talker.Say(10, messageToSay, 0);        // read verbatim
            }

            return returnedParameter;
        }
    }

    /// <summary>
    /// SoundSkinFactory produces a library of sounds defined by SoundSkinType.
    /// </summary>
    public class SoundSkinFactory
    {
        public const string soundsBasePathDefault = @"C:\Microsoft Robotics Dev Studio 4\Media\Sounds\";
        public const string soundsFileDefault = "Beep-SoundBible.com-923660219.wav";      // default sound for skin .None and for all not found sounds

        public static SoundSkin Produce(SoundSkinType type)
        {
            SoundSkin ret = new SoundSkin() { soundSkinType = type };

            switch (type)
            {
                case SoundSkinType.None:
                    ret.soundsBasePath = soundsBasePathDefault;
                    break;

                default:
                case SoundSkinType.Bullfight:
                    ret.soundsBasePath = soundsBasePathDefault;
                    //ret.Add("skeletons number changed","Cow_Moo-Mike_Koenig-42670858.wav");
                    ret.Add("skeletons number changed", "SoundCartoon59.mp3");
                    ret.Add("quick whistle", "cartoonsplit.mp3");
                    ret.Add("crush", "crush.wav");
                    ret.Add("wow cool", "wowcool.wav");
                    ret.Add("evil laugh", "Evil Laugh-SoundBible.com-874992221.wav|evil-laugh-lc.wav");
                    ret.Add("dog growl", "Dog Growling And Barking-SoundBible.com-883632423.wav");
                    ret.Add("taking over the world", "first-we-take-manhattan.wav|first-we-take-manhattan2.wav|first-we-take-manhattan3.wav");
                    ret.Add("lost all humans", "where-do-you-go.wav|where-are-you.wav|where-do-you-go-my-lovely.wav|come-back-and-save-me.wav");
                    ret.Add("you were amazing", "you-were-amazing.wav");
                    ret.Add("lonely", "I-am-Mr-Lonely.mp3");
                    ret.Add("lady in red", "lady-in-red.wav|lady-in-red2.wav|lady-in-red3.wav");
                    break;
            }

            return ret;
        }
    }
}
