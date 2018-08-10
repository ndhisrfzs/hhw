using System;
using System.Linq;
using System.Collections.Generic;

namespace Logic
{
    public class RoomInfo
    {
        public int key { get; set; }
        public GameState game_state { get; set; }
        public Games game { get; set; }
        public GameType game_type { get; set; }
        public byte model_type { get; set; }
        public PlayerInfo[] players { get; set; }
        public DiscardInfo discard_info { get; set; }
        public DateTime now { get; set; }

        public RoomInfo() { }

        public RoomInfo(int key, Games game, GameType game_type, byte model_type)
        {
            this.key = key;
            this.game = game;
            this.game_type = game_type;
            this.model_type = model_type;
            this.players = new PlayerInfo[] { null, null, null, null };
        }

        public void Broadcast()
        {
            //requestCallback?.Invoke(this);
            //foreach (var item in requestCallback.GetInvocationList())
            //{
            //    requestCallback -= item as Action<RoomInfo>;
            //} 
        }

        /// <summary>
        /// 玩家排序
        /// </summary>
        /// <param name="uid"></param>
        public void InitSort(long uid)
        {
            SortPlayers(uid);
        }

        /// <summary>
        /// 获取游戏模式
        /// </summary>
        /// <returns></returns>
        public List<ModelType> GetModelType()
        {
            List<ModelType> types = new List<ModelType>();
            if((model_type & (byte)ModelType.BombScore) == (byte)ModelType.BombScore)
            {
                types.Add(ModelType.BombScore);
            }
            if((model_type & (byte)ModelType.BombDouble) == (byte)ModelType.BombDouble)
            {
                types.Add(ModelType.BombDouble);
            }
            if((model_type & (byte)ModelType.ChangePartner) == (byte)ModelType.ChangePartner)
            {
                types.Add(ModelType.ChangePartner);
            }
            return types;
        }

        /// <summary>
        /// player排序
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        private void SortPlayers(long uid)
        {
            int index = 0;
            PlayerInfo[] sort_players = new PlayerInfo[4];
            bool isFind = false;
            for (int i = 0; i < 4; i++)
            {
                if (players[i] != null && uid == players[i].uid)
                {
                    isFind = true;
                    SortCards(players[i]);
                }
                if (isFind)
                {
                    sort_players[index++] = players[i];
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (players[i] != null && uid == players[i].uid)
                {
                    break;
                }

                sort_players[index++] = players[i];
            }

            players = sort_players;
        }

        /// <summary>
        /// 手牌排序
        /// </summary>
        /// <param name="player"></param>
        private void SortCards(PlayerInfo player)
        {
            if(player.show_cards != null && player.show_cards.Count > 1)
                player.show_cards = player.show_cards.OrderByDescending(c => c).ToList();
        }

        /// <summary>
        /// 拷贝自己手牌
        /// </summary>
        /// <param name="uid"></param>
        public void CopyHandCard(long uid)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[i] == null || players[i].hand_cards == null)
                    continue;

                if (players[i].uid != uid)
                {
                    players[i].show_cards = null;
                }
                else
                {
                    players[i].show_cards = players[i].hand_cards;
                }
                players[i].hand_card_num = players[i].hand_cards.Count;
            }
        }

        /// <summary>
        /// 拷贝对家手牌，自己出完手牌后可以看到对家手牌
        /// </summary>
        /// <param name="uid"></param>
        public void CopyPartnerHandCard(long uid)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[i] == null || players[i].hand_cards == null)
                    continue;
                if (players[i].uid != uid)
                {
                    players[i].show_cards = null;
                }
                else
                {
                    players[i].show_cards = players[i].hand_cards;
                };
                players[i].hand_card_num = players[i].hand_cards.Count;
            }

            var partner = GetPartner(uid);
            partner.show_cards = partner.hand_cards;
        }

        /// <summary>
        /// 下一个出牌的玩家
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PlayerInfo NextPlayer(long uid)
        {
            bool isFind = false;
            for (int i = 0; i < 4; i++)
            {
                if (isFind)
                {
                    return players[i];
                }
                if (players[i] != null && uid == players[i].uid)
                {
                    isFind = true;
                }
            }

            return players[0];
        }

        /// <summary>
        /// 获取结束名次
        /// </summary>
        /// <returns></returns>
        public int GetOverIndex()
        {
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!players[i].hand_cards.Any())
                {
                    index++;
                }
            }

            return index;
        }

        /// <summary>
        /// 根据UID获取玩家
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PlayerInfo GetPlayer(long uid)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[i] != null && players[i].uid == uid)
                {
                    return players[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 玩家离开
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool LeavePlayer(long uid)
        {
            if (game_state == GameState.Ready)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (players[i] != null && players[i].uid == uid)
                    {
                        players[i] = null;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 房间是否已空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            for(int i = 0; i < 4; i++)
            {
                if(players[i] != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取对家
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public PlayerInfo GetPartner(long uid)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[i] != null && players[i].uid == uid)
                {
                    return players[(i + 2) % 4];
                }
            }

            return null;
        }

        /// <summary>
        /// 新增玩家
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="head_url"></param>
        /// <returns></returns>
        public bool AddPlayer(long uid, string name, short sex, string head_url)
        {
            for (int i = 0; i < 4; i++)
            {
                if (players[i] == null)
                {
                    players[i] = new PlayerInfo(uid, name, sex, head_url, false, 0);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 前一个出牌的是对家
        /// </summary>
        /// <returns></returns>
        public bool PreDiscardIsPartner()
        {
            var partner = GetPartner(discard_info.cur_uid);
            return partner.uid == discard_info.pre_discard_uid;
        }

        /// <summary>
        /// 获取出牌的玩家
        /// </summary>
        /// <returns></returns>
        public PlayerInfo GetDiscardPlayer()
        {
            return GetPlayer(discard_info.cur_uid);
        }

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            if ((!players[0].hand_cards.Any() && !players[2].hand_cards.Any())
                || (!players[1].hand_cards.Any() && !players[3].hand_cards.Any()))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置分数
        /// </summary>
        public void SetRoundScore()
        {
            var types = GetModelType();
            //算贡献
            if (types.Contains(ModelType.BombScore))
            {
                foreach (var player in players)
                {
                    Contribute(player);
                }
            }
            int partner_1_score = players[0].over_index + players[2].over_index;
            int round_score = Score(partner_1_score);

            //算倍数
            int multiple = 1;
            if (types.Contains(ModelType.BombDouble))
            {
                multiple = GetMultiple();
            }

            players[0].round_score = round_score * multiple + players[0].contribute_score;
            players[1].round_score = -round_score * multiple + players[1].contribute_score;
            players[2].round_score = round_score * multiple + players[2].contribute_score;
            players[3].round_score = -round_score * multiple + players[3].contribute_score;
        }

        /// <summary>
        /// 获取分数倍数
        /// </summary>
        /// <returns></returns>
        private int GetMultiple()
        {
            int winner_index = -1;
            for(int i = 0; i < players.Count(); i++)
            {
                if(players[i].over_index == 1)
                {
                    winner_index = i;
                    break;
                }
            }

            int max_star = 4;
            foreach (var item in players[winner_index].bombs)
            {
                if(item.Key > max_star)
                {
                    max_star = item.Key;
                }
            }
            foreach (var item in players[(winner_index + 2) % 4].bombs)
            {
                if(item.Key > max_star)
                {
                    max_star = item.Key;
                }
            }

            return (int)Math.Pow(2, (max_star - 4));
        }

        /// <summary>
        /// 获取贡献分
        /// </summary>
        /// <param name="player"></param>
        private void Contribute(PlayerInfo player)
        {
            foreach (var bomb in player.bombs)
            {
                if (bomb.Key > 5)
                {
                    int score = (int)Math.Pow(2, (bomb.Key - 5));
                    player.contribute_score += (score * 3);
                    foreach (var item in players)
                    {
                        if (item != player)
                        {
                            item.contribute_score -= score;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 分数
        /// </summary>
        /// <param name="sum_index"></param>
        /// <returns></returns>
        private int Score(int sum_index)
        {
            switch (sum_index)
            {
                case -2:
                    return -3;
                case 0:
                    return 1;
                case 1:
                    return -2;
                case 3:
                    return 3;
                case 4:
                    return 2;
                case 5:
                    return -1;
            }

            throw new Exception("sum_index:" + sum_index + " is error");
        }

        /// <summary>
        /// 是否必须出牌
        /// </summary>
        /// <returns></returns>
        public bool MustDiscard()
        {
            return discard_info.pre_discard_uid == discard_info.cur_uid && GetDiscardPlayer().hand_cards.Any();
        }
    }

    public class PlayerInfo
    {
        public long uid { get; set; }
        public string name { get; set; }
        public short sex { get; set; }
        public string head_url { get; set; }
        public bool is_ready { get; set; }
        public int score { get; set; }
        public int over_index { get; set; }
        public int round_score { get; set; }
        public bool is_ai { get; set; }
        [NonSerialized]
        public List<byte> hand_cards;
        public List<byte> show_cards { get; set; }
        public int hand_card_num { get; set; }
        public Dictionary<byte, byte> bombs { get; set; }
        public int contribute_score { get; set; }

        public PlayerInfo() { }       
        public PlayerInfo(long uid, string name, short sex, string head_url, bool is_ready, int score, bool is_ai = false)
        {
            this.uid = uid;
            this.name = name;
            this.sex = sex;
            this.head_url = head_url;
            this.is_ready = is_ready;
            this.score = score;
            this.is_ai = is_ai;
            this.bombs = new Dictionary<byte, byte>();
            this.contribute_score = 0;
            Init();
        }

        public void Init()
        {
            this.over_index = -1;
            this.round_score = 0;
        }

        public bool CheckCards(byte card, int num)
        {
            return hand_cards.Count(c => c == card) >= num;
        }
        public void RemoveCards(List<byte> cards)
        {
            foreach (var card in cards)
            {
                hand_cards.Remove(card);
            }
        }

        public bool SetOverIndex(int index)
        {
            if (!hand_cards.Any() && over_index <= 0)
            {
                over_index = index;
                return true;
            }

            return false;
        }

        public void AddBombLog(byte star)
        {
            if (bombs.ContainsKey(star))
            {
                bombs[star]++;
            }
            else
            {
                bombs.Add(star, 1);
            }
        }
    }

    public class DiscardInfo
    {
        public long pre_uid { get; set; }
        public long pre_discard_uid { get; set; }
        public List<byte> pre_discard_cards { get; set; }

        public long cur_uid { get; set; }
        public DateTime wait_time { get; set; }
    }
}
