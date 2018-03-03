using System;

//This class abstracts a 2d array for the purpose of representing a board
//Specifically so I can easily use a coordinate as an index
public class Board<T> 
{
    private T[] innerBoard = new T[64];

    #region Indexers[4]
    public T this[int x, int y]
    {
        get {
            if (x < 0 || x > 7) throw new ArgumentException("The value provided for x is outside of the board");
            if (y < 0 || y > 7) throw new ArgumentException("The value provided for y is outside of the board");
            return innerBoard[x * 8 + y];
        }
        set {
            if (x < 0 || x > 7) throw new ArgumentException("The value provided for x is outside of the board");
            if (y < 0 || y > 7) throw new ArgumentException("The value provided for y is outside of the board");
            innerBoard[x * 8 + y] = value;
        }
    }

    public T this[Coordinate c]
    {
        get { return innerBoard[c.X * 8 + c.Y]; }
        set { innerBoard[c.X * 8 + c.Y] = value; }
    }

    public T this[int x]
    {
        get
        {
            if (x < 0 || x > 63) throw new ArgumentException("The value provided for x is outside of the board");            
            return innerBoard[x];
        }
        set
        {
            if (x < 0 || x > 63) throw new ArgumentException("The value provided for x is outside of the board");            
            innerBoard[x] = value;
        }
    }

    public T this[AI.Square s]
    {
        get
        {
            int x = (int)s;
            return innerBoard[x];
        }
        set
        {
            int x = (int)s;
            innerBoard[x] = value;
        }
    }
    #endregion

    #region Looping Functions
    //This one assigns to every member based on the index
    public Board<T> SetEach(Func<Coordinate, T> setToFunc, Func<Coordinate, bool> setIfTrueFunc)
    {
        for (int i = 0; i < 64; i++) if(setIfTrueFunc(new Coordinate(i % 8, i / 8))) innerBoard[i] = setToFunc(new Coordinate(i % 8, i / 8));

        return this;
    }
    public Board<T> SetEach(Func<Coordinate, T> setToFunc)
    {
        for (int i = 0; i < 64; i++) innerBoard[i] = setToFunc(new Coordinate(i % 8, i / 8));

        return this;
    }
    //This one assigns to every member based on the current value
    public Board<T> TransformEach(Func<T, T> setToFunc)
    {
        for (int i = 0; i < 64; i++) innerBoard[i] = setToFunc(innerBoard[i]);

        return this;
    }
    public Board<T> TransformEach(Func<T, T> setToFunc, Func<T, bool> transformIfTrueFunc)
    {
        for (int i = 0; i < 64; i++) if (transformIfTrueFunc(innerBoard[i])) innerBoard[i] = setToFunc(innerBoard[i]);

        return this;
    }
    //This one does an action for every member so it doesn't actually assign to the board
    public Board<T> ForEach(Action<T> a, Func<T, bool> doIfTrueFunc)
    {
        for (int x = 0; x < 64; x++) if(doIfTrueFunc(innerBoard[x])) a(innerBoard[x]);

        return this;
    }
    public Board<T> ForEach(Action<T> a)
    {
        for (int x = 0; x < 64; x++) a(innerBoard[x]);

        return this;
    }
    #endregion
}

