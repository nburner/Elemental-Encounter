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
        private GameCore GameCore { get; set; }

        public AI(AIType t, Turn color, GameCore g) { Type = t; Color = color; GameCore = g; }

		public void GetMove() {
			//Convert Gameboard
			bitboard white, black;
            GameCore.ConvertToBitboards(out white, out black);

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

            GameCore.UpdateBoard(toX, toY, fromX, fromY);
        }
		public override string ToString() {
			return GetType() + ": " + Type.ToString();
		}        
    }
}
