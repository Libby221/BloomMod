using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using libbymod.Content;
using Microsoft.Xna.Framework;
using libbymod.Content.Projectiles;
using System;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;

namespace libbymod.Content.NPCs
{
    public class PixieWitch : ModNPC
    {
        private int attackTimer = 0;
        private int projectileTimer = 0;
        private int projectilesFired = 0;
        private int frameCounter = 0; // Counter to track the elapsed time for frame changes
        private int currentFrame = 0; // The current frame being displayed

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.Pixie];

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                PortraitPositionYOverride = 1f,
                Direction = -1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
                               // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
                               // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
            };
            drawModifiers.Position = new Vector2(0, -10); // Adjust this value to position the NPC correctly
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Not just your regular pixie. She's angrier, and witchier!")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // Check if the spawnInfo is in the Hallow biome
            return spawnInfo.Player.ZoneHallow && spawnInfo.Player.ZoneOverworldHeight ? 0.1f : 0f; // Adjust the chance (0.05f = 5% chance to spawn)
        }

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 42;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.lifeMax = 125;
            NPC.knockBackResist = 0.40f;
            NPC.HitSound = SoundID.NPCHit5;  // Pixie hit sound
            NPC.DeathSound = SoundID.NPCDeath6;  // Pixie death sound
            NPC.aiStyle = 22;  // Same AI as Pixie
            NPC.noGravity = true;
            Main.npcFrameCount[NPC.type] = 4;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            // Make the NPC face the direction it's moving
            if (NPC.velocity.X > 0)
            {
                NPC.spriteDirection = 1; // Face right
            }
            else if (NPC.velocity.X < 0)
            {
                NPC.spriteDirection = -1; // Face left
            }

            // Create Pixie dust effect when moving
            if (NPC.velocity != Vector2.Zero) // Check if the NPC is moving
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie, 0f, 0f, 75, default(Color), 1f);
                Main.dust[dustIndex].noGravity = true; // Optional: make it float
                Main.dust[dustIndex].velocity *= 0.5f; // Slow down the dust particles
            }

            // Cycle through frames
            frameCounter++;
            if (frameCounter >= 5) // Adjust speed of frame change (lower = faster)
            {
                currentFrame++;
                if (currentFrame >= 4) // There are 4 frames (0-3)
                {
                    currentFrame = 0; // Reset to the first frame
                }
                NPC.frame.Y = currentFrame * NPC.height; // Set the Y position of the frame
                frameCounter = 0; // Reset the frame counter
            }

            // Increase attack timer each tick
            attackTimer++;

            // Fire all projectiles every 3 seconds (180 ticks)
            if (attackTimer >= 120)
            {
                FireProjectiles();
                attackTimer = 0; // Reset the timer
            }
        }

        private void FireProjectiles()
        {
            // Play Pixie hit sound when the projectiles are fired
            SoundEngine.PlaySound(SoundID.NPCHit5, NPC.position);

            // Calculate direction towards the player
            Vector2 direction = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero);
            float speed = 10f;

            // Fire three projectiles in a row with a delay
            for (int i = 0; i < 3; i++)
            {
                float delay = i * 10; // Adjust the delay between projectiles (10 ticks = 1/6 of a second)
                Main.instance.LoadProjectile(ModContent.ProjectileType<PixieWitchProjectile>()); // Preload the projectile to avoid lag
                int projectileIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * speed, ModContent.ProjectileType<PixieWitchProjectile>(), 20, 1f, Main.myPlayer);

                // Set the projectile to fire with a delay
                Main.projectile[projectileIndex].timeLeft = 600; // Set a lifetime for the projectile if needed
                Main.projectile[projectileIndex].ai[1] = delay; // Set a custom AI value to manage timing (you can use this in the projectile AI if needed)
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit5, NPC.position); // Play Pixie hit sound before explosion

            for (int i = 0; i < 40; i++) // Increased from 20 to 40 for a bigger effect
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie, 0f, 0f, 100, default(Color), 0.75f); // Increased size
                Main.dust[dustIndex].noGravity = true; // Keep it floating
                Main.dust[dustIndex].velocity *= 1f; // Increase speed for more dynamic effect
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit5, NPC.position); // Play Pixie hit sound before explosion

            // Create a larger explosion of GoldFire dust upon death
            for (int i = 0; i < 40; i++) // Increased from 20 to 40 for a bigger effect
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie, 0f, 0f, 100, default(Color), 0.75f); // Increased size
                Main.dust[dustIndex].noGravity = true; // Keep it floating
                Main.dust[dustIndex].velocity *= 1f; // Increase speed for more dynamic effect
            }
        }

        public override void OnKill()
        {

            // Play the Pixie death sound
            SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.position); // Play Pixie death sound

            // Create a larger explosion of GoldFire dust upon death
            for (int i = 0; i < 40; i++) // Increased from 20 to 40 for a bigger effect
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Pixie, 0f, 0f, 100, default(Color), 0.75f); // Increased size
                Main.dust[dustIndex].noGravity = true; // Keep it floating
                Main.dust[dustIndex].velocity *= 5f; // Increase speed for more dynamic effect
            }

            int goreType = ModContent.GoreType<Assets.Gores.PixieWitchGore>(); // Use the ModContent.GoreType method to get your custom gore type
            for (int i = 0; i < 1; i++) // Spawns 3 pieces of gore
            {
                // Parameters are gore type, position, velocity
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.5f, goreType);
            }

            base.OnKill(); // Call the base method

            // Drop 2-3 Pixie Dust
            int pixieDustCount = Main.rand.Next(2, 4); // Generates a random number between 2 and 3
            Item.NewItem(NPC.GetSource_Loot(), NPC.position, NPC.Size, ItemID.PixieDust, pixieDustCount);

            // 1% chance to drop Fast Clock
            if (Main.rand.NextFloat() < 0.01f) // 1% chance
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.position, NPC.Size, ItemID.FastClock);
            }

            // 1% chance to drop Megaphone
            if (Main.rand.NextFloat() < 0.01f) // 1% chance
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.position, NPC.Size, ItemID.Megaphone);
            }
        }
    }
}