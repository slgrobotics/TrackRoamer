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


This is a MRDS Service for Pololu Mini Maestro - just the minimum to control the servos. It was tested with Mini 12 device.

The devices can be found at http://www.pololu.com/catalog/category/12

Here is the MRDS code to control the servos:


		using pololumaestro = TrackRoamer.Robotics.Hardware.PololuMaestroService.Proxy;
		......
        /// <summary>
        /// Pololu Maestro Device partner
        /// </summary>
        [Partner("PololuMaestroService", Contract = pololumaestro.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected pololumaestro.PololuMaestroServiceOperations _pololuMaestroPort = new pololumaestro.PololuMaestroServiceOperations();
        protected pololumaestro.PololuMaestroServiceOperations _pololuMaestroNotify = new pololumaestro.PololuMaestroServiceOperations();
		......

            byte channel = 1;

            for (int i = 1; i <= 50; i++)
            {
                int servoPos = 1000 + 20 * i;

                pololumaestro.PololuMaestroCommand cmd = new pololumaestro.PololuMaestroCommand() { Command = "set", Channel = channel, Target = (ushort)(servoPos * 4) };

                _pololuMaestroPort.Post(new pololumaestro.SendPololuMaestroCommand(cmd));

                // wait some time
                yield return Timeout(1000);
            }

Enjoy!
