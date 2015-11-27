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
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Security.Permissions;

using Microsoft.Dss.Core;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.Core.DsspHttpUtilities;

using TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.Properties;

namespace TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor
{
    [Contract(Contract.Identifier)]
    [DisplayName("(User) CH Robotics UM6 Orientation Sensor")]
    [Description("Provides Data from CH Robotics UM6 Orientation Sensor")]
    class ChrUm6OrientationSensorService : DsspServiceBase
    {
        #region class variables and parameters

        [EmbeddedResource("TrackRoamer.Robotics.Hardware.ChrUm6OrientationSensor.ChrUm6OrientationSensor.user.xslt")]
        private string _transformChrData = null;

        private const string _configFile = ServicePaths.Store + "/ChrUm6OrientationSensor.config.xml";

        const string Tag_GYRO_PROC  = "GYRO_PROC";
        const string Tag_ACCEL_PROC = "ACCEL_PROC";
        const string Tag_MAG_PROC   = "MAG_PROC";
        const string Tag_EULER      = "EULER";
        const string Tag_QUAT       = "QUAT";

        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        [InitialStatePartner(Optional = true, ServiceUri = _configFile)]
        private ChrUm6OrientationSensorState _state = new ChrUm6OrientationSensorState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/ChrUm6OrientationSensor", AllowMultipleInstances = false)]
        ChrUm6OrientationSensorOperations _mainPort = new ChrUm6OrientationSensorOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Communicate with the Chr hardware
        /// </summary>
        private ChrConnection _chrConnection = null;

        /// <summary>
        /// A CCR port for receiving Chr data
        /// </summary>
        private ChrDataPort _chrDataPort = new ChrDataPort();

        /// <summary>
        /// Http helpers
        /// </summary>
        private DsspHttpUtilitiesPort _httpUtilities = new DsspHttpUtilitiesPort();

        #endregion // class variables and parameters

        /// <summary>
        /// Service Default constructor
        /// </summary>
        public ChrUm6OrientationSensorService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        #region Service start

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            if (_state == null)
            {
                _state = new ChrUm6OrientationSensorState();
                _state.ChrUm6OrientationSensorConfig = new ChrUm6OrientationSensorConfig();
                _state.ChrUm6OrientationSensorConfig.CommPort = 0;

                SaveState(_state);
            }
            else
            {
                // Clear old Chr readings
                _state.Quaternion = null;
            }

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            if (_state.ChrUm6OrientationSensorConfig == null)
                _state.ChrUm6OrientationSensorConfig = new ChrUm6OrientationSensorConfig();

            // Publish the service to the local Node Directory
            DirectoryInsert();

            _chrConnection = new ChrConnection(_state.ChrUm6OrientationSensorConfig, _chrDataPort);

            SpawnIterator(ConnectToChrUm6OrientationSensor);

            // Listen on the main port for requests and call the appropriate handler.
            Interleave mainInterleave = ActivateDsspOperationHandlers();

            mainInterleave.CombineWith(new Interleave(new ExclusiveReceiverGroup(
                Arbiter.Receive<short[]>(true, _chrDataPort, DataReceivedHandler),
                Arbiter.Receive<Exception>(true, _chrDataPort, ExceptionHandler),
                Arbiter.Receive<string>(true, _chrDataPort, MessageHandler)
                ),
                new ConcurrentReceiverGroup()));

            //base.Start(); -- can't have it here, we already started mainInterleave and added to directory via DirectoryInsert
        }

        #endregion // Service start

        #region Service Handlers

        /// <summary>
        /// Handle Errors
        /// </summary>
        /// <param name="ex"></param>
        private void ExceptionHandler(Exception ex)
        {
            LogError(ex.Message);
        }

        /// <summary>
        /// Handle diagnostic messages
        /// </summary>
        /// <param name="message"></param>
        private void MessageHandler(string message)
        {
            LogInfo(message);
        }

        /// <summary>
        /// Handle inbound sensor data coming from UM6, and convert it to notifications to service subscribers.
        /// All data comes as arrays of short[] with first word set to register address as defined in Chinterface\UM1B.XML file.
        /// See UM6 datasheet for more info. Source code in C++ for working with UM6 is at http://sourceforge.net/projects/chrinterface/  (get SVN tarball there)
        /// </summary>
        /// <param name="fields"></param>
        private void DataReceivedHandler(short[] fields)
        {
            Um6RegisterAddress um6address = (Um6RegisterAddress)fields[0];

            switch (um6address)
            {
                case Um6RegisterAddress.UM6_GYRO_PROC:
                    GyroProcHandler(fields);
                    break;

                case Um6RegisterAddress.UM6_ACCEL_PROC:
                    AccelProcHandler(fields);
                    break;

                case Um6RegisterAddress.UM6_MAG_PROC:
                    MagProcHandler(fields);
                    break;

                case Um6RegisterAddress.UM6_EULER:
                    EulerHandler(fields);
                    break;

                case Um6RegisterAddress.UM6_QUAT:
                    QuaternionHandler(fields);
                    break;

                default:
                    string line = string.Join(",", fields);
                    MessageHandler(Resources.UnhandledChrData + line);
                    break;
            }
        }

        /// <summary>
        /// Handle UM6_GYRO_PROC data packet
        /// </summary>
        /// <param name="fields"></param>
        private void GyroProcHandler(short[] fields)
        {
            try
            {
                if (_state.ProcGyro == null)
                    _state.ProcGyro = new DataProcGyro();

                if (fields.Length < 4)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in UM6_GYRO_PROC ({0}/{1})", fields.Length, 4));
                    return;
                }

                _state.ProcGyro.LastUpdate = DateTime.Now;
                _state.ProcGyro.xRate = fields[1];
                _state.ProcGyro.yRate = fields[2];
                _state.ProcGyro.zRate = fields[3];

                SendNotification(_subMgrPort, new ProcGyroNotification(_state.ProcGyro), Tag_GYRO_PROC, "OK");

            }
            catch (Exception ex)
            {
                _chrDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle UM6_ACCEL_PROC data packet
        /// </summary>
        /// <param name="fields"></param>
        private void AccelProcHandler(short[] fields)
        {
            try
            {
                if (_state.ProcAccel == null)
                    _state.ProcAccel = new DataProcAccel();

                if (fields.Length < 4)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in UM6_ACCEL_PROC ({0}/{1})", fields.Length, 4));
                    return;
                }

                _state.ProcAccel.LastUpdate = DateTime.Now;
                _state.ProcAccel.xAccel = fields[1];
                _state.ProcAccel.yAccel = fields[2];
                _state.ProcAccel.zAccel = fields[3];

                SendNotification(_subMgrPort, new ProcAccelNotification(_state.ProcAccel), Tag_ACCEL_PROC, "OK");

            }
            catch (Exception ex)
            {
                _chrDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle UM6_MAG_PROC data packet
        /// </summary>
        /// <param name="fields"></param>
        private void MagProcHandler(short[] fields)
        {
            try
            {
                if (_state.ProcMag == null)
                    _state.ProcMag = new DataProcMag();

                if (fields.Length < 4)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in UM6_MAG_PROC ({0}/{1})", fields.Length, 4));
                    return;
                }

                _state.ProcMag.LastUpdate = DateTime.Now;
                _state.ProcMag.x = fields[1];
                _state.ProcMag.y = fields[2];
                _state.ProcMag.z = fields[3];

                SendNotification(_subMgrPort, new ProcMagNotification(_state.ProcMag), Tag_MAG_PROC, "OK");

            }
            catch (Exception ex)
            {
                _chrDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle UM6_EULER data packet
        /// </summary>
        /// <param name="fields"></param>
        private void EulerHandler(short[] fields)
        {
            try
            {
                if (_state.Euler == null)
                    _state.Euler = new DataEuler();

                if (fields.Length < 4)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in UM6_EULER ({0}/{1})", fields.Length, 4));
                    return;
                }

                _state.Euler.LastUpdate = DateTime.Now;
                _state.Euler.phi = fields[1];
                _state.Euler.theta = fields[2];
                _state.Euler.psi = fields[3];

                SendNotification(_subMgrPort, new EulerNotification(_state.Euler), Tag_EULER, "OK");

            }
            catch (Exception ex)
            {
                _chrDataPort.Post(ex);
            }
        }

        /// <summary>
        /// Handle UM6 QUAT data packet
        /// </summary>
        /// <param name="fields"></param>
        private void QuaternionHandler(short[] fields)
        {
            try
            {
                if (_state.Quaternion == null)
                    _state.Quaternion = new DataQuaternion();

                if (fields.Length < 4)
                {
                    MessageHandler(string.Format(CultureInfo.InvariantCulture, "Invalid Number of parameters in UM6_QUAT ({0}/{1})", fields.Length, 4));
                    return;
                }

                _state.Quaternion.LastUpdate = DateTime.Now;
                _state.Quaternion.a = fields[1];
                _state.Quaternion.b = fields[2];
                _state.Quaternion.c = fields[3];
                _state.Quaternion.d = fields[4];

                SendNotification(_subMgrPort, new QuaternionNotification(_state.Quaternion), Tag_QUAT, "OK");

            }
            catch (Exception ex)
            {
                _chrDataPort.Post(ex);
            }
        }

        #endregion

        #region Operation Handlers

        /// <summary>
        /// Http Get Handler
        /// </summary>
        /// <param name="httpGet"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            HttpListenerRequest request = httpGet.Body.Context.Request;
            HttpListenerResponse response = httpGet.Body.Context.Response;

            HttpResponseType rsp = null;

            string path = request.Url.AbsolutePath.ToLowerInvariant();

            rsp = new HttpResponseType(HttpStatusCode.OK, _state, _transformChrData);
            httpGet.ResponsePort.Post(rsp);
            yield break;

        }

        /// <summary>
        /// Http Post Handler
        /// </summary>
        /// <param name="httpPost"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> HttpPostHandler(HttpPost httpPost)
        {
            // Use helper to read form data
            ReadFormData readForm = new ReadFormData(httpPost);
            _httpUtilities.Post(readForm);

            // Wait for result
            Activate(Arbiter.Choice(readForm.ResultPort,
                delegate(NameValueCollection parameters)
                {
                    if (!string.IsNullOrEmpty(parameters["Action"])
                        && parameters["Action"] == "ChrUm6OrientationSensorConfig"
                        )
                    {
                        if (parameters["buttonOk"] == "Search")
                        {
                            FindChrConfig findConfig = new FindChrConfig();
                            _mainPort.Post(findConfig);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<ChrUm6OrientationSensorConfig>(false, findConfig.ResponsePort,
                                        delegate(ChrUm6OrientationSensorConfig response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, findConfig.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );

                        }
                        else if (parameters["buttonOk"] == "Connect")
                        {
                            ChrUm6OrientationSensorConfig config = (ChrUm6OrientationSensorConfig)_state.ChrUm6OrientationSensorConfig.Clone();
                            int port;
                            if (int.TryParse(parameters["CommPort"], out port) && port >= 0)
                            {
                                config.CommPort = port;
                                config.PortName = "COM" + port.ToString();
                            }

                            int baud;
                            if (int.TryParse(parameters["BaudRate"], out baud) && ChrConnection.ValidBaudRate(baud))
                            {
                                config.BaudRate = baud;
                            }

                            Configure configure = new Configure(config);
                            _mainPort.Post(configure);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<DefaultUpdateResponseType>(false, configure.ResponsePort,
                                        delegate(DefaultUpdateResponseType response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, configure.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );
                        }
                        else if (parameters["buttonOk"] == "Refresh Data")
                        {
                            HttpPostSuccess(httpPost);
                        }
                        else if (parameters["buttonOk"] == "Set Accelerometer Reference Vector")
                        {
                            ChrUm6OrientationSensorCommand cmd = new ChrUm6OrientationSensorCommand() { Command = "SetAccelRefVector" };
                            SendChrUm6OrientationSensorCommand sCmd = new SendChrUm6OrientationSensorCommand(cmd);

                            _mainPort.Post(sCmd);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<DefaultUpdateResponseType>(false, sCmd.ResponsePort,
                                        delegate(DefaultUpdateResponseType response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, sCmd.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );
                        }
                        else if (parameters["buttonOk"] == "Set Magnetometer Reference Vector")
                        {
                            ChrUm6OrientationSensorCommand cmd = new ChrUm6OrientationSensorCommand() { Command = "SetMagnRefVector" };
                            SendChrUm6OrientationSensorCommand sCmd = new SendChrUm6OrientationSensorCommand(cmd);

                            _mainPort.Post(sCmd);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<DefaultUpdateResponseType>(false, sCmd.ResponsePort,
                                        delegate(DefaultUpdateResponseType response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, sCmd.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );
                        }
                        else if (parameters["buttonOk"] == "Zero Rate Gyros")
                        {
                            ChrUm6OrientationSensorCommand cmd = new ChrUm6OrientationSensorCommand() { Command = "ZeroRateGyros" };
                            SendChrUm6OrientationSensorCommand sCmd = new SendChrUm6OrientationSensorCommand(cmd);

                            _mainPort.Post(sCmd);
                            Activate(
                                Arbiter.Choice(
                                    Arbiter.Receive<DefaultUpdateResponseType>(false, sCmd.ResponsePort,
                                        delegate(DefaultUpdateResponseType response)
                                        {
                                            HttpPostSuccess(httpPost);
                                        }),
                                    Arbiter.Receive<Fault>(false, sCmd.ResponsePort,
                                        delegate(Fault f)
                                        {
                                            HttpPostFailure(httpPost, f);
                                        })
                                )
                            );
                        }
                    }
                    else
                    {
                        HttpPostFailure(httpPost, null);
                    }
                },
                delegate(Exception Failure)
                {
                    LogError(Failure.Message);
                })
            );
            yield break;
        }

        /// <summary>
        /// Send Http Post Success Response
        /// </summary>
        /// <param name="httpPost"></param>
        private void HttpPostSuccess(HttpPost httpPost)
        {
            HttpResponseType rsp =
                new HttpResponseType(HttpStatusCode.OK, _state, _transformChrData);
            httpPost.ResponsePort.Post(rsp);
        }

        /// <summary>
        /// Send Http Post Failure Response
        /// </summary>
        /// <param name="httpPost"></param>
        /// <param name="fault"></param>
        private static void HttpPostFailure(HttpPost httpPost, Fault fault)
        {
            HttpResponseType rsp =
                new HttpResponseType(HttpStatusCode.BadRequest, fault);
            httpPost.ResponsePort.Post(rsp);
        }


        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// Configure Handler
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ConfigureHandler(Configure update)
        {
            _state.ChrUm6OrientationSensorConfig = update.Body;
            bool connected = _chrConnection.Open(_state.ChrUm6OrientationSensorConfig.CommPort, _state.ChrUm6OrientationSensorConfig.BaudRate);

            SaveState(_state);
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

        /// <summary>
        /// Subscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            SubscribeRequest request = subscribe.Body;

            Console.WriteLine("SubscribeHandler() received Subscription request from Subscriber=" + subscribe.Body.Subscriber + "   for: " + request.MessageTypes);

            submgr.InsertSubscription insert = new submgr.InsertSubscription(request);
            insert.Body.FilterType = submgr.FilterType.Default;

            string valid = request.ValidOnly ? "True" : null;

            List<submgr.QueryType> query = new List<submgr.QueryType>();

            if (request.MessageTypes == ChrMessageType.All ||
                request.MessageTypes == ChrMessageType.None)
            {
                if (request.ValidOnly)
                {
                    query.Add(new submgr.QueryType(null, valid));
                }
            }
            else
            {
                // Subscriber supplied a bitmask requesting certain UM6 messages:
                if ((request.MessageTypes & ChrMessageType.GYRO_PROC) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_GYRO_PROC, valid));
                }
                if ((request.MessageTypes & ChrMessageType.ACCEL_PROC) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_ACCEL_PROC, valid));
                }
                if ((request.MessageTypes & ChrMessageType.MAG_PROC) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_MAG_PROC, valid));
                }
                if ((request.MessageTypes & ChrMessageType.EULER) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_EULER, valid));
                }
                if ((request.MessageTypes & ChrMessageType.QUAT) != 0)
                {
                    query.Add(new submgr.QueryType(Tag_QUAT, valid));
                }
                // add more types here to the query
            }

            if (query.Count > 0)
            {
                insert.Body.QueryList = query.ToArray();
            }
            _subMgrPort.Post(insert);

            yield return Arbiter.Choice(
                insert.ResponsePort,
                subscribe.ResponsePort.Post,
                subscribe.ResponsePort.Post
            );
        }

        /// <summary>
        /// Find a CHR UM6 Sensor on any serial port
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> FindChrConfigHandler(FindChrConfig query)
        {
            _state.Connected = _chrConnection.FindChr();
            if (_state.Connected)
            {
                _state.ChrUm6OrientationSensorConfig = _chrConnection.ChrUm6OrientationSensorConfig;
                SaveState(_state);
            }
            query.ResponsePort.Post(_state.ChrUm6OrientationSensorConfig);
            yield break;
        }

        /// <summary>
        /// Send UM6 Command Handler
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SendChrCommandHandler(SendChrUm6OrientationSensorCommand update)
        {

            //update.ResponsePort.Post(
            //    Fault.FromException(
            //        new NotImplementedException("The CHR UM6 Sensor service is a sample. Sending commands to the sensor is an exercise left to the community.")));

            string command = update.Body.Command;

            switch(command)
            {
                case "ZeroRateGyros":
                case "SetAccelRefVector":
                case "SetMagnRefVector":
                    Console.WriteLine("SendChrCommandHandler received command " + command);
                    if (_state.Connected)
                    {
                        _chrConnection.SendCommand(command);
                    }
                    break;

                default:
                    Console.WriteLine("SendChrCommandHandler received unknown command '" + command + "'" );
                    break;
            }

            update.ResponsePort.Post(new DefaultUpdateResponseType());
            yield break;
        }

        /// <summary>
        /// Shut down the CHR UM6 Sensor connection
        /// </summary>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
        {
            if (_state.Connected)
            {
                _chrConnection.Close();
                _state.Connected = false;
            }

            base.DefaultDropHandler(drop);
            yield break;
        }

        #endregion

        #region UM6 Serial Connection Helpers

        /// <summary>
        /// Connect to a CHR UM6 Sensor.
        /// If no configuration exists, search for the connection.
        /// The code here is close to GPS service code from Microsoft RDS R3.
        /// </summary>
        private IEnumerator<ITask> ConnectToChrUm6OrientationSensor()
        {
            try
            {
                _state.Quaternion = null;
                _state.Connected = false;

                if (_state.ChrUm6OrientationSensorConfig.CommPort != 0 && _state.ChrUm6OrientationSensorConfig.BaudRate != 0)
                {
                    _state.ChrUm6OrientationSensorConfig.ConfigurationStatus = "Opening UM6 on Port " + _state.ChrUm6OrientationSensorConfig.CommPort.ToString();
                    _state.Connected = _chrConnection.Open(_state.ChrUm6OrientationSensorConfig.CommPort, _state.ChrUm6OrientationSensorConfig.BaudRate);
                }
                else if (File.Exists(_state.ChrUm6OrientationSensorConfig.PortName))
                {
                    _state.ChrUm6OrientationSensorConfig.ConfigurationStatus = "Opening UM6 on " + _state.ChrUm6OrientationSensorConfig.PortName;
                    _state.Connected = _chrConnection.Open(_state.ChrUm6OrientationSensorConfig.PortName);
                }
                else
                {
                    _state.ChrUm6OrientationSensorConfig.ConfigurationStatus = "Searching for the CH Robotics UM6 Orientation Sensor Port";
                    _state.Connected = _chrConnection.FindChr();
                    if (_state.Connected)
                    {
                        _state.ChrUm6OrientationSensorConfig = _chrConnection.ChrUm6OrientationSensorConfig;
                        SaveState(_state);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError(ex);
            }
            catch (IOException ex)
            {
                LogError(ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                LogError(ex);
            }
            catch (ArgumentException ex)
            {
                LogError(ex);
            }
            catch (InvalidOperationException ex)
            {
                LogError(ex);
            }

            if (!_state.Connected)
            {
                _state.ChrUm6OrientationSensorConfig.ConfigurationStatus = "Not Connected";
                LogInfo(LogGroups.Console, "The CH Robotics UM6 Orientation Sensor is not detected.\r\n*   To configure the CHR UM6 Sensor, navigate to: ");
            }
            else
            {
                _state.ChrUm6OrientationSensorConfig.ConfigurationStatus = "Connected";
            }
            yield break;
        }

        #endregion

    }
}


