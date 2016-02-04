using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Outer_Space
{
    public enum Location { left, middle, right }

    public abstract class Ship : GameObject
    {
        // Public properties
        public Location ShipLocation { get; set; }
        public List<Weapon> Weapons
        {
            get
            {
                List<Weapon> weapons = new List<Weapon>();
                for (int i = 0; i < 3; i++)
                {
                    if (Inventory[2 + i, 5].Type == ItemType.weapon)
                    {
                        weapons.Add((Weapon)Inventory[2 + i, 5]);
                    }
                }
                return weapons;
            }
        }

        public Item[,] Inventory { get; set; }
        public Shield ShipShield { get { return (Shield)Inventory[0, 5]; } set { Inventory[0, 5] = value; } }
        public Hull ShipHull { get { return (Hull)Inventory[1, 5]; } set { Inventory[1, 5] = value; } }
        public float DirectionSpeed { get; set; }
        public float StandardDirection { get; set; }
        public int SelectedWeapon { get; set; }
        public Weapon CurrentWeapon { get { return Weapons[SelectedWeapon]; } set { Weapons[SelectedWeapon] = value; } }
        public float KnockBack { get; set; }

        // Engine animation
        internal Texture2D engineAnimation;
        internal int frame;
        internal int animationTimer;
        internal int maxFrame;
        internal int frameWidth;
        internal int frameHeight;

        // Damage over time
        public int DamageOverTimeCount { get; private set; }
        public float DamageOverTimeDamage { get; private set; }
        private int damageOverTimeTimer;
        private float shieldPiercing;

        public Bar Health { get; set; }

        // Constructor(s)
        public Ship(float standardDirection)
            : base()
        {
            this.ShipLocation = Location.middle;
            this.StandardDirection = standardDirection;
            this.Direction = StandardDirection;
            this.engineAnimation = TextureManager.none;
            this.Depth = 0.3f;
            this.frameWidth = 64;
            this.frameHeight = 64;

            this.Inventory = new Item[5, 6];
            // Fill inventory
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Inventory[i, j] = (new Item(Item.Nothing, ItemType.nothing, TextureManager.none, ""));
                }
            }

            Inventory[2, 5] = new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count()));
            Inventory[3, 5] = new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count()));

            this.ShipHull = new Hull(this, Globals.Randomizer.Next(0, Hull.ListOfHullMethods().Count()));

            this.Health = new Bar(new Vector2(0, Globals.ScreenSize.Y - 35), 100, 20, 140, Color.Red);
            this.ShipShield = new Shield(new Vector2(200, Globals.ScreenSize.Y - 35), 100, 20, 100, Globals.Randomizer.Next(0, Shield.ListOfShieldMethods().Count()));
        }

        // Method(s)
        public void Animation()
        {
            animationTimer++;
            if (animationTimer > 5)
            {
                animationTimer = 0;
                if (frame < maxFrame)
                {
                    frame++;
                }
                else
                {
                    frame = 0;
                }
            }
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            DamageOverTime();
            Animation();

            // Weapons
            foreach (Weapon w in Weapons)
            {
                w.UpdateLevel(level);
            }

            // Move
            Vector2 move = new Vector2((int)ShipLocation * 100 + 200, Position.Y) - Position;
            Position += move * 0.05f;
            Position += new Vector2(0, KnockBack);
            KnockBack = MathHelper.Lerp(KnockBack, 0, 0.1f);

            // Ship tilt
            if (move.Length() > 10)
            {
                Direction += move.Length() * DirectionSpeed;
            }
            Direction = MathHelper.Lerp(Direction, StandardDirection, 0.1f);
        }


        public virtual void TakeDamage(float damage, float goThroughShield, DamageType damageType, bool FromShield)
        {
            if (FromShield)
            {
                // Rock resist hull
                if (damageType == DamageType.rock)
                {
                    damage *= ShipHull.RockResistance;
                }

                // Armor
                damage *= (float)(1 - (float)ShipHull.Armor / 100);

                if (ShipShield.Value > 0 && goThroughShield < 1)
                {
                    float damageThroughShield = damage * goThroughShield;
                    damage -= damageThroughShield;
                    Health.Change(-damageThroughShield);
                    Health.Change(ShipShield.Change(-damage));
                }
                else if (ShipShield.Value <= 0 || goThroughShield >= 1)
                {
                    Health.Change(-damage);
                }
            }
            else
            {
                ShipShield.Method(damage, goThroughShield, damageType, this, ShipShield);
            }
        }

        public void SetDamageOverTime(float damage, int count, float shieldPiercingDamage)
        {
            DamageOverTimeDamage = damage;
            DamageOverTimeCount = count;
            shieldPiercing = shieldPiercingDamage;
        }

        public void DamageOverTime()
        {
            damageOverTimeTimer--;
            if (DamageOverTimeCount > 0 && damageOverTimeTimer < 0)
            {
                damageOverTimeTimer = 10;
                DamageOverTimeCount--;
                TakeDamage(DamageOverTimeDamage, shieldPiercing, DamageType.damageOverTime, true);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw(engineAnimation, Position, new Rectangle(frame * frameWidth, 0, frameWidth, frameHeight), Color.White, Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, Depth - 0.1f);

            Health.Draw(spriteBatch);
            ShipShield.Draw(spriteBatch);
            ShipHull.Draw(spriteBatch);
        }
    }
}
