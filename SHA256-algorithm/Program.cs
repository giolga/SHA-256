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

        private static uint H0 = 0x6a09e667;
        private static uint H1 = 0xbb67ae85;
        private static uint H2 = 0x3c6ef372;
        private static uint H3 = 0xa54ff53a;
        private static uint H4 = 0x510e527f;
        private static uint H5 = 0x9b05688c;
        private static uint H6 = 0x1f83d9ab;
        private static uint H7 = 0x5be0cd19;

        // These values are the first 32 bits of the fractional parts of the cube roots of the first 64 prime numbers
        // These constants prevent predictable collisions in the hash function
        private static readonly uint[] K = {
    0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
    0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
    0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
    0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
    0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
    0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
    0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
    0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
    0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
    0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
    0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
    0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
    0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
    0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
    0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
    0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
};


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

        private static List<string> DefineBlocks(string paddedMessage) //STEP - 4
        {
            List<string> blocks = new List<string>();

            int n = paddedMessage.Length / 512;

            for (int i = 0; i < n; i++)
            {
                blocks.Add(paddedMessage.Substring(i * 512, 512));
            }

            return blocks;
        }

        private static uint[] MessageSchedule(string block) //STEP - 5
        {
            uint[] W = new uint[64];

            for(int i = 0; i < 16; i++)
            {
                W[i] = Convert.ToUInt32(block.Substring(i * 32, 32), 2);
            }

            for (int i = 16; i < 64; i++)
            {
                uint s0 = RightRotate(W[i - 15], 7) ^ RightRotate(W[i - 15], 18) ^ (W[i - 15] >> 3);
                uint s1 = RightRotate(W[i - 2], 17) ^ RightRotate(W[i - 2], 19) ^ (W[i - 2] >> 10);

                W[i] = W[i - 16] + s0 + W[i - 7] + s1;
            }

            return W;
        }

        private static uint RightRotate(uint value, int bits)
        {
            return (value >> bits) | (value << (32 - bits));
        }

        private static void CompressionLoop(uint[] W)
        {
            // Initialize working variables with current hash values
            uint a = H0, b = H1, c = H2, d = H3;
            uint e = H4, f = H5, g = H6, h = H7;

            // Perform 64 rounds of compression
            for (int i = 0; i < 64; i++)
            {
                uint S1 = RightRotate(e, 6) ^ RightRotate(e, 11) ^ RightRotate(e, 25);
                uint ch = (e & f) ^ (~e & g);
                uint temp1 = h + S1 + ch + K[i] + W[i];

                uint S0 = RightRotate(a, 2) ^ RightRotate(a, 13) ^ RightRotate(a, 22);

                // picks the majority bit among a, b, c
                // It returns 1 if at least two out of three are 1s, otherwise, it returns 0
                uint maj = (a & b) ^ (a & c) ^ (b & c);
                uint temp2 = S0 + maj;

                h = g;
                g = f;
                f = e;
                e = d + temp1;
                d = c;
                c = b;
                b = a;
                a = temp1 + temp2;
            }

            //Update hash values
            H0 += a;
            H1 += b;
            H2 += c;
            H3 += d;
            H4 += e;
            H5 += f;
            H6 += g;
            H7 += h;
        }
        static void Main(string[] args)
        {
            //Console.BackgroundColor = ConsoleColor.Black;
            //Console.Clear(); // changing the background color
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;

            string s = Console.ReadLine();

            byte[] bytes = Encoding.UTF8.GetBytes(s);
            string bin = "";

            // Convert each byte to binary and append to bin string
            foreach (byte b in bytes)
            {
                bin += BinaryValue(b);
                messageBits.Add(BinaryValue(b));
            }

            Console.WriteLine(bin.Length + " " + s.Length * 8);
            Console.Write("Binary value of string: ");
            messageBits.ForEach(bits => Console.Write($"{bits} "));
            Console.WriteLine($"\nThe count of messageBits: {messageBits.Count} --- String length {s.Length}");
            //Console.WriteLine($"448 bit value: {MessagePadding(bin)}");
            Console.WriteLine($"n 512 bit value: {MessagePadding(bin) + SixtyFourRepresentation(s.Length * 8)}");

            string paddedMessage = MessagePadding(bin) + SixtyFourRepresentation(s.Length * 8);
            //Console.WriteLine("Padded Message {0}", paddedMessage);
            Console.WriteLine($"The total length is {MessagePadding(bin).Length + SixtyFourRepresentation(s.Length * 8).Length}");
            Console.WriteLine($"Number of blocks: {(MessagePadding(bin).Length + SixtyFourRepresentation(s.Length * 8).Length) / 512} \n--------------");

            List<string> blocks = DefineBlocks(paddedMessage);
            blocks.ForEach(block => Console.WriteLine(block.Length));

            foreach(string block in blocks)
            {
                uint[] W = MessageSchedule(block);
                CompressionLoop(W);
            }

            string finalHash = string.Join("", new uint[] { H0, H1, H2, H3, H4, H5, H6, H7 }
                                .Select(h => h.ToString("x8")));  // Format as 8-digit hexadecimal

            Console.WriteLine($"SHA-256: {finalHash}");

            Console.ReadKey();
        }

    }
}
