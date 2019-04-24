using System;
using System.Collections;

namespace Racko
{
    class Deck
    {
        const int XCORD = 60; // column adjustment
        Stack cards = new Stack();
        Stack discardPile = new Stack();
        public const int DECKSIZE = 60;

        public Deck()
        {
            for (int i = 0; i < DECKSIZE; i++) // fill the deck
            {
                cards.Push(i + 1);
            }
            Shuffle(); // shuffle it
            StartDiscard();
        }

        private void Shuffle()
        {
            Random rand = new Random();
            int spot;
            ArrayList temp = new ArrayList();


            while (cards.Count > 0) // move all cards to a temporary arraylist
            {
                temp.Add(cards.Pop());
            }

            while (temp.Count > 0) // take random cards out and move them back to the deck
            {
                spot = rand.Next(temp.Count);
                cards.Push(temp[spot]); // the temp arraylist shrinks after each removal keeping the count in bounds
                temp.RemoveAt(spot);
            }
        }

        public int DealCard()
        {
            int top = Convert.ToInt32(cards.Pop()); // take the top card

            if (cards.Count == 0) // if the deck is now empty shuffle in the discard pile
            {
                ReplaceDeck();
                Shuffle();
            } // discard pile will be restarted with the card removed at the end of this turn

            return top;
        }

        private void ReplaceDeck()
        {
            int size = discardPile.Count;
            for (int i = 0; i < size; i++) // push all the cards back into the deck
            {
                cards.Push(discardPile.Pop());
            }
        }

        public void StartDiscard() // take a card from the deck and put it in the discard
        { // should only be needed at the start of the game
            int card = DealCard();
            Discard(card);
        }

        public void Discard(int card) // put a card in the discard pile
        {
            discardPile.Push(card);
        }

        public int TakeDiscard() // take from the discard pile
        {
            return Convert.ToInt32(discardPile.Pop());
        }

        private int Print(string s, int y) // print the given string at a designated postition
        {
            
            Console.SetCursorPosition(XCORD, y);
            Console.WriteLine(s);

            return ++y;
        }
        
        public int PeekDiscard() // look at the top discard without removing it (for computer player)
        {
            return Convert.ToInt32(discardPile.Peek());
        }

        public void DisplayDeck() // prints out the number of cards in the deck and the top discard
        {
            int line = 1; // This is the x cord that the deck pieces start at

            line = Print("    RACKO! ", line);

            // Draw Pile
            line = Print(" ┌─────────┐    ", line);
            line = Print($" │     {String.Format("{0,2}", cards.Count.ToString())}  │     ", line);
            line = Print(" │         │      ", line);
            line = Print(" │         │      ", line);
            line = Print(" │         │      ", line);
            line = Print(" │   Draw  │      ", line);
            line = Print(" │         │      ", line);
            line = Print(" │         │      ", line);
            line = Print(" │         │      ", line);
            line = Print(" └─────────┘      ", line);

            // Discard Pile

            if (discardPile.Count > 0) // if the discard isn't empty show it
            { // should only be empty if your holding its only card

                line = Print(" ┌─────────┐    ", line);
                line = Print($" │       {String.Format("{0,2}", discardPile.Peek().ToString())}│      ", line);
                line = Print(" │         │      ", line);
                line = Print(" │         │      ", line);
                line = Print(" │ Discard │      ", line);
                line = Print(" │  Pile   │      ", line);
                line = Print(" │         │      ", line);
                line = Print(" │         │      ", line);
                line = Print($" │{String.Format("{0,2} ", discardPile.Peek().ToString())}      │      ", line);
                line = Print(" └─────────┘      ", line);

            }

        }

    }
}
