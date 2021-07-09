using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using HMUI;
using IPA.Utilities;
using NiceMiss.Configuration;
using System;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace NiceMiss.UI
{
    internal class HitscoreModal : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly GameplaySetupViewController gameplaySetupViewController;

        private bool parsed;
        private HitscoreColor hitscoreColor;
        public event Action<HitscoreColor> EntryAdded;
        public event PropertyChangedEventHandler PropertyChanged;

        [UIComponent("thresholdSlider")]
        private SliderSetting thresholdSlider;

        [UIComponent("leftButton")]
        private RectTransform leftButton;

        [UIComponent("rightButton")]
        private RectTransform rightButton;

        [UIComponent("hitscoreColorSetting")]
        private RectTransform hitscoreColorSetting;

        private ModalView hitscoreColorModal;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        [UIComponent("modal")]
        private readonly RectTransform modalTransform;

        [UIComponent("modal")]
        private ModalView modalView;

        [UIParams]
        private readonly BSMLParserParams parserParams;

        public HitscoreModal(GameplaySetupViewController gameplaySetupViewController)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
        }

        public void Initialize()
        {
            gameplaySetupViewController.didDeactivateEvent += CloseModalsOnDismiss;
        }

        public void Dispose()
        {
            gameplaySetupViewController.didDeactivateEvent -= CloseModalsOnDismiss;
        }

        private void Parse(Transform parent)
        {
            if (!parsed)
            {
                BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "NiceMiss.UI.hitscoreModal.bsml"), parent.gameObject, this);
                parsed = true;
                hitscoreColorModal = hitscoreColorSetting.transform.Find("BSMLModalColorPicker").GetComponent<ModalView>();
                SliderButton.Register(leftButton, rightButton, thresholdSlider, 1);
            }
            FieldAccessor<ModalView, bool>.Set(ref hitscoreColorModal, "_animateParentCanvas", false);
            FieldAccessor<ModalView, bool>.Set(ref modalView, "_animateParentCanvas", true);
        }

        public void ShowModal(Transform parent)
        {
            hitscoreColor = new HitscoreColor();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(type)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(useHitscore)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(hitscoreThreshold)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(color)));

            Parse(parent);
            parserParams.EmitEvent("closeModal");
            parserParams.EmitEvent("openModal");
        }

        private void CloseModalsOnDismiss(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            if (parsed && rootTransform != null && modalTransform != null)
            {
                modalTransform.SetParent(rootTransform);
                modalTransform.gameObject.SetActive(false);
            }

            if (parsed && hitscoreColorSetting != null && hitscoreColorModal != null)
            {
                hitscoreColorModal.transform.SetParent(hitscoreColorSetting);
                hitscoreColorModal.gameObject.SetActive(false);
            }
        }

        [UIValue("type")]
        private int type
        {
            get => (int)hitscoreColor.type;
            set
            {
                hitscoreColor.type = (HitscoreColor.TypeEnum)value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(type)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(useHitscore)));
            }
        }

        [UIAction("typeFormatter")]
        private string TypeFormatter(int typeNum) => ((HitscoreColor.TypeEnum)typeNum).ToString();

        [UIValue("hitscoreThreshold")]
        private int hitscoreThreshold
        {
            get => hitscoreColor.threshold;
            set
            {
                hitscoreColor.threshold = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(hitscoreThreshold)));
            }
        }

        [UIValue("color")]
        private Color color
        {
            get => hitscoreColor.color;
            set
            {
                hitscoreColor.color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(color)));
            }
        }

        [UIAction("addEntry")]
        private void AddEntry() => EntryAdded?.Invoke(hitscoreColor);

        [UIValue("useHitscore")]
        private bool useHitscore => hitscoreColor.type == HitscoreColor.TypeEnum.Hitscore;
    }
}
