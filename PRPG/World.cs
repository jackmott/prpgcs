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
        public const int TileSize = 128;
        public NPC[] npcs;

        public World(int w, int h) {
            
            TileTextureDict = new Dictionary<TerrainTile, Texture2D>();
            TileTextureDict.Add(TerrainTile.WATER, GetSolidTex(TileSize, TileSize, Color.Blue));
            TileTextureDict.Add(TerrainTile.GRASS, GetSolidTex(TileSize, TileSize, Color.Green));
            TileTextureDict.Add(TerrainTile.ROCK, GetSolidTex(TileSize, TileSize, Color.Gray));
            TileTextureDict.Add(TerrainTile.SNOW, GetSolidTex(TileSize, TileSize, Color.White));

            npcs = new NPC[1000];
            var r = new Random();
            for (int i = 0; i < npcs.Length; i++) {
                npcs[i] = new NPC(new Vector2((float)r.NextDouble() * w, (float)r.NextDouble() * h));
            }

            width = w;
            height = h;            
            tiles = new TerrainTile[w, h];
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    float f = Noise.Simplex(1337, 0.1f*x, 0.1f*y);
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
