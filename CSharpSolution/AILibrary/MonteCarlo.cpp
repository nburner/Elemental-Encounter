#include "Board.h"
#include "timer.h"
#include "AI.h"
#include <map>
#include <algorithm>
using namespace AI;

inline bool compareWins(const std::pair<move, int>& p1, const std::pair<move, int>& p2) {
	return p1.second < p2.second;
}
inline bool compareAvgTurns(const std::pair<move, double>& p1, const std::pair<move, double>& p2) {
	return p1.second > p2.second;
}

move AI::MonteCarlo(Board b) {
	timer t; t.start();
	auto boards = b.validWinBoards();
	if (!boards.empty()) return boards[0].lastMove;

	boards = b.validNextBoards();
	
	std::map<move, int> winCount;
	std::map<move, double> avgTurnCount;

	int k = 0;
	while (t.read() < 2.5) {
		#pragma omp parallel for num_threads(6)
		for (int i = 0; i < boards.size(); i++) {
			auto board = boards[i];
			int turnCount = 0;
			while (!board.gameOver()) {
				auto next = board.validWinBoards();
				if (next.empty()) next = board.validNextBoards();
				board = board.makeMove(next[rand() % next.size()].lastMove);
				turnCount++;
			}

			avgTurnCount[boards[i].lastMove] = avgTurnCount[boards[i].lastMove] * k / (double)(k+1.0) + turnCount / (double)(k+1.0);
			winCount[boards[i].lastMove] += board.turn() != b.turn();
			k++;
		}
	}

	auto winner = std::max_element(winCount.begin(), winCount.end(), compareWins);
	auto runnerUp = std::max_element(avgTurnCount.begin(), avgTurnCount.end(), compareAvgTurns);

	cout << "Monte Carlo ran " << k << " games in " << t.read() << " seconds" << endl;
	cout << "Monte Carlo picked a move that won " << winner->second << " games out of " << k / boards.size() << " in an average " << avgTurnCount[winner->first] << " turns" << endl;
	cout << "instead of picking a move that won " << winCount[runnerUp->first] << " games out of " << k / boards.size() << " in an average " << runnerUp->second << " turns" << endl;
	return winner->first;
}