using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static PRPG.GraphUtils;
using System;
using Microsoft.Xna.Framework.Content;

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
        public const int tileSize =64;        
        public Color[] pallette;
        public TerrainTile[] tilePallette;
        public Dictionary<TerrainTile, Texture2D> simpleTex;
        public LRACache<int, Texture2D> texCache;
        public const double cityDensity = 1.0 / 5000.0;
        public NPC[] npcs;
        public Resource[] resources;
        Color[] texColor;



        public World(int w, int h, ContentManager content)
        {
            
            Noise.InitNoise(tileSize, 5, 2.0f, 0.6f,0.6f);
            texColor = new Color[(tileSize) * (tileSize)];
            texCache = new LRACache<int, Texture2D>(10000);
            simpleTex = new Dictionary<TerrainTile, Texture2D>();
            simpleTex.Add(TerrainTile.WATER, GetSolidTex(tileSize, tileSize, Color.Blue));
            simpleTex.Add(TerrainTile.GRASS, GetSolidTex(tileSize, tileSize, Color.ForestGreen));
            simpleTex.Add(TerrainTile.DIRT, GetSolidTex(tileSize, tileSize, Color.SaddleBrown));
            simpleTex.Add(TerrainTile.ROCK, GetSolidTex(tileSize, tileSize, Color.Gray));
            simpleTex.Add(TerrainTile.SNOW, GetSolidTex(tileSize, tileSize, Color.White));


            width = w;
            height = h;

            var pal = new List<Color>(2560);
            var tilePal = new List<TerrainTile>(2560);
            int waterRange = 800;
            for (int i = 0; i < waterRange; i++) {
                pal.Add(Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, (float)i / (float)waterRange));
                tilePal.Add(TerrainTile.WATER);
            }

            tilePal.Add(TerrainTile.GRASS);
            pal.Add(Color.LightGoldenrodYellow);
            tilePal.Add(TerrainTile.GRASS);
            pal.Add(Color.LightGoldenrodYellow);
            tilePal.Add(TerrainTile.GRASS);
            pal.Add(Color.LightGoldenrodYellow);
            tilePal.Add(TerrainTile.GRASS);
            pal.Add(Color.LightGoldenrodYellow);

            int grassRange = 1100;
            for (int i = 0; i < grassRange; i++) {
                pal.Add(Color.Lerp(Color.ForestGreen, Color.DarkGreen, (float)i / (float)grassRange));
                tilePal.Add(TerrainTile.GRASS);
            }
            int dirtRange = 100;
            for (int i = 0; i < dirtRange; i++) {
                pal.Add(Color.Lerp(Color.DarkGreen, Color.SaddleBrown, (float)i / (float)dirtRange));
                tilePal.Add(TerrainTile.DIRT);
            }
            float rockRange = 550;
            for (int i = 0; i < rockRange; i++) {
                pal.Add(Color.Lerp(Color.Gray, Color.White, (float)i / (float)rockRange));
                if (i < rockRange / 2)
                    tilePal.Add(TerrainTile.ROCK);
                else
                    tilePal.Add(TerrainTile.SNOW);
            }

            tilePallette = tilePal.ToArray();
            pallette = pal.ToArray();

            resources = new Resource[2500];
            for (int i = 0; i < 1500 ;i++)
            {                
                resources[i] = new Tree(RandUtil.Vector2(width, height), RandUtil.Int(1, 3),content);
            }
            for (int i = 1500; i < 2000; i++)
            {
                resources[i] = new IronMine(RandUtil.Vector2(width, height), RandUtil.Int(10, 1000),content);
            }
            for (int i = 2000; i < 2500; i++)
            {
                resources[i] = new CoalMine(RandUtil.Vector2(width, height), RandUtil.Int(10, 1000),content);
            }
            
            

            npcs = new NPC[1000];

            var worldArea = w * h;
            var numCities =  (int)(worldArea * cityDensity);

            int npcIndex = 0;
            int npcsPerCity = 5;
            for (int i = 0; i < numCities; i++) {
                var cityPos = new Vector2(RandUtil.IntEx(4, w - 4), RandUtil.IntEx(4, h - 4));
                var tile = GetTile(cityPos);
                if (tile != TerrainTile.WATER) {
                    for (int j = 0; j < npcsPerCity; j++) {
                        var npcVector = new Vector2(RandUtil.Float(-2.0f, 2.0f), RandUtil.Float(-2.0f, 2.0f));
                        var npcPos = cityPos + npcVector;
                        tile = GetTile(npcPos);
                        if (tile != TerrainTile.WATER) {
                            npcs[npcIndex] = new NPC(npcPos, content);
                            npcIndex++;
                        }
                    }
                }
            }

            while (npcIndex < npcs.Length) {
                var npcPos = new Vector2(RandUtil.Float(w - 1), RandUtil.Float(h - 1));
                var tile = GetTile(npcPos);
                if (tile != TerrainTile.WATER) {
                    npcs[npcIndex] = new NPC(npcPos, content);
                    npcIndex++;
                }
            }


        }


        public unsafe Texture2D GetTex(int x, int y)
        {
            int key = (x << 12) + y;
            var tex = texCache.Get(key);
            if (tex != null) {
                return tex;
            }
            else {
                if (PRPGame.tilePool.Count > 0) {
                    tex = PRPGame.tilePool[PRPGame.tilePool.Count - 1];
                    PRPGame.tilePool.RemoveAt(PRPGame.tilePool.Count - 1);
                }
                else {
                    tex = new Texture2D(PRPGame.graphics, tileSize, tileSize);
                }
                float* noise = Noise.GetNoiseBlock((float)x, (float)y);
#if DEBUG
                float max = float.MinValue;
                float min = float.MaxValue;
#endif
                for (int i = 0; i < tileSize * tileSize; i++) {                    
#if DEBUG
                    float f = noise[i];
                    if (f < min) min = f;
                    if (f > max) max = f;
#endif
                    int colorIndex = MathHelper.Clamp((int)(noise[i] * ((float)pallette.Length - 1.0f)), 0, pallette.Length - 1);
                    texColor[i] = pallette[colorIndex];

                }
#if DEBUG
           //     Console.WriteLine("min:" + min + " max:" + max + " range:" + (max - min));
#endif

                tex.SetData(texColor);
                var evictedTex = texCache.Add(key, tex);
                if (evictedTex != null) PRPGame.pendingTilePool.Add(evictedTex);
                return tex;
            }
        }


        public TerrainTile GetTile(Vector2 pos)
        {
            float f = Noise.GetNoisePoint(pos.X , pos.Y);
            int colorIndex = MathHelper.Clamp((int)(f * ((float)pallette.Length - 1.0f)), 0, pallette.Length - 1);
            return tilePallette[colorIndex];
        }


    }
}
