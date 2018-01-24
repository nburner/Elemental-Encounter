#include "AI.h"

using namespace AI;

AIEngine::BasicOffense::BasicOffense() {

}

string AIEngine::BasicOffense::operator()(const Board b) const {
	auto boards = b.validNextBoards();
	return BoardHelpers::to_string(b.blackTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove);
}