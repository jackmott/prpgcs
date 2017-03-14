using System;
using static System.Math;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using static PRPG.ProgrammerArt;

namespace PRPG {
    

    public class World {
        public enum TerrainTile { WATER, GRASS, ROCK, SNOW };
        public static Dictionary<TerrainTile, Texture2D> TileTextureDict;
        public TerrainTile[,] tiles;
        public readonly int width;
        public readonly int height;
        public const int tileSize = 64;
        public const double cityDensity = 1.0 / 1000.0;
        public NPC[] npcs;

        public World(int w, int h) {
            
            TileTextureDict = new Dictionary<TerrainTile, Texture2D>();
            TileTextureDict.Add(TerrainTile.WATER, GetSolidTex(tileSize, tileSize, Color.Blue));
            TileTextureDict.Add(TerrainTile.GRASS, GetSolidTex(tileSize, tileSize, Color.Green));
            TileTextureDict.Add(TerrainTile.ROCK, GetSolidTex(tileSize, tileSize, Color.Gray));
            TileTextureDict.Add(TerrainTile.SNOW, GetSolidTex(tileSize, tileSize, Color.White));

            npcs = new NPC[5000];
            
            var worldArea = w * h;
            var numCities = (int)(worldArea * cityDensity);

            int npcIndex = 0;
            int npcsPerCity =(int)( ((double)npcs.Length * 0.75) / (double)numCities);
            for (int i = 0; i < numCities; i++) {
                var cityPos = new Vector2(RandUtil.IntEx(0, w), RandUtil.IntEx(0, h));
                for (int j = 0; j < npcsPerCity;j++) {
                    var npcVector = new Vector2(RandUtil.Float(-2.0f, 2.0f), RandUtil.Float(-2.0f, 2.0f));
                    var npcPos = cityPos + npcVector;
                    npcs[npcIndex] = new NPC(npcPos);
                    npcIndex++;
                }
            }

            for (; npcIndex < npcs.Length; npcIndex++) {
                npcs[npcIndex] = new NPC(new Vector2(RandUtil.Float(w),RandUtil.Float(h)));
            }

            width = w;
            height = h;            
            tiles = new TerrainTile[w, h];
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    float f = Noise.Simplex(1337, 0.05f*x, 0.05f*y);
                    if (f < -0.15f)
                        tiles[x, y] = TerrainTile.WATER;
                    else if (f < 0.15f)
                        tiles[x, y] = TerrainTile.GRASS;
                    else if (f < 0.45f)
                        tiles[x, y] = TerrainTile.ROCK;
                    else
                        tiles[x, y] = TerrainTile.SNOW;
                }
            }
        }


        public TerrainTile GetTile(Vector2 pos) {
            int x = (int)pos.X;
            int y = (int)pos.Y;
            return tiles[x,y];
        }

        public Texture2D GetTileTexture(int x, int y) {
            return TileTextureDict[tiles[x, y]];
        }
        

    }
}
