using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    /*
     * This program will run a five card draw poker tournament
     * for the Game AI course. This program will use separate PlayerN
     * classes (with N being the team number) for each team whose methods 
     * will be called for decisions.
     * Kevin Bierre  Spring 2017
     * 
     * Version 3    4/4/2017:
     *  Corrected a logic error when folding
     *  Corrected a logic error in processing a call correctly
     *  
     * Version 4    4/4/2017
     *  Corrected a logic error where the pot was not being reset in a couple of cases
     *  Corrected the handling of calls during betting. If a player calls,
     *      nobody else can bet.
     *  Open Issue: is A2345 a valid straight?
     */
    class Program
    {
        static void Main(string[] args)
        {
            //------------------------------------
            // Old testing code
            // create two players
            Human h0 = new Human(0, "Joe", 1000);
            //Human h1 = new Human(1, "Sue", 1000);
            //------------------------------------
            // create the player objects
            //Player1 one = new Player1(1, "Player1",1000);
            //Player2 two = new Player2(2, "Player2", 1000);
            //Player3 three = new Player3(3, "Player3", 1000);
            //Player4 four = new Player4(4, "Player4", 1000);
            //Player5 five = new Player5(5, "Player5", 1000);
            Player6 six = new Player6(6, "Player6", 1000);
            //Player7 seven = new Player7(7, "Player7", 1000);
            //Player8 eight = new Player8(8, "Player8", 1000);
            //Player9 nine = new Player9(9, "Player9", 1000);
            //Player10 ten = new Player10(10, "Player10", 1000);

            // Select the two players for this part of the tournament
            Player p0 = h0;

            Player p1 = six;


            /*
            int num1 = 0;
            do
            {
                Console.Write("Select the first player (1 - 10): ");
                string num1Str = Console.ReadLine();
                int.TryParse(num1Str, out num1);
            } while (num1 <= 0 || num1 > 10);

            switch(num1)
            {
                case 1: p0 = one; break;
                case 2: p0 = two; break;
                case 3: p0 = three; break;
                case 4: p0 = four; break;
                case 5: p0 = five; break;
                case 6: p0 = six; break;
                case 7: p0 = seven; break;
                case 8: p0 = eight; break;
                case 9: p0 = nine; break;
                case 10: p0 = ten; break;
            }

            int num2 = 0;
            do
            {
                Console.Write("Select the second player (1 - 10): ");
                string num2Str = Console.ReadLine();
                int.TryParse(num2Str, out num2);
            } while (num2 <= 0 || num2 > 10);

            switch (num2)
            {
                case 1: p1 = one; break;
                case 2: p1 = two; break;
                case 3: p1 = three; break;
                case 4: p1 = four; break;
                case 5: p1 = five; break;
                case 6: p1 = six; break;
                case 7: p1 = seven; break;
                case 8: p1 = eight; break;
                case 9: p1 = nine; break;
                case 10: p1 = ten; break;
            }

            */

            // create the Game
            Game myGame = new Game(h0, six);

            myGame.Tournament(); // run the game
        }
    }
}
