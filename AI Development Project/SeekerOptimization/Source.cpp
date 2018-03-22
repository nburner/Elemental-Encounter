#include "..\..\CSharpSolution\AILibrary\AI.h"
using namespace AI;

const int BEST_WEIGHTS = -1 * INT_MAX;
const int WEEB_WEIGHTS = -1 * INT_MAX + 1;
const int BEST_EVER_WEIGHTS = -1 * INT_MAX + 2;

typedef std::pair<Seeker, int> Specimen;

void evaluate(Specimen& s, bool black = true) {
	Board board(CommonBitboards::row1 | CommonBitboards::row2, CommonBitboards::row7 | CommonBitboards::row8, WHITE);
	
	Seeker * players[2];
	players[black] = &s.first;
	players[!black] = new Seeker(0);

	int turnCount = 0;

	while (++turnCount && !board.gameOver()) board.makeMove((*players[(turnCount + 1) % 2])(board));

	if (turnCount-- % 2 == black) s.second = INT_MIN - turnCount;
	else s.second = INT_MIN + turnCount;

	delete players[!black];
}

void main() {
	srand(time(NULL));
	bool alreadyMaxed[NULL_FEATURE][NULL_FEATURE] = { 0 };
	int maxedCount = 0;
	int bestScoreEvaa = INT_MIN;
		
	//So we generate our weebs first
	//And each weeb has their own feature bumped to start
	Specimen * weebs = new Specimen [NULL_FEATURE * NULL_FEATURE];
	int bump = 24;
	
	for (int i = 0; i < NULL_FEATURE; i++)
		for (int j = 0; j < NULL_FEATURE; j++) {
			weebs[i*NULL_FEATURE + j].first.readWeights(WEEB_WEIGHTS);
			weebs[i*NULL_FEATURE + j].first.adjustWeight((BoardFeature)i, (BoardFeature)j, bump);
		}
	
	while (maxedCount < NULL_FEATURE * NULL_FEATURE) {
		//Then we evaluate them all, well, not all anymore
#pragma omp parallel for
		for (int i = 0; i < NULL_FEATURE; i++)
			for (int j = 0; j < NULL_FEATURE; j++)
				if (!alreadyMaxed[i][j])
					if (rand() % 100 <= 400.0 / (NULL_FEATURE*NULL_FEATURE - maxedCount))
						evaluate(weebs[i*NULL_FEATURE + j]);

		//Then we see who did the best and is not already maxed
		std::pair<int, int> best; int bestScore = INT_MIN;
		for (int i = 0; i < NULL_FEATURE; i++)
			for (int j = 0; j < NULL_FEATURE; j++)
				if (!alreadyMaxed[i][j] && weebs[i*NULL_FEATURE + j].second > bestScore) {
					bestScore = weebs[i*NULL_FEATURE + j].second;
					best = { i,j };
				}

		//Then we save the weights if they were good
		if (bestScore >= bestScoreEvaa) {
			bestScoreEvaa = bestScore;
			weebs[best.first*NULL_FEATURE + best.second].first.writeWeights(BEST_EVER_WEIGHTS);
		}

		//Then we bump the winner's trait on everybody else
		for (int i = 0; i < NULL_FEATURE; i++)
			for (int j = 0; j < NULL_FEATURE; j++)
				if (!weebs[i*NULL_FEATURE + j].first.adjustWeight((BoardFeature)best.first, (BoardFeature)best.second, bump) && best.first == i && best.second == j) {
					alreadyMaxed[i][j] = true;
					maxedCount++;
				}

		//Then we repeat
		if (maxedCount % NULL_FEATURE == 1) bump = ++bump * .9375;
		cout << "Repetition is good for the soul" << endl;
	}

	delete[] weebs;
}