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
	Turn getColorTurn();
	void nextTurn();
	
	GameBoard& getGameBoard() const;
	string getMoveFrom() { return moveFrom; };
	string getMoveTo() { return moveTo; };

	
private:
	Player * players[2];
	GameBoard * gameBoard;
	string moveFrom;
	string moveTo;
	int getNumber(char);
	int pTurn = 0;
	int pieces[2] = { 16,16 };

	bool checkLastRow(char);
	bool checkForPieces(char);

	bool updateBoard(move move);

	void checkMove(move);

	void startGameLoop();
	void printBoard();
};