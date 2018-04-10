#include "AI.h"
#include "timer.h"
#include <fstream>

using namespace AI;
using std::ifstream; using std::ofstream;
typedef vector<Board>::iterator BoardIterator;

static int seekDepth = INT_MAX;
static double seekTime = 4;
static bool cutShortByDepth = true;

static bool verbose = false;

AI::Seeker::Seeker()
{
	t.start();
	MAX_SEEK_DEPTH = 20;
	FINAL_ALPHA_BETA_TIME = 5.88;
	SEEK_WIN = 2.5;
	SEEK_OPPONENT_WIN_SHORT = 1;
	SEEK_OPPONENT_WIN_LONG = 2.5;
	SEEK_BLOCKING_MOVE = 2;
}

AI::Seeker::Seeker(int) : Prune(0)
{
	t.start();
	MAX_SEEK_DEPTH = 20;
	FINAL_ALPHA_BETA_TIME = 5.88;
	SEEK_WIN = 2.5;
	SEEK_OPPONENT_WIN_SHORT = 1;
	SEEK_OPPONENT_WIN_LONG = 2.5;
	SEEK_BLOCKING_MOVE = 2;
}

AI::Seeker::Seeker(int, int i) : Prune(0)
{
	t.start();
	MAX_SEEK_DEPTH = 20;
	FINAL_ALPHA_BETA_TIME = 5.88;
	SEEK_WIN = 2.5;
	SEEK_OPPONENT_WIN_SHORT = 1;
	SEEK_OPPONENT_WIN_LONG = 2.5;
	SEEK_BLOCKING_MOVE = 2;

	readWeights(i);
}

AI::Seeker::Seeker(std::string hinter) : Prune(0)
{
	t.start();
	MAX_SEEK_DEPTH = 20;
	SEEK_WIN = .5;
	FINAL_ALPHA_BETA_TIME = 1;
	SEEK_OPPONENT_WIN_SHORT = 1;
	SEEK_OPPONENT_WIN_LONG = 1;
	SEEK_BLOCKING_MOVE = 1;
}


move seek(const Board& b, int movesMade = 0, const timer& time = timer());

inline bool movesToWinComparer(const Board& b1, const Board& b2) {
	return b1.movesBeforeWin < b2.movesBeforeWin;
}

inline bool valueComparer(const Board& b1, const Board& b2) {
	return b1.val < b2.val;
}

bool stop(const Board& b, int movesMade, const timer& time) {
	//Timer break point
	if (time.read() > seekTime) return true;
	
	auto boards = b.validNextBoards();
	int maxMovesBeforeWin = 0;

	for (int i = 0; i < boards.size(); i++) {
		move m = seek(boards[i], movesMade, time);
		maxMovesBeforeWin = std::max(boards[i].movesBeforeWin, maxMovesBeforeWin);
		if (m.first == m.second) return true;
	}

	b.movesBeforeWin = maxMovesBeforeWin + 1;
	return false;
}

move seek(const Board& b, int movesMade, const timer& time) {
	//Check to see if you can make a winning move : GOOD
	auto boards = b.validWinBoards(); if (!boards.empty()) {
		b.movesBeforeWin = 1;
		return boards[0].lastMove;
	}
	
	//Seek Depth anchor point
	if (++movesMade == seekDepth) {
		cutShortByDepth = true;
		return { A1, A1 };
	}

	//Timer break point
	if (time.read() > seekTime) return { A1, A1 }; 

	//Check to see if you have pawns and they don't : GOOD
	if (b.hasPawns(b.turn()) && !b.hasPawns(Turn(!b.turn()))) {
		Square from = b.furthestPiece(b.turn() ? BLACK : WHITE);
		Square to = Square(from + (b.turn() ? SOUTH : NORTH));
		b.movesBeforeWin = b.turn() ? from / 8 : 7 - from / 8;
		return { from, to };
	}
	
	//Look for moves that can't be stopped : GOOD
	boards = b.validNextBoards();
	for (auto it = boards.begin(); it != boards.end(); ) {
		if (stop(*it, movesMade, time)) it = boards.erase(it);
		else ++it;
	}
	if(!boards.empty()) return std::min_element(boards.begin(), boards.end(), movesToWinComparer)->lastMove;
		
	//Couldn't find any unstoppable moves : BAD
	return { A1, A1 };
}

move Seeker::deepTimedSeek(const Board& b, double time, int depth = 40) const {
	move seekResult = { A1, A1 };
	timer seekTimer; seekTimer.start(); seekTime = time;
	for (seekDepth = 2; seekDepth < MAX_SEEK_DEPTH && seekTimer.read() < seekTime && cutShortByDepth; seekDepth++) {
		cutShortByDepth = false;
		move newResult = seek(b, 0, seekTimer);
		seekResult = newResult.first == newResult.second ? seekResult : newResult;
	}
	return seekResult;
}

//Decent outline of flow of this method:
// - Win (no time)
// - Seek to Win (SEEK_WIN)
// ~ No way to win
//    - Seek opponent's win after your move (SEEK_OPPONENT_WIN_LONG)
//    - Alphabeta eval remaining moves (FINAL_ALPHA_BETA_TIME aka all remaining time)
// ~ Some way to win
//    - Seek opponent's win after your move (SEEK_OPPONENT_WIN_SHORT)
//    ~ If they do have a way to win
//       - Look for a way to block them (whatever that means) (SEEK_BLOCKING_MOVE)
//       ~ If there's a way to block them
//         - Alphabeta eval remaining moves (FINAL_ALPHA_BETA_TIME aka all remaining time)
move AI::Seeker::operator()(const Board b) const
{
	t.reset(); 

	//Always win if you can
	auto boards = b.validWinBoards();
	if (!boards.empty()) {
		if(verbose) cout << "This move took: " << t.read() << " seconds" << endl;
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
			return result;
		}
	//Otherwise evaluate your remaining options
		for (int i = 0; i < boards.size(); i++) {
			evaluate(boards[i]);
			boards[i].val /= 2;
			boards[i].val += alphabeta(boards[i], 0, FINAL_ALPHA_BETA_TIME, t) / 2;
		}
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
				//Maybe we can beat them to it
				if (minMovesToWin >= b.movesBeforeWin) return seekResult;
				//Maybe we can try to slow them down
				else return bestShot;
			}
	//Otherwise pick the best move that will stop them
			for (int i = 0; i < boards.size(); i++) {
				evaluate(boards[i]);
				boards[i].val /= 2;
				boards[i].val += alphabeta(boards[i], 0, FINAL_ALPHA_BETA_TIME, t) / 2;
			}
		}
	}

	//if (t.read() > 6) 
	if (verbose) cout << "This move took: " << t.read() << " seconds" << endl;
	return std::max_element(boards.begin(), boards.end(), valueComparer)->lastMove;
}

void AI::Seeker::readWeights(int k)
{
	ifstream fin("weights" + std::to_string(k) + ".save");

	if (fin.is_open())
		for (int i = 0; i < NULL_FEATURE; i++)
			for (int j = 0; j < NULL_FEATURE; j++) {
				int _;
				fin >> _;
				weights[i][j] = _;
			}
	else throw std::invalid_argument("weights" + std::to_string(k) + ".save not found");
}

void AI::Seeker::writeWeights(int k)
{
	ofstream fout("weights" + std::to_string(k) + ".save");
	for (int i = 0; i < NULL_FEATURE; i++) {
		for (int j = 0; j < NULL_FEATURE; j++)
			fout << std::to_string(weights[i][j]) << '\t';
		fout << std::endl;
	}
}

const int MIN_WEIGHT = -128;
const int MAX_WEIGHT = 127;

bool AI::Seeker::adjustWeight(BoardFeature f1, BoardFeature f2, int i)
{
	if ((weights[f1][f2] >= MAX_WEIGHT && i > 0) || (weights[f1][f2] <= MIN_WEIGHT && i < 0)) return false;
	
	int result = weights[f1][f2] + i;
	result = std::max(result, MIN_WEIGHT);
	result = std::min(result, MAX_WEIGHT);
	
	weights[f1][f2] = result;
	return i != 0;
}

