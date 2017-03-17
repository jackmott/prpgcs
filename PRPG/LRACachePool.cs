using System.Collections.Generic;
using System.Diagnostics;

namespace PRPG
{

    public class LRACachePool<U, T>
    {
                                
        private readonly Queue<U> queue = new Queue<U>();
        
        private readonly Dictionary<U,T> dict =
            new Dictionary<U,T>();

        private readonly List<T> evcitedList = new List<T>();

        public int Capacity { get; private set; }

        public LRACachePool(int capacity)
        {            
            Capacity = capacity;
        }

                        
        public void Add(U key, T item)
        {
            Debug.Assert(!dict.ContainsKey(key));

            if (Count >= Capacity) {
                var oldestKey = queue.Dequeue();
                evcitedList.Add(dict[oldestKey]);
                dict.Remove(oldestKey);                
            }            
            dict.Add(key, item);
            queue.Enqueue(key);
        }

        public bool Contains(U key)
        {
            return dict.ContainsKey(key);
        }
        
        public void Clear()
        {
            queue.Clear();
            dict.Clear();
            evcitedList.Clear();
        }
        
        public int Count {
            get {
                return queue.Count;
            }
        }

        public T Get(U key)
        {
            if (!dict.ContainsKey(key)) return default(T);            
            return dict[key];
        }

        public T GetEvicted()
        {
            if (evcitedList.Count == 0) return default(T);
            var e = evcitedList[evcitedList.Count - 1];
            evcitedList.RemoveAt(evcitedList.Count - 1);
            return e;
        }
        


    }


}
