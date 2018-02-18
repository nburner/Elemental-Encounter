#include "AI.h"
#include "timer.h"

using namespace AI;

AI::Seeker::Seeker()
{
}

AI::Seeker::Seeker(int) : Prune(0)
{
}

///Not sure necessary
static int seekDepth = INT_MAX;

move seek(const Board b, int movesMade = 0);

bool stop(const Board b, int movesMade) {
	auto boards = b.validNextBoards();
	for (int i = 0; i < boards.size(); i++) {
		move m = seek(boards[i], movesMade);
		if (m.first == m.second) return true;
	}

	return false;
}

move seek(const Board b, int movesMade) {
	//Check to see if you can make a winning move : GOOD
	auto boards = b.validWinBoards(); if (!boards.empty()) return boards[0].lastMove;
	
	//Seek Depth anchor point
	if (++movesMade == seekDepth) return { A1, A1 };

	//Check to see if you have pawns and they don't : GOOD
	if (b.hasPawns(b.turn()) && !b.hasPawns(Turn(!b.turn()))) {
		Square from = b.furthestPiece(b.turn() ? BLACK : WHITE);
		Square to = Square(from + (b.turn() ? -8 : 8));
		return { from, to };
	}
	
	//Look for moves that can't be stopped : GOOD
	boards = b.validNextBoards();
	for (int i = 0; i < boards.size(); i++) if (!stop(boards[i], movesMade)) return boards[i].lastMove;
	
	//Couldn't find any unstoppable moves : BAD
	return { A1, A1 };
}

move AI::Seeker::operator()(const Board b) const
{
	move result; static timer t;

	auto boards = b.validWinBoards();
	if (!boards.empty()) return boards[0].lastMove;
	
	t.start();
	//for(seekDepth = 2; seekDepth < 7; seekDepth++) 
		result = seek(b.ignoreBack());
	//cout << "Seek took " << t.read() << " and returned " << BoardHelpers::to_string(result) << endl;

	if (result.first == result.second) result = Prune::Pet::operator()(b);
	
	return result;
}
