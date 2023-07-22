using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RandBombs.Configs
{
    public class BombConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(12500)]
        [Range(1, 200000)]
        public int NumberOfBombs;

        public bool DefaultValues;

        [DefaultValue(8)]
        [Range(1, 50)]
        public int ExplosionSize;

        [DefaultValue(150)]
        [Range(1, 1000)]
        public int Damage;

        [DefaultValue(false)]
        public bool ScaleDamageInHardmode;

        [DefaultValue(true)]
        public bool SingleUseBombs;

        [DefaultValue(true)]
        private bool FollowNormalExplosionRules;
    }
}
