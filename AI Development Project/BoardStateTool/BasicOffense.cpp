#include "AI.h"

using namespace AI;

AIEngine::BasicOffense::BasicOffense() {

}

move AIEngine::BasicOffense::operator()(const Board b) const {
	auto boards = b.validNextBoards();
	return b.blackTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove;
}