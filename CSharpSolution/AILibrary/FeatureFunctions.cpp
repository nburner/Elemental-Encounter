#include "stdafx.h"
#include "FeatureFunctions.h"
#include "Board.h"

using namespace AI;
using namespace BoardHelpers;
using namespace CommonBitboards;

int FeatureFunctions::row_2_threatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard myRow2 = b.turn == BLACK ? row2 : row7;
	Direction forward = b.turn == BLACK ? NORTH : SOUTH;

	return count(threatens(myBoard, forward) & myRow2);
}
int FeatureFunctions::row_2_threatened_b(const Board& b) { return row_2_threatened(b)/8; }

int FeatureFunctions::row_3_threatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard myRow3 = b.turn == BLACK ? row3 : row6;

	Direction forward = b.turn == BLACK ? NORTH : SOUTH;

	return count(threatens(myBoard, forward) & myRow3);
}
int FeatureFunctions::row_3_threatened_b(const Board& b) { return row_3_threatened(b)/8; }

int FeatureFunctions::row_4_threatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard myRow4 = b.turn == BLACK ? row4 : row5;

	Direction forward = b.turn == BLACK ? NORTH : SOUTH;

	return count(threatens(myBoard, forward) & myRow4);
}
int FeatureFunctions::row_4_threatened_b(const Board& b) { return row_4_threatened(b)/8; }

int FeatureFunctions::my_pawn_count(const Board& b) { return count(b.bb[!b.turn]); }

int FeatureFunctions::their_pawn_count(const Board& b) { return count(b.bb[b.turn]); }

int FeatureFunctions::piece_advantage(const Board& b) { return my_pawn_count(b) - their_pawn_count(b); }
int FeatureFunctions::piece_advantage_b(const Board& b) { return piece_advantage(b) > 0 ? 1 : piece_advantage(b) < 0 ? -1 : 0; }

int FeatureFunctions::a_file(const Board& b) { return count(b.bb[!b.turn] & colA); }
int FeatureFunctions::a_file_b(const Board& b) { return (bool)a_file(b); }

int FeatureFunctions::h_file(const Board& b) { return count(b.bb[!b.turn] & colH); }
int FeatureFunctions::h_file_b(const Board& b) { return (bool)h_file(b); }

int FeatureFunctions::dispersion(const Board& b) { 
	bitboard myBoard = b.bb[!b.turn];

	double expected = my_pawn_count(b) / 8.0;
	double result = 0;

	result += std::abs(count(myBoard & colA) - expected);
	result += std::abs(count(myBoard & colB) - expected);
	result += std::abs(count(myBoard & colC) - expected);
	result += std::abs(count(myBoard & colD) - expected);
	result += std::abs(count(myBoard & colE) - expected);
	result += std::abs(count(myBoard & colF) - expected);
	result += std::abs(count(myBoard & colG) - expected);
	result += std::abs(count(myBoard & colH) - expected);

	return (int)(result + 0.5);
}

int FeatureFunctions::threatened_defended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];
	
	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = Direction(myforward * -1);
	
	return count(
		threatens(myBoard, myforward) &
		threatens(theirBoard, theirforward) &
		myBoard
	);
}
int FeatureFunctions::threatened_defended_b(const Board& b) { return (bool)threatened_defended(b); }

int FeatureFunctions::threatened_undefended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = Direction(myforward * -1);

	return count(
		~threatens(myBoard, myforward) &
		threatens(theirBoard, theirforward) &
		myBoard
	);
}
int FeatureFunctions::threatened_undefended_b(const Board& b) { return (bool)threatened_undefended(b); }

int FeatureFunctions::threaten_defended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = Direction(myforward * -1);

	return count(
		threatens(myBoard, myforward) &
		threatens(theirBoard, theirforward) &
		theirBoard
	);
}
int FeatureFunctions::threaten_defended_b(const Board& b) { return (bool)threaten_defended(b); }

int FeatureFunctions::threaten_undefended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = Direction(myforward * -1);

	return count(
		threatens(myBoard, myforward) &
		~threatens(theirBoard, theirforward) &
		theirBoard
	);
}
int FeatureFunctions::threaten_undefended_b(const Board& b) { return (bool)threaten_undefended(b); }

int FeatureFunctions::furthest_piece_defended(const Board& b) { 
	bitboard myBoard = b.bb[!b.turn];
	bitboard mylastrow = b.turn == BLACK ? row8 : row1;

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = threatens(myBoard,myforward) & myBoard; i; i = shift(i, myforward)) {
		if (i & mylastrow) return r;
		--r;
	}

	return 0; 
}

int FeatureFunctions::furthest_piece_undefended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard mylastrow = b.turn == BLACK ? row8 : row1;

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = ~threatens(myBoard, myforward) & myBoard; i; i = shift(i, myforward)) {
		if (i & mylastrow) return r;
		--r;
	}

	return 0;
}
int FeatureFunctions::furthest_piece_threatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	bitboard mylastrow = b.turn == BLACK ? row8 : row1;

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = b.turn != BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = threatens(theirBoard, theirforward) & myBoard; i; i = shift(i, myforward)) {
		if (i & mylastrow) return r;
		--r;
	}

	return 0;
}
int FeatureFunctions::furthest_piece_unthreatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	bitboard mylastrow = b.turn == BLACK ? row8 : row1;

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = b.turn != BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = ~threatens(theirBoard, theirforward) & myBoard; i; i = shift(i, myforward)) {
		if (i & mylastrow) return r;
		--r;
	}

	return 0;
}

int FeatureFunctions::closest_piece_defended(const Board& b) {
	bitboard theirBoard = b.bb[b.turn];

	bitboard theirlastrow = b.turn != BLACK ? row8 : row1;

	Direction theirforward = b.turn != BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = threatens(theirBoard, theirforward) & theirBoard; i; i = shift(i, theirforward)) {
		if (i & theirlastrow) return r;
		--r;
	}

	return 0;
}

int FeatureFunctions::closest_piece_undefended(const Board& b) {
	bitboard theirBoard = b.bb[b.turn];

	bitboard theirlastrow = b.turn != BLACK ? row8 : row1;

	Direction theirforward = b.turn != BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = ~threatens(theirBoard, theirforward) & theirBoard; i; i = shift(i, theirforward)) {
		if (i & theirlastrow) return r;
		--r;
	}

	return 0;
}

int FeatureFunctions::closest_piece_threatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	bitboard theirlastrow = b.turn != BLACK ? row8 : row1;

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = b.turn != BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = threatens(myBoard, myforward) & theirBoard; i; i = shift(i, theirforward)) {
		if (i & theirlastrow) return r;
		--r;
	}

	return 0;
}

int FeatureFunctions::closest_piece_unthreatened(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	bitboard theirlastrow = b.turn != BLACK ? row8 : row1;

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = b.turn != BLACK ? NORTH : SOUTH;

	int r = 8;
	for (bitboard i = ~threatens(myBoard, myforward) & theirBoard; i; i = shift(i, theirforward)) {
		if (i & theirlastrow) return r;
		--r;
	}

	return 0;
}

int FeatureFunctions::push_advantage(const Board& b) { return furthest_piece_defended(b) - closest_piece_defended(b); }
int FeatureFunctions::push_advantage_b(const Board& b) { return (bool)push_advantage(b); }

int FeatureFunctions::unthreatened_undefended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = Direction(myforward * -1);

	return count(
		~threatens(myBoard, myforward) &
		~threatens(theirBoard, theirforward) &
		myBoard
	);
}
int FeatureFunctions::unthreatened_undefended_b(const Board& b) { return (bool)unthreatened_undefended(b); }

int AI::FeatureFunctions::unthreaten_undefended(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];
	bitboard theirBoard = b.bb[b.turn];

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;
	Direction theirforward = Direction(myforward * -1);

	return count(
		~threatens(myBoard, myforward) &
		~threatens(theirBoard, theirforward) &
		theirBoard
	);
}
int AI::FeatureFunctions::unthreaten_undefended_b(const Board& b) { return (bool)unthreatened_undefended(b); }

int FeatureFunctions::threatened_squares(const Board& b) {
	bitboard myBoard = b.bb[!b.turn];

	Direction myforward = b.turn == BLACK ? NORTH : SOUTH;

	return count(threatens(myBoard, myforward));
}