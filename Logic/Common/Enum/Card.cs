using System.Collections.Generic;
namespace Logic
{
    public enum CardEnum : byte
    {
        Error = 0,
        Diamond = 1,    //方块
        Club = 2,       //梅花
        Hert = 3,       //红心
        Spade = 4,      //黑桃

        Three = 30,
        Four = 40,
        Five = 50,
        Six = 60,
        Seven = 70,
        Eight = 80,
        Nine = 90,
        Ten = 100,
        Jack = 110,
        Queen = 120,
        King = 130,
        Ace = 140,

        Two = 160,

        LittleJoker = 180,    //小王
        ElderJoker = 190      //大王
    }

    public enum Combine : byte
    {
        Error = 0,          //错误牌型
        Bridge = 1,         //桥牌
        OnePairs = 2,       //一对
        LinePairs = 3,      //连对
        FullHouse = 4,      //三张
        LineFullHouse = 5,  //连三张
        Straight = 6,       //顺子
        Bomb = 7,           //炸弹
    }

    public static class TwillCard
    {
        public static List<byte> cards = new List<byte>()
        {
            CardEnum.Three.ToCard(CardEnum.Diamond), CardEnum.Three.ToCard(CardEnum.Club), CardEnum.Three.ToCard(CardEnum.Hert), CardEnum.Three.ToCard(CardEnum.Spade),
            CardEnum.Three.ToCard(CardEnum.Diamond), CardEnum.Three.ToCard(CardEnum.Club), CardEnum.Three.ToCard(CardEnum.Hert), CardEnum.Three.ToCard(CardEnum.Spade),
            CardEnum.Four.ToCard(CardEnum.Diamond), CardEnum.Four.ToCard(CardEnum.Club), CardEnum.Four.ToCard(CardEnum.Hert), CardEnum.Four.ToCard(CardEnum.Spade),
            CardEnum.Four.ToCard(CardEnum.Diamond), CardEnum.Four.ToCard(CardEnum.Club), CardEnum.Four.ToCard(CardEnum.Hert), CardEnum.Four.ToCard(CardEnum.Spade),
            CardEnum.Five.ToCard(CardEnum.Diamond), CardEnum.Five.ToCard(CardEnum.Club), CardEnum.Five.ToCard(CardEnum.Hert), CardEnum.Five.ToCard(CardEnum.Spade),
            CardEnum.Five.ToCard(CardEnum.Diamond), CardEnum.Five.ToCard(CardEnum.Club), CardEnum.Five.ToCard(CardEnum.Hert), CardEnum.Five.ToCard(CardEnum.Spade),
            CardEnum.Six.ToCard(CardEnum.Diamond), CardEnum.Six.ToCard(CardEnum.Club), CardEnum.Six.ToCard(CardEnum.Hert), CardEnum.Six.ToCard(CardEnum.Spade),
            CardEnum.Six.ToCard(CardEnum.Diamond), CardEnum.Six.ToCard(CardEnum.Club), CardEnum.Six.ToCard(CardEnum.Hert), CardEnum.Six.ToCard(CardEnum.Spade),
            CardEnum.Seven.ToCard(CardEnum.Diamond), CardEnum.Seven.ToCard(CardEnum.Club), CardEnum.Seven.ToCard(CardEnum.Hert), CardEnum.Seven.ToCard(CardEnum.Spade),
            CardEnum.Seven.ToCard(CardEnum.Diamond), CardEnum.Seven.ToCard(CardEnum.Club), CardEnum.Seven.ToCard(CardEnum.Hert), CardEnum.Seven.ToCard(CardEnum.Spade),
            CardEnum.Eight.ToCard(CardEnum.Diamond), CardEnum.Eight.ToCard(CardEnum.Club), CardEnum.Eight.ToCard(CardEnum.Hert), CardEnum.Eight.ToCard(CardEnum.Spade),
            CardEnum.Eight.ToCard(CardEnum.Diamond), CardEnum.Eight.ToCard(CardEnum.Club), CardEnum.Eight.ToCard(CardEnum.Hert), CardEnum.Eight.ToCard(CardEnum.Spade),
            CardEnum.Nine.ToCard(CardEnum.Diamond), CardEnum.Nine.ToCard(CardEnum.Club), CardEnum.Nine.ToCard(CardEnum.Hert), CardEnum.Nine.ToCard(CardEnum.Spade),
            CardEnum.Nine.ToCard(CardEnum.Diamond), CardEnum.Nine.ToCard(CardEnum.Club), CardEnum.Nine.ToCard(CardEnum.Hert), CardEnum.Nine.ToCard(CardEnum.Spade),
            CardEnum.Ten.ToCard(CardEnum.Diamond), CardEnum.Ten.ToCard(CardEnum.Club), CardEnum.Ten.ToCard(CardEnum.Hert), CardEnum.Ten.ToCard(CardEnum.Spade),
            CardEnum.Ten.ToCard(CardEnum.Diamond), CardEnum.Ten.ToCard(CardEnum.Club), CardEnum.Ten.ToCard(CardEnum.Hert), CardEnum.Ten.ToCard(CardEnum.Spade),
            CardEnum.Jack.ToCard(CardEnum.Diamond), CardEnum.Jack.ToCard(CardEnum.Club), CardEnum.Jack.ToCard(CardEnum.Hert), CardEnum.Jack.ToCard(CardEnum.Spade),
            CardEnum.Jack.ToCard(CardEnum.Diamond), CardEnum.Jack.ToCard(CardEnum.Club), CardEnum.Jack.ToCard(CardEnum.Hert), CardEnum.Jack.ToCard(CardEnum.Spade),
            CardEnum.Queen.ToCard(CardEnum.Diamond), CardEnum.Queen.ToCard(CardEnum.Club), CardEnum.Queen.ToCard(CardEnum.Hert), CardEnum.Queen.ToCard(CardEnum.Spade),
            CardEnum.Queen.ToCard(CardEnum.Diamond), CardEnum.Queen.ToCard(CardEnum.Club), CardEnum.Queen.ToCard(CardEnum.Hert), CardEnum.Queen.ToCard(CardEnum.Spade),
            CardEnum.King.ToCard(CardEnum.Diamond), CardEnum.King.ToCard(CardEnum.Club), CardEnum.King.ToCard(CardEnum.Hert), CardEnum.King.ToCard(CardEnum.Spade),
            CardEnum.King.ToCard(CardEnum.Diamond), CardEnum.King.ToCard(CardEnum.Club), CardEnum.King.ToCard(CardEnum.Hert), CardEnum.King.ToCard(CardEnum.Spade),
            CardEnum.Ace.ToCard(CardEnum.Diamond), CardEnum.Ace.ToCard(CardEnum.Club), CardEnum.Ace.ToCard(CardEnum.Hert), CardEnum.Ace.ToCard(CardEnum.Spade),
            CardEnum.Ace.ToCard(CardEnum.Diamond), CardEnum.Ace.ToCard(CardEnum.Club), CardEnum.Ace.ToCard(CardEnum.Hert), CardEnum.Ace.ToCard(CardEnum.Spade),
            CardEnum.Two.ToCard(CardEnum.Diamond), CardEnum.Two.ToCard(CardEnum.Club), CardEnum.Two.ToCard(CardEnum.Hert), CardEnum.Two.ToCard(CardEnum.Spade),
            CardEnum.Two.ToCard(CardEnum.Diamond), CardEnum.Two.ToCard(CardEnum.Club), CardEnum.Two.ToCard(CardEnum.Hert), CardEnum.Two.ToCard(CardEnum.Spade),
            CardEnum.LittleJoker.ToCard(CardEnum.Error), CardEnum.LittleJoker.ToCard(CardEnum.Error), CardEnum.ElderJoker.ToCard(CardEnum.Error), CardEnum.ElderJoker.ToCard(CardEnum.Error)
        };

        public static List<byte> CloneCards() {
            List<byte> clone_cards = new List<byte>();
            foreach (var card in cards)
            {
                clone_cards.Add(card); 
            }
            return clone_cards;
        }

        public static byte ToCard(this CardEnum card, CardEnum huase)
        {
            return (byte)((byte)card + (byte)huase);
        }
    }
}