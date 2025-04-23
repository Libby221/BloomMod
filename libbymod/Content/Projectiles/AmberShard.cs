using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;

namespace libbymod.Content.Projectiles
{
    public class AmberShard : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.damage = 90;
        }

        public override void AI()
        {
            // Apply gravity
            Projectile.velocity.Y += 0.2f;

            // Make it spin while moving
            Projectile.rotation += 0.3f * (float)Projectile.direction;

            // Dust trail effect
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin);
            dust.scale = 0.8f;
            dust.velocity *= 0.5f;
            dust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Inflict On Fire! debuff
            target.AddBuff(BuffID.OnFire, 180);

            // Create explosion effect
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
            }
            SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.position);
            // Destroy projectile on hit
            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Explodes on impact with tiles
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
            }
            Projectile.Kill();
            return false;
        }
    }
}
