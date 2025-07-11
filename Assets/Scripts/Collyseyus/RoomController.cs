using Colyseus;
using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToe;
using UnityEngine;

public class RoomController 
{
    // Network Events
    //==========================
    //Custom game delegate functions
    //======================================
    public delegate void OnRoomStateChanged(State state, bool isFirstState);
    public static event OnRoomStateChanged onRoomStateChanged;
    private ColyseusClient _client;
    private ColyseusRoom<State> _room;
    public string roomName = "NO_ROOM_NAME_PROVIDED";
    private Dictionary<string, object> roomOptionsDictionary = new Dictionary<string, object>();


    /// <summary>
    ///     All the connected rooms.
    /// </summary>
    public List<IColyseusRoom> rooms = new List<IColyseusRoom>();
    public ColyseusRoom<State> Room
    {
        get { return _room; }
    }
    public void SetRoomOptions(Dictionary<string, object> options)
    {
        roomOptionsDictionary = options;
    }
    public void AddRoomOptions(string optinName, object optionVal)
    {
        roomOptionsDictionary.Add(optinName,optionVal);
    }
    /// <summary>
    ///     Set the client of the <see cref="ColyseusRoomManager" />.
    /// </summary>
    /// <param name="client"></param>
    public void SetClient(ColyseusClient client)
    {
        _client = client;
    }

    /// <summary>
    ///     Create a room with the given roomId.
    /// </summary>
    /// <param name="roomId">The ID for the room.</param>
    public async Task CreateSpecificRoom(ColyseusClient client, string roomName, string roomId)
    {
        Debug.Log($"Creating Room {roomId}");

        try
        {
            //Populate an options dictionary with custom options provided elsewhere as well as the critical option we need here, roomId
            Dictionary<string, object> options = new Dictionary<string, object> { ["roomId"] = roomId};
            foreach (KeyValuePair<string, object> option in roomOptionsDictionary)
            {
                options.Add(option.Key, option.Value);
            }

            _room = await client.Create<State>(roomName, options);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create room {roomId} : {ex.Message}");
            return;
        }

        Debug.Log($"Created Room: {_room.Id}");
        RegisterRoomHandlers();
    }

    /// <summary>
    ///     Join an existing room or create a new one using <see cref="roomName" /> with no options.
    ///     <para>Locked or private rooms are ignored.</para>
    /// </summary>
    public async Task JoinOrCreateRoom(Action<bool> onComplete = null)
    {
        try
        {
            Debug.Log($"Join Or Create Room - Name = {roomName}.... " + _client);

            // Populate an options dictionary with custom options provided elsewhere
            Dictionary<string, object> options = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> option in roomOptionsDictionary)
            {
                options.Add(option.Key, option.Value);
            }

            _room = await _client.JoinOrCreate<State>(roomName, options);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Room Controller Error - {ex.Message + ex.StackTrace}");

            onComplete?.Invoke(false);

            return;
        }

        onComplete?.Invoke(true);
        
        Debug.Log($"Joined / Created Room: {_room.Id}");

        RegisterRoomHandlers();
    }

    public async Task LeaveAllRooms(bool consented, Action onLeave = null)
    {
        if (_room != null && rooms.Contains(_room) == false)
        {
            await _room.Leave(consented);
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            await rooms[i].Leave(consented);
        }

        

        ClearRoomHandlers();

        onLeave?.Invoke();
    }

    /// <summary>
    ///     Subscribes the manager to <see cref="room" />'s networked events
    ///     and starts measuring latency to the server.
    /// </summary>
    /// 

    TicTacToeBoard Board;
    public void SetBoard(TicTacToeBoard board) {
        Board = board;
    }
    public virtual void RegisterRoomHandlers()
    {
      
        GameConfig.rowCount = GameConfig.columnCount = (int)_room.State.boardSize;
        _room.OnLeave += OnLeaveRoom;
        _room.OnStateChange += OnStateChangeHandler;
        _room.colyseusConnection.OnError += Room_OnError;
        _room.colyseusConnection.OnClose += Room_OnClose;
        _room.State.players.OnAdd((key, player) =>
        {
            Debug.Log($"Player {key} has joined the Game!");
        });
        _room.State.board.OnChange((index, val) => {
            GameConfig.rowCount = GameConfig.columnCount = (int)_room.State.boardSize;
            Debug.Log("board size " + _room.State.boardSize);
            Board.FillSlot(Symbol.EMPTY, index, -1, -1, (int)val);
        });
        
        
    }

    private void OnLeaveRoom(int code)
    {
        Debug.Log("ROOM: ON LEAVE =- Reason: " + code);

        _room = null;
    }
     
    private void ClearRoomHandlers()
    {
        if (_room == null)
        {
            return;
        }

        _room.OnStateChange -= OnStateChangeHandler;
        _room.colyseusConnection.OnError -= Room_OnError;
        _room.colyseusConnection.OnClose -= Room_OnClose;
        _room.OnLeave -= OnLeaveRoom;

        _room = null;
    }
    void OnBoardChange(Colyseus.Schema.PropertyChangeHandler<Colyseus.Schema.ArraySchema<float>> board,bool isimediate) { 
    }
    public async Task<ColyseusRoomAvailable[]> GetRoomListAsync()
    {
        ColyseusRoomAvailable[] allRooms = await _client.GetAvailableRooms(roomName);

        return allRooms;
    }

    public async Task JoinRoomId(string roomId, bool isNewJoin)
    {
        ClearRoomHandlers();
        Debug.Log("roomid "+ roomId);
        try
        {
            while (_room == null || !_room.colyseusConnection.IsOpen)
            {
                Dictionary<string, object> options = new Dictionary<string, object>();

                options.Add("joiningId", "test"+UnityEngine.Random.Range(0,100));

                _room = await _client.JoinById<State>(roomId, options);

                if (_room == null || !_room.colyseusConnection.IsOpen)
                {
                    Debug.Log($"Failed to Connect to {roomId}.. Retrying in 5 Seconds...");
                    await Task.Delay(5000);
                }
            }
            Debug.Log($"Connected to {roomId}..");

            RegisterRoomHandlers();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message + ex.StackTrace);
            Debug.LogError("Failed to join room");
        }
    }
    private static void Room_OnClose(int closeCode)
    {
        Debug.LogError("Room_OnClose: " + closeCode);
    }

    /// <summary>
    ///     Callback for when the room get an error.
    /// </summary>
    /// <param name="errorMsg">The error message.</param>
    private static void Room_OnError(string errorMsg)
    {
        Debug.LogError("Room_OnError: " + errorMsg);
    }
    private static void OnStateChangeHandler(State state, bool isFirstState)
    {
        // Setup room first state
        onRoomStateChanged?.Invoke(state, isFirstState);
    }
    public async void CleanUp()
    {
        List<Task> leaveRoomTasks = new List<Task>();

        foreach (IColyseusRoom roomEl in rooms)
        {
            leaveRoomTasks.Add(roomEl.Leave(false));
        }

        if (_room != null)
        {
            leaveRoomTasks.Add(_room.Leave(false));
        }

        await Task.WhenAll(leaveRoomTasks.ToArray());
    }
}
