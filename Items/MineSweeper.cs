using RandBombs.Configs;
using RandBombs.Systems;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RandBombs.Items
{
    public class MineSweeper : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.autoReuse = false;
            Item.useTime = 30;
            Item.useAnimation = 30;
        }


        //TODO: Update this to be less laggy, and remake the code to be more readable. Make a flow chart or smth
        public override bool? UseItem(Player player)
        {
            Main.NewText("Test 1");
            //make sure this doesn't run more than once
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                ref Point16[] BombLocations = ref ModContent.GetInstance<Explosives>().BombLocations; //get the Point16[] of bombs
                ref bool BombsGenerated = ref ModContent.GetInstance<Explosives>().BombsGenerated; //get whether or not bombs are present in the world

                //check if there are actually any mines
                if (BombsGenerated)
                {
                    int i = 1;
                    int b = 0;
                    if (BombLocations.Length < 500) { BombLocations = new Point16[BombLocations.Length + 500]; }
                    for (int j = 0; j < BombLocations.Length && i < 500; j++)
                    {
                        if (BombLocations[j] == Point16.NegativeOne || BombLocations[j] == Point16.Zero)
                        {
                            for (int k = 0; k < 500; k++)
                            {
                                Point16 point = new(Main.rand.Next(0, Main.maxTilesX), Main.rand.Next(0, Main.maxTilesY));

                                if (BombLocations.Contains(point)) { continue; } //make sure no two values are the same
                                if (Main.tile[point.ToPoint()].HasTile) { BombLocations[j] = Point16.NegativeOne; continue; }
                                BombLocations[j] = point;
                                b++;
                            }
                            i++;
                        }
                    }
                    //if (i < 500) 
                    //{
                    //    BombLocations = new Point16[BombLocations.Length + 500];
                    //    Main.NewText("added 500 mine space to the Bomb Point16[] array");
                    //}
                    Main.NewText($"Generated {i} more bombs");
                }
                else
                {
                    BombLocations = new Point16[ModContent.GetInstance<BombConfigs>().NumberOfBombs];

                    for (int i2 = 0; i2 < BombLocations.Length; i2++)
                    {
                        Point16 point = new(Main.rand.Next(0, Main.maxTilesX), Main.rand.Next(0, Main.maxTilesY));
                        if (BombLocations.Contains(point) || !Main.tile[point.ToPoint()].HasTile)
                        {
                            BombLocations[i2] = Point16.NegativeOne;
                        }
                        else
                        {
                            BombLocations[i2] = point;
                        }
                    }

                    BombsGenerated = true;
                    Main.NewText("generated initial mines (may have appeared in builds, be careful)");
                }
            }

            return true;
        }
    }
}
