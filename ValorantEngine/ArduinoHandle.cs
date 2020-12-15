using System;
using System.IO.Ports;

namespace ValorantEngine
{
    public class ArduinoHandle
    {
        //Port the arduino runs on
        private SerialPort port;

        //The arduino can only move the mouse a maximum of 127 pixels in any given direction at once
        private int roundtosignedchar(int a)
        {
            if (a < -127)
            {
                return -127;
            }
            else if (a > 127)
            {
                return 127;
            }
            else
            {
                return a;
            }
        }

        public ArduinoHandle(string COMPORT)
        {
            port = new SerialPort(COMPORT, 115200);
            port.Open();
        }

        public void MoveMouse(int x, int y)
        {
            port.Write(BitConverter.GetBytes(roundtosignedchar(x)), 0, 1);
            port.Write(BitConverter.GetBytes(roundtosignedchar(y)), 0, 1);
        }

        public void ClosePort()
        {
            port.Close();
        }
    }
}
