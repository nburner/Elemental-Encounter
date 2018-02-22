using System;
using System.Threading;

namespace GameCore.Misc
{
    class Fun
    {
        public static void TypeString(string str)
        {
            for(int i = 0; i < str.Length; i++)
            {
                TypeChar(str[i]);
            }
        }
        private static void TypeChar(char c)
        {
            int sleep = 50;
            if (Char.IsPunctuation(c)) sleep += 150;
            if (c == '.' || c == '\n') sleep += 100;
            Console.Write(c);
            Thread.Sleep(sleep);
        }
	}
}
