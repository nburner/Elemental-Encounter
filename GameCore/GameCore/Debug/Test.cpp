#include "Game.h"
#include "..\..\..\AI Development Project\BoardStateTool\AI.h"
#include <iostream>
//using namespace std;
using std::cout; using std::endl;

void main()
{

	Human * player1 = new Human();
	Human * player2 = new Human();
	Daryl * daryl = new Daryl();
	
	AI::AIEngine::AI * def = AI::AIEngine::start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * def2 = AI::AIEngine::start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * off = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);
	AI::AIEngine::AI * off2 = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);

	int irandom = 0; int ipet = 0;

#pragma omp parallel for reduction(+:ipet, irandom)
	for (int i = 0; i < 100; i++) {
		Game * game = new Game(0);

		AI::AIEngine::AI * pet = AI::AIEngine::start(AI::AIEngine::AIType::DARYLS_PET);
		AI::AIEngine::AI * random = AI::AIEngine::start(AI::AIEngine::AIType::B_RANDOM);

		game->newGame(random, pet, false);

		if (game->getTurn() % 2 == 1) ++irandom;
		else ++ipet;
		cout << "\r" << "Pet: " << ipet << "\t Random: " << irandom;


		delete game;
		game = new Game(0);

		game->newGame(pet, random, false);

		if (game->getTurn() % 2 == 1) ++ipet;
		else ++irandom;
		cout << "\r" << "Pet: " << ipet << "\t Random: " << irandom;

		delete game;
	}
}

