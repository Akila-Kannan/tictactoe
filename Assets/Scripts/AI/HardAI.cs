using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace TicTacToe
{
    public class HardAI : IAImove
    {
        public Symbol defaultSYmbol = Symbol.X;

        public Symbol symbol { get { return defaultSYmbol; } set { defaultSYmbol = value; } }

        public int Move(TicTacToeBoard board)
        {
            int bestVal = -1000 * GameConfig.rowCount;

            List<Move> moves = new List<Move>();

            for (int i = 0; i < GameConfig.rowCount; i++)
            {
                for (int j = 0; j < GameConfig.columnCount; j++)
                {
                    // Check if cell is empty 
                    if (board.boardCellHolder[i, j].symbol == Symbol.EMPTY)
                    {
                        // Make the move 
                        board.boardCellHolder[i, j].symbol = defaultSYmbol;

                        // compute evaluation function for this 
                        // move. 
                        int moveVal = MiniMax(board, 0, false);

                        // Undo the move 
                        board.boardCellHolder[i, j].symbol = Symbol.EMPTY;

                        // If the value of the current move is 
                        // more than the best value, then update 
                        // best/ 
                        if (moveVal > bestVal)
                        {
                            moves.Add(new Move(i, j, moveVal));
                            //scores.Add(moveVal, );
                            //board.FillSlot(defaultSYmbol, -1, i, j);
                            //return i + j;
                        }
                    }
                }
            }

            List<Move> sortedList = moves.OrderByDescending(obj => obj.score).ToList();
            if (sortedList.Count > 0) 
            board.FillSlot(defaultSYmbol, -1, sortedList[0].row, sortedList[0].col);
            return -1;
        }

        public int MiniMax(TicTacToeBoard board, int depth, bool isMax)
        {
            int score = evaluate(board);
            if (score == 10)
                return score;
            if (score == -10)
                return score;
            if (!board.HavingEmptyCell())
                return 0;
            if (isMax)
            {
                int best = -1000 * GameConfig.rowCount;

                // Traverse all cells 
                for (int i = 0; i < GameConfig.rowCount; i++)
                {
                    for (int j = 0; j < GameConfig.columnCount; j++)
                    {
                        // Check if cell is empty 
                        if (board.boardCellHolder[i, j].symbol == Symbol.EMPTY)
                        {
                            // Make the move 
                            board.boardCellHolder[i, j].symbol = defaultSYmbol;

                            // Call minimax recursively and choose 
                            // the maximum value 
                            best = Mathf.Max(best, MiniMax(board,
                                            depth + 1, !isMax));

                            // Undo the move 
                            board.boardCellHolder[i, j].symbol = Symbol.EMPTY;
                        }
                    }
                }
                return best;
            }

            // If this minimizer's move 
            else
            {
                int best = 1000 * GameConfig.rowCount;

                // Traverse all cells 
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty 
                        if (board.boardCellHolder[i, j].symbol == Symbol.EMPTY)

                        {
                            // Make the move 
                            board.boardCellHolder[i, j].symbol = GameConfig.pllayerSymbol;

                            // Call minimax recursively and choose 
                            // the minimum value 
                            best = Mathf.Min(best, MiniMax(board,
                                            depth + 1, !isMax));

                            // Undo the move 
                            board.boardCellHolder[i, j].symbol = Symbol.EMPTY;

                        }
                    }
                }
                return best;
            }
        }
        public int evaluate(TicTacToeBoard board)
        {

            if (board.CheckForWin(GameConfig.pllayerSymbol))
            {
                return -10;
            }
            if (board.CheckForWin(defaultSYmbol))
                return +10;
            return 0;
            // Checking for Rows for X or O victory. 
        }

    }
    public struct Move
    {
        public int row, col, score;
        public Move(int row, int col, int score)
        {
            this.row = row;
            this.col = col;
            this.score = score;
        }
    }
}
