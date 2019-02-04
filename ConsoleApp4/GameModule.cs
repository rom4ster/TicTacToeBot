using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace ConsoleApp4
{


    /**
     * Bot Command Module for Tic Tac Toe
     * Author: Rohan Sampat
     * **/
    public class GameModule : ModuleBase
    {
        /**
         * This command creates a new game and will make first move if human chooses to be o
         * **/
        [Command("play"), Summary("Start A new game")]
        public async Task Play([Remainder, Summary("Which Player")] string player) { //TODO support different size boards
            //TODO change to hashset/map
            // Search for duplicate games 
            foreach (Game game in Program.Games) if (game.owner == "" + Context.User.Id) return;
            //create new game
            bool hp1;
            if (player == "x") hp1 = true; else hp1 = false;
            Game g = new Game(new Board(), "" + Context.User.Id, hp1);
            if (!hp1) g.GameState.AiMove(true);
            //add game to Game List
            Program.Games.Add(g);
            //Print output
            //TODO Make helper function for this and build string dynamically
            await Context.Channel.SendMessageAsync(Context.User.Username + ", U HAVE CREATED A NEW GAME");
            if (!hp1)
            {
                await Context.Channel.SendMessageAsync(Context.User.Username + "AI has moved printing current board...");
                await Context.Channel.SendMessageAsync(g.GameState.ToString());
            }


        }

       
        /**
         * This command is used to signify the human player playing either x or o on an open position
         * **/
        [Command("place"), Summary("Make A Move")]
        public async Task Place([Remainder, Summary("What Part Of Board to place on 1-9")] string coordinate) {
            // Take number and validate input as a number between 1 and 9
            //TODO make number determinent on game size. 
            int num;
            
            if (!int.TryParse(coordinate, out num) || num < 1 || num > 9)
            {
                await Context.Channel.SendMessageAsync(Context.User.Username + ", Incorrect Syntax-- parameter not between 0 and 9 or not a number -- Board Layout is a 3x3 Board: Row 1 [ [1] [2] [3]] Row 2 [ [4] [5] [6]] Row 3 [ [7] [8] [9] ]");
                return;
            }
            // subtract 1 to convert to 0 based index
            num--;
            //Find game pertaining to command sender and send error message
            //TODO helper function for errors
            Game g = null;
            foreach (Game game in Program.Games) {
                //Console.WriteLine(game.owner);
                if (game.owner == "" + Context.User.Id) g = game;
            }
            //Console.WriteLine(Context.User.Id);
            if (g == null) { await Context.Channel.SendMessageAsync(Context.User.Username + ", No Game Found, please create one iwth the play command"); return; }
            // if requested move is valid have player play move
            // check for winner
            // have AI play move
            // check for winner
            // Print appropriately if winner
            // print board if no winner
            // TODO: helper functions for printing
            if (g.GameState.Valid(num))
            {
                bool PlayerEnd, AIEnd = false;
                 PlayerEnd = g.GameState.PlayerMove(g.HumanPlayer1, num);
                if (PlayerEnd || AIEnd)
                {
                    string winner = PlayerEnd ? "HUMAN" : "AI";
                    if (g.GameState.DrawGame()) await Context.Channel.SendMessageAsync(Context.User.Username + ", YOUR GAME HAS ENDED IN A DRAW, YOU DID NOT WIN... BUT AI DID NOT BEAT YOU");
                    else await Context.Channel.SendMessageAsync(Context.User.Username + ", YOUR GAME HAS ENDED AND THE WINNER IS: " + winner + ". YOU " + (PlayerEnd ? "WON" : "LOST") + ".");
                    Program.Games.Remove(g);
                    await Context.Channel.SendMessageAsync(g.GameState.ToString());
                    return;
                }
                AIEnd = g.GameState.AiMove(!g.HumanPlayer1);
                if (PlayerEnd || AIEnd)
                {
                    string winner = PlayerEnd ? "HUMAN" : "AI";
                    if (g.GameState.DrawGame()) await Context.Channel.SendMessageAsync(Context.User.Username + ", YOUR GAME HAS ENDED IN A DRAW, YOU DID NOT WIN... BUT AI DID NOT BEAT YOU");
                    else await Context.Channel.SendMessageAsync(Context.User.Username + ", YOUR GAME HAS ENDED AND THE WINNER IS: " + winner + ". YOU " + (PlayerEnd ? "WON" : "LOST") + ".");
                    Program.Games.Remove(g);
                    await Context.Channel.SendMessageAsync(g.GameState.ToString());
                    return;
                }

            }
            await Context.Channel.SendMessageAsync(Context.User.Username + ", Your move has been recorded, AI has played its turn, printing board...");
            await Context.Channel.SendMessageAsync(g.GameState.ToString());








        }



        /*This utility command is mostly for debugging. It will print the winner of a given board as a string.
         * The winner will be 1 or -1. If it is a tie 0 is printed, if the game is incomplete -2 will be printed. 
         */
        [Command("check"), Summary("Debug Utility")]
        public async Task GetWinner([Remainder] string inboard) {
            string[] inarr = inboard.Trim().Split(' ');
            int[] b = new int[9];
            for (int i = 0; i < b.Length; i++) {
                b[i] = int.Parse(inarr[i]);

               
            }
            Board dboard = new Board(b);
            await Context.Channel.SendMessageAsync("" + dboard.bc());
        }

    }
}
