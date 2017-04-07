using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;
using JM.LinqFaster;

namespace PRPG
{
    public enum Gender { Male, Female }

    public class CharSprites
    {
        //Body Sheets
        public static Texture2D[] maleBodySheets;
        public static Texture2D[] femaleBodySheets;
        public static Texture2D[] maleHairSheets;
        public static Texture2D[] femaleHairSheets;
        public static Texture2D[] maleEyeSheets;
        public static Texture2D[] femaleEyeSheets;

        //Clothing Sheets        
        public static Texture2D[] shirtSheets;
        public static Texture2D[] pantsSheets;
        public static Texture2D[] shoeSheets;

        //Weapon/Tool Sheets
        public static Texture2D saberSheet;






        public static int UP = 0;
        public static int LEFT = 1;
        public static int DOWN = 2;
        public static int RIGHT = 3;

        public const int CHAR_SIZE = 64;
        public const int OVERSIZE_WEAPON_SIZE = 192;
        public const int WALKING_INDEX = 8;
        public const int WALKING_WIDTH = 9;
        public const int SWING_WIDTH = 6;
        public const int SWING_INDEX = 12;


        public Rectangle[,] walkingAnimation;
        public Rectangle[,] swingAnimation;
        public Rectangle[,] oversizeWeaponAnimation;

        public Texture2D baseSheet;
        public Texture2D hairSheet;
        public Texture2D eyeSheet;
        public Texture2D shirtSheet;
        public Texture2D pantSheet;
        public Texture2D shoeSheet;
        public Color hairColor;

        public CharSprites(Gender gender, ContentManager content)
        {
#if DEBUG
            PRPGame.npcSprited++;
#endif

            if (gender == Gender.Male)
            {
                baseSheet = RandUtil.Index(maleBodySheets);
                hairSheet = RandUtil.Index(maleHairSheets);
                eyeSheet = RandUtil.Index(maleEyeSheets);

            }
            else
            {
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

            walkingAnimation = new Rectangle[4, WALKING_WIDTH];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < WALKING_WIDTH; x++)
                {
                    int walkIndex = y + WALKING_INDEX;
                    walkingAnimation[y, x] = new Rectangle(x * CHAR_SIZE, walkIndex * CHAR_SIZE, CHAR_SIZE, CHAR_SIZE);
                }
            }


            swingAnimation = new Rectangle[4, SWING_WIDTH];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < SWING_WIDTH; x++)
                {
                    int swingIndex = y + SWING_INDEX;
                    swingAnimation[y, x] = new Rectangle(x * CHAR_SIZE, swingIndex * CHAR_SIZE, CHAR_SIZE, CHAR_SIZE);
                }
            }

            oversizeWeaponAnimation = new Rectangle[4, SWING_WIDTH];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < SWING_WIDTH; x++)
                {
                    int swingIndex = y;
                    oversizeWeaponAnimation[y, x] = new Rectangle(x * OVERSIZE_WEAPON_SIZE, swingIndex * OVERSIZE_WEAPON_SIZE, OVERSIZE_WEAPON_SIZE, OVERSIZE_WEAPON_SIZE);
                }
            }


        }

        public static string[] LoadFileNames(ContentManager contentManager, string contentFolder, bool subDirs = true)
        {
            SearchOption searchOption = SearchOption.AllDirectories;
            if (!subDirs) searchOption = SearchOption.TopDirectoryOnly;
            var fileNames = Directory.GetFiles(contentManager.RootDirectory + "/" + contentFolder, "*.*", searchOption);
            for (int i = 0; i < fileNames.Length; i++)
            {
                var contentFolderIndex = fileNames[i].LastIndexOf(contentFolder);
                fileNames[i] = contentFolder + fileNames[i].Substring(contentFolderIndex + contentFolder.Length);
                fileNames[i] = fileNames[i].Substring(0, fileNames[i].Length - 4);
            }
            return fileNames;
        }

        public static T[] LoadAllContent<T>(ContentManager contentManager, string contentFolder, bool subDirs = true)
        {

            return LoadFileNames(contentManager, contentFolder, subDirs).SelectF(x => contentManager.Load<T>(x));
        }


        public static void Initialize(ContentManager content)
        {

            femaleBodySheets = LoadAllContent<Texture2D>(content, "LPC/body/human/female", false);
            maleBodySheets = LoadAllContent<Texture2D>(content, "LPC/body/human/male", false);

            maleHairSheets = LoadAllContent<Texture2D>(content, "LPC/hair/male");
            femaleHairSheets = LoadAllContent<Texture2D>(content, "LPC/hair/female");

            shirtSheets = LoadAllContent<Texture2D>(content, "LPC/torso/shirts/longsleeve");
            pantsSheets = LoadAllContent<Texture2D>(content, "LPC/legs/pants");

            shoeSheets = LoadAllContent<Texture2D>(content, "LPC/feet/shoes/male");

            maleEyeSheets = LoadAllContent<Texture2D>(content, "LPC/body/male_headparts/eyes");
            femaleEyeSheets = LoadAllContent<Texture2D>(content, "LPC/body/female_headparts/eyes");

            saberSheet = content.Load<Texture2D>("LPC/weapons/oversize/right hand/male/saber_male");

        }

    }
}