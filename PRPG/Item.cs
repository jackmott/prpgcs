using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.ComponentModel;
using Newtonsoft.Json;
using JM.LinqFaster;
using JM.LinqFaster.SIMD;
namespace PRPG
{

    public class Inventory {        
        List<Item> itemList;
        public int gold;

        public Inventory() {            
            itemList = new List<Item>();
        }

        public int TotalItems {
            get {
                return itemList.Sum(x => x.qty);
            }
        }

        public int ItemQty(string name) {
            return itemList.Where(x => x.name == name).Sum(x => x.qty);
        }

        public bool Contains(Item item)
        {
            return itemList.Exists(x => x.name == item.name);
        }

        public int DistinctItemsCount {
            get {
                return itemList.Count;
            }
        }

        public void Clear()
        {
            itemList.Clear();
        }


        public Item this[int index] {
            get {
                return itemList[index];
            }

            set {
                itemList[index] = value;
            }
        }

        public void Add(Item item) {
            var existingItems = itemList.Where(x => x.name == item.name);
            Debug.Assert(existingItems.Count() <= 1);
            if (existingItems.Count() == 0) {
                itemList.Add(item);
            } else {
                var existingItem = existingItems.First(); 
                existingItem.qty += item.qty;
            }

        }

        public void Remove(Item item) {
            var existingItems = itemList.Where(x => x.name == item.name);
            Debug.Assert(existingItems.Count() <= 1);
            if (existingItems.Count() > 0) {
                var existingItem = existingItems.First();
                existingItem.qty -= item.qty;
                if (existingItem.qty <= 0) itemList.Remove(existingItem);
            }
            
        }

        public List<Item>.Enumerator GetEnumerator()
        {
            return itemList.GetEnumerator();
        }

    }
 
    public class Item {

        
        public string name;

        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int qty;
        
        public Item(string name, int qty) {        
            this.name = name;
            this.qty = qty;
        }
        

        public override bool Equals(object obj) {
            if (obj == null) return false;
            var item = (Item)obj;
            return name.Equals(item.name);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
    }
}
