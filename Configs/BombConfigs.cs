using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RandBombs.Configs
{
    public class BombConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        [Header("Randomisation")]
        public bool CompletelyRandomise;

        [DefaultValue(200)]
        [Range(25, 5000)]
        public int RandomiseDenominator;

        [Header("Information")]
        public bool DefaultValues;

        [DefaultValue(12500)]
        [Range(1, 200000)]
        [Header("BombOptions")]
        public int NumberOfBombs;

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

        [Header("JoinMessage")]
        public bool ShowJoinMessage;

        [DefaultValue(true)]
        private bool FollowNormalExplosionRules;
    }
}
