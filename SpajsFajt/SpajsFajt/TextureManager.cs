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
    static class TextureManager
    {
        private static Dictionary<string, Rectangle> textureRectangles;
        public static Texture2D SpriteSheet { get; private set; }
        public static SpriteFont GameFont { get; private set; }
        private static Random rnd = new Random();
        private static Rectangle particlesRectangle;
        private static Dictionary<string, string> stringData;

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

        public static string GetString(string name)
        {
            if (stringData.ContainsKey(name))
                return stringData[name];
            else
                return "error";
        }

        public static void Load(ContentManager Content,string fileName)
        {
            textureRectangles = new Dictionary<string, Rectangle>();
            stringData = new Dictionary<string, string>();

            XDocument xDoc = XDocument.Load(Content.RootDirectory + "/" +fileName);
            xDoc.Element("doc").Elements("texture").ToList().ForEach(x => {
                textureRectangles.Add(x.Attribute("name").Value, new Rectangle(int.Parse(x.Attribute("x").Value),
                    int.Parse(x.Attribute("y").Value), int.Parse(x.Attribute("w").Value), int.Parse(x.Attribute("h").Value)));
            });
            SpriteSheet = Content.Load<Texture2D>(xDoc.Element("doc").Element("sheet").Attribute("name").Value);
            GameFont = Content.Load<SpriteFont>("gameFont");
            xDoc.Element("doc").Elements("stringData").ToList().ForEach(x =>
            {
                stringData.Add(x.Attribute("name").Value, x.Value);
            });
            particlesRectangle = GetRectangle("particles");
        }

        public static Rectangle GetParticle()
        {
            return new Rectangle(particlesRectangle.X +rnd.Next(0, 25) * 2,particlesRectangle.Y + rnd.Next(particlesRectangle.Y, 25) * 2, 2, 2);
        }

        public static void Draw(this Rectangle r, SpriteBatch spriteBatch, int width = 1)
        {
            //Top
            spriteBatch.Draw(SpriteSheet, new Rectangle(r.X, r.Y, r.Width, width), textureRectangles["error"], Color.Yellow, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            //Bot
            spriteBatch.Draw(SpriteSheet, new Rectangle(r.X, r.Y +r.Height -width, r.Width, width), textureRectangles["error"], Color.Yellow, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            //Left
            spriteBatch.Draw(SpriteSheet, new Rectangle(r.X, r.Y, width, r.Height), textureRectangles["error"], Color.Yellow, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            //Right
            spriteBatch.Draw(SpriteSheet, new Rectangle(r.X + r.Width -width, r.Y, width, r.Height), textureRectangles["error"], Color.Yellow,0f,Vector2.Zero,SpriteEffects.None,1f);
        }
    }
}
