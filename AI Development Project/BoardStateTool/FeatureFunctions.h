#pragma once

namespace AI {
	class Board;
	typedef int(*FeatureFunc)(const Board);

	class FeatureFunctions {
	public:
		static int row_2_threatened				   (Board);
		static int row_2_threatened_b			   (Board);
		static int row_3_threatened				   (Board);
		static int row_3_threatened_b			   (Board);
		static int row_4_threatened				   (Board);
		static int row_4_threatened_b			   (Board);
		static int my_pawn_count				   (Board);
		static int their_pawn_count				   (Board);
		static int piece_advantage				   (Board);
		static int piece_advantage_b			   (Board);
		static int a_file						   (Board);
		static int a_file_b						   (Board);
		static int h_file						   (Board);
		static int h_file_b						   (Board);
		static int dispersion					   (Board);
		static int threatened_defended			   (Board);
		static int threatened_defended_b		   (Board);
		static int threatened_undefended		   (Board);
		static int threatened_undefended_b		   (Board);
		static int threaten_defended			   (Board);
		static int threaten_defended_b			   (Board);
		static int threaten_undefended			   (Board);
		static int threaten_undefended_b		   (Board);
		static int furthest_piece_defended		   (Board);
		static int furthest_piece_undefended	   (Board);
		static int furthest_piece_threatened	   (Board);
		static int furthest_piece_unthreatened	   (Board);
		static int closest_piece_defended		   (Board);
		static int closest_piece_undefended		   (Board);
		static int push_advantage				   (Board);
		static int push_advantage_b				   (Board);
		static int unthreatened_undefended		   (Board);
		static int unthreatened_undefended_b	   (Board);
		static int threatened_squares			   (Board);
	};
}
