using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NiceMiss.Configuration
{
    public class HitscoreColor
    {
        [UseConverter(typeof(EnumConverter<TypeEnum>))]
        public TypeEnum type;

        public virtual int threshold { get; set; }

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color color { get; set; }

        public HitscoreColor()
        {
        }

        public HitscoreColor(TypeEnum type, int threshold, Color color)
        {
            this.type = type;
            this.threshold = threshold;
            this.color = color;
        }

        public enum TypeEnum
        {
            Miss,
            Hitscore
        }
    }
}
