using Humanizer;
using libbymod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.X86;
using Terraria;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace libbymod.Content.Items.Weapons
{
    public class WhiteWhale : ModItem
    {
        public override void SetDefaults()
        {
            // This method right here is the backbone of what we're doing here; by using this method, we copy all of
            // the meowmere's SetDefault stats (such as Item.melee and Item.shoot) on to our item, so we don't have to
            // go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner; if you're
            // going to copy the stats of an item, use CloneDefaults().

            Item.CloneDefaults(ItemID.Harpoon);

            // After CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
            // For the sake of example, let's swap the vanilla Meowmere projectile shot from our item for our own projectile by changing Item.shoot:

            Item.shoot = ModContent.ProjectileType<WhiteWhaleProjectile>(); // Remember that we must use ProjectileType<>() since it is a modded projectile!
                                                                              // Check out ExampleCloneProjectile to see how this projectile is different from the Vanilla Meowmere projectile.

            // While we're at it, let's make our weapon's stats a bit stronger than the Meowmere, which can be done
            // by using math on each given stat.

            Item.damage = 49; // Makes this weapon's damage double the Meowmere's damage.
            Item.shootSpeed *= 1.8f; // Makes this weapon's projectiles shoot 25% faster than the Meowmere's projectiles.
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, 0f); // Adjust X and Y as needed to bring it closer
        }
    }
}