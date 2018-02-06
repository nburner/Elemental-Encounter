#include"AI.h"
#include"..\..\GameCore\GameCore\Debug\Game.h"
#include<iostream>
#include<set>
#include<queue>
#include<time.h>
#include<intrin.h>   
#include<windows.h> 
#include<random>
//#include <stdio.h> 
using namespace AI;
using std::cout; using std::endl; using std::set; using std::queue;

typedef std::pair<AI::AIEngine::MemoryPet*, int> organism;

const int GEN_SIZE = 10;
organism parents[GEN_SIZE];

BOOL CtrlHandler(DWORD fdwCtrlType)
{
	for (int i = 0; i < GEN_SIZE; i++) parents[i].first->save(i);
	return 0;
}

inline int random(int count) {
	return rand() % count;
}

bool pairCompare(const organism& firstElem, const organism& secondElem) {
	return firstElem.second > secondElem.second;
}





int main() {
	SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, 1);
	
	srand(time(NULL));
	AIEngine engine;

	//Get parents
	for (int i = 0; i < GEN_SIZE; i++) {
		parents[i].first = static_cast<AIEngine::MemoryPet*>(engine.start(AIEngine::AIType::MEM_PET));
		parents[i].second = 0;
	}

	for (int gen = 0; gen >= 0; gen++) {
		//Evaluate fitness
		bool iWon[GEN_SIZE][GEN_SIZE] = { 0 };
		//#pragma omp parallel for
		for (int i = 0; i < GEN_SIZE; i++)
			for (int j = 0; j < GEN_SIZE; j++)
				if (i != j) {
					Game game(0);
					game.newGame(parents[i].first, parents[j].first, false);

					if (game.getColorTurn() == BLACK) iWon[i][j] = true;
				}
		//Count up wins
		for (int i = 0; i < GEN_SIZE; i++)
			for (int j = 0; j < GEN_SIZE; j++)
				if (i != j)
					++(iWon[i][j] ? parents[i].second : parents[j].second);

		std::sort(parents, parents + GEN_SIZE, pairCompare);
		//Print
		cout << "GEN " << gen << endl;
		for (int i = 0; i < GEN_SIZE; i++) cout << parents[i].second << "\t";
		cout << endl;

		//Kill and rebirth
		for (int i = GEN_SIZE - 1; i >= 0; i--) {
			if (parents[i].second >= GEN_SIZE/2) parents[i].first->tweak();
			else {
				delete parents[i].first;
				parents[i].first = AIEngine::MemoryPet::mix(parents[rand() % i].first, parents[rand() % i].first);
			}

			parents[i].second = 0;
			parents[i].first->save(i);
		}
	}
	cout << endl << "You're gonna have to hit Ctrl-C because this project is dumb";
	while (true) { cout << "\r"; }
}
