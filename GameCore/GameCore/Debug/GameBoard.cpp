#include "GameBoard.h"

GameBoard::GameBoard()
{
	char bar = 219;

	for (int i = 0; i < 64; i++) {
		if ((i/8 + i) % 2 == 0) space[i] = bar;
		else space[i] = ' ';
	}

	for (int i = A1; i <= H2; i++) space[i] = 'W';

	for (int i = A7; i <= H8; i++) space[i] = 'B';
}

GameBoard::~GameBoard()
{
	instance = NULL;
}

GameBoard* GameBoard::getInstance() 
{ 
	if (instance == NULL) {
		instance = new GameBoard();
	}

return instance;

}

bool GameBoard::hasOwnPiece(string space)
{
	return true;
}

GameBoard* GameBoard::instance = NULL;