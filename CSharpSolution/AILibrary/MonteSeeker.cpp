#include "Board.h"
#include "timer.h"
#include "AI.h"
#include <map>
#include <algorithm>
#include <thread>
using namespace AI;

static int seekDepth = INT_MAX;
static double seekTime = 4;
static bool cutShortByDepth = true;

static bool verbose = false;

inline bool valueComparer(const Board& b1, const Board& b2) {
	return b1.val < b2.val;
}
inline bool compareWins(const std::pair<move, int>& p1, const std::pair<move, int>& p2) {
	return p1.second < p2.second;
}
inline bool compareAvgTurns(const std::pair<move, double>& p1, const std::pair<move, double>& p2) {
	return p1.second > p2.second;
}

AI::MonteSeeker::MonteSeeker(int) : Seeker(0)
{
}

void AI::MonteSeeker::MonteCarlo(const Board& b) const {
	auto boards = b.validWinBoards();
	if (!boards.empty()) {
		MonteCarloResult = boards[0].lastMove;
		return;
	}

	boards = b.validNextBoards();

	std::map<move, int> winCount;
	std::map<move, double> avgTurnCount;

	int k = 0;
	while (t.read() < 5.88 && waitingForCarlo) {
		for (int i = 0; i < boards.size(); i++) {
			auto board = boards[i];
			int turnCount = 0;
			while (!board.gameOver()) {
				move nextMove;
				auto next = board.validWinBoards();
				if (next.empty()) next = board.validNextBoards();
				
				if (turnCount % 2) nextMove = next[rand() % next.size()].lastMove;				
				else {
					for (int i = 0; i < next.size(); i++) evaluate(next[i]);
					nextMove = std::max_element(next.begin(), next.end(), valueComparer)->lastMove;
				}

				board = board.makeMove(nextMove);
				turnCount++;
			}

			avgTurnCount[boards[i].lastMove] = avgTurnCount[boards[i].lastMove] * k / (double)(k + 1.0) + turnCount / (double)(k + 1.0);
			winCount[boards[i].lastMove] += board.turn() != b.turn();
			k++;
		}
	}

	auto winner = std::max_element(winCount.begin(), winCount.end(), compareWins);
	auto runnerUp = std::max_element(avgTurnCount.begin(), avgTurnCount.end(), compareAvgTurns);

	cout << "Monte Carlo ran " << k << " games in " << t.read() << " seconds" << endl;
	cout << "Monte Carlo picked a move that won " << winner->second << " games out of " << k / boards.size() << " in an average " << avgTurnCount[winner->first] << " turns" << endl;
	cout << "instead of picking a move that won " << winCount[runnerUp->first] << " games out of " << k / boards.size() << " in an average " << runnerUp->second << " turns" << endl;
	MonteCarloResult = winner->first;
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
		waitingForCarlo = false;
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
			if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
			waitingForCarlo = false;
			return result;
		}
		//Otherwise evaluate your remaining options
		//Scratch that evaluating swapped with Monte Carlo
		monteCarloThread.join();
		if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
		waitingForCarlo = false;
		return MonteCarloResult;
	}
	//But if there is a way to victory, ensure it doesn't cost you the game
	else {
		//See if your enemy will have a way to victory after your move
		move test = deepTimedSeek(b.makeMove(seekResult).onlyFront(), SEEK_OPPONENT_WIN_SHORT + savedTime);
		//savedTime = std::max(.0, SEEK_OPPONENT_WIN_SHORT + SEEK_WIN - t.read());		
		//If they don't, good for you!
		if (test.first == test.second) {
			if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
			if (verbose) cout << "Seeker: I've got you now!" << endl;
			waitingForCarlo = false;
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
				if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
				waitingForCarlo = false;
				//Maybe we can beat them to it
				if (minMovesToWin >= b.movesBeforeWin) return seekResult;
				//Maybe we can try to slow them down
				else return bestShot;
			}
			//Otherwise pick the best move that will stop them
			//Using Monte Carlo
			monteCarloThread.join();
			if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
			waitingForCarlo = false;
			return MonteCarloResult;
		}
	}

	//if (t.read() > 6) 
	if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
	waitingForCarlo = false;
	return std::max_element(boards.begin(), boards.end(), valueComparer)->lastMove;
}