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
            Console.Write(c);
            Thread.Sleep(150);
        }
	}
}
