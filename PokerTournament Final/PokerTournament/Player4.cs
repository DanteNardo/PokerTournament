using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerTournament
{
    class Player4 : Player
    {
        // Constructor
        public Player4(int idNum, string nm, int mny) : base(idNum, nm, mny)
        {
        }

        public override PlayerAction BettingRound1(List<PlayerAction> actions, Card[] hand)
        {
            throw new NotImplementedException();
        }

        public override PlayerAction Draw(Card[] hand)
        {
            throw new NotImplementedException();
        }

        public override PlayerAction BettingRound2(List<PlayerAction> actions, Card[] hand)
        {
            throw new NotImplementedException();
        }
    }
}
