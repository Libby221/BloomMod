using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace libbymod.Content.Buffs
{
    public class IndianasWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 4;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }



    public class IndianasWhipDebuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<IndianasWhipDebuff>())
            {
                // Apply a flat bonus to every hit
                modifiers.FlatBonusDamage += IndianasWhipDebuff.TagDamage * projTagMultiplier;
            }

        }
    }
}