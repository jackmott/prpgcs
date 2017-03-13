﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
    

namespace PRPG {

    public class Inventory {
        Dictionary<Item, InventorySlot> itemDict;
        List<InventorySlot> itemList;

        public Inventory() {
            itemDict = new Dictionary<Item, InventorySlot>();
            itemList = new List<InventorySlot>();
        }

        public int TotalCount() {
            return itemList.Sum(x => x.count);
        }

        public int CountItem(Item item) {
            InventorySlot slot;
            if (itemDict.TryGetValue(item, out slot)) {
                return slot.count;
            }
            return 0;
        }
        public int Count {
            get {
                return itemList.Count;
            }
        }
        
        public InventorySlot this[int index] {
            get {
                return itemList[index];
            }

            set {
                itemList[index] = value;
            }
        }

        public InventorySlot this[Item key] {
            get {
                return itemDict[key];
            }

            set {
                itemDict[key] = value;
            }
        }

        public void Add(InventorySlot slot) {
            itemDict.Add(slot.item, slot);
            itemList.Add(slot);
        }

        public void Remove(InventorySlot slot) {
            itemDict.Remove(slot.item);
            itemList.Remove(slot);
        }

        public void Add(Item item) {
            InventorySlot slot;
            if (itemDict.TryGetValue(item, out slot)) {
                slot.count++;
            }
            else {
                slot = new InventorySlot(1, item);
                itemDict.Add(item, slot);
                itemList.Add(slot);
            }
        }

        public bool Remove(Item item) {
            InventorySlot slot;
            if (itemDict.TryGetValue(item, out slot)) {
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

        public List<InventorySlot>.Enumerator GetEnumerator() {
            return itemList.GetEnumerator();
        }
              
    }
    public class InventorySlot {

        public int count;        
        public readonly Item item;

        public InventorySlot(int count, Item item) {
            this.count = count;
            this.item = item;            
        }
    }
    public struct Item {

        public static Dictionary<string,Item> itemPool;

        public static void Initialize() {
            itemPool = new Dictionary<string, Item>();
            var itemsText = CSVReader.ReadFile("Data/items.txt");
            foreach (var itemText in itemsText) {
                var item = new Item(itemText[0], itemText[1]);
                itemPool.Add(item.name, item);
            }            
        }

        public readonly string name;
        public readonly string pluralName;

        
        public Item(string name, string pluralName) {        
            this.name = name;
            this.pluralName = pluralName;
        }
        

        public override bool Equals(object obj) {
            var item = (Item)obj;
            return name.Equals(item.name);
        }

        public override int GetHashCode() {
            return name.GetHashCode();
        }
    }
}
