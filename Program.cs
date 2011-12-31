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
            //todo: var display = new Ardunix6DigitDisplay(ardunix);

            while (true)
            {
                ardunix.DisplayNumber(3741, ArduinixShield.DisplayMode.AlignLeft);
            }
        }



    }

}