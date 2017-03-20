using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace PRPG
{

    public static class GraphUtils
    {


        public static Color[] GetPallete()
        {
            var pal = new List<Color>(2560);
            int waterRange = 800;
            for (int i = 0; i < waterRange; i++) {
                pal.Add(Color.Lerp(Color.DarkBlue, Color.DeepSkyBlue, (float)i / (float)waterRange));
            }
            pal.Add(Color.LightGoldenrodYellow);
            pal.Add(Color.LightGoldenrodYellow);
            pal.Add(Color.LightGoldenrodYellow);
            pal.Add(Color.LightGoldenrodYellow);
            
            int grassRange = 1100;
            for (int i = 0; i < grassRange; i++) {
                pal.Add(Color.Lerp(Color.ForestGreen, Color.DarkGreen, (float)i / (float)grassRange));
            }
            int dirtRange = 100;
            for (int i = 0; i < dirtRange; i++) {
                pal.Add(Color.Lerp(Color.DarkGreen, Color.SaddleBrown, (float)i / (float)dirtRange));
            }
            float rockRange = 550;
            for (int i = 0; i < rockRange; i++) {
                pal.Add(Color.Lerp(Color.Gray, Color.White, (float)i / (float)rockRange));
            }
            return pal.ToArray();
        }

        public static Texture2D GetSubTex16x16(Texture2D srcTex, int x, int y)
        {
            return GetSubTex(srcTex, x, y, 16, 16, 1);
        }
        public static Texture2D GetSubTex(Texture2D srcTex, int x, int y, int w, int h, int spacing)
        {
            var srcRect = new Rectangle(x * (w + spacing), y * (h + spacing), w, h);
            Texture2D resultTex = new Texture2D(PRPGame.graphics, w, h);
            Color[] data = new Color[w * h];
            srcTex.GetData(0, srcRect, data, 0, data.Length);
            resultTex.SetData(data);
            return resultTex;
        }

        public static Texture2D MergeTex(Texture2D bottom, Texture2D top)
        {
            int w = top.Width;
            int h = top.Height;
            Color[] bottomData = new Color[w * h];
            Color[] topData = new Color[w * h];
            var rect = new Rectangle(0, 0, w, h);
            bottom.GetData(0, rect, bottomData, 0, bottomData.Length);
            top.GetData(0, rect, topData, 0, topData.Length);

            Color[] resultData = new Color[w * h];
            for (int i = 0; i < topData.Length; i++) {
                var c = topData[i];
                if (c == Color.Transparent || c == Color.TransparentBlack) {
                    resultData[i] = bottomData[i];
                }
                else {
                    resultData[i] = topData[i];
                }
            }
            var result = new Texture2D(PRPGame.graphics, w, h);
            result.SetData(resultData);
            return result;
        }

        public static Texture2D GetSolidTex(int w, int h, Color color)
        {
            var colors = new Color[w * h];
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = color;
            }
            var tex = new Texture2D(PRPGame.graphics, w, h);
            tex.SetData(colors);
            return tex;
        }
    }

}
