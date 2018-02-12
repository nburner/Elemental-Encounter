﻿using System;
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
			AI noob = new AI(AIType.DARYLS_PRUNE);
			AI ai2 = new AI(AIType.DARYLS_PET);

			int petWins = 0;
			int testWins = 0;
				Game game = new Game();

			//for (int i = 0; i < 100; i++) {
				game.newGame(ai2, noob, true);
			
			Console.WriteLine("Hooray for "+(game.Winner == Turn.WHITE? "Pet" : "Noob"));
				//if (game.Winner == Turn.WHITE) ++petWins; else ++testWins;
				//Console.Write("\rPet: {0}\tTest: {1}", petWins, testWins);

				game.newGame(noob, ai2, true);
			Console.WriteLine("Hooray for "+(game.Winner == Turn.BLACK ? "Pet" : "Noob"));
				//if (game.Winner == Turn.BLACK) ++petWins; else ++testWins;
				//Console.Write("\rPet: {0}\tTest: {1}", petWins, testWins);

			//}
		}
	}
}
