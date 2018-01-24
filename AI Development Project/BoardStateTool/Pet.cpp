#include "AI.h"
#include <random>

using namespace AI;

std::default_random_engine generator;
//std::uniform_int_distribution<int> distribution(1, 6);
std::binomial_distribution<int> distribution(254, 0.5);
//int dice_roll = distribution(generator);  // generates number in the range 1..6 

AIEngine::Pet::Pet() {
	for (int bf1 = 0; bf1 < NULL_FEATURE; bf1++) {
		for (int bf2 = 0; bf2 < NULL_FEATURE; bf2++) {
			weights[bf1][bf2] = distribution(generator);
		}
	}
}

int AI::AIEngine::Pet::evaluate(Board board)
{
	return 0;
}

string AI::AIEngine::Pet::operator()(const Board b) const
{
	return string();
}