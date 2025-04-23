using libbymod.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace libbymod.Content.Projectiles
{
    public class VoltaicEelProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.RangeMultiplier = 1.6f;
            Projectile.WhipSettings.Segments = 30;

            // use these to change from the vanilla defaults
            // Projectile.WhipSettings.Segments = 20;
            // Projectile.WhipSettings.RangeMultiplier = 1f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Apply the Voltaic Eel debuff to the target.
            target.AddBuff(ModContent.BuffType<VoltaicEelDebuff>(), 240);

            if (Main.rand.NextFloat() < 0.5f)
            {
                target.AddBuff(ModContent.BuffType<ShockedDebuff>(), 120);
            }

            // Set the player to attack the target.
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            // Apply multihit penalty (reduce damage with each additional enemy hit).
            Projectile.damage = (int)(Projectile.damage * 0.66f);

            // Spawn a bunch of smoke dusts at the enemy's position (target.position).
            for (int i = 0; i < 30; i++)
            {
                // Use target.position to spawn dust at the hit NPC's location
                var sparks = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Electric, 0f, 0f, 10, default, 0.5f);

                // Apply velocity to the dust to make it spread out.
                sparks.velocity *= 1.4f;
            }
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            Main.DrawWhip_WhipBland(Projectile, list);
           
            return false;
        }
    }
}