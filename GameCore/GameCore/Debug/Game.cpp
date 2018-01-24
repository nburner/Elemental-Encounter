#include "Game.h"
using std::cin; using std::cout; using std::endl;


Game::Game()
{
	gameBoard = GameBoard::getInstance(); 
}


Game::~Game(){
	delete gameBoard;
	gameBoard = NULL;
}

GameBoard& Game::getGameBoard() const
{
	return *gameBoard;
}


bool Game::checkForWin()
{
	bool Victory = false;

	if (pTurn % 2 == 1) 
	{
		Victory = checkLastRow('W');
		//Victory = checkForPieces('B');
	} 
	else
	{
		Victory = checkLastRow('B');
		//Victory = checkForPieces('W');
	}

	return Victory;
}

void Game::newGame(Player * white, Player * black) 
{
	players[WHITE] = white;
	players[BLACK] = black;

	white->myColor = WHITE;
	black->myColor = BLACK;

	printBoard();
	startGameLoop();

	/*bool flip = 0;
	for (int r = 0; r < 8; r++)
	{
		for (int c = 0; c < 8; c++)
		{
			if (r == 0 || r == 1)
			{
				GameBoard::getInstance()->space[r][c] = 'B';
			} 
			else if (r == 6 || r == 7)
			{
				GameBoard::getInstance()->space[r][c] = 'W';
			}
			else 
			{
				if (flip == 0)
				{
					GameBoard::getInstance()->space[r][c] = ' ';
					flip = 1;
				}
				else
				{
					GameBoard::getInstance()->space[r][c] = (char)bar;
					flip = 0;
				}
			}
		}

		flip = !flip;
	}*/
}

//Obsolete
bool Game::getMove(int player)
{
	//cout << "Player " << player <<"'s move" << endl;
	//cout << "Move From: ";
	//cin >> moveFrom;
	//cout << "Move To: ";
	//cin >> moveTo;

	//if (moveFrom == "exit" || moveTo == "exit")
	//	return false;

	////checkMove(moveFrom, moveTo);

	//updateBoard(moveFrom, moveTo, player);

	//return true;

	return false;
}

int Game::getTurn()
{
	return pTurn;
}

Turn Game::getColorTurn()
{
	return Turn(pTurn % 2);
}

void Game::nextTurn()
{
	pTurn++;
}

//Obsolete - see Human::validateMove
void Game::checkMove(move move)
{
	//// check to see if moving backwards
	//// check to see if moving sideways
	//if (((tolower(from[0]) >= 97) && (tolower(from[0]) <= 104)) && ((from[1] >= 1) && (from[1] <= 8)))
	//{
	//	if ((tolower(to[0]) >= 97 && tolower(to[0]) <= 104) && (to[1] >= 1 && to[1] <= 8))
	//	{
	//		int fromR = getNumber(from[0]);
	//		int fromC = getNumber(from[1]);

	//		int toR = getNumber(to[0]);
	//		int toC = getNumber(to[1]);

	//		if (getTurn() % 2 == 1)
	//		{
	//			// check to see if piece is Player One's
	//			if (getGameBoard()->space[fromC][fromR] == 'W')
	//			{
	//				//check to see if there is a piece in front of the player
	//				if (toR == fromR && (toC + 1) == fromC && (getGameBoard()->space[toC][toR] != ' ' && getGameBoard()->space[toC][toR] != '219'))
	//				{
	//					cout << "There is a piece in that space, you cannot move there. Please make a valid move selection" << endl;
	//					getMove(getTurn());
	//				}
	//				//check to see if the piece is moving forward and only moving forward one space
	//				else if (((toC + 1) != fromC) || (toC + 1) == fromC && ((toR != (fromR + 1)) && (toR != fromR) && (toR != (fromR - 1))))
	//				{
	//					cout << "You may only move forward one space. Please make a valid move selection." << endl;
	//					getMove(getTurn());
	//				}
	//				else
	//				{
	//					return;
	//				}
	//			}
	//			else
	//			{
	//				cout << "That is not your piece. Please make a valid selection" << endl;
	//				getMove(getTurn());
	//			}


	//		}
	//		else
	//		{
	//			// check to see if piece is Player Two's
	//			if (getGameBoard()->space[fromC][fromR] == 'B')
	//			{
	//				//check to see if there is a piece in front of the player
	//				if (toR == fromR && (toC - 1) == fromC && (getGameBoard()->space[toC][toR] != ' ' && getGameBoard()->space[toC][toR] != '219'))
	//				{
	//					cout << "There is a piece in that space, you cannot move there. Please make a valid move selection" << endl;
	//					getMove(getTurn());
	//				}
	//				//check to see if the piece is moving forward and only moving forward one space
	//				else if (((toC - 1) != fromC) || (toC - 1) == fromC && ((toR != (fromR + 1)) && (toR != fromR) && (toR != (fromR - 1))))
	//				{
	//					cout << "You may only move forward one space. Please make a valid move selection." << endl;
	//					getMove(getTurn());
	//				}
	//				else
	//				{
	//					return;
	//				}
	//			}
	//			else
	//			{
	//				cout << "That is not your piece. Please make a valid selection" << endl;
	//				getMove(getTurn());
	//			}
	//		}

	//		// check to see if a piece is where you want to move
	//	}
	//	else
	//	{
	//		cout << "Please make a Valid Move." << endl;
	//		getMove(getTurn());
	//	}
	//}
	//else
	//{
	//	cout << "Please make a Valid Move." << endl;
	//	getMove(getTurn());
	//}
}

void Game::startGameLoop()
{
	do
	{	//May need to cath that 0 (or whateveer) I was throwing here
		updateBoard(players[pTurn % 2]->getMove());
		printBoard();
		pTurn++;
	} while (!checkForWin());
}
	



int Game::getNumber(char c)
{
	int num = 7;
	c = tolower(c);

	switch (c) {
	case 'a': num = 0;
		break;
	case 'b': num = 1;
		break;
	case 'c': num = 2;
		break;
	case 'd': num = 3;
		break;
	case 'e': num = 4;
		break;
	case 'f': num = 5;
		break;
	case 'g': num = 6;
		break;
	case 'h': num = 7;
		break;
	case '1': num = 7;
		break;
	case '2': num = 6;
		break;
	case '3': num = 5;
		break;
	case '4': num = 4;
		break;
	case '5': num = 3;
		break;
	case '6': num = 2;
		break;
	case '7': num = 1;
		break;
	case '8': num = 0;
		break;
	}

	return num;
}

bool Game::checkLastRow(char piece)
{
	if (piece == 'W')						//If piece is 'W'
		for (int i = A8; i <= H8; i++)		//Loop through the back row
			if (getGameBoard()[i] == 'W')	//Check if one is 'W'	
				return true;				//Return true if it is
		
	if (piece == 'B')						//Do the same thing for black
		for (int i = A1; i <= H1; i++)		//But check the other side
			if (getGameBoard()[i] == 'B')
				return  true;

	return false;							//Both failed to find it or invalid piece
}

bool Game::checkForPieces(char)
{
	return !pieces[WHITE] || !pieces[BLACK];
}

bool Game::updateBoard(move move) 
{
	if (getGameBoard()[move.second] == 'W') pieces[WHITE]--;
	if (getGameBoard()[move.second] == 'B') pieces[BLACK]--;
 
	getGameBoard()[move.first] = move.first % 2 == 0 ? 219 : ' ';
	getGameBoard()[move.second] = pTurn % 2 == 0 ? 'W' : 'B';

	/*int fromR = getNumber(from[0]);
	int fromC = getNumber(from[1]);

	int toR = getNumber(to[0]);
	int toC = getNumber(to[1]);

	getGameBoard()->space[fromC][fromR] = ' ';

	if(player == 1)
		getGameBoard()->space[toC][toR] = 'W';
	else 
		getGameBoard()->space[toC][toR] = 'B';

	return true;*/
	
	//I don't need to return a value though....
	return true;
}

void Game::printBoard()
{
	for (int r = 7; r >= 0; r--)
	{
		cout << r+1 <<"|";
	
		for (int c = 0; c < 8; c++)
		{
			cout << (*GameBoard::getInstance())[r * 8 + c] << "|";
		}
		cout << endl;
	}

	cout << "  A B C D E F G H" << endl << "-----------------------------" << endl;
}