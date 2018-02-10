#include"AI.h"
#include<ctime>

AI::AIEngine::BasicRandom::BasicRandom()
{
	srand(time(NULL));
}

move AI::AIEngine::BasicRandom::operator()(const Board b) const
{
	auto next = b.validWinBoards();
	if (next.empty()) next = b.validAttackBoards();
	if (next.empty()) next = b.validNextBoards();
	
	return next[rand() % next.size()].lastMove;
}