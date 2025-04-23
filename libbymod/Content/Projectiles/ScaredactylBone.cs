using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.Audio;

namespace libbymod.Content.Projectiles
{
    public class ScaredactylBone : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 8000; // How long the projectile lasts
            Projectile.aiStyle = 0; // Custom AI (this is important for rotating and dust)
            Projectile.damage = 40;
        }

        public override void AI()
        {
            // Spinning effect
            Projectile.rotation += 0.1f;

            // Create a faint dust trail
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmber, 0f, 0f, 100, default, 0.5f);
            dust.noGravity = true;
            dust.velocity *= 0.3f;
            dust.fadeIn = 1f;

            // Apply gravity
            Projectile.velocity.Y += 0.5f;

            // Slight homing behavior
            float homingRange = 300f;
            float homingStrength = 0.8f; // Smaller = more subtle

            NPC target = null;
            float closestDist = homingRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this))
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDist && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1))
                    {
                        closestDist = distance;
                        target = npc;
                    }
                }
            }

            if (target != null)
            {
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity + direction, homingStrength);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // You can add extra effects here when the bone hits an enemy
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Kill the projectile when it hits a tile
            Projectile.Kill();

            // Return base method to ensure other tile collision behavior is still handled
            return base.OnTileCollide(oldVelocity);
        }


        public override void OnKill(int timeLeft)
        {

            // Optional: Add dust effect or sound effect on kill
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmber, 0, 0, 150, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.5f;
            }
            SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.position);
        }
    }
}