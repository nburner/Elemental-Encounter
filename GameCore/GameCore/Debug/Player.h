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
	bool isValidMove(const move) const;
public:
	virtual ::move getMove(GameBoard * gp = NULL);
};