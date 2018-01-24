#pragma once
#include "GameBoard.h"
#include "Player.h"
#include <string>

class Game
{
public:
	Game(); 
	~Game();
	
	void newGame(Player *, Player *);
	bool checkForWin();
	bool getMove(int);
	int getTurn();
	void nextTurn();
	
	GameBoard* getGameBoard() const;
	string getMoveFrom() { return moveFrom; };
	string getMoveTo() { return moveTo; };

	
private:
	Player * white;
	Player * black;
	GameBoard * gameBoard;
	string moveFrom;
	string moveTo;
	int getNumber(char);
	int pTurn = 0;
	int player1Pieces = 16;
	int player2Pieces = 16;

	bool checkLastRow(char);
	bool checkForPieces(char);

	bool updateBoard(string, string, int);

	void checkMove(string, string);
};