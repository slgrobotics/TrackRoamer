using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.Utility.LibPicSensors
{
    public class ProximityBoard
    {
        // as defined on the board:
        //const int SERVO_VALUE_MIN = 1275;		// minimum 1275 to make pulse  850 mks
        //const int SERVO_VALUE_MAX = 3150;	    // maximum 3150 to make pulse 2100 mks
        private const int SERVO_VALUE_MIN = 750;		// minimum 750 to make pulse  500 mks
        private const int SERVO_VALUE_MAX = 3750;	    // maximum 3750 to make pulse 2500 mks
        private const int SERVO_VALUE_MID = 2250;	    // neutral 2250 to make pulse 1500 mks

        // we are talking microseconds here, when it comes to servo pulses, passed as parameters:
        //const int SERVO_MKS_MIN = 850;		    // minimum pulse  850 mks
        //const int SERVO_MKS_MAX = 2100;	        // maximum pulse 2100 mks
        private const int SERVO_MKS_MIN = 500;		    // minimum pulse  500 mks
        private const int SERVO_MKS_MAX = 2500;	        // maximum pulse 2500 mks

        // true and tried values to produce 26 rays (13 per side):
        // Parameters of the sweep, they are servo dependent. These are for EXI D227F servos, shaft looking down, left is #1. See trackroamer.com for specifics.
        // It is imperative to receive 26 rays in a frame, or frames will be rejected. Every servo must make 13 steps to contribute to its half frame.
        public const double sweepStartPos1 = 2100.0d; // us
        public const double sweepStartPos2 = 850.0d;  // us
        public const double sweepStep = 148;          // How big a single step is. These are not us, but some internal to PIC microcontroller code number
        public const int    sweepSteps = 13;          // not used, would be nice to calculate SweepStep based on number of steps
        public const double sweepMax = 2100.0d;       // us
        public const double sweepMin = 850;           // us
        public const double sweepRate = 28;           // How fast the servos will move (wait on a step). Must be divisible by 7, due to microcontroller servo pulse cycle interactions.
        public const bool   initialDirectionUp = false;

        // these parameters can be computed and memorized:
        public int rawAngle1_Min = int.MaxValue;
        public double angleDegrees1_Min = -120.0d;
        public int rawAngle1_Max = int.MinValue;
        public double angleDegrees1_Max = -5.0d;

        public int rawAngle2_Min = int.MaxValue;
        public double angleDegrees2_Min = 5.0d;
        public int rawAngle2_Max = int.MinValue;
        public double angleDegrees2_Max = 120.0d;

        SortedList<int, int> rawAngle1Values = new SortedList<int, int>();
        SortedList<int, int> rawAngle2Values = new SortedList<int, int>();

        public int numRays1;
        public int numRays2;
        public int angleRawStep1 = 0;
        public int angleRawStep2 = 0;
        public double angleDegreesStep1 = 0.0d;
        public double angleDegreesStep2 = 0.0d;

        public void registerAnglesRaw(int angleRaw1, int angleRaw2)
        {
            rawAngle1_Min = Math.Min(rawAngle1_Min, angleRaw1);
            rawAngle1_Max = Math.Max(rawAngle1_Max, angleRaw1);

            rawAngle2_Min = Math.Min(rawAngle2_Min, angleRaw2);
            rawAngle2_Max = Math.Max(rawAngle2_Max, angleRaw2);

            if (!rawAngle1Values.ContainsKey(angleRaw1))
            {
                rawAngle1Values.Add(angleRaw1, angleRaw1);
            }

            if (!rawAngle2Values.ContainsKey(angleRaw2))
            {
                rawAngle2Values.Add(angleRaw2, angleRaw2);
            }
        }

        public void finishPrerun()
        {
            numRays1 = rawAngle1Values.Count;
            numRays2 = rawAngle2Values.Count;

            angleRawStep1 = (int)Math.Round((double)(rawAngle1_Max - rawAngle1_Min) / (double)numRays1);
            angleRawStep2 = (int)Math.Round((double)(rawAngle2_Max - rawAngle2_Min) / (double)numRays2);

            angleDegreesStep1 = (angleDegrees1_Max - angleDegrees1_Min) / numRays1;
            angleDegreesStep2 = (angleDegrees2_Max - angleDegrees2_Min) / numRays2;
        }

        public override string ToString()
        {
            StringBuilder sBuf = new StringBuilder();

            sBuf.Append("numRays1=" + numRays1 + "  angleRawStep1=" + angleRawStep1 + "  rawAngle1_Min=" + rawAngle1_Min + "  rawAngle1_Max=" + rawAngle1_Max + "  angleDegreesStep1=" + angleDegreesStep1 + "\r\nsweep angles1 (us) : ");

            for (int count = 0; count < rawAngle1Values.Count; count++)
            {
                //  Display bytes as 2-character Hex strings.
                sBuf.AppendFormat("{0} ", rawAngle1Values.ElementAt(count).Key);
            }

            sBuf.Append("\r\nnumRays2=" + numRays2 + "  angleRawStep2=" + angleRawStep2 + "  rawAngle2_Min=" + rawAngle2_Min + "  rawAngle2_Max=" + rawAngle2_Max + "  angleDegreesStep2=" + angleDegreesStep2 + "\r\nsweep angles2 (us) : ");

            for (int count = 0; count < rawAngle2Values.Count; count++)
            {
                //  Display bytes as 2-character Hex strings.
                sBuf.AppendFormat("{0} ", rawAngle2Values.ElementAt(count).Key);
            }

            return sBuf.ToString();
        }

        internal static double servoTargetToMks(int servotarget)
        {
            return Math.Round((double)(SERVO_MKS_MIN + (servotarget - SERVO_VALUE_MIN) * (SERVO_MKS_MAX - SERVO_MKS_MIN) / (SERVO_VALUE_MAX - SERVO_VALUE_MIN)));
        }

        internal static int mksToServoTarget(double dPosMks)
        {
            return (int)Math.Round(SERVO_VALUE_MIN + (SERVO_VALUE_MAX - SERVO_VALUE_MIN) * (dPosMks - SERVO_MKS_MIN) / (SERVO_MKS_MAX - SERVO_MKS_MIN));
        }

        internal static double pingValueToDistanceM(int pingResponseValue)
        {
            return Math.Max(0.0d, Math.Round((double)(pingResponseValue - 1300.0d) * 1320.0d / (12500.0d - 1300.0d))) / 1000.0d;
        }

        internal static double toWithinServoRangeMks(double dPosMks)
        {
            return Math.Min(Math.Max(dPosMks, SERVO_MKS_MIN), SERVO_MKS_MAX);        // cut it to the allowed limits
        }


        // =================================================================================================================


        private static int angleRawMin = 1200; //883;
        private static int angleRawMax = 1500; //3350;
        private double angleSweep = 180.0d;
        private double angleSweepCombo = 90.0d;     // each servo sweeps this angle
        private double angleAdjust = 90.0d / 26.0d;

        public RangeReading angleRawToSonarAngle(RangeReading rr)
        {
            if (rr.angleRaw < angleRawMin)
            {
                angleRawMin = rr.angleRaw;
            }
            else if (rr.angleRaw > angleRawMax)
            {
                angleRawMax = rr.angleRaw;
            }

            rr.angleDegrees = (rr.angleRaw - angleRawMin) * angleSweep / (angleRawMax - angleRawMin);

            return rr;
        }

        public RangeReading angleRawToSonarAngleCombo(int channel, int angleRaw, double distM, long timestamp)
        {
            double angleDegrees = 0.0d;

            // these formulas are very dependent on the positioning of the servos/sonars and type of servos. 
            // for my particular robot servos are located upside down, the shaft facing the ground. EXI 227F servos are fast and dirt cheap, but the metal gears have generally short life compared to new Hitec carbon gears.
            // see CommLink::OnMeasurement() for the way the sonar data should look like, and how it will be converted to a 180 degrees sweep.
            switch (channel)
            {
                case 1:     // channel 1 is on the left
                    angleDegrees = (angleRaw - angleRawMin) * angleSweepCombo / (angleRawMax - angleRawMin) - angleAdjust;
                    // ret = rawAngle
                    // ret = rawAngle1_Max + rawAngle2_Max + 99 - rawAngle;                     // Futaba S3003 left, upside down, and Hitec HS300 right,  upside down
                    angleRaw = rawAngle1_Max + rawAngle2_Max + 132 - angleRaw;            // Two EXI 227F upside down
                    break;
                case 2:     // channel 2 is on the right
                    angleDegrees = (angleRaw - angleRawMin) * angleSweepCombo / (angleRawMax - angleRawMin) + angleSweepCombo + angleAdjust;
                    // ret = rawAngle2_Max - (rawAngle - rawAngle2_Min) + rawAngle1_Max + 99;
                    // ret = rawAngle;
                    angleRaw = rawAngle1_Min + (rawAngle1_Max - angleRaw) - 99;           // Two EXI 227F upside down
                    break;
            }

            RangeReading rr = new RangeReading(angleRaw, distM, timestamp);
            rr.angleDegrees = angleDegrees;

            return rr;
        }

        /// <summary>
        /// converts byte 0...255 to range -2G...2G (1G=64; 2G=127)
        /// </summary>
        /// <param name="accB"></param>
        /// <returns></returns>
        internal static double toAccel(byte accB)
        {
            double ret = accB;

            if (ret > 127.0d)
            {
                ret -= 256.0d;
            }

            return ret / 64.0d;
        }

    }
}
