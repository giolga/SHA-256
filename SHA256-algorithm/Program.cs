using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SHA256_algorithm
{
    internal class Program
    {
        private static List<string> messageBits = new List<string>();
        private static List<string> nBlocks = new List<string>();

        private static string BinaryValue(int asciiValue) // STEP - 1
        {
            string bin = "";

            while (asciiValue > 0)
            {
                bin = (asciiValue % 2).ToString() + bin;
                asciiValue /= 2;
            }

            bin = bin.PadLeft(8, '0');

            return bin;
        }

        private static string MessagePadding(string incomingBits) // STEP - 2
        {
            string output = incomingBits + '1';

            int zeros = (448 - (incomingBits.Length % 512)) % 512;
            if (zeros < 0)
            {
                zeros += 512;
            }
            //MessageBox.Show($"Padding zeros: {zeros} ");
            zeros--; //in the beginning we added one extra '1' bit so that's why we decrement zeros

            while (zeros > 0)
            {
                output += '0';
                zeros--;
            }

            return output;
        }

        private static string SixtyFourRepresentation(int bits) // STEP - 3
        {
            string sixtyFourBits = "";

            while (bits > 0)
            {
                sixtyFourBits = (bits % 2).ToString() + sixtyFourBits;
                bits /= 2;
            }

            return sixtyFourBits.PadLeft(64, '0');
        }

        static void Main(string[] args)
        {
            //Console.BackgroundColor = ConsoleColor.Black;
            //Console.Clear(); // changing the background color
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;

            string s = Console.ReadLine();
            string bin = "";

            for (int i = 0; i < s.Length; i++)
            {
                bin += BinaryValue(Convert.ToInt32(s[i]));
                messageBits.Add(BinaryValue(Convert.ToInt32(s[i])));
            }

            Console.WriteLine(bin.Length + " " + s.Length * 8);
            Console.Write("Binary value of string: ");
            messageBits.ForEach(bits => Console.Write($"{bits} "));
            Console.WriteLine($"\nThe count of messageBits: {messageBits.Count} --- String length {s.Length}");
            //Console.WriteLine($"448 bit value: {MessagePadding(bin)}");
            Console.WriteLine($"n 512 bit value: {MessagePadding(bin) + SixtyFourRepresentation(s.Length * 8)}");

            Console.WriteLine($"The total length is {MessagePadding(bin).Length + SixtyFourRepresentation(s.Length * 8).Length}");
            Console.WriteLine($"Number of blocks: {(MessagePadding(bin).Length + SixtyFourRepresentation(s.Length * 8).Length) / 512}");

            Console.ReadKey();
        }

    }
}
