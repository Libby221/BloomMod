using libbymod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace libbymod.Content.Projectiles
{

    public class WhiteWhaleProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 13;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 80;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
            {
                // Spawn a new projectile with the newly rotated velocity, belonging to the original projectile owner. The new projectile will inherit the spawning source of this projectile.
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity / 3, ModContent.ProjectileType<Projectiles.GhostHarpoon>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

                Projectile GhostHarpoon = Main.projectile[ModContent.ProjectileType<Projectiles.GhostHarpoon>()];
                GhostHarpoon.rotation = Projectile.rotation;  // Match the primary projectile's rotation
            }
        }

     /*   public override bool PreDraw(ref Color lightColor)
        {
            // Load the chain texture
            Texture2D chainTexture = ModContent.Request<Texture2D>("libbymod/Content/Projectiles/WhiteWhaleChain").Value;
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 directionToProjectile = Projectile.Center - playerCenter;
            float rotation = directionToProjectile.ToRotation();
            float chainSegmentLength = chainTexture.Height;

            // Draw each chain segment
            for (float i = 0; i <= directionToProjectile.Length(); i += chainSegmentLength)
            {
                Vector2 segmentPosition = playerCenter + directionToProjectile * (i / directionToProjectile.Length());
                Main.spriteBatch.Draw(
                    chainTexture,
                    segmentPosition - Main.screenPosition,
                    null,
                    lightColor,
                    rotation,
                    new Vector2(chainTexture.Width / 2, chainTexture.Height / 2),
                    1f,
                    SpriteEffects.None,
                    0
                );
            }

            return true; // Draw the projectile itself after drawing the chain
        } */
    }
}
    
