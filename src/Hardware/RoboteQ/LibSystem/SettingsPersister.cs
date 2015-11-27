using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;

namespace LibSystem
{
	public class SettingsPersister
	{
		public static int controllerMakeIndex = 0;

		public static string[] AllControllerMakes = new string[] {
															  "RoboteQ",
															  "LEGO"
														  };

		public static string currentControllerMake { get { return AllControllerMakes[controllerMakeIndex]; } }

		// returns true if could restore from file
		public static bool restoreOrDefaultPortSettings()
		{
			bool ret = false;

			// restore controllerPortSettings from the file or fill it with default values:
			string controllerPortFilePath = Project.GetMiscPath(Project.CONTROLLER_PORTCONFIG_FILE_NAME + "-" + currentControllerMake + ".xml");
			if (!File.Exists(controllerPortFilePath))
			{
				// older versions used single file for all Controllers
				controllerPortFilePath = Project.GetMiscPath(Project.CONTROLLER_PORTCONFIG_FILE_NAME + ".xml");
			}
			Stream fs = null;
			try
			{
				fs = new FileStream(controllerPortFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				Project.controllerPortSettings = CommBaseSettings.LoadFromXML(fs);
				if (Project.controllerPortSettings == null)
				{
					throw new Exception();
				}
				ret = true;
			}
			catch
			{
				Project.controllerPortSettings = new CommBaseSettings();
				Project.controllerPortSettings.port = "COM1:";
				Project.controllerPortSettings.baudRate = 9600;
				Project.controllerPortSettings.parity = Parity.none;
				Project.controllerPortSettings.autoReopen = true;

			}
			finally
			{
				if (fs != null) { fs.Close(); }
			}

			return ret;
		}

		#region Read/Write Options file

		public static void ReadOptions()
		{
			string optionsFilePath = Project.GetMiscPath(Project.OPTIONS_FILE_NAME);
#if DEBUG
			Tracer.Trace("IP: Project:ReadOptions() path=" + optionsFilePath);
#endif
			DateTime startedRead = DateTime.Now;
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(optionsFilePath);

				// we want to traverse XmlDocument fast, as tile load operations can be numerous
				// and come in pack. So we avoid using XPath and rely mostly on "foreach child":
				foreach (XmlNode nnode in xmlDoc.ChildNodes)
				{
					if (nnode.Name.Equals("options"))
					{
						foreach (XmlNode node in nnode.ChildNodes)
						{
							string nodeName = node.Name;
							try
							{
							}
							catch (Exception ee)
							{
								// bad node - not a big deal...
								Tracer.Error("Project:ReadOptions() node=" + nodeName + " " + ee.Message);
							}
						}
					}
				}

			}
			catch (Exception e)
			{
				Tracer.Error("Project:ReadOptions() " + e.Message);
			}

			Tracer.Trace("ReadOptions: " + Math.Round((DateTime.Now - startedRead).TotalMilliseconds) + " ms");

			// restore controllerPortSettings from the file or fill it with default values:
			restoreOrDefaultPortSettings();
		}

		public static void SaveOptions()
		{
			string optionsFilePath = Project.GetMiscPath(Project.OPTIONS_FILE_NAME);
			//Tracer.Trace("Project:SaveOptions: " + optionsFilePath);
			try
			{
				string seedXml = Project.SEED_XML + "<options></options>";
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(seedXml);

				XmlNode root = xmlDoc.DocumentElement;

				SetValue(xmlDoc, root, "time", "" + DateTime.Now);
			}
			catch (Exception e)
			{
				Tracer.Error("Project:SaveOptions() " + e.Message);
			}
		}

		public static void savePortSettings()
		{
			FileStream fs = null;

			try
			{
				string controllerPortFilePath = Project.GetMiscPath(Project.CONTROLLER_PORTCONFIG_FILE_NAME + "-" + currentControllerMake + ".xml");
				fs = new FileStream(controllerPortFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				Project.controllerPortSettings.SaveAsXML(fs);
			}
			catch (Exception ee)
			{
				Tracer.Error("saving port settings: " + ee.Message);
			}
			finally
			{
				if (fs != null) { fs.Close(); }
			}

		}
		#endregion //  Read/Write Options file

		#region XML helpers

		public static XmlNode SetValue(XmlDocument xmlDoc, XmlNode root, string name, string vvalue)
		{
			XmlNode ret = xmlDoc.CreateElement(name);
			if (vvalue != null && vvalue.Length > 0)
			{
				ret.InnerText = vvalue;
			}
			root.AppendChild(ret);
			return ret;
		}

		public static XmlNode SetValue(XmlDocument xmlDoc, XmlNode root, string prefix, string name, string namespaceURI, string vvalue)
		{
			XmlNode ret = xmlDoc.CreateElement(prefix, name, namespaceURI);
			if (vvalue != null && vvalue.Length > 0)
			{
				ret.InnerText = vvalue;
			}
			root.AppendChild(ret);
			return ret;
		}

		#endregion // XML helpers

	}
}
