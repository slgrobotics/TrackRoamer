using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using animatedheadservice = TrackRoamer.Robotics.Services.AnimatedHeadService.Proxy;

namespace TrackRoamer.Robotics.Services.AnimatedHeadTestService
{
    [Contract(Contract.Identifier)]
    [DisplayName("AnimatedHeadTestService")]
    [Description("AnimatedHeadTestService service (no description provided)")]
    class AnimatedHeadTestService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        AnimatedHeadTestServiceState _state = new AnimatedHeadTestServiceState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/AnimatedHeadTestService", AllowMultipleInstances = false)]
        AnimatedHeadTestServiceOperations _mainPort = new AnimatedHeadTestServiceOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// AnimatedHeadService partner
        /// </summary>
        [Partner("AnimatedHeadService", Contract = animatedheadservice.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        animatedheadservice.AnimatedHeadServiceOperations _animatedHeadCommandPort = new animatedheadservice.AnimatedHeadServiceOperations();
        animatedheadservice.AnimatedHeadServiceOperations _animatedHeadServiceNotify = new animatedheadservice.AnimatedHeadServiceOperations();

        /// <summary>
        /// Service constructor
        /// </summary>
        public AnimatedHeadTestService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            Console.WriteLine("AnimatedHeadTestService - Start");

            base.Start();

            SpawnIterator(this.TestAnimatedHead);
        }

        #region TestAnimatedHead

        /// <summary>
        /// for a test, uncomment the call to this metod above and watch console
        /// </summary>
        /// <returns></returns>
        private IEnumerator<ITask> TestAnimatedHead()
        {
            Console.WriteLine("AnimatedHeadTestService:TestAnimatedHead - started");

            yield return Timeout(8000);

            Console.WriteLine("AnimatedHeadTestService:TestAnimatedHead - ClearAnimations");

            animatedheadservice.ArduinoDeviceCommand cmd = new animatedheadservice.ArduinoDeviceCommand() { Command = animatedheadservice.AnimatedHeadCommands.ANIMATIONS_CLEAR };

            _animatedHeadCommandPort.Post(new animatedheadservice.SendArduinoDeviceCommand(cmd));

            yield return Timeout(4000);

            Console.WriteLine("AnimatedHeadTestService:TestAnimatedHead - DefaultAnimations");

            cmd = new animatedheadservice.ArduinoDeviceCommand() { Command = animatedheadservice.AnimatedHeadCommands.ANIMATIONS_DEFAULT };

            _animatedHeadCommandPort.Post(new animatedheadservice.SendArduinoDeviceCommand(cmd));

            yield return Timeout(3000);

            Console.WriteLine("AnimatedHeadTestService:TestAnimatedHead - SetAnimCombo('Mad')");

            // Valid Combo names:
            //  Acknowledge, Afraid, Alert, Alert_lookleft, Alert_lookright, Angry, Blink, Blink1, BlinkCycle, Decline, Look_down, Look_up, Lookaround,
            //  Mad, Mad2, Notgood, Ohno, Pleased, Restpose, Sad,
            //  Smallturnleft, Smallturnleftdow, Smallturnrigh, Smallturnrightdow,
            //  Smile, Surprised, Think, Turn_left, Turn_left_smile, Turn_right, Turn_right_smile, Upset, What, Wow, Yeah, Yell, Talk

            cmd = new animatedheadservice.ArduinoDeviceCommand() { Command = animatedheadservice.AnimatedHeadCommands.SetAnimCombo, Args = "Mad", Scale = 0.2d, doRepeat = true };

            _animatedHeadCommandPort.Post(new animatedheadservice.SendArduinoDeviceCommand(cmd));

            yield return Timeout(4000);

            Console.WriteLine("AnimatedHeadTestService:TestAnimatedHead - SetAnimCombo('Acknowledge')");

            cmd = new animatedheadservice.ArduinoDeviceCommand() { Command = animatedheadservice.AnimatedHeadCommands.SetAnimCombo, Args = "Acknowledge", Scale = 0.2d, doRepeat = true };

            _animatedHeadCommandPort.Post(new animatedheadservice.SendArduinoDeviceCommand(cmd));

            yield return Timeout(4000);

            Console.WriteLine("AnimatedHeadTestService:TestAnimatedHead - ClearAnimations");

            cmd = new animatedheadservice.ArduinoDeviceCommand() { Command = animatedheadservice.AnimatedHeadCommands.ANIMATIONS_CLEAR };

            _animatedHeadCommandPort.Post(new animatedheadservice.SendArduinoDeviceCommand(cmd));

            yield break;
        }

        #endregion // TestAnimatedHead

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }
    }
}


