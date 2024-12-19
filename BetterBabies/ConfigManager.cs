using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using CSync.Lib;
using CSync.Extensions;
using System.Runtime.Serialization;

namespace BetterBabies
{
    internal class ConfigManager : SyncedConfig2<ConfigManager>
    {



        [SyncedEntryField] public static SyncedEntry<bool> CanBabyGoOutside;

        [SyncedEntryField] public static SyncedEntry<bool> CanBabyGoIntoOrbit;

        [SyncedEntryField] public static SyncedEntry<bool> CanBabyGrowUp;

        //Selling

        [SyncedEntryField] public static SyncedEntry<bool> CanSellBaby;

        [SyncedEntryField] public static SyncedEntry<int> BabyPriceMinInclusive;
        [SyncedEntryField] public static SyncedEntry<int> BabyPriceMaxExclusive;


        //public static ConfigEntry<int> DebugLevel;

        public ConfigManager(ConfigFile configFile) : base(MyPluginInfo.PLUGIN_GUID)
        {
            

            CreateConfigs(configFile);

            CSync.Lib.ConfigManager.Register(this);
        }

        public void CreateConfigs(ConfigFile cfg) 
        {
            CanBabyGoOutside = cfg.BindSyncedEntry("General",
                "CanBabyGoOutside",
                true,
                "Can the baby go outside without crying?");

            CanBabyGoIntoOrbit = cfg.BindSyncedEntry("General",
                "CanBabyGoIntoOrbit",
                true,
                "Can the baby go into orbit? (unstable)");

            CanBabyGrowUp = cfg.BindSyncedEntry("General",
                "CanBabyGrowUp",
                true,
                "Can the baby become an adult?");

            //Selling

            CanSellBaby = cfg.BindSyncedEntry("Selling",
                "CanSellBaby",
                true,
                "Can the Baby be sold at the company");

            BabyPriceMinInclusive = cfg.BindSyncedEntry("Selling",
                "BabyPriceMinInclusive",
                100,
                "Minimum baby price when sold. (inclusive");

            BabyPriceMaxExclusive = cfg.BindSyncedEntry("Selling",
                "BabyPriceMaxExclusive",
                150,
                "Maximum baby price when sold. (exclusive");
        }

    }

}
