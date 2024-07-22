/*
 * Copyright (C) 2024 Game4Freak.io
 * This mod is provided under the Game4Freak EULA.
 * Full legal terms can be found at https://game4freak.io/eula/
 */

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Cupboard Accepts Anything", "VisEntities", "1.0.0")]
    [Description("Lets you put any item in tool cupboards.")]
    public class CupboardAcceptsAnything : RustPlugin
    {
        #region Fields

        private static CupboardAcceptsAnything _plugin;
        private static Configuration _config;

        #endregion Fields

        #region Configuration

        private class Configuration
        {
            [JsonProperty("Version")]
            public string Version { get; set; }

            [JsonProperty("Allow All Items")]
            public bool AllowAllItems { get; set; }

            [JsonProperty("Blocked Item Short Names")]
            public List<string> BlockedItemShortNames { get; set; }

            [JsonProperty("Blocked Item Categories")]
            public List<string> BlockedItemCategories { get; set; }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            _config = Config.ReadObject<Configuration>();

            if (string.Compare(_config.Version, Version.ToString()) < 0)
                UpdateConfig();

            SaveConfig();
        }

        protected override void LoadDefaultConfig()
        {
            _config = GetDefaultConfig();
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(_config, true);
        }

        private void UpdateConfig()
        {
            PrintWarning("Config changes detected! Updating...");

            Configuration defaultConfig = GetDefaultConfig();

            if (string.Compare(_config.Version, "1.0.0") < 0)
                _config = defaultConfig;

            PrintWarning("Config update complete! Updated from version " + _config.Version + " to " + Version.ToString());
            _config.Version = Version.ToString();
        }

        private Configuration GetDefaultConfig()
        {
            return new Configuration
            {
                Version = Version.ToString(),
                AllowAllItems = true,
                BlockedItemShortNames = new List<string>
                {
                    "coffeecan.helmet",
                    "corn",
                    "furnace"
                },
                BlockedItemCategories = new List<string>
                {
                    "Weapons",
                    "Traps",
                }
            };
        }

        #endregion Configuration

        #region Oxide Hooks

        private void Init()
        {
            _plugin = this;
        }

        private void Unload()
        {
            _config = null;
            _plugin = null;
        }

        private object OnItemFilter(Item item, StorageContainer storageContainer, int targetSlot)
        {
            if (item == null || storageContainer == null)
                return null;

            if (!(storageContainer is BuildingPrivlidge))
                return null;

            if (_config.AllowAllItems)
            {
                if (_config.BlockedItemShortNames.Contains(item.info.shortname) ||
                    _config.BlockedItemCategories.Contains(item.info.category.ToString()))
                {
                    return false;
                }

                return true;
            }

            if (_config.BlockedItemShortNames.Contains(item.info.shortname) ||
                _config.BlockedItemCategories.Contains(item.info.category.ToString()))
            {
                return false;
            }

            return null;
        }

        #endregion Oxide Hooks
    }
}