#include "Game.h"
#include <iostream>
using namespace std;


void main()
{

	const bool GAMENOTWON = false;
	int Player1 = 1; 
	int Player2 = 2;

	Game game;
	Human * player1 = new Human();
	Human * player2 = new Human();
	
	game.newGame(player1, player2);
	
	if (game.getTurn() % 2 == 0)
	{
		cout << "White Wins!!" << endl;
	} 
	else
	{
		cout << "Black Wins!!!" << endl;
	}
	
}

