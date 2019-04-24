/*
 * Joshua Diehr
 * 
 * Racko game
 * 
 * play up to 4 people or computers
 * in C#
 */

using System;
using System.Collections.Generic;

namespace Racko
{
    class Racko
    {

        static void Main(string[] args)
        {
            const int TOTALPLAYERS = 4;
            Deck deck = new Deck();
            List<PlayerSystem> players = new List<PlayerSystem>(); // all players

            bool poorInput = true;
            int numHumans = 0;

            while (poorInput) // get total number of people
            {
                Console.WriteLine("How many human players are there? (4 or Less)");
                string input = Console.ReadLine();
                try
                {
                    numHumans = Convert.ToInt32(input);
                }
                catch (Exception)
                {

                    Console.WriteLine("Poor Input");
                }

                if (0 < numHumans && numHumans < 4)
                {
                    poorInput = false;
                }
            }

            for (int i = 1; i <= numHumans; i++)  //Add Human Players
            {
                players.Add(new Player(i+1));
            }

            for (int i = 1; i <= (TOTALPLAYERS - numHumans); i++)  // Add Computer Players using remaining space
            {
                players.Add(new Computer(i));
            }

            for (int i = 0; i < TOTALPLAYERS; i++) // deal all hands
            {
                players[i].GetHand(deck);
            }

            string winner = null;
            int playerNum = 0;

            while (winner == null)
            {
                for (playerNum = 0; playerNum < TOTALPLAYERS; playerNum++)
                {
                    if(players[playerNum].CheckWinner()) // check for winner before each turn
                    {
                        winner = players[playerNum].GetName(); //if there is a winner exit the loop
                        break;
                    }
                    Console.Clear();
                    players[playerNum].TakeTurn(deck);
                    if (players[playerNum].CheckWinner()) // check for winner after each turn
                    {
                        winner = players[playerNum].GetName();
                        break;
                    }
                }
                
            }

            Console.WriteLine($" {players[playerNum].GetName()} has won"); // announce winner
            players[playerNum].DisplayHand(); // show the winners hand
            Console.ReadKey();
        }

    }
    



}
