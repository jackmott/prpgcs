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
        public static string[] maleHairPaths;
        public static string[] femaleHairPaths;
        public static string[] shirtPaths;
        public static string[] pantsPaths;
        public static string[] shoePaths;
        public static string[] maleEyePaths;
        public static string[] femaleEyePaths;



        public static int UP = 0;
        public static int LEFT = 1;
        public static int DOWN = 2;        
        public static int RIGHT = 3;

        const int CHAR_SIZE = 64;
        const int WALKING_INDEX = 8;
        const int WALKING_WIDTH = 9;
        public Rectangle[,] walking;
        public Texture2D spriteSheet;

        public CharSprites(Gender gender,ContentManager content) {
#if DEBUG
            PRPGame.npcSprited++;
#endif
            Texture2D baseSheet;
            Texture2D hairSheet;
            Texture2D eyeSheet;
            if (gender == Gender.Male) {
                baseSheet = RandUtil.Index(maleBodySheets);
                hairSheet = content.Load<Texture2D>(RandUtil.Index(maleHairPaths));
                eyeSheet = content.Load<Texture2D>(RandUtil.Index(maleEyePaths));
                
            } else {
                baseSheet = RandUtil.Index(femaleBodySheets);
                hairSheet = content.Load<Texture2D>(RandUtil.Index(femaleHairPaths));
                eyeSheet = content.Load<Texture2D>(RandUtil.Index(femaleEyePaths));
            }

            Texture2D shirtSheet = content.Load<Texture2D>(RandUtil.Index(shirtPaths));            
            Texture2D pantSheet = content.Load<Texture2D>(RandUtil.Index(pantsPaths));
            Texture2D shoeSheet = content.Load<Texture2D>(RandUtil.Index(shoePaths));
                        
            var renderTarget = new RenderTarget2D(PRPGame.graphics,baseSheet.Width,baseSheet.Height,false,SurfaceFormat.Color,DepthFormat.None,0,RenderTargetUsage.DiscardContents);

                     
            PRPGame.graphics.SetRenderTarget(renderTarget);
            PRPGame.graphics.Clear(Color.Transparent);
            PRPGame.batch.Begin();            
            PRPGame.batch.Draw(baseSheet, Vector2.Zero, Color.White);
            PRPGame.batch.Draw(eyeSheet, Vector2.Zero, Color.White);
            PRPGame.batch.Draw(hairSheet, Vector2.Zero, Color.White);
            PRPGame.batch.Draw(shirtSheet, Vector2.Zero, Color.White);
            PRPGame.batch.Draw(pantSheet, Vector2.Zero, Color.White);
            PRPGame.batch.Draw(shoeSheet, Vector2.Zero, Color.White);
            PRPGame.batch.End();
            PRPGame.graphics.SetRenderTarget(null);

            

            spriteSheet = renderTarget;
          
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
            var maleBodyFilenames = LoadFileNames(content,"LPC/body/human/male");
            var femaleBodyFilenames = LoadFileNames(content,"LPC/body/human/female");

            femaleBodySheets = LoadAllContent<Texture2D>(content,"LPC/body/human/female",false);
            maleBodySheets = LoadAllContent<Texture2D>(content, "LPC/body/human/male",false);

            maleHairPaths = LoadFileNames(content,"LPC/hair/male");
            femaleHairPaths = LoadFileNames(content, "LPC/hair/female");

            shirtPaths = LoadFileNames(content, "LPC/torso/shirts/longsleeve");
            pantsPaths = LoadFileNames(content, "LPC/legs/pants");

            shoePaths = LoadFileNames(content, "LPC/feet/shoes/male");

            maleEyePaths = LoadFileNames(content, "LPC/body/male_headparts/eyes");
            femaleEyePaths = LoadFileNames(content, "LPC/body/female_headparts/eyes");

        }

    }
}