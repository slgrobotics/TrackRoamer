using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using Microsoft.DirectX.DirectInput;

using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.Runtime.InteropServices; 

using LibSystem;
using LibLvrGenericHid;

namespace LibPicSensors
{
	public partial class ProximityModule
	{
		private Form m_mainForm = null;
        private System.Windows.Forms.Timer picUsbTickerTimer = null;

		public ProximityModule(Form mainForm)
        {
			m_mainForm = mainForm;

            Startup();                 
        }

        public void Open()
        {
            try
            {
                bool isConnected = FindTheHid(vendorId, productId);

                if (isConnected)
                {
                    Tracer.Trace("OK: USB Interface connected with PIC USB Proximity Board");
                }
                else
                {
                    string str = string.Format("USB Interface could not connect with device with Vendor ID={0} Product ID={1}", vendorId, productId);
                    Tracer.Error(str);
                    throw new Exception(str);
                }

                picUsbTickerTimer = new System.Windows.Forms.Timer();
                picUsbTickerTimer.Interval = 20;    // ms
                picUsbTickerTimer.Tick += new EventHandler(picUsbTicker);
                picUsbTickerTimer.Start();

                Tracer.Trace("OK: PIC Proximity Board ticker ON");
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }             

        }

		public void Close()
		{
			if (picUsbTickerTimer != null)
			{
				picUsbTickerTimer.Enabled = false;
				picUsbTickerTimer.Dispose();
				picUsbTickerTimer = null;

                Tracer.Trace("OK: PIC Proximity Board ticker OFF");
            }

            Shutdown();
        }

        public void picUsbTicker(object obj, System.EventArgs args)
        {
            try
            {
                //Tracer.Trace("...picUsb ticker... " + DateTime.Now);

                //  Don't allow another transfer request until this one completes.
                //  Move the focus away from cmdOnce to prevent the focus from 
                //  switching to the next control in the tab order on disabling the button.

                ;
                //ReadAndWriteToDevice();
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
                throw;
            }

            picUsbTickerTimer.Enabled = true;   // for the next cycle
        }
	}
}
