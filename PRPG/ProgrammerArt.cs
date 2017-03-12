using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PRPG {

    public static class ProgrammerArt {            
        public static Texture2D GetSolidTex(int w, int h, Color color) {
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
