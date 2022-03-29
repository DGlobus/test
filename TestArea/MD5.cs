using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TestArea
{
    internal class MD5
    {
        static void Main(string[] args)
        {
            
            //string message = Console.ReadLine();
            string messageHex = Console.ReadLine();

            //string[] arr = new string[16];
            string[] blocks32BitBin = new string[16];
            
            for(int i = 1; i<=16; i++)
            {
                for(int j = 1; j<=8; j++)
                {
                    //string a = message[8 * i - j].ToString();
                    string hexSimbol = messageHex[8 * i - j].ToString();
                    
                    //a = Convert.ToString(int.Parse(a, System.Globalization.NumberStyles.HexNumber), 2);
                    string binView = Convert.ToString(int.Parse(hexSimbol, System.Globalization.NumberStyles.HexNumber), 2);
                   
                    //while (a.Length < 4)
                    while (binView.Length < 4)
                    {
                        //a = "0" + a;
                        binView = "0" + binView;
                    }
                   
                    //arr[i - 1] += a[3];
                    blocks32BitBin[i - 1] += binView[3];
                    //arr[i - 1] += a[2];
                    blocks32BitBin[i - 1] += binView[2];
                    //arr[i - 1] += a[1];
                    blocks32BitBin[i - 1] += binView[1];
                    //arr[i - 1] += a[0];
                    blocks32BitBin[i - 1] += binView[0];
                }
            }

            long A = 0x67452301;
            long B = 0xEFCDAB89;
            long C = 0x98badcfe;
            long D = 0x10325476;

            //long[] abcd = { A, B, C, D };
            long[] abcdInitialValue = { A, B, C, D };

            //int[][] rounds = new int[4][];
            int[][] shiftAmount = new int[4][];
            shiftAmount[0] = new int[]{ 7, 12, 17, 22};
            shiftAmount[1] = new int[]{ 5, 9, 14, 20};
            shiftAmount[2] = new int[]{ 4, 11, 16, 23};
            shiftAmount[3] = new int[]{ 6, 10, 15, 21};

           // int[][] blocks = new int[4][];
            int[][] blockNumber = new int[4][];
            blockNumber[0] = new int[] {1, 2, 3, 4, 5, 6, 7, 8,9, 10,11,12,13,14,15,16 };
            blockNumber[1] = new int[] { 2,7,12,1,6,11,16,5,10,15,4,9,14,3,8,13 };
            blockNumber[2] = new int[] { 6,9,12,15,2,5,8,11,14,1,4,7,10,13,16,3 };
            blockNumber[3] = new int[] { 1,8,15,6,13,4,11,2,9,16,7,14,5,12,3,10 };

            
            for (int i = 0; i<4; i++)
            {
                Console.WriteLine(i);
                
                for (int j = 0; j<16; j++)
                {
                    if (j == 0 || j == 15 || j == 1)
                    {
                        Console.WriteLine("A:" + A.ToString("X"));
                        Console.WriteLine("B:" + B.ToString("X"));
                        Console.WriteLine("C:" + C.ToString("X"));
                        Console.WriteLine("D:" + D.ToString("X"));
                        Console.WriteLine();
                    }

                    //long func = 0;
                    long functionValue = GetFunctionValue(i, B, C, D);
                    //long k = (long)(Math.Pow(2, 32) * Math.Abs(Math.Sin(j +1 + 16 * i)));
                    long constanta = (long)(Math.Pow(2, 32) * Math.Abs(Math.Sin(j +1 + 16 * i)));
                    //long t = Convert.ToInt64(arr[blocks[i][j]-1], 2);
                    long currentBlock = Convert.ToInt64(blocks32BitBin[blockNumber[i][j]-1], 2);

                    //A = (A + functionValue) % (long)Math.Pow(2, 32);
                    //A = (A + currentBlock) % (long)Math.Pow(2, 32);
                    //A = (A + constanta) % (long)Math.Pow(2, 32);
                    //A = (A << shiftAmount[i][j%4]) % (long)Math.Pow(2, 32);
                    //A = (A + B) % (long)Math.Pow(2, 32);

                    A = CalcilationModul32(A + functionValue);
                    A = CalcilationModul32(A + currentBlock);
                    A = CalcilationModul32(A + constanta);
                    A = CalcilationModul32(A << shiftAmount[i][j % 4]);
                    A = CalcilationModul32(A + B);

                    //Change(ref A, ref B, ref C, ref D);
                    ReplaceValue(ref A, ref B, ref C, ref D);
                    if(j ==0 || j == 15 || j==1)
                    {
                        Console.WriteLine("A:"+A.ToString("X"));
                        Console.WriteLine("B:"+B.ToString("X"));
                        Console.WriteLine("C:"+C.ToString("X"));
                        Console.WriteLine("D:"+D.ToString("X"));
                        Console.WriteLine();
                    }
                }
                
            }
            Console.WriteLine("Extra");

            long[] abcdLastValue = new long[4];
            //abcd[0] = (A + abcdInitialValue[0]) % (long)Math.Pow(2, 32);
            abcdLastValue[0] = CalcilationModul32(A + abcdInitialValue[0]);

            //abcd[1] = (B + abcdInitialValue[1]) % (long)Math.Pow(2, 32);
            abcdLastValue[1] = CalcilationModul32(B + abcdInitialValue[1]);

            //abcd[2] = (C + abcdInitialValue[2]) % (long)Math.Pow(2, 32);
            abcdLastValue[2] = CalcilationModul32(C + abcdInitialValue[2]);

            //abcd[3] = (D + abcdInitialValue[3]) % (long)Math.Pow(2, 32);
            abcdLastValue[3] = CalcilationModul32(D + abcdInitialValue[3]);

            string result = "";
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(abcdInitialValue[i].ToString("X"));
                //string a = Convert.ToString(abcdLastValue[i], 2);
                string currentLetterValue = Convert.ToString(abcdLastValue[i], 2);
                //string buff = "";
                string hexView = "";
                
                while (currentLetterValue.Length < 32)
                {
                    currentLetterValue = "0" + currentLetterValue;
                }
                
                for (int j = 8; j >0; j--)
                {
                    //string b = a[j * 4 - 1].ToString() + a[j * 4 - 2] + a[j * 4 - 3] + a[j * 4 - 4];
                    string binTetrad = currentLetterValue[j * 4 - 1].ToString() + currentLetterValue[j * 4 - 2] + currentLetterValue[j * 4 - 3] + currentLetterValue[j * 4 - 4];
                    hexView += Convert.ToInt32(binTetrad, 2).ToString("X");
                }
                
                result += hexView;
                Console.WriteLine(hexView);
                Console.WriteLine();
            }
            Console.WriteLine(result);
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
}
}
