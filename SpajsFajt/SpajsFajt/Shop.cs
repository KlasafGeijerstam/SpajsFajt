using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpajsFajt
{
    class Shop : IDrawable
    {
        public Vector2 Position { get; set; }

        public string RectangleName { get; set; }

        public Rectangle TextureRectangle { get; set; }
        private Vector2 auraOffset = new Vector2(210, 210);
        private Vector2 offset = new Vector2(41, 25);
        private float radius = 210;
        private Rectangle textureSource;
        private Rectangle auraTextureSource;
        private Vector2 shopPos;
        private World localWorld;
        private List<ShopItem> shopItems = new List<ShopItem>();

        public bool ShowShop
        {
            get; set;
        }

        public Shop(World local)
        {
            auraTextureSource = TextureManager.GetRectangle("auraShop");
            textureSource = TextureManager.GetRectangle("shop");
            Position = new Vector2(1000, 1000);
            shopPos = Position + offset * 2;
            localWorld = local;

            //Setup
            for (int i = 1; i < 13; i++)
            {
                shopItems.Add(new ShopItem("text" + i, GetItemRectangle(i)));
            }

        }

        public bool InShop(Vector2 v)
        {
            return Vector2.Distance(v, Position + auraOffset) < radius;
        }

        public void UpdateItems()
        {
            
        }

        private Rectangle GetItemRectangle(int num)
        {
            Rectangle rect = new Rectangle(0,0,35,35);

            if (num >= 9)
            {
                num -= 8;
                rect.X = (int)shopPos.X + 175;
                rect.Y = (int)shopPos.Y + (80 + ((num - 1) * 40));
            }
            else if (num >= 5)
            {
                num -= 4;
                rect.X = (int)shopPos.X + 105;
                rect.Y = (int)shopPos.Y + (80 + ((num - 1) * 40));
            }
            else
            {
                rect.X = (int)shopPos.X + 35;
                rect.Y = (int)shopPos.Y + (80 + ((num - 1) * 40));
            }
            return rect;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet, Position, auraTextureSource, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(TextureManager.SpriteSheet, Position + auraOffset, textureSource, Color.White, 0f, offset, 1f, SpriteEffects.None, 0.55f);
            if (ShowShop)
            {
                spriteBatch.Draw(TextureManager.SpriteSheet, shopPos, TextureManager.GetRectangle("shopButtons"), Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0.71f);
                spriteBatch.Draw(TextureManager.SpriteSheet, MousePos(), TextureManager.GetRectangle("cursor"), Color.White, 0f, new Vector2(7,7), 2f, SpriteEffects.None, 0.75f);
            }
            foreach (var item in shopItems)
            {
                item.Draw(shopPos, spriteBatch);
            }
        }
        private Vector2 MousePos()
        {
            return Mouse.GetState().Position.ToVector2() + Game1.CameraPosition - new Vector2(320, 200);
        }
        private Rectangle MouseRect()
        {
            return new Rectangle(MousePos().ToPoint(), new Point(2, 2));
        }
        public void Update(GameTime gameTime)
        {
            foreach (var item in shopItems)
            {
                if (ShowShop && MouseRect().Intersects(item.CollisionRectangle))
                {
                    item.Hover = true;
                }
                else
                    item.Hover = false;
            }
        }
    }

    class ShopItem
    {
        public bool Hover { get; set; }
        public Rectangle CollisionRectangle { get; set; }
        private string data;
        private Vector2 position;
        public ShopItem(string textName,Rectangle colRect)
        {
            CollisionRectangle = colRect;
            //data = TextureManager.GetString(textName);
            data = textName;
            position = new Vector2(colRect.X, colRect.Y);
        }

        public void Draw(Vector2 basePos,SpriteBatch spriteBatch)
        {
            if (Hover)
            {
                spriteBatch.Draw(TextureManager.SpriteSheet, CollisionRectangle, TextureManager.GetRectangle("shopGreen"), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.72f);
                spriteBatch.DrawString(TextureManager.GameFont, data, basePos, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.73f);
            }
            
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
