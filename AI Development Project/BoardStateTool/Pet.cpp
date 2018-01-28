#include "AI.h"
#include <random>

using namespace AI;

const int MAX_WEIGHT = 12;

std::default_random_engine generator;
//std::uniform_int_distribution<int> distribution(1, 6);
std::binomial_distribution<int> distribution(2 * MAX_WEIGHT, 0.5);
//int dice_roll = distribution(generator);  // generates number in the range 1..6 

FeatureFunc AI::AIEngine::Pet::featureCalculators[AI::BoardFeature::NULL_FEATURE] = { NULL };

AIEngine::Pet::Pet() {
	for (int bf1 = 0; bf1 < NULL_FEATURE; bf1++) {
		for (int bf2 = 0; bf2 < NULL_FEATURE; bf2++) {
			weights[bf1][bf2] = distribution(generator) - MAX_WEIGHT;
		}
	}

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
		featureCalculators[PUSH_ADVANTAGE] = FeatureFunctions::push_advantage;
		featureCalculators[PUSH_ADVANTAGE_B] = FeatureFunctions::push_advantage_b;
		featureCalculators[UNTHREATENED_UNDEFENDED] = FeatureFunctions::unthreatened_undefended;
		featureCalculators[UNTHREATENED_UNDEFENDED_B] = FeatureFunctions::unthreatened_undefended_b;
		featureCalculators[THREATENED_SQUARES] = FeatureFunctions::threatened_squares;
}

int AI::AIEngine::Pet::evaluate(const Board board) const
{
	return (*featureCalculators[0])(board);
}

move AI::AIEngine::Pet::operator()(const Board b) const
{
	return b.validNextBoards()[evaluate(b)].lastMove;
}

