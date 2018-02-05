#include"AI.h"
#include"..\..\GameCore\GameCore\Debug\Game.h"
#include<iostream>
#include<set>
#include<queue>
#include<time.h>
#include<intrin.h>   
using namespace AI;
using std::cout; using std::endl; using std::set; using std::queue;

typedef std::pair<AI::AIEngine::AI*, int> organism;

inline int random(int count) {
	return rand() % count;
}

bool pairCompare(const organism& firstElem, const organism& secondElem) {
	return firstElem.second < secondElem.second;
}

int main() {
	srand(time(NULL));
	const int GEN_SIZE = 4;

	//Get parents
	organism parents[GEN_SIZE];
	for (int i = 0; i < GEN_SIZE; i++) {
		parents[i].first = AIEngine::start(AIEngine::AIType::PET);
		parents[i].second = 0;
	}

	//Evaluate fitness
	bool iWon[GEN_SIZE][GEN_SIZE] = { 0 };
#pragma omp parallel for
	for (int i = 0; i < GEN_SIZE; i++)
		for (int j = 0; j < GEN_SIZE; j++) 
			if (i != j) {
				Game game;
				game.newGame(parents[i].first, parents[j].first, false);

				if(game.getColorTurn() == WHITE) iWon[i][j] = true;
	}

	for (int i = 0; i < GEN_SIZE; i++)
		for (int j = 0; j < GEN_SIZE; j++)
			if (i != j) 
				++(iWon[i][j] ? parents[i].second : parents[j].second);

	std::sort(parents, parents + GEN_SIZE, pairCompare);

	for (int i = 0; i < GEN_SIZE; i++) cout << parents[i].second << endl;

}
