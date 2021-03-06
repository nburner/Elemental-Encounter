#pragma once
//#include"..\..\GameCore\GameCore\Debug\GameBoard.h"
#include<bitset>
#include<iostream>
#include<set>
#include<vector>
#include<intrin.h>
using std::cout; using std::endl; using std::string;

typedef uint64_t bitboard;

namespace AI {

	#pragma region Types
	/*Arranged the squares to look like the board
	Below is a picture of the board for my own sake
	So A1 is 0, and H8 is 63, and it counts from left to right, bottom to top

	56 57 58 59 60 61 62 63
	48 49 50 51 52 53 54 55
	40 41 42 43 44 45 46 47
	32 33 34 35 36 37 38 39
	24 25 26 27 28 29 30 31
	16 17 18 19 20 21 22 23
	8  9  10 11 12 13 14 15
	0  1  2  3  4  5  6  7

	*/
	enum Square 
	{
		A8 = 56, B8, C8, D8, E8, F8, G8, H8,
		A7 = 48, B7, C7, D7, E7, F7, G7, H7,
		A6 = 40, B6, C6, D6, E6, F6, G6, H6,
		A5 = 32, B5, C5, D5, E5, F5, G5, H5,
		A4 = 24, B4, C4, D4, E4, F4, G4, H4,
		A3 = 16, B3, C3, D3, E3, F3, G3, H3,
		A2 = 8, B2, C2, D2, E2, F2, G2, H2,
		A1 = 0, B1, C1, D1, E1, F1, G1, H1
	};

	//a pair of squares is known as a move
	//the first square is the from, the second square is the to
	typedef std::pair<Square, Square> move;


	//Add a direction to a square and you get the square in that direction
	//So A1 + NORTH is A2, while B4 + SOUTHEAST is C3
	enum Direction : short
	{
		NORTH = 8,
		EAST = 1,
		SOUTH = -8,
		WEST = -1,

		NORTHEAST = 9,
		SOUTHEAST = -7,
		SOUTHWEST = -9,
		NORTHWEST = 7
	};

	//A simple enum for black and white
	//Useful because you can make an array and use white or black as the subscript
	enum Turn : bool { WHITE, BLACK };
#pragma endregion

	namespace CommonBitboards {
		//This gives all the moves for the white piece at the position indicated by the index of the array
		//const bitboard whiteMoves[64] = {
		//	0x300,
		//	0x700,
		//	0xE00,
		//	0x1C00,
		//	0x3800,
		//	0x7000,
		//	0xE000,
		//	0xC000,					//End of Row 1
		//	0x30000,
		//	0x70000,
		//	0xE0000,
		//	0x1C0000,
		//	0x380000,
		//	0x700000,
		//	0xE00000,
		//	0xC00000,				//End of Row 2
		//	0x3000000,
		//	0x7000000,
		//	0xE000000,
		//	0x1C000000,
		//	0x38000000,
		//	0x70000000,
		//	0xE0000000,
		//	0xC0000000,				//End of Row 3
		//	0x300000000,
		//	0x700000000,
		//	0xE00000000,
		//	0x1C00000000,
		//	0x3800000000,
		//	0x7000000000,
		//	0xE000000000,
		//	0xC000000000,			//End of Row 4
		//	0x30000000000,
		//	0x70000000000,
		//	0xE0000000000,
		//	0x1C0000000000,
		//	0x380000000000,
		//	0x700000000000,
		//	0xE00000000000,
		//	0xC00000000000,			//End of Row 5
		//	0x3000000000000,
		//	0x7000000000000,
		//	0xE000000000000,
		//	0x1C000000000000,
		//	0x38000000000000,
		//	0x70000000000000,
		//	0xE0000000000000,
		//	0xC0000000000000,		//End of Row 6
		//	0x300000000000000,
		//	0x700000000000000,
		//	0xE00000000000000,
		//	0x1C00000000000000,
		//	0x3800000000000000,
		//	0x7000000000000000,
		//	0xE000000000000000,
		//	0xC000000000000000,		//End of Row 7
		//	0x0000000000000000,
		//	0x0000000000000000,
		//	0x0000000000000000,
		//	0x0000000000000000,
		//	0x0000000000000000,
		//	0x0000000000000000,
		//	0x0000000000000000,
		//	0x0000000000000000		//Row 8 is Victory (no moves) so the bitboards are empty
		//};

		//This is an array of boards with only one bit set, corresponding with(to?) the index
		const bitboard onePiece[64] = {
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
		const bitboard colA = bitboard(0x0101010101010101);
		const bitboard colB = bitboard(0x0202020202020202);
		const bitboard colC = bitboard(0x0404040404040404);
		const bitboard colD = bitboard(0x0808080808080808);
		const bitboard colE = bitboard(0x1010101010101010);
		const bitboard colF = bitboard(0x2020202020202020);
		const bitboard colG = bitboard(0x4040404040404040);
		const bitboard colH = bitboard(0x8080808080808080);

		//These have all the bits of a row set
		const bitboard row1 = bitboard(0x00000000000000ff);
		const bitboard row2 = bitboard(0x000000000000ff00);
		const bitboard row3 = bitboard(0x0000000000ff0000);
		const bitboard row4 = bitboard(0x00000000ff000000);
		const bitboard row5 = bitboard(0x000000ff00000000);
		const bitboard row6 = bitboard(0x0000ff0000000000);
		const bitboard row7 = bitboard(0x00ff000000000000);
		const bitboard row8 = bitboard(0xff00000000000000);
	}

	namespace BoardHelpers {
		const string _SquaresStr[64]{ "A1", "B1", "C1", "D1", "E1", "F1", "G1", "H1", "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2", "A3", "B3", "C3", "D3", "E3", "F3", "G3", "H3", "A4", "B4", "C4", "D4", "E4", "F4", "G4", "H4", "A5", "B5", "C5", "D5", "E5", "F5", "G5", "H5", "A6", "B6", "C6", "D6", "E6", "F6", "G6", "H6", "A7", "B7", "C7", "D7", "E7", "F7", "G7", "H7", "A8", "B8", "C8", "D8", "E8", "F8", "G8", "H8" };
		
		inline string to_string(Square s) {
			_ASSERT(s < 64);
			return _SquaresStr[s];
		}

		inline string to_string(move m, bool addX = false) {
			return _SquaresStr[m.first] + (addX ? " X " : " ") + _SquaresStr[m.second];
		}

		//This takes a bitboard and shifts all the bits in a direction on the board, returning the new bitboard
		inline bitboard shift(const bitboard b, const Direction d) {
			return d > 0 ? b << d : b >> (-d);
		}

		//This takes a bitboard and a forward direction and returns a bitboard of the squares threatened by those pieces
		inline bitboard threatens(const bitboard b, Direction forward) {
			return (shift(b, Direction(forward+WEST)) & ~CommonBitboards::colH) | (shift(b, Direction(forward + EAST)) & ~CommonBitboards::colA);
		}

		//This sets one bit (to 1) of a bitboard and returns said bitboard
		inline void set(bitboard& b, int i) {
			_bittestandset64((long long*)(void*)&b, i);
		}

		//This resets one bit (to 0) of a bitboard and returns said bitboard
		inline void reset(bitboard& b, int i) {
			_bittestandreset64((long long*)(void*)&b, i);
		}

		//This counts the number of bits set on a bitboard
		inline int count(const bitboard& b) {
			return __popcnt64(b);
		}

		//This function takes a turn and returns the other turn
		inline const Turn flip(Turn t) {
			return Turn(!t);
		}
	}

	class Board
	{
		bitboard bb[2];
		Turn _turn;

		inline void nextTurn() {
			_turn = Turn(!_turn);
		}

	public:
		friend std::ostream& operator<<(std::ostream&, const Board&);
		friend bool operator<(Board const& a, Board const& b);
		friend class FeatureFunctions;

		std::vector<Board> validNextBoards() const;
		std::vector<Board> validAttackBoards() const;
		std::vector<Board> validWinBoards() const;
		Board makeMove(move) const;
		Board onlyFront() const;
		Board onlyMiddle() const;

		Square furthestPiece(Turn) const;

		int gameOver() const;
		inline Turn turn() const { return _turn; }
		inline bool hasPawns(Turn t) const { return bb[t]; }
		
		mutable move lastMove;
		mutable int beta = INT16_MAX;
		mutable int alpha = INT16_MIN;
		mutable int val = 0;
		mutable int movesBeforeWin = 0;

		Board();
		Board(bitboard white, bitboard black, Turn t);
		Board(const Board&, bool = false);
	};

}