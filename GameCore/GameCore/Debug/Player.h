#pragma once
#include "GameBoard.h"
//#include <string>

class Player
{
public:
	Player();
	~Player();
	virtual ::move getMove(GameBoard * gp = NULL) = NULL;
};

class Human : public Player {
	
public:
	virtual ::move getMove(GameBoard * gp = NULL);
};