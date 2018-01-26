#include "Game.h"
#include "..\..\..\AI Development Project\BoardStateTool\AI.h"
#include <iostream>
//using namespace std;
using std::cout; using std::endl;

void main()
{
	Game game;

	Human * player1 = new Human();
	Human * player2 = new Human();
	
	AI::AIEngine::AI * def = AI::AIEngine::start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * def2 = AI::AIEngine::start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * off = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);
	AI::AIEngine::AI * off2 = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);
	
	game.newGame(def, def2);
	
	if (game.getTurn() % 2 == 1)
	{
		cout << "White Wins!!" << endl;
	} 
	else
	{
		cout << "Black Wins!!!" << endl;
	}
}

