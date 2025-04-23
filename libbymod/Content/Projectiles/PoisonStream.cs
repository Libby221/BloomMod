using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace libbymod.Content.Projectiles
{
    public class PoisonStream : ModProjectile
    {
        private int bounceCount = 3; // Tracks remaining bounces
        private const float initialOffset = 10f; // Change this value for the desired offset

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1; // Continuous stream, so no limit on penetration
            Projectile.timeLeft = 150; // Short lifespan for stream effect
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            // Apply gravity
            Projectile.velocity.Y += 0.3f; // Increase this value to make gravity stronger

            // Emit cursed torch dust (green fire)
            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 200, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 200, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 200, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.1f;
                Main.dust[dust].scale = 0.5f;
            }

            // Rotate the projectile to face the direction it’s moving
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Apply poison debuff on hit
            //Projectile.GetGlobalProjectile<MyGlobalProjectile>().poisonDebuff = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Apply Poisoned debuff
            target.AddBuff(BuffID.Poisoned, 300); // Lasts 5 seconds (60 ticks per second)

            if (target.life <= 0)
                // Spawn 4 smaller projectiles in different directions
                for (int i = 0; i < 4; i++)
                {
                    Vector2 velocity = new Vector2(2, 0).RotatedBy(MathHelper.ToRadians(90 * i)); // Spread out at 90 degree angles
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, Projectile.type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (target.type == NPCID.TheDestroyer || target.type == NPCID.TheDestroyerBody || target.type == NPCID.TheDestroyerTail || target.type == NPCID.Probe)
                modifiers.FinalDamage *= 0.66f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Slightly reduce speed on bounce for realism
            Projectile.velocity *= 0.8f;

            // Generate a random angle in radians
            float randomAngle = Main.rand.NextFloat(0, MathHelper.TwoPi);

            // Set the new velocity based on the random angle
            float speed = Projectile.velocity.Length(); // Preserve the speed of the projectile
            Projectile.velocity = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * speed;

            // Decrease bounce count and destroy projectile after 3 bounces
            bounceCount--;
            if (bounceCount <= 0)
            {
                Projectile.Kill();
            }

            return false; // Prevent immediate destruction on collision
        }


        public override void OnKill(int timeLeft)
        {

            // Optional: Add dust effect or sound effect on kill
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0, 0, 150, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.5f;
            }
        }
    }
}
