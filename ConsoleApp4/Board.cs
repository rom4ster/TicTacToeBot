using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{

    /**
     * Board is a class that represents a tic tac toe board. There are
     * public and private methods to interface with the board along
     * with method implementations for minimax. The board is stored as 
     * a single integer array. Expected values are -1 for o, 1 for x  and 0 for blank
     * Author: Rohan Sampat
     * **/
    class Board
    {
        private int[] board;
        //Best Successors for minimax use
        Board ABestSucc=null;
        Board BBestSucc=null;

        public Board(int[] board) {
            this.board = board;
        }

        public Board() {
            this.board = new int[9];
        }


        public override string ToString() {
            string s = "";
            for (int i = 0; i< board.Length; i++) {
                if (i % ((int)Math.Sqrt(board.Length)) == 0 && i != 0) s += "\n";
                if (board[i] == 1) s += "[x]";
                if (board[i] == -1) s += "[o]";
                if (board[i] == 0) s += "[ ]";                
            }
            return s;
        }


        public int[] GetBoard() {
            int[] board2 = new int[9];
            Array.Copy(board, board2 , 9);
            return board2;
            
        }
        /**
         * This function generates a list of possible next moves given an
         * integer representation of x or o and a board representing the base
         * state to generate successors from
         * **/
        private List<Board> GetSucc(int val, Board s) { //TODO: remove dependence on base state and use current instance as base state
            int[] myBoard = new int[9];
            var succStates = new List<Board>();
            Array.Copy(s.board, myBoard, myBoard.Length);
            for (int i = 0; i < myBoard.Length; i++) {
                if (myBoard[i] != 0) continue;
                int[] newBoard = new int[9];
                Array.Copy(myBoard, newBoard, newBoard.Length);
                newBoard[i] = val;
                succStates.Add(new Board(newBoard));
            }
            return succStates;
        }
        public bool SetBoard(int pos, int val) {
            if (val != 1 && val != -1) return false;
            board[pos] = val;
            return true;
        }

        /*Debugging "wrapper" function used to publicly access boardcheck also used in check command*/
        public int bc() { return BoardCheck(new Board(this.GetBoard())); }

        /**
         * BoardCheck will take the given board and check to see if there is a winner. It will return the winner as 1 or -1 for
         * x and o respectively. It will return 0 if there is a tie and -2 if there is no tie and no winner. 
         * **/
        private int BoardCheck(Board s) { //TODO: remove dependance on base state and use instance as base state
            int[] boardArr = s.GetBoard();
            int rowLength = (int) Math.Sqrt(boardArr.Length);
            int counter = -1;
            bool win = false;
            int winner = -2;
            //TODO: clean up rows/column code to look more like diagonals 
            //Check rows
            // if the counter is at a new row, check win
            // if there is a win, set winner and break
            // if at end of array, break
            // if win check failed then look at element and previous element and 
            // set win to false if they are not equal
            // repeat while counter <= board.Length
            while (++counter <= boardArr.Length)
            {
                if (counter % rowLength == 0)
                {
                    if (win)
                    {
                        winner = boardArr[counter - 1];
                        break;
                    }
                    if (counter == boardArr.Length) break;
                    if (boardArr[counter] != 0) win = true;
                }
                else
                {
                    win = (boardArr[counter] == boardArr[counter - 1] ? win : false);
                }


            }
            // if there is a winner return it
            if (winner != -2) return winner;
            win = false;

            // Check Columns
            //set j to 0
            // For each row 
            // take the jth element of the row 
            // if it is not equal to its previous row's jth element
            // set win to false;
            // increment j and repeat until j is equal to length of row.
            for (int i = 0; i < rowLength; i++) {
                win = true;
                if (boardArr[i] == 0) win = false;
                for (int j = 1; j < rowLength; j++) {
                    win = (boardArr[i + j*rowLength] == boardArr[i + (j-1)*(rowLength)]) ? win : false;
                }
                if (win)
                {
                    winner = boardArr[i];
                    break;
                }
            }

            //return winner if any
            if (win) return winner;

            winner = -2;
            win = false;

            // check diagonals
            // store each diagonal in an array
            // treat as a row and check if previous element matches current
            // if not update win/winner accordingly
            int[] dpos = new int[rowLength];
            int start = 0;
            for (int i = 0; i < rowLength; i++) {

                dpos[i] = boardArr[start];
                start += (rowLength + 1);
                
            }

            for (int i = 1; i < rowLength; i++)
            {
                if (dpos[i] != dpos[i - 1] || dpos[i] == 0)
                {
                    winner = -2;
                    break;
                }
                else
                {
                    winner = dpos[i];
                }
            } 
            //return if winner
            if (winner != -2) return winner;
            winner = -2;
            win = false;
             start = rowLength - 1;
            int[] pos = new int[rowLength];
            for (int i = 0; i < rowLength; i++)
            {

                pos[i] = boardArr[start];
                start += rowLength - 1;
               

            }
            for (int i = 1; i < rowLength; i++)
            {
                if (pos[i] != pos[i - 1] || pos[i] == 0)
                {
                    winner = -2;
                    break;
                }
                else
                {
                    winner = pos[i];
                }
            }
            //return if winner
            if (winner != -2) return winner;

            //check to see if board is full and return appropriate code else board is 
            // full and tie is in place, return appropriate code 
            foreach (int i in s.GetBoard()) if (i == 0) return -2;
            return 0;

            



        }

        /**
         * Maximizer in Mini-Max algorithm this function will check to see
         * if the board has a winner/tie. If there is winner or tie, then the function will
         * set the appropriate successor in this case ABestSucc and return that value
         * **/
        private int Maximizer() { //TODO: implement Alpha Beta Pruning
            int GTValue = BoardCheck(this);
            if (GTValue != -2)
            {
                ABestSucc = this;
                return GTValue;
            }
            int? alpha = null;
         
            // for every successor
            // set alpha to a greater value (if there is one) 
            // update best successor if there exists a greater value
            // use the Minimizer() function to get the opposing value and use that to 
            // do the previous two steps. 
            foreach (Board succ in GetSucc(1, this)) {
                int? old = alpha;
                if (alpha == null) ABestSucc = succ;
                alpha = (alpha == null ? succ.Minimizer() :  Math.Max((int)alpha, succ.Minimizer()));
                if (alpha == null || (alpha == 0 && old != alpha)) ABestSucc = succ;
                if (alpha == 1) {
                    ABestSucc = succ;
                    break;
                }
            }

            return (int)alpha;
            

        }
        /**
      * Minimizer in Mini-Max algorithm this function will check to see
      * if the board has a winner/tie. If there is winner or tie, then the function will
      * set the appropriate successor in this case BBestSucc and return that value
      * **/
        private int Minimizer()
        { //TODO: implement Alpha Beta Pruning
            int GTValue = BoardCheck(this);
            if (GTValue != -2)
            {
                BBestSucc = this;
                return GTValue;
                
            }
            int? beta = null;
            // for every successor
            // set beta to a lesser value (if there is one) 
            // update best successor if there exists a lesser value
            // use the Maximizer() function to get the opposing value and use that to 
            // do the previous two steps. 
            foreach (Board succ in GetSucc(-1, this))
            {
                int? old = beta;
                if (beta == null) BBestSucc = succ;
                beta = (beta == null ? succ.Maximizer() : Math.Min((int)beta, succ.Maximizer()));
                if (beta == null || (beta == 0 && old != beta)) BBestSucc = succ;
                if (beta == -1) {
                    BBestSucc = succ;
                    break;
                }
                
            }

            return (int)beta;
        }

        /**
         * Signifies a move done by the player, the function must know if 
         * the player is the x player (player1 == true) or not and what 
         * part of the board the player would like to use for their move
         * **/
        public bool PlayerMove(bool player1, int move) {
            //determine x or o
            int turn = player1 ? 1 : -1;
            //set position player requests
            this.board[move] = turn;
            // check for winner/tie
            return BoardCheck(this) == turn || BoardCheck(this) == 0;
        }

        /**
         * Signifies move done by the AI. The function must know if 
         * the AI is x player (player1==true) or not. It will pick the 
         * best move for the AI. 
         * **/
        public bool AiMove(bool player1) {
            //determine x or o
            int turn = player1 ? 1 : -1;
            // run minimax appropriate function based on x or o
            if (player1) Maximizer(); else Minimizer();
            // use appropriate variable to change the board.
            board = (turn == 1 ? ABestSucc.board : BBestSucc.board);
            //check for winner/tie
            return BoardCheck(this) == turn || BoardCheck(this) == 0;
        }

        /* checks for draw game*/
        public bool DrawGame() {
            return BoardCheck(this) == 0;
        }

        /*Checks to see if given move is an actual possible move*/
        public bool Valid(int move) {
            return board[move] == 0;
        }


    }
}
