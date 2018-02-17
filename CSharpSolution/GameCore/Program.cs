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
			AI test = new AI(AIType.DARYLS_PRUNE);
			//AI test = new AI(AIType.TEST);
			AI ai = new AI(AIType.DARYLS_PET);

			int petWins = 0;
			int testWins = 0;
				Game game = new Game();

			//for (int i = 0; i < 100; i++) {
				game.newGame(ai, test, false);
			
			Console.WriteLine("Hooray for "+(game.Winner == Turn.WHITE? "Pet (White)" : "Test (Black)"));
			if (game.Winner == Turn.WHITE) ++petWins; else ++testWins;
				//Console.Write("\rPet: {0}\tTest: {1}", petWins, testWins);

				game.newGame(test, ai, false);
			Console.WriteLine("Hooray for "+(game.Winner == Turn.BLACK ? "Pet (Black)" : "Test (White)"));
			if (game.Winner == Turn.BLACK) ++petWins; else ++testWins;
				//Console.Write("\rPet: {0}\tTest: {1}", petWins, testWins);

			//}
		}
	}
}
