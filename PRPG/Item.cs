using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PRPG
{

    public class Inventory {
        Dictionary<Item, ItemQty> itemDict;
        List<ItemQty> itemList;

        public Inventory() {
            itemDict = new Dictionary<Item, ItemQty>();
            itemList = new List<ItemQty>();
        }

        public int TotalCount() {
            return itemList.Sum(x => x.count);
        }

        public int CountItem(Item item) {            
            if (itemDict.TryGetValue(item, out var slot)) {
                return slot.count;
            }
            return 0;
        }
        public int Count {
            get {
                return itemList.Count;
            }
        }
        
        public ItemQty this[int index] {
            get {
                return itemList[index];
            }

            set {
                itemList[index] = value;
            }
        }

        public ItemQty this[Item key] {
            get {
                return itemDict[key];
            }

            set {
                itemDict[key] = value;
            }
        }

        public void Add(ItemQty slot) {
            itemDict.Add(slot.item, slot);
            itemList.Add(slot);
        }

        public void Remove(ItemQty slot) {
            itemDict.Remove(slot.item);
            itemList.Remove(slot);
        }

        public void Add(Item item) {            
            if (itemDict.TryGetValue(item, out var slot)) {
                slot.count++;
            }
            else {
                slot = new ItemQty(1, item);
                itemDict.Add(item, slot);
                itemList.Add(slot);
            }
        }

        public bool Remove(Item item) {            
            if (itemDict.TryGetValue(item, out var slot)) {
                slot.count--;
                if (slot.count == 0) {
                    itemDict.Remove(item);
                    itemList.Remove(slot);
                }
                return true;
            }
            return false;
        }

        public bool ContainsKey(Item key) {
            return itemDict.ContainsKey(key);
        }
        
        public void Clear() {
            itemDict.Clear();
            itemList.Clear();
        }

        public List<ItemQty>.Enumerator GetEnumerator() {
            return itemList.GetEnumerator();
        }
              
    }

    public class ItemQty {

        public int count;        
        public readonly Item item;

        public ItemQty(int count, Item item) {
            this.count = count;
            this.item = item;            
        }
    }
    public struct Item {

        public static Dictionary<string,Item> itemPool;

        public static void Initialize() {
            itemPool = new Dictionary<string, Item>();
            var classes = (JArray)JObject.Parse(File.ReadAllText("Data/classes.json"))["Classes"];

            foreach (var c in classes)
            {
                var items = (JArray)c["Desires"];
                foreach (string item in items) {
                    itemPool.Add(item, new Item(item));
                }
            }                        
        }

        public readonly string name;        

        
        public Item(string name) {        
            this.name = name;            
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
