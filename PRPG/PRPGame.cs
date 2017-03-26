using System;
using static System.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PRPG
{

    public enum GameState { ROAM, DIALOGUE};
    public enum GameCommand { NONE, TALK, BACK };


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
        };

        public const float actionDist = 2.0f;

        // GLOBAL STATE
        private FrameCounter frameCounter = new FrameCounter();
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
           // this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
            Content.RootDirectory = "Content";
            graphicsManager = new GraphicsDeviceManager(this);
            tilePool = new List<Texture2D>(32);
            pendingTilePool = new List<Texture2D>(32);
            //graphicsManager.IsFullScreen = true;
        }

        public void AdvanceState(GameCommand command)
        {
            var trans = new GameStateTransition(state, command);
            if (stateMachine.TryGetValue(trans, out var newState))
            {
                if (newState == GameState.DIALOGUE)
                {
                    Dialogue.Setup(player, closestNPC);
                }                
                state = newState;
            }
            else
            {
                Debug.Assert(command == GameCommand.NONE);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphicsManager.PreferredBackBufferHeight = 1080;
            graphicsManager.PreferredBackBufferWidth = 1920;
            graphicsManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            graphicsManager.ApplyChanges();
            graphics = GraphicsDevice;
            windowHeight = GraphicsDevice.Viewport.Bounds.Height;
            windowWidth = GraphicsDevice.Viewport.Bounds.Width;
            numTilesX = windowWidth / (float)World.tileSize;
            numTilesY = windowHeight / (float)World.tileSize;
            maxDist = (float)Math.Sqrt(numTilesX * numTilesX + numTilesY * numTilesY);

            wordBank = new WordBank();
            CharSprites.Initialize(Content);
            Dialogue.Initialize(Content);            
            NPC.Initialize();

            world = new World(500, 500, Content);
            player = new Player(new Vector2(world.width / 2, world.height / 2), world,Content);
            worldPos = player.pos;
        }


        private bool OnScreen(Vector2 pos, int w, int h)
        {
            return
                pos.X >= -w &&
                pos.X < windowWidth &&
                pos.Y >= -h &&
                pos.Y < windowHeight;
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

        private enum Action { MAIN, USE_ITEM, CONFIRM, BACK, LEFT, RIGHT, UP, DOWN };
        private bool IsNewAction(Action a)
        {
            switch (a)
            {
                case Action.USE_ITEM:
                    return IsNewKeyPress(Keys.Space) || IsNewButtonPress(Buttons.X);
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

            if (state == GameState.DIALOGUE)
            {
                if (IsNewAction(Action.UP))
                {
                    Dialogue.DecRow();
                }
                else if (IsNewAction(Action.DOWN))
                {
                    Dialogue.IncRow();
                }
                else if (IsNewAction(Action.LEFT))
                {
                    Dialogue.DecColumn();
                }
                else if (IsNewAction(Action.RIGHT))
                {
                    Dialogue.IncColumn();
                }
                else if (IsNewAction(Action.BACK))
                {
                    command = GameCommand.BACK;
                }
                else if (IsNewAction(Action.MAIN))
                {
                    Dialogue.MoveItem();
                }
                else if (IsNewAction(Action.CONFIRM))
                {
                    if (Dialogue.Accept())
                    {
                        command = GameCommand.BACK;
                    }
                }
            }
            else
            {

                if (IsNewAction(Action.USE_ITEM))
                {
                    var (closestResource, minDist) =
                        world.resources.ClosestTo(player);
                    
                    if (minDist < actionDist + (closestResource.width / 2 / World.tileSize) && closestResource != null)
                    {
                        player.Chop(closestResource);                        
                    }
                }

                var tile = world.GetTile(player.pos);

                float moveDistance = 0.0f;
                if (tile == World.TerrainTile.WATER)
                    moveDistance = 0.015f;
                else
                    moveDistance = 0.05f;

                Vector2 movement = Vector2.Zero;                

                var gamePadCap = GamePad.GetCapabilities(PlayerIndex.One);
                if (gamePadCap.IsConnected)
                {
                    if (gamePadCap.HasLeftXThumbStick)
                    {
                        movement += new Vector2(padState.ThumbSticks.Left.X * moveDistance, 0);
                    }
                    if (gamePadCap.HasLeftYThumbStick)
                    {
                        movement -= new Vector2(0, padState.ThumbSticks.Left.Y * moveDistance);
                    }
                }


                if (keyState.IsKeyDown(Keys.Right))
                {
                    movement += new Vector2(moveDistance, 0);
                }
                if (keyState.IsKeyDown(Keys.Left))
                {
                    movement -= new Vector2(moveDistance, 0);
                }
                if (keyState.IsKeyDown(Keys.Up))
                {
                    movement -= new Vector2(0, moveDistance);
                }
                if (keyState.IsKeyDown(Keys.Down))
                {
                    movement += new Vector2(0, moveDistance);
                }



                if (movement.Length() > moveDistance)
                {
                    movement = Vector2.Normalize(movement) * moveDistance;
                }


                player.pos += movement;
                player.Update(gameTime);

                float xDiff = worldPos.X - player.pos.X;
                float yDiff = worldPos.Y - player.pos.Y;

                float threshold = 1.5f;
                if (xDiff > threshold)
                {
                    worldPos.X += movement.X;
                }
                else if (xDiff < -threshold)
                {
                    worldPos.X += movement.X;
                }

                if (yDiff > threshold)
                {
                    worldPos.Y += movement.Y;
                }
                else if (yDiff < -threshold)
                {
                    worldPos.Y += movement.Y;
                }

                foreach (var npc in world.npcs)
                {
                    npc.Update(gameTime, player, Content);
                }
                float closestNPCDist;
                (closestNPC, closestNPCDist) =
                    world.npcs.ClosestTo(player);
                                 

                if (IsNewAction(Action.MAIN) && closestNPCDist < actionDist)
                {
                    command = GameCommand.TALK;
                }

            }
            AdvanceState(command);
            if (IsNewButtonPress(Buttons.X))
            {
                renderFancyTiles = !renderFancyTiles;
            }


            lastPadState = padState;
            lastKeyState = keyState;
        }


        
        public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color,float scale , float depth)
        {
            batch.DrawString(font, s, pos, color, 0, Vector2.Zero, scale, SpriteEffects.None, depth);
        }

        public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color, float depth)
        {
            batch.DrawString(font, s, pos, color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, depth);
        }

        public static void Draw(Texture2D tex, Rectangle rect, float depth)
        {
            batch.Draw(tex, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public static void Draw(Texture2D tex, Vector2 pos,Vector2 scale, float depth)
        {
            batch.Draw(tex, pos, null, Color.White,0, Vector2.Zero, scale,SpriteEffects.None,depth);
        }

        public static void Draw(Texture2D tex, Vector2 pos, Color color,Vector2 scale, float depth)
        {
            batch.Draw(tex, pos, null, color, 0, Vector2.Zero, scale, SpriteEffects.None, depth);
        }

        protected override void Draw(GameTime gameTime)
        {

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            var fps = string.Format("FPS:{0}", frameCounter.AverageFramesPerSecond);

            //GraphicsDevice.Clear(Color.Black);                                    
            for (int i = 0; i < pendingTilePool.Count; i++)
            {
                tilePool.Add(pendingTilePool[i]);
            }
            pendingTilePool.Clear();

            startX = (int)Floor(worldPos.X - numTilesX / 2.0f);
            endX = (int)Ceiling(worldPos.X + numTilesX / 2.0f);
            startY = (int)Floor(worldPos.Y - numTilesY / 2.0f);
            endY = (int)Ceiling(worldPos.Y + numTilesY / 2.0f);

            screenCenter = new Vector2(windowWidth / 2, windowHeight / 2);

            offset = worldPos * (float)World.tileSize - screenCenter;

            batch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {

                    Texture2D tile = world.GetTex(x, y);

                    var screenPos = new Vector2(x, y) * World.tileSize - offset;
                    if (OnScreen(screenPos, World.tileSize, World.tileSize))                                                                    
                        batch.Draw(tile, new Vector2(screenPos.X, screenPos.Y),null, Color.White,0,Vector2.Zero,1f,SpriteEffects.None,1.0f);
                    
                }
            }



            foreach (var npc in world.npcs)
            {
                var screenPos = npc.pos*World.tileSize-offset;
                if (OnScreen(screenPos,CharSprites.CHAR_SIZE,CharSprites.CHAR_SIZE))
                { 
                    npc.Draw(screenPos);
                }
            }

            foreach (var resource in world.resources)
            {
                var screenPos = resource.pos * World.tileSize - offset;
                if (OnScreen(screenPos,resource.width,resource.height))
                {                 
                    resource.Draw(screenPos);
                }
            }

            foreach (var resource in world.deadResources)
            {
                var screenPos = resource.pos * World.tileSize - offset;
                if (OnScreen(screenPos, resource.width, resource.height))
                {
                    resource.Draw(screenPos);
                }

            }

            var playerScreenPos = player.pos * World.tileSize - offset;
            player.Draw(playerScreenPos);

            if (state == GameState.DIALOGUE)
            {
                Dialogue.Draw();
            }            

            batch.DrawString(mainFont, fps, new Vector2(0, 40), Color.Yellow);
#if DEBUG
            batch.DrawString(mainFont, "x:" + (int)worldPos.X + "," + (int)worldPos.Y + "  sprited:" + npcSprited, Vector2.Zero, Color.Yellow);

#endif
            batch.End();
            base.Draw(gameTime);
        }
    }
}
