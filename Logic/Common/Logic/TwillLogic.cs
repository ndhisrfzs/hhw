using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class CombineResult
    {
        public CombineResult(Combine combine, byte star, CardEnum min_card, byte card_num)
        {
            this.combine = combine;
            this.star = star;
            this.min_card = min_card;
            this.card_num = card_num;
        }

        public Combine combine;
        public byte star;
        public CardEnum min_card;
        public byte card_num;

        public static bool operator <(CombineResult lhs, CombineResult rhs)
        {
            return (lhs.combine != Combine.Bomb && rhs.combine == Combine.Bomb)                                     //左边不是炸弹，右边是炸弹
                || (lhs.combine == Combine.Bomb && rhs.combine == Combine.Bomb && ((lhs.star < rhs.star)            //同是炸弹,比星数
                || (lhs.star == rhs.star && lhs.card_num > rhs.card_num)                                            //同是炸弹,星数相同，比牌数量，数量少的大
                || (lhs.star == rhs.star && lhs.card_num == rhs.card_num && lhs.min_card < rhs.min_card)))          //同是炸弹,星数相同，比牌数量，数量相同，比牌大小 
                || (lhs.combine == rhs.combine && lhs.card_num == rhs.card_num && lhs.min_card < rhs.min_card);     //相同组合，比最小牌大小
        }
        public static bool operator >(CombineResult lhs, CombineResult rhs)
        {
            return false;
        }
    }

    public static class TwillLogic
    {
        static List<CardEnum> Jokers = new List<CardEnum>(){ CardEnum.LittleJoker, CardEnum.ElderJoker };
        public static List<CombineResult> CheckCombines(List<byte> cards)
        {
            List<CombineResult> results = new List<CombineResult>();
            if (cards == null || cards.Count <= 0)
            {
                return results;
            }

            var cards_number = cards.ConvertToNumber();

            CombineResult result = null;
            if (CheckJokerBomb(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
                return results;
            }
            if (CheckBomb(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
                return results;
            }

            if (CheckBridge(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
            }
            if (CheckOnePairs(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
            }
            if (CheckFullHouse(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
            }
            if (CheckLinePairs(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
            }
            if (CheckLineFullHouse(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
            }
            if (CheckStraight(Games.Metallic, cards_number, out result))
            {
                results.Add(result);
            }

            return results;
        }

        public static CombineResult CheckCombine(List<byte> cards)
        {
            CombineResult result = null;
            if (cards == null || cards.Count <= 0) {
                return result;
            }

            var cards_number = cards.ConvertToNumber();
            if (CheckBridge(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if (CheckOnePairs(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if (CheckFullHouse(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if (CheckBomb(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if(CheckJokerBomb(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if (CheckLinePairs(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if (CheckLineFullHouse(Games.Classical, cards_number, out result))
            {
                return result;
            }
            else if (CheckStraight(Games.Classical, cards_number, out result))
            {
                return result;
            }

            return result;
        }

        /// <summary>
        /// 桥牌
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckBridge(Games game_type, List<CardEnum> cards, out CombineResult result) 
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count == 1)
            {
                result = new CombineResult(Combine.Bridge, 0, cards_clone[0], (byte)cards_clone.Count);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 一对
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckOnePairs(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count == 2 && 
                (cards_clone[0] == cards_clone[1] || 
                (game_type.IsMetallic() && cards_clone.Exists(c=> Jokers.Contains(c))   //千变时只要存在大小王都可以算对子
                )))    
            {
                result = new CombineResult(Combine.OnePairs, 0, cards_clone[0], (byte)cards_clone.Count);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 三张
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckFullHouse(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count != 3)
                return false;

            if (cards_clone[0] == cards_clone[1] && cards_clone[0] == cards_clone[2])
            {
                result = new CombineResult(Combine.FullHouse, 0, cards_clone[0], (byte)cards_clone.Count);
                return true;
            }

            if(game_type.IsMetallic())
            {
                //千变规则
                int joker_count = cards_clone.Count(c => Jokers.Contains(c));
                if(joker_count >= 2 || (joker_count == 1 && cards_clone[0] == cards_clone[1]))
                {
                    result = new CombineResult(Combine.FullHouse, 0, cards_clone[0], (byte)cards_clone.Count);
                    return true;
                }
            }


            return false;
        }

        /// <summary>
        /// 炸弹
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckBomb(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count < 4)
                return false;

            if(game_type.IsMetallic() && cards_clone.Count() >= 12)
            {
                //大于12张牌，连炸可能性比较大，先判断连炸，不是判断普通炸
                if(CheckLineBomb(game_type, cards_clone, out result))
                {
                    return true;
                }
            }

            CardEnum base_card = cards_clone[0];
            foreach (var card in cards_clone)
            {
                if (!(card == base_card || (game_type.IsMetallic() && Jokers.Contains(card))))
                    return false;
            }

            result = new CombineResult(Combine.Bomb, (byte)cards_clone.Count, cards_clone[0], (byte)cards_clone.Count);
            return true;
        }

        /// <summary>
        /// 连炸
        /// </summary>
        /// <param name="game_type"></param>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckLineBomb(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            int cards_count = cards_clone.Count();
            if (cards_count >= 12)
            {
                CardEnum min_card = cards_clone[0];
                int joker_count = 0;
                if (game_type.IsMetallic())
                {
                    joker_count = cards_clone.RemoveAll(c => Jokers.Contains(c));
                }

                var card_groups = cards_clone.GroupBy(c => c).ToList();
                List<int> bomb_count = new List<int>();
                CardEnum next_card = cards_clone[0];
               
                for (int i = 0; i < card_groups.Count; i++)
                {
                    if(next_card == card_groups[i].First())
                    {
                        int group_count = card_groups[i].Count();
                        if(group_count >= 4)
                        {
                            next_card = NextCardNumber(next_card);
                            bomb_count.Add(group_count);
                        }
                        else if(joker_count >= 4 - group_count)
                        {
                            joker_count -= (4 - group_count);
                            next_card = NextCardNumber(next_card);
                            bomb_count.Add(4);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if(joker_count >= 4)
                    {
                        joker_count -= 4;
                        i -= 1;
                        next_card = NextCardNumber(next_card);
                        bomb_count.Add(4);
                    }
                    else
                    {
                        return false;
                    }
                }

                
                result = new CombineResult(Combine.Bomb, (byte)GetLineBombStar(bomb_count, joker_count), min_card, (byte)cards_count);
                return true;
            }

            return false;
        }

        private static int GetLineBombStar(List<int> bomb_count, int last_joker_count)
        {
            for(int i = 0; i < last_joker_count; i++)
            {
                bomb_count = bomb_count.OrderBy(c => c).ToList();
                bomb_count[0] += 1;
            }

            bomb_count = bomb_count.OrderBy(c => c).ToList();
            return bomb_count[0] + bomb_count.Count;
        }

        private static CardEnum[] joker_bomb_cards = new CardEnum[] { CardEnum.LittleJoker, CardEnum.LittleJoker, CardEnum.ElderJoker, CardEnum.ElderJoker };
        /// <summary>
        /// 王炸
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckJokerBomb(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            var cards_clone = cards.Clone();
            result = null;
            if (game_type.IsMetallic() && cards_clone.Count == 3)
            {
                if(cards_clone.Count(c=> Jokers.Contains(c)) == 3)
                {
                    //6星王炸中最小的
                    result = new CombineResult(Combine.Bomb, 5, cards_clone[0], (byte)cards_clone.Count);
                    return true;
                }
            }
            else if(cards_clone.Count == 4)
            {
                if (cards_clone.Count(c => Jokers.Contains(c)) == 4)
                {
                    if (game_type.IsMetallic())
                    {
                        //7星王炸
                        result = new CombineResult(Combine.Bomb, 7, cards_clone[0], (byte)cards_clone.Count);
                    }
                    else
                    {
                        //传统模式天王炸最大
                        result = new CombineResult(Combine.Bomb, 8, cards_clone[0], (byte)cards_clone.Count);
                    }
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 连对
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckLinePairs(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count >= 6 && cards_clone.Count % 2 == 0)
            {
                CardEnum min_card = cards_clone[0];
                byte card_count = (byte)cards_clone.Count;

                int joker_count = 0;
                if (game_type.IsMetallic())
                {
                    joker_count = cards_clone.RemoveAll(c => Jokers.Contains(c));
                }

                int count = cards_clone.Count;
                CardEnum next_card = cards_clone[0];
                for (int i = 0; i < cards_clone.Count; i += 2)
                {
                    if (i < count && cards_clone[i] == next_card)
                    {
                        if (i + 1 < count && cards_clone[i + 1] == next_card)
                        {
                            next_card = NextCardNumber(next_card);
                        }
                        else if (joker_count >= 1)
                        {
                            --joker_count;
                            --i;
                            next_card = NextCardNumber(next_card);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if(joker_count >= 2)
                    {
                        joker_count -= 2;
                        i -= 2;
                        next_card = NextCardNumber(next_card);
                    }
                    else
                    {
                        return false;
                    }
                }

                result = new CombineResult(Combine.LinePairs, 0, min_card, card_count);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 连三张
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckLineFullHouse(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count >= 9 && cards_clone.Count % 3 == 0)
            {
                CardEnum min_card = cards_clone[0];
                byte card_count = (byte)cards_clone.Count;

                int joker_count = 0;
                if (game_type.IsMetallic())
                {
                    joker_count = cards_clone.RemoveAll(c => Jokers.Contains(c));
                }

                int count = cards_clone.Count;
                CardEnum next_card = cards_clone[0];
                for (int i = 0; i < cards_clone.Count; i += 3)
                {
                    if (i < count && cards_clone[i] == next_card)
                    {
                        if(i + 1 < count && cards_clone[i + 1] == next_card)
                        {
                            if(i + 2 < count && cards_clone[i + 2] == next_card)
                            {
                                next_card = NextCardNumber(next_card);
                            }
                            else if(joker_count >= 1)
                            {
                                joker_count -= 1;
                                i -= 1;
                                next_card = NextCardNumber(next_card);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if(joker_count >= 2)
                        {
                            joker_count -= 2;
                            i -= 2;
                            next_card = NextCardNumber(next_card);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if(joker_count >= 3)
                    {
                        joker_count -= 3;
                        i -= 3;
                        next_card = NextCardNumber(next_card);
                    }
                    else
                    {
                        return false;
                    }
                }

                result = new CombineResult(Combine.LineFullHouse, 0, min_card, card_count);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 顺子
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckStraight(Games game_type, List<CardEnum> cards, out CombineResult result)
        {
            result = null;
            var cards_clone = cards.Clone();
            if (cards_clone.Count >= 5)
            {
                CardEnum min_card = cards_clone[0];
                byte card_count = (byte)cards_clone.Count;

                int joker_count = 0;
                if (game_type.IsMetallic())
                {
                    joker_count = cards_clone.RemoveAll(c => Jokers.Contains(c));
                }

                CardEnum next_card = cards_clone[0];
                for (int i = 0; i < cards_clone.Count; i++)
                {
                    if (cards_clone[i] == next_card)
                    {
                        next_card = NextCardNumber(next_card);
                    }
                    else if(joker_count >= 1)
                    {
                        joker_count -= 1;
                        i -= 1;
                        next_card = NextCardNumber(next_card);
                    }
                    else
                    {
                        return false;
                    }
                }

                result = new CombineResult(Combine.Straight, 0, min_card, card_count);
                return true;
            }

            return false;
        }

        public static CardEnum NextCardNumber(CardEnum card)
        {
            card += 10;
            return card;
        }

        public static CardEnum CardNumber(byte card)
        {
            return (CardEnum)(card / 10 * 10);
        }

        public static CardEnum CardHuase(byte card)
        {
            return (CardEnum)(card % 10);
        }

        public static List<CardEnum> ConvertToNumber(this List<byte> cards)
        {
            List<CardEnum> ret = new List<CardEnum>();
            foreach (var card in cards)
            {
                ret.Add(CardNumber(card)); 
            }

            return ret.OrderBy(c => c).ToList();
        } 

        public static List<CardEnum> Clone(this List<CardEnum> cards)
        {
            List<CardEnum> ret = new List<CardEnum>();
            foreach (var card in cards)
            {
                ret.Add(card);
            }

            return ret;
        }
        
        public static bool IsMetallic(this Games type)
        {
            return type == Games.Metallic;
        }
    }
}
