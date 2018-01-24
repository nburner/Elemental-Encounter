#include"AI.h"
#include<iostream>
#include<set>
#include<queue>
#include<time.h>
#include<intrin.h>   
using namespace AI;
using std::cout; using std::endl; using std::set; using std::queue;

inline int random(int count) {
	return rand() % count;
}

int main() {
	srand(time(NULL));

	/*
	unsigned short us[3] = { 0, 0xFF, 0xFFFF };
	unsigned short usr;
	unsigned int   ui[4] = { 0, 0xFF, 0xFFFF, 0xFFFFFFFF };
	unsigned int   uir;
	unsigned long long  ul[8] = { 0, 0xFF, 0xFFFF, 0xFFFFFFFF, 0xFFFFFFFFFF,0xFFFFFFFFFFFF,0xFFFFFFFFFFFFFF,0xFFFFFFFFFFFFFFFF };
	unsigned long long  ulr;

	for (int i = 0; i<3; i++) {
		usr = __lzcnt16(us[i]);
		cout << "__lzcnt16(0x" << hex << us[i] << ") = " << dec << usr << endl;
	}

	for (int i = 0; i < 4; i++) {
		uir = __lzcnt(ui[i]);
		cout << "__lzcnt(0x" << hex << ui[i] << ") = " << dec << uir << endl;
	}

	for (int i = 0; i < 8; i++) {
		ulr = __lzcnt64(ul[i]);
		cout << "__lzcnt64(0x" << hex << ul[i] << ") = " << dec << ulr << endl;
	}
	*/

	/*
	Board myBoard;
	queue<Board> validBoards;
	validBoards.push(myBoard);

	set<Board> usefulBoards;

	int turn = 0;
	int statesTillTurnIncreases[181] = { 0 };
	while (!validBoards.empty() && turn < 3) {
		auto myset = validBoards.front().validNextBoards();
		
		for (auto it = myset.begin(); it != myset.end(); ++it) if (usefulBoards.insert(*it).second) {
			//cout << *it << endl;
			validBoards.push(*it);
			++statesTillTurnIncreases[turn + 1];
		}

		if (statesTillTurnIncreases[turn]-- == 0) {
			++turn;
			cout << "****** TURN " << turn + 1 << " *******" << endl;
			cout << "Total number of Board States: " << usefulBoards.size() << endl;
		}
		validBoards.pop();		
	}

	//cout << "Total number of Board States: " << usefulBoards.size();
	*/

	/*Board myBoard;
	for (int i = 0; i < 161; i++) {
		cout << "Move: " << i << endl << myBoard << endl;

		auto attacks = myBoard.validAttackBoards();
		if (!attacks.empty()) myBoard = attacks[random(attacks.size())];
		else {
			auto moves = myBoard.validNextBoards();
			if (moves.empty()) break;
			else myBoard = moves[random(moves.size())];
		}		
	}*/

	auto ai = AIEngine::start(AIEngine::AIType::PET);

	//system("pause");
}
