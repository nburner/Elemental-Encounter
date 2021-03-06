﻿using System;
using System.Runtime.InteropServices;
using GameCore.Core;

namespace GameCore.AIWrapper
{
	using bitboard = UInt64;
	using move = Tuple<Square, Square>;
	enum AIType
	{
		B_OFFENSE, B_DEFENSE, B_RANDOM, RANDOM_PET, DARYLS_PET, MEM_PET, TEST,
		DARYLS_PRUNE,
        SEEKER,
		L337,
		MonteCarlo,
		MonteSeeker
	};

	class AI : Player
	{
		[DllImport("AILibrary.dll")]
		public static extern void BasicRandom(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void BasicDefense(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void BasicOffense(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void DarylsPet(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void MemPet(bitboard white, bitboard black, Turn t, out int from, out int to, int id = 0);
		[DllImport("AILibrary.dll")]
		public static extern void RandomPet(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void Test(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void DarylsPrune(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void Seeker(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void L337(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void MonteCarlo(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("AILibrary.dll")]
		public static extern void MonteSeeker(bitboard white, bitboard black, Turn t, out int from, out int to);


		public AIType Type { get; private set; }
		private bool Verbose { get; set; }

        public AI(AIType t, bool verbose = false) { Type = t; Verbose = verbose; }

		public override move getMove(GameBoard gb) {
			//Convert Gameboard
			bitboard white = 0;
			bitboard black = 0;
			for (int i = 0; i < 64; i++) {
				if (gb[(Square)i] == 'W') { white = Board.set(white, i); }
				if (gb[(Square)i] == 'B') { black = Board.set(black, i); }
			}

			Turn t = myColor;

			//Get move from type
			int from = 0, to = 0;
			switch (Type) {
				case AIType.B_OFFENSE: BasicOffense(white, black, t, out from, out to); break;
				case AIType.B_DEFENSE: BasicDefense(white, black, t, out from, out to); break;
				case AIType.B_RANDOM: BasicRandom(white, black, t, out from, out to); break;
				case AIType.RANDOM_PET: RandomPet(white, black, t, out from, out to); break;
				case AIType.DARYLS_PET: DarylsPet(white, black, t, out from, out to); break;
				case AIType.MEM_PET: MemPet(white, black, t, out from, out to); break;
				case AIType.TEST: Test(white, black, t, out from, out to); break;
				case AIType.DARYLS_PRUNE: DarylsPrune(white, black, t, out from, out to); break;
				case AIType.SEEKER: Seeker(white, black, t, out from, out to); break;
				case AIType.L337: L337(white, black, t, out from, out to); break;
				case AIType.MonteCarlo: MonteCarlo(white, black, t, out from, out to); break;
				case AIType.MonteSeeker: MonteSeeker(white, black, t, out from, out to); break;
				default: break;
			}
            move result = new move((Square)from, (Square)to);

            if(Verbose) Console.WriteLine("AI Move: " + Board.to_string(result));
			return result;
		}

		public override string ToString() {
			return GetType() + ": " + Type.ToString();
		}
	}
}
