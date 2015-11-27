using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using TrackRoamer.Robotics.LibMapping;
using TrackRoamer.Robotics.Utility.LibSystem;

namespace TrackRoamer.Robotics.LibBehavior
{
    public class Mission
    {
        private string missionFileName;
        public List<LocationWp> waypoints = new List<LocationWp>();

        public LocationWp home { get; private set; }

        public Mission()
        {
        }

        public void Init(string missionFileName)
        {
            this.missionFileName = missionFileName;

            waypoints.Clear();

            readQGC110wpfile();

            home = (from w in waypoints where w.isHome select w).FirstOrDefault();
        }

        public LocationWp nextTargetWp
        {
            get
            {
                return (from wp in waypoints
                        where !wp.isHome && (wp.waypointState == WaypointState.None || wp.waypointState == WaypointState.SelectedAsTarget) 
                        orderby wp.number
                        select wp).FirstOrDefault();
            }
        }

        /// <summary>
        /// see C:\Projects\Robotics\DIY_Drones\ArduPlane-2.40\ArduPlane-2.40\Tools\ArdupilotMegaPlanner\GCSViews\FlightPlanner.cs line 1786, 1144
        /// </summary>
        /// <param name="missionFileName"></param>
        private void readQGC110wpfile()
        {
            int wp_count = 0;
            bool error = false;
            string identLine = "QGC WPL 110";

            Tracer.Trace("IP: readQGC110wpfile()  missionFileName=" + missionFileName);

            try
            {
                using (StreamReader sr = new StreamReader(missionFileName))
                {
                    string header = sr.ReadLine();
                    if (header == null || !header.Contains(identLine))
                    {
                        Tracer.Trace("Invalid Waypoint file '" + missionFileName + "' - must contain first line '" + identLine + "'");
                        return;
                    }

                    System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

                    while (!error && !sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        // waypoints

                        if (line.StartsWith("#"))
                            continue;

                        string[] items = line.Split(new char[] { (char)'\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (items.Length <= 9)
                            continue;

                        try
                        {
                            Locationwp temp = new Locationwp();

                            temp.number = int.Parse(items[0]);

                            temp.ishome = byte.Parse(items[1]);

                            if (items[2] == "3")
                            { 
                                // normally altitute is above mean sea level (MSL), or relative to ground (AGL) when MAV_FRAME_GLOBAL_RELATIVE_ALT=3
                                temp.options = 1;
                            }
                            else
                            {
                                temp.options = 0;
                            }
                            temp.id = (byte)(int)Enum.Parse(typeof(MAV_CMD), items[3], false);
                            temp.p1 = float.Parse(items[4], cultureInfo);

                            if (temp.id == 99)
                                temp.id = 0;

                            temp.alt = (float)(double.Parse(items[10], cultureInfo));
                            temp.lat = (float)(double.Parse(items[8], cultureInfo));
                            temp.lng = (float)(double.Parse(items[9], cultureInfo));

                            temp.p2 = (float)(double.Parse(items[5], cultureInfo));
                            temp.p3 = (float)(double.Parse(items[6], cultureInfo));
                            temp.p4 = (float)(double.Parse(items[7], cultureInfo));

                            waypoints.Add(new LocationWp(temp));

                            wp_count++;

                        }
                        catch { Tracer.Error("Line invalid: " + line); }

                        if (wp_count == byte.MaxValue)
                        {
                            Tracer.Error("Too many Waypoints!!! - limited to " + byte.MaxValue);
                            break;
                        }
                    }
                }
                Tracer.Trace("OK: readQGC110wpfile()  done, waypoint count=" + waypoints.Count);
            }
            catch (Exception ex)
            {
                Tracer.Error("Can't open file: '" + missionFileName + "'" + ex.ToString());
            }
        }
    }
}
