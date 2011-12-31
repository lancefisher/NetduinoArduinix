using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware;

namespace NetduinoArduinix
{
    public class Program
    {

        public static void Main()
        {
            var ardunix = new ArduinixShield();

            while (true)
            {
                ardunix.DisplayNumber(123456);
            }

        }

    }

}