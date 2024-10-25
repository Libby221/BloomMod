using libbymod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
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
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
            {
                // Spawn a new projectile with the newly rotated velocity, belonging to the original projectile owner. The new projectile will inherit the spawning source of this projectile.
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.velocity / 3, ModContent.ProjectileType<Projectiles.GhostHarpoon>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);

                Projectile GhostHarpoon = Main.projectile[ModContent.ProjectileType<Projectiles.GhostHarpoon>()];
                GhostHarpoon.rotation = Projectile.rotation;  // Match the primary projectile's rotation
            }
        }
    }

    }
    
