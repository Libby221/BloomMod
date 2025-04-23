using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace libbymod.Content.Buffs
{
    public class ShockedDebuff : ModBuff
    {


        // Called every frame the debuff is active
        public override void Update(NPC npc, ref int buffIndex)
        {
            // If the NPC is a boss, do nothing and return
            if (npc.boss)
            {
                return;
            }
            // Reduce movement speed by 50%
            npc.velocity *= 0.75f;

            // Inflict 5 damage every second (every 60 ticks)
            if (npc.buffTime[buffIndex] % 60 == 0)  // Apply damage every second (60 ticks)
            {
                //npc.StrikeNPC(5, 0f, 0, false, false, false);  // Inflict 5 damage to the NPC
            }

            // Spawn electric dust constantly at the NPC's position
            for (int i = 0; i < 3; i++) // Adjust the number of dust particles spawned
            {
                // Create electric dust near the NPC's position
                var dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Electric, 0f, 0f, 100, default, 0.5f);

                // Add some random velocity to spread the dust slightly
                dust.velocity += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
            }
        }
    }
}