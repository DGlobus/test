using System;

namespace TestArea
{
    internal class MD5Decompose
    {
        static long A, B, C, D;

        static int[][] shiftAmount = new int[4][];
        static int[][] blockNumber = new int[4][];

        static void Main(string[] args)
        {
            string messageHex = GetHexMessage();

            string[] blocks32BitBin = GetBlocks(messageHex);

            InitializeVariable();

            long[] abcdInitialValue = { A, B, C, D };


            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(i);

                for (int j = 0; j < 16; j++)
                {
                    if (j == 0 || j == 15 || j == 1)
                    {
                        PrintVariables();
                    }

                    long functionValue = GetFunctionValue(i, B, C, D);
                    long constanta = (long)(Math.Pow(2, 32) * Math.Abs(Math.Sin(j + 1 + 16 * i)));
                    long currentBlock = Convert.ToInt64(blocks32BitBin[blockNumber[i][j] - 1], 2);


                    A = CalcilationModul32(A + functionValue);
                    A = CalcilationModul32(A + currentBlock);
                    A = CalcilationModul32(A + constanta);
                    A = CalcilationModul32(A << shiftAmount[i][j % 4]);
                    A = CalcilationModul32(A + B);

                    ReplaceValue(ref A, ref B, ref C, ref D);
                   
                    if (j == 0 || j == 15 || j == 1)
                    {
                        PrintVariables();
                    }
                }

            }
            Console.WriteLine("Extra");

            long[] abcdLastValue = new long[4];
            abcdLastValue[0] = CalcilationModul32(A + abcdInitialValue[0]);
            abcdLastValue[1] = CalcilationModul32(B + abcdInitialValue[1]);
            abcdLastValue[2] = CalcilationModul32(C + abcdInitialValue[2]);
            abcdLastValue[3] = CalcilationModul32(D + abcdInitialValue[3]);

            string result = ConvetrToResaltValue(abcdLastValue);
            
            Console.WriteLine(result);
        }

        private static string GetHexMessage()
        {
            return Console.ReadLine();
        }

        private static string[] GetBlocks(string messageHex)
        {
            string[] blocks32BitBin = new string[16];

            for (int i = 1; i <= 16; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    string hexSimbol = messageHex[8 * i - j].ToString();

                    string binView = Convert.ToString(int.Parse(hexSimbol, System.Globalization.NumberStyles.HexNumber), 2);

                    while (binView.Length < 4)
                    {
                        binView = "0" + binView;
                    }

                    blocks32BitBin[i - 1] += binView[3];
                    blocks32BitBin[i - 1] += binView[2];
                    blocks32BitBin[i - 1] += binView[1];
                    blocks32BitBin[i - 1] += binView[0];
                }
            }

            return blocks32BitBin;
        }

        private static void InitializeVariable()
        {
            A = 0x67452301;
            B = 0xEFCDAB89;
            C = 0x98badcfe;
            D = 0x10325476;

            shiftAmount[0] = new int[] { 7, 12, 17, 22 };
            shiftAmount[1] = new int[] { 5, 9, 14, 20 };
            shiftAmount[2] = new int[] { 4, 11, 16, 23 };
            shiftAmount[3] = new int[] { 6, 10, 15, 21 };


            blockNumber[0] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            blockNumber[1] = new int[] { 2, 7, 12, 1, 6, 11, 16, 5, 10, 15, 4, 9, 14, 3, 8, 13 };
            blockNumber[2] = new int[] { 6, 9, 12, 15, 2, 5, 8, 11, 14, 1, 4, 7, 10, 13, 16, 3 };
            blockNumber[3] = new int[] { 1, 8, 15, 6, 13, 4, 11, 2, 9, 16, 7, 14, 5, 12, 3, 10 };
        }

        private static void PrintVariables()
        {
            Console.WriteLine("A:" + A.ToString("X"));
            Console.WriteLine("B:" + B.ToString("X"));
            Console.WriteLine("C:" + C.ToString("X"));
            Console.WriteLine("D:" + D.ToString("X"));
            Console.WriteLine();
        }

        private static long CalcilationModul32(long x)
        {
            return x % (long)Math.Pow(2, 32);
        }

        private static long GetFunctionValue(int i, long B, long C, long D)
        {
            switch (i)
            {
                case 0:
                    return (B & C) | (~B & D);
                case 1:
                    return (B & D) | (~D & C);
                case 2:
                    return B ^ C ^ D;
                case 3:
                    return C ^ (~D | B);
                default: return 0;
            }
        }

        private static void ReplaceValue(ref long a, ref long b, ref long c, ref long d)
        {
            long buff = a;
            a = d;
            d = c;
            c = b;
            b = buff;
        }

        private static string ConvetrToResaltValue(long[] abcdLastValue)
        {
            string result = "";

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(abcdLastValue[i].ToString("X"));
                string currentLetterValue = Convert.ToString(abcdLastValue[i], 2);
                string hexView = "";

                while (currentLetterValue.Length < 32)
                {
                    currentLetterValue = "0" + currentLetterValue;
                }

                for (int j = 8; j > 0; j--)
                {
                    string binTetrad = currentLetterValue[j * 4 - 1].ToString() + currentLetterValue[j * 4 - 2] + currentLetterValue[j * 4 - 3] + currentLetterValue[j * 4 - 4];
                    hexView += Convert.ToInt32(binTetrad, 2).ToString("X");
                }

                result += hexView;
                Console.WriteLine(hexView);
                Console.WriteLine();
            }
            return result;
        }
    }
}
