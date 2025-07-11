using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class TicTacToeColyseusManager : ColyseusManager<TicTacToeColyseusManager>, GameMode
    {
        TicTacToeBoard Board;
        [SerializeField] Canvas waitAndConfig;
        [SerializeField] GameObject configPanel;
        [SerializeField] GameObject waitingPanel;
        [SerializeField] TMPro.TMP_InputField roomname;
        [SerializeField] TMPro.TMP_InputField boardSize;
        [SerializeField] TMPro.TMP_Dropdown available_rooms;
        [SerializeField] Button create;
        [SerializeField] Button join;
        public delegate void OnRoomsReceived(ColyseusRoomAvailable[] rooms);

        public static OnRoomsReceived onRoomsReceived;

        public ColyseusRoom<State> Room
        {
            get
            {
                return _roomController.Room;
            }
        }
        [SerializeField]
        private RoomController _roomController;
        private bool isInitialized;

        public static bool IsReady
        {
            get
            {
                return Instance != null;
            }
        }


        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private void Awake()
        {
            Initialize("my_room", new Dictionary<string, object>());
            OverrideSettings(GameManager.instance.settings);
            InitializeClient();

        }
        //        /// <summary>
        //        ///     <see cref="MonoBehaviour" /> callback when a script is enabled just before any of the Update methods are called the
        //        ///     first time.
        //        /// </summary>
        protected override void Start()
        {
            // For this example we're going to set the target frame rate
            // and allow the app to run in the background for continuous testing.
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            waitAndConfig.gameObject.SetActive(true);
            waitingPanel.SetActive(true);
            configPanel.gameObject.SetActive(false);
            //while (!IsReady)
            //{
            //    yield return new WaitForEndOfFrame();

            //}
            GetAvailableRooms();
            onRoomsReceived += (rooms) =>
            {
                availableRooms = rooms;
                this.available_rooms.ClearOptions();
                if (rooms.Length > 0)
                {
                    List<string> roomnames = new List<string>();

                    for (int i = 0; i < rooms.Length; i++)
                    {
                        roomnames.Add(rooms[i].name);

                    }
                    this.available_rooms.AddOptions(roomnames);
                    available_rooms.gameObject.SetActive(true);
                    join.gameObject.SetActive(true);
                }
                else
                {
                    available_rooms.gameObject.SetActive(false);
                    join.gameObject.SetActive(false);
                }
                waitingPanel.gameObject.SetActive(false);
                configPanel.gameObject.SetActive(true);
            };



            
            create.onClick.AddListener(() => {
                waitingPanel.SetActive(true);
                configPanel.gameObject.SetActive(false);
                GameConfig.rowCount = GameConfig.columnCount = int.Parse(boardSize.text);
                _roomController.AddRoomOptions("BOARD_WIDTH", int.Parse(boardSize.text));
                CreateNewRoom(roomname.text);
                waitAndConfig.gameObject.SetActive(false);
                Board.gameObject.SetActive(true);
            });
            join.onClick.AddListener(() => {
                waitingPanel.SetActive(false);
                JoinExistingRoom(availableRooms[(available_rooms.value)].roomId, true);
               
            });


        }
        ColyseusRoomAvailable[] availableRooms;
        //IEnumerator CreateRoom(Task task) {
        //    while (task.IsCompleted)
        //    {
        //        yield return new WaitForEndOfFrame();
        //    }

        //}
        public void Initialize(string roomName = "", Dictionary<string, object> roomOptions = null)
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            // Set up room controller
            _roomController = new RoomController { roomName = roomName };
            _roomController.SetRoomOptions(roomOptions);
        }

        /// <summary>
        /// Initialize the client
        /// </summary>
        public override void InitializeClient()
        {
            base.InitializeClient();

            _roomController.SetClient(client);
        }

        public async void GetAvailableRooms()
        {
            try
            {
                ColyseusRoomAvailable[] rooms = await client.GetAvailableRooms<ColyseusRoomAvailable>(_roomController.roomName);
                onRoomsReceived?.Invoke(rooms);

            }
            catch (Exception ex) {
                Debug.LogError(" can't get the available rooms " + ex);
                onRoomsReceived?.Invoke(null);

            }
        }

        public async void JoinExistingRoom(string roomID, bool isNewJoin)
        {
            await _roomController.JoinRoomId(roomID, isNewJoin);
            closeAllPanelsOpenBoard();
        }
        void closeAllPanelsOpenBoard() {
            waitAndConfig.gameObject.SetActive(false);
            configPanel.gameObject.SetActive(false);
            waitAndConfig.gameObject.SetActive(false);
            Board.gameObject.SetActive(true);
            _roomController.Room.OnStateChange += ((state,isFirstState) => {
                if (isFirstState) {
                    StartCoroutine(GameCheck());
                }
            });
        }

        public async void CreateNewRoom(string roomID)
        {
            await _roomController.CreateSpecificRoom(client, _roomController.roomName, roomID);
            closeAllPanelsOpenBoard();
        }

        public async void LeaveAllRooms(Action onLeave)
        {
            await _roomController.LeaveAllRooms(true, onLeave);
        }

        /// <summary>
        ///     On detection of <see cref="OnApplicationQuit" /> will disconnect
        ///     from all <see cref="rooms" />.
        /// </summary>
        private void CleanUpOnAppQuit()
        {
            if (client == null)
            {
                return;
            }

            _roomController.CleanUp();
        }

        /// <summary>
        ///     <see cref="MonoBehaviour" /> callback that gets called just before app exit.
        /// </summary>
        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            _roomController.LeaveAllRooms(true);

            CleanUpOnAppQuit();
        }

#if UNITY_EDITOR
        public void OnEditorQuit()
        {
            OnApplicationQuit();
        }
#endif

        /// <summary>
        ///     Send an action and message object to the room.
        /// </summary>
        /// <param name="action">The action to take</param>
        /// <param name="message">The message object to pass along to the room</param>
        public static void NetSend(string action, object message = null)
        {
            if (Instance._roomController.Room == null)
            {
                Debug.LogError($"Error: Not in room for action {action} msg {message}");
                return;
            }

            _ = message == null
                ? Instance._roomController.Room.Send(action)
                : Instance._roomController.Room.Send(action, message);
        }

        public void SetBoard(TicTacToeBoard board)
        {
            Board = board;
            _roomController.SetBoard(board);
        }

        public void UpdateTurn()
        {
        }
        IEnumerator GameCheck() {
            
            Debug.Log("ROOM CONTROLLER "+ string.IsNullOrEmpty(_roomController.Room.State.winner)+ _roomController.Room.State.draw);
            while (string.IsNullOrEmpty(_roomController.Room.State.winner) && !_roomController.Room.State.draw) {
                Debug.Log("ROOM CONTROLLER while " + string.IsNullOrEmpty(_roomController.Room.State.winner) + _roomController.Room.State.draw);

                yield return new  WaitForEndOfFrame();
            }
            string msg;
            if (!string.IsNullOrEmpty(_roomController.Room.State.winner))
            {
                msg = int.Parse(_roomController.Room.State.winner)== 1 ? " X ": " O " + " wins";
            }
            else
                msg = "match draw";
            GameManager.instance.OnGameWin(msg);

            StartCoroutine( RoomLeave(_roomController.Room.Leave(true)));
           
        }
        bool roomLeft = false;
        bool resetCalledAlready = false;
        IEnumerator RoomLeave(Task task) {
            roomLeft = false;
            while (!task.IsCompleted) {
                yield return new WaitForEndOfFrame();
            }
            roomLeft = true;
            Reset();

        }
        public void UpdateCurrentPlayerMove(int xpo, int ypos, int pos = -1)
        {
            Debug.Log("pos "+xpo +" y"+ ypos);
            _roomController.Room.Send("action", new Vector2(xpo,ypos) );

        }


        public void OnGameConfigSubmit()
        {
           
        }

        public void Reset()
        {

            if (roomLeft && resetCalledAlready) {
                Board.DestroyCell();
                waitAndConfig.gameObject.SetActive(true);
                waitingPanel.SetActive(true);
                configPanel.gameObject.SetActive(false);
                GetAvailableRooms();
                resetCalledAlready = false;
            }
            resetCalledAlready = true;
        }
    }
}
