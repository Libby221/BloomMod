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
    internal class GhostHarpoon : ModProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.damage = 10;
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.penetrate = 7;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;  // Adjust as needed
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int j = 0; j < 20; j++)
            {
                var GhostDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terragrim, 0f, 0f, 100, default, 3.5f);
                GhostDust.noGravity = true;
                GhostDust.velocity *= 7f;
                GhostDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Terragrim, 0f, 0f, 100, default, 1.5f);
                GhostDust.velocity *= 3f;
            }
        }
    }
}
