#include "Player.h"
#include <string>
using std::cin; using std::cout;


Player::Player()
{
}


Player::~Player()
{
}

move Human::getMove(GameBoard * gp)
{
	move result = move();
	string moveFrom, moveTo;

	do {
		cout << "Move From: ";
		cin >> moveFrom;
	} while (!((moveFrom[0] >= 'A' && moveFrom[0] <= 'H') || (moveFrom[0] >= 'a' && moveFrom[0] <= 'h')) || moveFrom[1] > '8' || moveFrom[1] < '1');
	do {
		cout << "Move To: ";
		cin >> moveTo;		
	} while (!((moveTo[0] >= 'A' && moveTo[0] <= 'H') || (moveTo[0] >= 'a' && moveTo[0] <= 'h')) || moveTo[1] > '8' || moveTo[1] < '1');

	result.first = Square((toupper(moveFrom[0]) - 'A') * 8 + moveFrom[1] - '1');
	result.second = Square((toupper(moveTo[0]) - 'A') * 8 + moveTo[1] - '1');

	if (moveFrom == "exit" || moveTo == "exit")
		throw 0;

	//checkMove(moveFrom, moveTo);

	//updateBoard(moveFrom, moveTo, player);

	return result;
}
