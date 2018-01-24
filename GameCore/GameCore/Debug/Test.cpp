#include "Game.h"
#include "..\..\..\AI Development Project\BoardStateTool\AI.h"
#include <iostream>
using namespace std;


void main()
{
	Game game;

	Human * player1 = new Human();
	Human * player2 = new Human();
	
	AI::AIEngine::AI * ai = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);
	
	game.newGame(player1, ai);
	
	if (game.getTurn() % 2 == 1)
	{
		cout << "White Wins!!" << endl;
	} 
	else
	{
		cout << "Black Wins!!!" << endl;
	}
}

