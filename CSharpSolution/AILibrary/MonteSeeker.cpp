#include "Board.h"
#include "timer.h"
#include "AI.h"
#include <map>
#include <algorithm>
#include <thread>
#include <atomic>
#include <mutex>
using namespace AI;

static int seekDepth = INT_MAX;
static double seekTime = 4;
static bool cutShortByDepth = true;

static bool verbose = true;

static std::atomic_bool waitingForCarlo = false;
static std::mutex scoreMutex;

inline bool valueComparer(const Board& b1, const Board& b2) {
	return b1.val < b2.val;
}
inline bool compareWins(const std::pair<Board, std::pair<int, int>>& p1, const std::pair<Board, std::pair<int, int>>& p2) {
	return p1.second.first / (double)p1.second.second < p2.second.first / (double)p2.second.second;
}
inline bool compareAvgTurns(const std::pair<Board, double>& p1, const std::pair<Board, double>& p2) {
	return p1.second > p2.second;
}

AI::MonteSeeker::MonteSeeker(int) : Seeker(0)
{
}

static std::map<Board, std::pair<int, int>> monteCarloScores;

void AI::MonteSeeker::MonteCarlo(const MonteSeeker& m, const Board& b) {
	auto boards = b.validWinBoards();
	if (!boards.empty()) {
		waitingForCarlo = false;
		return;
	}

	boards = b.validNextBoards();

	//std::map<move, double> avgTurnCount;

	int k = 0;
	while (m.t.read() < 5.5 && waitingForCarlo) {
		#pragma omp parallel for num_threads(6)
		for (int i = 0; i < boards.size(); i++) {
			auto board = boards[i];
			int turnCount = 1;
			vector<Board> seenBoards;
			while (!board.gameOver()) {
				move nextMove;
				auto next = board.validWinBoards();
				if (next.empty()) next = board.validNextBoards();

				if (turnCount % 2) nextMove = next[rand() % next.size()].lastMove;
				else {
					for (int i = 0; i < next.size(); i++) m.evaluate(next[i]);
					std::sort(next.begin(), next.end(), valueComparer);
					nextMove = next[0].lastMove;
				}

				if (turnCount % 2) seenBoards.push_back(board);
				board = board.makeMove(nextMove);
			}

			//if(board.turn() != b.turn()) avgTurnCount[boards[i].lastMove] = avgTurnCount[boards[i].lastMove] * monteCarloScores[boards[i].lastMove] / (double)(monteCarloScores[boards[i].lastMove] + 1.0) + turnCount / (double)(monteCarloScores[boards[i].lastMove] + 1.0);
			for (int p = 0; p < seenBoards.size(); p++) {
				std::lock_guard<std::mutex> lock(scoreMutex);
				monteCarloScores[seenBoards[p]].first += board.turn() != b.turn();
				monteCarloScores[seenBoards[p]].second++;
			}
			k++;
		}
	}
	if (waitingForCarlo) {
		
		//auto runnerUp = std::max_element(avgTurnCount.begin(), avgTurnCount.end(), compareAvgTurns);

		cout << "Monte Carlo ran " << k << " games in " << m.t.read() << " seconds" << endl;
		//cout << "Monte Carlo picked a move (" << BoardHelpers::to_string(winner.lastMove) << ") that won " << monteCarloScores[winner].first << " games out of " << monteCarloScores[winner].second
		//	<< endl; //<< " in an average " << avgTurnCount[winner->first] << " turns" << endl;
		//cout << "instead of picking a move (" << BoardHelpers::to_string(runnerUp->first) << ") that won " << monteCarloScores[runnerUp->first] << " games out of " << k / boards.size() << " in an average " << runnerUp->second << " turns" << endl;
		//MonteCarloResult = winner.lastMove;
		waitingForCarlo = false;
	}
}

move AI::MonteSeeker::operator()(const Board b) const
{
	t.reset();
	waitingForCarlo = true;

	std::thread monteCarloThread = std::thread(&MonteSeeker::MonteCarlo, *this, b);

	//Always win if you can
	auto boards = b.validWinBoards();
	if (!boards.empty()) {
		if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
		monteCarloThread.join();
		while (waitingForCarlo);
		return boards[0].lastMove;
	}

	//If you can't, look for a way to win
	move seekResult = deepTimedSeek(b.onlyFront(), SEEK_WIN);
	if (verbose) cout << "Seek took " << t.read() << " and returned " << BoardHelpers::to_string(seekResult) << endl;

	//double savedTime = std::max(.0, SEEK_WIN - t.read());
	double savedTime = 0;

	//Suppose you couldn't find a way to victory
	if (seekResult.first == seekResult.second) {
		//Then look at all the possible moves
		boards = b.validNextBoards();
		//And ignore all the ones that would cause your enemy to win
		double allottedTime = (SEEK_OPPONENT_WIN_LONG + savedTime) / boards.size();
		for (auto it = boards.begin(); it != boards.end(); ) {
			move test = deepTimedSeek(it->onlyFront(), allottedTime);
			if (test.first != test.second) it = boards.erase(it);
			else ++it;
		}
		//If there are no remaining options then sucks to suck lol, just race
		//TODO - Currently this makes it obvious (to me at least) that you've got a solid victory lined up. I need a better defensive ploy too.
		if (boards.empty()) {
			move result;
			//NO! No more just racing! Now we attack! And if we can't then we race :P
			boards = b.validAttackBoards();

			if (!boards.empty()) result = boards[0].lastMove;
			else {
				result.first = b.furthestPiece(b.turn() ? BLACK : WHITE);
				result.second = Square(result.first + (b.turn() ? SOUTH : NORTH) + (result.first % 8 == 0 ? EAST : WEST));
			}

			if (verbose) cout << "Seeker: I think I'm screwed" << endl;
			waitingForCarlo = false;
			monteCarloThread.join();
			if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
			return result;
		}
		//Otherwise evaluate your remaining options
		//Scratch that evaluating swapped with Monte Carlo
		monteCarloThread.join();
		while (waitingForCarlo);
		
		Board winner; double maxWinRatio = -1;
		for (int i = 0; i < boards.size(); i++) {
			double winRatio = monteCarloScores[boards[i]].first / (double)monteCarloScores[boards[i]].second;
			if (winRatio > maxWinRatio && !featureCalculators[THREATENED_UNDEFENDED_B](boards[i])) {
				maxWinRatio = winRatio;
				winner = boards[i];
			}
		}
		if (maxWinRatio < 0) {
			boards = b.validAttackBoards();
			for (int i = 0; i < boards.size(); i++) {
				double winRatio = monteCarloScores[boards[i]].first / (double)monteCarloScores[boards[i]].second;
				if (winRatio > maxWinRatio) {
					maxWinRatio = winRatio;
					winner = boards[i];
				}
			}
		}
		if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
		return winner.lastMove;
	}
	//But if there is a way to victory, ensure it doesn't cost you the game
	else {
		//See if your enemy will have a way to victory after your move
		move test = deepTimedSeek(b.makeMove(seekResult).onlyFront(), SEEK_OPPONENT_WIN_SHORT + savedTime);
		//savedTime = std::max(.0, SEEK_OPPONENT_WIN_SHORT + SEEK_WIN - t.read());		
		//If they don't, good for you!
		if (test.first == test.second) {
			waitingForCarlo = false;
			monteCarloThread.join();
			if (verbose) cout << "Seeker: I've got you now!" << endl;
			if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
			return seekResult;
		}
		//If they do, see if there is a way to stop them
		else {
			boards = b.validNextBoards();
			int maxMovesToWin = 0; int minMovesToWin = 165; move bestShot = { A1, A1 };
			double allottedTime = (SEEK_BLOCKING_MOVE + savedTime) / boards.size();
			for (auto it = boards.begin(); it != boards.end(); ) {
				move test = deepTimedSeek(it->onlyFront(), allottedTime);
				if (test.first != test.second) {
					minMovesToWin = (minMovesToWin > it->movesBeforeWin) ? it->movesBeforeWin : minMovesToWin;
					if (maxMovesToWin < it->movesBeforeWin) {
						maxMovesToWin = it->movesBeforeWin;
						bestShot = it->lastMove;
					}
					it = boards.erase(it);
				}
				else ++it;
			}
			//If there's no way to stop them...
			if (boards.empty()) {
				waitingForCarlo = false;
				monteCarloThread.join();
				if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
				//Maybe we can beat them to it
				if (minMovesToWin >= b.movesBeforeWin) return seekResult;
				//Maybe we can try to slow them down
				else return bestShot;
			}
			//Otherwise pick the best move that will stop them
			//Using Monte Carlo
			monteCarloThread.join();
			while (waitingForCarlo);

			Board winner; double maxWinRatio = -1;
			for (int i = 0; i < boards.size(); i++) {
				double winRatio = monteCarloScores[boards[i]].first / (double)monteCarloScores[boards[i]].second;
				if (winRatio > maxWinRatio) {
					maxWinRatio = winRatio;
					winner = boards[i];
				}
			}
			if (maxWinRatio < 0) {
				boards = b.validAttackBoards();
				for (int i = 0; i < boards.size(); i++) {
					double winRatio = monteCarloScores[boards[i]].first / (double)monteCarloScores[boards[i]].second;
					if (winRatio > maxWinRatio) {
						maxWinRatio = winRatio;
						winner = boards[i];
					}
				}
			}
			if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
			return winner.lastMove;
		}
	}

	//if (t.read() > 6) 
	if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
	waitingForCarlo = false;
	monteCarloThread.join();
	return std::max_element(boards.begin(), boards.end(), valueComparer)->lastMove;

}