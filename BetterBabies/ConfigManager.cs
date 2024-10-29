using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;

namespace BetterBabies
{
    internal class ConfigManager
    {

        public BaseUnityPlugin Owner;

        public ConfigEntry<bool> CanBabyGoOutside;

        public ConfigEntry<bool> CanBabyGoIntoOrbit;

        //Selling

        public ConfigEntry<int> BabyPriceMinInclusive;
        public ConfigEntry<int> BabyPriceMaxExclusive;

        //Debug

        public ConfigEntry<bool> disableAgentOnSell;

        public ConfigManager(BaseUnityPlugin owner)
        {
            Owner = owner;

            CreateConfigs();
        }

        public void CreateConfigs() 
        {
            CanBabyGoOutside = Owner.Config.Bind("General",
                "CanBabyGoOutside",
                true,
                "Can the baby go outside without crying?");

            CanBabyGoIntoOrbit = Owner.Config.Bind("General",
                "CanBabyGoIntoOrbit",
                true,
                "Can the baby go into orbit? (unstable)");

            //Selling

            BabyPriceMinInclusive = Owner.Config.Bind("Selling",
                "BabyPriceMinInclusive",
                100,
                "Minimum baby price when sold. (inclusive");

            BabyPriceMaxExclusive = Owner.Config.Bind("Selling",
                "BabyPriceMaxExclusive",
                150,
                "Maximum baby price when sold. (exclusive");

            //Debug

            disableAgentOnSell = Owner.Config.Bind("Debug",
                "disableAgentOnSell",
                true,
                "disable baby nav mesh agent on sell? toggle this on or off might fix issues regarding selling");

        }

    }
}
