using GN;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Logic
{
    public interface IMatcher
    {
        long uid { get; set; }
        int score { get; set; }
        Games game { get; set; }
        GameType game_type { get; set; }
        byte model_type { get; set; }
    }

    public class MatchData<T>
        where T : class, IMatcher
    {
        public T data;
        public DateTime time;
        public MatchData(T data)
        {
            this.data = data;
            this.time = DateTime.Now;
        }
    }

    public class Matcher : IMatcher
    {
        public long uid { get; set; }
        public string name { get; set; }
        public short sex { get; set; }
        public string head_url { get; set; }
        public int score { get; set; }
        public Games game { get; set; }
        public GameType game_type { get; set; }
        public byte model_type { get; set; }
    }

    public class RoomManagerComponent : Component
    {
        private ConcurrentDictionary<int, RoomInfo> m_Hall = new ConcurrentDictionary<int, RoomInfo>();
        private ConcurrentDictionary<long, int> m_playings = new ConcurrentDictionary<long, int>();

        protected void Start()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                foreach (var room in m_Hall)
                {
                    if (room.Value.discard_info != null && room.Value.discard_info.wait_time <= now)
                    {
                        List<byte> cards = null;
                        if (room.Value.MustDiscard())
                        {
                            cards = TwillAI.ChooseCard(room.Value.game, room.Value.GetDiscardPlayer().hand_cards, null);
                        }
                        NextPlayer(room.Value, cards);
                    }
                }
                Task.Delay(1000);
            }
        }

        public Task<RoomInfo> WaitRoomInfoUpdate(int roomKey, long uid)
        {
            RoomInfo roomInfo;
            if (m_Hall.TryGetValue(roomKey, out roomInfo))
            {
                var tcs = new TaskCompletionSource<RoomInfo>();
                roomInfo.requestCallback += (response) =>
                {
                    try
                    {
                        tcs.SetResult(response);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                };

                return tcs.Task;
            }
            return null;
        }

        private void AIDiscard(RoomInfo room_info)
        {
            System.Timers.Timer timerClock = new System.Timers.Timer();
            timerClock.Elapsed += new ElapsedEventHandler((source, e) => {
                var player = room_info.GetPlayer(room_info.discard_info.cur_uid);
                List<byte> cards = null;
                if (room_info.discard_info.pre_discard_uid <= 0 || room_info.discard_info.pre_discard_uid == room_info.discard_info.cur_uid)
                {
                    cards = TwillAI.ChooseCard(room_info.game, player.hand_cards, null);
                }
                else
                {
                    cards = TwillAI.ChooseCard(room_info.game, player.hand_cards, room_info.discard_info.pre_discard_cards, room_info.PreDiscardIsPartner());
                }

                if (cards != null)
                    player.RemoveCards(cards);

                NextPlayer(room_info, cards);
            });
            timerClock.Interval = 1000;
            timerClock.AutoReset = false;
            timerClock.Enabled = true;
        }

        /// <summary>
        /// 初始化房间，匹配模式接口
        /// </summary>
        /// <param name="players"></param>
        public void InitRoom<T>(Queue<MatchData<T>> players)
            where T : class, IMatcher
        {
            var first_player = players.Peek();
            RoomInfo room_info = new RoomInfo(Game.Scene.GetComponent<RoomKeyComponent>().Key, first_player.data.game, first_player.data.game_type, first_player.data.model_type);

            for (int i = 0; i < 4; i++)
            {
                var data = players.Count > 0 ? players.Dequeue() : null;
                if (data != null)
                {
                    var player_info = data.data as Matcher;
                    room_info.players[i] = new PlayerInfo(player_info.uid, player_info.name, player_info.sex, player_info.head_url, true, 0);
                }
                else
                {
                    room_info.players[i] = new PlayerInfo(i, "我是AI", 0, "", true, 0, true);
                }
            }

            if (AddPlaying(room_info) && m_Hall.TryAdd(room_info.key, room_info))
            {
                DealCards(room_info);
            }
        }

        /// <summary>
        /// 初始化房间
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="name"></param>
        /// <param name="sex"></param>
        /// <param name="head_url"></param>
        /// <param name="game"></param>
        /// <param name="game_type"></param>
        /// <param name="model_type"></param>
        /// <returns></returns>
        public bool InitRoom(long uid, string name, short sex, string head_url, Games game, GameType game_type, byte model_type)
        {
            RoomInfo room_info = new RoomInfo(Game.Scene.GetComponent<RoomKeyComponent>().Key, game, game_type, model_type);

            if (!AddPlaying(uid, room_info.key))
                return false;

            if (!room_info.AddPlayer(uid, name, sex, head_url))
            {
                return false;
            }

            if (m_Hall.TryAdd(room_info.key, room_info))
            {
                SyncRoomInfo(room_info, GameState.Ready);
            }

            return true;
        }

        public bool EnterRoom(long uid, string name, short sex, string head_url, int key)
        {
            var room_info = GetRoomInfo(key);
            if (room_info == null)
                return false;

            if (!AddPlaying(uid, room_info.key))
                return false;

            if (!room_info.AddPlayer(uid, name, sex, head_url))
            {
                return false;
            }

            SyncRoomInfo(room_info, GameState.Ready);

            return true;
        }

        public bool Ready(long uid)
        {
            var room_info = GetRoomInfo(uid);
            if (room_info == null)
                return false;

            var player = room_info.GetPlayer(uid);
            if (player == null)
                return false;

            player.is_ready = true;

            //准备
            SyncRoomInfo(room_info, GameState.Ready);

            if (CheckGameStart(room_info))
            {
                //四个人都准备，游戏开始，发牌
                DealCards(room_info);
            }

            return true;
        }

        public bool Leave(long uid)
        {
            var room_info = GetRoomInfo(uid);
            if (room_info == null)
                return false;

            bool ret = room_info.LeavePlayer(uid);
            if (ret)
            {
                RemovePlaying(uid);
                if (room_info.IsEmpty())
                {
                    m_Hall.TryRemove(room_info.key, out room_info);
                }
            }

            return ret;
        }

        /// <summary>
        /// 发牌
        /// </summary>
        /// <param name="room_info"></param>
        private void DealCards(RoomInfo room_info)
        {
            var cards = TwillCard.CloneCards();
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                room_info.players[i].hand_cards = GetHandCard(cards, ref index);
            }

            RandomDiscardPlayer(room_info);
            SyncRoomInfo(room_info, GameState.DealCard);
        }

        /// <summary>
        /// 检测是否都准备了
        /// </summary>
        /// <param name="room_info"></param>
        /// <returns></returns>
        public bool CheckGameStart(RoomInfo room_info)
        {
            for (int i = 0; i < 4; i++)
            {
                if (room_info.players[i] == null || room_info.players[i].is_ready == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Discard(long uid, List<byte> cards)
        {
            RoomInfo room_info = GetRoomInfo(uid);
            if (room_info != null)
            {
                var player = room_info.GetPlayer(uid);
                var card_groups = cards.GroupBy(c => c);
                foreach (var card_group in card_groups)
                {
                    if (!player.CheckCards(card_group.Key, card_group.Count()))
                    {
                        return false;
                    }
                }

                bool bRet = CheckRule(room_info, uid, cards);
                if (bRet)
                {
                    player.RemoveCards(cards);
                    NextPlayer(room_info, cards);
                }

                return bRet;
            }

            return false;
        }

        public void Pass(long uid)
        {
            var room_info = GetRoomInfo(uid);
            if (room_info != null)
            {
                NextPlayer(room_info, null);
            }
        }

        private bool CheckRule(RoomInfo room_info, long uid, List<byte> cards)
        {
            if (room_info.game.IsMetallic())
            {
                //千变双扣
                var cur_results = TwillLogic.CheckCombines(cards);
                if (room_info.discard_info.pre_discard_uid == uid && cur_results.Any())
                {
                    return true;
                }

                var pre_results = TwillLogic.CheckCombines(room_info.discard_info.pre_discard_cards);
                if (!pre_results.Any() && cur_results.Any())
                {
                    return true;
                }


                if (pre_results.Any() && cur_results.Any())
                {
                    foreach (var pre_result in pre_results)
                    {
                        foreach (var cur_result in cur_results)
                        {
                            if (pre_result < cur_result)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                //传统双扣
                var cur_result = TwillLogic.CheckCombine(cards);
                if (room_info.discard_info.pre_discard_uid == uid && cur_result != null)
                {
                    return true;
                }

                var pre_result = TwillLogic.CheckCombine(room_info.discard_info.pre_discard_cards);
                if (pre_result == null && cur_result != null)
                {
                    return true;
                }

                if (pre_result != null && cur_result != null)
                {
                    if (pre_result < cur_result)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RandomDiscardPlayer(RoomInfo room_info)
        {
            int index = Game.Scene.GetComponent<RandomComponent>().rand.Next(4);
            room_info.discard_info = new DiscardInfo()
            {
                cur_uid = room_info.players[index].uid,
                wait_time = DateTime.Now.AddSeconds(30)
            };

            if (room_info.players[index].is_ai)
                AIDiscard(room_info);
        }

        public bool IsPlaying(long uid)
        {
            return m_playings.ContainsKey(uid);
        }

        private bool AddPlaying(long uid, int key)
        {
            return m_playings.TryAdd(uid, key);
        }
        private bool AddPlaying(RoomInfo room_info)
        {
            int key = room_info.key;
            for (int i = 0; i < 4; i++)
            {
                if (!room_info.players[i].is_ai && !m_playings.TryAdd(room_info.players[i].uid, key))
                {
                    for (int n = 0; n < i; n++)
                    {
                        m_playings.TryRemove(room_info.players[n].uid, out key);
                    }

                    return false;
                }
            }

            return true;
        }

        private void RemovePlaying(RoomInfo room_info)
        {
            int key = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!room_info.players[i].is_ai)
                {
                    m_playings.TryRemove(room_info.players[i].uid, out key);
                }
            }
        }

        private void RemovePlaying(long uid)
        {
            int key = 0;
            m_playings.TryRemove(uid, out key);
        }

        /// <summary>
        /// 设置出牌结束顺序
        /// </summary>
        /// <param name="room_info"></param>
        private void SetOverIndex(RoomInfo room_info)
        {
            var cur_player = room_info.GetPlayer(room_info.discard_info.cur_uid);
            if (!cur_player.hand_cards.Any())
            {
                int over_index = room_info.GetOverIndex();
                cur_player.SetOverIndex(over_index);
            }
        }

        private void NextPlayer(RoomInfo room_info, List<byte> cards)
        {
            room_info.discard_info.pre_uid = room_info.discard_info.cur_uid;
            if (cards != null)
            {
                SetOverIndex(room_info);
                room_info.discard_info.pre_discard_uid = room_info.discard_info.cur_uid;
                room_info.discard_info.pre_discard_cards = cards;
                //设置炸弹记录
                SetBombLog(room_info);
            }

            PlayerInfo player = room_info.NextPlayer(room_info.discard_info.cur_uid);
            while (!player.hand_cards.Any())
            {
                //上一个出手牌的人是自己,并且自己没手牌了
                if (player.uid == room_info.discard_info.pre_discard_uid)
                {
                    //找到对家
                    player = room_info.GetPartner(player.uid);
                    if (!player.hand_cards.Any())
                    {
                        //对家也没手牌了，游戏结束
                        GameOver(room_info);
                        return;
                    }
                    //对家出牌
                    room_info.discard_info.pre_discard_uid = player.uid;
                    room_info.discard_info.pre_discard_cards = null;
                }
                else
                {
                    //不是自己则跳过
                    player = room_info.NextPlayer(player.uid);
                }
            }

            room_info.discard_info.cur_uid = player.uid;
            room_info.discard_info.wait_time = DateTime.Now.AddSeconds(30);

            if (room_info.IsGameOver())
            {
                //游戏结束
                GameOver(room_info);
                return;
            }

            SyncRoomInfo(room_info, GameState.Discard);

            if (player.is_ai)
                AIDiscard(room_info);
        }

        private void SetBombLog(RoomInfo room_info)
        {
            if (room_info.game.IsMetallic())
            {
                var results = TwillLogic.CheckCombines(room_info.discard_info.pre_discard_cards);
                foreach (var result in results)
                {
                    if (result.combine == Combine.Bomb)
                    {
                        var player = room_info.GetPlayer(room_info.discard_info.pre_discard_uid);
                        player.AddBombLog(result.star);
                    }
                }
            }
            else
            {
                var result = TwillLogic.CheckCombine(room_info.discard_info.pre_discard_cards);
                if (result.combine == Combine.Bomb)
                {
                    var player = room_info.GetPlayer(room_info.discard_info.pre_discard_uid);
                    player.AddBombLog(result.star);
                }
            }
        }

        private void GameOver(RoomInfo room_info)
        {
            room_info.SetRoundScore();
            RemovePlaying(room_info);
            m_Hall.TryRemove(room_info.key, out room_info);
            Game.Scene.GetComponent<RoomKeyComponent>().Recovery(room_info.key);
            SyncRoomInfo(room_info, GameState.GameOver);
        }

        public RoomInfo GetRoomInfo(long uid)
        {
            int key;
            if (m_playings.TryGetValue(uid, out key))
            {
                return GetRoomInfo(key);
            }

            return null;
        }

        private RoomInfo GetRoomInfo(int key)
        {
            RoomInfo room_info;
            if (m_Hall.TryGetValue(key, out room_info))
            {
                return room_info;
            }

            return null;
        }

        /// <summary>
        /// 客户端同步房间信息
        /// </summary>
        /// <param name="room_info"></param>
        /// <param name="state"></param>
        public void SyncRoomInfo(RoomInfo room_info, GameState state)
        {
            room_info.game_state = state;
            room_info.now = DateTime.Now;
            for (int i = 0; i < 4; i++)
            {
                if (room_info.players[i] != null && !room_info.players[i].is_ai)
                {
                    if (!room_info.players[i].hand_cards.Any())
                    {
                        room_info.CopyPartnerHandCard(room_info.players[i].uid);
                    }
                    else
                    {
                        room_info.CopyHandCard(room_info.players[i].uid);
                    }

                    //Env.TheServer.BeginInvokeClientService(room_info.players[i].uid, (int)ClientCommand.UpdateRoom, new object[] { room_info });
                }
            }
        }

        public List<byte> GetHandCard(List<byte> cards, ref int i)
        {
            int card_len = cards.Count;
            int range = i + 27;
            List<byte> hand_card = new List<byte>(27);
            for (; i < range; i++)
            {
                int index = Game.Scene.GetComponent<RandomComponent>().rand.Next(card_len - i);
                hand_card.Add(cards[index]);
                cards[index] = cards[card_len - (i + 1)];
            }

            return hand_card;
        }
    }
}
