using System;
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
        public event Action ConfigChanged;
        public virtual bool Enabled { get; set; } = false;

        [UseConverter(typeof(EnumConverter<ModeEnum>))]
        public virtual ModeEnum Mode { get; set; } = ModeEnum.Multiplier;

        public virtual float ColorMultiplier { get; set; } = 1.85f;

        public virtual float OutlineWidth { get; set; } = 4f;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color LeftMissColor { get; set; } = Color.red;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color RightMissColor { get; set; } = Color.blue;

        [UseConverter(typeof(ListConverter<HitscoreColor>))]
        public virtual List<HitscoreColor> HitscoreColors { get; set; } = new List<HitscoreColor>();

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            ConfigChanged?.Invoke();
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }

        public enum ModeEnum
        {
            Multiplier,
            Outline
        }
    }
}
