// Fill out your copyright notice in the Description page of Project Settings.
#include "GameBoard.h"

GameBoard::GameBoard()
{
	char bar = 219;

	for (int i = 0; i < 64; i++) {
		if ((i / 8 + i) % 2 == 0) space[i] = bar;
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

void GameBoard::updateBoard(move move, Turn t)
{
	space[move.first] = ((move.first / 8 + move.first) % 2 == 0 ? 219 : ' ');
	space[move.second] = (t == 0 ? 'W' : 'B');
	justTaken = t;
}

GameBoard* GameBoard::instance = NULL;