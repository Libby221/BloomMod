using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using libbymod.Content.Buffs;

namespace libbymod.Content.Projectiles
{
    public class Scaredactyl : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3; // Three-frame animation
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            heightOffset = Main.rand.NextFloat(-40f, 40f);
        }

        private float heightOffset;  // Store the random height offset for this minion

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Keep minion alive
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<ScaredactylBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<ScaredactylBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            // Teleport to player if too far (200 tiles = 3200 pixels)
            float maxDistance = 1000f;
            if (Vector2.Distance(Projectile.Center, player.Center) > maxDistance)
            {
                Projectile.Center = player.Center;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                return; // Skip the rest of the logic this frame
            }

            NPC target = FindTarget();
            float baseHoverHeight = 180f + heightOffset;
            float swayAmplitude = 10f;
            float swaySpeed = 0.05f;
            float lerpFactor = 0.2f;
            float maxSpeed_Attack = 9f;
            float maxSpeed_Idle = 6f;
            float hoverThreshold = 20f; // Less twitching around hover position

            // Determine the minion's index among all active minions
            int minionIndex = 0;
            int totalMinions = 0;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type)
                {
                    if (proj.whoAmI < Projectile.whoAmI)
                        minionIndex++;
                    totalMinions++;
                }
            }

            float spacingOffset = (minionIndex - (totalMinions - 1) / 2f) * 30f;

            if (target != null)
            {
                // Attacking: Hover and bob above enemy

                // Swaying bobbing motion
                float swayOffset = (float)Math.Sin(Main.GameUpdateCount * swaySpeed + Projectile.whoAmI) * swayAmplitude;

                // Intended hover position (with bobbing)
                Vector2 desiredHoverPosition = target.Center + new Vector2(spacingOffset, -baseHoverHeight + swayOffset);
                Vector2 hoverPosition = desiredHoverPosition;

                // Check for tile collision and try to resolve by scanning in a radius
                bool isInsideTile = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
                if (isInsideTile)
                {
                    bool foundClearSpot = false;
                    const int maxSearchDistance = 96; // pixels
                    const int stepSize = 8;

                    for (int offsetY = -maxSearchDistance; offsetY <= maxSearchDistance; offsetY += stepSize)
                    {
                        for (int offsetX = -maxSearchDistance; offsetX <= maxSearchDistance; offsetX += stepSize)
                        {
                            Vector2 testPosition = desiredHoverPosition + new Vector2(offsetX, offsetY);
                            if (!Collision.SolidCollision(testPosition, Projectile.width, Projectile.height))
                            {
                                hoverPosition = testPosition;
                                foundClearSpot = true;
                                break;
                            }
                        }
                        if (foundClearSpot) break;
                    }
                }

                Vector2 moveDirection = hoverPosition - Projectile.Center;

                if (moveDirection.Length() > hoverThreshold)
                {
                    Vector2 desiredVelocity = moveDirection.SafeNormalize(Vector2.Zero) * maxSpeed_Attack;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, lerpFactor);
                }
                else
                {
                    // Close to target position, slow down
                    Projectile.velocity *= 0.9f;
                }

                // Face movement direction
                if (Projectile.velocity.X > 0) Projectile.spriteDirection = -1;
                else if (Projectile.velocity.X < 0) Projectile.spriteDirection = 1;

                // Firing timer
                if (++Projectile.ai[0] >= Main.rand.Next(30, 60))
                {
                    Projectile.ai[0] = 0;

                    Vector2 shootDirection = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 5f;
                    shootDirection.Y += 0.2f;

                    int projectileType = Main.rand.Next(10) == 0 ? ModContent.ProjectileType<AmberShard>() : ModContent.ProjectileType<ScaredactylBone>();

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        shootDirection,
                        projectileType,
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }
            else
            {
                // Idle movement
                float hoverDistance = 50f;
                float orbitRadius = 40f;
                float orbitSpeed = 0.05f;
                float angle = (Main.GameUpdateCount * orbitSpeed) + (minionIndex * MathHelper.PiOver4);

                Vector2 hoverPosition = player.Center + new Vector2((float)Math.Cos(angle) * orbitRadius + spacingOffset, -hoverDistance + (float)Math.Sin(angle * 2) * 5);
                Vector2 moveDirection = hoverPosition - Projectile.Center;
                Projectile.velocity.Y += (float)Math.Sin(Main.GameUpdateCount * 0.05f + minionIndex) * 0.1f;

                if (moveDirection.Length() > hoverThreshold)
                {
                    Vector2 desiredVelocity = moveDirection.SafeNormalize(Vector2.Zero) * maxSpeed_Idle;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, lerpFactor);
                }
                else
                {
                    Projectile.velocity *= 0.9f;
                }

                if (moveDirection.Length() > 5f)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, moveDirection.SafeNormalize(Vector2.Zero) * 5f, lerpFactor); // Increased idle speed to 8f
                }
            }

            // Face direction of movement
            if (Projectile.velocity.X > 0)
                Projectile.spriteDirection = -1;
            else if (Projectile.velocity.X < 0)
                Projectile.spriteDirection = 1;

            // Animation logic
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 3;
            }
        }

        private NPC FindTarget()
        {
            NPC closest = null;
            float maxDistance = 1000f;
            Player player = Main.player[Projectile.owner];  // Get the player the minion is following

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    // Calculate distance from player to NPC
                    float distance = Vector2.Distance(npc.Center, player.Center);

                    if (distance < maxDistance)
                    {
                        maxDistance = distance;
                        closest = npc;
                    }
                }
            }
            return closest;
        }
    }
}