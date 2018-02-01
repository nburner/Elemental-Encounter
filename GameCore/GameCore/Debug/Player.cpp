#include "Player.h"
#include <string>
using std::cin; using std::cout; using std::endl;


Player::Player()
{
}


Player::~Player()
{
}

//This is meant to replace checkMove
//However it may not be identical to the original as I'm not sure exactly what the original was doing
//I'm going off of the comments and printed messages, and adding things that may have been missed
bool Human::isValidMove(const move move) const
{
	Square from = move.first;
	Square to = move.second;
	GameBoard * board = GameBoard::getInstance();

	char myPiece = myColor == WHITE ? 'W' : 'B';
	Direction myForward = myColor == WHITE ? NORTH : SOUTH;
	Direction myLeft = Direction(myForward + WEST);
	Direction myRight = Direction(myForward + EAST);
	
	// check to see if selected piece is your own
	if ((*board)[from] != myPiece) {
		cout << "That is not your piece. Please make a valid move selection." << endl;
		return false;
	}

	//check to see if there is a piece in front of the player
	if (from + myForward == to && ((*board)[to] == 'W' || (*board)[to] == 'B')){
		cout << "There is a piece in that space, you cannot move there. Please make a valid move selection" << endl;
		return false;
	}

	//check to see if there is the player's piece diagonal of the player
	if ((from + myLeft == to || from + myRight == to) && (*board)[to] == myPiece) {
		cout << "There is a piece in that space, you cannot move there. Please make a valid move selection" << endl;
		return false;
	}

	//check to see not moving
	if (to == from) {
		cout << "You must move. Please make a valid move selection." << endl;
		return false;
	}

	//check to see if the piece is moving forward and only moving forward one space
	if (to - from % 8 == 0 && (to - from) * myForward > 0)	{
		cout << "You may only move forward one space. Please make a valid move selection." << endl;
		return false;
	}

	//check to see if moving backwards
	if ((to - from) * myForward < 0)	{
		cout << "You may not move backwards. Please make a valid move selection." << endl;
		return false;
	}

	//check to see if moving sideways
	if (to/8 == from/8)	{
		cout << "You may not move sideways. Please make a valid move selection." << endl;
		return false;
	}

	//check to see wrapping
	if ((from + myRight == to && to % 8 == 0) || (from + myLeft == to && to % 8 == 7)) {
		cout << "You may not wrap around the board. Please make a valid move selection." << endl;
		return false;
	}

	//catch all?
	if (from + myForward != to && from + myLeft != to && from + myRight != to) {
		cout << "That is an invalid move. Please make a valid move selection" << endl;
		return false;
	}

	return true;
}

move Human::getMove(GameBoard * gp)
{
	move result = move();
	string moveFrom, moveTo;

	do {
		do {
			cout << "Move From: ";
			cin >> moveFrom;
			if (moveFrom == "exit")	throw 0;
		} while (!((moveFrom[0] >= 'A' && moveFrom[0] <= 'H') || (moveFrom[0] >= 'a' && moveFrom[0] <= 'h')) || moveFrom[1] > '8' || moveFrom[1] < '1');
		do {
			cout << "Move To: ";
			cin >> moveTo;
			if (moveTo == "exit") throw 0;
		} while (!((moveTo[0] >= 'A' && moveTo[0] <= 'H') || (moveTo[0] >= 'a' && moveTo[0] <= 'h')) || moveTo[1] > '8' || moveTo[1] < '1');

		result.first = Square((toupper(moveFrom[0]) - 'A')  + (moveFrom[1] - '1') * 8);
		result.second = Square((toupper(moveTo[0]) - 'A')  + (moveTo[1] - '1') * 8);

	} while (!isValidMove(result));

	return result;
}
