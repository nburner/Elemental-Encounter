#pragma once
#include "GameBoard.h"
//#include <string>

class Player
{
public:
	Turn myColor;
	Player();
	~Player();
	virtual ::move getMove(GameBoard * gp = NULL) = NULL;
};

class Human : public Player {
	bool isValidMove(move);
public:
	virtual ::move getMove(GameBoard * gp = NULL);
};