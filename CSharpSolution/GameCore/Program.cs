using System;
using GameCore.Core;
using GameCore.AIWrapper;

namespace GameCore
{
	class Program
	{
		static void Main(string[] args)
		{
			Daryl daryl = new Daryl();
			Daryl daryl2 = new Daryl();
			Human human = new Human();
			AI ai = new AI(AIType.B_RANDOM);
			AI ai2 = new AI(AIType.DARYLS_PET);

			Game game = new Game();
			
			game.newGame(ai2, ai, true);

			Console.WriteLine("Winner: {0}", game.Winner.ToString());
		}
	}
}
