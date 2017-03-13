﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static PRPG.ProgrammerArt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPG {
    public static class Trade {


        public static Texture2D tradeBackground;
        public static Texture2D lineTexture;
        public static Texture2D selectionBackground;

        private const int PLAYER_COLUMN = 1;
        private const int NPC_COLUMN = 0;
        private static int row = 0;
        private static int column = 0;

        private static Inventory npcItems;
        private static Inventory playerItems;                       
        

        private static NPC npc;
        private static Player player;

        public static void Initialize() {
            tradeBackground = GetSolidTex(1, 1, Color.Black);
            selectionBackground = GetSolidTex(1, 1, Color.Blue);
            lineTexture = GetSolidTex(1, 1, Color.White);
            npcItems = new Inventory();
            playerItems = new Inventory();
        }

        public static void Setup(Player _player, NPC _npc) {
            npcItems.Clear();
            playerItems.Clear();
            npc = _npc;
            player = _player;
            foreach (var slot in npc.items) {
                npcItems.Add(new InventorySlot(slot.count,slot.item));
            }
            foreach (var slot in player.items) {
                playerItems.Add(new InventorySlot(slot.count, slot.item));
            }            
        }

        public static void IncRow() {
            row++;
            CheckRow();
        }

        public static void DecRow() {
            row--;
            CheckRow();                    
        }

        public static void CheckRow() {
            int count = 0;
            if (column == PLAYER_COLUMN)
                count = playerItems.Count;
            else
                count = npcItems.Count;
            if (row == count)
                row--;
            if (row < 0)
                row = 0;
        }

        public static void IncColumn() {
            column++;            
            CheckRow();
        }
        public static void DecColumn() {
            column--;
            CheckColumn();            
            CheckRow();
        }

        public static void CheckColumn() {
            if (column < 0)
                column = 0;
            if (column > 1)
                column = 1;
            if (column == PLAYER_COLUMN && playerItems.Count == 0)
                column = NPC_COLUMN;
            if (column == NPC_COLUMN && npcItems.Count == 0)
                column = PLAYER_COLUMN;

        }

        public static void MoveItem() {
            
            if (column == NPC_COLUMN) {
                var item = npcItems[row].item;
                npcItems.Remove(item);
                playerItems.Add(item);                
            }
            else {
                var item = playerItems[row].item;
                playerItems.Remove(item);
                npcItems.Add(item);
            }
            CheckColumn();
            CheckRow();            
        }


        public static void Draw() {
            int left = (int)(PRPGame.windowWidth * 0.1f);
            int right = (int)(PRPGame.windowWidth * 0.9f);
            int top = (int)(PRPGame.windowHeight * 0.1f);
            int bottom = (int)(PRPGame.windowHeight * 0.9f);

            PRPGame.batch.Draw(tradeBackground, new Rectangle(left, top, right - left, bottom - top), Color.White);
            PRPGame.batch.Draw(lineTexture, new Rectangle(PRPGame.windowWidth / 2, top, 1, bottom - top), Color.White);
            if (column == 0) {
                PRPGame.batch.Draw(selectionBackground, new Rectangle(left + 10, top + 10 + row * 20, 200, 20),Color.White);
             }
            else {
                PRPGame.batch.Draw(selectionBackground, new Rectangle(left + 10 + (right - left) / 2, top + 10 + row * 20, 200, 20),Color.White);
            }
            for (int i = 0; i < playerItems.Count; i++) {
                var slot = playerItems[i];
                var diff = slot.count - PRPGame.player.items.CountItem(slot.item);
                var diffString = diff == 0 ? "" : "+" + diff;
                PRPGame.batch.DrawString(PRPGame.mainFont, slot.count + " " + slot.item.name + diffString, new Vector2(left + 10 + (right-left)/2, top + 10+i*20), Color.White);
            }

            for (int i = 0; i < npcItems.Count; i++) {
                var slot = npcItems[i];
                var diff = slot.count - PRPGame.closestTalkableNPC.items.CountItem(slot.item);
                var diffString = diff == 0 ? "" : "+" + diff;
                PRPGame.batch.DrawString(PRPGame.mainFont, slot.count + " " +slot.item.name + " " + diffString, new Vector2(left + 10, top + 10 + i * 20), Color.White);
            }



        }
    }
}
