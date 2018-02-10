using System;

namespace GameCore.Core
{
	using move = Tuple<Square, Square>;
	abstract class Player
	{
		public Turn myColor;
		public Player() { }
		public abstract move getMove(GameBoard gp);
	}
}
