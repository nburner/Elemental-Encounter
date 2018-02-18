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
			Daryl daryl = new Daryl();

			AI seeker = new AI(AIType.SEEKER);
			AI pet = new AI(AIType.DARYLS_PET);
			
			for (int i = 0; i < 250; i++) {
				AI rand = new AI(AIType.B_RANDOM);
				AI rand2 = new AI(AIType.RANDOM_PET);

				playAIMatch(seeker, rand);
				playAIMatch(seeker, rand2);
				playAIMatch(rand, seeker);
				playAIMatch(rand2, seeker);

				playAIMatch(pet, rand);
				playAIMatch(pet, rand2);
				playAIMatch(rand, pet);
				playAIMatch(rand2, pet);
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
