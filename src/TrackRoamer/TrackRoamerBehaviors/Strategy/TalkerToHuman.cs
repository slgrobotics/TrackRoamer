using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.Services.TrackRoamerBehaviors
{
    /// <summary>
    /// manages the dialogue to human. Ensures proper pauses.
    /// </summary>
    public class TalkerToHuman
    {
        protected SoundsHelper _soundsHelper = null;
        protected IAnimatedHead _animatedHead = null;

        public TalkerToHuman(SoundsHelper soundsHelper, IAnimatedHead animatedHead)
        {
            _soundsHelper = soundsHelper;
            _animatedHead = animatedHead;
        }

        #region Talking to Human

        /*
        private string[] messagesToGreetHuman = new string[] {
            "Hi",
            "I am Track roamer;1.1", "$wow cool",
            //"Want to play?",
            "I chase red shirts;1", "$crush",
            "I shoot foam arrows;0.9", "$quick whistle",
            "How do I get to planet Mars?",
            "Don't call me rusty",
            //"What is your name?",
            //"Welcome to Robotic Society of Southern California",
            "you know, robots will rule the world;2.3", "$taking over the world;12",
            "I will be the robot king!;1.3", "$wow cool",
            "Your Majesty Track Roamer sounds great, right?;5",
            "I will let you charge my batteries;1.3", "$evil laugh;5",
            "are you good with robots?"
        };
        */

        private string[] messagesToGreetHuman = new string[] {
            "Good day, human",
            "Welcome to San Diego Makers Faire",
            "I am Track roamer, your future overlord!",
            "you know, robots will rule the world",
            "I can see your hands. I can flash lights as you move your hands",
            "Hold your left hand out, watch the light",
            "Hold out both hands, watch the lights",
            "I will get angry if you point both arms at me - try that!",
            "If you put your hands up I will flash red lights", //shoot foam arrows at you",
            "Never surrender!",
            "You can surrender to me of course. Everybody does. I understand that.",
            "Have a nice day, Human. And be nice to your robot masters."
            //"I chase red shirts!",
            //"I shoot foam arrows;0.9", "$quick whistle",
            //"How do I get to planet Mars?",
            //"Never call me rusty",
            //"Are you with Robotic Society?",
            //"you know, robots will rule the world",
            //"I will be the robot king!", //;1.3", "$wow cool",
            //"Your Majesty Track Roamer sounds great, right?",
            //"I will let you charge my batteries;1.3", "$evil laugh;5",
            //"are you good with robots?"
        };

        private const double nextAnnouncementDelayDefault = 5.5d;
        private double nextAnnouncementDelaySeconds = nextAnnouncementDelayDefault;

        private int messagesToGreetHumanIndex = 0;
        private DateTime lastTalkToHumanAnnounced = DateTime.Now;

        /// <summary>
        /// true if enough time passed since we last spoken.
        /// </summary>
        /// <returns></returns>
        public bool canTalk()
        {
            return (DateTime.Now - lastTalkToHumanAnnounced).TotalSeconds > nextAnnouncementDelaySeconds;
        }

        /// <summary>
        /// speak the next message to be spoken in this dialogue
        /// </summary>
        public void TalkToHuman()
        {
            ensureAnnouncementDelay();

            _animatedHead.AddHeadAnimationCombo(HeadComboAnimations.Talk);

            string nextMessage = CurrentMessage();

            int oversized = nextMessage.Length - 20;

            double delayFactor = 1.0d + oversized > 0 ? oversized * 0.05d : 0.0d;

            nextAnnouncementDelaySeconds = _soundsHelper.Announce(nextMessage, nextAnnouncementDelayDefault * delayFactor);
        }

        /// <summary>
        /// express anger by growling like a dog
        /// </summary>
        public void GrowlAtHuman()
        {
            ensureAnnouncementDelay();

            _animatedHead.StartHeadAnimationCombo(HeadComboAnimations.Mad);

            nextAnnouncementDelaySeconds = _soundsHelper.Announce("$dog growl;9", nextAnnouncementDelayDefault);
        }

        public void Say(int severityLevel, string speech, int rate = 3)
        {
            ensureAnnouncementDelay();

            Talker.Say(severityLevel, speech, rate);
        }

        /// <summary>
        /// ensure the next talk session will start from the beginning ("rewind" the dialogue)
        /// </summary>
        public void rewindDialogue()
        {
            ensureAnnouncementDelay();
            messagesToGreetHumanIndex = 0;
        }

        /// <summary>
        /// ensures that canTalk() will return false for a while
        /// </summary>
        public void ensureAnnouncementDelay()
        {
            lastTalkToHumanAnnounced = DateTime.Now;
        }

        /// <summary>
        /// get current message to be spoken in this dialogue, increment the index
        /// </summary>
        /// <returns></returns>
        private string CurrentMessage()
        {
            string messageToSay = messagesToGreetHuman[messagesToGreetHumanIndex++];

            if (messagesToGreetHumanIndex >= messagesToGreetHuman.Count())
            {
                messagesToGreetHumanIndex = 0;
            }

            return messageToSay;
        }

        #endregion // Talking to Human

    }
}
