using System;

namespace GameCore.Core
{
	using move = Tuple<Square, Square>;
	class Game
	{
		public Game()
		{
		}
		public void newGame(Player white, Player black, bool print = true)
		{
			Board = new GameBoard();
			pieces = new int[]{ 16, 16 };

			players[(int)Turn.WHITE] = white;
			players[(int)Turn.BLACK] = black;

			white.myColor = Turn.WHITE;
			black.myColor = Turn.BLACK;

			if (print) printBoard();
			startGameLoop(print);
		}
		public bool GameOver {
			get {
				for (Square i = Square.A8; i <= Square.H8; i++)	//Loop through the back row
					if (Board[i] == 'W')						//Check if one is 'W'	
						return true;							//Return true if it is

				for (Square i = Square.A1; i <= Square.H1; i++)	//check the other side
					if (Board[i] == 'B')
						return true;

				return pieces[0] == 0 || pieces[1] == 0;
			}
		}
		
		public Turn Winner {
			get {
				for (Square i = Square.A8; i <= Square.H8; i++) //Loop through the back row
					if (Board[i] == 'W')                        //Check if one is 'W'	
						return Turn.WHITE;

				for (Square i = Square.A1; i <= Square.H1; i++) //check the other side
					if (Board[i] == 'B')
						return Turn.BLACK;

				if (pieces[(int)Turn.WHITE] == 0) return Turn.BLACK;

				else return Turn.WHITE; //Because someone has to win if you're asking
			}
		}


		private Player[] players = new Player[2];
		private GameBoard Board { get; set; }
		private int[] pieces = { 16, 16 };
		public Turn PlayerTurn { get; private set; }
				
		
		private void updateBoard(move move)
		{
			if (Board[move.Item2] == 'W') pieces[(int)Turn.WHITE]--;
			if (Board[move.Item2] == 'B') pieces[(int)Turn.BLACK]--;

			Board.updateBoard(move, PlayerTurn);
		}
		private void startGameLoop(bool print = true)
		{
			do
			{
				updateBoard(players[(int)PlayerTurn].getMove(Board));

				if (print) printBoard();
				
				PlayerTurn = (Turn)Convert.ToInt16(!Convert.ToBoolean(PlayerTurn));
			} while (!GameOver);
		}
		private void printBoard()
		{
			for (int r = 7; r >= 0; r--)
			{
				Console.Write("{0}|", r + 1);

				for (int c = 0; c < 8; c++)
				{
					Console.Write("{0}|", Board[(Square)(r * 8 + c)]);

				}
				Console.WriteLine("");
			}

			Console.WriteLine("  A B C D E F G H");
			Console.WriteLine("-----------------------------");
		}
	}
}
