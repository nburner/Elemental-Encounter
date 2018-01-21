#include "Board.h"
#include <intrin.h>
#include <algorithm>

using namespace CommonBitboards;
using namespace BoardHelpers;

#pragma region Constructors
Board::Board() {
	bb[WHITE] = bitboard(0x000000000000FFFF);
	bb[BLACK] = bitboard(0xFFFF000000000000);
	turn = WHITE;
}

Board::Board(const Board& b, bool flipTurn)
{
	bb[WHITE] = b.bb[WHITE];
	bb[BLACK] = b.bb[BLACK];
	turn = Turn((flipTurn + b.turn) % 2);
}
#pragma endregion

#pragma region Operator Overloads
std::ostream & operator<<(std::ostream& out, const Board& board)
{
	std::bitset<64> white(board.bb[WHITE]);
	std::bitset<64> black(board.bb[BLACK]);
	for (int i = 7; i >= 0; i--) {
		for (int j = 0; j < 8; j++) {
			if (white[i * 8 + j]) out << 'w';
			if (black[i * 8 + j]) out << 'b';
			if (!(white[i * 8 + j] | black[i * 8 + j])) out << '-';
			out << ' ';
		}
		out << endl;
	}

	return out;
}

bool operator<(Board const & a, Board const & b)
{
	if (std::min(a.bb[WHITE], a.bb[BLACK]) < std::min(b.bb[WHITE], b.bb[BLACK])) return true;
	else if (std::min(a.bb[WHITE], a.bb[BLACK]) == std::min(b.bb[WHITE], b.bb[BLACK]) && std::max(a.bb[WHITE], a.bb[BLACK]) < std::max(b.bb[WHITE], b.bb[BLACK])) return true;
	else return false;
}
#pragma endregion

std::vector<Board> Board::validNextBoards() const
{
	if ((row8 & bb[WHITE]) || !bb[BLACK] || (row1 & bb[BLACK]) || !bb[WHITE]) return std::vector<Board>();

	std::vector<Board> result = std::vector<Board>();

	Direction forward = turn == WHITE ? NORTH : SOUTH;
	Direction left = Direction(forward + WEST);
	Direction right = Direction(forward + EAST);

	unsigned long to;

	//Move Up
	bitboard possibleUps = shift(bb[turn], forward) & ~bb[WHITE] & ~bb[BLACK];
	while (_BitScanForward64(&to, possibleUps)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[turn], to);
		reset(newBoard.bb[turn], to - forward);
		newBoard.lastMove = move(Square(to), Square(to - forward));
		result.push_back(newBoard);

		reset(possibleUps, to);
	}

	//Move Right
	bitboard possibleRights = shift(bb[turn], right) & ~bb[turn] & ~colA;
	while (_BitScanForward64(&to, possibleRights)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[turn], to);
		reset(newBoard.bb[newBoard.turn], to);
		reset(newBoard.bb[turn], to - right);
		newBoard.lastMove = move(Square(to), Square(to - right));
		result.push_back(newBoard);

		reset(possibleRights, to);
	}

	//Move Left
	bitboard possibleLefts = shift(bb[turn], left) & ~bb[turn] & ~colH;
	while (_BitScanForward64(&to, possibleLefts)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[turn], to);
		reset(newBoard.bb[newBoard.turn], to);
		reset(newBoard.bb[turn], to - left);
		newBoard.lastMove = move(Square(to), Square(to - left));
		result.push_back(newBoard);

		reset(possibleLefts, to);
	}

	return result;
}

std::vector<Board> Board::validAttackBoards() const
{
	if ((row8 & bb[WHITE]) || !bb[BLACK] || (row1 & bb[BLACK]) || !bb[WHITE]) return std::vector<Board>();

	std::vector<Board> result = std::vector<Board>();

	Direction forward = turn == WHITE ? NORTH : SOUTH;
	Direction left = Direction(forward + WEST);
	Direction right = Direction(forward + EAST);

	unsigned long to;

	//Move Right
	bitboard possibleRights = shift(bb[turn], right) & ~bb[turn] & bb[!turn] & ~colA;
	while (_BitScanForward64(&to, possibleRights)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[turn], to);
		reset(newBoard.bb[newBoard.turn], to);
		reset(newBoard.bb[turn], to - right);
		newBoard.lastMove = move(Square(to), Square(to - right));
		result.push_back(newBoard);

		reset(possibleRights, to);
	}

	//Move Left
	bitboard possibleLefts = shift(bb[turn], left) & ~bb[turn] & bb[!turn] & ~colH;
	while (_BitScanForward64(&to, possibleLefts)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[turn], to);
		reset(newBoard.bb[newBoard.turn], to);
		reset(newBoard.bb[turn], to - left);
		newBoard.lastMove = move(Square(to), Square(to - left));
		result.push_back(newBoard);

		reset(possibleLefts, to);
	}

	return result;
}