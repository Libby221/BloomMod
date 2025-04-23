using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using libbymod.Content.Projectiles;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace libbymod.Content.Items.Weapons
{
    public class BlackSting : ModItem
    {


        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item13; // Similar to Aqua Scepter sound
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PoisonStream>();
            Item.shootSpeed = 18f;
            Item.mana = 5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Length from the player's hand to the end of the weapon's sprite
            float weaponLength = 44f; // Adjust based on your sprite

            // Calculate the offset based on the weapon's rotation
            Vector2 offset = new Vector2(weaponLength, 0).RotatedBy(player.itemRotation);

            // Adjust for player direction
            offset.X *= player.direction;

            // Reverse Y offset when player is facing left
            if (player.direction == -1)
            {
                offset.Y = -offset.Y;
            }

            // Calculate the final spawn position
            Vector2 spawnPosition = player.Center + offset;

            // Spawn the projectile
            Projectile.NewProjectile(source, spawnPosition, velocity, type, damage, knockback, player.whoAmI);

            return false; // Return false because we've manually created the projectile
        }
    }
}