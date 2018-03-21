#include "..\..\CSharpSolution\AILibrary\AI.h"
using namespace AI;

const int BEST_WEIGHTS = -1*INT_MAX;

typedef std::pair<Seeker, int> Specimen;

void main() {
	//So we'll start with some basic weights... Well, let's get them from a file.
	Seeker king(0);
	king.readWeights(BEST_WEIGHTS);
	
	//So we generate our weebs first
	int bump = 35;
	Specimen weebs[NULL_FEATURE * NULL_FEATURE * 2];
	
	for (int i = 0; i < NULL_FEATURE; i++)
		for (int j = 0; j < NULL_FEATURE; j++) {
			weebs[i*NULL_FEATURE + j].first.readWeights(BEST_WEIGHTS);
			weebs[i*NULL_FEATURE + j].first.adjustWeight((BoardFeature)i, (BoardFeature)j, bump);
		}

}