using System.Collections.Generic;

namespace PRPG
{

    public class LRACachePool<U, T>
    {

        private readonly Queue<U> queue;        
        private readonly Dictionary<U, T> dict;            
        private readonly List<T> evictedList;

        public int Capacity { get; private set; }
        

        public LRACachePool(int capacity)
        {            
            Capacity = capacity;
            queue = new Queue<U>(capacity);
            dict = new Dictionary<U, T>(capacity);
            evictedList = new List<T>(4);
        }

                        
        public void Add(U key, T item)
        {            
            if (Count >= Capacity) {
                var oldestKey = queue.Dequeue();
                var e = dict[oldestKey];                
                dict.Remove(oldestKey);                
                evictedList.Add(e);                
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
            evictedList.Clear();
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
            if (evictedList.Count == 0) return default(T);
            var e = evictedList[evictedList.Count - 1];
            evictedList.RemoveAt(evictedList.Count - 1);            
            return e;
        }
        


    }


}
