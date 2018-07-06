using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public static class TwillAI
    {
        /// <summary>
        /// 选可以出的牌
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<byte> ChooseCard(Games game_type, List<byte> cards, List<byte> pre_discard_cards, bool isPartner = false)
        {
            if (pre_discard_cards == null)
            {
                return ChooseCard(game_type, cards);
            }

            List<CombineResult> results = null;
            if (game_type.IsMetallic())
            {
                results = TwillLogic.CheckCombines(pre_discard_cards);
            }
            else
            {
                results = new List<CombineResult>() { TwillLogic.CheckCombine(pre_discard_cards) };
            }

            foreach (var result in results)
            {
                var hand_cards = cards.ConvertToNumber();
                var hand_cards_clone = hand_cards.Clone();

                List<CardEnum> combine_cards = null;
                if (result.combine != Combine.Bomb)
                {
                    var bombs = RemoveBombs(hand_cards);                                //移除炸弹
                    combine_cards = FindCombine(game_type, hand_cards, result);         //获取符合条件牌组
                    if (combine_cards != null)
                    {
                        return GetCards(cards, hand_cards_clone, combine_cards);        //转换为具体的牌
                    }
                }

                //不对对家使用炸弹
                if (isPartner)
                    return null;

                //找炸弹
                combine_cards = FindBomb(game_type, hand_cards_clone, result);
                return GetCards(cards, hand_cards_clone, combine_cards);    //转换为具体的牌
            }

            return null;
        }

        private static List<byte> ChooseCard(Games game_type, List<byte> cards)
        {
            if (cards == null || cards.Count <= 0)
                return null;

            var hand_cards = cards.ConvertToNumber();
            var hand_card_groups = hand_cards.GroupBy(c => c);
            foreach (var card_group in hand_card_groups)
            {
                if (card_group.Count() < 4)
                {
                    return GetCards(cards, hand_cards, card_group.ToList());
                }
            }

            return GetCards(cards, hand_cards, hand_card_groups.First().ToList());
        }

        /// <summary>
        /// 获得具体的牌
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="card_numbers"></param>
        /// <param name="combine_cards"></param>
        /// <returns></returns>
        private static List<byte> GetCards(List<byte> cards, List<CardEnum> card_numbers, List<CardEnum> combine_cards)
        {
            if (combine_cards == null || combine_cards.Count <= 0)
                return null;

            cards = cards.OrderBy(c => c).ToList();
            List<byte> ret = new List<byte>();
            foreach (var combine_card in combine_cards)
            {
                var index = card_numbers.FindIndex(c => c == combine_card);
                card_numbers[index] = CardEnum.Error;
                ret.Add(cards[index]);
            }
            return ret;
        }

        /// <summary>
        /// 移除炸弹牌
        /// </summary>
        /// <param name="hand_cards"></param>
        /// <returns></returns>
        private static List<CardEnum> RemoveBombs(List<CardEnum> hand_cards)
        {
            List<CardEnum> bombs = new List<CardEnum>();
            var group_cards = hand_cards.GroupBy(c => c).OrderBy(c => c.Count());
            foreach (var group_card in group_cards)
            {
                if (group_card.Count() >= 4)
                {
                    //是炸弹
                    bombs.Add(group_card.Key);
                }
            }

            //移除炸弹牌
            foreach (var bomb in bombs)
            {
                hand_cards.RemoveAll(c => c == bomb);
            }

            return bombs;
        }

        /// <summary>
        /// 找适合的组合
        /// </summary>
        /// <param name="hand_cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<CardEnum> FindCombine(Games game_type, List<CardEnum> hand_cards, CombineResult result)
        {
            var hand_cards_clone = hand_cards.Clone();
            List<CardEnum> useful_cards = new List<CardEnum>();
            List<CardEnum> jokers = new List<CardEnum>();
            if (game_type.IsMetallic())
            {
                jokers = hand_cards.FindAll(c => c == CardEnum.LittleJoker || c == CardEnum.ElderJoker);
                hand_cards_clone.RemoveAll(c => c == CardEnum.LittleJoker || c == CardEnum.ElderJoker);
            }

            foreach (var item in hand_cards_clone.FindAll(c => c > result.min_card).GroupBy(c => c))
            {
                switch (result.combine)
                {
                    case Combine.Bridge:        //一张
                        if (item.Count() == 1)
                        {
                            useful_cards.AddRange(item);
                        }
                        break;
                    case Combine.OnePairs:      //一对
                        if (item.Count() == 2)
                        {
                            useful_cards.AddRange(item);
                        }
                        break;
                    case Combine.LinePairs:     //连对
                        if (item.Count() >= 2)
                        {
                            useful_cards.AddRange(item);
                        }
                        break;
                    case Combine.FullHouse:     //三张
                        if (item.Count() >= 3)
                        {
                            useful_cards.AddRange(item);
                        }
                        break;
                    case Combine.LineFullHouse: //连三张
                        if (item.Count() >= 3)
                        {
                            useful_cards.AddRange(item);
                        }
                        break;
                    case Combine.Straight:  //顺子
                        if (item.Count() == 1)
                        {
                            useful_cards.AddRange(item);
                        }
                        break;
                }
            }

            //第一遍，去除大小王，选最佳匹配方式
            if (useful_cards.Count < result.card_num)
            {
                //牌数量不够的情况下，直接加上大小王做匹配
                useful_cards.AddRange(jokers);
                useful_cards = useful_cards.FindAll(c => c > result.min_card);
                if (useful_cards.Count < result.card_num)
                {
                    goto ReCheck;
                }
            }
            var cards = CheckCombine(game_type, useful_cards, result);
            if (cards != null)
                return cards;

            //第二遍，所有符合条件牌都加上
        ReCheck:
            useful_cards = hand_cards.FindAll(c => c > result.min_card);
            if (useful_cards.Count < result.card_num)
            {
                return null;
            }
            return CheckCombine(game_type, useful_cards, result);
        }

        /// <summary>
        /// 检测所有组合
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<CardEnum> CheckCombine(Games game_type, List<CardEnum> cards, CombineResult result)
        {
            var combines = Combination<CardEnum>.GetCombination(cards, result.card_num);
            CombineResult temp;
            foreach (var combine in combines)
            {
                switch (result.combine)
                {
                    case Combine.Bridge:        //一张
                        if(TwillLogic.CheckBridge(game_type, combine, out temp))
                        {
                            return combine;
                        }
                        break;
                    case Combine.OnePairs:      //一对
                        if (TwillLogic.CheckOnePairs(game_type, combine, out temp))
                        {
                            return combine;
                        }
                        break;
                    case Combine.LinePairs:     //连对
                        if (TwillLogic.CheckLinePairs(game_type, combine, out temp))
                        {
                            return combine;
                        }
                        break;
                    case Combine.FullHouse:     //三张
                        if (TwillLogic.CheckFullHouse(game_type, combine, out temp))
                        {
                            return combine;
                        }
                        break;
                    case Combine.LineFullHouse: //连三张
                        if (TwillLogic.CheckLineFullHouse(game_type, combine, out temp))
                        {
                            return combine;
                        }
                        break;
                    case Combine.Straight:  //顺子
                        if (TwillLogic.CheckStraight(game_type, combine, out temp))
                        {
                            return combine;
                        }
                        break;
                } 
            }

            return null;
        }


        /// <summary>
        /// 找炸弹
        /// </summary>
        /// <param name="hand_cards"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static List<CardEnum> FindBomb(Games game_type, List<CardEnum> hand_cards, CombineResult result)
        {
            var hand_cards_clone = hand_cards.Clone();

            List<CardEnum> jokers = new List<CardEnum>();
            if (game_type.IsMetallic())
            {
                jokers = hand_cards_clone.FindAll(c => c == CardEnum.LittleJoker || c == CardEnum.ElderJoker);
                hand_cards_clone.RemoveAll(c => c == CardEnum.LittleJoker || c == CardEnum.ElderJoker);
            }

            var group_cards = hand_cards_clone.GroupBy(c => c).OrderBy(c => c.Count());
            foreach (var group_card in group_cards)
            {
                if (group_card.Count() >= 4)
                {
                    if (result.combine == Combine.Bomb)
                    {
                        //符合条件的炸弹
                        if (group_card.Count() > result.card_num || (group_card.Count() == result.card_num && group_card.Key > result.min_card))
                        {
                            //符合条件炸弹
                            return group_card.ToList();
                        }
                    }
                    else
                    {
                        //炸弹就行
                        return group_card.ToList();
                    }
                }
            }

            if (game_type.IsMetallic())
            {
                foreach (var group_card in group_cards)
                {
                    if (group_card.Count() + jokers.Count() >= 4)
                    {
                        if (result.combine == Combine.Bomb)
                        {
                            if (group_card.Count() + jokers.Count() >= result.card_num && group_card.Key > result.min_card)
                            {
                                //同星级炸弹，牌比较大
                                var cards = group_card.ToList();
                                cards.AddRange(jokers.GetRange(0, result.card_num - group_card.Count()));
                                return cards;
                            }
                            else if (group_card.Count() + jokers.Count() > result.card_num)
                            {
                                //升一个星级
                                var cards = group_card.ToList();
                                cards.AddRange(jokers.GetRange(0, result.card_num - group_card.Count() + 1));
                                return cards;
                            }
                        }
                        else
                        {
                            //炸弹就行
                            var cards = group_card.ToList();
                            cards.AddRange(jokers.GetRange(0, 4 - group_card.Count()));
                            return cards;
                        }
                    }
                }
            }

            return null;
        }
    }
}
