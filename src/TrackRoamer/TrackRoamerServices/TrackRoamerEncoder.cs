
//#define TRACEDEBUG
//#define TRACEDEBUGTICKS
#define TRACELOG

using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Permissions;

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

using TrackRoamer.Robotics.Utility.LibSystem;

using submgr = Microsoft.Dss.Services.SubscriptionManager;
using pxencoder = Microsoft.Robotics.Services.Encoder.Proxy;

using powerbrick = TrackRoamer.Robotics.Services.TrackRoamerBrickPower.Proxy;

namespace TrackRoamer.Robotics.Services.TrackRoamerServices.Encoder
{
    [Contract(Contract.Identifier)]
    [AlternateContract(pxencoder.Contract.Identifier)]
    [DisplayName("(User) TrackRoamer Encoder")]
	[Description("Provides access to the TrackRoamer encoder.\n(Uses the Generic Encoder contract.)")]
    public class EncoderService : DsspServiceBase
    {
		[EmbeddedResource("TrackRoamer.Robotics.Services.TrackRoamerServices.TrackRoamerEncoder.xslt")]
        string _transform = null;
        
        [InitialStatePartner(Optional=true)]
        private pxencoder.EncoderState _state = new pxencoder.EncoderState();

		private double? m_lastResetTicks;
        private object m_lastResetTicksLock = new object();

		private bool _subscribed = false;

		[ServicePort("/TrackRoamerEncoder", AllowMultipleInstances = true)]
        private pxencoder.EncoderOperations _mainPort = new pxencoder.EncoderOperations();

        [Partner("TrackRoamerPowerBrick", Contract = powerbrick.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate, Optional = false)]
        private powerbrick.TrackRoamerBrickPowerOperations _trackroamerbotPort = new powerbrick.TrackRoamerBrickPowerOperations();
        // Create a notification port; we will get only UpdateMotorEncoder on it:
        private Port<powerbrick.UpdateMotorEncoder> notificationPortEncoder = new Port<powerbrick.UpdateMotorEncoder>();

        [SubscriptionManagerPartner]
        private submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        public EncoderService(DsspServiceCreationPort creationPort) : 
                base(creationPort)
        {
        }

		protected override void Start()
		{
			// Configure default state. It will be replaced by the drive during configuration.
			if (_state == null)
			{
				_state = new pxencoder.EncoderState();
				_state.HardwareIdentifier = -1;			// 1=Left 2=Right
				_state.TicksPerRevolution = 100;

				SaveState(_state);
			}

            base.Start();

            MainPortInterleave.CombineWith(new Interleave(
                new ExclusiveReceiverGroup(
                    // prepare listening for Power Brick's encoder change notifications
                    Arbiter.Receive<powerbrick.UpdateMotorEncoder>(true, notificationPortEncoder, MotorEncoderNotificationHandler)
                ),
                new ConcurrentReceiverGroup()
            ));

			// display HTTP service Uri
			LogInfo("Service uri: " + ServiceInfo.HttpUri());

			// Subscribe to the Hardware Controller for encoder notifications
			if (ValidState(_state))
			{
				SubscribeToPowerBrick();
			}
		}

		private static bool ValidState(pxencoder.EncoderState state)
		{
			if (state != null)
			{
				if (state.HardwareIdentifier >= 1 && state.HardwareIdentifier <= 2)
				{
					return true;
				}
			}
			return false;
		}

        public string Name {
            get
            {
                string ret = "(INVALID STATE)";

                if (ValidState(_state))
                {
                    ret = (_state.HardwareIdentifier == 1 ? "Left" : "Right");
                }
                return ret + " Encoder";
            }
        }

		private void SubscribeToPowerBrick()
		{
            Type[] notifyMeOf = new Type[] { typeof(powerbrick.UpdateMotorEncoder) };

            Tracer.Trace("TrackRoamerEncoder:  calling Subscribe() for UpdateMotorEncoder");

            // Subscribe to the Power Brick and wait for a response
            Activate(
                Arbiter.Choice(_trackroamerbotPort.Subscribe(notificationPortEncoder, notifyMeOf),
                    delegate(SubscribeResponseType Rsp)
                    {
                        // Subscription was successful, update our state with subscription status:
                        _subscribed = true;
                        LogInfo("EncoderService: " + Name + " Subscription to Power Brick Service succeeded");
                    },
                    delegate(Fault F)
                    {
                        LogError("EncoderService: " + Name + " Subscription to Power Brick Service failed");
                    }
                )
            );
		}

        /// <summary>
        /// we are getting encoder ticks for both sides from Power Brick here 
        /// </summary>
        /// <param name="update"></param>
		private void MotorEncoderNotificationHandler(powerbrick.UpdateMotorEncoder update)
		{
			if (_state.HardwareIdentifier != update.Body.HardwareIdentifier)
			{
#if TRACEDEBUGTICKS
            //LogInfo("TrackRoamerEncoder:MotorEncoderNotificationHandler() " + Name + "  ignored other side -- update: left=" + update.Body.LeftDistance + "  right=" + update.Body.RightDistance);
#endif // TRACEDEBUGTICKS
				// addressed to the other side encoder, ignore it
				return;
			}

#if TRACEDEBUGTICKS
            LogInfo("TrackRoamerEncoder:MotorEncoderNotificationHandler() " + Name + "  got update: left=" + update.Body.LeftDistance + "  right=" + update.Body.RightDistance);
#endif // TRACEDEBUGTICKS

			bool changed = false;
			int val = 0;
			double dval = 0.0d;
			double dabs = 0.0d;

            if (_state.HardwareIdentifier == 1 && update.Body.LeftDistance != null)
            {
                lock (m_lastResetTicksLock)
                {
                    // Generic Drive operates on positive tick counter values
                    dabs = (double)update.Body.LeftDistance;

                    if (m_lastResetTicks == null)
                    {
                        m_lastResetTicks = dabs;
                    }
                    else
                    {
                        dval = Math.Abs(dabs - (double)m_lastResetTicks);
                        val = (int)dval;
                    }

                    changed = _state.TicksSinceReset != val;

#if TRACEDEBUGTICKS
                    //LogInfo("TrackRoamerEncoder:MotorEncoderNotificationHandler() --  left: " + _state.TicksSinceReset + " --> " + val + "  (" + update.Body.LeftDistance + ")");
#endif // TRACEDEBUGTICKS
                }
            }

            if (_state.HardwareIdentifier == 2 && update.Body.RightDistance != null)
            {
                lock (m_lastResetTicksLock)
                {
                    // Generic Drive operates on positive tick counter values
                    dabs = (double)update.Body.RightDistance;

                    if (m_lastResetTicks == null)
                    {
                        m_lastResetTicks = dabs;
                    }
                    else
                    {
                        dval = Math.Abs(dabs - (double)m_lastResetTicks);
                        val = (int)dval;
                    }

                    changed = _state.TicksSinceReset != val;

#if TRACEDEBUGTICKS
                    //LogInfo("TrackRoamerEncoder:MotorEncoderNotificationHandler() -- right: " + _state.TicksSinceReset + " --> " + val + "  (" + update.Body.RightDistance + ")");
#endif // TRACEDEBUGTICKS
                }
            }

			if (changed)
			{
                //LogInfo("TrackRoamerEncoder:MotorEncoderNotificationHandler() --  "
                //                    + (_state.HardwareIdentifier == 1 ? " left: " : "right: ")
                //                    + _state.TicksSinceReset + " --> " + val + "  (" + dabs + ")");

				_state.TicksSinceReset = val;
                _state.CurrentReading = val;
                _state.CurrentAngle = val * 2.0d * Math.PI / _state.TicksPerRevolution;
				//update time
				_state.TimeStamp = DateTime.Now;
				this.SendNotification<pxencoder.UpdateTickCount>(_subMgrPort, new pxencoder.UpdateTickCountRequest(_state.TimeStamp, _state.TicksSinceReset));
			}

            //update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetHandler(pxencoder.Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// HttpGet Handler
        /// </summary>
        /// <param name="httpGet"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> HttpGetHandler(HttpGet httpGet)
        {
            httpGet.ResponsePort.Post(new HttpResponseType(
                HttpStatusCode.OK,
                _state,
                _transform)
            );
            yield break;
        }

        /// <summary>
        /// Reset Handler
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ResetHandler(pxencoder.Reset update)
        {
            LogInfo("TrackRoamerEncoder : ResetHandler()  m_lastResetTicks=" + m_lastResetTicks);

            // reset physical counter in the bot AX2850 controller:
            //trackroamerbot.ResetMotorEncoder resetMotorEncoder = new trackroamerbot.ResetMotorEncoder();
            //resetMotorEncoder.Body.HardwareIdentifier = _state.HardwareIdentifier;
            //_trackroamerbotPort.Post(resetMotorEncoder);

            // initialize our local logical counter:
            lock (m_lastResetTicksLock)
            {
                _state.TicksSinceReset = 0;
                _state.CurrentReading = 0;
                _state.CurrentAngle = 0.0d;
                m_lastResetTicks = null;
            }

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }


        /// <summary>
        /// Replace Handler
        /// </summary>
        /// <param name="replace"></param>
        /// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReplaceHandler(pxencoder.Replace replace)
        {
            //LogInfo("TrackRoamerEncoder : ReplaceHandler()");

			if (_subscribed)
			{
				LogError("TrackRoamerEncoder : ReplaceHandler(): Already subscribed");
			}
			else if (ValidState(replace.Body))
			{
				//_state = (TREncoderState)replace.Body;
				_state = replace.Body;
				SaveState(_state);
				replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
				SubscribeToPowerBrick();
			}
			else
			{
				LogError("TrackRoamerEncoder : ReplaceHandler(): Invalid State for replacement");
			}

            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Teardown)]
        public virtual IEnumerator<ITask> DropHandler(DsspDefaultDrop drop)
        {
            LogInfo("TrackRoamerEncoder:DropHandler()");

            base.DefaultDropHandler(drop);

            yield break;
        }

		#region Subscriptions
        /// <summary>
        /// Subcribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public IEnumerator<ITask> SubscribeHandler(pxencoder.Subscribe subscribe)
		{
            LogInfo("TrackRoamerEncoder " + _state.HardwareIdentifier + " received Subscription request from Subscriber=" + subscribe.Body.Subscriber);

			yield return Arbiter.Choice(
				SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
				delegate(SuccessResult success)
				{
                    LogInfo("TrackRoamerEncoder : Subscription granted (succeeded) subscriber: " + subscribe.Body.Subscriber);
                    //_subMgrPort.Post(new submgr.Submit(subscribe.Body.Subscriber, DsspActions.ReplaceRequest, _state, null));
				},
				delegate(Exception fault)
				{
					LogError(fault);
				}
			);
		}

        /// <summary>
        /// Reliable Subscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
		[ServiceHandler(ServiceHandlerBehavior.Exclusive)]
		public IEnumerator<ITask> ReliableSubscribeHandler(pxencoder.ReliableSubscribe subscribe)
		{
            LogInfo("TrackRoamerEncoder " + _state.HardwareIdentifier + " received Reliable Subscription request from Subscriber=" + subscribe.Body.Subscriber);

            yield return Arbiter.Choice(
				SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
				delegate(SuccessResult success)
				{
                    LogInfo("TrackRoamerEncoder : Reliable Subscription granted (succeeded) subscriber: " + subscribe.Body.Subscriber);
                    //_subMgrPort.Post(new submgr.Submit(subscribe.Body.Subscriber, DsspActions.ReplaceRequest, _state, null));
				},
				delegate(Exception fault)
				{
					LogError(fault);
				}
			);
		}
		#endregion

#if TRACEDEBUG
        protected new void LogInfo(string str)
        {
            Tracer.Trace(str);
        }
        protected new void LogError(string str)
        {
            Tracer.Error(str);
        }
#endif // TRACEDEBUG
    }
}
