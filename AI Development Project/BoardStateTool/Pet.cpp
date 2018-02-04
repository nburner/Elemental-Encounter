#include "AI.h"
#include "Board.h"
#include <random>
#include <set>

using namespace AI;
using std::set;

const int MAX_WEIGHT = 12;

std::default_random_engine generator;
std::binomial_distribution<int> distribution(2 * MAX_WEIGHT, 0.5);

FeatureFunc AIEngine::Pet::featureCalculators[AI::BoardFeature::NULL_FEATURE] = { NULL };

void AIEngine::Pet::setFeatureCalculators() {
	AIEngine::Pet::featureCalculators[ROW_2_THREATENED] = FeatureFunctions::row_2_threatened;
	AIEngine::Pet::featureCalculators[ROW_2_THREATENED_B] = FeatureFunctions::row_2_threatened_b;
	AIEngine::Pet::featureCalculators[ROW_3_THREATENED] = FeatureFunctions::row_3_threatened;
	AIEngine::Pet::featureCalculators[ROW_3_THREATENED_B] = FeatureFunctions::row_3_threatened_b;
	AIEngine::Pet::featureCalculators[ROW_4_THREATENED] = FeatureFunctions::row_4_threatened;
	AIEngine::Pet::featureCalculators[ROW_4_THREATENED_B] = FeatureFunctions::row_4_threatened_b;
	AIEngine::Pet::featureCalculators[MY_PAWN_COUNT] = FeatureFunctions::my_pawn_count;
	AIEngine::Pet::featureCalculators[THEIR_PAWN_COUNT] = FeatureFunctions::their_pawn_count;
	AIEngine::Pet::featureCalculators[PIECE_ADVANTAGE] = FeatureFunctions::piece_advantage;
	AIEngine::Pet::featureCalculators[PIECE_ADVANTAGE_B] = FeatureFunctions::piece_advantage_b;
	AIEngine::Pet::featureCalculators[A_FILE] = FeatureFunctions::a_file;
	AIEngine::Pet::featureCalculators[A_FILE_B] = FeatureFunctions::a_file_b;
	AIEngine::Pet::featureCalculators[H_FILE] = FeatureFunctions::h_file;
	AIEngine::Pet::featureCalculators[H_FILE_B] = FeatureFunctions::h_file_b;
	AIEngine::Pet::featureCalculators[DISPERSION] = FeatureFunctions::dispersion;
	AIEngine::Pet::featureCalculators[THREATENED_DEFENDED] = FeatureFunctions::threatened_defended;
	AIEngine::Pet::featureCalculators[THREATENED_DEFENDED_B] = FeatureFunctions::threatened_defended_b;
	AIEngine::Pet::featureCalculators[THREATENED_UNDEFENDED] = FeatureFunctions::threatened_undefended;
	AIEngine::Pet::featureCalculators[THREATENED_UNDEFENDED_B] = FeatureFunctions::threatened_undefended_b;
	AIEngine::Pet::featureCalculators[THREATEN_DEFENDED] = FeatureFunctions::threaten_defended;
	AIEngine::Pet::featureCalculators[THREATEN_DEFENDED_B] = FeatureFunctions::threaten_defended_b;
	AIEngine::Pet::featureCalculators[THREATEN_UNDEFENDED] = FeatureFunctions::threaten_undefended;
	AIEngine::Pet::featureCalculators[THREATEN_UNDEFENDED_B] = FeatureFunctions::threaten_undefended_b;
	AIEngine::Pet::featureCalculators[FURTHEST_PIECE_DEFENDED] = FeatureFunctions::furthest_piece_defended;
	AIEngine::Pet::featureCalculators[FURTHEST_PIECE_UNDEFENDED] = FeatureFunctions::furthest_piece_undefended;
	AIEngine::Pet::featureCalculators[FURTHEST_PIECE_THREATENED] = FeatureFunctions::furthest_piece_threatened;
	AIEngine::Pet::featureCalculators[FURTHEST_PIECE_UNTHREATENED] = FeatureFunctions::furthest_piece_unthreatened;
	AIEngine::Pet::featureCalculators[CLOSEST_PIECE_DEFENDED] = FeatureFunctions::closest_piece_defended;
	AIEngine::Pet::featureCalculators[CLOSEST_PIECE_UNDEFENDED] = FeatureFunctions::closest_piece_undefended;
	AIEngine::Pet::featureCalculators[CLOSEST_PIECE_THREATENED] = FeatureFunctions::closest_piece_threatened;
	AIEngine::Pet::featureCalculators[CLOSEST_PIECE_UNTHREATENED] = FeatureFunctions::closest_piece_unthreatened;
	AIEngine::Pet::featureCalculators[PUSH_ADVANTAGE] = FeatureFunctions::push_advantage;
	AIEngine::Pet::featureCalculators[PUSH_ADVANTAGE_B] = FeatureFunctions::push_advantage_b;
	AIEngine::Pet::featureCalculators[UNTHREATENED_UNDEFENDED] = FeatureFunctions::unthreatened_undefended;
	AIEngine::Pet::featureCalculators[UNTHREATENED_UNDEFENDED_B] = FeatureFunctions::unthreatened_undefended_b;
	AIEngine::Pet::featureCalculators[UNTHREATEN_UNDEFENDED] = FeatureFunctions::unthreaten_undefended;
	AIEngine::Pet::featureCalculators[UNTHREATEN_UNDEFENDED_B] = FeatureFunctions::unthreaten_undefended_b;
	AIEngine::Pet::featureCalculators[THREATENED_SQUARES] = FeatureFunctions::threatened_squares;
}

AIEngine::Pet::Pet() {
	for (int bf1 = 0; bf1 < NULL_FEATURE; bf1++) {
		for (int bf2 = 0; bf2 < NULL_FEATURE; bf2++) {
			weights[bf1][bf2] = distribution(generator) - MAX_WEIGHT;
		}
	}

	setFeatureCalculators();
}

AI::AIEngine::Pet::Pet(int)
{
	for (int bf1 = 0; bf1 < NULL_FEATURE; bf1++) {
		for (int bf2 = 0; bf2 < NULL_FEATURE; bf2++) {
			weights[bf1][bf2] = 0;
		}
	}

	weights[ROW_2_THREATENED][ROW_2_THREATENED] = 0;
	weights[ROW_2_THREATENED_B][ROW_2_THREATENED_B] = 80;
	weights[ROW_3_THREATENED][ROW_3_THREATENED] = 0;
	weights[ROW_3_THREATENED_B][ROW_3_THREATENED_B] = 40;
	weights[ROW_4_THREATENED][ROW_4_THREATENED] = 10;
	weights[ROW_4_THREATENED_B][ROW_4_THREATENED_B] = 40;
	weights[MY_PAWN_COUNT][MY_PAWN_COUNT] = 25;
	weights[THEIR_PAWN_COUNT][THEIR_PAWN_COUNT] = -25;
	weights[PIECE_ADVANTAGE][PIECE_ADVANTAGE] = 70;
	weights[PIECE_ADVANTAGE_B][PIECE_ADVANTAGE_B] = 100;
	weights[A_FILE][A_FILE] = -10;
	weights[A_FILE_B][A_FILE_B] = 0;
	weights[H_FILE][H_FILE] = -10;
	weights[H_FILE_B][H_FILE_B] = 0;
	weights[DISPERSION][DISPERSION] = -1;
	weights[THREATENED_DEFENDED][THREATENED_DEFENDED] = 0;
	weights[THREATENED_DEFENDED_B][THREATENED_DEFENDED_B] = 0;
	weights[THREATENED_UNDEFENDED][THREATENED_UNDEFENDED] = -100;
	weights[THREATENED_UNDEFENDED_B][THREATENED_UNDEFENDED_B] = -50;
	weights[THREATEN_DEFENDED][THREATEN_DEFENDED] = 0;
	weights[THREATEN_DEFENDED_B][THREATEN_DEFENDED_B] = 0;
	weights[THREATEN_UNDEFENDED][THREATEN_UNDEFENDED] = -20;
	weights[THREATEN_UNDEFENDED_B][THREATEN_UNDEFENDED_B] = -50;
	weights[FURTHEST_PIECE_DEFENDED][FURTHEST_PIECE_DEFENDED] = 20;
	weights[FURTHEST_PIECE_UNDEFENDED][FURTHEST_PIECE_UNDEFENDED] = 0;
	weights[FURTHEST_PIECE_THREATENED][FURTHEST_PIECE_THREATENED] = 0;
	weights[FURTHEST_PIECE_UNTHREATENED][FURTHEST_PIECE_UNTHREATENED] = 35;
	weights[FURTHEST_PIECE_UNTHREATENED][FURTHEST_PIECE_UNDEFENDED] = 35;
	weights[CLOSEST_PIECE_DEFENDED][CLOSEST_PIECE_DEFENDED] = -50;
	weights[CLOSEST_PIECE_UNDEFENDED][CLOSEST_PIECE_UNDEFENDED] = -80;
	weights[CLOSEST_PIECE_UNTHREATENED][CLOSEST_PIECE_UNDEFENDED] = -127;
	weights[CLOSEST_PIECE_THREATENED][CLOSEST_PIECE_UNDEFENDED] = -127;
	weights[CLOSEST_PIECE_THREATENED][CLOSEST_PIECE_THREATENED] = -80;
	weights[CLOSEST_PIECE_UNTHREATENED][CLOSEST_PIECE_UNTHREATENED] = -60;
	weights[PUSH_ADVANTAGE][PUSH_ADVANTAGE] = 0;
	weights[PUSH_ADVANTAGE_B][PUSH_ADVANTAGE_B] = 5;
	weights[UNTHREATENED_UNDEFENDED][UNTHREATENED_UNDEFENDED] = 0;
	weights[UNTHREATENED_UNDEFENDED_B][UNTHREATENED_UNDEFENDED_B] = 0;
	weights[UNTHREATEN_UNDEFENDED][UNTHREATEN_UNDEFENDED] = 0;
	weights[UNTHREATEN_UNDEFENDED_B][UNTHREATEN_UNDEFENDED_B] = 0;
	weights[THREATENED_SQUARES][THREATENED_SQUARES] = 10;


	setFeatureCalculators();
}

void AI::AIEngine::Pet::evaluate(Board& board) const
{
	static set<Board> seenBoards = set<Board>();
	auto insertionResult = seenBoards.insert(board);

	if (insertionResult.second) {
		if (board.gameOver()) {
			insertionResult.first->val = board.gameOver();
		}
		else {
			int result = 0;

			for (int i = 0; i < NULL_FEATURE; i++) {
				for (int j = 0; j < NULL_FEATURE; j++) if (weights[i][j]) {
					if (i < j) result += weights[i][j] * ((*featureCalculators[i])(board) + (*featureCalculators[j])(board) - 1);
					if (i > j) result += weights[i][j] * ((*featureCalculators[i])(board) - (*featureCalculators[j])(board));
					else result += weights[i][j] * (*featureCalculators[i])(board);
				}
			}

			insertionResult.first->val = result;
		}
	}

	board.val = insertionResult.first->val;
}

int AI::AIEngine::Pet::minmax(Board board, int currentPly) const {
	auto boards = board.validNextBoards();
	int bestVal = INT_MIN;

	if (boards.empty() || currentPly == this->PLY_COUNT) { 
		evaluate(board); 
		return board.val;
	}

	for (int i = 0; i < boards.size(); i++) {
		boards[i].val = -1 * minmax(boards[i], currentPly + 1);
		if (boards[i].val > bestVal) {
			bestVal = boards[i].val;
		}
	}
	
	return bestVal;
}

move AI::AIEngine::Pet::operator()(const Board b) const
{
	auto boards = b.validNextBoards();
	
	move result; int bestVal = INT_MIN;

	for (int i = 0; i < boards.size(); i++) {
		evaluate(boards[i]);
		boards[i].val /= 2;
		boards[i].val += minmax(boards[i], 0)/2;
		if (boards[i].val > bestVal) {
			bestVal = boards[i].val;
			result = boards[i].lastMove;
		}
	}

	/*std::sort(boards.begin(), boards.end(), [](Board& a, Board& b) {
		return a.val > b.val;
	});*/

	cout << "AI's Move: " << BoardHelpers::to_string(result.first) << " - " << BoardHelpers::to_string(result.second) << endl;
	cout << " Best Val: " << bestVal << endl;
	return result;
}

void AI::AIEngine::Pet::debug()
{
	cout << Board() << endl;
	auto b = Board().validNextBoards();
	do {
		//cout << b[0] << evaluate(b[0]) << endl << endl;
		b = b[0].validNextBoards();
	} while (!b.empty());

	//cout << INT_MIN << " TO " << INT_MAX << endl;
}

