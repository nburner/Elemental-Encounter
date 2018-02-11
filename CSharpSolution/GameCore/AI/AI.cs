using System;
using System.Runtime.InteropServices;
using GameCore.Core;

namespace GameCore.AIWrapper
{
	using bitboard = UInt64;
	using move = Tuple<Square, Square>;
	enum AIType
	{
		B_OFFENSE, B_DEFENSE, B_RANDOM, RANDOM_PET, DARYLS_PET, MEM_PET
	};

	class AI : Player
	{
		[DllImport("AILibrary.dll")]
		public static extern void BasicRandom(bitboard white, bitboard black, Turn t, ref int from, ref int to);
		[DllImport("AILibrary.dll")]
		public static extern void BasicDefense(bitboard white, bitboard black, Turn t, ref int from, ref int to);
		[DllImport("AILibrary.dll")]
		public static extern void BasicOffense(bitboard white, bitboard black, Turn t, ref int from, ref int to);
		[DllImport("AILibrary.dll")]
		public static extern void DarylsPet(bitboard white, bitboard black, Turn t, ref int from, ref int to);
		[DllImport("AILibrary.dll")]
		public static extern void MemPet(bitboard white, bitboard black, Turn t, ref int from, ref int to, int id = 0);
		[DllImport("AILibrary.dll")]
		public static extern void RandomPet(bitboard white, bitboard black, Turn t, ref int from, ref int to);
		public AIType Type { get; private set; }

		public AI(AIType t) { Type = t; }

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

			if (Type == AIType.B_RANDOM) BasicRandom(white, black, t, ref from, ref to);
			if (Type == AIType.B_OFFENSE) BasicOffense(white, black, t, ref from, ref to);
			if (Type == AIType.B_DEFENSE) BasicDefense(white, black, t, ref from, ref to);
			if (Type == AIType.DARYLS_PET) DarylsPet(white, black, t, ref from, ref to);
			if (Type == AIType.MEM_PET) MemPet(white, black, t, ref from, ref to);
			if (Type == AIType.RANDOM_PET) RandomPet(white, black, t, ref from, ref to);

			return new move((Square)from, (Square)to);
		}
	}
}
