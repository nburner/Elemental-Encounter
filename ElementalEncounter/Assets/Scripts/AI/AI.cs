﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AI
{
	using bitboard = UInt64;

    public enum AIType
	{
		B_OFFENSE, B_DEFENSE, B_RANDOM, RANDOM_PET, DARYLS_PET, MEM_PET, TEST, DARYLS_PRUNE, SEEKER, HINTER,
		L337,
		MonteCarlo,
		MonteSeeker,
        HyperSeeker
    };

	class AI : MonoBehaviour
	{        
        void Update()
        {
            if (dllCaller != null)
            {
                if (FrameCounter < UInt32.MaxValue) FrameCounter++;
                if (dllCaller.IsDone && FrameCounter > 120)
                {
                    Callback(dllCaller.Move);
                    dllCaller = null;
                    FrameCounter = 0;
                }
            }
        }

        private uint FrameCounter = 0;
        public Turn Color { get; private set; }
        public AIType Type { get; private set; }
        private Action<Move> Callback { get; set; }
        private AIJob dllCaller;

        public AI Initialize(AIType t, Turn color, Action<Move> callback) { Type = t; Color = color; Callback = callback; return this; }

		public void GetMove(Board<char> pieces) {
			bitboard white, black;
            ConvertToBitboards(pieces, out white, out black);

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

        private void ConvertToBitboards(Board<char> Pieces, out bitboard white, out bitboard black)
        {
            white = 0; black = 0;
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    if (Pieces[x, y] == 'W') { white = Board.set(white, y * 8 + x); }
                    if (Pieces[x, y] == 'B') { black = Board.set(black, y * 8 + x); }
                }
        }
    }
}
