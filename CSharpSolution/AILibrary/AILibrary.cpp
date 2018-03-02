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
	
	__declspec(dllexport) void Hinter(bitboard white, bitboard black, Turn t, int& from, int& to)
	{
		Board b(white, black, t);

		static AI::Seeker seeker = AI::Seeker("Hint Please");

		auto result = seeker(b);

		from = result.first;
		to = result.second;
	}
}
