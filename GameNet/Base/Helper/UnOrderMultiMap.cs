using System.Collections.Generic;

namespace GN
{
    public class UnOrderMultiMap<T, K>
    {
        private readonly Dictionary<T, List<K>> dictionary = new Dictionary<T, List<K>>();

        public void Add(T t, K k)
        {
            List<K> list;
            this.dictionary.TryGetValue(t, out list);
            if(list == null)
            {
                list = new List<K>();
                this.dictionary[t] = list;
            }
            list.Add(k);
        }

        public bool Remove(T t, K k)
        {
            List<K> list = null; 
            if(!this.dictionary.TryGetValue(t, out list))
            {
                return false;
            }
            if(!list.Remove(k))
            {
                return false;
            }
            if(list.Count == 0)
            {
                this.dictionary.Remove(t);
            }
            return true;
        }

        public bool Remove(T t)
        {
            List<K> list = null;
            if(!this.dictionary.TryGetValue(t, out list))
            {
                return this.dictionary.Remove(t);
            }
            return false;
        }

        public List<K> this[T t]
        {
            get
            {
                List<K> list = null;
                if (this.dictionary.TryGetValue(t, out list))
                {
                    return list;
                }
                return null;
            }
        }

        public bool ContainsKey(T t)
        {
            return this.dictionary.ContainsKey(t);
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }
    }
}
