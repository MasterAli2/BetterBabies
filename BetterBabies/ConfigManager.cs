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
    internal class ConfigManager : SyncedConfig<ConfigManager>
    {



        [DataMember] public static SyncedEntry<bool> CanBabyGoOutside;

        [DataMember] public static SyncedEntry<bool> CanBabyGoIntoOrbit;

        //Selling

        [DataMember] public static SyncedEntry<bool> CanSellBaby;

        [DataMember] public static SyncedEntry<int> BabyPriceMinInclusive;
        [DataMember] public static SyncedEntry<int> BabyPriceMaxExclusive;

        //Debug

        public static ConfigEntry<bool> DisableAgentOnSell;

        //public static ConfigEntry<int> DebugLevel;

        public ConfigManager(ConfigFile configFile) : base(MyPluginInfo.PLUGIN_GUID)
        {
            CSync.Lib.ConfigManager.Register(this);

            CreateConfigs(configFile);

            
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

            //Debug

            DisableAgentOnSell = cfg.Bind("Debug",
                "DisableAgentOnSell",
                true,
                "disable baby nav mesh agent on sell? toggle this on or off might fix issues regarding selling");

            /*
            DebugLevel = cfg.Bind("Debug",
                "DebugLevel",
                0,
                "Currently Unused.");
            */
        }

    }

}
