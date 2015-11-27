
namespace TrackRoamer.Robotics.Services.TrackroamerRP2011AbstractionLayer
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    using Microsoft.Ccr.Core;
    using Microsoft.Dss.Core.Attributes;
    using Microsoft.Dss.Core.DsspHttp;
    using Microsoft.Dss.ServiceModel.Dssp;
    using Microsoft.Dss.ServiceModel.DsspServiceBase;

    using battery = Microsoft.Robotics.Services.Battery;
    using soap = W3C.Soap;
    using submgr = Microsoft.Dss.Services.SubscriptionManager;

    using TrackRoamer.Robotics.Utility.LibSystem;

    // see C:\Microsoft Robotics Dev Studio 4\samples\Platforms\ReferencePlatform2011\MarkRobot\Battery.cs

    /// <summary>
    /// Main class for the service (the Battery related part of it)
    /// </summary>
    public partial class TrackroamerRP2011AbstractionLayerService : DsspServiceBase
    {
        /// <summary>
        /// GPIO Port Identifier used in attributes
        /// </summary>
        private const string BatteryPortName = "batteryPort";

        /// <summary>
        /// Alternate contract service port
        /// </summary>
        [AlternateServicePort(AlternateContract = battery.Contract.Identifier)]
        private battery.BatteryOperations batteryPort = new battery.BatteryOperations();

        /// <summary>
        /// GPIO Pin Array service subscription port
        /// </summary>
        [SubscriptionManagerPartner("battery")]
        private submgr.SubscriptionManagerPort submgrBatteryPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Handles Get requests on alternate port
        /// </summary>
        /// <param name="get">Request message</param>
        [ServiceHandler(PortFieldName = BatteryPortName)]
        public void BatteryGetHandler(battery.Get get)
        {
            get.ResponsePort.Post(this.state.BatteryState);
        }

        /// <summary>
        /// Handles HttpGet requests on alternate port
        /// </summary>
        /// <param name="httpget">Request message</param>
        [ServiceHandler(PortFieldName = BatteryPortName)]
        public void BatteryHttpGetHandler(Microsoft.Dss.Core.DsspHttp.HttpGet httpget)
        {
            HttpResponseType resp = new HttpResponseType(HttpStatusCode.OK, this.state.BatteryState);
            httpget.ResponsePort.Post(resp);
        }

        /// <summary>
        /// Sets the voltage level at which subscribe notifications will be sent
        /// indicating that the battery has reached a detrimental low power state
        /// </summary>
        /// <param name="setcriticallevel">Percentage of battery voltage</param>
        [ServiceHandler(PortFieldName = BatteryPortName)]
        public void BatterySetCriticalLevel(battery.SetCriticalLevel setcriticallevel)
        {
            this.state.BatteryState.PercentCriticalBattery = setcriticallevel.Body.PercentCriticalBattery;
            this.SendNotification(this.submgrBatteryPort, setcriticallevel);
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">The subscribe request</param>
        /// <returns>Enumerator of type ITask</returns>
        [ServiceHandler(PortFieldName = BatteryPortName)]
        public IEnumerator<ITask> BatterySubscribeHandler(battery.Subscribe subscribe)
        {
            SuccessFailurePort responsePort = SubscribeHelper(this.submgrBatteryPort, subscribe.Body, subscribe.ResponsePort);
            yield return responsePort.Choice();

            var success = (SuccessResult)responsePort;
            if (success != null)
            {
                SendNotificationToTarget<Replace>(subscribe.Body.Subscriber, this.submgrBatteryPort, this.state.BatteryState);
            }

            yield break;
        }
    }
}