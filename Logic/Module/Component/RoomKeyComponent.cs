using GN;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Logic
{
    public class RoomKeyComponent : Component
    {
        //当前未使用的key
        private ConcurrentQueue<int> keys = new ConcurrentQueue<int>();

        /// <summary>
        /// 初始化房间号
        /// </summary>
        /// <param name="key_count">初始化房间号个数</param>
        public void Awake(int key_count = 899999)
        {
            List<int> temp_keys = new List<int>();
            for (int i = 100000; i < 999999; i++)
            {
                temp_keys.Add(i);
            }
            for (int i = 100000; i < 999999; i++)
            {
                int index = Game.Scene.GetComponent<RandomComponent>().rand.Next(999999 - i);
                int key = temp_keys[index];

                keys.Enqueue(temp_keys[index]);
                key_count--;
                if (key_count == 0)
                {
                    break;
                }

                temp_keys[index] = temp_keys[999999 - (i + 1)];
            }
        }

        /// <summary>
        /// 获取一个新的key
        /// </summary>
        public int Key
        {
            get
            {
                int temp_key = 0;
                while (!keys.TryDequeue(out temp_key)) ;
                return temp_key;
            }
        }

        /// <summary>
        /// 回收一个key
        /// </summary>
        /// <param name="key"></param>
        public void Recovery(string key)
        {
            int temp_key = 0;
            if (int.TryParse(key, out temp_key))
            {
                Recovery(temp_key);
            }
        }
        /// <summary>
        /// 回收一个key
        /// </summary>
        /// <param name="key"></param>
        public void Recovery(int key)
        {
            keys.Enqueue(key);
        }
    }
}
