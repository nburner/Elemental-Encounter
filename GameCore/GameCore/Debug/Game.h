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
	int getTurn();
	Turn getColorTurn();
	void nextTurn();
	GameBoard& getGameBoard() const;

private:
	Player * players[2];
	GameBoard * gameBoard;
	bool playerTurn = 0;
	int pieces[2] = { 16,16 };

	bool checkLastRow(char);
	bool checkForPieces(char);
	bool updateBoard(move move);
	void startGameLoop();
	void printBoard();
};