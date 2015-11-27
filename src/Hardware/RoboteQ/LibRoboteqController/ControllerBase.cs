using System;
using System.Collections;

using LibSystem;

namespace LibRoboteqController
{
	/// <summary>
	/// ControllerBase - base class for all motor Controllers
	/// </summary>
	public abstract class ControllerBase : IDisposable
	{
		public ControllerBase()		{}
		public abstract void init();				// opens port, starts monitoring it, readies to send commands. May throw ControllerException

		public abstract void IdentifyDeviceType();	// throws ControllerException
		public abstract bool DeviceValid();			// IdentifyDeviceType found something real

		public abstract bool GrabController();		// returns true on success
		public abstract bool ResetController();		// returns true on success

		public abstract int SetMotorPowerOrSpeedLeft(int powerOrSpeed);		// returns actual powerOrSpeed
		public abstract int SetMotorPowerOrSpeedRight(int powerOrSpeed);	// returns actual powerOrSpeed

		public abstract void Dispose();
	}
}
