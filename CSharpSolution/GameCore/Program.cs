using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Core;

namespace GameCore
{
	class Program
	{
		static void Main(string[] args)
		{
			Daryl daryl = new Daryl();
			Human human = new Human();

			Game game = new Game();
			game.newGame(daryl, human, true);

			Console.WriteLine("Winner: {0}", game.Winner.ToString());
		}
	}
}
