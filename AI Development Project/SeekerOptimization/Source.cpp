#include "..\..\CSharpSolution\AILibrary\AI.h"
#include <map>
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

	while (++turnCount && !board.gameOver()) board = board.makeMove((*players[(turnCount + 1) % 2])(board));

	if (turnCount-- % 2 == black) s.second = INT_MIN - turnCount;
	else s.second = INT_MIN + turnCount;

	delete players[!black];
}

FeatureFunc featureCalculators[NULL_FEATURE] = { NULL };

void setFeatureCalculators() {
	featureCalculators[ROW_2_THREATENED] = FeatureFunctions::row_2_threatened;
	featureCalculators[ROW_2_THREATENED_B] = FeatureFunctions::row_2_threatened_b;
	featureCalculators[ROW_3_THREATENED] = FeatureFunctions::row_3_threatened;
	featureCalculators[ROW_3_THREATENED_B] = FeatureFunctions::row_3_threatened_b;
	featureCalculators[ROW_4_THREATENED] = FeatureFunctions::row_4_threatened;
	featureCalculators[ROW_4_THREATENED_B] = FeatureFunctions::row_4_threatened_b;
	featureCalculators[MY_PAWN_COUNT] = FeatureFunctions::my_pawn_count;
	featureCalculators[THEIR_PAWN_COUNT] = FeatureFunctions::their_pawn_count;
	featureCalculators[PIECE_ADVANTAGE] = FeatureFunctions::piece_advantage;
	featureCalculators[PIECE_ADVANTAGE_B] = FeatureFunctions::piece_advantage_b;
	featureCalculators[A_FILE] = FeatureFunctions::a_file;
	featureCalculators[A_FILE_B] = FeatureFunctions::a_file_b;
	featureCalculators[H_FILE] = FeatureFunctions::h_file;
	featureCalculators[H_FILE_B] = FeatureFunctions::h_file_b;
	featureCalculators[DISPERSION] = FeatureFunctions::dispersion;
	featureCalculators[THREATENED_DEFENDED] = FeatureFunctions::threatened_defended;
	featureCalculators[THREATENED_DEFENDED_B] = FeatureFunctions::threatened_defended_b;
	featureCalculators[THREATENED_UNDEFENDED] = FeatureFunctions::threatened_undefended;
	featureCalculators[THREATENED_UNDEFENDED_B] = FeatureFunctions::threatened_undefended_b;
	featureCalculators[THREATEN_DEFENDED] = FeatureFunctions::threaten_defended;
	featureCalculators[THREATEN_DEFENDED_B] = FeatureFunctions::threaten_defended_b;
	featureCalculators[THREATEN_UNDEFENDED] = FeatureFunctions::threaten_undefended;
	featureCalculators[THREATEN_UNDEFENDED_B] = FeatureFunctions::threaten_undefended_b;
	featureCalculators[FURTHEST_PIECE_DEFENDED] = FeatureFunctions::furthest_piece_defended;
	featureCalculators[FURTHEST_PIECE_UNDEFENDED] = FeatureFunctions::furthest_piece_undefended;
	featureCalculators[FURTHEST_PIECE_THREATENED] = FeatureFunctions::furthest_piece_threatened;
	featureCalculators[FURTHEST_PIECE_UNTHREATENED] = FeatureFunctions::furthest_piece_unthreatened;
	featureCalculators[CLOSEST_PIECE_DEFENDED] = FeatureFunctions::closest_piece_defended;
	featureCalculators[CLOSEST_PIECE_UNDEFENDED] = FeatureFunctions::closest_piece_undefended;
	featureCalculators[CLOSEST_PIECE_THREATENED] = FeatureFunctions::closest_piece_threatened;
	featureCalculators[CLOSEST_PIECE_UNTHREATENED] = FeatureFunctions::closest_piece_unthreatened;
	featureCalculators[PUSH_ADVANTAGE] = FeatureFunctions::push_advantage;
	featureCalculators[PUSH_ADVANTAGE_B] = FeatureFunctions::push_advantage_b;
	featureCalculators[UNTHREATENED_UNDEFENDED] = FeatureFunctions::unthreatened_undefended;
	featureCalculators[UNTHREATENED_UNDEFENDED_B] = FeatureFunctions::unthreatened_undefended_b;
	featureCalculators[UNTHREATEN_UNDEFENDED] = FeatureFunctions::unthreaten_undefended;
	featureCalculators[UNTHREATEN_UNDEFENDED_B] = FeatureFunctions::unthreaten_undefended_b;
	featureCalculators[THREATENED_SQUARES] = FeatureFunctions::threatened_squares;
}

string to_string(BoardFeature b) {
	switch (b)
	{
	case AI::ROW_2_THREATENED:
		return "ROW_2_THREATENED";
	case AI::ROW_2_THREATENED_B:
		return "ROW_2_THREATENED_B";
	case AI::ROW_3_THREATENED:
		return "ROW_3_THREATENED";
	case AI::ROW_3_THREATENED_B:
		return "ROW_3_THREATENED_B";
	case AI::ROW_4_THREATENED:
		return "ROW_4_THREATENED";
	case AI::ROW_4_THREATENED_B:
		return "ROW_4_THREATENED_B";
	case AI::MY_PAWN_COUNT:
		return "MY_PAWN_COUNT";
	case AI::THEIR_PAWN_COUNT:
		return "THEIR_PAWN_COUNT";
	case AI::PIECE_ADVANTAGE:
		return "PIECE_ADVANTAGE";
	case AI::PIECE_ADVANTAGE_B:
		return "PIECE_ADVANTAGE_B";
	case AI::A_FILE:
		return "A_FILE";
	case AI::A_FILE_B:
		return "A_FILE_B";
	case AI::H_FILE:
		return "H_FILE";
	case AI::H_FILE_B:
		return "H_FILE_B";
	case AI::DISPERSION:
		return "DISPERSION";
	case AI::THREATENED_DEFENDED:
		return "THREATENED_DEFENDED";
	case AI::THREATENED_DEFENDED_B:
		return "THREATENED_DEFENDED_B";
	case AI::THREATENED_UNDEFENDED:
		return "THREATENED_UNDEFENDED";
	case AI::THREATENED_UNDEFENDED_B:
		return "THREATENED_UNDEFENDED_B";
	case AI::THREATEN_DEFENDED:
		return "THREATEN_DEFENDED";
	case AI::THREATEN_DEFENDED_B:
		return "THREATEN_DEFENDED_B";
	case AI::THREATEN_UNDEFENDED:
		return "THREATEN_UNDEFENDED";
	case AI::THREATEN_UNDEFENDED_B:
		return "THREATEN_UNDEFENDED_B";
	case AI::FURTHEST_PIECE_DEFENDED:
		return "FURTHEST_PIECE_DEFENDED";
	case AI::FURTHEST_PIECE_UNDEFENDED:
		return "FURTHEST_PIECE_UNDEFENDED";
	case AI::FURTHEST_PIECE_THREATENED:
		return "FURTHEST_PIECE_THREATENED";
	case AI::FURTHEST_PIECE_UNTHREATENED:
		return "FURTHEST_PIECE_UNTHREATENED";
	case AI::CLOSEST_PIECE_DEFENDED:
		return "CLOSEST_PIECE_DEFENDED";
	case AI::CLOSEST_PIECE_UNDEFENDED:
		return "CLOSEST_PIECE_UNDEFENDED";
	case AI::CLOSEST_PIECE_THREATENED:
		return "CLOSEST_PIECE_THREATENED";
	case AI::CLOSEST_PIECE_UNTHREATENED:
		return "CLOSEST_PIECE_UNTHREATENED";
	case AI::PUSH_ADVANTAGE:
		return "PUSH_ADVANTAGE";
	case AI::PUSH_ADVANTAGE_B:
		return "PUSH_ADVANTAGE_B";
	case AI::UNTHREATENED_UNDEFENDED:
		return "UNTHREATENED_UNDEFENDED";
	case AI::UNTHREATENED_UNDEFENDED_B:
		return "UNTHREATENED_UNDEFENDED_B";
	case AI::UNTHREATEN_UNDEFENDED:
		return "UNTHREATEN_UNDEFENDED";
	case AI::UNTHREATEN_UNDEFENDED_B:
		return "UNTHREATEN_UNDEFENDED_B";
	case AI::THREATENED_SQUARES:
		return "THREATENED_SQUARES";
	case AI::NULL_FEATURE:
	default:
		throw "still so dumb";
	}
}

struct featureSet {
	mutable double similarity;
	std::pair<BoardFeature, BoardFeature> features() { return{ first,second }; }
	string to_string() {
		return "(" + ::to_string(first) + ", " + ::to_string(second) + ")";
	}
	featureSet(int b1, int b2) {
		first = (BoardFeature)std::min(b1, b2);
		second = (BoardFeature)std::max(b1, b2);
		if (b1 == b2) throw"You dumb nut get gud";
	}
	featureSet(BoardFeature b1, BoardFeature b2) {
		first = std::min(b1, b2);
		second = std::max(b1, b2);
		if (b1 == b2) throw"You dumb nut get gud";
	}
	bool operator<(const featureSet p) const {
		if (first == p.first) return second < p.second;
		else return first < p.first;
	}
private:
	BoardFeature first;
	BoardFeature second;
};

void mainY(BoardFeature * bp, int wantCount) {
	srand(time(NULL));
	setFeatureCalculators();

	//Gen board data
	std::set<Board> boards;
	Board currentBoard;
	while (boards.size() < 500000) {
		auto b = currentBoard.validNextBoards();

		if (b.empty()) currentBoard = Board();
		else currentBoard = b[rand() % b.size()];

		boards.insert(currentBoard);
	}

	//Use board data to make feature vectors
	std::vector<double> featureVectors[NULL_FEATURE];
	for (Board b : boards)
		for (int i = 0; i < NULL_FEATURE; i++) {
			featureVectors[i].push_back(featureCalculators[i](b));
		}

	//Normalize
	int count = boards.size();
	for (int i = 0; i < NULL_FEATURE; i++) {
		auto maxel = *std::max_element(featureVectors[i].begin(), featureVectors[i].end());
		for (int e = 0; e < count; e++) featureVectors[i][e] /= maxel;
	}

	//calculate similarities
	std::set<featureSet> similarities;
	for (int i = 0; i < NULL_FEATURE; i++)
		for (int j = 0; j < NULL_FEATURE; j++)
			if (i != j) {
				auto insertion = similarities.insert(featureSet(i, j));
				if (insertion.second) {
					double dotProd = 0, mag1 = 0, mag2 = 0;
					for (int k = 0; k < count; k++) {
						dotProd += featureVectors[i][k] * featureVectors[j][k];
						mag1 += featureVectors[i][k] * featureVectors[i][k];
						mag2 += featureVectors[j][k] * featureVectors[j][k];
					}
					insertion.first->similarity = dotProd / (sqrt(mag1) * sqrt(mag2));
				}
			}
	
	//Count total similarity
	double totalSimilarity[NULL_FEATURE] = { 0 };
	for (auto a : similarities) {
		totalSimilarity[a.features().first] += std::abs(a.similarity);
		totalSimilarity[a.features().second] += std::abs(a.similarity);
	}

	//std::sort(totalSimilarity, totalSimilarity + NULL_FEATURE);
	double minSimilarity = INFINITY;
	for (int i = 0; i < NULL_FEATURE; i++) if (totalSimilarity[i] < minSimilarity) {
		cout << "Feature: " << to_string((BoardFeature)i) << ": " << totalSimilarity[i] << endl;
		bp[0] = (BoardFeature)i;
		minSimilarity = totalSimilarity[i];
	}

	for (int keptGuys = 1; keptGuys < wantCount; keptGuys++) {
		double totalSimilarity[NULL_FEATURE] = { 0 };
		for (int guysToCount = 0; guysToCount < keptGuys; guysToCount++) {
			totalSimilarity[bp[guysToCount]] = INFINITY;
			for (auto a : similarities) {
				if (a.features().first == bp[guysToCount]) totalSimilarity[a.features().second] += std::abs(a.similarity);
				if (a.features().second == bp[guysToCount]) totalSimilarity[a.features().first] += std::abs(a.similarity);
			}
		}
		double minSimilarity = INFINITY;
		for (int i = 0; i < NULL_FEATURE; i++) if (totalSimilarity[i] < minSimilarity) {
			cout << "Feature: " << to_string((BoardFeature)i) << ": " << totalSimilarity[i] << endl;
			bp[keptGuys] = (BoardFeature)i;
			minSimilarity = totalSimilarity[i];
		}
		cout << endl;
	}

	for (int keptGuys = 0; keptGuys < wantCount; keptGuys++) cout << "Feature: " << to_string(bp[keptGuys]) << endl;

	
}

void main(){
	srand(time(NULL));
	bool alreadyMaxed[NULL_FEATURE][NULL_FEATURE] = { 0 };
	int maxedCount = 0;
	int bestScoreEvaa = INT_MIN;

	//So we generate our weebs first
	//And each weeb has their own feature bumped to start
	Specimen * weebs = new Specimen[NULL_FEATURE * NULL_FEATURE];
}

void mainX() {
	srand(time(NULL));
	bool alreadyMaxed[NULL_FEATURE][NULL_FEATURE] = { 0 };
	int maxedCount = 0;
	int bestScoreEvaa = INT_MIN;

	//So we generate our weebs first
	//And each weeb has their own feature bumped to start
	Specimen * weebs = new Specimen[NULL_FEATURE * NULL_FEATURE];
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
					if (rand() % 100 <= 50000.0 / (NULL_FEATURE*NULL_FEATURE - maxedCount))
						evaluate(weebs[i*NULL_FEATURE + j]);
					else weebs[i*NULL_FEATURE + j].second = INT_MIN;
				else weebs[i*NULL_FEATURE + j].second = INT_MIN;

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
				cout << "Repetition is good for the soul: " << bestScoreEvaa << endl;
	}

	delete[] weebs;
}