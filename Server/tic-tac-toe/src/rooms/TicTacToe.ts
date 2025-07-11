import { Room, Delayed, Client } from 'colyseus';
import { State } from "./schema/State";
import { type, Schema, MapSchema, ArraySchema } from '@colyseus/schema';

const TURN_TIMEOUT = 30;
// let BOARD_WIDTH = 3;

export class TicTacToe extends Room<State> {
  maxClients = 2;
  BOARD_WIDTH = 3;
  randomMoveTimeout: Delayed;

  onCreate (options:any) {
    console.log("optin ",options);
    
    this.setState(new State());
    this.state.board = new ArraySchema<number>(this.BOARD_WIDTH);
    this.state.boardSize =options.BOARD_WIDTH;
    
    // if(this.BOARD_WIDTH!= options.BOARD_WIDTH){
      this.BOARD_WIDTH = options.BOARD_WIDTH;
      for(let i=0; i< (this.BOARD_WIDTH*this.BOARD_WIDTH); i++){
        this.state.board[i] = 0;
      }
    // }
    this.onMessage("action", (client, message) => this.playerAction(client, message));
    console.log("Room Created!");
  }

  onJoin (client: Client) {
    console.log("joined");
    this.state.players.set(client.sessionId, true);

    if (this.state.players.size === 2) {
      this.state.currentTurn = client.sessionId;
      this.setAutoMoveTimeout();

      // lock this room for new users
      this.lock();
    }
  }

  playerAction (client: Client, data: any) {
    if (this.state.winner || this.state.draw) {
      return false;
    }
    console.log("current turn ", this.state.currentTurn, "client ",client.sessionId);
    if (client.sessionId === this.state.currentTurn) {

      const playerIds = Array.from(this.state.players.keys());

      const index = data.x* this.BOARD_WIDTH   + data.y;
      console.log(" before index action ", data, index, "board ",this.state.board[index]);

      if (this.state.board[index] === 0) {
        const move = (client.sessionId === playerIds[0]) ? 1 : 2;
        this.state.board[index] = move;

        if (this.checkWin(data.x, data.y, move)) {
          this.state.winner = (client.sessionId === playerIds[0]) ? ""+1 :""+ 2;

        } else if (this.checkBoardComplete()) {
          this.state.draw = true;

        } else {
          // switch turn
          const otherPlayerSessionId = (client.sessionId === playerIds[0]) ? playerIds[1] : playerIds[0];

          this.state.currentTurn = otherPlayerSessionId;

          this.setAutoMoveTimeout();
        }
      console.log("action ", data, index, "board ",this.state.board[index]);

      }
    }
  }

  setAutoMoveTimeout() {
    if (this.randomMoveTimeout) {
      this.randomMoveTimeout.clear();
    }

    this.randomMoveTimeout = this.clock.setTimeout(() => this.doRandomMove(), TURN_TIMEOUT * 1000);
  }

  checkBoardComplete () {
    return this.state.board
      .filter(item => item === 0)
      .length === 0;
  }

  doRandomMove () {
    const sessionId = this.state.currentTurn;
    for (let x=0; x<this.BOARD_WIDTH; x++) {
      for (let y=0; y<this.BOARD_WIDTH; y++) {
        const index = this.BOARD_WIDTH *x +  y;
        if (this.state.board[index] === 0) {
          this.playerAction({ sessionId } as Client, { x, y });
          return;
        }
      }
    }
  }

  checkWin (x: any, y: any, move: any) {
    let won = false;
    let board = this.state.board;

    // horizontal
    for(let y = 0; y < this.BOARD_WIDTH; y++){
      const i = this.BOARD_WIDTH *x +  y;
      if (board[i] !== move) { break; }
      if (y == this.BOARD_WIDTH-1) {
        won = true;
      }
    }

    // vertical
    for(let x = 0; x < this.BOARD_WIDTH; x++){
      const i = this.BOARD_WIDTH *x +  y;
      if (board[i] !== move) { break; }
      if (x == this.BOARD_WIDTH-1) {
        won = true;
      }
    }

    // cross forward
    if(x === y) {
      for(let xy = 0; xy < this.BOARD_WIDTH; xy++){
        const i =  this.BOARD_WIDTH *xy + xy;
        if(board[i] !== move) { break; }
        if(xy == this.BOARD_WIDTH-1) {
          won = true;
        }
      }
    }

    // cross backward
    for(let x = 0;x<this.BOARD_WIDTH; x++){
      const y =(this.BOARD_WIDTH-1)-x;
      const i =  this.BOARD_WIDTH *x + y;
      if(board[i] !== move) { break; }
      if(x == this.BOARD_WIDTH-1){
        won = true;
      }
    }

    return won;
  }

  onLeave (client: Client) {
    this.state.players.delete(client.sessionId);

    if (this.randomMoveTimeout) {
      this.randomMoveTimeout.clear()
    }

    let remainingPlayerIds = Array.from(this.state.players.keys());
    if (remainingPlayerIds.length > 0) {
      this.state.winner = (client.sessionId === playerIds[0]) ? ""+1 :""+ 2;
    }
  }

}