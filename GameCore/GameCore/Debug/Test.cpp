#include "Game.h"
#include "..\..\..\AI Development Project\BoardStateTool\AI.h"
#include <iostream>
//using namespace std;
using std::cout; using std::endl;

void main()
{
	Game game;

	Human * player1 = new Human();
	Human * player2 = new Human();
	
	AI::AIEngine::AI * def = AI::AIEngine::start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * def2 = AI::AIEngine::start(AI::AIEngine::AIType::B_DEFENSE);
	AI::AIEngine::AI * off = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);
	AI::AIEngine::AI * off2 = AI::AIEngine::start(AI::AIEngine::AIType::B_OFFENSE);
	AI::AIEngine::AI * pet = AI::AIEngine::start(AI::AIEngine::AIType::DARYLS_PET);
		
	game.newGame(player1, pet);
	//((AI::AIEngine::Pet*)pet)->debug();


	if (game.getTurn() % 2 == 1)
	{
		cout << "White Wins!!" << endl;
	} 
	else
	{
		cout << "Black Wins!!!" << endl;
	}

	//LEAVE THIS COMMENTED CODE HERE FOR NOW
	//This code will let me (Daryl) test the hardware on the computer. I tested my own PC and it worked.
	//I need to ensure that the __popcnt instruction is supported by the processor.

	/*
	unsigned short us[3] = { 0, 0xFF, 0xFFFF };
	unsigned short usr;
	unsigned int   ui[4] = { 0, 0xFF, 0xFFFF, 0xFFFFFFFF };
	unsigned int   uir;

	for (int i = 0; i<3; i++) {
		usr = __popcnt16(us[i]);
		cout << "__popcnt16(0x" << std::hex << us[i] << ") = " << std::dec << usr << endl;
	}

	for (int i = 0; i<4; i++) {
		uir = __popcnt(ui[i]);
		cout << "__popcnt(0x" << std::hex << ui[i] << ") = " << std::dec << uir << endl;
	}
	*/
}

