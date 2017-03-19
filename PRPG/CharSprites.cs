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
    public class CharSprites
    {
        public static Texture2D[] maleBodySheets;
        public static Texture2D[] femaleBodySheets;

        public static int UP = 0;
        public static int LEFT = 1;
        public static int DOWN = 2;        
        public static int RIGHT = 3;

        const int CHAR_SIZE = 64;
        const int WALKING_INDEX = 8;
        const int WALKING_WIDTH = 9;
        public Rectangle[,] walking;
        public Texture2D spriteSheet;

        public CharSprites(int index, bool male) {            
            if (male) {
                spriteSheet = maleBodySheets[index];
            } else {
                spriteSheet = femaleBodySheets[index];
            }

            
            walking = new Rectangle[4, 9];
            //walk y index starts at 8 ends at 12
            //walk width is 9
            for (int y = 0; y < 4; y++) { 
                for (int x = 0; x < 9; x++) {
                    int walkIndex = y+ WALKING_INDEX;
                    walking[y, x] = new Rectangle(x * CHAR_SIZE, walkIndex * CHAR_SIZE,CHAR_SIZE,CHAR_SIZE);
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
