using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TicTacToe
{
    public class MediumAI : IAImove
    {
        public Symbol defaultSYmbol = Symbol.X;

        public Symbol symbol { get { return defaultSYmbol; } set { defaultSYmbol = value; } }


        public int Move(TicTacToeBoard board)
        {
            int block = TryBlock(board);
            return block > -1 ? block : FillRandomly(board);
        }
        int TryBlock(TicTacToeBoard board)
        {
            int AIROws = CheckRows(board, defaultSYmbol);
            if (AIROws < 0)
            {
                int AIColumn = CheckColumns(board, defaultSYmbol);
                if (AIColumn < 0)
                {
                    int AIDiagonal = CheckDiagonals(board, defaultSYmbol);
                    if (AIDiagonal < 0)
                    {
                        int playerRows = CheckRows(board, GameConfig.pllayerSymbol);
                        if (playerRows < 0)
                        {
                            int playerColumn = CheckColumns(board, GameConfig.pllayerSymbol);
                            if (playerColumn < 0)
                            {
                                int playerDiagonal = CheckDiagonals(board, GameConfig.pllayerSymbol);
                                return playerDiagonal;
                            }
                            else return playerColumn;
                        }
                        else return playerRows;
                    }
                    else return AIDiagonal;
                }
                else
                    return AIColumn;
            }
            else return AIROws;

            //return -1;
        }
        private int CheckRows(TicTacToeBoard board, Symbol checkState)
        {
            int count = 0;
            for (int row = 0; row < GameConfig.rowCount; row++)
            {
                count = 0;
                int emptySymbolIndeX = -1;
                int emptySymbolIndeY = -1;

                for (int col = 0; col < GameConfig.columnCount; col++)
                {

                    if (board.boardCellHolder[row, col].symbol == checkState)
                    {
                        count++;

                    }
                    else if (board.boardCellHolder[row, col].symbol == Symbol.EMPTY)
                    {
                        emptySymbolIndeX = row;
                        emptySymbolIndeY = col; ;
                    }
                }
                if ((count + 1) == GameConfig.columnCount && emptySymbolIndeX > -1)
                {
                    board.FillSlot(defaultSYmbol, -1, emptySymbolIndeX, emptySymbolIndeY);
                    return emptySymbolIndeX + emptySymbolIndeY;
                }
                Debug.Log("row " + row + " count " + count + " symbol " + checkState);
            }

            return -1;
        }

        private int CheckColumns(TicTacToeBoard board, Symbol checkState)
        {
            int count = 0;

            for (int col = 0; col < GameConfig.columnCount; col++)
            {

                int emptySymbolIndeX = -1;
                int emptySymbolIndeY = -1;



                for (int row = 0; row < GameConfig.rowCount; row++)
                {
                    if (board.boardCellHolder[row, col].symbol == checkState)
                    {
                        count++;

                    }
                    else if (board.boardCellHolder[row, col].symbol == Symbol.EMPTY)
                    {
                        emptySymbolIndeX = row;
                        emptySymbolIndeY = col; ;
                    }
                }
                if ((count + 1) == GameConfig.columnCount && emptySymbolIndeX > -1)
                {
                    board.FillSlot(defaultSYmbol, -1, emptySymbolIndeX, emptySymbolIndeY);
                    return emptySymbolIndeX + emptySymbolIndeY;
                }
                Debug.Log("column " + col + " count " + count + " symbol " + checkState);

                count = 0;

            }
            return -1;
        }


        private int CheckDiagonals(TicTacToeBoard board, Symbol checkState)
        {

            int diagonalCount = 0;
            int antiDiagonalCount = 0;
            int diagonalEmptyX = -1;
            int diagonalEmptyY = -1;
            int antiDiagonalEmptyX = -1;
            int antiDiagonalEmptyY = -1;
            bool diagonalEmpty = false;
            bool antidiagonalEmpty = false;

            for (int i = 0; i < GameConfig.rowCount; i++)
            {

                if (board.boardCellHolder[i, i].symbol == checkState)
                {
                    diagonalCount++;

                }
                else if (board.boardCellHolder[i, i].symbol == Symbol.EMPTY)
                {
                    diagonalEmpty = true;
                    diagonalEmptyX = diagonalEmptyY = i;

                }

                if (board.boardCellHolder[i, GameConfig.rowCount - 1 - i].symbol == checkState)
                {
                    antiDiagonalCount++;

                }
                else if (board.boardCellHolder[i, GameConfig.rowCount - 1 - i].symbol == Symbol.EMPTY)
                {
                    antiDiagonalEmptyX = i;
                    antiDiagonalEmptyY = GameConfig.rowCount - 1 - i;
                    antidiagonalEmpty = true;


                }
            }
            Debug.Log("Diagonal count " + diagonalCount + " symbol " + checkState);
            Debug.Log("antiDiagonal count " + antiDiagonalCount + " symbol " + checkState);
            if ((diagonalCount + 1) == GameConfig.rowCount && diagonalEmpty)
            {
                board.FillSlot(defaultSYmbol, -1, diagonalEmptyX, diagonalEmptyY);
                return diagonalEmptyX + diagonalEmptyY;
            }
            if ((antiDiagonalCount + 1) == GameConfig.rowCount && antidiagonalEmpty)
            {
                board.FillSlot(defaultSYmbol, -1, antiDiagonalEmptyX, antiDiagonalEmptyY);
                return antiDiagonalEmptyX + antiDiagonalEmptyY;

            }
            return -1;
        }
        int FillRandomly(TicTacToeBoard board)
        {
            int emptyCellCount = 0;

            for (int row = 0; row < GameConfig.rowCount; row++)
            {
                for (int col = 0; col < GameConfig.columnCount; col++)
                {
                    if (board.boardCellHolder[row, col].symbol == Symbol.EMPTY)
                        emptyCellCount++;
                }
            }
            int randomIndex = Random.Range(0, emptyCellCount);

            if (emptyCellCount > 0)
            {
                //randomIndex = Random.Range(0, emptyCellCount);
                int count = 0;

                for (int row = 0; row < GameConfig.rowCount; row++)
                {
                    for (int col = 0; col < GameConfig.columnCount; col++)
                    {
                        if (board.boardCellHolder[row, col].symbol == Symbol.EMPTY)
                        {
                            if (count == randomIndex)
                            {
                                board.FillSlot(defaultSYmbol, -1, row, col);
                                return randomIndex;
                            }
                            count++;
                        }
                    }
                }
            }
            return randomIndex;
        }


    }
}
