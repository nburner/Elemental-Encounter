#include "AI.h"
#include "timer.h"

using namespace AI;
typedef vector<Board>::iterator BoardIterator;

AI::Seeker::Seeker()
{
}

AI::Seeker::Seeker(int) : Prune(0)
{
}

///Not sure necessary
static int seekDepth = INT_MAX;
static timer t;

move seek(const Board& b, int movesMade = 0);

inline bool movesToWinComparer(const Board& b1, const Board& b2) {
	return b1.movesBeforeWin < b2.movesBeforeWin;
}

inline bool valueComparer(const Board& b1, const Board& b2) {
	return b1.val < b2.val;
}

bool stop(const Board& b, int movesMade) {
	//Timer break point
	//if (t.read() > 3.2) return true;
	
	auto boards = b.validNextBoards();
	int maxMovesBeforeWin = 0;

	for (int i = 0; i < boards.size(); i++) {
		move m = seek(boards[i], movesMade);
		maxMovesBeforeWin = std::max(boards[i].movesBeforeWin, maxMovesBeforeWin);
		if (m.first == m.second) return true;
	}

	b.movesBeforeWin = maxMovesBeforeWin;
	return false;
}

move seek(const Board& b, int movesMade) {
	//Check to see if you can make a winning move : GOOD
	auto boards = b.validWinBoards(); if (!boards.empty()) {
		b.movesBeforeWin = 1;
		return boards[0].lastMove;
	}
	
	//Seek Depth anchor point
	if (++movesMade == seekDepth) return { A1, A1 };

	//Timer break point
	//if (t.read() > 3.2) return { A1, A1 }; 

	//Check to see if you have pawns and they don't : GOOD
	if (b.hasPawns(b.turn()) && !b.hasPawns(Turn(!b.turn()))) {
		Square from = b.furthestPiece(b.turn() ? BLACK : WHITE);
		Square to = Square(from + (b.turn() ? SOUTH : NORTH));
		b.movesBeforeWin = 7 - from / 8;
		return { from, to };
	}
	
	//Look for moves that can't be stopped : GOOD
	boards = b.validNextBoards();
	for (auto it = boards.begin(); it != boards.end(); ) {
		if (stop(*it, movesMade)) it = boards.erase(it);
		else ++it;
	}
	if(!boards.empty()) return std::min_element(boards.begin(), boards.end(), movesToWinComparer)->lastMove;
		
	//Couldn't find any unstoppable moves : BAD
	return { A1, A1 };
}

move AI::Seeker::operator()(const Board b) const
{
	t.start(); 
	move seekResult;

	//Always win if you can
	auto boards = b.validWinBoards();
	if (!boards.empty()) return boards[0].lastMove;
	
	//If you can't, look for a way to win
	///for(seekDepth = 2; seekDepth < 7; seekDepth++) 
	seekResult = seek(b.ignoreBack());
	if(t.read() > 3.2) cout << "Seek took " << t.read() << " and returned " << BoardHelpers::to_string(seekResult) << endl;

	//Suppose you couldn't find a way to victory
	if (seekResult.first == seekResult.second) {
	//Then look at all the possible moves
		boards = b.validNextBoards();
	//And ignore all the ones that would cause your enemy to win
		for (auto it = boards.begin(); it != boards.end(); ) {
			move test = seek(it->ignoreBack());
			if (test.first != test.second) it = boards.erase(it);
			else ++it;
		}
	//If there are no remaining options then sucks to suck lol, just race
		if (boards.empty()) {
			cout << "Seeker: I think I'm screwed" << endl;
			Square from = b.furthestPiece(b.turn() ? BLACK : WHITE);
			Square to = Square(from + (b.turn() ? SOUTH : NORTH) + (from/8 == 0 ? EAST : WEST));
			return { from, to };
		}
	//Otherwise evaluate your remaining options
		for (int i = 0; i < boards.size(); i++) {
			evaluate(boards[i]);
			boards[i].val /= 2;
			boards[i].val += alphabeta(boards[i], 0) / 2;
		}
	}
	//But if there is a way to victory, ensure it doesn't cost you the game
	else {
	//See if your enemy will have a way to victory after your move
		move test = seek(b.makeMove(seekResult).ignoreBack());
	//If they don't, good for you!
		if (test.first == test.second) return seekResult; 
	//If they do, see if there is a way to stop them
		else {
			boards = b.validNextBoards();
			int maxMovesToWin = 0; int minMovesToWin = 165; move bestShot = { A1, A1 };
			for (auto it = boards.begin(); it != boards.end(); ) {
				move test = seek(it->ignoreBack());
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
			if (boards.empty()) 
	//Maybe we can beat them to it
				if(minMovesToWin >= b.movesBeforeWin) return seekResult;
	//Maybe we can try to slow them down
				else return bestShot;
	//Otherwise pick the best move that will stop them
			for (int i = 0; i < boards.size(); i++) {
				evaluate(boards[i]);
				boards[i].val /= 2;
				boards[i].val += alphabeta(boards[i], 0) / 2;
			}
		}
	}

	if (t.read() > 6) cout << "This move took: " << t.read() << " seconds" << endl;
	return std::max_element(boards.begin(), boards.end(), valueComparer)->lastMove;
}

