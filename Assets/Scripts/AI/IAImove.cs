using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TicTacToe
{
    public interface IAImove
    {
        public Symbol symbol { get; set; }

        int Move(TicTacToeBoard board);
    }
}
