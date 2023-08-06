using RandBombs.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace RandBombs
{
    public class BombGNPC : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Demolitionist)
            {
                shop.Add<MineSweeper>();
            }
        }
    }
}
