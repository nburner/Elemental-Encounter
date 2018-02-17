#include "AI.h"
#include <random>
#include <ctime>

using namespace AI;

static timer _timer;
int Prune::PLY_COUNT = 2;
const auto TIME_CUTOFF = 5.85;

AI::Prune::Prune(int) : Pet(0)
{
}

int Prune::alphabeta(Board board, int currentPly) const {
	auto boards = board.validNextBoards();

	if (boards.empty() || currentPly == this->PLY_COUNT) {
		evaluate(board);
		return board.val;
	}

	for (int i = 0; i < boards.size(); i++) {
		boards[i].alpha = -1 * board.beta;
		boards[i].beta = -1 * board.alpha;
		boards[i].val = -1 * alphabeta(boards[i], currentPly + 1);

		if (boards[i].val >= board.beta)
			return board.beta;   //  fail hard beta-cutoff
		if (boards[i].val > board.alpha)
			board.alpha = boards[i].val; // alpha acts like max in MiniMax

		if (_timer.read() > TIME_CUTOFF) break;
	}

	return board.alpha;
}

bool valueCompare(const Board& a, const Board& b) {
	return a.val < b.val;
}

move Prune::operator()(const Board b) const
{
	_timer.start();
	
	PLY_COUNT = 2;
	move result; bool timeRemaining = true;
	
	while (timeRemaining) {
		auto boards = b.validWinBoards();
		if (!boards.empty()) return boards[0].lastMove;
		boards = b.validNextBoards();

		++PLY_COUNT;
		result = std::max_element(boards.begin(), boards.end(), valueCompare)->lastMove;

		//#pragma omp parallel for
		for (int i = 0; i < boards.size(); i++) {
			evaluate(boards[i]);
			boards[i].val /= 2;
			boards[i].val += alphabeta(boards[i], 0) / 2;

			if (_timer.read() > TIME_CUTOFF) {
				timeRemaining = false;
				break;
			}
		}
	}
	//cout << "AI's Move: " << BoardHelpers::to_string(result.first) << " - " << BoardHelpers::to_string(result.second) << endl;
	//cout << " Best Val: " << bestVal << endl;

	cout << "PLY_COUNT: " << PLY_COUNT << endl;
	cout << "This move took: " << _timer.read() << " seconds" << endl;
	return result;
}