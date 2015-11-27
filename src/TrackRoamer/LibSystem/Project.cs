using System.Text;
using System.IO;

namespace TrackRoamer.Robotics.Utility.LibSystem
{
    public sealed class Project
    {
        public const string PROGRAM_VERSION_RELEASEDATE = "20120704";

        public static string driveSystem = @"C:\";
        public static string driveProgramInstalled = @"C:\";

        public static string LogPath = @"C:\temp";

        public const string PROGRAM_NAME_LOGICAL = "trackroamerbot";	// used for making URLs on the servers
		public const string PROGRAM_NAME_HUMAN = "TrackRoamerBot";		// used for title in the frame etc.
        public const string PROGRAM_VERSION_HUMAN = "0.5";			    // used for greeting.
		public const string WEBSITE_NAME_HUMAN = "TrackRoamer.com";	    // used for watermark printing etc.
		public const string WEBSITE_LINK_WEBSTYLE = "http://www.trackroamer.com";	// used for links etc.

        #region System Helpers

        public static void writeTextFile(string filename, string content)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                StreamWriter tw = new StreamWriter(fs);
                tw.WriteLine(content);
                tw.Close();
            }
        }

        public static Encoding xmlEncoding = Encoding.ASCII;

        public static byte[] StrToByteArray(string str)
        {
            return xmlEncoding.GetBytes(str);
        }

        public static string ByteArrayToStr(byte[] bytes)
        {
            return xmlEncoding.GetString(bytes, 0, bytes.Length);
        }

        #endregion // System Helpers
    }

    public static class Helper
    {
        public static bool HasGoodValue(this double? val)
        {
            return val.HasValue && !double.IsNaN(val.Value) && !double.IsInfinity(val.Value);
        }
    }
}
