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
    public class Shield : Item
    {
        // Public properties
        public Bar ShieldBar { get; set; }
        public List<string> Descriptions { get; set; }
        public ShieldMethod Method { get; set; }
        public float ShieldHeal { get; set; }
        public int Chance { get; set; }

        // Shieldbar
        public float MaxValue { get { return ShieldBar.MaxValue; } set { ShieldBar.MaxValue = value; } }
        public float Value { get { return ShieldBar.Value; } }
        public float Width { get { return ShieldBar.Width; } }

        // Constructor(s)
        public Shield(Vector2 position, int width, int height, float shieldValue, int method, Difficulty difficulty)
            : base(Item.Nothing, ItemType.shield, TextureManager.shields[Globals.Randomizer.Next(0, TextureManager.shields.Count)], "", "Shield")
        {
            this.Type = ItemType.shield;
            this.ShieldHeal = Globals.Randomizer.Next(6 + (int)difficulty * 2, 14 + (int)difficulty * 2);
            this.Chance = Globals.Randomizer.Next(10 + (int)difficulty * 3, 21 + (int)difficulty * 4);

            this.ShieldBar = new Bar(position, width, height, shieldValue, Color.LightBlue);

            this.Method = ListOfShieldMethods()[method];

            this.Descriptions = new List<string>();
            Descriptions.Add("A standard shield.");
            Descriptions.Add("Has a |255,0,255|" + Chance + "|W|% chance to reflect a shot.");
            Descriptions.Add("Has a |255,0,255|" + Chance + "|W|% chance to fire you current weapon when you are hit.");
            Descriptions.Add("Has a |255,0,255|" + Chance + "|W|% chance to loose energy instead of Shield/HP when you are hit.");
            Descriptions.Add("When you are hit, the damage is split up in five and taken over time.");
            Descriptions.Add("Has a |255,0,255|" + Chance + "|W|% chance to teleport to another location just before being hit.");
            Descriptions.Add("Increase damage by 1-3 every time you get hit for the remainder of the fight.");
            Descriptions.Add("50 % chance to reduce damage by 50 %.");

            this.Description = "|W|Shield: |0,0,255|" + MaxValue + "|W|\n" + "Shield heal on Match: |0,150,255|" + ShieldHeal + "|W|\n" + Descriptions[method];
        }

        // Method(s)
        public static List<ShieldMethod> ListOfShieldMethods()
        {
            List<ShieldMethod> methods = new List<ShieldMethod>();
            methods.Add(ShieldStandard);
            methods.Add(ShieldReflect);
            methods.Add(ShieldCounterShoot);
            methods.Add(ShieldDamageEnergy);
            methods.Add(ShieldDamageOverTime);
            methods.Add(ShieldChanceToTeleport);
            methods.Add(ShieldBonusDamage);
            methods.Add(ShieldChanceReduceDamage);
            return methods;
        }

        public float Change(float value)
        {
            return ShieldBar.Change(value);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (SceneManager.CurrentScene == SceneManager.mapScene)
            {
                ShieldBar.Draw(spriteBatch); 
            }

            // Description
            if (Globals.MRectangle.Intersects(Box) && SceneManager.CurrentScene == SceneManager.mapScene)
            {
                Text.TextDifferentColor(spriteBatch, Description, new Vector2(Position.X + Texture.Width / 2 + 84, Position.Y - Texture.Height / 2), 1f, TextureManager.SpriteFont15, false);
            }
        }

        public delegate void ShieldMethod(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield);

        public static void ShieldStandard(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            ship.TakeDamage(damage, goThroughShield, damageType, true);
        }

        public static void ShieldReflect(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            if (damageType == DamageType.laser && Globals.Randomizer.Next(0, 101) < shield.Chance)
            {
                List<string> targets = new List<string>();
                targets.Add(ship.GetType().Name == "Player" ? "Enemy" : "Player");
                SceneManager.mapScene.CurrentLevel.ToAdd.Add(new Shot(ship.Position, MathHelper.Lerp((float)Math.PI + 0.2f, (float)Math.PI * 2 - 0.2f, (float)Globals.Randomizer.NextDouble()), damage, Shot.HitBasic, targets, 0f, 0));
            }
            else
            {
                ship.TakeDamage(damage, goThroughShield, damageType, true);
            }
        }

        public static void ShieldCounterShoot(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            if (Globals.Randomizer.Next(0, 101) < shield.Chance)
            {
                ship.CurrentWeapon.Method(ship, ship.CurrentWeapon, 3, SceneManager.mapScene.CurrentLevel, false);
            }
            ship.TakeDamage(damage, goThroughShield, damageType, true);
        }

        public static void ShieldDamageEnergy(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            if (Globals.Randomizer.Next(0, 101) < shield.Chance && ship.GetType().Name == "Player")
            {
                Player p = (Player)ship;
                p.Energy.Change(-damage);
            }
            else
            {
                ship.TakeDamage(damage, goThroughShield, damageType, true);
            }
        }

        public static void ShieldDamageOverTime(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            ship.SetDamageOverTime(damage / 5, 5, goThroughShield);
        }

        public static void ShieldChanceToTeleport(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            if (Globals.Randomizer.Next(0, 101) < shield.Chance)
            {
                List<int> locations = new List<int>();
                for (int i = 0; i < 3; i++)
			    {
                    if (i != (int)ship.ShipLocation)
	                {
		                locations.Add(i); 
	                }
			    }
                ship.ShipLocation = (Location)locations[Globals.Randomizer.Next(0, locations.Count())];
                ship.Size = 0;
                ship.Position = new Vector2((int)ship.ShipLocation * 100 + 200, ship.Position.Y);
            }
            else
            {
                ship.TakeDamage(damage, goThroughShield, damageType, true);
            }
        }

        public static void ShieldBonusDamage(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            ship.TakeDamage(damage, goThroughShield, damageType, true);
            ship.BonusDamageOneFight += Globals.Randomizer.Next(1, 4);
        }

        public static void ShieldChanceReduceDamage(float damage, float goThroughShield, DamageType damageType, Ship ship, Shield shield)
        {
            if (Globals.Randomizer.Next(0, 101) < 50)
            {
                ship.TakeDamage(damage / 2, goThroughShield, damageType, true);
            }
            else
            {
                ship.TakeDamage(damage, goThroughShield, damageType, true);
            }
        }
    }
}
