using libbymod.Content.Buffs;
using libbymod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace libbymod.Content.Items.Weapons
{
    public class IndianasWhip : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(IndianasWhipDebuff.TagDamage);

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<IndianasWhipProjectile>(), 20, 2, 4);
            Item.rare = ItemRarityID.Green;
            Item.damage = 15;
            Item.knockBack = 4;
         
        }


        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}