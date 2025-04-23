using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace libbymod.Content.Items.Accessories
{
    public class MermaidPendant : ModItem
    {

        public override void SetDefaults()
        {
            // Set item properties
            Item.width = 24;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;  // It's an accessory item
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // This applies the fishing power increase when the accessory is equipped
            player.fishingSkill += 10; // Increases the player's fishing power by 10
        }
    }
}