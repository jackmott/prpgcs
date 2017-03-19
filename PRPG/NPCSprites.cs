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

        public enum Dir : int {UP,LEFT,DOWN,RIGHT };
        const int CHAR_SIZE = 64;
        const int WALKING_INDEX = 8;
        const int WALKING_WIDTH = 9;
        public Rectangle[,] walking;
        public Texture2D spriteSheet;

        public NPCSprites(int index, bool male) {            
            if (male) {
                spriteSheet = maleBodySheets[index];
            } else {
                spriteSheet = femaleBodySheets[index];
            }

            var dirs = (Dir[])Enum.GetValues(typeof(Dir));
            walking = new Rectangle[4, 9];
            //walk y index starts at 8 ends at 12
            //walk width is 9
            foreach (var dir in dirs) { 
                for (int x = 0; x < 9; x++) {
                    int y = (int)dir + WALKING_INDEX;
                    walking[(int)dir, x] = new Rectangle(x * CHAR_SIZE, y * CHAR_SIZE,CHAR_SIZE,CHAR_SIZE);
                }
            }
        }

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
