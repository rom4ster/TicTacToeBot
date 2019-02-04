using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.IO;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Collections.Generic;

namespace ConsoleApp4
{

    /**
     * This is the main class of the program. This class handles tasks such as token input, client setup and other
     * important tasks that ensure a bot can connect to discord, this class will save the token to a file 
     * and read it in. This applications supports every token type but not all are shown in the UI. 
     * This is done to guide users while not limiting them. 
     * Author: Rohan Sampat
     * **/
    class Program
    {
        // TODO token deletion UI option, windows forums or ASP.NET GUI and token encryption
        // TODO Add warning to user for tokentype user. <-- This is in accordance with my policy of not limiting user but giving them full information
        
        // Discord connection variables
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;
        // Debug switch  // TODO: Implement UI control for Debug Switch 
        private bool DebugMode = false;
        // Token variables 
        private TokenType BotTokenType;
        private string Token;
        // Game List Variable for use in GameModule.cs
        public static List<Game> Games = new List<Game>();

        /* Redirect Main to MainAsync */
        static void Main(string[] args) =>  new Program().MainAsync().GetAwaiter().GetResult();

        /**
         * This is the main function of the program
         * It will handle file reading for tokens and
         * Setting up the connection
         * **/
        public async Task MainAsync() {
    
            // Check for existing settings file, create one if necessary
            bool valid = true;
            if (!File.Exists("./botsettings.txt"))
            {
                (File.Create("./botsettings.txt")).Dispose();
                valid = false;
            }
            // if file is valid, aka not created, 
            // read in token as string 
            // read in token type 
            FileStream f = new FileStream("./botsettings.txt", FileMode.Open, FileAccess.ReadWrite);
            if (valid)
            {
                StreamReader fs = new StreamReader(f);
                string line = fs.ReadLine();
                Token = line.Trim();
                line = fs.ReadLine();
                int lineInt = int.Parse(line.Trim());
                BotTokenType = (TokenType)0;
                foreach (TokenType t in Enum.GetValues(typeof(TokenType))) BotTokenType = ((int)t == lineInt ? t : BotTokenType);
                fs.Dispose();
            }
            // In case of created file, ask user for token and bot type
            else {
                StreamWriter fs = new StreamWriter(f);
                string s = "";
                await Log("Enter Token ");
                s += Console.ReadLine();
                s += "\n";
                await Log("Enter Token Type - 0 for user and 2 for bot ");
                s += Console.ReadLine();
                fs.WriteLine(s);
                fs.Dispose();
            }
            //Console.WriteLine(Token + "hello");

            // initiate connection variables
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            // Subscribe to Logging and Command handling
            client.Log += Log;
            client.MessageReceived += HandleCommand;
            //Install commands
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            //Start Client
            await client.LoginAsync(BotTokenType, Token);
            await client.StartAsync();
            await Task.Delay(-1);
        }


        /*Simple function to handle log messages as string*/
        private Task Log(string message) {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
        /*Simple function to handle log messages from the discord client*/
        private Task Log(LogMessage message) {
            Log("[LOG] " +message.ToString());
            return Task.CompletedTask;
        }
        /*A debug message function, only used if global switch is activated, essentially a Log function*/
        private Task Debug(string message) {
            if (DebugMode) Log("[DEBUG] " + message);
            return Task.CompletedTask;
        }

        /**
         * This function determines if a message is a command, if it is
         * then the message is passed to the command service. 
         * **/
        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            int argPos =0;
            if (!(message.HasCharPrefix('}', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}