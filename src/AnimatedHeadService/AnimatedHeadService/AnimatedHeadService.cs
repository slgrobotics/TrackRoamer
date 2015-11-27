using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

using Microsoft.Dss.Core;
using Microsoft.Dss.Core.DsspHttpUtilities;
using System.IO;
using Trackroamer.Library.LibAnimatronics;

namespace TrackRoamer.Robotics.Services.AnimatedHeadService
{
    // The ActivationSettings attribute with Sharing == false makes the runtime dedicate a dispatcher thread pool just for this service.
    // ExecutionUnitsPerDispatcher	- Indicates the number of execution units allocated to the dispatcher
    // ShareDispatcher	            - Inidicates whether multiple service instances can be pooled or not
    //[ActivationSettings(ShareDispatcher = false, ExecutionUnitsPerDispatcher = 8)]
    [Contract(Contract.Identifier)]
    [DisplayName("(User) Animated Head Service")]
    [Description("Animated Head Service service controls animatronic head capable of expressing emotions")]
    class AnimatedHeadService : DsspServiceBase
    {
        private const string _configFile = ServicePaths.Store + "/AnimatedHead.config.xml";

        /// <summary>
        /// Service state
        /// </summary>
        //[ServiceState]
        [ServiceState(StateTransform = "TrackRoamer.Robotics.Services.AnimatedHeadService.AnimatedHeadService.xslt")]
        [InitialStatePartner(Optional = true, ServiceUri = _configFile)]
        AnimatedHeadServiceState _state = new AnimatedHeadServiceState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/AnimatedHeadService", AllowMultipleInstances = false)]
        AnimatedHeadServiceOperations _mainPort = new AnimatedHeadServiceOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Animated Head Device partner
        /// </summary>
        //[Partner("AnimatedHeadService", Contract = Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        protected AnimatedHeadServiceOperations _animatedHeadCommandPort = new AnimatedHeadServiceOperations();
        protected AnimatedHeadServiceOperations _animatedHeadNotify = new AnimatedHeadServiceOperations();

        /// <summary>
        /// Communicate with the Animated Head hardware
        /// </summary>
        private AnimatedHeadConnection _ahConnection = null;

        /// <summary>
        /// A CCR port for receiving  Animated Head data
        /// </summary>
        private AnimatedHeadDataPort _ahDataPort = new AnimatedHeadDataPort();

        /// <summary>
        /// Http helpers
        /// </summary>
        private DsspHttpUtilitiesPort _httpUtilities = new DsspHttpUtilitiesPort();

        private IBrickConnector brickConnector;

        private Animations animations = new Animations();
        private AnimationCombo animationCombo = new AnimationCombo();

        /// <summary>
        /// Service constructor
        /// </summary>
        public AnimatedHeadService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
            brickConnector = new BrickConnectorArduino();
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            base.Start();   // start MainPortInterleave; wireup [ServiceHandler] methods

            if (_state == null)
            {
                _state = new AnimatedHeadServiceState();
                _state.AnimatedHeadServiceConfig = new AnimatedHeadConfig();
                _state.AnimatedHeadServiceConfig.CommPort = 0;

                SaveState(_state);
            }
            else
            {
                // Clear old Animated Head readings
                //_state.Quaternion = null;
                _state.Connected = false;
            }

            _httpUtilities = DsspHttpUtilitiesService.Create(Environment);

            if (_state.AnimatedHeadServiceConfig == null)
            {
                _state.AnimatedHeadServiceConfig = new AnimatedHeadConfig();
            }

            // Publish the service to the local Node Directory
            //DirectoryInsert();

            _ahConnection = new AnimatedHeadConnection(_state.AnimatedHeadServiceConfig, _ahDataPort);

            SpawnIterator(ConnectToAnimatedHead);

            //SpawnIterator(TestAnimatedHead);

            // if we have anything coming from Arduino - the _ahDataPort Arbiter.Receive<string[]> and others must be operational:

            // Listen on the main port for requests and call the appropriate handler.

            MainPortInterleave.CombineWith(new Interleave(new ExclusiveReceiverGroup(
                Arbiter.Receive<string[]>(true, _ahDataPort, DataReceivedHandler),
                Arbiter.Receive<Exception>(true, _ahDataPort, ExceptionHandler),
                Arbiter.Receive<string>(true, _ahDataPort, MessageHandler)
                ),
                new ConcurrentReceiverGroup()));

            //base.Start(); // can't have it here, we already started mainInterleave and added to directory via DirectoryInsert
        }

        /// <summary>
        /// for self-contained test, uncomment the call to this metod above and watch console
        /// </summary>
        /// <returns></returns>
        private IEnumerator<ITask> TestAnimatedHead()
        {
            while (!_state.Connected)
            {
                Console.WriteLine("TestAnimatedHead - waiting to connect...");
                yield return Timeout(2000);
            }

            Console.WriteLine("TestAnimatedHead - ClearAnimations()");
            //ClearAnimations();

            ArduinoDeviceCommand cmd = new ArduinoDeviceCommand() { Command = AnimatedHeadCommands.ANIMATIONS_CLEAR };

            _animatedHeadCommandPort.Post(new SendArduinoDeviceCommand(cmd));

            yield return Timeout(4000);

            Console.WriteLine("TestAnimatedHead - DefaultAnimations()");
            //DefaultAnimations();

            cmd = new ArduinoDeviceCommand() { Command = AnimatedHeadCommands.ANIMATIONS_DEFAULT };

            _animatedHeadCommandPort.Post(new SendArduinoDeviceCommand(cmd));

            yield return Timeout(3000);

            Console.WriteLine("TestAnimatedHead - SetAnimCombo('Mad')");
            //SetAnimCombo("Mad", 0.2d, true);

            // Valid Combo names:
            //  Acknowledge, Afraid, Alert, Alert_lookleft, Alert_lookright, Angry, Blink, Blink1, BlinkCycle, Decline, Look_down, Look_up, Lookaround,
            //  Mad, Mad2, Notgood, Ohno, Pleased, Restpose, Sad,
            //  Smallturnleft, Smallturnleftdow, Smallturnrigh, Smallturnrightdow,
            //  Smile, Surprised, Think, Turn_left, Turn_left_smile, Turn_right, Turn_right_smile, Upset, What, Wow, Yeah, Yell, Talk

            cmd = new ArduinoDeviceCommand() { Command = AnimatedHeadCommands.SetAnimCombo, Args="Mad", Scale=0.2d, doRepeat=true };

            _animatedHeadCommandPort.Post(new SendArduinoDeviceCommand(cmd));

            yield return Timeout(4000);

            cmd = new ArduinoDeviceCommand() { Command = AnimatedHeadCommands.SetAnimCombo, Args="Acknowledge", Scale=0.2d, doRepeat=true };

            _animatedHeadCommandPort.Post(new SendArduinoDeviceCommand(cmd));

            yield return Timeout(4000);

            cmd = new ArduinoDeviceCommand() { Command = AnimatedHeadCommands.ANIMATIONS_CLEAR };

            _animatedHeadCommandPort.Post(new SendArduinoDeviceCommand(cmd));

            yield break;
        }

        /// <summary>
        /// Animated Head / Arduino Device Command Handler
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ArduinoDeviceCommandHandler(SendArduinoDeviceCommand cmd)
        {
            Console.WriteLine("=========================== AnimatedHeadService:ArduinoDeviceCommandHandler received ArduinoDeviceCommand: " + cmd.Body);

            switch(cmd.Body.Command)
            {
                //case AnimatedHeadCommands.SET_VALUE:
                //    break;

                //case AnimatedHeadCommands.SET_FRAMES:
                //    break;

                case AnimatedHeadCommands.ANIMATIONS_CLEAR:
                    ClearAnimations();
                    break;

                case AnimatedHeadCommands.ANIMATIONS_DEFAULT:
                    DefaultAnimations();
                    break;

                case AnimatedHeadCommands.SetAnim:
                    {
                        string[] anims = cmd.Body.Args.Split(new char[] { '|' });
                        SetAnim(anims, cmd.Body.Scale, cmd.Body.doRepeat.GetValueOrDefault(false));
                    }
                    break;

                case AnimatedHeadCommands.AddAnim:
                    {
                        string[] anims = cmd.Body.Args.Split(new char[] { '|' });
                        AddAnim(anims, cmd.Body.Scale, cmd.Body.doRepeat.GetValueOrDefault(false));
                    }
                    break;

                case AnimatedHeadCommands.SetAnimCombo:
                    SetAnimCombo(cmd.Body.Args, cmd.Body.Scale, cmd.Body.doRepeat.GetValueOrDefault(false));
                    break;

                default:
                    break;
            }

            cmd.ResponsePort.Post(DefaultUpdateResponseType.Instance);

            yield break;
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }

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
        /// Handle messages
        /// </summary>
        /// <param name="message"></param>
        private void MessageHandler(string message)
        {
            LogInfo(message);
        }

        /// <summary>
        /// Handle inbound Gps Data
        /// </summary>
        /// <param name="fields"></param>
        private void DataReceivedHandler(string[] fields)
        {
            string ahLine = string.Join(",", fields);       // no checksum
            Console.WriteLine(ahLine);

            //if (fields[0].Equals("$GPGGA"))
            //    GgaHandler(fields);
            //else
            //{
            //    string line = string.Join(",", fields);
            //    MessageHandler(Resources.UnhandledGpsData + line);
            //}
        }

        #endregion // Service Handlers

        #region AnimatedHead Serial Connection Helpers

        /// <summary>
        /// Connect to a Animated Head.
        /// If no configuration exists, search for the connection.
        /// The code here is close to GPS service code from Microsoft RDS R3.
        /// </summary>
        private IEnumerator<ITask> ConnectToAnimatedHead()
        {
            try
            {
                //_state.Quaternion = null;
                _state.Connected = false;

                if (_state.AnimatedHeadServiceConfig.CommPort != 0 && _state.AnimatedHeadServiceConfig.BaudRate != 0)
                {
                    _state.AnimatedHeadServiceConfig.ConfigurationStatus = "Opening Animated Head on Port " + _state.AnimatedHeadServiceConfig.CommPort.ToString();
                    _state.Connected = _ahConnection.Open(_state.AnimatedHeadServiceConfig.CommPort, _state.AnimatedHeadServiceConfig.BaudRate);
                    if (_state.Connected)
                    {
                        _state.AnimatedHeadServiceConfig.ConfigurationStatus = "Connected at saved Port " + _ahConnection.getPortName();
                    }
                }
                else
                {
                    _state.AnimatedHeadServiceConfig.ConfigurationStatus = "Searching for the Animated Head Port";
                    _state.Connected = _ahConnection.FindAnimatedHead();
                    if (_state.Connected)
                    {
                        _state.AnimatedHeadServiceConfig.ConfigurationStatus = "Connected at found Port " + _ahConnection.getPortName();
                        _state.AnimatedHeadServiceConfig = _ahConnection.AnimatedHeadConfig;
                        SaveState(_state);
                    }
                }

                if (_state.Connected)
                {
                    brickConnector.Open(_ahConnection);
                    //SafePosture();
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
                _state.AnimatedHeadServiceConfig.ConfigurationStatus = "Not Connected";
                string msg = "The Animated Head is not detected.\r\n*   To configure the Animated Head, navigate to: ";
                LogInfo(LogGroups.Console, msg);
                Console.WriteLine(msg);
            }
            else
            {
                string msg = "The Animated Head connected at Port " + _ahConnection.getPortName();
                LogInfo(LogGroups.Console, msg);
                Console.WriteLine(msg);
            }
            yield break;
        }

        #endregion // AnimatedHead Serial Connection Helpers

        #region Animatronics related

        private void ClearAnimations()
        {
            brickConnector.clearAnimations();
        }

        private void DefaultAnimations()
        {
            brickConnector.setDefaultAnimations();
        }

        /// <summary>
        /// if combo name starts with "+", do not clear animations. The new combo will overwrite only channels that it includes. 
        /// </summary>
        /// <param name="comboName"></param>
        /// <param name="scale"></param>
        /// <param name="doRepeat"></param>
        private void SetAnimCombo(string comboName, double scale, bool doRepeat)
        {
            if (!string.IsNullOrEmpty(comboName) && comboName.StartsWith("+"))
            {
                comboName = comboName.Substring(1);
            }
            else
            {
                brickConnector.clearAnimations();
            }

            if (!string.IsNullOrEmpty(comboName) && animationCombo.ContainsKey(comboName))
            {
                string[] animNames = animationCombo[comboName];
                SetAnimations(animNames, scale, doRepeat);
            }
            else
            {
                // Valid Combo names:
                //  Acknowledge, Afraid, Alert, Alert_lookleft, Alert_lookright, Angry, Blink, Blink1, BlinkCycle, Decline, Look_down, Look_up, Lookaround,
                //  Mad, Mad2, Notgood, Ohno, Pleased, Restpose, Sad,
                //  Smallturnleft, Smallturnleftdow, Smallturnrigh, Smallturnrightdow,
                //  Smile, Surprised, Think, Turn_left, Turn_left_smile, Turn_right, Turn_right_smile, Upset, What, Wow, Yeah, Yell, Talk

                Console.WriteLine("Error: SetAnimCombo() - combo name not valid: " + comboName + "\r\nValid names:\r\n" + string.Join(", ", animationCombo.Keys));
            }
        }

        private void AddAnim(string[] animNames, double scale, bool doRepeat)
        {
            SetAnimations(animNames, scale, doRepeat);
        }

        private void SetAnim(string[] animNames, double scale, bool doRepeat)
        {
            brickConnector.clearAnimations();
            SetAnimations(animNames, scale, doRepeat);
        }

        private void SetAnimations(string[] animNames, double scale = 1.0d, bool doRepeat = false)
        {
            foreach (string name in animNames)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    Animation anim = animations[name];

                    brickConnector.setAnimation(anim, scale, doRepeat);
                }
            }
        }

        #endregion // Animatronics related
    }
}


