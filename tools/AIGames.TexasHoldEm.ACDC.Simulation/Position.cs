using System.Xml.Serialization;

namespace AIGames.TexasHoldEm.ACDC.Simulation
{
    public class Position
    {
        [XmlAttribute("id")]
        public int Game { get; set; }
        [XmlAttribute("r")]
        public int Round { get; set; }
        [XmlAttribute("r")]
        public SubRoundType SubRound { get; set; }

        [XmlAttribute("odd")]
        public decimal Odds { get; set; }
   
        [XmlAttribute("blind")]
        public int BigBlind { get; set; }
        [XmlAttribute("call")]
        public int Call { get; set; }

        [XmlAttribute("s0")]
        public int Stack0 { get; set; }
        [XmlAttribute("s1")]
        public int Stack1 { get; set; }

        [XmlAttribute("act")]
        public GameActionType ActionType { get; set; }

        [XmlAttribute("a")]
        public int Amount { get; set; }

        [XmlAttribute("win")]
        public int Win { get; set; }
    }
}
