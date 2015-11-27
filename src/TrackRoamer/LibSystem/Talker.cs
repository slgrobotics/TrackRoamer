using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Speech;
using System.Speech.Synthesis;


namespace TrackRoamer.Robotics.Utility.LibSystem
{
    public static class Talker
    {
        private static SpeechSynthesizer speaker = new SpeechSynthesizer();

        private static int currentLevel  = 5;    // 10=errors, 9-lifecycle(start&exit) 1=elementary moves, 2=higher moves, 3=data/sensor inputs, 4=state changes, 5=decision making, 6=human interactions
        private static int currentLevelH = 1;    // what gets put into History
        private static bool doTalk = true;

        public static History HistorySaid = new History();

        public static DateTime lastSpoken = DateTime.Now;   // for speech recognizer to be able to skip talker's speech


        /// <summary>
        /// says whatever string aloud
        /// </summary>
        /// <param name="severityLevel">10-errors, 9-lifecycle, 1-elementary moves, 2-higher moves, 3-data and sensor inputs, 4-state changes, 5-decision making, 6-human interactions</param>
        /// <param name="speech">text to say</param>
        /// <param name="rate">rate -10...10 default 3</param>
        public static void Say(int severityLevel, string speech, int rate = 3)
        {
            string tospeak = string.Format("{0}: {1}", severityLevel, speech);
            Tracer.Trace("Say " + tospeak);

            try
            {
                if (severityLevel >= currentLevelH)
                {
                    HistorySaid.Record(new HistoryItem() { timestamp = DateTime.Now.Ticks, level = severityLevel, message = speech });
                }

                if (doTalk && severityLevel >= currentLevel)
                {
                    lock (speaker)
                    {
                        speaker.SpeakAsyncCancelAll();
                        // Volume:
                        speaker.Volume = 100;  // 0...100
                        // talking speed:
                        speaker.Rate = rate;     // -10...10
                        lastSpoken = DateTime.Now;
                        speaker.SpeakAsync(" " + speech + (speech.EndsWith("?") ? "" : ". "));
                        //speaker.Speak(" " + speech + ". ");
                    }
                }
            }
            catch (Exception exc)
            {
                // stay silent
            }
        }
    }
}
