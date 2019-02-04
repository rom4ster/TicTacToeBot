using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
    /** 
     * This class represents a Game, containing a gamestate, owner, and a variable
     * for determining player x. 
     * This wrapper object allows multiple games to go on at once. 
     * Author: Rohan Sampat
     * **/
    class Game
    {
        public Board GameState;
        public string owner;
        public bool HumanPlayer1;

        public Game(Board GameState, string owner, bool HumanPlayer1) {
            this.GameState = GameState;
            this.owner = owner;
            this.HumanPlayer1 = HumanPlayer1;
        }
    }
}
