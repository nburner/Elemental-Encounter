using System;

namespace GameCore.Core
{
	using move = Tuple<Square, Square>;
	class Daryl : Human
	{
		public override move getMove(GameBoard gp)
		{
			move result;
			char movefromletter, movefromnumber, movetoletter;

			do
			{
				Console.Write( "Move From: ");
				do
				{
					movefromletter = Console.ReadKey().KeyChar;
				} while (Char.ToUpper(movefromletter) < 'A' || Char.ToUpper(movefromletter) > 'H');
				do
				{
					movefromnumber = Console.ReadKey().KeyChar;
				} while (movefromnumber < '1' || movefromnumber > '8');

				Console.WriteLine("");
				Console.Write("Move To: ");
				do
				{
					movetoletter = Console.ReadKey().KeyChar;
				} while (Char.ToUpper(movefromletter) < 'A' || Char.ToUpper(movefromletter) > 'H');
				Console.WriteLine((char)(movefromnumber - ((int)myColor * 2 - 1)));

				result = new move( (Square)((Char.ToUpper(movefromletter) - 'A') + (movefromnumber - '1') * 8), (Square)((Char.ToUpper(movetoletter) - 'A') + (movefromnumber - '1' + 1) * 8));
			} while (!isValidMove(result, gp));

			return result;
		}
	}
}
