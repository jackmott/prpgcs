using System.Collections.Generic;

namespace PRPG
{

    public class LRACache<U, T> 
    {

        private readonly Queue<U> queue;        
        private readonly Dictionary<U, T> dict;            
        

        public int Capacity { get; private set; }
        

        public LRACache(int capacity)
        {            
            Capacity = capacity;
            queue = new Queue<U>(capacity);
            dict = new Dictionary<U, T>(capacity);            
        }

                        
        public T Add(U key, T item)
        {
            var e = default(T);
            if (Count >= Capacity) {
                var oldestKey = queue.Dequeue();
                e = dict[oldestKey];                
                dict.Remove(oldestKey);
                   
            }            
            dict.Add(key, item);
            queue.Enqueue(key);
            return e;
        }

        public bool Contains(U key)
        {
            return dict.ContainsKey(key);
        }
        
        public void Clear()
        {
            queue.Clear();
            dict.Clear();            
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
      

    }


}
