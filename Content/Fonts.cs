using System.IO;
using AudioMarcoPolo.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AudioMarcoPolo.Content
{
    public static class Fonts
    {
        //public static GameFont FontSpecialTile = new GameFont("Arial", 10);
        //public static GameFont FontTileLetter = new GameFont("Arial", 14);
        public static GameFont ArialSmall;// = new GameFont("Arial", 8);
        public static GameFont ArialLarge;// = new GameFont("Arial", 8);

        public static void LoadContent(ContentManager content)
        {
            //  ParticleFlowerBurst = content.Load<Texture2D>(@"Particle\FlowerBurst.png");

            var fontFilePath = Path.Combine(content.RootDirectory, "Fonts\\Arial16.fnt");
            var fontFile = FontLoader.Load(TitleContainer.OpenStream(fontFilePath));
            var fontTexture = content.Load<Texture2D>(@"Fonts\Arial16_0.png");
            ArialSmall = new GameFont(fontFile, fontTexture);
            fontFilePath = Path.Combine(content.RootDirectory, "Fonts\\Arial32.fnt");
            fontFile = FontLoader.Load(TitleContainer.OpenStream(fontFilePath));
            fontTexture = content.Load<Texture2D>(@"Fonts\Arial32_0.png");
            ArialLarge = new GameFont(fontFile, fontTexture);
        }
    }
}
