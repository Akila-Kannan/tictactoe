using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public GameMode gameMode;
        public SinglePlayer singlePlayerGame;
        public TicTacToeColyseusManager multiplayer;
        [SerializeField] TicTacToeBoard board;
        public GameObject gameWin;
        public TMPro.TMP_Text gameWinText;
        [SerializeField] TMPro.TMP_Dropdown gameAiSelect;
        public Colyseus.ColyseusSettings settings;
        [SerializeField] ParticleSystem particleSystem;
        //public Multiplayer multiPlayerGame;
        private void Awake()
        {
            instance = this;
            gameAiSelect.onValueChanged.AddListener((val) => ((SinglePlayer)gameMode).ChooseAI(val));
        }
        public void OnGameModeSelect(int index)
        {
            if (index == 0)
            {
                gameMode = Instantiate(singlePlayerGame);

            }
            if (index == 1) {
                gameMode = Instantiate(multiplayer);
            }
            gameMode.SetBoard(board);
        }
        [SerializeField] TMPro.TMP_InputField boardSize;
        public void OnGameConfig()
        {
            GameConfig.rowCount = GameConfig.columnCount = int.Parse(boardSize.text);
            gameMode.OnGameConfigSubmit();
        }
        public void OnGameWin(string message)
        {
            gameWinText.text = message;
            particleSystem.gameObject.SetActive(true);
            Invoke(nameof(delayDisplayWin), 5);
        }
        void delayDisplayWin() {
            particleSystem.gameObject.SetActive(false);
            gameWin.SetActive(true);
        }
        public void OnGameReset()
        {
            gameMode.Reset();
        }

    }
}
