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
	_turn = WHITE;
}

Board::Board(bitboard white, bitboard black, Turn t) {
	bb[WHITE] = white;
	bb[BLACK] = black;
	_turn = t;
}

Board::Board(const Board& b, bool flipTurn)
{
	bb[WHITE] = b.bb[WHITE];
	bb[BLACK] = b.bb[BLACK];
	_turn = Turn((flipTurn + b._turn) % 2);

	lastMove = b.lastMove;
	val = b.val;
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

	Direction forward = _turn == WHITE ? NORTH : SOUTH;
	Direction left = Direction(forward + WEST);
	Direction right = Direction(forward + EAST);

	unsigned long to;

	//Move Up
	bitboard possibleUps = shift(bb[_turn], forward) & ~bb[WHITE] & ~bb[BLACK];
	while (_BitScanForward64(&to, possibleUps)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[_turn], to - forward);
		newBoard.lastMove = move(Square(to - forward), Square(to));
		result.push_back(newBoard);

		reset(possibleUps, to);
	}

	//Move Right
	bitboard possibleRights = shift(bb[_turn], right) & ~bb[_turn] & ~colA;
	while (_BitScanForward64(&to, possibleRights)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[newBoard._turn], to);
		reset(newBoard.bb[_turn], to - right);
		newBoard.lastMove = move(Square(to - right), Square(to));
		result.push_back(newBoard);

		reset(possibleRights, to);
	}

	//Move Left
	bitboard possibleLefts = shift(bb[_turn], left) & ~bb[_turn] & ~colH;
	while (_BitScanForward64(&to, possibleLefts)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[newBoard._turn], to);
		reset(newBoard.bb[_turn], to - left);
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

	Direction forward = _turn == WHITE ? NORTH : SOUTH;
	Direction left = Direction(forward + WEST);
	Direction right = Direction(forward + EAST);

	unsigned long to;

	//Move Right
	bitboard possibleRights = shift(bb[_turn], right) & ~bb[_turn] & bb[!_turn] & ~colA;
	while (_BitScanForward64(&to, possibleRights)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[newBoard._turn], to);
		reset(newBoard.bb[_turn], to - right);
		newBoard.lastMove = move(Square(to - right), Square(to));
		result.push_back(newBoard);

		reset(possibleRights, to);
	}

	//Move Left
	bitboard possibleLefts = shift(bb[_turn], left) & ~bb[_turn] & bb[!_turn] & ~colH;
	while (_BitScanForward64(&to, possibleLefts)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[newBoard._turn], to);
		reset(newBoard.bb[_turn], to - left);
		newBoard.lastMove = move(Square(to - left), Square(to));
		result.push_back(newBoard);

		reset(possibleLefts, to);
	}

	return result;
}

std::vector<Board> Board::validWinBoards() const
{
	if ((row8 & bb[WHITE]) || !bb[BLACK] || (row1 & bb[BLACK]) || !bb[WHITE]) return std::vector<Board>();

	std::vector<Board> result = std::vector<Board>();

	Direction forward = _turn == WHITE ? NORTH : SOUTH;
	Direction left = Direction(forward + WEST);
	Direction right = Direction(forward + EAST);
	auto myWinRow = _turn == WHITE ? row8 : row1;

	unsigned long to;

	//Move Up
	bitboard possibleUps = shift(bb[_turn], forward) & ~bb[WHITE] & ~bb[BLACK] & myWinRow;
	while (_BitScanForward64(&to, possibleUps)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[_turn], to - forward);
		newBoard.lastMove = move(Square(to - forward), Square(to));
		result.push_back(newBoard);

		reset(possibleUps, to);
	}

	//Move Right
	bitboard possibleRights = shift(bb[_turn], right) & ~bb[_turn] & ~colA & myWinRow;
	while (_BitScanForward64(&to, possibleRights)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[newBoard._turn], to);
		reset(newBoard.bb[_turn], to - right);
		newBoard.lastMove = move(Square(to - right), Square(to));
		result.push_back(newBoard);

		reset(possibleRights, to);
	}

	//Move Left
	bitboard possibleLefts = shift(bb[_turn], left) & ~bb[_turn] & ~colH & myWinRow;
	while (_BitScanForward64(&to, possibleLefts)) {
		Board newBoard = Board(*this, true);

		set(newBoard.bb[_turn], to);
		reset(newBoard.bb[newBoard._turn], to);
		reset(newBoard.bb[_turn], to - left);
		newBoard.lastMove = move(Square(to - left), Square(to));
		result.push_back(newBoard);

		reset(possibleLefts, to);
	}

	return result;
}

Board AI::Board::makeMove(move m) const
{
	Board newBoard = Board(*this, true);

	set(newBoard.bb[_turn], m.second);
	reset(newBoard.bb[_turn], m.first);
	if((m.second - m.first) % 8 != 0) reset(newBoard.bb[newBoard._turn], m.second);
	newBoard.lastMove = m;

	return newBoard;
}

Board AI::Board::onlyFront() const
{
	Board result = *this;

	result.bb[this->_turn] &= this->_turn ? (row2 | row3 | row4) : (row5 | row6 | row7);
	result.bb[!this->_turn] &= this->_turn ? (row1 | row2 | row3) : (row6 | row7 | row8);

	return result;
}

Board AI::Board::onlyMiddle() const
{
	Board result = *this;

	result.bb[this->_turn] &= this->_turn ? (row4 | row5 | row6) : (row3 | row4 | row5);
	result.bb[!this->_turn] &= this->_turn ? (row3 | row4 | row5) : (row4 | row5 | row6);

	return result;
}

Square AI::Board::furthestPiece(Turn t) const
{
	unsigned long result;

	if (!t) _BitScanReverse64(&result, bb[t]);
	else _BitScanForward64(&result, bb[t]);

	return (Square)result;
}

//returns INT_MAX if the current player won, INT_MIN if the current player lost, 0 otherwise
int Board::gameOver() const
{
	/*if (bb[WHITE] & row8 || !bb[BLACK]) {
		if (!_turn) return -1*INT_MAX;
		if (_turn) return INT_MAX;
	}
	if (bb[BLACK] & row1 || !bb[WHITE]) {
		if (!_turn) return INT_MAX;
		if (_turn) return -1*INT_MAX;
	}*/
	if (bb[WHITE] & row8 || !bb[BLACK] || bb[BLACK] & row1 || !bb[WHITE]) return -1 * INT_MAX;
	
	return 0;
}
