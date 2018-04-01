using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    class Player6 : Player
    {
        string name;

        // 0: Hearts, 1: Diamonds, 2: Spades, 3: Clubs
        int[] suits = new int[4];

        //number of cards with a value in a hand
        // 0: 2, 1: 3, 2: 4  ...  king: 11, ace: 12
        int[] values = new int[13];

        //money the player has bet sofar
        int moneyBet = 0;

        //maximum raise the player with take in the first round
        int r1MaxBet = 0;

        bool atLimit = false;

        // 1 - 10 how agressive the betting will be
        int aggression = 0;

        //value of the highest card in a hand (ex pair)
        int highHandValue = 0;

        //Random numbers
        Random r = new Random();

        // Constructor
        public Player6(int idNum, string nm, int mny) : base(idNum, nm, mny)
        {
            name = nm;

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
            //ListTheHand(hand);

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

                r1MaxBet = 0;


                //determine the maximum bet
                //if the high card is in the low half of cards
                if (highCard.Value < 8)
                {
                    r1MaxBet = 5 + 10 * (handValue - 1);
                }
                //high half of the values
                else
                {
                    r1MaxBet = 10 + 10 * (handValue - 1);
                }


                //make sure that we are not exceeding the money that we have when trying to bet
                if(Money < r1MaxBet)
                {
                    r1MaxBet = Money;

                    atLimit = true;
                }
                else
                {
                    atLimit = false;
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
                
            }


            //check to see if they are going first
            if (actions.Count == 0)
            {
                if(aggression <= 2)
                {
                    return new PlayerAction(name, "Bet1", "check", 0);
                }
                else
                {
                    //use random to find the next 
                    //high bet
                    if(r.Next(3, 11) <= aggression)
                    {
                        moneyBet += 10;
                        return new PlayerAction(name, "Bet1", "bet", 10);
                    }
                    //low bet
                    else if(r.Next(1, 4) <= aggression)
                    {
                        moneyBet += 5;
                        return new PlayerAction(name, "Bet1", "bet", 5);
                    }
                    //check to pass
                    else
                    {
                        return new PlayerAction(name, "Bet1", "check", 0);
                    }
                }
            }


            
            

            //if the player is not making the first move, it is following a move from the opponent
            else
            {
                //store the last action
                PlayerAction lastAction = actions[actions.Count - 1];

                ///----------Check------------------------------------------------------------------

                //if the last action was a check
                if (lastAction.ActionName == "check")
                {
                    //if the money bet has exceeded 3/4 of the max bet set, check to end the betting
                    if(moneyBet >= r1MaxBet * 3 / 4)
                    {
                        return new PlayerAction(name, "Bet1", "check", 0);
                    }

                    //if we are at the money limit, don't raise to same money for second bet
                    if(atLimit)
                    {
                        return new PlayerAction(name, "Bet1", "check", 0);
                    }

                    //player is not reaching the betting max set
                    else
                    {
                        //get the money avaiable to bet
                        moneyAvailable = r1MaxBet - moneyBet;

                        //high bet
                        if (r.Next(4, 11) <= aggression && moneyAvailable > 10)
                        {
                            moneyAvailable -= 10;
                            moneyToBet = 10 + r.Next(moneyAvailable / 2, moneyAvailable * 3 / 4);

                            moneyBet += moneyToBet;
                            return new PlayerAction(name, "Bet1", "bet", moneyToBet);
                        }

                        //low bet
                        else if (r.Next(1, 5) <= aggression && moneyAvailable > 5)
                        {
                            moneyAvailable -= 5;
                            moneyToBet = 5 + r.Next(moneyAvailable / 2);

                            moneyBet += moneyToBet;
                            return new PlayerAction(name, "Bet1", "bet", moneyToBet);
                        }

                        //check to end the betting
                        else
                        {
                            return new PlayerAction(name, "Bet1", "check", 0);
                        }
                    }
                }

                ///----------Call------------------------------------------------------------------

                //the opponent called the players raise, the only valid move is to call
                else if (lastAction.ActionName == "call")
                {
                    return new PlayerAction(name, "Bet1", "call", 0);
                }

                ///----------Raise/Bet------------------------------------------------------------------

                //bet and raise are handled the same way
                else if (lastAction.ActionName == "raise" || lastAction.ActionName == "bet")
                {
                    //adjust money the player has bet since there has been a raise/bet
                    moneyBet += lastAction.Amount;

                    //get the amount of money the player has to bet
                    moneyAvailable = r1MaxBet - moneyBet;


                    //check to see if the opponents raise will goes over the max bet set, but if the value bet is insignificant to our maximum, call the bet
                    if (lastAction.Amount > moneyAvailable && lastAction.Amount <= Math.Max(r1MaxBet * .15, 15))
                    {
                        return new PlayerAction(name, "Bet1", "call", 0);
                    }


                    //if we are at our money limit and reaching our raise max, call to end betting to try and win and not lose money from the initial ante
                    else if (moneyAvailable < r1MaxBet * 3 / 4 && atLimit)
                    {
                        return new PlayerAction(name, "Bet1", "call", 0);
                    }


                    //if the amount that the opponent has bet exceeds the limit that has been placed and is significant (isnt caught in the last check), fold the hand
                    else if(lastAction.Amount > moneyAvailable)
                    {
                        return new PlayerAction(name, "Bet1", "fold", 0);
                    }


                    //we still have money to bet and the last amount does not exceed it (caught before reaching here)
                    else
                    {

                        //high bet
                        if (r.Next(3, 11) <= aggression && moneyAvailable > 10)
                        {
                            moneyAvailable -= 10;

                            //adjust the amount to bet by the amount that the opponent placed last turn, so the bet is limited and will not "scare" the opponent
                            //if the bet that would be placed would be too high
                            moneyToBet = 10 + (int)(r.Next(moneyAvailable / 2, moneyAvailable) * Math.Min(((float)lastAction.Amount / (float)moneyAvailable), 1.0f) );

                            moneyBet += moneyToBet;

                            return new PlayerAction(name, "Bet1", "raise", moneyToBet);
                        }

                        //low bet
                        else if (r.Next(1, 5) <= aggression && moneyAvailable > 5)
                        {
                            moneyAvailable -= 5;

                            //adjust the amount to bet by the amount that the opponent placed last turn, so the bet is limited and will not "scare" the opponent
                            //if the bet that would be placed would be too high
                            moneyToBet = 5 + (int)(r.Next(moneyAvailable * 1/4, moneyAvailable / 2) * Math.Min(((float)lastAction.Amount / (float)moneyAvailable), 1.0f));

                            moneyBet += moneyToBet;

                            return new PlayerAction(name, "Bet1", "raise", moneyToBet);
                        }

                        //now decide to call the raise that was made since the player is not placing a high or low bet
                        else
                        {
                            return new PlayerAction(name, "Bet1", "call", 0);
                        }
                    }
                }



                //no fold scenario since a fold will end the round before coming here
            }

            //Console.WriteLine("didnt catch");

            return new PlayerAction(name, "Bet1", "fold", 0);
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
