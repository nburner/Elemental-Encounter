// Fill out your copyright notice in the Description page of Project Settings.

#pragma once
#include "Player1.h"
#include "CoreMinimal.h"


/**
 * 
 */
class ELEMENTALENCOUNTER_API Game
{
public:
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
};
