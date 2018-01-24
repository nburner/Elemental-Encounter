#include "AI.h"

using namespace AI;

AIEngine::BasicDefense::BasicDefense() {

}

string AIEngine::BasicDefense::operator()(const Board b) const {
	auto boards = b.validNextBoards();
	return BoardHelpers::to_string(b.whiteTurn() ? boards.begin()->lastMove : boards.rbegin()->lastMove);
}