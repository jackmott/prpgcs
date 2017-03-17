// Copyright (C) 2009 Robert Rossney <rrossney@gmail.com>
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace PRPG
{
    public class LRACachePool<U, T>
    {
        public struct KVP
        {
            public U key;
            public T value;

            public KVP(U key, T value)
            {
                this.key = key;
                this.value = value;
            }
        };

        private const int _DefaultCapacity = 1000;

        /// <summary>
        /// The default Capacity that the LRUCache uses if none is provided in the constructor.
        /// </summary>
        public static int DefaultCapacity { get { return _DefaultCapacity; } }

        // The list of items in the cache.  New items are added to the end of the list;
        // existing items are moved to the end when added; the items thus appear in
        // the list in the order they were added/used, with the least recently used
        // item being the first.  This is internal because the LRUCacheEnumerator
        // needs to access it.
        internal readonly LinkedList<KVP> List = new LinkedList<KVP>();

        // The index into the list, used by Add, Remove, and Contains.
        private readonly Dictionary<U, LinkedListNode<KVP>> Index =
            new Dictionary<U, LinkedListNode<KVP>>();

        private List<T> Evicted = new List<T>();

        // Add, Clear, CopyTo, and Remove lock on this object to keep them threadsafe.                

        /// <summary>
        /// Initializes a new instance of the LRUCache class that is empty and has the default
        /// capacity.
        /// </summary>
        public LRACachePool() : this(_DefaultCapacity) { }

        /// <summary>
        /// Initializes a new instance of the LRUCache class that is empty and has the specified
        /// initial capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public LRACachePool(int capacity)
        {
            if (capacity < 0) {
                throw new InvalidOperationException("LRUCache capacity must be positive.");
            }
            Capacity = capacity;
        }


        /// <summary>
        /// The maximum number of items that the LRUCache can contain without discarding
        /// the oldest item when a new one is added.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// The oldest (i.e. least recently used) item in the LRUCache.
        /// </summary>
        public KVP Oldest {
            get {
                return List.First.Value;
            }
        }

        
        public void Add(U key, T item)
        {

            if (Index.ContainsKey(key)) {
                List.Remove(Index[key]);
                Index[key] = List.AddLast(new KVP(key, item));
                return;
            }

            if (Count >= Capacity) {
                Evicted.Add(Index[Oldest.key].Value.value);
                Remove(Oldest.key);
            }
            Index.Add(key, List.AddLast(new KVP(key, item)));



        }

        public bool Contains(U key)
        {
            return Index.ContainsKey(key);
        }


        /// <summary>
        /// Clear the contents of the LRUCache.
        /// </summary>
        public void Clear()
        {
            List.Clear();
            Index.Clear();
            Evicted.Clear();
        }

        /// <summary>
        /// Gets the number of items contained in the LRUCache.
        /// </summary>
        public int Count {
            get {
                return List.Count;
            }
        }

        public T Get(U key)
        {            
            Index.TryGetValue(key, out var node);
            if (node == null) return default(T);
            return Index[key].Value.value;
        }

        public T GetEvicted()
        {
            if (Evicted.Count == 0) return default(T);
            var e = Evicted[Evicted.Count - 1];
            Evicted.RemoveAt(Evicted.Count - 1);
            return e;
        }

        public bool Remove(U key)
        {

            if (Index.ContainsKey(key)) {                
                List.Remove(Index[key]);
                Index.Remove(key);
                return true;
            }
            return false;

        }


    }


}
