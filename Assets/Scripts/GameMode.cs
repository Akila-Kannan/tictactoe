using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TicTacToe
{
    public interface  GameMode 
    {
        public  void SetBoard(TicTacToeBoard board);

        public  void UpdateTurn();
        public  void UpdateCurrentPlayerMove(int xpo, int ypos, int pos = -1);
        public  void OnGameConfigSubmit();
        public  void Reset();
    }
}
