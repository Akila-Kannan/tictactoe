using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    public class EasyAI : IAImove
    {
        public Symbol defaultSYmbol = Symbol.X;

        public Symbol symbol { get { return defaultSYmbol; } set { defaultSYmbol = value; } }

        public int Move(TicTacToeBoard board)
        {
            // Simple AI: Randomly choose an empty cell
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