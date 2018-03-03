using System;

//Immutable class representing a move
class Move
{
    public enum Laterality { LEFT, RIGHT, CENTER };

    public Coordinate To { get; private set; }
    public Coordinate From { get; private set; }
    public Laterality Direction { get; private set; }
    public Move(Coordinate to, Coordinate from)
    {

        if (From.X == To.X) Direction = Laterality.CENTER;
        else if (From.X - 1 == To.X) Direction = Laterality.LEFT;
        else if (From.X + 1 == To.X) Direction = Laterality.RIGHT;
        //WARNING - while the following line provides some error checking, it is NOT foolproof;
        //          it does not check forward/backward, for example. Provide your own validation.
        else throw new ArgumentException("This is not a valid move");

        To = to; From = from;
    }
}

//Immutable class representing a space on the board
class Coordinate
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
}
