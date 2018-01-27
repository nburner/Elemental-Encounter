#include "AI.h"
#include <random>

using namespace AI;

const int MAX_WEIGHT = 12;

const FeatureFunc row_2_threatened;
const FeatureFunc row_2_threatened_b;
const FeatureFunc row_3_threatened;
const FeatureFunc row_3_threatened_b;
const FeatureFunc row_4_threatened;
const FeatureFunc row_4_threatened_b;
const FeatureFunc my_pawn_count;
const FeatureFunc their_pawn_count;
const FeatureFunc piece_advantage;
const FeatureFunc piece_advantage_b;
const FeatureFunc a_file;
const FeatureFunc a_file_b;
const FeatureFunc h_file;
const FeatureFunc h_file_b;
const FeatureFunc dispersion;
const FeatureFunc threatened_defended;
const FeatureFunc threatened_defended_b;
const FeatureFunc threatened_undefended;
const FeatureFunc threatened_undefended_b;
const FeatureFunc threaten_defended;
const FeatureFunc threaten_defended_b;
const FeatureFunc threaten_undefended;
const FeatureFunc threaten_undefended_b;
const FeatureFunc furthest_piece_defended;
const FeatureFunc furthest_piece_undefended;
const FeatureFunc furthest_piece_threatened;
const FeatureFunc furthest_piece_unthreatened;
const FeatureFunc closest_piece_defended;
const FeatureFunc closest_piece_undefended;
const FeatureFunc push_advantage;
const FeatureFunc push_advantage_b;
const FeatureFunc unthreatened_undefended;
const FeatureFunc unthreatened_undefended_b;
const FeatureFunc threatened_squares;

std::default_random_engine generator;
//std::uniform_int_distribution<int> distribution(1, 6);
std::binomial_distribution<signed char> distribution(2 * MAX_WEIGHT, 0.5);
//int dice_roll = distribution(generator);  // generates number in the range 1..6 

FeatureFunc AI::AIEngine::Pet::featureCalculators[AI::BoardFeature::NULL_FEATURE] = { NULL };

AIEngine::Pet::Pet() {
	for (int bf1 = 0; bf1 < NULL_FEATURE; bf1++) {
		for (int bf2 = 0; bf2 < NULL_FEATURE; bf2++) {
			weights[bf1][bf2] = distribution(generator) - MAX_WEIGHT;
		}
	}
}

int AI::AIEngine::Pet::evaluate(Board board)
{
	return 0;
}

move AI::AIEngine::Pet::operator()(const Board b) const
{
	return move();
}

