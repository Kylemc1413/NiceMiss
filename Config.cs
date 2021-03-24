using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace NiceMiss
{
    public class Config
    {
        internal static BS_Utils.Utilities.Config config = new BS_Utils.Utilities.Config("NiceMiss");
        public static bool enabled = false;
        public static float colorMultiplier = 1.85f;
        public static bool useMultiplier = true;
        public static Color leftMissColor = Color.red;
        public static Color rightMissColor = Color.blue;
        public static void Read()
        {
            //  ModifierUI.instance.enabled = config.GetBool("DiffReducer", "Enabled", false, true);
            enabled = config.GetBool("NiceMiss", "Enabled", false, true);
            colorMultiplier = config.GetFloat("NiceMiss", "Color Multiplier", 1.85f, true);
            useMultiplier = config.GetBool("NiceMiss", "Use Color Multiplier", true, true);
            leftMissColor = new Color(
                config.GetFloat("NiceMiss", "leftMissColorR", 1f, true),
                config.GetFloat("NiceMiss", "leftMissColorG", 0f, true),
                config.GetFloat("NiceMiss", "leftMissColorB", 0f, true));
            rightMissColor = new Color(
                config.GetFloat("NiceMiss", "rightMissColorR", 0f, true),
                config.GetFloat("NiceMiss", "rightMissColorG", 0f, true),
                config.GetFloat("NiceMiss", "rightMissColorB", 1f, true));
        }

        public static void Write()
        {
            //    Plugin.log.Debug("Writing config");
            config.SetBool("NiceMiss", "Enabled", enabled);
            config.SetFloat("NiceMiss", "Color Multiplier", colorMultiplier);
            config.SetBool("NiceMiss", "Use Color Multiplier", useMultiplier);
            config.SetFloat("NiceMiss", "leftMissColorR", leftMissColor.r);
            config.SetFloat("NiceMiss", "leftMissColorG", leftMissColor.g);
            config.SetFloat("NiceMiss", "leftMissColorB", leftMissColor.b);
            config.SetFloat("NiceMiss", "rightMissColorR", rightMissColor.r);
            config.SetFloat("NiceMiss", "rightMissColorG", rightMissColor.g);
            config.SetFloat("NiceMiss", "rightMissColorB", rightMissColor.b);

        }
    }
}
