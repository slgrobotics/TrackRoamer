/*
* Copyright (c) 2011..., Sergei Grichine
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Sergei Grichine nor the
*       names of contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Sergei Grichine ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Sergei Grichine BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*
* this is a X11 (BSD Revised) license - you do not have to publish your changes,
* although doing so, donating and contributing is always appreciated
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.IO.Ports;
using System.Globalization;

using Microsoft.Ccr.Core;
using W3C.Soap;

using TrackRoamer.Robotics.Utility.LibSystem;
using TrackRoamer.Robotics.Hardware.LibRoboteqController;

namespace TrackRoamer.Robotics.Services.TrackRoamerBrickPower
{
    /// <summary>
    /// a CCR-ready version of ControllerRQAX2850, able to work as a part of DSS Service.
    /// All COM port operations are externalized.
    /// </summary>
    internal class ControllerRQAX2850_CCR : ControllerRQAX2850
    {
        public ControllerRQAX2850_CCR(string portName)
            : base(portName)
        {
        }

        public SerialPort serialPort { get { return m_port; } set { m_port = value; } }

        public override void ensurePort()
        {
            Tracer.Trace("ControllerRQAX2850: ensurePort() -- m_portName=" + m_portName);
        }

        byte[] oneCR = new byte[] { 0x0D };
        public int tenCRcnt = 11;
        DateTime lastCR = DateTime.Now;

        public override bool GrabController()	// call very often, returns true on success
        {
            if (tenCRcnt == 0)
            {
                Tracer.Trace("ControllerRQAX2850: GrabController()");
                ensurePort();
                isUnknownState = true;
                tenCRcnt++;
            }

            if (tenCRcnt < 11 && !isGrabbed)
            {
                DateTime now = DateTime.Now;
                if ((now - lastCR).TotalMilliseconds > 30)      // 10 doesn't work, 20 and more works fine
                {
                    m_port.Write(oneCR, 0, 1);
                    tenCRcnt++;
                    lastCR = now;
                }
            }

            return true;
        }
    }
}
