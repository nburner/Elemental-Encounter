using System;

//Immutable class representing a move
public class Move : IEquatable<Move>
{
    public enum Laterality { LEFT, RIGHT, CENTER };

    public Coordinate To { get; private set; }
    public Coordinate From { get; private set; }
    public Laterality Direction { get; private set; }
    public Move(Coordinate from, Coordinate to)
    {

        if (from.X == to.X) Direction = Laterality.CENTER;
        else if (from.X - 1 == to.X) Direction = Laterality.LEFT;
        else if (from.X + 1 == to.X) Direction = Laterality.RIGHT;
        //WARNING - while the following line provides some error checking, it is NOT foolproof;
        //          it does not check forward/backward, for example. Provide your own validation.
        else throw new ArgumentException("This is not a valid move");

        To = to; From = from;
    }

    public bool Equals(Move other) {
        return To.Equals(other.To) && From.Equals(other.From);
    }
}

//Immutable class representing a space on the board
public class Coordinate : IEquatable<Coordinate>
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Coordinate(int x, int y)
    {
        if (x < 0 || x > 7) throw new ArgumentException("The value provided for x is outside of the board");
        if (y < 0 || y > 7) throw new ArgumentException("The value provided for y is outside of the board");

        X = x; Y = y;
    }

    public static implicit operator Coordinate(AI.Square s)
    {
        return new Coordinate((int)s % 8, (int)s / 8);
    }

    public bool Equals(Coordinate other) {
        return X == other.X && Y == other.Y;
    }
}
