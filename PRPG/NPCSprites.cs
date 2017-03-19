using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace PRPG
{
    public class NPCSprites
    {
        public static Texture2D[] maleBodySheets;
        public static Texture2D[] femaleBodySheets;

        public static Rectangle testSpriteRect = new Rectangle(0, 0, 64, 64);

        public NPCSprites() { }

        public static void Initialize()
        {
            var maleBodyFilenames = Directory.GetFiles("LPC/body/male");
            var femaleBodyFilenames = Directory.GetFiles("LPC/body/female");

            femaleBodySheets = femaleBodyFilenames.Select(file => {
                using (var fStream = new FileStream(file, FileMode.Open)) {
                    return Texture2D.FromStream(PRPGame.graphics, fStream);
                }
            }).ToArray();

            maleBodySheets = maleBodyFilenames.Select(file => {
                using (var fStream = new FileStream(file, FileMode.Open)) {
                    return Texture2D.FromStream(PRPGame.graphics, fStream);
                }
            }).ToArray();

        }

    }
}
