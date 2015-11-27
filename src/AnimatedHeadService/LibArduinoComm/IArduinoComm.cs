using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trackroamer.Library.LibArduinoComm
{
    public interface IArduinoComm
    {
        bool Open(string comPort);
        string getPortName();
        void Close();

        void SendToArduino(ToArduino toArduino);
        void SendToArduino2(ToArduino toArduino1, ToArduino toArduino2);
    }
}
