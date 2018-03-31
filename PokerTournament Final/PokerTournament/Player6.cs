using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    class Player6 : Player
    {
        // 0: Hearts, 1: Diamonds, 2: Spades, 3: Clubs
        int[] suits = new int[4];
        // 0: 2, 1: 3, 2: 4  ...  king: 11, ace: 12
        int[] values = new int[13];


        // Constructor
        public Player6(int idNum, string nm, int mny) : base(idNum, nm, mny)
        {
            for(int i = 0; i < 4; i++)
            {
                suits[i] = 0;
            }

            for (int i = 0; i < 12; i++)
            {
                values[i] = 0;
            }
        }

        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {
            //display the hand
            ListTheHand(hand);



            return new PlayerAction("Player 6", "Bet 1", "check", 0);
        }

        public override PlayerAction Draw(Card[] hand)
        {
            throw new NotImplementedException();
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            throw new NotImplementedException();
        }


        // helper method - list the hand
        private void ListTheHand(Card[] hand)
        {
            // evaluate the hand
            Card highCard = null;
            int rank = Evaluate.RateAHand(hand, out highCard);

            // list your hand
            Console.WriteLine("\nName: " + Name + " Your hand:   Rank: " + rank);
            for (int i = 0; i < hand.Length; i++)
            {
                Console.Write(hand[i].ToString() + " ,");
            }
            Console.WriteLine();
        }


        private void GetSuitValues(Card[] hand)
        {
            for(int i = 0; i < 5; i++)
            {
                values[hand[i].Value - 2] = values[hand[i].Value - 2] + 1;

                switch(hand[i].Suit)
                {
                    case "Hearts":
                        suits[0] = suits[0] + 1;
                        break;
                    case "Diamonds":
                        suits[1] = suits[1] + 1;
                        break;
                    case "Spades":
                        suits[2] = suits[2] + 1;
                        break;
                    case "Clubs":
                        suits[3] = suits[3] + 1;
                        break;
                }
            }
        }
    }
}
