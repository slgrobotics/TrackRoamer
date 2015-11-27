using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trackroamer.Library.LibArduinoComm
{
    public class ToArduino
    {
        public int channel;
        public int command;
        public int[] commandValues = null;

        /// <summary>
        /// special case - Arduino side should operate in Frames (3 values at a time)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            int checksum = -(channel + command);

            string ret = "*" + channel + " " + command;

            if (commandValues != null)
            {
                foreach (int val in commandValues)
                {
                    ret = ret + " " + val;
                    checksum -= val;
                }
            }

            // note: on Arduino side the last Serial.ParseInt must meet a non-digit char.
            //       that's why we need a trailing whitespace - usually achieved by sending the string using WriteLine()
            return ret + " " + checksum;   // + "\n";
        }

        /*
         * this is a version used for more generic case, where there are no Frames, just Values:
         * 
        public override string ToString()
        {
            int commandValuesLength = commandValues == null ? 0 : commandValues.Length;
            int cmdPlusCount = command + (commandValuesLength << 8);
            int checksum = -(channel + cmdPlusCount);

            string ret = "*" + channel + " " + cmdPlusCount;

            if (commandValuesLength > 0)
            {
                foreach (int val in commandValues)
                {
                    ret = ret + " " + val;
                    checksum -= val;
                }
            }

            // note: on Arduino side the last Serial.ParseInt must meet a non-digit char.
            //       that's why we need a trailing whitespace - usually achieved by sending the string using WriteLine()
            return ret + " " + checksum;   // + "\n";
        }
        */
    }
}

