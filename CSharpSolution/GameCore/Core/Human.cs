using System;

namespace GameCore.Core
{
	using move = Tuple<Square, Square>;

	class Human : Player
	{
		public override move getMove(GameBoard gp)
		{
			move result;
			string moveFrom, moveTo;

			do
			{
				do
				{
					Console.Write("Move From: ");
					moveFrom = Console.ReadLine();
					//if (moveFrom == "exit") throw 0;
				} while (Char.ToUpper(moveFrom[0]) < 'A' || Char.ToUpper(moveFrom[0]) > 'H' || moveFrom[1] > '8' || moveFrom[1] < '1');
				do
				{
					Console.Write("Move To: ");
					moveTo = Console.ReadLine();
					//if (moveTo == "exit") throw 0;
				} while (Char.ToUpper(moveTo[0]) < 'A' || Char.ToUpper(moveTo[0]) > 'H'  || moveTo[1] > '8' || moveTo[1] < '1');
				
				result = new move((Square)((Char.ToUpper(moveFrom[0]) - 'A') + (moveFrom[1] - '1') * 8), (Square)((Char.ToUpper(moveTo[0]) - 'A') + (moveTo[1] - '1') * 8));

			} while (!isValidMove(result, gp));

			return result;
		}
		protected bool isValidMove(move move, GameBoard board)
		{

			Square from = move.Item1;
			Square to = move.Item2;

			char myPiece = myColor == Turn.WHITE ? 'W' : 'B';
			Direction myForward = myColor == Turn.WHITE ? Direction.NORTH : Direction.SOUTH;
			Direction myLeft = (Direction)((int)myForward + (int)Direction.WEST);
			Direction myRight = (Direction)((int)myForward + (int)Direction.EAST);

			// check to see if selected piece is your own
			if (board[from] != myPiece)
			{
				Console.WriteLine("That is not your piece. Please make a valid move selection.");
				return false;
			}

			//check to see if there is a piece in front of the player
			if ((int)from + (int)myForward == (int)to && (board[to] == 'W' || board[to] == 'B'))
			{
				Console.WriteLine("There is a piece in that space, you cannot move there. Please make a valid move selection");
				return false;
			}

			//check to see if there is the player's piece diagonal of the player
			if (((int)from + (int)myLeft == (int)to || (int)from + (int)myRight == (int)to) && board[to] == myPiece)
			{
				Console.WriteLine("There is a piece in that space, you cannot move there. Please make a valid move selection");
				return false;
			}

			//check to see not moving
			if (to == from)
			{
				Console.WriteLine("You must move. Please make a valid move selection.");
				return false;
			}

			//check to see if the piece is moving forward and only moving forward one space
			if ((int)to - (int)from % 8 == 0 && ((int)to - (int)from) * (int)myForward > 0)
			{
				Console.WriteLine("You may only move forward one space. Please make a valid move selection.");
				return false;
			}

			//check to see if moving backwards
			if (((int)to - (int)from) * (int)myForward < 0)
			{
				Console.WriteLine("You may not move backwards. Please make a valid move selection.");
				return false;
			}

			//check to see if moving sideways
			if ((int)to / 8 == (int)from / 8)
			{
				Console.WriteLine("You may not move sideways. Please make a valid move selection.");
				return false;
			}

			//check to see wrapping
			if (((int)from + (int)myRight == (int)to && (int)to % 8 == 0) || ((int)from + (int)myLeft == (int)to && (int)to % 8 == 7))
			{
				Console.WriteLine("You may not wrap around the board. Please make a valid move selection.");
				return false;
			}

			//catch all?
			if ((int)from + (int)myForward != (int)to && (int)from + (int)myLeft != (int)to && (int)from + (int)myRight != (int)to)
			{
				Console.WriteLine("That is an invalid move. Please make a valid move selection");
				return false;
			}

			return true;
		}
	}
}
