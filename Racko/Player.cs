using System;
using System.Collections;

namespace Racko
{
    class PlayerSystem
    {
        protected const int XCORD = 40; // column adjustment
        protected const int SPOTADJUSTMENT = 1; // for switching between user input and array spot
        public ArrayList hand = new ArrayList();
        public const int HANDSIZE = 10;
        protected string name;

        public PlayerSystem()
        {

        }

        public string GetName()
        {
            return name;
        }

        public void GetHand(Deck deck) // deal hand
        {
            for (int i = 0; i < HANDSIZE; i++)
            {
                hand.Add(deck.DealCard());
            }
        }

        private int Print(string s1, int y) // prints a single line at the designated spot
        {

            Console.SetCursorPosition(XCORD, y);
            Console.WriteLine(s1);

            return ++y;
        }

        private int Print(string s1, string s2, int y) // prints two lines namely the card and slot
        {

            Console.SetCursorPosition(XCORD, y);
            Console.WriteLine(s1);
            Console.SetCursorPosition(XCORD, ++y);
            Console.WriteLine(s2);

            ++y;
            return y;
        }

        public void DisplayHand() // uses the print methods to display a players board and cards
        {
            int line = 1;

            line = Print("       Hand ", line);

            line = Print(" ┌──────────────┐       ", line);

            for (int i = 0; i < HANDSIZE; i++)
            {
                line = Print(" │  ┌──────────┐│       ", $" │{String.Format("{0,2}", (i + 1).ToString())}│        {String.Format("{0,2}", hand[i].ToString())}││       ", line);
            }

            line = Print(" └──────────────┘       ", line);

        }

        public bool CheckWinner() // checks if all cards are in proper order
        {
            for (int i = 0; i < HANDSIZE - 1; i++) // compare each card to the next
            {
                if (Convert.ToInt32(hand[i]) < Convert.ToInt32(hand[i + 1]))
                {
                    return false; // exit if its wrong
                }
            }
            return true;
        }

        public virtual void TakeTurn(Deck deck) // meant to be overridden by derived classes
        {

        }
    }

    class Player : PlayerSystem
    {
        public Player(int input) // create player and set their name
        {
            name = "Player" + input;
        }

        public int Swap(int incoming, Deck deck) // take the given card and a user input to swap cards
        {
            string input;
            int spot = 0;
            bool poorInput = true;

            while (poorInput) // ask for a spot and check if its a proper value
            {
                Console.Clear();
                DisplayHand();
                deck.DisplayDeck();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Which slots would you like \nto swap with? (1-10)");
                input = Console.ReadLine();

                try
                {
                    spot = Convert.ToInt32(input);
                }
                catch(Exception)
                {
                    Console.WriteLine("Enter a proper number");
                }

                if (0 < spot && spot <= HANDSIZE)
                {
                    poorInput = false;
                }
                
            }

            int card = Convert.ToInt32(hand[spot - SPOTADJUSTMENT]); // take card out of board
            hand[spot - SPOTADJUSTMENT] = incoming; // put new card in
            return card; // return old card
        }

        public override void TakeTurn(Deck deck)
        {
            string input;
            int cardTaken = 0;

            bool poorInput = true;

            while (poorInput) // ask for a command
            {
                Console.Clear();
                DisplayHand();
                deck.DisplayDeck();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("Type: 1 to draw from deck\nor 2 to take the face-up card");
                input = Console.ReadLine();
                if (input == "1")
                {
                    cardTaken = deck.DealCard();
                    poorInput = false; // exit loop if command is good
                }
                else if (input == "2")
                {
                    cardTaken = deck.TakeDiscard();
                    poorInput = false; // exit loop if command is good
                }
            }

            poorInput = true;

            while (poorInput)
            {
                Console.Clear();
                DisplayHand();
                deck.DisplayDeck();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Do you want to keep? {cardTaken}\n1 for yes 2 for no");
                input = null;
                input = Console.ReadLine();
                if (input == "1") // ask if they want to keep the card they took
                {
                    deck.Discard(Swap(cardTaken, deck)); // swap card if they want it then discard the removed card
                    poorInput = false;
                }
                else if (input == "2")
                {
                    deck.Discard(cardTaken); // discard if they dont
                    poorInput = false;
                }
            }
        }
    }

    class Computer : PlayerSystem
    {
        int[] topValue = new int[HANDSIZE]; // create boundaries for each slot
        int[] bottomValue = new int[HANDSIZE];
        bool[] groupFilled = new bool[HANDSIZE]; // determine if the boundaries are satisfied

        public Computer(int input) // create name and set up the boundary system
        {
            name = "Computer" + input;
            GroupSetUp();
        }

        public void GroupSetUp()
        {
            int interval = Deck.DECKSIZE / HANDSIZE; // determine the spacing based on hand slots and number of cards in deck

            for (int i = 0; i < HANDSIZE; i++)
            {
                topValue[i] = Deck.DECKSIZE - (interval * i); // max value for this slot
                bottomValue[i] = topValue[i] - interval; // min value for this slot
            }
        }

        public override void TakeTurn(Deck deck)
        {
            int spot;
            CheckFilled(); // check which spots already have a good card

            int card = deck.PeekDiscard(); // check what the discard is

            spot = OptimalSpot(card); // check for IDEAL fit
            if (spot == HANDSIZE) // Didn't find it
            {
                spot = GroupingSpot(card); // check for an OK fit
            }

            if (spot != HANDSIZE) // If there was a good spot for the discard
            {
                card = deck.TakeDiscard(); // take discard
                deck.Discard(Swap(card, spot));// switch cards
                return; // next player
            }

            if (spot == HANDSIZE) //discard wasn't useful
            {
                card = deck.DealCard(); // take from the draw pile
                spot = OptimalSpot(card);
            }
            if (spot == HANDSIZE)
            {
                spot = GroupingSpot(card);
            }
            if (spot == HANDSIZE)
            {
                deck.Discard(card); // if there wasn't a good spot for this card discard it
                return;
            }

            deck.Discard(Swap(card, spot));// if the drawn card was useful switch it
        }

        private void CheckFilled() // check if slots are filled with a card within the slots boundaries
        {
            for (int i = 0; i < HANDSIZE; i++)
            {
                if (topValue[i] >= Convert.ToInt32(hand[i]) && Convert.ToInt32(hand[i]) > bottomValue[i])
                {
                    groupFilled[i] = true;
                }
            }
        }

        private int GroupingSpot(int card) // check if the given card is a good fit in an available slot
        {
            for (int i = 0; i < HANDSIZE; i++)
            {
                if (topValue[i] >= card && card > bottomValue[i] && !groupFilled[i])
                {
                    return i; // return the good spot
                }
            }
            return HANDSIZE; // return an out of bounds spot
        }

        private int OptimalSpot(int card) // check if the card can fit between two slots even if it isn't within boundaries
        {
            const int GROUPSIZE = 2;

            for (int i = 0; i < HANDSIZE - GROUPSIZE; i++)
            {
                if (Convert.ToInt32(hand[i]) > card && card > Convert.ToInt32(hand[i+GROUPSIZE]) && groupFilled[i] && groupFilled[i+GROUPSIZE])
                {
                    return ++i; // return the good spot
                }
            }

            return HANDSIZE; // return out of bounds if there wasn't a good spot
        }

        public int Swap(int incoming, int spot) // use the given spot to swap the new card with the old one
        {
            int card = Convert.ToInt32(hand[spot]);
            hand[spot] = incoming;
            return card;
        }
    }
}
