using System;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.ComponentModel;

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
            get => Config.enabled;
            set
            {
                Config.enabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(modEnabled)));
                Config.Write();
            }
        }

        [UIValue("notuseMultiplier")]
        private bool notuseMultiplier => !useMultiplier;

        [UIValue("useMultiplier")]
        public bool useMultiplier
        {
            get => Config.useMultiplier;
            set
            {
                Config.useMultiplier = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(useMultiplier)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(notuseMultiplier)));
                Config.Write();
            }
        }

        [UIValue("colorMultiplier")]
        public float colorMultiplier
        {
            get => Config.colorMultiplier;
            set
            {
                Config.colorMultiplier = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(colorMultiplier)));
                Config.Write();
            }
        }

        [UIValue("leftMiss")]
        public Color leftMissColor
        {
            get => Config.leftMissColor;
            set
            {
                Config.leftMissColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(leftMissColor)));
                Config.Write();
            }
        }

        [UIValue("rightMiss")]
        public Color rightMissColor
        {
            get => Config.rightMissColor;
            set
            {
                Config.rightMissColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(rightMissColor)));
                Config.Write();
            }
        }
    }
}
