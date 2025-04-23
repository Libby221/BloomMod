using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using libbymod.Content.Items.Accessories;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace libbymod.Content.Projectiles
{
    public class LilDukeMinion : ModProjectile
    {
        private int chargeDelay = 0;
        private const int maxChargeDelay = 30;
        private bool isCharging = false;
        private Vector2 chargeDirection;
        private float originalRotation = 0f;
        private int repositionTime = 0;
        private const int maxRepositionTime = 120; // Maximum delay before repositioning (2 seconds)

        // Dictionary to store the last strike time for each NPC
        private Dictionary<int, double> lastStrikeTimes = new Dictionary<int, double>();

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1; // Allow the minion to pierce enemies
            Projectile.aiStyle = 0;
            Projectile.frameCounter = 0;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.damage = 40;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || !player.GetModPlayer<FishyWhistlePlayer>().hasFishyWhistle)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;
            AnimateMinion();

            NPC target = FindTarget();
            if (isCharging)
            {
                ChargeMovement(target);
            }
            else if (target != null)
            {
                PrepareCharge(target);
            }
            else
            {
                HoverAroundPlayer(player);
                Projectile.rotation = originalRotation;
            }

            // Reposition after charging with random delay
            if (!isCharging && repositionTime > 0)
            {
                repositionTime--;
            }
            else if (!isCharging)
            {
                repositionTime = Main.rand.Next(30, maxRepositionTime); // Random delay before next attack
            }
        }

        private void AnimateMinion()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 4;
            }
        }

        private NPC FindTarget()
        {
            NPC closestTarget = null;
            float closestDist = 500f;
            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(Projectile) && Vector2.Distance(Projectile.Center, npc.Center) < closestDist)
                {
                    closestTarget = npc;
                    closestDist = Vector2.Distance(Projectile.Center, npc.Center);
                }
            }
            return closestTarget;
        }

        private void PrepareCharge(NPC target)
        {
            // Directly charge towards the target
            chargeDirection = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            isCharging = true;
            chargeDelay = maxChargeDelay;
        }

        private void ChargeMovement(NPC target)
        {
            if (chargeDelay > 0)
            {
                chargeDelay--;
                // Charge towards target with force
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, chargeDirection * 20f, 0.2f);
                AdjustRotation();

                // Only strike if 1 second has passed since last strike
                if (target != null && Vector2.Distance(Projectile.Center, target.Center) < 40f) // Adjust the range
                {
                    double currentTime = Main.time;
                    int targetID = target.whoAmI;

                    // Check if the NPC has been struck recently
                    if (!lastStrikeTimes.ContainsKey(targetID) || currentTime - lastStrikeTimes[targetID] >= 15.0) // 60 ticks = 1 second
                    {
                        target.SimpleStrikeNPC(160, 1, false, 3, DamageClass.Summon, true);

                        // Update the last strike time for this NPC
                        lastStrikeTimes[targetID] = currentTime;
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0.95f;
                if (Projectile.velocity.Length() < 2f)
                {
                    isCharging = false;
                    Projectile.velocity = Vector2.Zero;
                }
            }
        }

        private void AdjustRotation()
        {
            float targetRotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            float rotationDifference = MathHelper.WrapAngle(targetRotation - Projectile.rotation);

            if (Math.Abs(rotationDifference) > MathHelper.PiOver2)
            {
                Projectile.rotation = targetRotation + MathHelper.Pi;
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.rotation = targetRotation;
                Projectile.spriteDirection = 1;
            }
        }

        private void HoverAroundPlayer(Player player)
        {
            Vector2 targetPosition = player.Center + new Vector2(0, -60);
            Vector2 direction = targetPosition - Projectile.Center;
            float curveFactor = 0.1f * (float)Math.Sin(Main.GameUpdateCount * 0.1f);
            direction = direction.RotatedBy(curveFactor);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 0.1f, 0.2f);
            AdjustRotation();
        }
    }
}
