#pragma once

namespace AI {
	class Board;
	typedef int(*FeatureFunc)(const Board&);

	class FeatureFunctions {
	public:
		static int row_2_threatened(const Board&);
		static int row_2_threatened_b(const Board&);
		static int row_3_threatened(const Board&);
		static int row_3_threatened_b(const Board&);
		static int row_4_threatened(const Board&);
		static int row_4_threatened_b(const Board&);
		static int my_pawn_count(const Board&);
		static int their_pawn_count(const Board&);
		static int piece_advantage(const Board&);
		static int piece_advantage_b(const Board&);
		static int a_file(const Board&);
		static int a_file_b(const Board&);
		static int h_file(const Board&);
		static int h_file_b(const Board&);
		static int dispersion(const Board&);
		static int threatened_defended(const Board&);
		static int threatened_defended_b(const Board&);
		static int threatened_undefended(const Board&);
		static int threatened_undefended_b(const Board&);
		static int threaten_defended(const Board&);
		static int threaten_defended_b(const Board&);
		static int threaten_undefended(const Board&);
		static int threaten_undefended_b(const Board&);
		static int furthest_piece_defended(const Board&);
		static int furthest_piece_undefended(const Board&);
		static int furthest_piece_threatened(const Board&);
		static int furthest_piece_unthreatened(const Board&);
		static int closest_piece_defended(const Board&);
		static int closest_piece_undefended(const Board&);
		static int closest_piece_threatened(const Board&);
		static int closest_piece_unthreatened(const Board&);
		static int push_advantage(const Board&);
		static int push_advantage_b(const Board&);
		static int unthreatened_undefended(const Board&);
		static int unthreatened_undefended_b(const Board&);
		static int unthreaten_undefended(const Board&);
		static int unthreaten_undefended_b(const Board&);
		static int threatened_squares(const Board&);
	};
}
