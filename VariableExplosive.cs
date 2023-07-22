using Microsoft.Xna.Framework;
using RandBombs.Configs;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RandBombs
{
    public class VariableExplosive : ModProjectile
    {
        readonly byte DefaultSize = 15;

        public override void SetDefaults()
        {
            Projectile.width = DefaultSize;
            Projectile.height = DefaultSize;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //change damage when spawning from certain blocks
        }

        public override void AI()
        {
            var BombConfigs = ModContent.GetInstance<BombConfigs>();

            if (Projectile.timeLeft < 3)
            {
                Projectile.damage = BombConfigs.Damage;
                Projectile.alpha = 255;
                Projectile.Resize(16 + 32 * BombConfigs.ExplosionSize, 16 + 32 * BombConfigs.ExplosionSize);

                if (BombConfigs.ScaleDamageInHardmode && Main.hardMode)
                {
                    Projectile.damage *= 2;
                }
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            var BombConfigs = ModContent.GetInstance<BombConfigs>();
            if (target.Hitbox.Distance(Projectile.Center) < BombConfigs.ExplosionSize + 8)
            {
                return true;
            }
            else { return false; }
        }

        public override void Kill(int timeLeft) //Majority of this is directly copied from ExampleExplosive, just made the explosion size vary
        {

            // Play explosion sound
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            // Smoke Dust spawn
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire Dust spawn
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }

            // Large Smoke Gore spawn
            for (int g = 0; g < 2; g++)
            {
                var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y -= 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y -= 1.5f;
            }

            //Exploding the tiles
            if (Projectile.owner == Main.myPlayer)
            {
                int explosionRadius = ModContent.GetInstance<BombConfigs>().ExplosionSize;
                int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
                int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
                int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
                int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);

                // Ensure that all tile coordinates are within the world bounds
                Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);

                // These 2 methods handle actually mining the tiles and walls while honoring tile explosion conditions
                bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
                Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);

                
            }
        }
    }
}
