// AILibrary.cpp : Defines the exported functions for the DLL application.
//

#include "Board.h"
#include "AI.h"
#include<ctime>

using namespace AI;
typedef unsigned long long bitboard;

extern "C"
{
	__declspec(dllexport) void BasicRandom(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		static int seed = 0; if (!seed) { seed = 1; srand(time(NULL)); }

		Board b(white, black, t);

		auto next = b.validWinBoards();
		if (next.empty()) next = b.validAttackBoards();
		if (next.empty()) next = b.validNextBoards();

		auto result = next[rand() % next.size()].lastMove;

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void BasicDefense(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		auto boards = b.validWinBoards();
		if (boards.empty()) boards = b.validAttackBoards();
		if (boards.empty())	boards = b.validNextBoards();

		auto result = !b.turn() ? boards.begin()->lastMove : boards.rbegin()->lastMove;

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void BasicOffense(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		auto boards = b.validWinBoards();
		if (boards.empty()) boards = b.validAttackBoards();
		if (boards.empty())	boards = b.validNextBoards();

		auto result = b.turn() ? boards.begin()->lastMove : boards.rbegin()->lastMove;

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void DarylsPet(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static Pet pet = Pet(0);

		auto result = pet(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void Test(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static TestPet pet = TestPet(0);

		auto result = pet(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void RandomPet(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static Pet pet = Pet();

		auto result = pet(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void MemPet(bitboard white, bitboard black, Turn t, int& from, int& to, int id = 0)
	{
		Board b(white, black, t);

		static MemoryPet pet = MemoryPet(id);

		auto result = pet(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void DarylsPrune(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static Prune prune = Prune(0);

		auto result = prune(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void Seeker(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static AI::Seeker seeker = AI::Seeker(0);

		auto result = seeker(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void L337(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static AI::Seeker seeker = AI::Seeker(0,20141337);

		auto result = seeker(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void MonteCarlo(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		auto result = MonteCarlo(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void MonteSeeker(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static AI::MonteSeeker seeker = AI::MonteSeeker(0);

		auto result = seeker(b);

		from = result.first;
		to = result.second;
	}

	__declspec(dllexport) void HyperSeeker(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		move result;
		Board b(white, black, t);

		static int moveRequests = 0;
		if (!(b < Board() || b < Board())) moveRequests = 0;

		static AI::Seeker seeker = AI::Seeker(0);
		static AI::MonteSeeker monteseeker = AI::MonteSeeker(0);

		if (moveRequests <= 12) result = seeker(b);
		else if (moveRequests <= 30) result = moveRequests % 2 == 0 ? seeker(b) : monteseeker(b);
		else if (moveRequests <= 40) result = moveRequests % 3 == 0 ? seeker(b) : monteseeker(b);
		else result = moveRequests % 2 == 0 ? seeker(b) : monteseeker(b);

		moveRequests++;

		from = result.first;
		to = result.second;
	}
	
	__declspec(dllexport) void Hinter(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static AI::Seeker seeker = AI::Seeker("Hint Please");

		auto result = seeker(b);

		from = result.first;
		to = result.second;
	}
}
