using System;
using static PRPG.ProgrammerArt;
using static System.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace PRPG {

    public enum GameState { ROAM, DIALOGUE };
    public enum GameCommand { NONE,TALK,BACK };

    public struct GameStateTransition {
        public GameState currentState;
        public GameCommand command;

        public GameStateTransition(GameState currentState, GameCommand command) {
            this.currentState = currentState;
            this.command = command;
        }
    }

    public class PRPGame : Game {

        private static Dictionary<GameStateTransition,GameState> stateMachine = 
            new Dictionary<GameStateTransition,GameState> {
            { new GameStateTransition(GameState.ROAM,GameCommand.TALK),GameState.DIALOGUE },
            { new GameStateTransition(GameState.DIALOGUE,GameCommand.BACK),GameState.ROAM}
        };

        // GLOBAL STATE
        public static GraphicsDevice graphics;
        public static SpriteFont mainFont;

        public GameState state = GameState.ROAM;
        GraphicsDeviceManager graphicsManager;
        SpriteBatch batch;

        Vector2 worldPos;
        Player player;
        World world;
        int WindowWidth;
        int WindowHeight;

        KeyboardState lastKeyState;
        GamePadState lastPadState;

        Texture2D dialogBackground;

        public PRPGame() {
            Content.RootDirectory = "Content";
            graphicsManager = new GraphicsDeviceManager(this);
        }

        public void AdvanceState(GameCommand command) {
            GameState newState;
            var trans = new GameStateTransition(state, command);
            if (stateMachine.TryGetValue(trans, out newState)) {
                state = newState;
            }
        }

        protected override void Initialize() {
            base.Initialize();
            graphicsManager.PreferredBackBufferHeight = 1080;
            graphicsManager.PreferredBackBufferWidth = 1920;
            graphicsManager.ApplyChanges();
            graphics = GraphicsDevice;
            WindowHeight = GraphicsDevice.Viewport.Bounds.Height;
            WindowWidth = GraphicsDevice.Viewport.Bounds.Width;
            dialogBackground = GetSolidTex(200, 100, Color.Black);
            world = new World(100, 100);
            player = new Player(new Vector2(world.width / 2, world.height / 2));
            worldPos = player.pos;
        }

        protected override void LoadContent() {
            batch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("MainFont");
        }

        protected override void UnloadContent() {
        }

        private bool IsNewKeyPress(Keys key) {
            bool currentlyPressed = Keyboard.GetState().IsKeyDown(key);
            bool wasPressed = lastKeyState.IsKeyDown(key);
            return currentlyPressed && !wasPressed;
        }

        private bool isNewButtonPress(Buttons button) {
            bool currentlyPressed = GamePad.GetState(PlayerIndex.One).IsButtonDown(button);
            bool wasPressed = lastPadState.IsButtonDown(button);
            return currentlyPressed && !wasPressed;            
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
                var padState = GamePad.GetState(PlayerIndex.One);
                if (gamePadCap.HasLeftXThumbStick) {
                    movement += new Vector2(padState.ThumbSticks.Left.X * moveDistance, 0);
                }
                if (gamePadCap.HasLeftYThumbStick) {
                    movement -= new Vector2(0, padState.ThumbSticks.Left.Y * moveDistance);
                }
                if (isNewButtonPress(Buttons.A)) {
                    talkButton = true;
                }
                lastPadState = padState;
            }



            var keyState = Keyboard.GetState();

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

            GameCommand command = GameCommand.NONE;
            foreach (var npc in world.npcs) {
                var npcState = npc.Update(gameTime, player,talkButton);
                if (npcState == ENPCState.DIALOG)
                    command = GameCommand.TALK;
            }

            
            
            AdvanceState(command);           

            lastKeyState = keyState;
            

        }


        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);

            
            float numTilesX = WindowWidth / (float)World.TileSize;
            float numTilesY = WindowHeight / (float)World.TileSize;
            int startX = (int)Floor(worldPos.X - numTilesX / 2.0f);
            int endX = (int)Ceiling(worldPos.X + numTilesX / 2.0f);
            int startY = (int)Floor(worldPos.Y - numTilesY / 2.0f);
            int endY = (int)Ceiling(worldPos.Y + numTilesY / 2.0f);

            var screenCenter = new Vector2((float)WindowWidth / 2.0f, (float)WindowHeight / 2.0f);

            var offset = worldPos * (float)World.TileSize - screenCenter;
            batch.Begin();

            for (int y = startY; y <= endY; y++) {
                for (int x = startX; x <= endX; x++) {
                    var tile = world.GetTileTexture(x, y);
                    var screenPos = new Vector2(x, y) * World.TileSize - offset;
                    batch.Draw(tile, screenPos, Color.White);
                }
            }

            float maxDist = (float)Math.Sqrt(numTilesX * numTilesX + numTilesY * numTilesY);
            foreach (var npc in world.npcs) {
                if (Vector2.Distance(npc.pos, worldPos) <= maxDist) {
                    npc.Draw(batch, World.TileSize, offset);
                }
            }

            player.Draw(batch, World.TileSize, offset);

            if (state == GameState.DIALOGUE) {
                int left = (int)(WindowWidth * 0.1f);
                int right = (int)(WindowWidth * 0.9f);
                int top = (int)(WindowHeight* 0.1f);
                int bottom = (int)(WindowHeight * 0.9f);

                batch.Draw(dialogBackground, new Rectangle(left,top,right-left,bottom-top), Color.White);
                batch.DrawString(mainFont, "Lorem Ipsum Something Something", new Vector2(left+10, top+10), Color.White);
            }

            batch.End();
        }
    }
}
