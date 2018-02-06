#include <iostream>
#include <fstream>
#include <random>
#include "AI.h"
using namespace AI;
using std::ifstream; using std::ofstream;

const int MAX_WEIGHT = 127;

AI::AIEngine::MemoryPet::MemoryPet(int k) {
	static std::default_random_engine generator;
	static const std::uniform_int_distribution<short> distribution(-1*MAX_WEIGHT, MAX_WEIGHT);
	setFeatureCalculators();

	ifstream fin("memorypet" + std::to_string(k) + ".save");
	
	if (fin.is_open())
		for (int i = 0; i < NULL_FEATURE; i++)
			for (int j = 0; j < NULL_FEATURE; j++) {
				int _;
				fin >> _;
				weights[i][j] = _;
			}
	else
		for (int i = 0; i < NULL_FEATURE; i++)
			//for (int j = 0; j < NULL_FEATURE; j++)
				weights[i][i] = distribution(generator);
}

void AI::AIEngine::MemoryPet::save(int k)
{
	ofstream fout("memorypet" + std::to_string(k) + ".save");
	for (int i = 0; i < NULL_FEATURE; i++) {
		for (int j = 0; j < NULL_FEATURE; j++)
			fout << std::to_string(weights[i][j]) << ' ';
		fout << std::endl;
	}
}


bool percentChance(int i) {
	static std::random_device rd;
	static std::mt19937 gen(rd());
	static std::bernoulli_distribution d;

	d = std::bernoulli_distribution(i / 100.0);

	return d(gen);
}

void AI::AIEngine::MemoryPet::tweak()
{
	for (int i = 0; i < NULL_FEATURE; i++)
		for (int j = 0; j < NULL_FEATURE; j++) {
			if (percentChance(1)) weights[i][j] = ~weights[i][j];
			if (percentChance(1)) weights[i][j] *= 1.25;
			if (percentChance(1)) weights[i][j] *= .75;
			if (percentChance(1)) weights[i][j] += 30;
			if (percentChance(1)) weights[i][j] += -30;
		}
}

AI::AIEngine::MemoryPet * AI::AIEngine::MemoryPet::mix(MemoryPet * mom, MemoryPet * dad)
{
	MemoryPet* result = new MemoryPet(-1);

	for (int i = 0; i < NULL_FEATURE; i++)
		for (int j = 0; j < NULL_FEATURE; j++)
			result->weights[i][j] = percentChance(50) ? mom->weights[i][j] : dad->weights[i][j];
	
	return result;
}
