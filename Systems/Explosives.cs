using Microsoft.Xna.Framework;
using System;
using RandBombs.Configs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Graphics;

namespace RandBombs.Systems
{
    public class Explosives : ModSystem
    {
        public bool BombsGenerated;
        public Point16[] BombLocations;

        public override void PreWorldGen()
        {
            BombLocations = new Point16[1] { new(0, 0) };
        }

        public override void PostWorldGen()
        {
            int amountBombs = ModContent.GetInstance<BombConfigs>().NumberOfBombs;
            BombLocations = new Point16[amountBombs];
            for (int i = 0; i < amountBombs; i++)
            {

                //for (int j = 0; j < 5; j++) //i tried to remove this but it broke everything?? doesn't make sense to me but w/e
                //{
                    Point16 point = new(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, Main.maxTilesY));
                    if (BombLocations.Contains(point))
                    {
                        continue;
                    }
                    else
                    {
                        BombLocations[i] = point;
                        break;
                    }
                //}
            }

            for (int k = 0; k < amountBombs; k++)
            {
                Point16 point = BombLocations[k];
                if (!Main.tile[point.ToPoint()].HasTile)
                {
                    BombLocations[k] = Point16.NegativeOne;
                }
            }

            BombsGenerated = true;
        }

        /*public override void PostWorldGen()
        {
            int amountBombs = ModContent.GetInstance<BombConfigs>().NumberOfBombs;
            BombLocations = new Point16[amountBombs];
            for (int i = 0; i < amountBombs; i++)
            {
                Point16 point = new(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, Main.maxTilesY));
                if (BombLocations.Contains(point)) //make sure no two values are identical
                {
                    BombLocations[i] = Point16.NegativeOne;
                }
                else
                {
                    BombLocations[i] = point;
                    break;
                }
            }

            for (int k = 0; k < amountBombs; k++)
            {
                Point16 point = BombLocations[k];
                if (!Main.tile[point.ToPoint()].HasTile)
                {
                    BombLocations[k] = Point16.NegativeOne;
                }
            }

        BombsGenerated = true;
        }*/

        public override void OnWorldLoad()
        {
            BombsGenerated = false;
            BombLocations = Array.Empty<Point16>();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["BombsGenerated"] = BombsGenerated;
            tag["BombLocations"] = BombLocations;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            BombsGenerated = tag.GetBool("BombsGenerated");
            BombLocations = tag.Get<Point16[]>("BombLocations");
        }

        /*public override void ClearWorld()
        {
            BombsGenerated = false;
            BombLocations = Array.Empty<Point16>();
        }*/
    }

    public class ExplosiveTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //bool WorldGenActive = WorldGen.
            var Explosives = ModContent.GetInstance<Explosives>();
            var BombConfig = ModContent.GetInstance<BombConfigs>();
            Point16 point = new(i, j);
            int damage = BombConfig.Damage;

            if (BombConfig.ScaleDamageInHardmode && Main.hardMode == true)
            {
                damage *= 2;
            }

            if (Explosives.BombLocations.Contains(point))
            {
                Projectile.NewProjectile(new EntitySource_TileBreak(i, j, Main.tile[i,j].TileType.ToString()), new Vector2(i, j).ToWorldCoordinates(), new(0,0), ModContent.ProjectileType<VariableExplosive>(), damage, 5f);
                if (BombConfig.SingleUseBombs)
                {
                    int index = Array.IndexOf(Explosives.BombLocations, new Point16(i, j));
                    Explosives.BombLocations[index] = Point16.NegativeOne;
                }
            }
        }
    }
}