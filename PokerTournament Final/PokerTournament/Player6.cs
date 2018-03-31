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

        //number of cards with a value in a hand
        // 0: 2, 1: 3, 2: 4  ...  king: 11, ace: 12
        int[] values = new int[13];

        //money the player has bet sofar
        int moneyBet = 0;

        //maximum raise the player with take in the first round
        int r1MaxRaise = 0;

        // 1 - 10 how agressive the betting will be
        int aggression = 0;

        //value of the highest card in a hand (ex pair)
        int highHandValue = 0;

        //Random numbers
        Random r = new Random();

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

            //get the suits and values of the cards
            GetSuitsValues(hand);


            Card highCard;
            int handValue = Evaluate.RateAHand(hand, out highCard);

            int moneyAvailable;

            int moneyToBet = 0;

            //if this is the first or second action, it is this players first action of the round
            if (actions.Count <= 1)
            {
                //reset the money bet since it is a new round
                moneyBet = 0;

                r1MaxRaise = 0;

                //determine the maximum bet
                //if the high card is in the low half of cards
                if (highCard.Value < 8)
                {
                    r1MaxRaise = 10 + 20 * (handValue - 1);
                }
                //high half of the values
                else
                {
                    r1MaxRaise = 20 + 20 * (handValue - 1);
                }

                

                //set the aggression level of the Player
                if(handValue == 1)
                {
                    aggression = 1 + (highCard.Value - 2) / 5;
                }
                else if(handValue == 2)
                {
                    //find the value of the pair
                    for(int i = 12; i >= 0; i--)
                    {
                        if(values[i] == 2)
                        {
                            highHandValue = i;
                        }
                    }

                    aggression = 4 + highHandValue / 5;
                }
                else if(handValue == 3)
                {
                    //find the value of the higher pair
                    for (int i = 12; i >= 0; i--)
                    {
                        if (values[i] == 2)
                        {
                            highHandValue = i;
                        }
                    }

                    aggression = 7 + highHandValue / 5;
                }
                else
                {
                    aggression = 10;
                }

                Console.WriteLine("MaxRaise: " + r1MaxRaise);
                Console.WriteLine("Aggression: " + aggression);
                
            }


            

            //check to see if they are going first
            if (actions.Count == 0)
            {
                if(aggression <= 2)
                {
                    return new PlayerAction("Player 6", "Bet 1", "check", 0);
                }
                else
                {
                    Random r = new Random();

                    //high bet
                    if(r.Next(3, 11) <= aggression)
                    {
                        moneyBet += 10;
                        return new PlayerAction("Player 6", "Bet 1", "bet", 10);
                    }
                    //low bet
                    else if(r.Next(0, 4) <= aggression)
                    {
                        moneyBet += 5;
                        return new PlayerAction("Player 6", "Bet 1", "bet", 5);
                    }
                    //check to pass
                    else
                    {
                        return new PlayerAction("Player 6", "Bet 1", "check", 0);
                    }
                }
            }


            //if the player is following an action
            else
            {
                //store the last action
                PlayerAction lastAction = actions[actions.Count - 1];

                //if the last action was a check
                if(lastAction.ActionName == "check")
                {
                    //if the money bet has exceeded 3/4 of the max bet set, check to end the betting
                    if(moneyBet >= r1MaxRaise / 2 + r1MaxRaise / 4)
                    {
                        return new PlayerAction("Player 6", "Bet 1", "check", 0);
                    }

                    //player is not reaching the betting max set
                    else
                    {
                        //get the money avaiable to bet
                        moneyAvailable = r1MaxRaise - moneyBet;

                        //high bet
                        if (r.Next(2, 11) <= aggression && moneyAvailable > 5)
                        {
                            moneyAvailable -= 5;
                            moneyToBet = 5 + r.Next(moneyAvailable / 2, moneyAvailable * 3 / 4);

                            moneyBet += moneyToBet;
                            return new PlayerAction("Player 6", "Bet 1", "bet", moneyToBet);
                        }

                        //low bet
                        else if (r.Next(0, 5) <= aggression && moneyAvailable > 5)
                        {
                            moneyAvailable -= 5;
                            moneyToBet = 5 + r.Next(moneyAvailable / 2);

                            moneyBet += moneyToBet;
                            return new PlayerAction("Player 6", "Bet 1", "bet", moneyToBet);
                        }

                        //check to end the betting
                        else
                        {
                            return new PlayerAction("Player 6", "Bet 1", "check", 0);
                        }
                    }
                }
                else if(lastAction.ActionName == "call")
                {

                }
            }

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


        private void GetSuitsValues(Card[] hand)
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
