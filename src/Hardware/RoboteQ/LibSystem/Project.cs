using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Threading;
using Microsoft.Win32;		// for registry

namespace LibSystem
{
    public sealed class Project
    {
        public const string PROGRAM_VERSION_RELEASEDATE = "20070220";

        public static string driveSystem = "C:\\";
        public static string driveProgramInstalled = "C:\\";

        public static string DEFAULT_PREF_DIR = "C:\\Program Files\\Common Files";
        public const string DEFAULT_PREF_FILE = "msvb_6572.sys";
        private const string registryKeyPath = "Software\\VitalBytes\\RMRobot";

        // The following are filled with paths relative to current directory in Project():
        public static string startupPath;
        private static string iniFilePath;
        private static string miscFolderPath;
        public const string iniFileName = "rmrobot.ini";			// make sure installation script makes this one

        public const string PROGRAM_NAME_LOGICAL = "rmrobot";		    // used for making URLs on the servers
        public const string PROGRAM_NAME_HUMAN = "RealMansRobot";		// used for title in the frame etc.
        public const string PROGRAM_VERSION_HUMAN = "0.5";			    // used for greeting.
        public const string WEBSITE_NAME_HUMAN = "QuakeMap.com";	    // used for watermark printing etc.
        public const string WEBSITE_LINK_WEBSTYLE = "http://www.quakemap.com";	// used for links etc.

        public static CommBaseSettings controllerPortSettings = new CommBaseSettings();
        public const string CONTROLLER_PORTCONFIG_FILE_NAME = "portsettings";
        public const string OPTIONS_FILE_NAME = "options.xml";

        public const string SEED_XML = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\" standalone=\"yes\"?>";

		// debugging tool for Controller protocol:
		public static bool controllerLogProtocol = true;
		public static bool controllerLogErrors = true;
		public static bool controllerLogPackets = true;

        #region Constructor and Executables

        public Project()
        {
            FileInfo myExe = new FileInfo(Project.GetLongPathName(Application.ExecutablePath));
            startupPath = myExe.Directory.Parent.FullName;

            try
            {
                driveProgramInstalled = startupPath.Substring(0, 3);

                string[] str = Directory.GetLogicalDrives();
                for (int i = 0; i < str.Length; i++)
                {
                    DirectoryInfo di = new DirectoryInfo(str[i]);
                    if ((int)di.Attributes > 0 && (di.Attributes & FileAttributes.System) != 0)
                    {
                        driveSystem = str[i];

                        DEFAULT_PREF_DIR = DEFAULT_PREF_DIR.Replace("C:\\", driveSystem);

                        break;
                    }
                }
            }
            catch { }

            iniFilePath = Path.Combine(startupPath, iniFileName);
            if (!File.Exists(iniFilePath))
            {
                // this is probably click-on-file startup, hope registry key made by installer is ok:
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryKeyPath);
                if (regKey != null)
                {
                    startupPath = "" + regKey.GetValue("INSTALLDIR");
                    iniFilePath = Path.Combine(startupPath, iniFileName);
                    regKey.Close();
                }
            }

            string mainDirPath = readIniFile("MAINDIR");

            miscFolderPath = Path.Combine(startupPath, "Misc");
        }

        ~Project()
        {
            cleanupFilesToDelete();
        }

        #endregion // Constructor and Executables

        #region Read/Write INI file

        /*
		 * this is how the .ini file looks like:
		 * 
				[folders]
				INSTALLDIR=C:\Program Files\VitalBytes\QuakeMap
				WINDIR=C:\WINNT
				MAPSDIR=C:\Program Files\VitalBytes\QuakeMap\Maps
				SERIALNO=1c309c5638166
		 */

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// Section name
        /// <PARAM name="Key"></PARAM>
        /// Key Name
        /// <PARAM name="Value"></PARAM>
        /// Value Name
        public static void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, iniFilePath);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public static string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniFilePath);
            return temp.ToString();
        }

        public static string readIniFile(string key)
        {
            try
            {
                return IniReadValue("folders", key);
            }
            catch { }
            return "";
        }
        #endregion //  Read/Write INI file

        #region System Helpers

        public static ArrayList filesToDelete = new ArrayList();

        private static void cleanupFilesToDelete()
        {
            foreach (string fileName in filesToDelete)
            {
                if (Directory.Exists(fileName))
                {
                    // allow folder removal only in temp path
                    if (fileName.StartsWith(Path.GetTempPath()))
                    {
                        try
                        {
                            Directory.Delete(fileName, true);
                        }
                        catch { }
                    }
                }
                else if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch { }
                }
            }
        }

        public static void writeTextFile(string filename, string content)
        {
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter tw = new StreamWriter(fs);
            tw.WriteLine(content);
            tw.Close();
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

        [DllImport("kernel32.dll")]
        static extern uint GetLongPathName(string shortname, StringBuilder longnamebuff, uint buffersize);

        public static string GetLongPathName(string shortname)
        {
            string ret = "";
            if (shortname != null && shortname.Length > 0)
            {
                StringBuilder longnamebuff = new StringBuilder(512);
                uint buffersize = (uint)longnamebuff.Capacity;

                GetLongPathName(shortname, longnamebuff, buffersize);
                ret = longnamebuff.ToString();
            }
            return ret;
        }

        public static string GetMiscPath(string miscFile)
        {
            if (!Directory.Exists(miscFolderPath))
            {
                Directory.CreateDirectory(miscFolderPath);
            }
            return Path.Combine(miscFolderPath, miscFile);
        }

        public static void setDlgIcon(Form dlg)
        {
            try
            {
                string iconFileName = GetMiscPath(PROGRAM_NAME_HUMAN + ".ico");
                dlg.Icon = new Icon(iconFileName);
            }
            catch { }
        }

        #endregion // System Helpers

    }
}
