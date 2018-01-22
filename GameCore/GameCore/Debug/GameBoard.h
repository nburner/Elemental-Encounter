#pragma once
#include <iostream>
using namespace std;
 
class GameBoard
{
public:
	~GameBoard(); 
	static GameBoard* getInstance();
	char space[8][8];
	bool hasOwnPiece(string area);

private:
	GameBoard();
	static GameBoard* instance;
};

 

