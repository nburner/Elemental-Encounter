using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GameCore.Core;

namespace GameCore
{
	class Program
	{
		[DllImport("AILibrary.dll")]
		public static extern void DisplayHelloFromDLL();
		static void Main(string[] args)
		{
			Daryl daryl = new Daryl();
			Human human = new Human();

			Game game = new Game();

			DisplayHelloFromDLL();

			game.newGame(daryl, human, true);

			Console.WriteLine("Winner: {0}", game.Winner.ToString());
		}
	}
}
