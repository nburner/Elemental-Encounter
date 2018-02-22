using System;
using System.Linq;
using System.Collections.Generic;

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
            if (print) Console.WriteLine("WHITE: {0}\t\tBLACK: {1}", white.ToString(), black.ToString());

            Board = new GameBoard();
            previousStates = new Stack<GameBoard>();
            pieces = new int[] { 16, 16 };

            players[(int)Turn.WHITE] = white;
            players[(int)Turn.BLACK] = black;

            white.myColor = Turn.WHITE;
            black.myColor = Turn.BLACK;

            PlayerTurn = Turn.WHITE;

            moveLog.Push("Game Begins");

            if (print) printBoard();
            startGameLoop(print);
        }
        public void newGame(Player white, Square[] whitePieces, Player black, Square[] blackPieces, Turn t, bool print = true)
        {
            if (print) Console.WriteLine("WHITE: {0}\t\tBLACK: {1}", white.ToString(), black.ToString());

            Board = new GameBoard(whitePieces, blackPieces);
            previousStates = new Stack<GameBoard>();
            pieces = new int[] { whitePieces.Length, blackPieces.Length };

            players[(int)Turn.WHITE] = white;
            players[(int)Turn.BLACK] = black;

            white.myColor = Turn.WHITE;
            black.myColor = Turn.BLACK;

            PlayerTurn = t;

            moveLog.Push("Game Begins");

            if (print) printBoard();
            startGameLoop(print);
        }
        public bool GameOver
        {
            get
            {
                for (Square i = Square.A8; i <= Square.H8; i++) //Loop through the back row
                    if (Board[i] == 'W')                        //Check if one is 'W'	
                        return true;                            //Return true if it is

                for (Square i = Square.A1; i <= Square.H1; i++) //check the other side
                    if (Board[i] == 'B')
                        return true;

                return pieces[0] == 0 || pieces[1] == 0;
            }
        }

        public Turn Winner
        {
            get
            {
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
        private Stack<GameBoard> previousStates = new Stack<GameBoard>();
        private Stack<String> moveLog = new Stack<String>();

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
                previousStates.Push(Board.Clone());

                move m = players[(int)PlayerTurn].getMove(Board);
                if (m.Item1 == Square.A1 && m.Item2 == Square.A1)
                {
                    Undo();
                    if (print) printBoard();
                    continue;
                }
                updateBoard(m);
                moveLog.Push(String.Format("{2} by {0} ({1})", players[(int)PlayerTurn], PlayerTurn, m));

                if (print) printBoard();

                PlayerTurn = (Turn)Convert.ToInt16(!Convert.ToBoolean(PlayerTurn));
            } while (!GameOver);
            if (print) Console.WriteLine("WINNER: {0} ({1})", Winner, players[(int)Winner]);
        }

        private void Undo()
        {
            if (previousStates.Count > 2)
            {
                moveLog.Pop();
                moveLog.Pop();
                previousStates.Pop();
                previousStates.Pop();
                Board = previousStates.Peek();
            }
        }

        private static int BoardPrintTop = -1;
        private static int BoardPrintLeft = -1;
        private static int PreviousTop = -1;
        private static int PreviousLeft = -1;
        private static bool FirstPrint = true;
        private void printBoard()
        {
            if (BoardPrintTop < 0) BoardPrintTop = Console.CursorTop;
            else Console.CursorTop = BoardPrintTop;
            if (BoardPrintLeft < 0) BoardPrintLeft = Console.CursorLeft;
            else Console.CursorLeft = BoardPrintLeft;

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

            if (FirstPrint)
            {
                PreviousTop = Console.CursorTop;
                PreviousLeft = Console.CursorLeft;
                FirstPrint = false;
            }

            //Clear Log
            Console.CursorTop = PreviousTop + 5;
            Console.CursorLeft = PreviousLeft + 5;
            //if(moveLog.Count > 1) Console.WriteLine(moveLog.Skip(1).Select((str) => { return String.Concat(Enumerable.Repeat(" ", str.Length)); }).Aggregate((str1, str2) => { return str1 + "\n" + str2; }));
            Console.WriteLine(moveLog.Select((str) => { return String.Concat(Enumerable.Repeat(" ", str.Length)); }).Aggregate((str1, str2) => { return str1 + "\n" + str2; }));
            //Print Log
            Console.CursorTop = PreviousTop + 5;
            Console.CursorLeft = PreviousLeft;
            Console.WriteLine("*********** LOG ***********");
            Console.WriteLine(moveLog.Aggregate((str1, str2) => { return str1 + "\n" + str2; }));

            //Clear the move space
            Console.CursorTop = PreviousTop + (PlayerTurn == Turn.BLACK ? 2 : 0);
            Console.CursorLeft = PreviousLeft;
            Console.WriteLine("                             \n                             ");
            //if(PlayerTurn == Turn.WHITE) Console.WriteLine("                             \n                             ");

            Console.CursorTop = PreviousTop + (PlayerTurn == Turn.BLACK ? 2 : 0);
            Console.CursorLeft = PreviousLeft;
        }
    }
}
