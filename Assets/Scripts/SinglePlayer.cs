using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TicTacToe
{
    public class SinglePlayer : MonoBehaviour, GameMode
    {
        FactoryAI AIFactory;
        //TMPro.TMP_Text boardSize;
        TicTacToeBoard Board;
        Player pllayer0;
        Player AIPlayer;


        private void Awake()
        {
            RandomIntialization();
            AIFactory = new FactoryAI();

        }
        public  void SetBoard(TicTacToeBoard board)
        {
            Board = board;
        }
        void RandomIntialization()
        {
            int randomIntialPlayerSymbol = Random.Range(1, 3);
            GameConfig.pllayerSymbol = randomIntialPlayerSymbol == 1 ? Symbol.X : Symbol.O;
            GameConfig.AISymbo = randomIntialPlayerSymbol == 1 ? Symbol.O : Symbol.X;
            playerTurn = Random.Range(0, 2);
        }
        int aiMode;
        public void ChooseAI(int aimode)
        {
            aiMode = aimode;
        }


        int playerTurn;
        public  void UpdateTurn()
        {
            Board.EnableInteractable(false);
            Debug.Log("Turn "+ playerTurn+ " PLAYER SYMBOL "+ pllayer0.symbol );
            if (playerTurn == 0)
            {
                if (Board.CheckForWin(pllayer0.symbol))
                {
                    GameManager.instance.OnGameWin("player win");
                    return;
                }
            }
            else
                if (Board.CheckForWin(AIPlayer.symbol))
            {
                GameManager.instance.OnGameWin("AI win");
                return;

            }
            else
            {
                if (!Board.HavingEmptyCell())
                {
                    GameManager.instance.OnGameWin("Match Draw");
                    return;

                }
            }
            playerTurn++;
            playerTurn = playerTurn % 2;
            if (playerTurn == 0)
            {
                Board.EnableInteractable(true);
            }
            else
            {
                Board.EnableInteractable(false);
                AIPlayer.Move(Board);
                UpdateTurn();
            }
        }

        public  void UpdateCurrentPlayerMove(int xpo, int ypos, int pos = -1)
        {
            if (playerTurn == 0)
            {
                pllayer0.UpdateMove(Board, xpo, ypos, pos);
            }
            else
            {
                AIPlayer.UpdateMove(Board, xpo, ypos, pos);
            }
        }

        public  void OnGameConfigSubmit()
        {
            pllayer0 = new Player(GameConfig.pllayerSymbol, null, -1);
            AIPlayer = new Player(GameConfig.AISymbo, AIFactory.GetAI(aiMode), -1);
            if (pllayer0.symbol == AIPlayer.symbol)
                Debug.Log("symbol same");
            Board.CreateBoard();
            UpdateTurn();
        }

        public  void Reset()
        {
            Board.Reset();
            UpdateTurn();
        }
    }
    public class FactoryAI
    {
        public IAImove GetAI(int aimode)
        {
            switch (aimode)
            {
                case 0: return new EasyAI();
                case 1: return new MediumAI();
                case 2: return new HardAI();
                default: return new EasyAI();
            }

        }
    }

    public struct Player
    {
        public Symbol symbol;
        public float PlayerId;
        IAImove Ai;

        public Player(Symbol symbol, IAImove Ai = null, float playerID = -1)
        {
            this.symbol = symbol;
            this.Ai = Ai;
            this.PlayerId = playerID;
            if (Ai != null)
                Ai.symbol = symbol;
        }
        public void Move(TicTacToeBoard board)
        {
            if (Ai != null)
            {
                Ai.Move(board);
            }
        }
        public void UpdateMove(TicTacToeBoard board, int xpos, int ypos, int pos = -1)
        {
            board.FillSlot(symbol, pos, xpos, ypos);
        }
    }
}
