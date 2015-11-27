using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using trackroamerbot = TrackRoamer.Robotics.Services.TrackRoamerBot.Proxy;
using follower = Microsoft.Robotics.Services.Sample.Follower.Proxy;

namespace TrackRoamerFollower
{
    [Contract(Contract.Identifier)]
    [DisplayName("TrackRoamerFollower")]
    [Description("TrackRoamerFollower service (no description provided)")]
    [AlternateContract(Microsoft.Robotics.Services.Sample.Follower.Contract.Identifier)]
    class TrackRoamerFollowerService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        follower.FollowerState _state = new follower.FollowerState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/TrackRoamerFollower", AllowMultipleInstances = true)]
        follower.FollowerOperations _mainPort = new follower.FollowerOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// TrackRoamerBotService partner
        /// </summary>
        [Partner("TrackRoamerBotService", Contract = trackroamerbot.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        trackroamerbot.TrackRoamerBotOperations _trackRoamerBotServicePort = new trackroamerbot.TrackRoamerBotOperations();
        trackroamerbot.TrackRoamerBotOperations _trackRoamerBotServiceNotify = new trackroamerbot.TrackRoamerBotOperations();

        /// <summary>
        /// TrackRoamerBotService partner
        /// </summary>
        [Partner("TrackRoamerBotService", Contract = trackroamerbot.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        trackroamerbot.TrackRoamerBotOperations _trackRoamerBotServicePort = new trackroamerbot.TrackRoamerBotOperations();

        /// <summary>
        /// Service constructor
        /// </summary>
        public TrackRoamerFollowerService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {

            // 
            // Add service specific initialization here
            // 

            base.Start();
        }

        /// <summary>
        /// Handles SetMotorActivate requests
        /// </summary>
        /// <param name="setmotoractivate">request message</param>
        [ServiceHandler]
        public void SetMotorActivateHandler(follower.SetMotorActivate setmotoractivate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles SetRobotBehavior requests
        /// </summary>
        /// <param name="setrobotbehavior">request message</param>
        [ServiceHandler]
        public void SetRobotBehaviorHandler(follower.SetRobotBehavior setrobotbehavior)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles Get requests
        /// </summary>
        /// <param name="get">request message</param>
        [ServiceHandler]
        public void GetHandler(follower.Get get)
        {
            throw new NotImplementedException();
        }
    }
}


