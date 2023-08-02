﻿using RandBombs.Configs;
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
            //make sure this doesn't run more than once
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                ref Point16[] BombLocations = ref ModContent.GetInstance<Explosives>().BombLocations; //get the Point16[] of bombs
                ref bool BombsGenerated = ref ModContent.GetInstance<Explosives>().BombsGenerated; //get whether or not bombs are present in the world

                if (player.altFunctionUse == 2)
                {
                    if (BombLocations.Length > 250000)
                    {
                        Main.NewText(
                            """
                            There are too many mines existing in this world
                            If you think you need more for whatever reason, you can come and talk to me @mangochicken_ on discord
                            """);
                        return true;
                    }

                    int i = BombLocations.Length;
                    int b = 0;
                    BombLocations = new Point16[BombLocations.Length + 500];
                    for (; i < BombLocations.Length; i++)
                    {
                        for (int k = 0; k < 50; k++)
                        {
                            Point16 point = new(Main.rand.Next(0, Main.maxTilesX), Main.rand.Next(0, Main.maxTilesY));
                            if (BombLocations.Contains(point) || Main.tile[point.ToPoint()].HasTile)
                            {
                                continue;
                            }
                            else
                            {
                                BombLocations[i] = point;
                                b++;
                                break;
                            }
                        }
                    }
                    Main.NewText($"Added 500 mine spots, and filled {b} of them");
                }
                else
                {
                    //check if there are actually any mines
                    if (BombsGenerated)
                    {
                        int b = 0; //number of bombs generated by this use of the item

                        for (int i = 0; i < BombLocations.Length; i++)
                        {
                            if (BombLocations[i].Equals(Point16.NegativeOne) || BombLocations[i].Equals(Point16.Zero))
                            {
                                for (int k = 0; k < 500; k++)
                                {
                                    Point16 point = new(Main.rand.Next(0, Main.maxTilesX), Main.rand.Next(0, Main.maxTilesY));
                                    if (BombLocations.Contains(point) || Main.tile[point.ToPoint()].HasTile)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        BombLocations[i] = point;
                                        b++;
                                        break;
                                    }
                                } //500 chances for each point to be valid, just an arbitrary number chosen to limit the for loop
                            }

                            if (b >= 500)
                            {
                                break;
                            }

                            continue;
                        }

                        Main.NewText($"Generated {b} more bombs");
                    }
                    else
                    {
                        BombLocations = new Point16[ModContent.GetInstance<BombConfigs>().NumberOfBombs];

                        for (int i = 0; i < BombLocations.Length; i++)
                        {
                            Point16 point = new(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, Main.maxTilesY));
                            if (BombLocations.Contains(point) || !Main.tile[point.ToPoint()].HasTile)
                            {
                                BombLocations[i] = Point16.NegativeOne;
                            }
                            else
                            {
                                BombLocations[i] = point;
                            }
                        }

                        BombsGenerated = true;
                        Main.NewText($"generated {ModContent.GetInstance<BombConfigs>().NumberOfBombs} initial mines (may have appeared in builds, be careful)");
                    }
                }
            }

            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}