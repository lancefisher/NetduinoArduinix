using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NetduinoArduinix
{
    public class ArduinixShield
    {
        /// <summary>
        /// There are two SN74141 chips on the Arduinix. These Netduino Output ports are connected
        /// to the inputs of the first SN74141. 
        /// </summary>
        /// <remarks>
        /// SN74141 Truth Table
        /// D C B A #
        /// L,L,L,L 0
        /// L,L,L,H 1
        /// L,L,H,L 2
        /// L,L,H,H 3
        /// L,H,L,L 4
        /// L,H,L,H 5
        /// L,H,H,L 6
        /// L,H,H,H 7
        /// H,L,L,L 8
        /// H,L,L,H 9
        /// </remarks>
        public static OutputPort[] SN74141AInputs = 
            {
                new OutputPort(Pins.GPIO_PIN_D2, false), //A
                new OutputPort(Pins.GPIO_PIN_D3, false), //B
                new OutputPort(Pins.GPIO_PIN_D4, false), //C
                new OutputPort(Pins.GPIO_PIN_D5, false)  //D
            };

        /// <summary>
        /// There are two SN74141 chips on the Arduinix. These Netduino Output ports are connected
        /// to the inputs of the second SN74141. 
        /// </summary>
        public static OutputPort[] SN74141BInputs = 
            {
                new OutputPort(Pins.GPIO_PIN_D6, false), //A
                new OutputPort(Pins.GPIO_PIN_D7, false), //B
                new OutputPort(Pins.GPIO_PIN_D8, false), //C
                new OutputPort(Pins.GPIO_PIN_D9, false)  //D
            };

        /// <summary>
        /// Writing to these Netduino OutputPorts fires the high voltage anode on the Arduinix Shield
        /// </summary>
        public static OutputPort[] Anodes = 
            {
                new OutputPort(Pins.GPIO_PIN_D10, false),
                new OutputPort(Pins.GPIO_PIN_D11, false),
                new OutputPort(Pins.GPIO_PIN_D12, false),
                //todo: support the 4th anode for larger displays
            };

        /// <summary>
        /// Displays the numbers on the nixie tubes. Designed for 6 tube display.
        /// Each integer should be &lt; 16. 0-9 will show on the display. 11-15 will blank that digit. 
        /// Integers greater than 15 start over at 0. e.g. 16 displays 0, 17 displays 1, etc.
        /// </summary>
        public void DisplayNumbers(int number1, int number2, int number3, int number4, int number5, int number6)
        {
            //I'm not sure if my anodes are wired to my display in the standard way. -Lance
            DisplaySet(Anodes[1], number1, number4);
            DisplaySet(Anodes[0], number5, number2);
            DisplaySet(Anodes[2], number3, number6);            
        }

        /// <summary>
        /// Displays the number on the arduinix display.
        /// </summary>
        /// <param name="number">
        /// The number to display. For a six-digit display, 
        /// the number should be less than 999,999.
        /// </param>
        /// <param name="displayMode">
        /// The number can be padded with zeros to the left, 
        /// or aligned to the right or left with leading zeros blanked.
        /// </param>
        public void DisplayNumber(int number, DisplayMode displayMode = DisplayMode.ZeroPadded)
        {
            //Get each digit of the number, up to 6 digits
            int[] digits = 
            {
                number / 100000 % 10, 
                number / 10000 % 10, 
                number / 1000 % 10, 
                number / 100 % 10, 
                number / 10 % 10, 
                number % 10
            };

            if (displayMode == DisplayMode.AlignRight)
            {
                //blank leading zeros
                for (var i = 0; i < digits.Length; i++)
                {
                    if (digits[i] != 0) break;
                    digits[i] = 10; //setting to 10 will blank that digit
                }
            }

            if (displayMode == DisplayMode.AlignLeft)
            {
                var leadingZeroCount = 0;
                for (var i = 0; i < digits.Length; i++)
                {
                    if (digits[i] != 0) break;
                    leadingZeroCount++;
                }

                //move the digits left
                for (var i = 0; i < digits.Length - leadingZeroCount; i++)
                {
                    digits[i] = digits[i + leadingZeroCount];
                }

                //blank the old positions
                for (var i = digits.Length - leadingZeroCount; i < digits.Length; i++)
                {
                    digits[i] = 10;
                }
            }

            DisplayNumbers(
                digits[0],
                digits[1],
                digits[2],
                digits[3],
                digits[4],
                digits[5]
            );
        }

        public enum DisplayMode
        {
            ZeroPadded,
            AlignRight,
            AlignLeft
        }

        //digits, anode, chip
        //1, 2, 1
        //2, 1, 1
        //3, 3, 1
        //4, 2, 2
        //5, 1, 2
        //6, 3, 2
        private static void DisplaySet(OutputPort anode, int number1, int number2)
        {
            var input1 = GetInput(number1);
            WriteInput(input1, SN74141AInputs);

            var input2 = GetInput(number2);
            WriteInput(input2, SN74141BInputs);

            anode.Write(true);
            Thread.Sleep(2);
            anode.Write(false);
        }


        private static bool[] GetInput(int number)
        {
            //Get an array of bool that represents the binary version of number...
            var b = new[]
                        {
                            (number & 1) != 0,
                            (number & 2) != 0,
                            (number & 4) != 0,
                            (number & 8) != 0,
                        };

            return b;
        }

        private static void WriteInput(bool[] input, OutputPort[] inputs)
        {
            for (var i = 0; i < input.Length; i++)
                inputs[i].Write(input[i]);
        }

    }
}