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

void Game::startGameLoop()
{
	do
	{	//May need to cath that 0 (or whateveer) I was throwing here
		updateBoard(players[pTurn % 2]->getMove());
		printBoard();
		pTurn++;
	} while (!checkForWin());
}


bool Game::checkLastRow(char piece)
{
	if (piece == 'W')						//If piece is 'W'
		for (int i = A8; i <= H8; i++)		//Loop through the back row
			if (getGameBoard()[(Square)i] == 'W')	//Check if one is 'W'	
				return true;				//Return true if it is
		
	if (piece == 'B')						//Do the same thing for black
		for (int i = A1; i <= H1; i++)		//But check the other side
			if (getGameBoard()[(Square)i] == 'B')
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
	
	getGameBoard().updateBoard(move, Turn(pTurn % 2));
	
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
			cout << (*GameBoard::getInstance())[Square(r * 8 + c)] << "|";
		}
		cout << endl;
	}

	cout << "  A B C D E F G H" << endl << "-----------------------------" << endl;
}