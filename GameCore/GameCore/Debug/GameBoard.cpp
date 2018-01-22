#include "GameBoard.h"

GameBoard::GameBoard()
{
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