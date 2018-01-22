#pragma once
#include "GameBoard.h"
#include <string>

class Game
{
public:
	Game(); 
	~Game();
	
	void newGame();
	bool checkForWin();
	bool getMove(int);
	
	GameBoard* getGameBoard() const;
	string getMoveFrom() { return moveFrom; };
	string getMoveTo() { return moveTo; };

	
private:
	GameBoard * gameBoard;
	string moveFrom;
	string moveTo;
	int getNumber(char);

	bool updateBoard(string, string, int);

	bool checkMove(string, string);
};