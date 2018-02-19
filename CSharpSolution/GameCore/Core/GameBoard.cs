using System;

namespace GameCore.Core
{
	using move = Tuple<Square, Square>;
	class GameBoard
	{
		public Turn justTaken;
		public char this[Square i]
		{
			get { return space[(int)i]; }
			set { space[(int)i] = value; }
		}
		protected char[] space = new char[64];
		public void updateBoard(move move, Turn t)
		{
			this[move.Item1] = ((((int)move.Item1) / 8 + ((int)move.Item1)) % 2 == 0 ? (char)9608 : ' ');
			this[move.Item2] = (t == 0 ? 'W' : 'B');
			justTaken = t;
		}
		public GameBoard()
		{
			char bar = (char)9608;

			for (int i = 0; i < 64; i++)
			{
				if ((i / 8 + i) % 2 == 0) space[i] = bar;
				else space[i] = ' ';
			}

			for (Square i = Square.A1; i <= Square.H2; i++) this[i] = 'W';

			for (Square i = Square.A7; i <= Square.H8; i++) this[i] = 'B';
		}
        public GameBoard(Square[] white, Square[] black)
        {
            char bar = (char)9608;

            for (int i = 0; i < 64; i++)
            {
                if ((i / 8 + i) % 2 == 0) space[i] = bar;
                else space[i] = ' ';
            }

            for (int i = 0; i < white.Length; i++) this[white[i]] = 'W';

            for (int i = 0; i < black.Length; i++) this[black[i]] = 'B';
        }
        private GameBoard(GameBoard _) { }
	}
}
