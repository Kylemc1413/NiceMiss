using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMUI;
using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
namespace NiceMiss.UI
{
    class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        [UIValue("notuseMultiplier")]
        private bool notuseMultiplier
        {
            get => !useMultiplier;
            set
            {
            }
        }
        [UIValue("useMultiplier")]
        public bool useMultiplier
        {
            get => Config.useMultiplier;
            set
            {
                Config.useMultiplier = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("notuseMultiplier");
                Config.Write();
            }
        }
        [UIAction("setUseMultiplier")]
        public void setUseMultiplier(bool value)
        {
            useMultiplier = value;
        }
       [UIValue("enabled")]
       public bool modEnabled
        {
            get => Config.enabled;
            set
            {
                Config.enabled = value;
                Config.Write();
            }
        }
        [UIAction("setEnabled")]
        public void setEnabled(bool value)
        {
            modEnabled = value;
        }
        [UIValue("colorMultiplier")]
        public float colorMultiplier
        {
            get => Config.colorMultiplier;
            set
            {
                Config.colorMultiplier = value;
                Config.Write();
            }
        }
        [UIAction("setColorMultiplier")]
        public void setColorMultiplier(float value)
        {
            colorMultiplier = value;
        }
        [UIValue("leftMiss")]
        public Color leftMissColor
        {
            get => Config.leftMissColor;
            set
            {
                Config.leftMissColor = value;
                Config.Write();
            }
        }
        [UIAction("setLeftMiss")]
        public void setLeftMiss(Color value)
        {
            leftMissColor = value;
        }
        [UIValue("rightMiss")]
        public Color rightMissColor
        {
            get => Config.rightMissColor;
            set
            {
                Config.rightMissColor = value;
                Config.Write();
            }
        }
        [UIAction("setRightMiss")]
        public void setRightMiss(Color value)
        {
            rightMissColor = value;
        }
    }
}
