#include "AI.h"

using namespace AI;

AIEngine::BasicDefense::BasicDefense() {

}

move AIEngine::BasicDefense::operator()(const Board b) const {
	auto boards = b.validAttackBoards();
	if (!boards.empty())	return b.blackTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove;

	boards = b.validNextBoards();
	return b.whiteTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove;
}