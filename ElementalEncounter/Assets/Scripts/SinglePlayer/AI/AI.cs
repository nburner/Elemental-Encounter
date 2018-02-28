using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AI
{
	using bitboard = UInt64;
	using move = Tuple<Square, Square>;
	enum AIType
	{
		B_OFFENSE, B_DEFENSE, B_RANDOM, RANDOM_PET, DARYLS_PET, MEM_PET, TEST,
		DARYLS_PRUNE,
        SEEKER
    };

	class AI : MonoBehaviour
	{
        #region DLL Imports
        [DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void BasicRandom(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void BasicDefense(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void BasicOffense(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void DarylsPet(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void MemPet(bitboard white, bitboard black, Turn t, out int from, out int to, int id = 0);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void RandomPet(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void Test(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void DarylsPrune(bitboard white, bitboard black, Turn t, out int from, out int to);
		[DllImport("Assets/Plugins/AILibrary.dll")]
		public static extern void Seeker(bitboard white, bitboard black, Turn t, out int from, out int to);
        #endregion

        void Update()
        {
            //if (myJob != null)
            //{
            //    if (myJob.Update())
            //    {
            //        // Alternative to the OnFinished callback
            //        myJob = null;
            //    }
            //}
        }

        public Turn Color { get; private set; }
        public AIType Type { get; private set; }

        public AI(AIType t, Turn color) { Type = t; Color = color; }

		public void GetMove(Piece[,] gb, Action<int, int, int, int> finishAIMove) {
			//Convert Gameboard
			bitboard white = 0;
			bitboard black = 0;
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    if (gb[x, y] != null)
                    {
                        if (gb[x, y].isIce) { white = Board.set(white, y * 8 + x); }
                        if (!gb[x, y].isIce) { black = Board.set(black, y * 8 + x); }
                    }

            //Get move from type
			int from = 0, to = 0;
			switch (Type) {
				case AIType.B_OFFENSE: BasicOffense(white, black, Color, out from, out to); break;
				case AIType.B_DEFENSE: BasicDefense(white, black, Color, out from, out to); break;
				case AIType.B_RANDOM: BasicRandom(white, black, Color, out from, out to); break;
				case AIType.RANDOM_PET: RandomPet(white, black, Color, out from, out to); break;
				case AIType.DARYLS_PET: DarylsPet(white, black, Color, out from, out to); break;
				case AIType.MEM_PET: MemPet(white, black, Color, out from, out to); break;
				case AIType.TEST: Test(white, black, Color, out from, out to); break;
				case AIType.DARYLS_PRUNE: DarylsPrune(white, black, Color, out from, out to); break;
				case AIType.SEEKER: Seeker(white, black, Color, out from, out to); break;
				default: break;
			}

            //Do callback function
            int toX = to % 8;
            int toY = to / 8;
            int fromX = from % 8;
            int fromY = from / 8;

            finishAIMove(toX, toY, fromX, fromY);
        }
		public override string ToString() {
			return GetType() + ": " + Type.ToString();
		}        
    }
}
