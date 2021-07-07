using System;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.ComponentModel;
using NiceMiss.Configuration;

namespace NiceMiss.UI
{
    internal class ModifierUI : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private GameplaySetupViewController gameplaySetupViewController;
        public event PropertyChangedEventHandler PropertyChanged;

        [UIComponent("left-color-setting")]
        private RectTransform leftColorSetting;

        private Transform leftColorModal;

        [UIComponent("right-color-setting")]
        private RectTransform rightColorSetting;

        private Transform rightColorModal;

        public ModifierUI(GameplaySetupViewController gameplaySetupViewController)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab(nameof(NiceMiss), "NiceMiss.UI.modifierUI.bsml", this);
            gameplaySetupViewController.didDeactivateEvent += CloseModalsOnDismiss;
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab(nameof(NiceMiss));
            gameplaySetupViewController.didDeactivateEvent -= CloseModalsOnDismiss;
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            leftColorModal = leftColorSetting.transform.Find("BSMLModalColorPicker");
            rightColorModal = rightColorSetting.transform.Find("BSMLModalColorPicker");
        }

        private void CloseModalsOnDismiss(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (leftColorSetting != null && leftColorModal != null)
            {
                leftColorModal.SetParent(leftColorSetting);
                leftColorModal.gameObject.SetActive(false);
            }

            if (rightColorSetting != null && rightColorModal != null)
            {
                rightColorModal.SetParent(rightColorSetting);
                rightColorModal.gameObject.SetActive(false);
            }
        }

        [UIValue("enabled")]
        public bool modEnabled
        {
            get => PluginConfig.Instance.Enabled;
            set
            {
                PluginConfig.Instance.Enabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(modEnabled)));
            }
        }

        [UIValue("notuseMultiplier")]
        private bool notuseMultiplier => !useMultiplier;

        [UIValue("useMultiplier")]
        public bool useMultiplier
        {
            get => PluginConfig.Instance.UseMultiplier;
            set
            {
                PluginConfig.Instance.UseMultiplier = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(useMultiplier)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(notuseMultiplier)));
            }
        }

        [UIValue("colorMultiplier")]
        public float colorMultiplier
        {
            get => PluginConfig.Instance.ColorMultiplier;
            set
            {
                PluginConfig.Instance.ColorMultiplier = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(colorMultiplier)));
            }
        }

        [UIValue("leftMiss")]
        public Color leftMissColor
        {
            get => PluginConfig.Instance.LeftMissColor;
            set
            {
                PluginConfig.Instance.LeftMissColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(leftMissColor)));
            }
        }

        [UIValue("rightMiss")]
        public Color rightMissColor
        {
            get => PluginConfig.Instance.RightMissColor;
            set
            {
                PluginConfig.Instance.RightMissColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rightMissColor)));
            }
        }
    }
}
