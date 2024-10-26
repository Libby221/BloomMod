using libbymod.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace libbymod.Content.NPCs
{
    [AutoloadHead]
    class Mermaid : ModNPC
    {


        // The list of items in the traveler's shop. Saved with the world and set when the traveler spawns
        public List<Item> shopItems = new List<Item>();

        private static bool IsNpcOnscreen(Vector2 center)
        {
            int w = NPC.sWidth + NPC.safeRangeX * 2;
            int h = NPC.sHeight + NPC.safeRangeY * 2;
            Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.player)
            {
                // If any player is close enough to the traveling merchant, it will prevent the npc from despawning
                if (player.active && player.getRect().Intersects(npcScreenRect)) return true;
            }
            return false;
        }

        public void CreateNewShop()
        {
            // create a list of item ids
            var itemIds = new List<int>();

            // For each slot we add a switch case to determine what should go in that slot
            switch (Main.rand.Next(2))
            {
                case 0:
                    itemIds.Add(ModContent.ItemType<H2OBomb>());
                    break;
                default:
                    itemIds.Add(ModContent.ItemType<WhiteWhale>());
                    break;
            }

            // convert to a list of items
            shopItems = new List<Item>();
            foreach (int itemId in itemIds)
            {
                Item item = new Item();
                item.SetDefaults(itemId);
                shopItems.Add(item);
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.HatOffsetY[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            Main.npcFrameCount[NPC.type] = 16;
            AnimationType = NPCID.Guide;
            TownNPCStayingHomeless = true;
            CreateNewShop();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["itemIds"] = shopItems;
        }

        public override void LoadData(TagCompound tag)
        {
            shopItems = tag.Get<List<Item>>("shopItems");
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return new MermaidProfile();
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Yonca",
                "Christine",
                "Ariel",
                "Aqua"
            };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl >= 0)
            {
                chat.Add(Language.GetTextValue("Mods.libbymod.Dialogue.Mermaid.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            }

            chat.Add(Language.GetTextValue("Mods.libbymod.Dialogue.Mermaid.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.libbymod.Dialogue.Mermaid.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.libbymod.Dialogue.Mermaid.StandardDialogue3"));

            string hivePackDialogue = Language.GetTextValue("Mods.libbymod.Dialogue.Mermaid.HiveBackpackDialogue");
            chat.Add(hivePackDialogue);

            string dialogueLine = chat; // chat is implicitly cast to a string.
            if (hivePackDialogue.Equals(dialogueLine))
            {
                // Main.npcChatCornerItem shows a single item in the corner, like the Angler Quest chat.
                Main.npcChatCornerItem = ItemID.HiveBackpack;
            }

            return dialogueLine;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void AI()
        {
            NPC.homeless = true; // Make sure it stays homeless
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.Trident;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }
    }

    public class MermaidProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("libbymod/Content/NPCs/Mermaid");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("libbymod/Content/NPCs/ExamplePerson_Party");

            return ModContent.Request<Texture2D>("libbymod/Content/NPCs/Mermaid");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("libbymod/Content/NPCs/Mermaid_Head");
    }
}