#pragma once
#include "Board.h"
#include "FeatureFunctions.h"
#include "timer.h"
#include <algorithm>
#include <vector>
#include <set>

using std::vector;
using std::string;

namespace AI {
	
	enum BoardFeature {					//NOTE - THE BELOW COMMENTS MAY BE BIASED TOWARDS WHITE
		ROW_2_THREATENED,				//number of threatened squares in row 2 (0-8)
		ROW_2_THREATENED_B,				//1 if all of row 2 is threatened, 0 otherwise
		ROW_3_THREATENED,				//number of threatened squares in row 3 (0-8)
		ROW_3_THREATENED_B,				//1 if all of row 3 is threatened, 0 otherwise
		ROW_4_THREATENED,				//number of threatened squares in row 4 (0-8)
		ROW_4_THREATENED_B,				//1 if all of row 4 is threatened, 0 otherwise
		MY_PAWN_COUNT,					//number of my pawns
		THEIR_PAWN_COUNT,				//number of their pawns
		PIECE_ADVANTAGE,				//your pawn count minus their pawn count
		PIECE_ADVANTAGE_B,				//1 if you have more pawns, -1 if they have more pawns, 0 otherwise
		A_FILE,							//number of pawns in file A
		A_FILE_B,						//1 if you have pawns in file A, 0 otherwise
		H_FILE,							//number of pawns in file H
		H_FILE_B,						//1 if you have pawns in file H, 0 otherwise
		DISPERSION,						//the absolute value of the average number of pawns per column, minus 2
		THREATENED_DEFENDED,			//number of your pieces that are threatened but defended
		THREATENED_DEFENDED_B,			//1 if you have at least one pawn that is threatened but defended, 0 otherwise
		THREATENED_UNDEFENDED,			//number of your pieces that are threatened and not defended
		THREATENED_UNDEFENDED_B,		//1 if you have at least one pawn that is threatened and undefended, 0 otherwise
		THREATEN_DEFENDED,				//number of pieces you threaten that are defended
		THREATEN_DEFENDED_B,			//1 if you threaten a defended piece, 0 otherwise
		THREATEN_UNDEFENDED,			//number of pieces you threaten that aren't defended
		THREATEN_UNDEFENDED_B,			//1 if you threaten an undefended piece, 0 otherwise
		FURTHEST_PIECE_DEFENDED,		//row of your furthest forward defended pawn, minus 2
		FURTHEST_PIECE_UNDEFENDED,		//row of your furthest forward undefended pawn, minus 2
		FURTHEST_PIECE_THREATENED,		//row of your furthest forward threatened pawn, minus 2
		FURTHEST_PIECE_UNTHREATENED,	//row of your furthest forward not threatened pawn, minus 2
		CLOSEST_PIECE_DEFENDED,			//row of their closest defended pawn, minus 2
		CLOSEST_PIECE_UNDEFENDED,		//row of their closest undefended pawn, minus 2
		CLOSEST_PIECE_THREATENED,		//row of their closest threatened pawn, minus 2
		CLOSEST_PIECE_UNTHREATENED,		//row of their closest unthreatened pawn, minus 2
		PUSH_ADVANTAGE,					//difference of your number of **DEFENDED** rows forward and their number of rows close
		PUSH_ADVANTAGE_B,				//1 if your furthest **DEFENDED** piece is farther than their closest piece, 0 otherwise
		UNTHREATENED_UNDEFENDED,		//number of pieces you have that are neither defended nor threatened
		UNTHREATENED_UNDEFENDED_B,		//1 if you have a piece that is neither defended nor threatened
		UNTHREATEN_UNDEFENDED,			//number of pieces they have that are neither defended nor threatened
		UNTHREATEN_UNDEFENDED_B,		//1 if they have a piece that is neither defended nor threatened
		THREATENED_SQUARES,				//number of squares you threaten
		//ABOUT_TO_LOSE,				//1 if enemy canmove a piece forward to win, 0 otherwise 

		NULL_FEATURE					//Gives me the count and a place to stop for-loops
	};


	class Pet {
	protected:
		void setFeatureCalculators();

		const int PLY_COUNT = 2;
		__int8 weights[NULL_FEATURE][NULL_FEATURE];
		static FeatureFunc featureCalculators[NULL_FEATURE];

		mutable std::set<Board> seenBoards;

		void evaluate(Board&) const;
		int minmax(Board, int) const;

	public:
		Pet();
		Pet(int);
		virtual move operator()(const Board b) const;
		void debug();
	};

	class Prune : public Pet {
	protected:
		static int PLY_COUNT;
		int alphabeta(Board board, int currentPly, double duration = 1.5, const timer& t = timer()) const;
		
	public:
		Prune();
		Prune(int);
		virtual move operator()(const Board b) const;
	};

	class MemoryPet : public Pet {
	public:
		MemoryPet(int);
		void save(int);
		void tweak();
		static MemoryPet * mix(MemoryPet*, MemoryPet*);
	};

	class TestPet {
	protected:
		void setFeatureCalculators();

		const int PLY_COUNT = 2;
		__int8 weights[NULL_FEATURE][NULL_FEATURE];
		static FeatureFunc featureCalculators[NULL_FEATURE];

		void evaluate(Board&) const;
		int minmax(Board, int) const;

	public:
		TestPet();
		TestPet(int);
		virtual move operator()(const Board b) const;
		void debug();
	};

	class Seeker : public Prune {
	protected:
		//int alphabeta(Board board, int currentPly) const;
		int MAX_SEEK_DEPTH;
		double FINAL_ALPHA_BETA_TIME;
		double SEEK_WIN;
		double SEEK_OPPONENT_WIN_SHORT;
		double SEEK_OPPONENT_WIN_LONG;
		double SEEK_BLOCKING_MOVE;
		mutable timer t;
		move deepTimedSeek(const Board & b, double time, int depth) const;

	public:
		Seeker();
		Seeker(int);
		Seeker(int,int);
		Seeker(std::string hinter);
		virtual move operator()(const Board b) const;

		void readWeights(int);
		void writeWeights(int);
		bool adjustWeight(BoardFeature, BoardFeature, int);
	};
}
