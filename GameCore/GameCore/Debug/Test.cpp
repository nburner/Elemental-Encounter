#include "Game.h"
#include <iostream>
using namespace std;


void printBoard(const Game&);

void main()
{

	const bool GAMENOTWON = false;
	int Player1 = 1; 
	int Player2 = 2;

	Game game;

	game.newGame();
	
	printBoard(game);

	bool keepPlaying = true;

	do
	{
		game.nextTurn();
		if (game.getTurn() % 2 == 1)
		{
			keepPlaying = game.getMove(Player1);			
		}
		else
		{
			keepPlaying = game.getMove(Player2);
		}
		printBoard(game);

		

	} while (!game.checkForWin() && keepPlaying);

	if (game.getTurn() % 2 == 1)
	{
		cout << "Player 1 Win's!!" << endl;
	} 
	else
	{
		cout << "player 2 Win's!!!" << endl;
	}
	
}

void printBoard(const Game & game) 
{
	for (int r = 0; r < 8; r++)
	{
		switch (r) {
		case 0: cout << "8|";
				break;
		case 1: cout << "7|";
				break;
		case 2: cout << "6|";
			break;
		case 3: cout << "5|";
			break;
		case 4: cout << "4|";
			break;
		case 5: cout << "3|";
			break;
		case 6: cout << "2|";
			break;
		case 7: cout << "1|";
			break;
		}
		for (int c = 0; c < 8; c++)
		{
			cout << game.getGameBoard()->space[r][c] << "|";
		}
		cout << endl;
	}

	cout << "  A B C D E F G H" << endl << "-----------------------------" << endl;
}