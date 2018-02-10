#include "Game.h"
#include "..\..\..\AI Development Project\BoardStateTool\AI.h"
#include <iostream>
//using namespace std;
using std::cout; using std::endl;

void main()
{
	auto engine = AI::AIEngine();
	Human * player1 = new Human();
	Human * player2 = new Human();
	Daryl * daryl = new Daryl();
	
	AI::AIEngine::AI * def = engine.start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * def2 = engine.start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * off = engine.start(AI::AIEngine::AIType::B_OFFENSE);
	AI::AIEngine::AI * off2 = engine.start(AI::AIEngine::AIType::B_OFFENSE);

	int irandom = 0; int ipet = 0;

//#pragma omp parallel for reduction(+:ipet, irandom)
	for (int i = 0; i < 5; i++) {
		Game * game = new Game(0);

		AI::AIEngine::AI * pet = engine.start(AI::AIEngine::AIType::DARYLS_PET);
		AI::AIEngine::AI * random = engine.start(AI::AIEngine::AIType::B_RANDOM);
		AI::AIEngine::AI * randompet = engine.start(AI::AIEngine::AIType::MEM_PET);

		/*game->newGame(random, pet, false);

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
		game = new Game(0);*/

		game->newGame(player1, pet, true);

		if (game->getTurn() % 2 == 1) ++irandom;
		else ++ipet;
		cout << "\r" << "Pet: " << ipet << "\t Random: " << irandom;


		delete game;
		game = new Game(0);

		game->newGame(pet, randompet, false);

		if (game->getTurn() % 2 == 1) ++ipet;
		else ++irandom;
		cout << "\r" << "Pet: " << ipet << "\t Random: " << irandom;

		delete game;

		static_cast<AI::AIEngine::MemoryPet*>(randompet)->save(i);
		engine.clean();
	}
}

