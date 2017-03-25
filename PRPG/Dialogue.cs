using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using static PRPG.GraphUtils;
using Microsoft.Xna.Framework.Content;

namespace PRPG
{
    public static class Dialogue {
        public static Texture2D tradeBackground;
        public static Texture2D lineTexture;
        public static Texture2D selectionBackground;

        private const int PLAYER_COLUMN = 1;
        private const int NPC_COLUMN = 0;
        private static int row = 0;
        private static int column = 0;

        private static Inventory npcItems;
        private static Inventory playerItems;

        private enum TradeState {NONE,GOOD,BAD };
        private static TradeState tradeState;

        public static string currentResponse = string.Empty;

        private static NPC npc;
        private static Player player;

        public static int w;
        public static int h;
        public static Rectangle rect;

        public static void Initialize(ContentManager content) {
            int left = (int)(PRPGame.windowWidth * 0.1f);
            int right = (int)(PRPGame.windowWidth * 0.9f);
            int top = (int)(PRPGame.windowHeight * 0.1f);
            int bottom = (int)(PRPGame.windowHeight * 0.9f);            
            w = right - left;
            h = bottom - top;
            rect = new Rectangle(left, top, w, h);

            tradeBackground = content.Load<Texture2D>("UI/paper-plain");
            selectionBackground = GetSolidTex(1, 1, Color.Brown);
            lineTexture = GetSolidTex(1, 1, Color.White);
            npcItems = new Inventory();
            playerItems = new Inventory();
        }

        public static void Setup(Player _player, NPC _npc) {
            npcItems.Clear();
            playerItems.Clear();
            tradeState = TradeState.NONE;
            npc = _npc;
            player = _player;
            foreach (var item in npc.items) {
                npcItems.Add(item);
            }
            foreach (var item in player.items) {
                playerItems.Add(item);
            }
        }

        public static bool Accept() {           
            if (tradeState == TradeState.GOOD) {
                PRPGame.player.items.Clear();
                foreach (var slot in playerItems) {
                    PRPGame.player.items.Add(slot);
                }
                PRPGame.closestNPC.items.Clear();
                foreach (var slot in npcItems) {
                    PRPGame.closestNPC.items.Add(slot);
                }                
                npcItems.Clear();
                playerItems.Clear();
                return true;
            }
            else {
                return false;
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
                count = playerItems.DistinctItemsCount;
            else
                count = npcItems.DistinctItemsCount;
            if (row >= count)
                row=count-1;
            if (row < 0)
                row = 0;
        }

        public static void IncColumn() {
            column++;
            CheckColumn();
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
            if (column == PLAYER_COLUMN && playerItems.DistinctItemsCount == 0)
                column = NPC_COLUMN;
            if (column == NPC_COLUMN && npcItems.DistinctItemsCount == 0)
                column = PLAYER_COLUMN;

        }

        public static void MoveItem() {

            if (column == NPC_COLUMN) {
                var item = npcItems[row];
                npcItems.Remove(new Item(item.name,1));
                playerItems.Add(new Item(item.name,1));
            }
            else {
                var item = playerItems[row];
                playerItems.Remove(new Item(item.name,1));
                npcItems.Add(new Item(item.name,1));
            }            

            if (npc.IsTradeAcceptable(npcItems)) {
                tradeState = TradeState.GOOD;
                currentResponse = RandUtil.Index(PRPGame.closestNPC.personality.goodTrade);
            } else {
                tradeState = TradeState.BAD;
                currentResponse = RandUtil.Index(PRPGame.closestNPC.personality.badTrade);
            }
            CheckRow();
            CheckColumn();
        }


        public static void Draw() {

            int left = rect.Left;
            int right = rect.Right;
            int top = rect.Top;
            int bottom = rect.Bottom;
            
            PRPGame.batch.Draw(tradeBackground, new Vector2((float)left, (float)top), null, Color.White, 0, Vector2.Zero, new Vector2(2.05f,1.15f), SpriteEffects.None, 0.005f);            
            
            int strLen = (int)(PRPGame.mainFont.MeasureString(npc.fullName).X * 1.5f);
            var strPos = new Vector2(320 / 2 + (left+5) - strLen / 2, top+15);
            PRPGame.batch.DrawString(PRPGame.mainFont, npc.fullName, strPos, Color.Black,0,Vector2.Zero,1.5f,SpriteEffects.None,0.0f);
            npc.Draw(new Vector2(left + 75, top + 25),2.75f,true);

            left = left + w - 370;           
            
            

            strLen = (int)(PRPGame.mainFont.MeasureString(player.fullName).X * 1.5f);
            strPos = new Vector2(320 / 2 + (left + 5) - strLen / 2, top + 15);
            PRPGame.batch.DrawString(PRPGame.mainFont, player.fullName, strPos, Color.Black, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0.0f);
            player.facing = CharSprites.DOWN;
            player.Draw(new Vector2(left + 75, top + 25), 2.75f, true);

            left = rect.Left;
            top += 215;                                    

            strLen = (int)PRPGame.mainFont.MeasureString(player.firstName).X;
            strPos = new Vector2((w - w/4) + left - strLen / 2, top);
            PRPGame.batch.DrawString(PRPGame.mainFont, player.firstName, strPos, Color.White);

            top += 20;

            if (column == 0) {
                PRPGame.batch.Draw(selectionBackground, new Rectangle(left + 10, top + 10 + row * 20, 200, 20), Color.White);
            }
            else {
                PRPGame.batch.Draw(selectionBackground, new Rectangle(left + 10 + w / 2, top + 10 + row * 20, 200, 20), Color.White);
            }
            for (int i = 0; i < playerItems.DistinctItemsCount; i++) {
                var item = playerItems[i];
                var diff = item.qty - PRPGame.player.items.ItemQty(item.name);
                var diffString = diff <= 0 ? "" : " +" + diff;
                PRPGame.batch.DrawString(PRPGame.mainFont, item.qty+ " " + item.name + diffString, new Vector2(left + 10.0f + w / 2.0f, top + 10.0f + i * 20.0f), Color.White);
            }

            for (int i = 0; i < npcItems.DistinctItemsCount; i++) {
                var item = npcItems[i];
                var diff = item.qty - PRPGame.closestNPC.items.ItemQty(item.name);
                var diffString = diff <= 0 ? "" : " +" + diff;
                PRPGame.batch.DrawString(PRPGame.mainFont, item.qty+ " " + item.name + " " + diffString, new Vector2(left + 10.0f, top + 10.0f + i * 20.0f), Color.White);
            }

            
            strLen = (int)PRPGame.mainFont.MeasureString(currentResponse).X;
            Color tradeColor = Color.Red;
            if (tradeState == TradeState.GOOD) tradeColor = Color.Green;
            PRPGame.batch.DrawString(PRPGame.mainFont, currentResponse, new Vector2(w / 4.0f + left - strLen / 2.0f, bottom - 100.0f), tradeColor);


#if DEBUG
            string likes = string.Empty;
            foreach (var desire in npc.desires) {
                likes += desire.item.name + "("+desire.level+","+desire.sufficient+"),";
            }
            PRPGame.batch.DrawString(PRPGame.mainFont, likes, new Vector2(left, bottom - 60), Color.White);
            PRPGame.batch.DrawString(PRPGame.mainFont,"Before Utility:"+npc.Happiness(),new Vector2(left,bottom-40),Color.White);
            PRPGame.batch.DrawString(PRPGame.mainFont,"After Utility:" +npc.Happiness(npcItems), new Vector2(left, bottom - 20), Color.White);
#endif


        }
    }
}
