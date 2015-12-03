using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class Modifiers
    {
        private float speedMod;
        private int damageMod;
        private int PlayerID;

        public Shield Shield
        {
            get; set;
        }

        public WeaponLevel WeaponLevel = WeaponLevel.Four;

        public Modifiers(int id)
        {
            PlayerID = id;
        }

        public float SpeedModification(float i)
        {
            return i * speedMod;
        }

        public int DamageModification(int i)
        {
            return i * damageMod;
        }

        public List<Projectile> GetProjectiles(Vector2 pos, float rot)
        {
            var proj = new List<Projectile>();
            switch (WeaponLevel)
            {
                case WeaponLevel.One:
                    proj.Add(new Projectile(GameServer.NextID(), rot, pos) { SenderID = PlayerID });
                    break;
                case WeaponLevel.Two:
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 20), pos) { SenderID = PlayerID });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 20), pos) { SenderID = PlayerID });
                    break;
                case WeaponLevel.Three:
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 20), pos) { SenderID = PlayerID });
                    proj.Add(new Projectile(GameServer.NextID(), rot, pos) { SenderID = PlayerID });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 20), pos) { SenderID = PlayerID });
                    break;
                case WeaponLevel.Four:
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 20), pos) { SenderID = PlayerID });
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 50), pos) { SenderID = PlayerID });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 50), pos) { SenderID = PlayerID });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 20), pos) { SenderID = PlayerID });
                    break;
            }

            return proj;
        }
    }

    enum ShieldEnum { None = 0, One, Two, Three}
    enum WeaponLevel { One = 1,Two,Three, Four}
    class Shield
    {
        public ShieldEnum Level = ShieldEnum.None;
        public float ShieldTimer { get; set; }
        public bool Active { get; set; }
        public Vector2 Position { get; set; }
        private Vector2 origin = new Vector2(40, 40);
        
        public Shield()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(TextureManager.SpriteSheet, Position, TextureManager.GetRectangle("shield" + (int)Level), Color.White, 0f, origin,1f,SpriteEffects.None, 0.55f);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                ShieldTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ShieldTimer >= (200 + (100 * (int)Level)))
                {
                    Active = false;
                }
            }
        }
    }
}
