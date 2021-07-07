using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NiceMiss.Configuration
{
    public class AccColor
    {
        public virtual int threshold { get; set; }

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color color { get; set; }

        public AccColor()
        {
        }

        public AccColor(int threshold, Color color)
        {
            this.threshold = threshold;
            this.color = color;
        }
    }
}
