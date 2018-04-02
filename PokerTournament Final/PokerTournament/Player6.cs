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
        float moneyBetR2 = 0.0f;
        float aggressionR2 = 0.0f;

        // 1 - 10 how agressive the betting will be
        int aggression = 0;

        float opponentAggression = 0.0f;
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
            bool potential1 = HasPotential(1, cHand, iHand);
            bool potential2 = HasPotential(2, cHand, iHand);
            bool potential3 = HasPotential(3, cHand, iHand);

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
                return new PlayerAction(name, "Draw", "stand pat", 0);
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
            //compared against aggressionR2, if opponent is not aggressive then we will assume that our hand is stronger above a certain aggressionR2 threshold
            
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


            //Player is playing first in round 2
            if (actions[actions.Count - 1].ActionPhase == "Draw")
            {
                //
                if (handValue < 2 || (handValue == 2 && pairHigh < 14))
                {
                    aggressionR2 = 0;
                    return new PlayerAction(name, "Bet2", "check", 0);
                }
                //pair of kings or better play conservatively
                else if (handValue == 2 && pairHigh >= 14)
                {
                    aggressionR2 = .3f;
                    moneyBetR2 += 10;
                    return new PlayerAction(name, "Bet2", "bet", 10);
                }
                else if (handValue == 3)
                {
                    if (pairHigh >= 12) //two pair high of 10s
                    {
                        aggressionR2 = .7f;
                        moneyBetR2 += 20;
                        return new PlayerAction(name, "Bet2", "bet", 20);
                    }
                    else //two pair high of less than 10s
                    {
                        aggressionR2 = 0.5f;
                        moneyBetR2 += 15;
                        return new PlayerAction(name, "Bet2", "bet", 15);
                    }
                }
                else if (handValue == 4)
                {
                    //3 of a kind 10s or higher
                    if (pairHigh <= 12)
                    {
                        aggressionR2 = .9f;
                        moneyBetR2 += 30;
                        return new PlayerAction(name, "Bet2", "bet", 30);
                    }
                    else // 3 of a kind 9s or lower
                    {
                        aggressionR2 = .8f;
                        moneyBetR2 += 25;
                        return new PlayerAction(name, "Bet2", "bet", 25);
                    }
                }
                else if (handValue >= 5)
                {
                    aggressionR2 = 1.0f;
                    moneyBetR2 += 30;
                    return new PlayerAction(name, "Bet2", "bet", 30);

                }
            }
            //player is playing second in round 2
            else if (actions[actions.Count - 2].ActionPhase == "Draw" && actions[actions.Count - 1].ActionPhase == "Bet2")
            {
                if (lastAction.ActionName == "check")
                {
                    //neutral
                    opponentAggression = .5f;
                    if (aggressionR2 > .5f)
                    {
                        moneyBetR2 += 20;
                        return new PlayerAction(name, "Bet2", "bet", 20);
                    }
                    else
                    {
                        moneyBetR2 += 10;
                        return new PlayerAction(name, "Bet2", "bet", 10);
                    }
                }
                //I'd love to scale aggression based on how much they bet but I'm really not sure how much money is a lot in this game
                //so I opt to scale it based on how the opponent chooses actions instead of the specific values
                else if(lastAction.ActionName == "bet")
                {
                    opponentAggression = .5f;
                    if(aggressionR2 > .5f )
                    {
                        //raise
                        int moneyThisTurn = (int)(((float)r2MaxRaise / 2.0f * (float)aggressionR2));
                        moneyBetR2 += moneyThisTurn;
                        return new PlayerAction(name, "Bet2", "raise", moneyThisTurn);
                    }
                    else if(aggressionR2 > .3f)
                    {
                        //call
                        int moneyThisTurn = lastAction.Amount;
                        moneyBetR2 += moneyThisTurn;
                        return new PlayerAction(name, "Bet2", "call", moneyThisTurn);
                    }
                    else
                    {
                        //fold
                        return new PlayerAction(name, "Bet2", "fold", 0);
                    }

                }
            }
            //not player's first turn in the round
            else
            {

            }

            return null;
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
            for (int i = 0; i < 5; i++)
            {
                suits[i] = 0;
            }

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

        // Determines whether drawing *change* amount of cards has potential
        private bool HasPotential(int change, Card[] cardHand, int[] intHand)
        {
            // Used for hand rating
            Card c;

            // The maximum numbers of iterations
            int max = (int)Math.Floor(Math.Pow(52, change));

            // The minimum potential necessary (based on iterations) to decide to draw that many cards
            float tolerance = 0.1f;
            int handPotential = Evaluate.RateAHand(cardHand, out c);
            int minPotential = (int)(tolerance * max * handPotential);

            // Initialize variables for finding potential
            int potential = 0;

            // Determines which card we are currently effecting
            // Heuristic: We only attempt to swap the weaker cards (bad)
            int first = 0;
            int second = 1;
            int third = 2;
            
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
                potential += Clamp(0, 10, Evaluate.RateAHand(tempC, out c) - handPotential + 1);
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
    }

    class DrawNode
    {
        List<DrawNode> children;
        public int strength;
        public float chance;

        public DrawNode(int strength, float chance)
        {
            this.strength = strength;
            this.chance = chance;
        }
    }
}
