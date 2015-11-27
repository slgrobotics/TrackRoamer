using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrackRoamer.Robotics.LibMapping
{
    public class Projection
    {
        private Projection() { /* No instances allowed */ }

        public static int LonLatPtToZone(LonLatPt lonlat)
        {
            return ((int)((lonlat.Lon + 180) / 6 + 1));
        }

        private static double e0fn(double x)
        {
            return (1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x)));
        }

        private static double e1fn(double x)
        {
            return (0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x)));
        }

        private static double e2fn(double x)
        {
            return (0.05859375 * x * x * (1.0 + 0.75 * x));
        }

        private static double e3fn(double x)
        {
            return (x * x * x * (35.0 / 3072.0));
        }

        private static double mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return (e0 * phi - e1 * Math.Sin(2.0 * phi) + e2 * Math.Sin(4.0 * phi) - e3 * Math.Sin(6.0 * phi));
        }

        public static double AdjustLon(double x)
        {
            // Check to see if x is already OK
            if (Math.Abs(x) <= Math.PI)
            {
                return x;
            }
            x = x - Math.Sign(x) * (int)(x / (2 * Math.PI)) * 2 * Math.PI;
            if (x > Math.PI)
            {
                x -= 2 * Math.PI;
            }
            return x;
        }

        public static UtmPt LonLatPtToUtmNad83Pt(LonLatPt lonlat, int zone)
        {
            double rMajor = 6378137.0;
            double rMinor = 6356752.3142450;
            double scaleFactor = .9996;
            double degreesToRadians = 1.745329251994328e-2;
            double factor = degreesToRadians;
            double latOrigin = 0;
            double lonCenter = ((6 * Math.Abs(zone)) - 183) * degreesToRadians;
            //Debug.WriteLine("lonCenter = " + lonCenter);
            double falseEasting = 500000;
            double falseNorthing = (zone < 0) ? 10000000 : 0;
            //Debug.WriteLine(String.Format("rMajor = {0}\n rMinor = {1}\n scaleFactor = {2}\n degreesToRadians = {3}\n latOrigin = {4}\n lonCenter = {5}\n falseEasting = {6}\n falseNorthing = {7}",
            //	new Object[] {rMajor, rMinor, scaleFactor, degreesToRadians, latOrigin, lonCenter, falseEasting, falseNorthing}));

            double temp = rMinor / rMajor;
            //Debug.WriteLine("temp = " + temp);

            double es = 1 - (temp * temp);
            double e = Math.Sqrt(es);
            double e0 = e0fn(es);
            double e1 = e1fn(es);
            double e2 = e2fn(es);
            double e3 = e3fn(es);
            double ml0 = rMajor * mlfn(e0, e1, e2, e3, latOrigin);
            double esp = es / (1 - es);
            double ind = (es < .0001) ? 1 : 0;
            //		double radius = 6370997;	// radius of earth in meters
            //Debug.WriteLine(String.Format("es = {0}\n e = {1}\n e0 = {2}\n e1 = {3}\n e2 = {4}\n e3 = {5}\n ml0 = {6}\n esp = {7}\n ind = {8}",
            //	new Object[] {es, e, e0, e1, e2, e3, ml0, esp, ind}));
            //Debug.WriteLine("");

            double longitude = lonlat.Lon * factor;
            double latitude = lonlat.Lat * factor;
            double deltaLon = AdjustLon(longitude - lonCenter);
            double sinPhi = Math.Sin(latitude);
            double cosPhi = Math.Cos(latitude);
            double al = cosPhi * deltaLon;
            double als = al * al;
            double c = esp * cosPhi * cosPhi;
            double tq = Math.Tan(latitude);
            double t = tq * tq;
            double con = 1.0 - es * sinPhi * sinPhi;
            double n = rMajor / Math.Sqrt(con);
            double ml = rMajor * mlfn(e0, e1, e2, e3, latitude);
            //Debug.WriteLine(String.Format("logitude = {0}\n latitude = {1}\n delaLon = {2}\n sinPhi = {3}\n cosPhi = {4}\n al = {5}\n als = {6}\n c = {7}\n tq = {8}\n t = {9}\n con = {10}\n n = {11}\n ml = {12}",
            //	new Object[]{longitude, latitude, deltaLon, sinPhi, cosPhi, al, als, c, tq, t, con, n, ml}));

            UtmPt utmpt = new UtmPt();
            utmpt.X = scaleFactor * n * al * (1.0 + als / 6.0 *
                (1.0 - t + c + als / 20.0 * (5.0 - 18.0 * t + (t * t) + 72.0 * c - 58.0 * esp))) + falseEasting;

            utmpt.Y = scaleFactor * (ml - ml0 + n * tq * (als * (0.5 + als / 24.0 *
                (5.0 - t + 9.0 * c + 4.0 * (c * c) + als / 30.0 *
                (61.0 - 58.0 * t + (t * t) + 600.0 * c - 330.0 * esp))))) + falseNorthing;
            utmpt.Zone = zone;
            return (utmpt);
        }


        public static UtmPt LonLatPtToUtmNad83Pt(LonLatPt lonlat)
        {
            int zone = LonLatPtToZone(lonlat);
            return (LonLatPtToUtmNad83Pt(lonlat, zone));
        }

        public static LonLatPt UtmNad83PtToLonLatPt(UtmPt u)
        {
            // make a local copy so that we can change values there:
            UtmPt utmpt = new UtmPt();
            utmpt.X = u.X;
            utmpt.Y = u.Y;
            utmpt.Zone = u.Zone;

            double rMajor = 6378137.0;
            double rMinor = 6356752.3142450;
            double latOrigin = 0;
            double degreesToRadians = 1.745329251994328e-2;
            double factor = 57.29577951308231;
            double lonCenter = ((6 * Math.Abs(utmpt.Zone)) - 183) * degreesToRadians;
            double falseEasting = 500000;
            double falseNorthing = (utmpt.Zone < 0) ? 10000000 : 0;
            double scaleFactor = .9996;
            double temp = rMinor / rMajor;
            //Debug.WriteLine("temp = " + temp);

            double es = 1 - (temp * temp);
            double e = Math.Sqrt(es);
            double e0 = e0fn(es);
            double e1 = e1fn(es);
            double e2 = e2fn(es);
            double e3 = e3fn(es);
            double ml0 = rMajor * mlfn(e0, e1, e2, e3, latOrigin);
            double esp = es / (1 - es);
            double ind = (es < .0001) ? 1 : 0;
            //Debug.WriteLine(String.Format("es = {0}\n e = {1}\n e0 = {2}\n e1 = {3}\n e2 = {4}\n e3 = {5}\n ml0 = {6}\n esp = {7}\n ind = {8}",
            //	new Object[] {es, e, e0, e1, e2, e3, ml0, esp, ind}));
            //Debug.WriteLine("");
            int maxIterations = 6;
            utmpt.X -= falseEasting;
            utmpt.Y -= falseNorthing;
            double con = (ml0 + utmpt.Y / scaleFactor) / rMajor;
            double phi = con;
            for (int i = 0; ; i++)
            {
                double delta_phi = ((con + e1 * Math.Sin(2.0 * phi) - e2 * Math.Sin(4.0 * phi) + e3 * Math.Sin(6.0 * phi))
                    / e0) - phi;
                phi += delta_phi;
                if (Math.Abs(delta_phi) <= 1.0e-10) break;
                if (i >= maxIterations)
                {
                    throw new Exception("Latitude failed to converge");
                }
            }
            LonLatPt lonlat = new LonLatPt();
            if (Math.Abs(phi) < (Math.PI / 2))
            {
                double sinPhi = Math.Sin(phi);
                double cosPhi = Math.Cos(phi);
                double tanPhi = Math.Tan(phi);
                double c = esp * (cosPhi * cosPhi);
                double cs = (c * c);
                double t = (tanPhi * tanPhi);
                double ts = (t * t);
                con = 1.0 - es * (sinPhi * sinPhi);
                double n = rMajor / Math.Sqrt(con);
                double r = n * (1.0 - es) / con;
                double d = utmpt.X / (n * scaleFactor);
                double ds = (d * d);
                lonlat.Lat = phi - (n * tanPhi * ds / r) * (0.5 - ds / 24.0 * (5.0 + 3.0 * t +
                    10.0 * c - 4.0 * cs - 9.0 * esp - ds / 30.0 * (61.0 + 90.0 * t +
                    298.0 * c + 45.0 * ts - 252.0 * esp - 3.0 * cs)));

                lonlat.Lon = AdjustLon(lonCenter + (d * (1.0 - ds / 6.0 * (1.0 + 2.0 * t +
                    c - ds / 20.0 * (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs + 8.0 * esp +
                    24.0 * ts))) / cosPhi));
            }
            else
            {
                lonlat.Lat = (Math.PI / 2) * Math.Sign(utmpt.Y);
                lonlat.Lon = lonCenter;
            }
            lonlat.Lon *= factor;
            lonlat.Lat *= factor;
            return (lonlat);
        }

        public static UtmPt LonLatPtToUtmNad27Pt(LonLatPt lonlat, int zone)
        {
            double rMajor = 6378137.0;
            double rMinor = 6356752.3142450;
            double scaleFactor = .9996;
            double degreesToRadians = 1.745329251994328e-2;
            double factor = degreesToRadians;
            double latOrigin = 0;
            double lonCenter = ((6 * Math.Abs(zone)) - 183) * degreesToRadians;
            //Debug.WriteLine("lonCenter = " + lonCenter);
            double falseEasting = 500000;
            double falseNorthing = (zone < 0) ? 10000000 : 0;
            //Debug.WriteLine(String.Format("rMajor = {0}\n rMinor = {1}\n scaleFactor = {2}\n degreesToRadians = {3}\n latOrigin = {4}\n lonCenter = {5}\n falseEasting = {6}\n falseNorthing = {7}",
            //	new Object[] {rMajor, rMinor, scaleFactor, degreesToRadians, latOrigin, lonCenter, falseEasting, falseNorthing}));

            double temp = rMinor / rMajor;
            //Debug.WriteLine("temp = " + temp);

            double es = 1 - (temp * temp);
            double e = Math.Sqrt(es);
            double e0 = e0fn(es);
            double e1 = e1fn(es);
            double e2 = e2fn(es);
            double e3 = e3fn(es);
            double ml0 = rMajor * mlfn(e0, e1, e2, e3, latOrigin);
            double esp = es / (1 - es);
            double ind = (es < .0001) ? 1 : 0;
            //		double radius = 6370997;	// radius of earth in meters
            //Debug.WriteLine(String.Format("es = {0}\n e = {1}\n e0 = {2}\n e1 = {3}\n e2 = {4}\n e3 = {5}\n ml0 = {6}\n esp = {7}\n ind = {8}",
            //	new Object[] {es, e, e0, e1, e2, e3, ml0, esp, ind}));
            //Debug.WriteLine("");

            double longitude = lonlat.Lon * factor;
            double latitude = lonlat.Lat * factor;
            double deltaLon = AdjustLon(longitude - lonCenter);
            double sinPhi = Math.Sin(latitude);
            double cosPhi = Math.Cos(latitude);
            double al = cosPhi * deltaLon;
            double als = al * al;
            double c = esp * cosPhi * cosPhi;
            double tq = Math.Tan(latitude);
            double t = tq * tq;
            double con = 1.0 - es * sinPhi * sinPhi;
            double n = rMajor / Math.Sqrt(con);
            double ml = rMajor * mlfn(e0, e1, e2, e3, latitude);
            //Debug.WriteLine(String.Format("logitude = {0}\n latitude = {1}\n delaLon = {2}\n sinPhi = {3}\n cosPhi = {4}\n al = {5}\n als = {6}\n c = {7}\n tq = {8}\n t = {9}\n con = {10}\n n = {11}\n ml = {12}",
            //	new Object[]{longitude, latitude, deltaLon, sinPhi, cosPhi, al, als, c, tq, t, con, n, ml}));

            UtmPt utmpt = new UtmPt();
            utmpt.X = scaleFactor * n * al * (1.0 + als / 6.0 *
                (1.0 - t + c + als / 20.0 * (5.0 - 18.0 * t + (t * t) + 72.0 * c - 58.0 * esp))) + falseEasting;

            utmpt.Y = scaleFactor * (ml - ml0 + n * tq * (als * (0.5 + als / 24.0 *
                (5.0 - t + 9.0 * c + 4.0 * (c * c) + als / 30.0 *
                (61.0 - 58.0 * t + (t * t) + 600.0 * c - 330.0 * esp))))) + falseNorthing;
            utmpt.Zone = zone;

            return (utmpt);
        }

        public static UtmPt LonLatPtToUtmNad27Pt(LonLatPt lonlat)
        {
            int zone = LonLatPtToZone(lonlat);
            return (LonLatPtToUtmNad27Pt(lonlat, zone));
        }

        //public static LonLatPt UtmNad27PtToLonLatPt(UtmPt utmpt) {
        //	return(new Lon
        //}
    }

    /// <summary>
    /// minimal container for geo coordinates
    /// </summary>
    public class LonLatPt
    {
        public double Lon { get; set; }

        public double Lat { get; set; }
    }

    /// <summary>
    /// minimal container for UTM coordinates
    /// </summary>
    public class UtmPt
    {
        public int Zone { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
