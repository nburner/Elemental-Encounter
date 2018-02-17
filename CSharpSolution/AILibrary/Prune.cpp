#include "stdafx.h"
#include "AI.h"
#include "Board.h"
#include "timer.h"
#include <random>
#include <set>
#include <ctime>

using namespace AI;

int Prune::PLY_COUNT = 4;

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
	}

	return board.alpha;
}

move Prune::operator()(const Board b) const
{
	static timer _timer;
	_timer.start();
	//PLY_COUNT = 3 + (32 - featureCalculators[MY_PAWN_COUNT](b) - featureCalculators[THEIR_PAWN_COUNT](b))/2;

	auto boards = b.validWinBoards();
	if (!boards.empty()) return boards[0].lastMove;

	boards = b.validNextBoards();

	move result; int bestVal = INT_MIN;

#pragma omp parallel for
	for (int i = 0; i < boards.size(); i++) {
		evaluate(boards[i]);
		boards[i].val /= 2;
		boards[i].val += alphabeta(boards[i], 0) / 2;
		if (boards[i].val > bestVal) {
			bestVal = boards[i].val;
			result = boards[i].lastMove;
		}
	}

	/*std::sort(boards.begin(), boards.end(), [](Board& a, Board& b) {
	return a.val > b.val;
	});*/

	//cout << "AI's Move: " << BoardHelpers::to_string(result.first) << " - " << BoardHelpers::to_string(result.second) << endl;
	//cout << " Best Val: " << bestVal << endl;

	//cout << "PLY_COUNT: " << PLY_COUNT << endl;
	cout << "This move took: " << _timer.read() << " seconds" << endl;
	return result;
}