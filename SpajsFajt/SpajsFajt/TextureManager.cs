using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SpajsFajt
{
    class TextureManager
    {
        private static Dictionary<string, Rectangle> textureRectangles;
        public static Texture2D SpriteSheet { get; private set; }
        public static SpriteFont GameFont { get; private set; }
        private static Random rnd = new Random();
        private static Rectangle particlesRectangle;

        public static Rectangle GetRectangle(string name)
        {
            if (textureRectangles == null)
                throw new NullReferenceException("Load needs to be called before GetRectangle");
            else
            {
                if (textureRectangles.ContainsKey(name))
                {
                    return textureRectangles[name];
                }
                else
                {
                    return textureRectangles["error"];
                }
            }
            
        }
        
        public static void Load(ContentManager Content,string fileName)
        {
            textureRectangles = new Dictionary<string, Rectangle>();
            XDocument xDoc = XDocument.Load(Content.RootDirectory + "/" +fileName);
            xDoc.Element("doc").Elements("texture").ToList().ForEach(x => {
                textureRectangles.Add(x.Attribute("name").Value, new Rectangle(int.Parse(x.Attribute("x").Value),
                    int.Parse(x.Attribute("y").Value), int.Parse(x.Attribute("w").Value), int.Parse(x.Attribute("h").Value)));
            });
            SpriteSheet = Content.Load<Texture2D>(xDoc.Element("doc").Element("sheet").Attribute("name").Value);
            GameFont = Content.Load<SpriteFont>("gameFont");
            particlesRectangle = GetRectangle("particles");
        }

        public static Rectangle GetParticle()
        {
            return new Rectangle(particlesRectangle.X +rnd.Next(0, 25) * 2,particlesRectangle.Y + rnd.Next(particlesRectangle.Y, 25) * 2, 2, 2);
        }
    }
}
