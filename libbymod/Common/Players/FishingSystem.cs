using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using libbymod.Content.NPCs;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Personalities;
using Terraria.ID;

namespace libbymod.Common.Players
{
    public class FishingPlayer : ModPlayer
    {
        public void OnCatchFish(Player player, FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (player.ZoneBeach && attempt.playerFishingConditions.BaitItemType == ItemID.MasterBait && inWater)
            {
                // In this example, we will fish up an Example Person from the water in Example Surface Biome,
                // as long as there isn't one in the world yet
                // NOTE: if a fishing rod has multiple bobbers, then each one can spawn the NPC
                int npc = ModContent.NPCType<Mermaid>();
                if (!NPC.AnyNPCs(npc))
                {
                    // Make sure itemDrop = -1 when summoning an NPC, as otherwise terraria will only spawn the item
                    npcSpawn = npc;
                    itemDrop = -1;

                    // Also, to make it cooler, we will make a special sonar message for when it shows up
                    sonar.Text = "It's a Big One!";
                    sonar.Color = Color.SkyBlue;
                    sonar.DurationInFrames = 300;

                    return; // This is important so your code after this that rolls items will not run
                }
            }
        }
    }
}
