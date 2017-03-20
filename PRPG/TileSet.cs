using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PRPG
{
    public class TileSet
    {

        public Texture2D[,] waterBorders;
        public Texture2D[] waterSingles;
        public Texture2D[] waterFull;

        public Texture2D[] grassFull;

        public const int TILE_SIZE = 32;

        public TileSet(ContentManager content)
        {
            Texture2D waterSheet = content.Load<Texture2D>("Tiles/water");
            Texture2D grassSheet = content.Load<Texture2D>("Tiles/grass");

            waterFull = new Texture2D[3];
            for (int i = 0; i < waterFull.Length; i++) {

                waterFull[i] = GetSubTexture(waterSheet, new Rectangle(i * TILE_SIZE, 5 * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }

            grassFull = new Texture2D[3];
            for (int i = 0; i < grassFull.Length; i++) {

                grassFull[i] = GetSubTexture(grassSheet, new Rectangle(i * TILE_SIZE, 5 * TILE_SIZE, TILE_SIZE, TILE_SIZE));
            }
            

            waterBorders = new Texture2D[3,3];            
            for (int x = 0; x < 3; x++) {
                for (int y = 0; y < 3; y++) {
                    int yIndex = y * TILE_SIZE + 2 * TILE_SIZE;
                    int xIndex = x * TILE_SIZE;
                    var water = GetSubTexture(waterSheet, new Rectangle(xIndex, yIndex, TILE_SIZE, TILE_SIZE));
                    
                    waterBorders[x, y] = ComposeTextures(RandUtil.Index(grassFull), water);
                }
            }

            waterSingles = new Texture2D[2];
            waterSingles[0] = GetSubTexture(waterSheet, new Rectangle(0, 0, TILE_SIZE, TILE_SIZE));
            waterSingles[1] = GetSubTexture(waterSheet, new Rectangle(0, TILE_SIZE, TILE_SIZE, TILE_SIZE));

           
            

        }


        public Texture2D GetSubTexture(Texture2D srcTex, Rectangle rect)
        {
            var renderTarget = new RenderTarget2D(PRPGame.graphics, rect.Width, rect.Height);
            PRPGame.graphics.SetRenderTarget(renderTarget);
            PRPGame.graphics.Clear(Color.Transparent);
            PRPGame.batch.Begin();
            PRPGame.batch.Draw(srcTex, Vector2.Zero, rect, Color.White);
            PRPGame.batch.End();
            PRPGame.graphics.SetRenderTarget(null);
            return renderTarget;
        }

        public Texture2D ComposeTextures(Texture2D tex1, Texture2D tex2)
        {            
            var renderTarget = new RenderTarget2D(PRPGame.graphics, tex1.Width, tex1.Height);
            PRPGame.graphics.SetRenderTarget(renderTarget);
            PRPGame.graphics.Clear(Color.Transparent);
            PRPGame.batch.Begin();
            PRPGame.batch.Draw(tex1, Vector2.Zero,Color.White);
            PRPGame.batch.Draw(tex2, Vector2.Zero, Color.White);
            PRPGame.batch.End();
            PRPGame.graphics.SetRenderTarget(null);
            return renderTarget;
        }

    }
}
