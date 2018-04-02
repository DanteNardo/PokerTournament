using System;
using System.Collections.Generic;

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

        //second round
        int r2MaxRaise = 0;
        // 1 - 10 how agressive the betting will be
        int aggression = 0;

        //value of the highest card in a hand (ex pair)
        int highHandValue = 0;

        //Random numbers
        Random r = new Random();


        //round 2 variables
        static int initialTurn;

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


                    //if we are at the money limit, don't raise to same money for second bet
                    if(atLimit)
                    {
                        return new PlayerAction(name, "Bet1", "check", 0);
                    }

                    //if we are not at our money limit we should try and make bets
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

        public override PlayerAction Draw(Card[] cHand)
        {
            // Create an integer array to represent the hand
            int[] iHand = new int[5]
            {
                CardToInt(cHand[0]),
                CardToInt(cHand[1]),
                CardToInt(cHand[2]),
                CardToInt(cHand[3]),
                CardToInt(cHand[4])
            };

            // Determines if there is potential when drawing 1, 2, or 3 cards
            // Specifically, HasPotential iterates through every possibility of 1-3 cards being changed
            // This DOES NOT cover every single permutation of a hand
            // However, it covers a lot
            bool potential1 = false, potential2 = false, potential3 = false;
            for (int i = 0; i < 5; i++)
            {
                if (HasPotential(1, cHand, iHand, i))
                    potential1 = true;
                if (HasPotential(2, cHand, iHand, i))
                    potential2 = true;
                if (HasPotential(3, cHand, iHand, i))
                    potential3 = true;
            }

            // Determine if we are drawing 1-3 cards, or none
            if (potential3)
            {
                return new PlayerAction(name, "Draw", "draw", 3);
            }
            else if (potential2)
            {
                return new PlayerAction(name, "Draw", "draw", 2);
            }
            else if (potential1)
            {
                return new PlayerAction(name, "Draw", "draw", 1);
            }
            else
            {
                return new PlayerAction(Name, "Draw", "stand pat", 0);
            }
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            //todo gather information about the other player beforehand based on what they are drawing
            //ie if they draw 3 cards and we have a two pair then we have a 2/3 chance of beating them
            //since we know they have one pair
            


            GetSuitsValues(hand);
            Card highCard;

            PlayerAction lastAction = actions[actions.Count - 1];

            int handValue = Evaluate.RateAHand(hand, out highCard);
            
            //how aggressively we will raise/call
            float aggression = 0.0f;
            //compared against aggression, if opponent is not aggressive then we will assume that our hand is stronger above a certain aggression threshold
            float opponentAggression = 0.0f;
            initialTurn = actions.Count;
            //int moneyToBet = 0;
            bool firstPlay = false;

            if (lastAction.ActionName == "stand pat" || lastAction.ActionName == "draw")
            {
                firstPlay = true;
            }

            int pairHigh = -1;
            Card currentCard;
            for (int outer = 0; outer < 5; outer++)
            {
                currentCard = hand[outer];

                for (int inner = 0; inner < 5; inner++)
                {
                    if (outer != inner)
                    {
                        if ((hand[inner].Value == hand[outer].Value) && hand[inner].Value > pairHigh)
                        {
                            pairHigh = hand[inner].Value;
                        }
                    }
                }
            }


            if (handValue >= 8)
            {
                r2MaxRaise = Money;
            }
            else if (handValue < 2 || (handValue == 2 && pairHigh < 14))
            {
                r2MaxRaise = 0;
            }
            else
            {
                r2MaxRaise = 20 + 20 * (handValue - 1);
            }


            //todo change this
            if (firstPlay)
            {
                //
                if ( handValue < 2 ||( handValue == 2 && pairHigh < 14 ))
                {
                    aggression = 0;
                    return new PlayerAction(name, "Bet2", "check", 0);
                }
                //pair of kings or better play conservatively
                else if(handValue == 2 && pairHigh >=14 )
                {
                    aggression = .3f;
                    return new PlayerAction(name, "Bet2", "bet", 10);
                }
                else if( handValue == 3 )
                {
                    if ( pairHigh >= 12 ) //two pair high of 10s
                    {
                        aggression = .7f;
                        return new PlayerAction(name, "Bet2", "bet", 20);
                    }
                    else //two pair high of less than 10s
                    {
                        aggression = 0.5f;
                        return new PlayerAction(name, "Bet2", "bet", 15);
                    }
                }
                else if(handValue ==4 )
                {
                    //3 of a kind 10s or higher
                    if (pairHigh <= 12)
                    {
                        aggression = .9f;
                        return new PlayerAction(name, "Bet2", "bet", 30);
                    }
                    else // 3 of a kind 9s or lower
                    {
                        aggression = .8f;
                        return new PlayerAction(name, "Bet2", "bet", 25);
                    }
                }
                else if(handValue >=5 )
                {
                    aggression = 1.0f;
                }
            }
            else
            {

            }

            return new PlayerAction(name, "Bet1", "fold", 0);
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
            for (int i = 0; i < 4; i++)
            {
                suits[i] = 0;
            }

            for(int i = 0; i < 4; i++)
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

        // Determines whether drawing *change* amount of cards has potential
        private bool HasPotential(int change, Card[] cardHand, int[] intHand, int start)
        {
            // Used for hand rating
            Card c;

            // The maximum numbers of iterations
            int max = (int)Math.Floor(Math.Pow(52, change));

            // The minimum potential necessary (based on iterations) to decide to draw that many cards
            float tolerance = 0.25f;
            int handPotential = Evaluate.RateAHand(cardHand, out c);
            int minPotential = (int)(tolerance * max * handPotential);

            // Initialize variables for finding potential
            int potential = 0;

            // Determines which cards we are currently effecting
            int first = start;
            int second = Next(first, 4);
            int third = Next(second, 4);
            
            // Temporary hands that we can modify
            Card[] tempC = new Card[cardHand.Length];
            cardHand.CopyTo(tempC, 0);
            int[] tempI = new int[intHand.Length];
            intHand.CopyTo(tempI, 0);

            // Acts as multiple for loops by changing index intelligently
            for (int i = 0; i < max; i++)
            {
                // Iterate the first card every iteration
                Iterate(tempI, first);

                // Every 52 iterations, iterate the second card
                if (i % 52 == 0)
                {
                    Iterate(tempI, second);
                }

                // Every 2704 iterations, iterate the third card
                if (i % 2704 == 0)
                {
                    Iterate(tempI, third);
                }

                // Add any new potential from drawing
                tempC[0] = IntToCard(tempI[0]);
                tempC[1] = IntToCard(tempI[1]);
                tempC[2] = IntToCard(tempI[2]);
                tempC[3] = IntToCard(tempI[3]);
                tempC[4] = IntToCard(tempI[4]);
                potential += Clamp(0, 10, Evaluate.RateAHand(tempC, out c) - handPotential);
            }

            // This draw amount only has potential if it is greater than the minimum
            return potential > minPotential;
        }

        // Iterates to the next card that isn't already in the hand
        private void Iterate(int[] hand, int index)
        {
            int next = hand[index] + 1;
            while (true)
            {
                if (next == 52) next = 0;
                if (NotInHand(hand, next))
                {
                    hand[index] = next;
                    return;
                }
                else next++;
            }
        }

        // Determines whether or not a card is already in our hand
        private bool NotInHand(int[] hand, int potential)
        {
            for (int i = 0; i < hand.Length; i++)
            {
                if (hand[i] == potential)
                    return false;
            }

            return true;
        }

        // Easy way to store, creates a unique int for each card
        private int CardToInt(Card c)
        {
            switch (c.Suit)
            {
                case "Spades": return 39 + c.Value;
                case "Diamonds": return 26 + c.Value;
                case "Clubs": return 13 + c.Value;
                case "Hearts": return c.Value;
                default: return 0;
            }
        }

        // Easy way to convert ints to cards
        private Card IntToCard(int c)
        {
            // Hearts
            if (c >= 0 && c <= 12)
            {
                return new Card("Hearts", c);
            }
            // Clubs
            else if (c >= 13 && c <= 25)
            {
                return new Card("Clubs", c - 13);
            }
            // Diamonds
            else if (c >= 26 && c <= 38)
            {
                return new Card("Diamonds", c - 26);
            }
            // Spades
            else if (c >= 39 && c <= 52)
            {
                return new Card("Spades", c - 39);
            }

            return new Card("ERROR", -1);
        }

        // Simple clamping function
        private int Clamp(int min, int max, int value)
        {
            if (value < min) return min;
            if (value > max) return max;

            return value;
        }

        // Returns i++, but such that it won't be greater than a value (loops back to zero)
        private int Next(int i, int cap)
        {
            if (i + 1 >= cap) return 0;
            else              return i + 1;
        }
    }
}
