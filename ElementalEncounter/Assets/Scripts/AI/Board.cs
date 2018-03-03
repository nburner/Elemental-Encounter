using System;

namespace AI
{
	using bitboard = UInt64;
	//using move = Tuple<Square, Square>;

	class Board
	{
		#region Predefined Bitboards
		private static readonly bitboard[] onePiece = {
			0x1,
			0x2,
			0x4,
			0x8,
			0x10,
			0x20,
			0x40,
			0x80,
			0x100,
			0x200,
			0x400,
			0x800,
			0x1000,
			0x2000,
			0x4000,
			0x8000,
			0x10000,
			0x20000,
			0x40000,
			0x80000,
			0x100000,
			0x200000,
			0x400000,
			0x800000,
			0x1000000,
			0x2000000,
			0x4000000,
			0x8000000,
			0x10000000,
			0x20000000,
			0x40000000,
			0x80000000,
			0x100000000,
			0x200000000,
			0x400000000,
			0x800000000,
			0x1000000000,
			0x2000000000,
			0x4000000000,
			0x8000000000,
			0x10000000000,
			0x20000000000,
			0x40000000000,
			0x80000000000,
			0x100000000000,
			0x200000000000,
			0x400000000000,
			0x800000000000,
			0x1000000000000,
			0x2000000000000,
			0x4000000000000,
			0x8000000000000,
			0x10000000000000,
			0x20000000000000,
			0x40000000000000,
			0x80000000000000,
			0x100000000000000,
			0x200000000000000,
			0x400000000000000,
			0x800000000000000,
			0x1000000000000000,
			0x2000000000000000,
			0x4000000000000000,
			0x8000000000000000
		};

		//These have all the bits of a column set
		private const bitboard colA = 0x0101010101010101;
		private const bitboard colB = 0x0202020202020202;
		private const bitboard colC = 0x0404040404040404;
		private const bitboard colD = 0x0808080808080808;
		private const bitboard colE = 0x1010101010101010;
		private const bitboard colF = 0x2020202020202020;
		private const bitboard colG = 0x4040404040404040;
		private const bitboard colH = 0x8080808080808080;

		//These have all the bits of a row set
		private const bitboard row1 = 0x00000000000000ff;
		private const bitboard row2 = 0x000000000000ff00;
		private const bitboard row3 = 0x0000000000ff0000;
		private const bitboard row4 = 0x00000000ff000000;
		private const bitboard row5 = 0x000000ff00000000;
		private const bitboard row6 = 0x0000ff0000000000;
		private const bitboard row7 = 0x00ff000000000000;
		private const bitboard row8 = 0xff00000000000000;
		#endregion

		#region Static Helper Functions
		private static readonly string[] _SquaresStr = { "A1", "B1", "C1", "D1", "E1", "F1", "G1", "H1", "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2", "A3", "B3", "C3", "D3", "E3", "F3", "G3", "H3", "A4", "B4", "C4", "D4", "E4", "F4", "G4", "H4", "A5", "B5", "C5", "D5", "E5", "F5", "G5", "H5", "A6", "B6", "C6", "D6", "E6", "F6", "G6", "H6", "A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7", "A8", "B8", "C8", "D8", "E8", "F8", "G8", "H8" };
		public static string to_string(Square s) {
			return _SquaresStr[(int)s];
		}
		public static string to_string(int s) {
			return _SquaresStr[s];
		}

		//public static string to_string(move m, bool addX = false) {
		//	return _SquaresStr[(int)m.Item1] + (addX ? " X " : " ") + _SquaresStr[(int)m.Item2];
		//}

		//This takes a bitboard and shifts all the bits in a direction on the board, returning the new bitboard
		public static bitboard shift(bitboard b, Direction d) {
			return d > 0 ? b << (int)d : b >> (-(int)d);
		}

		//This takes a bitboard and a forward direction and returns a bitboard of the squares threatened by those pieces
		public static bitboard threatens(bitboard b, Direction forward) {
			return (shift(b, (Direction)((int)forward + (int)Direction.WEST)) & ~colH) | (shift(b, (Direction)((int)forward + (int)Direction.EAST)) & ~colA);
		}

		//This sets one bit (to 1) of a bitboard and returns said bitboard
		public static bitboard set(bitboard b, int i) {
			b |= onePiece[i];
			return b;
		}

		//This resets one bit (to 0) of a bitboard and returns said bitboard
		public static bitboard reset(bitboard b, int i) {
			b &= ~onePiece[i];
			return b;
		}

		//This function takes a turn and returns the other turn
		public static Turn flip(Turn t) {
			return (Turn)(((int)t + 1) % 2);

		}
		#endregion
	}
}
