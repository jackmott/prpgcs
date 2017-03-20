using System;
using static System.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;


namespace PRPG
{

    public enum GameState { ROAM, DIALOGUE, TRADE };
    public enum GameCommand { NONE, TALK, TRADE, BACK };


    public struct GameStateTransition
    {
        public GameState currentState;
        public GameCommand command;

        public GameStateTransition(GameState currentState, GameCommand command)
        {
            this.currentState = currentState;
            this.command = command;
        }
    }

    public class PRPGame : Game
    {

        public static GameState state = GameState.ROAM;
        private static Dictionary<GameStateTransition, GameState> stateMachine =
            new Dictionary<GameStateTransition, GameState> {
            { new GameStateTransition(GameState.ROAM,GameCommand.TALK),GameState.DIALOGUE },
            { new GameStateTransition(GameState.DIALOGUE,GameCommand.BACK),GameState.ROAM},
            { new GameStateTransition(GameState.DIALOGUE,GameCommand.TRADE),GameState.TRADE},
            { new GameStateTransition(GameState.TRADE,GameCommand.BACK),GameState.DIALOGUE }
        };

        // GLOBAL STATE
        public static GraphicsDevice graphics;
        public static SpriteFont mainFont;
        public static int windowWidth;
        public static int windowHeight;
        public static SpriteBatch batch;
        public static WordBank wordBank;
        public static float numTilesX, numTilesY;
        public static float maxDist;        
        public static List<Texture2D> tilePool;
        public static List<Texture2D> pendingTilePool;

        public static int startX;
        public static int endX;
        public static int startY;
        public static int endY;
        public Vector2 screenCenter;
        public Vector2 offset;


        public static int npcSprited = 0;        

        public static bool renderFancyTiles = true;


        GraphicsDeviceManager graphicsManager;

        public ShaderManager shaderManager;
        public static Vector2 worldPos;
        public static Player player;
        public static World world;

        KeyboardState lastKeyState;
        GamePadState lastPadState;

        public static NPC closestNPC = null;

        public PRPGame()
        {
            Content.RootDirectory = "Content";
            graphicsManager = new GraphicsDeviceManager(this);            
            tilePool = new List<Texture2D>(32);
            pendingTilePool = new List<Texture2D>(32);
            //graphicsManager.IsFullScreen = true;
        }

        public void AdvanceState(GameCommand command)
        {
            var trans = new GameStateTransition(state, command);
            if (stateMachine.TryGetValue(trans, out var newState)) {
                if (newState == GameState.TRADE) {
                    Trade.Setup(player, closestNPC);
                }
                else if (newState == GameState.DIALOGUE) {
                    Dialogue.Setup();
                }
                state = newState;
            }
            else {
                Debug.Assert(command == GameCommand.NONE);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphicsManager.PreferredBackBufferHeight = 1080;
            graphicsManager.PreferredBackBufferWidth = 1920;



            graphicsManager.ApplyChanges();
            graphics = GraphicsDevice;
            windowHeight = GraphicsDevice.Viewport.Bounds.Height;
            windowWidth = GraphicsDevice.Viewport.Bounds.Width;
            numTilesX = windowWidth / (float)World.tileSize;
            numTilesY = windowHeight / (float)World.tileSize;
            maxDist = (float)Math.Sqrt(numTilesX * numTilesX + numTilesY * numTilesY);

            wordBank = new WordBank();
            CharSprites.Initialize(Content);
            Dialogue.Initialize();
            Trade.Initialize();
            Item.Initialize();
            NPC.Initialize();

            foreach (var item in Item.itemPool.Values) {
                var noun = wordBank.QueryNoun(item.name);
                Debug.Assert(noun != null);
            }

            world = new World(500, 500, Content);
            player = new Player(new Vector2(world.width / 2, world.height / 2), Content);
            worldPos = player.pos;
        }

        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("MainFont");
            shaderManager = new ShaderManager(Content, GraphicsDevice);

        }

        protected override void UnloadContent()
        {
        }

        private bool IsNewKeyPress(Keys key)
        {
            bool currentlyPressed = Keyboard.GetState().IsKeyDown(key);
            bool wasPressed = lastKeyState.IsKeyDown(key);
            return currentlyPressed && !wasPressed;
        }

        private bool IsNewButtonPress(Buttons button)
        {
            bool currentlyPressed = GamePad.GetState(PlayerIndex.One).IsButtonDown(button);
            bool wasPressed = lastPadState.IsButtonDown(button);
            return currentlyPressed && !wasPressed;
        }

        private enum Action { MAIN, CONFIRM, BACK, LEFT, RIGHT, UP, DOWN };
        private bool IsNewAction(Action a)
        {
            switch (a) {
                case Action.CONFIRM:
                    return IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.Y);
                case Action.MAIN:
                    return IsNewKeyPress(Keys.E) || IsNewButtonPress(Buttons.A);
                case Action.BACK:
                    return IsNewKeyPress(Keys.Q) || IsNewButtonPress(Buttons.B);
                case Action.LEFT:
                    return IsNewKeyPress(Keys.Left) || IsNewButtonPress(Buttons.LeftThumbstickLeft);
                case Action.RIGHT:
                    return IsNewKeyPress(Keys.Right) || IsNewButtonPress(Buttons.LeftThumbstickRight);
                case Action.UP:
                    return IsNewKeyPress(Keys.Up) || IsNewButtonPress(Buttons.LeftThumbstickUp);
                case Action.DOWN:
                    return IsNewKeyPress(Keys.Down) || IsNewButtonPress(Buttons.LeftThumbstickDown);
            }
            return false;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var keyState = Keyboard.GetState();
            var padState = GamePad.GetState(PlayerIndex.One);
            var command = GameCommand.NONE;

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            if (state == GameState.DIALOGUE) {
                if (IsNewAction(Action.UP)) {
                    Dialogue.DecrementSelection();
                }
                else if (IsNewAction(Action.DOWN)) {
                    Dialogue.IncrementSelection();
                }
                else if (IsNewAction(Action.MAIN)) {
                    command = Dialogue.Selection();
                }
                else if (IsNewAction(Action.BACK)) {
                    command = GameCommand.BACK;
                }
            }
            else if (state == GameState.TRADE) {
                if (IsNewAction(Action.UP)) {
                    Trade.DecRow();
                }
                else if (IsNewAction(Action.DOWN)) {
                    Trade.IncRow();
                }
                else if (IsNewAction(Action.LEFT)) {
                    Trade.DecColumn();
                }
                else if (IsNewAction(Action.RIGHT)) {
                    Trade.IncColumn();
                }
                else if (IsNewAction(Action.BACK)) {
                    command = GameCommand.BACK;
                }
                else if (IsNewAction(Action.MAIN)) {
                    Trade.MoveItem();
                }
                else if (IsNewAction(Action.CONFIRM)) {
                    if (Trade.Accept()) {
                        command = GameCommand.BACK;
                    }
                }
            }
            else {


                var tile = world.GetTile(player.pos);

                float moveDistance = 0.0f;
                if (tile == World.TerrainTile.WATER)
                    moveDistance = 0.01f;
                else
                    moveDistance = 0.1f;

                Vector2 movement = Vector2.Zero;
                bool talkButton = false;

                var gamePadCap = GamePad.GetCapabilities(PlayerIndex.One);
                if (gamePadCap.IsConnected) {

                    if (gamePadCap.HasLeftXThumbStick) {
                        movement += new Vector2(padState.ThumbSticks.Left.X * moveDistance, 0);
                    }
                    if (gamePadCap.HasLeftYThumbStick) {
                        movement -= new Vector2(0, padState.ThumbSticks.Left.Y * moveDistance);
                    }
                    if (IsNewAction(Action.MAIN)) {
                        talkButton = true;
                    }

                }


                if (keyState.IsKeyDown(Keys.Right)) {
                    movement += new Vector2(moveDistance, 0);
                }
                if (keyState.IsKeyDown(Keys.Left)) {
                    movement -= new Vector2(moveDistance, 0);
                }
                if (keyState.IsKeyDown(Keys.Up)) {
                    movement -= new Vector2(0, moveDistance);
                }
                if (keyState.IsKeyDown(Keys.Down)) {
                    movement += new Vector2(0, moveDistance);
                }
                if (IsNewKeyPress(Keys.E)) {
                    talkButton = true;
                }


                if (movement.Length() > moveDistance) {
                    movement = Vector2.Normalize(movement) * moveDistance;
                }


                player.pos += movement;
                player.Update(gameTime);

                float xDiff = worldPos.X - player.pos.X;
                float yDiff = worldPos.Y - player.pos.Y;

                float threshold = 1.5f;
                if (xDiff > threshold) {
                    worldPos.X += movement.X;
                }
                else if (xDiff < -threshold) {
                    worldPos.X += movement.X;
                }

                if (yDiff > threshold) {
                    worldPos.Y += movement.Y;
                }
                else if (yDiff < -threshold) {
                    worldPos.Y += movement.Y;
                }


                closestNPC = null;
                float closestNPCDist = float.MaxValue;
                foreach (var npc in world.npcs) {
                    npc.Update(gameTime, player, Content);
                    if (npc.state == ENPCState.HELLO) {
                        var dist = Vector2.DistanceSquared(npc.pos, player.pos);
                        if (dist < closestNPCDist) {
                            closestNPC = npc;
                            closestNPCDist = dist;
                        }
                    }
                }

                if (talkButton && closestNPC != null)
                    command = GameCommand.TALK;

            }
            AdvanceState(command);
            if (IsNewButtonPress(Buttons.X)) {
                renderFancyTiles = !renderFancyTiles;
            }

            startX = (int)Floor(worldPos.X - numTilesX / 2.0f);
            endX = (int)Ceiling(worldPos.X + numTilesX / 2.0f);
            startY = (int)Floor(worldPos.Y - numTilesY / 2.0f);
            endY = (int)Ceiling(worldPos.Y + numTilesY / 2.0f);

            screenCenter = new Vector2(windowWidth / 2, windowHeight / 2);

            offset = worldPos * (float)World.tileSize - screenCenter;

            lastPadState = padState;
            lastKeyState = keyState;
        }


        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Black);                                    
            for (int i = 0; i < pendingTilePool.Count;i++) {
                tilePool.Add(pendingTilePool[i]);
            }
            pendingTilePool.Clear();
            
            var screenTiles = new Texture2D[((windowWidth / World.tileSize) + 4) * ((windowHeight / World.tileSize) + 4)];
            int index = 0;
            for (int y = startY; y <= endY; y++) {
                for (int x = startX; x <= endX; x++) {

                    Texture2D tile;
                    if (!renderFancyTiles) {
                        tile = world.GetTexSimple(x, y);
                    }
                    else {
                        tile = world.GetTex(x, y);
                    }
                    screenTiles[index] = tile;
                    index++;
                }
            }

            batch.Begin(SpriteSortMode.Immediate);            
            index = 0;
            for (int y = startY; y <= endY; y++) {
                for (int x = startX; x <= endX; x++) {

                    Texture2D tile = screenTiles[index];
                    index++;
                    var screenPos = new Vector2(x, y) * World.tileSize - offset;
                    if (screenPos.X >= -World.tileSize && screenPos.Y >= -World.tileSize && screenPos.X < windowWidth && screenPos.Y < windowHeight)
                        batch.Draw(tile, new Rectangle((int)screenPos.X, (int)screenPos.Y, World.tileSize, World.tileSize), Color.White);
                }
            }



            foreach (var npc in world.npcs) {
                if (Vector2.Distance(npc.pos, worldPos) <= maxDist) {
                    npc.Draw(batch, World.tileSize, offset);
                }
            }

            player.Draw(batch, World.tileSize, offset);

            if (state == GameState.DIALOGUE) {
                Dialogue.Draw();
            }
            else if (state == GameState.TRADE) {
                Trade.Draw();
            }


            batch.DrawString(mainFont, "x:" + (int)worldPos.X + "," + (int)worldPos.Y + "  sprited:" + npcSprited, Vector2.Zero, Color.Yellow);

            batch.End();            
            base.Draw(gameTime);
        }
    }
}
