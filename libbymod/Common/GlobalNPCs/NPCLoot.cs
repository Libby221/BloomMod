using libbymod.Content.Items;
using libbymod.Content.Items.Weapons;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace libbymod.Common.GlobalNPCs
{
    // This file shows numerous examples of what you can do with the extensive NPC Loot lootable system.
    // You can find more info on the wiki: https://github.com/tModLoader/tModLoader/wiki/Basic-NPC-Drops-and-Loot-1.4
    // Despite this file being GlobalNPC, everything here can be used with a ModNPC as well! See examples of this in the Content/NPCs folder.
    public class libbymodNPCLoot : GlobalNPC
    {
        // ModifyNPCLoot uses a unique system called the ItemDropDatabase, which has many different rules for many different drop use cases.
        // Here we go through all of them, and how they can be used.
        // There are tons of other examples in vanilla! In a decompiled vanilla build, GameContent/ItemDropRules/ItemDropDatabase adds item drops to every single vanilla NPC, which can be a good resource.

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // First, we need to check the npc.type to see if the code is running for the vanilla NPC we want to change
            if (npc.type == NPCID.DoctorBones)
            {
                // This is where we add item drop rules for VampireBat, here is a simple example:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IndianasWhip>()));
            }

            if (npc.type == NPCID.DesertScorpionWalk)
            {
                // This is where we add item drop rules for VampireBat, here is a simple example:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlackSting>(), 20, 0, 1));
            }

            if (npc.type == NPCID.DesertScorpionWall)
            {
                // This is where we add item drop rules for VampireBat, here is a simple example:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlackSting>(), 20, 0, 1));
            }
        }


    }
}