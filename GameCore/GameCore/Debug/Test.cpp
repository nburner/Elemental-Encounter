#include "Game.h"
#include <iostream>
using namespace std;


void main()
{
	Game game;
	Human * player1 = new Human();
	Human * player2 = new Human();
	
	game.newGame(player1, player2);
	
	if (game.getTurn() % 2 == 1)
	{
		cout << "White Wins!!" << endl;
	} 
	else
	{
		cout << "Black Wins!!!" << endl;
	}
}

