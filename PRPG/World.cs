using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static PRPG.GraphUtils;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;

namespace PRPG
{
    public class World
    {
        public enum TerrainTile : int
        {
            WATER = 0, GRASS, DIRT, ROCK, SNOW
        };


        public readonly int width;
        public readonly int height;
        public const int tileSize = 64;
        public TerrainTile[,] tiles;
        public Color[] pallette;
        public TerrainTile[] tilePallette;
        public Dictionary<TerrainTile, Texture2D> simpleTex;
        public LRACachePool<int, Texture2D> texCache;
        public const double cityDensity = 1.0 / 1000.0;
        public static readonly int CPUCount = Environment.ProcessorCount;
        public static readonly int chunkSize = tileSize / CPUCount;
        public NPC[] npcs;
        Color[] texColor;
        Task[] fillTasks;

        public World(int w, int h)
        {
            fillTasks = new Task[CPUCount];
            
            texColor = new Color[tileSize * tileSize];
            texCache = new LRACachePool<int, Texture2D>(1000);
            simpleTex = new Dictionary<TerrainTile, Texture2D>();
            simpleTex.Add(TerrainTile.WATER, GetSolidTex(tileSize, tileSize, Color.Blue));
            simpleTex.Add(TerrainTile.GRASS, GetSolidTex(tileSize, tileSize, Color.ForestGreen));
            simpleTex.Add(TerrainTile.DIRT, GetSolidTex(tileSize, tileSize, Color.SaddleBrown));
            simpleTex.Add(TerrainTile.ROCK, GetSolidTex(tileSize, tileSize, Color.Gray));
            simpleTex.Add(TerrainTile.SNOW, GetSolidTex(tileSize, tileSize, Color.White));


            width = w;
            height = h;

            tilePallette = new TerrainTile[100];
            pallette = new Color[100];
            for (int i = 0; i < 25; i++) {
                tilePallette[i] = TerrainTile.WATER;
                pallette[i] = Color.DarkBlue;
            }
            for (int i = 25; i < 30; i++) {
                tilePallette[i] = TerrainTile.WATER;
                pallette[i] = Color.Blue;
            }
            for (int i = 30; i < 32; i++) {
                tilePallette[i] = TerrainTile.GRASS;
                pallette[i] = Color.LightGoldenrodYellow;
            }
            for (int i = 32; i < 55; i++) {
                tilePallette[i] = TerrainTile.GRASS;
                pallette[i] = Color.ForestGreen;
            }
            for (int i = 55; i < 70; i++) {
                tilePallette[i] = TerrainTile.DIRT;
                pallette[i] = Color.SaddleBrown;
            }
            for (int i = 70; i < 90; i++) {
                tilePallette[i] = TerrainTile.ROCK;
                pallette[i] = Color.Lerp(Color.Gray, Color.White, (float)(i - 70) / 20.0f);
            }
            for (int i = 90; i < 100; i++) {
                tilePallette[i] = TerrainTile.SNOW;
                pallette[i] = Color.White;
            }


            tiles = new TerrainTile[w, h];


            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    float f = Noise.FractalFBM(1337, 0.01f * x, 0.01f * y);

                    int tileIndex = (int)Math.Floor(MathHelper.Clamp(f, 0.0f, 1.0f) * (tilePallette.Length - 1));
                    tiles[x, y] = tilePallette[tileIndex];
                }
            }



            npcs = new NPC[5000];

            var worldArea = w * h;
            var numCities = (int)(worldArea * cityDensity);

            int npcIndex = 0;
            int npcsPerCity = (int)(((double)npcs.Length * 0.75) / (double)numCities);
            for (int i = 0; i < numCities; i++) {
                var cityPos = new Vector2(RandUtil.IntEx(4, w - 4), RandUtil.IntEx(4, h - 4));
                var tile = tiles[(int)cityPos.X, (int)cityPos.Y];
                if (tile != TerrainTile.WATER) {
                    for (int j = 0; j < npcsPerCity; j++) {
                        var npcVector = new Vector2(RandUtil.Float(-2.0f, 2.0f), RandUtil.Float(-2.0f, 2.0f));
                        var npcPos = cityPos + npcVector;
                        tile = tiles[(int)npcPos.X, (int)npcPos.Y];
                        if (tile != TerrainTile.WATER) {
                            npcs[npcIndex] = new NPC(npcPos);
                            npcIndex++;
                        }
                    }
                }
            }

            while (npcIndex < npcs.Length) {
                var npcPos = new Vector2(RandUtil.Float(w - 1), RandUtil.Float(h - 1));
                var tile = tiles[(int)npcPos.X, (int)npcPos.Y];
                if (tile != TerrainTile.WATER) {
                    npcs[npcIndex] = new NPC(npcPos);
                    npcIndex++;
                }
            }


        }



        public Texture2D GetTexSimple(int x, int y)
        {
            return simpleTex[tiles[x, y]];

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void fillTex(int x, int y,int startTy, int endTy)
        {            
            for (int ty = startTy; ty < endTy; ty++) {
                float fy = y + ((float)ty / (float)tileSize);
                int tyIndex = ty * tileSize;
                for (int tx = 0; tx < tileSize; tx++) {
                    float fx =x + ((float)tx / (float)tileSize);
                    float f = Noise.FractalFBM(1337, 0.01f * fx, 0.01f * fy);
                    int colorIndex = (int)Math.Floor(f * ((float)pallette.Length - 1.0f));
                    colorIndex = MathHelper.Clamp(colorIndex, 0, pallette.Length);
                    texColor[tyIndex + tx] = pallette[colorIndex];
                }
            }
        }

        public Texture2D GetTex(int x, int y)
        {
            int key = x * 30_000 + y;

            var tex = texCache.Get(key);
            if (tex != null) {
                return tex;
            }
            else {
                tex = texCache.GetEvicted();                
                if (tex == null)
                    tex = new Texture2D(PRPGame.graphics, tileSize, tileSize);
                
                int chunksRemaining = tileSize;
                for (int i = 0; i < CPUCount; i++) {
                    int innerI = i;
                    if (chunkSize > chunksRemaining) {
                        fillTasks[innerI] = Task.Factory.StartNew(() =>
                        { fillTex(x,y,tileSize - chunksRemaining, tileSize); });
                    }
                    else {
                        int start = innerI * chunkSize;
                        fillTasks[innerI] = Task.Factory.StartNew(() =>
                        { fillTex(x,y,start, start + chunkSize); });
                    }
                    chunksRemaining -= chunkSize;
                }

                Task.WaitAll(fillTasks);
                
                tex.SetData(texColor);
                texCache.Add(key, tex);
                return tex;
            }
        }


        public TerrainTile GetTile(Vector2 pos)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;
            return tiles[x, y];
        }


    }
}
