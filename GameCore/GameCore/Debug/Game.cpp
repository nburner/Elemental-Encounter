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

	if (playerTurn == WHITE) 
	{
		Victory = checkLastRow('B') || checkForPieces('B');
	} 
	else
	{
		Victory = checkLastRow('W') || checkForPieces('W');
	}

	return Victory;
}

void Game::newGame(Player * white, Player * black, bool print) 
{
	players[WHITE] = white;
	players[BLACK] = black;

	white->myColor = WHITE;
	black->myColor = BLACK;

	if(print) printBoard();
	startGameLoop(print);
}

int Game::getTurn()
{
	return playerTurn;
}

Turn Game::getColorTurn()
{
	return Turn(playerTurn % 2);
}

void Game::startGameLoop(bool print)
{
	do
	{	//May need to cath that 0 (or whateveer) I was throwing here
		updateBoard(players[playerTurn]->getMove(gameBoard));
		//auto move = players[playerTurn]->getMove(gameBoard);
		//cout << "Move: " << move.first << " - " << move.second << endl;
		//updateBoard(move);
		if(print) printBoard();
		playerTurn = !playerTurn;
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

bool Game::checkForPieces(char piece)
{

	if (piece == 'W')
	{
		return !pieces[BLACK];
	}
	else
	{
		return !pieces[WHITE];
	}
}

bool Game::updateBoard(move move) 
{
	if (getGameBoard()[move.second] == 'W') pieces[WHITE]--;
	if (getGameBoard()[move.second] == 'B') pieces[BLACK]--;
	
	getGameBoard().updateBoard(move, Turn(playerTurn));
	
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