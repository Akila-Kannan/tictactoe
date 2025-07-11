using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToe
{
    public class TicTacToeBoard : MonoBehaviour
    {
        public BoardCell[,] boardCellHolder;
        Sprite xImage;
        Sprite oImage;
        [SerializeField] GameObject CellPrefab;
        [SerializeField] GameObject cellHead;
        [SerializeField] GridLayoutGroup layoutGroup;

        private void Awake()
        {
            layoutGroup = GetComponentInChildren<GridLayoutGroup>();
        }
        public void CreateBoard()
        {
            boardCellHolder = new BoardCell[GameConfig.rowCount, GameConfig.columnCount];
            GameObject cell;
            Image cellImage;
            Button cellButton;
            TMPro.TMP_Text cellText;
            layoutGroup.constraintCount = GameConfig.rowCount;
            for (int i = 0, index = 0; i < GameConfig.rowCount; i++)
            {
                for (int j = 0; j < GameConfig.columnCount; j++)
                {
                    cell = Instantiate(CellPrefab, cellHead.transform);
                    cellImage = cell.GetComponent<Image>();
                    cellButton = cell.GetComponent<Button>();
                    cellText = cell.GetComponentInChildren<TMPro.TMP_Text>();
                    boardCellHolder[i, j] = new BoardCell(cellImage, cellButton, Symbol.EMPTY, cellText, index,i,j);
                    index++;
                }
            }
        }
        public void Reset()
        {
            for (int i = 0; i < GameConfig.rowCount; i++)
            {
                for (int j = 0; j < GameConfig.columnCount; j++)
                {
                    boardCellHolder[i, j].symbol = Symbol.EMPTY;
                    boardCellHolder[i, j].text.text = "";

                }
            }


        }
        public void DestroyCell()
        {
            for (int i = 0; i < GameConfig.rowCount; i++)
            {
                for (int j = 0; j < GameConfig.columnCount; j++)
                {
                    Destroy(boardCellHolder[i, j].button.gameObject);
                }
            }
            boardCellHolder = null;
        }
        public void EnableInteractable(bool interactable)
        {
            for (int i = 0; i < GameConfig.rowCount; i++)
            {
                for (int j = 0; j < GameConfig.columnCount; j++)
                {
                    if (boardCellHolder[i, j].symbol == Symbol.EMPTY)
                        boardCellHolder[i, j].button.interactable = interactable;
                }
            }
        }
        public void FillSlot(Symbol c = Symbol.EMPTY, int pos = -1, int xPos = -1, int ypos = -1,int symbolCode=0)
        {
            if (boardCellHolder==null || boardCellHolder.Length==0)
                CreateBoard();
            if (symbolCode > 0) {
                c = symbolCode == 1 ? Symbol.X : Symbol.O;
            }
            if (pos != -1)
            {
                for (int i = 0, count = 0; i < boardCellHolder.GetLength(0); i++)
                {
                    for (int j = 0; j < boardCellHolder.GetLength(1); j++)
                    {
                        if (count == pos)
                        {
                            if (c != Symbol.EMPTY)
                            {
                                boardCellHolder[i, j].image.sprite = (c == Symbol.X) ? xImage : oImage;
                                boardCellHolder[i, j].symbol = c;
                                boardCellHolder[i, j].text.text = (c == Symbol.X) ? "X" : "O";
                            }
                        }
                        count++;
                    }
                }
            }
            else
            {
                if (c != Symbol.EMPTY)
                {
                    boardCellHolder[xPos, ypos].image.sprite = (c == Symbol.X) ? xImage : oImage;
                    boardCellHolder[xPos, ypos].symbol = c;
                    boardCellHolder[xPos, ypos].text.text = (c == Symbol.X) ? "X" : "O";
                }

            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool CheckForWin(Symbol playerSymbol)
        {
            return CheckRows(playerSymbol) || CheckColumns(playerSymbol) || CheckDiagonals(playerSymbol);
        }

        private bool CheckRows(Symbol checkState)
        {
            for (int row = 0; row < GameConfig.rowCount; row++)
            {
                Symbol firstCell = boardCellHolder[row, 0].symbol;

                if (firstCell == Symbol.EMPTY) continue;

                bool rowWin = true;

                for (int col = 0; col < GameConfig.columnCount; col++)
                {
                    if (boardCellHolder[row, col].symbol != checkState)
                    {
                        rowWin = false;
                        break;
                    }
                }

                if (rowWin)
                {
                    Debug.Log("row win");
                    return true;
                }
            }

            return false;
        }
        public bool HavingEmptyCell()
        {
            for (int i = 0; i < GameConfig.rowCount; i++)
                for (int j = 0; j < GameConfig.columnCount; j++)
                    if (boardCellHolder[i, j].symbol == Symbol.EMPTY)
                        return true;
            return false;
        }

        private bool CheckColumns(Symbol checkState)
        {
            for (int col = 0; col < GameConfig.columnCount; col++)
            {
                Symbol firstCell = boardCellHolder[0, col].symbol;

                if (firstCell == Symbol.EMPTY) continue;


                bool colWin = true;

                for (int row = 0; row < GameConfig.rowCount; row++)
                {
                    if (boardCellHolder[row, col].symbol != checkState)
                    {
                        colWin = false;
                        break;
                    }
                }

                if (colWin)
                {
                    Debug.Log("column win");

                    return true;
                }
            }

            return false;
        }

        private bool CheckDiagonals(Symbol checkState)
        {
            bool mainDiagonalWin = true;
            bool antiDiagonalWin = true;

            for (int i = 0; i < GameConfig.rowCount; i++)
            {
                if (boardCellHolder[i, i].symbol != checkState)
                {
                    mainDiagonalWin = false;
                }

                if (boardCellHolder[i, GameConfig.rowCount - 1 - i].symbol != checkState)
                {
                    antiDiagonalWin = false;
                }
            }
            if (mainDiagonalWin)
                Debug.Log("diagonal win");
            if (antiDiagonalWin)
                Debug.Log("anti diagonal win");


            return mainDiagonalWin || antiDiagonalWin;
        }
    }
    public enum Symbol { EMPTY, X, O };
    public struct BoardCell
    {
        public Image image;
        public Button button;
        public Symbol symbol;
        public TMPro.TMP_Text text;
        int index;
        int x;
        int y;
        public BoardCell(Image image, Button button, Symbol symbol, TMPro.TMP_Text tMP_Text, int index,int x, int y)
        {
            this.image = image;
            this.button = button;
            this.symbol = symbol;
            text = tMP_Text;
            this.index = index;
            this.x = x;
            this.y = y;

            this.button.onClick.AddListener(() =>
            {
                if (symbol == Symbol.EMPTY)
                {
                    GameManager.instance.gameMode.UpdateCurrentPlayerMove(x, y, index);
                    GameManager.instance.gameMode.UpdateTurn();
                }
            });
        }

        public void UpdateInteractable(bool interactable)
        {
            button.interactable = interactable;
        }

    }
}
