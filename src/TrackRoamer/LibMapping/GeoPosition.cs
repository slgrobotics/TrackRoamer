using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    public class GeoPosition : ICloneable
    {
        public const double EARTH_RADIUS = 6371000.0d;		// meters

        private double m_X;
        public double Lng { get { return m_X; } set { m_X = value; } }
        public double X { get { return m_X; } set { m_X = value; } }

        private double m_Y;
        public double Lat { get { return m_Y; } set { m_Y = value; } }
        public double Y { get { return m_Y; } set { m_Y = value; } }

        private double m_H;	// elevation, meters
        public double Elev { get { return m_H; } set { m_H = value; } }
        public double H { get { return m_H; } set { m_H = value; } }
        public double Z { get { return m_H; } set { m_H = value; } }

        public long TimeStamp = 0L;

        public Distance distanceFrom(GeoPosition from)
        {
            // here is a version that is lighter computationally and works well over miles of distance.
            // compared to distanceFromExact() the difference is 1mm per meter (0.1%)

            double x = m_X - from.X;            //double x = this.subtract(from, false).m_X;
            double y = m_Y - from.Y;            //double y = this.subtract(from, false).m_Y;

            // a grad square is cos(latitude) thinner, we need latitude in radians:
            double midLatRad = ((from.Y + m_Y) / 2.0d) * Math.PI / 180.0d;            //double midLatRad = (this.add(from).m_Y / 2.0d) * Math.PI / 180.0d;
            double latitudeFactor = Math.Cos(midLatRad);
            double xMeters = Distance.METERS_PER_DEGREE * x * latitudeFactor;
            double yMeters = Distance.METERS_PER_DEGREE * y;
            double meters = Math.Sqrt(xMeters * xMeters + yMeters * yMeters);

            Distance distance = new Distance(meters);

            return distance;
        }

        public Distance distanceFromExact(GeoPosition from)
        {
            // from http://www.movable-type.co.uk/scripts/LatLong.html

            double lon1 = this.Lng * Math.PI / 180.0d;
            double lon2 = from.Lng * Math.PI / 180.0d;
            double lat1 = this.Lat * Math.PI / 180.0d;
            double lat2 = from.Lat * Math.PI / 180.0d;

            double dLat = lat2 - lat1;
            double dLong = lon2 - lon1;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2.0d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0d - a));
            double meters = EARTH_RADIUS * c;

            Distance distance = new Distance(meters);

            return distance;
        }


		public GeoPosition(double lng, double lat)
		{
			m_X = lng;
			m_Y = lat;
			m_H = 0.0d;
		}

		public GeoPosition(double lng, double lat, double elev)
		{
			m_X = lng;
			m_Y = lat;
			m_H = elev;
		}

		public GeoPosition(GeoPosition loc)
		{
			m_X = loc.Lng;
			m_Y = loc.Lat;
			m_H = loc.Elev;
		}

		public 	GeoPosition(string sPos)	// sPos in form: W117.23'45" N36.44'48" or N33,27.661 W117,42.945
		{
			m_X = 0;
			m_Y = 0;
			m_H = 0;
			bool west  = sPos.IndexOf('W') != -1;
			bool north = sPos.IndexOf('N') != -1;
			int spaceIndex = sPos.IndexOf(' ');
			if(spaceIndex > 0) 
			{
				if(sPos.StartsWith("N") || sPos.StartsWith("S"))
				{
					string sLatitude = sPos.Substring(1, spaceIndex-1);
					m_Y = toDegree(sLatitude);
					if(!north) 
					{
						m_Y *= -1;
					}
					string sLongitude = sPos.Substring(spaceIndex + 2);
					m_X = toDegree(sLongitude);
					if(west) 
					{
						m_X *= -1;
					}
				}
				else
				{
					string sLongitude = sPos.Substring(1, spaceIndex-1);
					m_X = toDegree(sLongitude);
					if(west) 
					{
						m_X *= -1;
					}
					string sLatitude = sPos.Substring(spaceIndex + 2);
					m_Y = toDegree(sLatitude);
					if(!north) 
					{
						m_Y *= -1;
					}
				}
			}
			// System.Console.WriteLine("BACK: " + toString());
		}

		protected double toDegree(string str)	// str in form: 117.23'45 or 117,42.945
		{
			// System.Console.WriteLine("TO DEGREE: |" + str + "|");
			int i = 0;
			if(str.IndexOf(",") > 0)
			{
				int j = str.IndexOf(',');
				string val = str.Substring(0, j);
				// System.Console.WriteLine("val: |" + val + "|");
				int deg = Convert.ToInt32(val);
				i = j + 1;
				val = str.Substring(i);
				// System.Console.WriteLine("val: |" + val + "|");
				double min = Convert.ToDouble(val);
				return ((double)deg) + min / 60;
			}
			else
			{
				int j = str.IndexOf('.');
				string val = str.Substring(0, j);
				// System.Console.WriteLine("val: |" + val + "|");
				int deg = Convert.ToInt32(val);
				i = j + 1;
				j = str.IndexOf('\'');
				val = str.Substring(i, j-i);
				// System.Console.WriteLine("val: |" + val + "|");
				int min = Convert.ToInt32(val);
				i = j + 1;
				val = str.Substring(i).Replace("\"", "");
				// System.Console.WriteLine("val: |" + val + "|");
				int sec = Convert.ToInt32(val);
				return ((double)deg) + ((double)min) / 60 + ((double)sec) / 3600;
			}
		}

		public void Normalize()
		{
			while(m_X >= 180.0d) 
			{
				m_X -= 360.0d;
			}

			while(m_X < -180.0d)
			{
				m_X += 360.0d;
			}
		}
    
		public void	moveTo(double x, double y, double h)
		{
			m_X = x;
			m_Y = y;
			m_H = h;
		}

        public void moveTo(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }

        public void moveTo(GeoPosition to)
		{
			m_X = to.X;
			m_Y = to.Y;
			m_H = to.H;
		}

        // works only for small distances, within miles.
        public void translate(Distance byX, Distance byY)
		{
            m_X += toDegreesX(byX);
            m_Y += toDegreesY(byY);
		}

        // works only for small distances, within miles. For general case - when bearing is missing, or when we need to move towards bearing.
		public void	translate(Direction dir, Distance by)
		{
            double range = by.Meters;
            double angle = (double)dir.heading; // degrees

            if (dir.bearing.HasValue)
            {
                // if both heading and bearing are supplied, then heading represents robot direction, and bearing - absolute direction to the object.
                // we are interested in translating in the "bearing" direction in this case, it is done to the objects relative to the robot.
                angle = (double)dir.bearing;
            }

            m_X += toDegreesX(new Distance(range * Math.Sin(angle * Math.PI / 180.0d)));
            m_Y += toDegreesY(new Distance(range * Math.Cos(angle * Math.PI / 180.0d)));
		}

        // works only for small distances, within miles. For rare cases when we have both heading and bearing, but want to move in the "heading" direction.
        public void translateToHeading(Direction dir, Distance by)
		{
            double range = by.Meters;
            double angle = (double)dir.heading; // degrees

            m_X += toDegreesX(new Distance(range * Math.Sin(angle * Math.PI / 180.0d)));
            m_Y += toDegreesY(new Distance(range * Math.Cos(angle * Math.PI / 180.0d)));
		}

        // converts distance along the latitude line (X) into degrees
        public double toDegreesX(Distance d)
        {
            // horizontal degrees are shorter in meters as we go up the latitude:
            double latitudeFactor = Math.Cos(this.Lat * Math.PI / 180.0d);

            return d.Meters / Distance.METERS_PER_DEGREE / latitudeFactor;
        }

        // converts distance along the longitude line (Y) into degrees
        public double toDegreesY(Distance d)
        {
            // vertical degrees are the same always:
            return d.Meters / Distance.METERS_PER_DEGREE;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

		public override bool Equals(object obj)
		{
			if(obj == null) 
			{
				return false;
			}
			GeoPosition other = (GeoPosition)obj;
			//other.normalize();
			//normalize();
			return other.Lat == m_Y && other.Lng == m_X && other.Elev == m_H;
		}

		public bool sameAs(object obj)
		{
			if(obj == null) 
			{
				return false;
			}
			GeoPosition other = (GeoPosition)obj;
			other.Normalize();
			Normalize();
			return other.Lat == m_Y && other.Lng == m_X; // && other.Elev == m_H;
		}

		// tolerance 0.0001 degree = 10 meters
		public bool almostAs(object obj, double tolerance)
		{
			if(obj == null) 
			{
				return false;
			}
			GeoPosition other = (GeoPosition)obj;
			other.Normalize();
			Normalize();
			return Math.Abs(other.Lat - m_Y) <= tolerance && Math.Abs(other.Lng - m_X) <= tolerance; // && other.Elev == m_H;
		}

		public 	GeoPosition(double x, double xmin, double xsec,
			double y, double ymin, double ysec)
		{
			m_X = x + xmin/60 + xsec/3600;
			m_Y = y + ymin/60 + ysec/3600;
		}

		// returns bearing in degrees. To get rads, multiply by Math.PI / 180.0d
		public double bearing(GeoPosition nextLoc)
		{
			double Lon1 = this.Lng * Math.PI / 180.0d;
			double Lon2 = nextLoc.Lng * Math.PI / 180.0d;
			double Lat1 = this.Lat * Math.PI / 180.0d;
			double Lat2 = nextLoc.Lat * Math.PI / 180.0d;

			double y = Math.Sin(Lon1-Lon2) * Math.Cos(Lat2);
			double x = Math.Cos(Lat1) * Math.Sin(Lat2) - Math.Sin(Lat1) * Math.Cos(Lat2) * Math.Cos(Lon1 - Lon2);

			// from http://www.movable-type.co.uk/scripts/LatLong.html
			if (Math.Sin(Lon2 - Lon1) > 0.0)
			{
                return toDegrees(Math.Atan2(-y, x));
			} 
			else
			{
                return toDegrees(2.0d * Math.PI - Math.Atan2(y, x));
			}

            /*
            // see http://www.malaysiagis.com/related_technologies/gps/article3.cfm for the formula and some code
            // see http://www.fcaglp.unlp.edu.ar/~esuarez/gmt/1997/0148.html for more
            double ret = 0.0d;

            double rad_bearing;

            double rad_dist = Math.Acos(Math.Sin(Lat1) * Math.Sin(Lat2) + Math.Cos(Lat1) * Math.Cos(Lat2) * Math.Cos(Lon1 - Lon2));

            if (Math.Sin(Lon2 - Lon1) > 0.0)
            {
                double t1 = Math.Sin(Lat2) - Math.Sin(Lat1) * Math.Cos(rad_dist);
                double t2 = Math.Cos(Lat1) * Math.Sin(rad_dist);
                double t3 = t1 / t2;
                double t4 = Math.Atan(-t3 / Math.Sqrt(-t3 * t3 + 1)) + 2 * Math.Atan(1);
                rad_bearing = t4;
            }
            else
            {
                double t1 = Math.Sin(Lat2) - Math.Sin(Lat1) * Math.Cos(rad_dist);
                double t2 = Math.Cos(Lat1) * Math.Sin(rad_dist);
                double t3 = t1 / t2;
                double t4 = -t3 * t3 + 1;
                double t5 = 2.0d * Math.PI - (Math.Atan(-t3 / Math.Sqrt(-t3 * t3 + 1)) + 2 * Math.Atan(1));
                rad_bearing = t5;
            }

            ret = toDegrees(rad_bearing);

            return ret;
            */
        }

		public double magneticVariation()
		{
			// see http://www.csgnetwork.com/e6bcalc.html   - US Continental only

			double lat = m_Y;
			double lon = -m_X;

			double v = -65.6811 + .99 * lat + .0128899 * Math.Pow(lat, 2) - .0000905928 *

				Math.Pow(lat, 3) + 2.87622 * lon - .0116268 * lat * lon - .00000603925 *

				Math.Pow(lat, 2) * lon - .0389806 * Math.Pow(lon, 2) - .0000403488 *

				lat * Math.Pow(lon, 2) + .000168556 * Math.Pow(lon, 3);

			return v;
		}

		public GeoPosition add(GeoPosition a)
		{
			return new GeoPosition(m_X + a.X, m_Y + a.Y, m_H + a.H);
		}

		public GeoPosition subtract(GeoPosition a, bool spans180)
		{
			double x = a.X;
			double dx = m_X - x;
			if(spans180) 
			{     // dx < 360.0 && Math.Abs(dx) > 180.0) {    
				if(x > 90.0 && m_X < -90) 
				{
					x -= 360.0;
				} 
				else if(m_X > 90.0 && x < -90) 
				{
					x += 360.0;
				}
				dx = m_X - x;
			}
			double dy = m_Y - a.Y;
			double dz = m_H - a.H;
			return new GeoPosition(dx, dy, dz);
		}

        #region ICloneable Members

        public object Clone()
        {
            // return new GeoPosition(m_X, m_Y, m_H);
            return this.MemberwiseClone();  // shallow copy, only value types are cloned
        }

        #endregion // ICloneable Members

		public override string ToString()
		{
			return toString0(false, MapperSettings.coordStyle);
		}

		public string ToStringWithElev()
		{
			return toString0(true, MapperSettings.coordStyle);
		}

		public string toString0(bool doHeight, int coordStyle)
		{
			if(m_X > 180.0 || m_X < -180.0 || m_Y > 90.0 || m_Y < -90.0) 
			{
				return "---";
			}

			double dAbsX = Math.Abs(m_X);
			double dFloorX = Math.Floor(dAbsX);
			double dMinX = (dAbsX - dFloorX) * 60.0;
			long degX = (long)Math.Round(dFloorX);
			long minX = (long)Math.Round(Math.Floor(dMinX));
			long secX = (long)Math.Round(Math.Floor((dMinX - (double)minX) * 60.0));

			double dAbsY = Math.Abs(m_Y);
			double dFloorY = Math.Floor(dAbsY);
			double dMinY = (dAbsY - dFloorY) * 60.0;
			long degY = (long)Math.Round(dFloorY);
			long minY = (long)Math.Round(Math.Floor(dMinY));
			long secY = (long)Math.Round(Math.Floor((dMinY - (double)minY) * 60.0));

			string height = doHeight ? heightToString() : "";

			switch (coordStyle)
			{
				default:
				case 0:
				{
					string mxfiller = minX > 9 ? "°" : "°0";
					string myfiller = minY > 9 ? "°" : "°0";
					string sxfiller = secX > 9 ? "'" : "'0";
					string syfiller = secY > 9 ? "'" : "'0";
					return (m_Y > 0.0 ? "N" : "S") + degY
						+ myfiller + minY + syfiller + secY + "\""
						+ (m_X > 0.0 ? "  E" : "  W") + degX
						+ mxfiller + minX + sxfiller + secX + "\""
						+ height;
				}
				case 1:
				{
					double drMinX = Math.Round(dMinX, 5);
					double drMinY = Math.Round(dMinY, 5);
					string mxfiller = drMinX >= 10.0d ? "°" : "°0";
					string myfiller = drMinY >= 10.0d ? "°" : "°0";
					return (m_Y > 0.0 ? "N" : "S") + degY
						+ myfiller + string.Format("{0:F3}'", drMinY)
						+ (m_X > 0.0 ? "  E" : "  W") + degX
						+ mxfiller + string.Format("{0:F3}'", drMinX)
						+ height;
				}
				case 2:
					return (m_Y > 0.0 ? "N" : "S") + string.Format("{0:F6}", Math.Abs(m_Y))
						+ (m_X > 0.0 ? "  E" : "  W") + string.Format("{0:F6}", Math.Abs(m_X))
						+ height;
				case 3:			// UTM = 11S E433603 N3778359
					// requires LibNet, which would cause circular dependency:
					return toUtmString(m_X, m_Y, m_H) + height;
			}
		}

		// same as toString0() but dir letters after coords
		public string toString00(bool doHeight, int coordStyle)
		{
			if(m_X > 180.0 || m_X < -180.0 || m_Y > 90.0 || m_Y < -90.0) 
			{
				return "---";
			}

			double dAbsX = Math.Abs(m_X);
			double dFloorX = Math.Floor(dAbsX);
			double dMinX = (dAbsX - dFloorX) * 60.0;
			long degX = (long)Math.Round(dFloorX);
			long minX = (long)Math.Round(Math.Floor(dMinX));
			long secX = (long)Math.Round(Math.Floor((dMinX - (double)minX) * 60.0));

			double dAbsY = Math.Abs(m_Y);
			double dFloorY = Math.Floor(dAbsY);
			double dMinY = (dAbsY - dFloorY) * 60.0;
			long degY = (long)Math.Round(dFloorY);
			long minY = (long)Math.Round(Math.Floor(dMinY));
			long secY = (long)Math.Round(Math.Floor((dMinY - (double)minY) * 60.0));

			string height = doHeight ? heightToString() : "";

			switch (coordStyle)
			{
				default:
				case 0:
				{
					string mxfiller = minX > 9 ? "°" : "°0";
					string myfiller = minY > 9 ? "°" : "°0";
					string sxfiller = secX > 9 ? "'" : "'0";
					string syfiller = secY > 9 ? "'" : "'0";
					return ("" + degY
						+ myfiller + minY + syfiller + secY + "\"" + (m_Y > 0.0 ? " N  " : " S  ")
						+ degX
						+ mxfiller + minX + sxfiller + secX + "\"" + (m_X > 0.0 ? " E  " : " W  ") 
						+ height).Trim();
				}
				case 1:
				{
					double drMinX = Math.Round(dMinX, 5);
					double drMinY = Math.Round(dMinY, 5);
					string mxfiller = drMinX >= 10.0d ? "°" : "°0";
					string myfiller = drMinY >= 10.0d ? "°" : "°0";
					return ("" + degY
						+ myfiller + string.Format("{0:F3}'", drMinY) + (m_Y > 0.0 ? " N  " : " S  ")
						+ degX
						+ mxfiller + string.Format("{0:F3}'", drMinX) + (m_X > 0.0 ? " E  " : " W  ")
						+ height).Trim();
				}
				case 2:
					return (string.Format("{0:F6}", Math.Abs(m_Y)) + (m_Y > 0.0 ? " N  " : " S  ")
						+ string.Format("{0:F6}", Math.Abs(m_X)) + (m_X > 0.0 ? " E  " : " W  ")
						+ height).Trim();
				case 3:			// UTM = 11S E433603 N3778359
					// requires LibNet, which would cause circular dependency:
					return toUtmString(m_X, m_Y, m_H) + height;
			}
		}

		public string heightToString()
		{
			string height = "";
			if(m_H > 0.3d) // meters
			{
				Distance d = new Distance(m_H);
				height = " " + d.ToStringCompl() + " high";
			} 
			else if(m_H < -0.3d) // meters
			{
				Distance d = new Distance(-m_H);
				height = " " + d + " deep";
			} 
			return height;
		}

		public static string latToString(double lat, int coordStyle, bool doDegree, bool doDir)
		{
			double dAbsY = Math.Abs(lat);
			double dFloorY = Math.Floor(dAbsY);
			double dMinY = (dAbsY - dFloorY) * 60.0;
			long degY = (long)Math.Round(dFloorY);
			long minY = (long)Math.Round(Math.Floor(dMinY));
			long secY = (long)Math.Round(Math.Floor((dMinY - (double)minY) * 60.0));
			string sDir = doDir ? (lat > 0.0 ? "N" : "S") : "";

			switch (coordStyle)
			{
				default:
				case 0:
				{
					string myfiller = minY > 9 ? "" : "0";
					string syfiller = secY > 9 ? "'" : "'0";
					return sDir + (doDegree ? ("" + degY + "°") : "°")
						+ myfiller + minY + syfiller + secY + "\"";
				}
				case 3:			// UTM = 11S E433603 N3778359 -- never should call here:
				case 1:
				{
					double drMinY = Math.Round(dMinY, 5);
					string myfiller = drMinY >= 10.0d ? "" : "0";
					return sDir + (doDegree ? ("" + degY + "°") : "°") + myfiller + string.Format("{0:F3}'", drMinY);
				}
				case 2:
					return sDir + string.Format("{0:F6}", Math.Abs(lat));
//				case 3:			// UTM = 11S E433603 N3778359 -- never should call here:
//					return "YYYY";
			}
		}

		public static string lngToString(double lng, int coordStyle, bool doDegree, bool doDir)
		{
			double dAbsX = Math.Abs(lng);
			double dFloorX = Math.Floor(dAbsX);
			double dMinX = (dAbsX - dFloorX) * 60.0;
			long degX = (long)Math.Round(dFloorX);
			long minX = (long)Math.Round(Math.Floor(dMinX));
			long secX = (long)Math.Round(Math.Floor((dMinX - (double)minX) * 60.0));
			string sDir = doDir ? (lng > 0.0 ? "E" : "W") : "";

			switch (coordStyle)
			{
				default:
				case 0:
				{
					string mxfiller = minX > 9 ? "" : "0";
					string sxfiller = secX > 9 ? "'" : "'0";
					return sDir + (doDegree ? ("" + degX + "°") : "°")
						+ mxfiller + minX + sxfiller + secX + "\"";
				}
				case 3:			// UTM = 11S E433603 N3778359 -- never should call here:
				case 1:
				{
					double drMinX = Math.Round(dMinX, 5);
					string mxfiller = drMinX >= 10.0d ? "" : "0";
					return sDir + (doDegree ? ("" + degX + "°") : "°") + mxfiller + string.Format("{0:F3}'", drMinX);
				}
				case 2:
					return sDir + string.Format("{0:F6}", Math.Abs(lng));
//				case 3:			// UTM = 11S E433603 N3778359 -- never should call here:
//					return "XXXX";
			}
		}

		public static double stringLatToDouble(string sLat)
		{
			double ret = 0.0d;
			double sign = 1.0d;
			sLat = sLat.Trim();

			if(sLat.StartsWith("-") || sLat.ToLower().StartsWith("s"))
			{
				sign = -1.0d;
				sLat = sLat.Substring(1);
			}

			if(sLat.StartsWith("+") || sLat.ToLower().StartsWith("n"))
			{
				sLat = sLat.Substring(1);
			}

			if(sLat.EndsWith("\"") || sLat.EndsWith("'"))
			{
				sLat = sLat.Substring(0, sLat.Length-1);
			}

			//throw new Exception("invalid format");

			switch (MapperSettings.coordStyle)
			{
				default:
				case 0:			// N33,17'12" or N33°17'12"
				{
					int pos1 = sLat.IndexOf("°");
					int pos1a = sLat.IndexOf(",");
					if(pos1 == -1)
					{
						pos1 = pos1a;
					}
					int pos2 = sLat.IndexOf("'");
					if(pos1 == -1 || pos2 == -1)
					{
						throw new Exception("lat - invalid format - needs N33,17'12\" or N33°17'12\"");
					}
					double degrees = Convert.ToDouble(sLat.Substring(0, pos1));
					double mins = Convert.ToDouble(sLat.Substring(pos1+1, pos2-pos1-1));
					double secs = Convert.ToDouble(sLat.Substring(pos2+1));
					ret = degrees + mins / 60.0d + secs / 3600.0d;
				}
					break;
				case 1:			// N33,17.123' or N33°17.123'
				{
					int pos1 = sLat.IndexOf("°");
					int pos1a = sLat.IndexOf(",");
					if(pos1 == -1)
					{
						pos1 = pos1a;
					}
					if(pos1 == -1)
					{
						throw new Exception("lat - invalid format - needs N33,17.123' or N33°17.123'");
					}
					double degrees = Convert.ToDouble(sLat.Substring(0, pos1));
					double mins = Convert.ToDouble(sLat.Substring(pos1+1));
					ret = degrees + mins / 60.0d;
				}
					break;
				case 2:			// N33.123
					ret = Convert.ToDouble(sLat);
					break;
				case 3:			// UTM = 11S E433603 N3778359
				{
				}
					break;
			}

			return ret * sign;
		}

		public static double stringLngToDouble(string sLng)
		{
			double ret = 0.0d;
			double sign = -1.0d;	// assume west
			sLng = sLng.Trim();
			
			if(sLng.StartsWith("-") || sLng.ToLower().StartsWith("w"))
			{
				sign = -1.0d;
				sLng = sLng.Substring(1);
			}

			if(sLng.StartsWith("+") || sLng.ToLower().StartsWith("e"))
			{
				sign = 1.0d;
				sLng = sLng.Substring(1);
			}

			if(sLng.EndsWith("\"") || sLng.EndsWith("'"))
			{
				sLng = sLng.Substring(0, sLng.Length-1);
			}

			switch (MapperSettings.coordStyle)
			{
				default:
				case 0:			// W117,17'12" or W117°17'12"
				{
					int pos1 = sLng.IndexOf("°");
					int pos1a = sLng.IndexOf(",");
					if(pos1 == -1)
					{
						pos1 = pos1a;
					}
					int pos2 = sLng.IndexOf("'");
					if(pos1 == -1 || pos2 == -1)
					{
						throw new Exception("lat - invalid format - needs W117,17'12\" or W117°17'12\"");
					}
					double degrees = Convert.ToDouble(sLng.Substring(0, pos1));
					double mins = Convert.ToDouble(sLng.Substring(pos1+1, pos2-pos1-1));
					double secs = Convert.ToDouble(sLng.Substring(pos2+1));
					ret = degrees + mins / 60.0d + secs / 3600.0d;
				}
					break;
				case 1:			// W117,17.123' or W117°17.123'
				{
					int pos1 = sLng.IndexOf("°");
					int pos1a = sLng.IndexOf(",");
					if(pos1 == -1)
					{
						pos1 = pos1a;
					}
					if(pos1 == -1)
					{
						throw new Exception("lng - invalid format - needs W117,17.123' or W117°17.123'");
					}
					double degrees = Convert.ToDouble(sLng.Substring(0, pos1));
					double mins = Convert.ToDouble(sLng.Substring(pos1+1));
					ret = degrees + mins / 60.0d;

				}
					break;
				case 2:			// W117.123
					ret = Convert.ToDouble(sLng);
					break;
				case 3:			// UTM = 11S E433603 N3778359
				{
				}
					break;
			}

			return ret * sign;
		}

		// the following produces tokenizable string suitable for dialog forms.
		// the string looks like the following: "W 114 23 11 N 36 02 45 -2000"
		public string toString2()
		{
			double dAbsX = Math.Abs(m_X);
			double dFloorX = Math.Floor(dAbsX);
			double dMinX = (dAbsX - dFloorX) * 60.0;
			long degX = (long)Math.Round(dFloorX);
			long minX = (long)Math.Round(Math.Floor(dMinX));
			long secX = (long)Math.Round(Math.Floor((dMinX - (double)minX) * 60.0));

			double dAbsY = Math.Abs(m_Y);
			double dFloorY = Math.Floor(dAbsY);
			double dMinY = (dAbsY - dFloorY) * 60.0;
			long degY = (long)Math.Round(dFloorY);
			long minY = (long)Math.Round(Math.Floor(dMinY));
			long secY = (long)Math.Round(Math.Floor((dMinY - (double)minY) * 60.0));

			string mxfiller = minX > 9 ? " " : " 0";
			string myfiller = minY > 9 ? " " : " 0";
			string sxfiller = secX > 9 ? " " : " 0";
			string syfiller = secY > 9 ? " " : " 0";
			return (m_X > 0.0 ? "E " : "W ") + degX
				+ mxfiller + minX + sxfiller + secX +
				(m_Y > 0.0 ? " N " : " S ") + degY
				+ myfiller + minY + syfiller + secY +
				" " + m_H;
		}

        public string toUtmString(double lon, double lat, double elev)
        {
            string ret = "";		// UTM = 11S 0433603E 3778359N

            LonLatPt lonlat = new LonLatPt();
            lonlat.Lat = lat;
            lonlat.Lon = lon;

            UtmPt utmpt = Projection.LonLatPtToUtmNad83Pt(lonlat);

            ret = "" + utmpt.Zone + "S " + string.Format("{0:d07}", (int)Math.Round(utmpt.X)) + "E " + string.Format("{0:d07}", (int)Math.Round(utmpt.Y)) + "N";

            return ret;
        }

        public double toDegrees(double radians)
        {
            return radians * 180.0d / Math.PI;
        }
    }
}
