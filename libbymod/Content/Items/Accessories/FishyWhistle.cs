using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using libbymod.Content.Projectiles;

namespace libbymod.Content.Items.Accessories
{
    public class FishyWhistle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 5);
            Item.damage = 50;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FishyWhistlePlayer>().hasFishyWhistle = true;
        }
    }

    public class FishyWhistlePlayer : ModPlayer
    {
        private int fishingTimer = 0;
        private int noFishingTimer = 0;
        public bool hasFishyWhistle;

        public override void ResetEffects()
        {
            hasFishyWhistle = false;
        }
        public bool HasFishingBobber()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == Player.whoAmI && proj.aiStyle == ProjAIStyleID.Bobber)
                {
                    return true;
                }
            }
            return false;
        }
        public override void PreUpdate()
        {
            if (!hasFishyWhistle) return;

            bool isFishing = Player.HeldItem.fishingPole > 0 && HasFishingBobber();

            if (isFishing)
            {
                noFishingTimer = 0;
                fishingTimer++;
                if (fishingTimer == 100 && Player.ownedProjectileCounts[ModContent.ProjectileType<LilDukeMinion>()] == 0)
                {
                    if (Main.myPlayer == Player.whoAmI)
                    {
                        Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<LilDukeMinion>(), 40, 2f, Player.whoAmI);
                    }
                }
            }
            else
            {
                fishingTimer = 0;
                noFishingTimer++;
                if (noFishingTimer >= 300)
                {
                    foreach (Projectile proj in Main.projectile)
{
    if (proj.active && proj.type == ModContent.ProjectileType<LilDukeMinion>() && proj.owner == Player.whoAmI)
    {
        // Create explosion effect
        for (int i = 0; i < 10; i++)
        {
            // Create dust (using DustID.Water here, adjust as needed)
            Dust.NewDust(proj.position, proj.width, proj.height, DustID.Water, 2, 2);
            Dust.NewDust(proj.position, proj.width, proj.height, DustID.Water, 2, 2);
            Dust.NewDust(proj.position, proj.width, proj.height, DustID.Water, 2, 2);

        }
        
        // Kill the projectile after explosion effect
        proj.Kill();
    }
}
                }
            }
        }
    }
}