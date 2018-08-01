using GN;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

    public class MatchComponent : Component
    {
        private const int score_range = 100;

        private ConcurrentDictionary<short, Dictionary<int, Queue<MatchData<IMatcher>>>> m_AllMatchDic = new ConcurrentDictionary<short, Dictionary<int, Queue<MatchData<IMatcher>>>>();
        private HashSet<long> m_PlayerSet = new HashSet<long>();

        public async void Start()
        {
            while (true)
            {
                foreach (var match_dic in m_AllMatchDic.Values)
                {
                    lock (match_dic)
                    {
                        DateTime now = DateTime.Now;
                        List<int> need_start = new List<int>();
                        foreach (var item in match_dic)
                        {
                            var first = item.Value.Peek();
                            if (item.Value.Count > 4 || first.time <= now.AddSeconds(-5))
                            {
                                need_start.Add(item.Key);
                            }
                        }

                        foreach (var item in need_start)
                        {
                            var players = match_dic[item];
                            if (players != null)
                            {
                                match_dic.Remove(item);
                                foreach (var player in players)
                                {
                                    m_PlayerSet.Remove(player.data.uid);
                                }
                                //做开始处理
                                //RoomManager.Instance.InitRoom(players);
                            }
                        }
                    }
                }

                await Game.Scene.GetComponent<TimerComponent>().WaitAsync(1000);
            }
        }

        public Dictionary<int, Queue<MatchData<IMatcher>>> GetMatchDic(IMatcher data)
        {
            short key = (short)(((byte)data.game) << 8 | data.model_type);
            Dictionary<int, Queue<MatchData<IMatcher>>> match_dic = null;
            if (!m_AllMatchDic.TryGetValue(key, out match_dic))
            {
                match_dic = new Dictionary<int, Queue<MatchData<IMatcher>>>();
                m_AllMatchDic.TryAdd(key, match_dic);
            }

            return match_dic;
        }

        public bool WaitMatch(IMatcher data)
        {
            lock (this)
            {
                if (!m_PlayerSet.Add(data.uid))
                {
                    //玩家已经在匹配中
                    return false;
                }
            }

            var match_dic = GetMatchDic(data);
            lock (match_dic)
            {
                foreach (var item in match_dic)
                {
                    if (item.Key > (data.score - score_range) && item.Key < (data.score + score_range))
                    {
                        //找到相匹配的玩家
                        item.Value.Enqueue(new MatchData<IMatcher>(data));
                        if (item.Value.Count == 4)
                        {
                            match_dic.Remove(item.Key);
                            //做开始处理
                        }
                        return true;
                    }
                }

                //未找到相匹配玩家
                var queue = new Queue<MatchData<IMatcher>>(4);
                queue.Enqueue(new MatchData<IMatcher>(data));
                match_dic.Add(data.score, queue);

                return true;
            }
        }

        public bool IsMatching(long uid)
        {
            return m_PlayerSet.Contains(uid);
        }
    }
}
