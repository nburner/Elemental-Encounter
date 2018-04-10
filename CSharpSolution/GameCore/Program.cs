using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Core;
using GameCore.AIWrapper;
using System.IO;

namespace GameCore
{
	class Program
	{
		private static Game game = new Game();
		private static Dictionary<AIType, int> wins = Enum.GetValues(typeof(AIType)).Cast<AIType>().ToDictionary<AIType, AIType, int>((a) => { return a; }, (a) => { return 0; });
		static void Main(string[] args) {
			functionForDaryl();
			//aiCompare(AIType.SEEKER, AIType.L337);
        }
		private static void functionForDaryl() {
			Console.WindowHeight = (int)(Console.LargestWindowHeight / 1.2);

			//This declares the players (one ai and one human who represents the opponents ai)
			AI ai = new AI(AIType.L337, true); //NOTE - Change AIType to DARYLS_PET if Seeker crashes or takes too long (hopefully shouldn't happen)
			Daryl opponent = new Daryl();

			//Fun instructions
			Misc.Fun.TypeString("Hello Master. Welcome back.\n");
			Misc.Fun.TypeString("Should I play as W or B?  ");

			//This gets the user input for which side the ai is playing
			char sideChar;
			do {
				sideChar = Char.ToUpper(Console.ReadKey().KeyChar);
			} while (sideChar != 'W' && sideChar != 'B');
			Console.WriteLine("\n");

			//This starts the game based on which color the user selected. Note the order of the players is swapped in the two function calls
			if (sideChar == 'W') game.newGame(ai, opponent, true);
			else game.newGame(opponent, ai, true);
		}
		private static void functionForNathan()
        {
            Console.WindowHeight = (int)(Console.LargestWindowHeight / 1.2);

            //This declares the players (one ai and one human who represents the opponents ai)
            AI ai = new AI(AIType.SEEKER, true); //NOTE - Change AIType to DARYLS_PET if Seeker crashes or takes too long (hopefully shouldn't happen)
            Nathan opponent = new Nathan(); 

            //Fun instructions
            Misc.Fun.TypeString("Hello Nathan. This process should be as painful, er, painless as possible.\n");
            Misc.Fun.TypeString("First, determine what side the AI will play as.\n");
            Misc.Fun.TypeString("W or B?  ");

            //This gets the user input for which side the ai is playing
            char sideChar;
            do {
                sideChar = Char.ToUpper(Console.ReadKey().KeyChar);
            } while (sideChar != 'W' && sideChar != 'B');
            Console.WriteLine("\n");

            //This starts the game based on which color the user selected. Note the order of the players is swapped in the two function calls
            if(sideChar == 'W') game.newGame(ai, opponent, true);
            else game.newGame(opponent, ai, true);
        }
        private static void playThatOneTestCase()
        {
            AI seeker = new AI(AIType.SEEKER);
            AI pet = new AI(AIType.DARYLS_PET);

            game.newGame(seeker,
                new[]{Square.A2,
                    Square.B1, Square.B2, Square.B3,
                    Square.C1, Square.C2, Square.C4,
                    Square.D2,
                    Square.E2,
                    Square.F1, Square.F2,
                    Square.G1, Square.G2, Square.G5,
                    Square.H2},
                pet,
                new[]{Square.A8,
                    Square.B8,
                    Square.C8,
                    Square.D8, Square.D4,
                    Square.E8, Square.E6, Square.E4,
                    Square.F8, Square.F7, Square.F4,
                    Square.G8, Square.G7,
                    Square.H8, Square.H7},
                Turn.WHITE, true);
        }
        private static void aiCompare(AIType ai1, AIType ai2, int count = 250)
        {
            AI _1 = new AI(ai1);
            AI _2 = new AI(ai2);

            for (int i = 0; i < 250; i++)
            {
                AI rand = new AI(AIType.B_RANDOM);
                AI rand2 = new AI(AIType.RANDOM_PET);

                playAIMatch(_1, rand);
                playAIMatch(_1, rand2);
                playAIMatch(rand, _1);
                playAIMatch(rand2, _1);

                playAIMatch(_2, rand);
                playAIMatch(_2, rand2);
                playAIMatch(rand, _2);
                playAIMatch(rand2, _2);
            }
        }
        private static void playAIMatch(AI ai1, AI ai2, bool verbose = true) {
			using (StreamWriter file = File.AppendText("log.txt")) {
				Console.SetOut(file);
				game.newGame(ai1, ai2, verbose);
			}
			wins[game.Winner == Turn.WHITE ? ai1.Type : ai2.Type]++;
			printWins();
		}

		private static void printWins(bool re = true) {
			resetConsole();
			if (re) Console.SetCursorPosition(0, Math.Max(Console.CursorTop - wins.Where((e) => { return e.Value != 0; }).Count(), 0));
			Console.Write(wins.Where((e) => { return e.Value != 0; }).Select((e) => { return string.Format("{0}: {1}\t     ", e.Key, e.Value); }).Aggregate((str1, str2) => { return string.Join("\n", str1, str2); }));
		}

		private static void resetConsole() {
			var standardOutput = new StreamWriter(Console.OpenStandardOutput());
			standardOutput.AutoFlush = true;
			Console.SetOut(standardOutput);
		}
	}
}
