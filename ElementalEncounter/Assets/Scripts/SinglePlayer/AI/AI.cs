using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AI
{
	using bitboard = UInt64;

    public enum AIType
	{
		B_OFFENSE, B_DEFENSE, B_RANDOM, RANDOM_PET, DARYLS_PET, MEM_PET, TEST,
		DARYLS_PRUNE,
        SEEKER
    };

	class AI : MonoBehaviour
	{        
        void Update()
        {
            if (dllCaller != null)
            {
                if (dllCaller.Update())
                {
                    GameCore.UpdateBoard(dllCaller.fromX, dllCaller.fromY, dllCaller.toX, dllCaller.toY);
                    dllCaller = null;
                }
            }
        }

        public Turn Color { get; private set; }
        public AIType Type { get; private set; }
        private GameCore GameCore { get; set; }
        private AIJob dllCaller;

        public void Initialize(AIType t, Turn color, GameCore g) { Type = t; Color = color; GameCore = g; }

		public void GetMove() {
			bitboard white, black;
            GameCore.ConvertToBitboards(out white, out black);

            dllCaller = new AIJob
            {
                black = black,
                white = white,
                Color = Color,
                Type = Type
            };
            dllCaller.Start();
        }
		public override string ToString() {
			return GetType() + ": " + Type.ToString();
		}        
    }
}
