#include "Board.h"
#include <intrin.h>
#include <algorithm>

using namespace AI;
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

	lastMove = b.lastMove;
}

AI::Board::Board(const GameBoard & gb)
{
	bb[WHITE] = 0;
	bb[BLACK] = 0;
	for (int i = 0; i < 64; i++) {
		if (gb[Square(i)] == 'W') set(bb[WHITE], i);
		if (gb[Square(i)] == 'B') set(bb[BLACK], i);
	}
	turn = Turn(!gb.justTaken);
}
#pragma endregion

#pragma region Operator Overloads
std::ostream & AI::operator<<(std::ostream& out, const Board& board)
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

bool AI::operator<(Board const & a, Board const & b)
{
	if (a.bb[WHITE] < b.bb[WHITE]) return true;
	else if (a.bb[WHITE] == b.bb[WHITE] && a.bb[BLACK] < b.bb[BLACK]) return true;
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
		newBoard.lastMove = move(Square(to - forward), Square(to));
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
		newBoard.lastMove = move(Square(to - right), Square(to));
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
		newBoard.lastMove = move(Square(to - left), Square(to));
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
		newBoard.lastMove = move(Square(to - right), Square(to));
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
		newBoard.lastMove = move(Square(to - left), Square(to));
		result.push_back(newBoard);

		reset(possibleLefts, to);
	}

	return result;
}

//returns INT_MAX if the current player won, INT_MIN if the current player lost, 0 otherwise
int Board::gameOver() const
{
	if (bb[WHITE] & row8 || !bb[BLACK]) {
		if (!turn) return INT_MIN;
		if (turn) return INT_MAX;
	}
	if (bb[BLACK] & row1 || !bb[WHITE]) {
		if (!turn) return INT_MAX;
		if (turn) return INT_MIN;
	}
	return 0;
}
