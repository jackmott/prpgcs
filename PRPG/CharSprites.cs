using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace PRPG
{
    public enum Gender { Male,Female}

    public class CharSprites
    {
        public static Texture2D[] maleBodySheets;
        public static Texture2D[] femaleBodySheets;
        public static Texture2D[] maleHairSheets;
        public static Texture2D[] femaleHairSheets;
        public static Texture2D[] shirtSheets;
        public static Texture2D[] pantsSheets;
        public static Texture2D[] shoeSheets;
        public static Texture2D[] maleEyeSheets;
        public static Texture2D[] femaleEyeSheets;



        public static int UP = 0;
        public static int LEFT = 1;
        public static int DOWN = 2;        
        public static int RIGHT = 3;

        public const int CHAR_SIZE = 64;
        const int WALKING_INDEX = 8;
        const int WALKING_WIDTH = 9;
        public Rectangle[,] walking;

        public Texture2D baseSheet;
        public Texture2D hairSheet;
        public Texture2D eyeSheet;
        public Texture2D shirtSheet;
        public Texture2D pantSheet; 
        public Texture2D shoeSheet;
        public Color hairColor;

        public CharSprites(Gender gender,ContentManager content) {
#if DEBUG
            PRPGame.npcSprited++;
#endif
            
            if (gender == Gender.Male) {
                baseSheet = RandUtil.Index(maleBodySheets);
                hairSheet = RandUtil.Index(maleHairSheets);
                eyeSheet = RandUtil.Index(maleEyeSheets);
                
            } else {
                baseSheet = RandUtil.Index(femaleBodySheets);
                hairSheet = RandUtil.Index(femaleHairSheets);
                eyeSheet = RandUtil.Index(femaleHairSheets);
            }


            // Compose a bunch of sprite sheets to make a uniqe npc as they
            // come on screen. The sheets contain all the animation frames.
            shirtSheet = RandUtil.Index(shirtSheets);            
            pantSheet = RandUtil.Index(pantsSheets);
            shoeSheet = RandUtil.Index(shoeSheets);

            hairColor = new Color(new Vector3(RandUtil.Float(1.0f), RandUtil.Float(1.0f), RandUtil.Float(1.0f)));
         
            walking = new Rectangle[4, 9];
            for (int y = 0; y < 4; y++) { 
                for (int x = 0; x < 9; x++) {
                    int walkIndex = y+ WALKING_INDEX;
                    walking[y, x] = new Rectangle(x * CHAR_SIZE, walkIndex * CHAR_SIZE,CHAR_SIZE,CHAR_SIZE);
                }
            }
        }

        public static string[] LoadFileNames(ContentManager contentManager,string contentFolder, bool subDirs = true)
        {
            SearchOption searchOption = SearchOption.AllDirectories;
            if (!subDirs) searchOption = SearchOption.TopDirectoryOnly;
            var fileNames = Directory.GetFiles(contentManager.RootDirectory + "/" + contentFolder,"*.*",searchOption);
            for (int i = 0; i < fileNames.Length;i++) {
                var contentFolderIndex = fileNames[i].LastIndexOf(contentFolder);
                fileNames[i] = contentFolder+fileNames[i].Substring(contentFolderIndex+contentFolder.Length);
                fileNames[i] = fileNames[i].Substring(0, fileNames[i].Length - 4);
            }
            return fileNames;
        }
        public static T[] LoadAllContent<T>(ContentManager contentManager, string contentFolder,bool subDirs = true)
        {
            
            return LoadFileNames(contentManager,contentFolder,subDirs).Select(x => contentManager.Load<T>(x)).ToArray();                
        }
      

        public static void Initialize(ContentManager content)
        {
         
            femaleBodySheets = LoadAllContent<Texture2D>(content,"LPC/body/human/female",false);
            maleBodySheets = LoadAllContent<Texture2D>(content, "LPC/body/human/male",false);

            maleHairSheets = LoadAllContent<Texture2D>(content, "LPC/hair/male");
            femaleHairSheets = LoadAllContent<Texture2D>(content, "LPC/hair/female");

            shirtSheets = LoadAllContent<Texture2D>(content, "LPC/torso/shirts/longsleeve");
            pantsSheets = LoadAllContent<Texture2D>(content, "LPC/legs/pants");

            shoeSheets = LoadAllContent<Texture2D>(content, "LPC/feet/shoes/male");

            maleEyeSheets = LoadAllContent<Texture2D>(content, "LPC/body/male_headparts/eyes");
            femaleEyeSheets = LoadAllContent<Texture2D>(content, "LPC/body/female_headparts/eyes");

        }

    }
}