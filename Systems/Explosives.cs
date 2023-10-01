using Microsoft.Xna.Framework;
using System;
using RandBombs.Configs;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.IO;

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

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            tasks.Add(new BombsPass("Random Explosives Bombs", 3f));
        }

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
    }

    public class BombsPass : GenPass
    {
        public Point16[] BombLocations;
        public BombsPass(string name, float loadWeight) : base(name, loadWeight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Hidden Explosives";

            int amountBombs = ModContent.GetInstance<BombConfigs>().NumberOfBombs;

            BombLocations = new Point16[amountBombs];
            int n = 0;
            for (int i = 0; i < amountBombs; i++)
            {
                progress.Set((float)i / amountBombs * 0.8f);
                Point16 point = new(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, Main.maxTilesY));
                if (BombLocations.Contains(point))
                {
                    n++;
                    continue;
                }
                else
                {
                    BombLocations[i] = point;
                }

            }
            ModContent.GetInstance<RandBombs>().Logger.InfoFormat("{0} mines are inactive", n);

            for (int k = 0; k < amountBombs; k++)
            {
                progress.Set(0.8 + (float)k / amountBombs * 0.2f);
                Point16 point = BombLocations[k];
                if (!Main.tile[point.ToPoint()].HasTile)
                {
                    BombLocations[k] = Point16.NegativeOne;
                }
            }

            ModContent.GetInstance<Explosives>().BombLocations = BombLocations;
            ModContent.GetInstance<Explosives>().BombsGenerated = true;
        }
    }

    public class ExplosiveTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //bool WorldGenActive = WorldGen.
            if (ModContent.GetInstance<BombConfigs>().CompletelyRandomise == false)
            {
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
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j, Main.tile[i, j].TileType.ToString()), new Vector2(i, j).ToWorldCoordinates(), new(0, 0), ModContent.ProjectileType<VariableExplosive>(), damage, 5f);
                    if (BombConfig.SingleUseBombs)
                    {
                        int index = Array.IndexOf(Explosives.BombLocations, new Point16(i, j));
                        Explosives.BombLocations[index] = Point16.NegativeOne;
                        return;
                    }
                }
            }
            else
            {
                var BombConfig = ModContent.GetInstance<BombConfigs>();

                if (Main.rand.Next(0, BombConfig.RandomiseDenominator) == 0)
                {
                    int damage = BombConfig.Damage;
                    if (BombConfig.ScaleDamageInHardmode && Main.hardMode == true)
                    {
                        damage *= 2;
                    }

                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j, Main.tile[i, j].TileType.ToString()), new Vector2(i, j).ToWorldCoordinates(), new(0), ModContent.ProjectileType<VariableExplosive>(), damage, 5f);
                }
            }
        }
    }

    public class JoinMessage : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (ModContent.GetInstance<BombConfigs>().ShowJoinMessage)
            {
                Main.NewText(
                    """
                    You seem to enjoy explosions
                    This mod is set up by default for a small world
                    Please make sure to check the configs for this mod so you can make this experience to your liking
                    """);
            }
        }
    }
}