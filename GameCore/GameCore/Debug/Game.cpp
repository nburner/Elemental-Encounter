#include "Game.h"



Game::Game()
{
	gameBoard = GameBoard::getInstance(); 
}


Game::~Game(){
	delete gameBoard;
	gameBoard = NULL;
}

GameBoard* Game::getGameBoard() const
{
	return gameBoard;
}


bool Game::checkForWin()
{
	bool Victory = false;

	if (pTurn % 2 == 1) 
	{
		Victory = checkLastRow('W');
		//Victory = checkForPieces('B');
	} 
	else
	{
		Victory = checkLastRow('B');
		//Victory = checkForPieces('W');
	}

	return Victory;
}

void Game::newGame() 
{
	bool flip = 0;
	int bar = 219;

	for (int r = 0; r < 8; r++)
	{
		for (int c = 0; c < 8; c++)
		{
			if (r == 0 || r == 1)
			{
				GameBoard::getInstance()->space[r][c] = 'B';
			} 
			else if (r == 6 || r == 7)
			{
				GameBoard::getInstance()->space[r][c] = 'W';
			}
			else 
			{
				if (flip == 0)
				{
					GameBoard::getInstance()->space[r][c] = ' ';
					flip = 1;
				}
				else
				{
					GameBoard::getInstance()->space[r][c] = (char)bar;
					flip = 0;
				}
			}
		}

		flip = !flip;
	}
}

bool Game::getMove(int player)
{
	cout << "Player " << player <<"'s move" << endl;
	cout << "Move From: ";
	cin >> moveFrom;
	cout << "Move To: ";
	cin >> moveTo;

	if (moveFrom == "exit" || moveTo == "exit")
		return false;

	updateBoard(moveFrom, moveTo, player);

	return true;
}

int Game::getTurn()
{
	return pTurn;
}

void Game::nextTurn()
{
	pTurn++;
}

bool Game::checkMove(string from, string to)
{
	// check to see if moving backwards
	// check to see if moving sideways 
	// check to see if moving your piece
	// check to see if a piece is where you want to move

	return false;
}



int Game::getNumber(char c)
{
	int num = 7;

	switch (c) {
	case 'a': num = 0;
		break;
	case 'b': num = 1;
		break;
	case 'c': num = 2;
		break;
	case 'd': num = 3;
		break;
	case 'e': num = 4;
		break;
	case 'f': num = 5;
		break;
	case 'g': num = 6;
		break;
	case 'h': num = 7;
		break;
	case '1': num = 7;
		break;
	case '2': num = 6;
		break;
	case '3': num = 5;
		break;
	case '4': num = 4;
		break;
	case '5': num = 3;
		break;
	case '6': num = 2;
		break;
	case '7': num = 1;
		break;
	case '8': num = 0;
		break;
	}

	return num;
}

bool Game::checkLastRow(char piece)
{
	bool playerWon = false;

	if (piece == 'W')
	{
		for (int i = 0; i < 9; i++) 
		{
			if (getGameBoard()->space[0][i] == piece)
			{
				playerWon = true;
			}
		}
	} 
	else if (piece == 'B')
	{
		for (int i = 0; i < 9; i++) 
		{
			if (getGameBoard()->space[7][i] == piece)
			{
				playerWon = true;
			}
		}
	}

	return playerWon;
}

bool Game::checkForPieces(char)
{
	return false;
}

bool Game::updateBoard(string from, string to, int player) 
{
	
	int fromR = getNumber(from[0]);
	int fromC = getNumber(from[1]);

	int toR = getNumber(to[0]);
	int toC = getNumber(to[1]);

	getGameBoard()->space[fromC][fromR] = ' ';

	if(player == 1)
		getGameBoard()->space[toC][toR] = 'W';
	else 
		getGameBoard()->space[toC][toR] = 'B';

	return true;
	
}
