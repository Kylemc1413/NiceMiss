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

        public virtual int min { get; set; }
        public virtual int max { get; set; }

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color color { get; set; }

        public HitscoreColor()
        {
        }

        public HitscoreColor(TypeEnum type, int min, int max, Color color)
        {
            this.type = type;
            this.min = min;
            this.max = max;
            this.color = color;
        }

        public enum TypeEnum
        {
            Miss,
            Hitscore,
            Angle,
            Accuracy
        }
    }
}
