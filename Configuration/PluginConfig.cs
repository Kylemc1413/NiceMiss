using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NiceMiss.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool Enabled { get; set; } = false;
        public virtual float ColorMultiplier { get; set; } = 1.85f;
        public virtual bool UseMultiplier { get; set; } = true;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color LeftMissColor { get; set; } = Color.red;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color RightMissColor { get; set; } = Color.blue;

        public virtual bool AccColoring { get; set; } = false;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color AccMissColor { get; set; } = Color.black;

        [UseConverter(typeof(ListConverter<AccColor>))]
        public virtual List<AccColor> AccColors { get; set; } = new List<AccColor>() { new AccColor(115, Color.white) };

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
}
