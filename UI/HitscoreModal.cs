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

        [UIComponent("minSlider")]
        private SliderSetting minSlider;

        [UIComponent("maxSlider")]
        private SliderSetting maxSlider;

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

                SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), minSlider, 1);
                SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), maxSlider, 1);
                GameObject.Destroy(leftButton.gameObject);
                GameObject.Destroy(rightButton.gameObject);
            }
            FieldAccessor<ModalView, bool>.Set(ref hitscoreColorModal, "_animateParentCanvas", false);
            FieldAccessor<ModalView, bool>.Set(ref modalView, "_animateParentCanvas", true);
        }

        public void ShowModal(Transform parent)
        {
            hitscoreColor = new HitscoreColor();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(type)));
            UpdateSliderRange();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(notUseMiss)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(min)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(max)));
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
                hitscoreColor.min = 0;
                hitscoreColor.max = 0;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(type)));
                UpdateSliderRange();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(notUseMiss)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(min)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(max)));
            }
        }

        [UIAction("typeFormatter")]
        private string TypeFormatter(int typeNum) => ((HitscoreColor.TypeEnum)typeNum).ToString();

        [UIValue("min")]
        private int min
        {
            get => hitscoreColor.min;
            set
            {
                hitscoreColor.min = value;
                if (value > max)
                {
                    max = value;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(min)));
            }
        }

        [UIValue("max")]
        private int max
        {
            get => hitscoreColor.max;
            set
            {
                hitscoreColor.max = value;
                if (value < min)
                {
                    min = value;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(max)));
            }
        }

        private void UpdateSliderRange()
        {
            switch (hitscoreColor.type)
            {
                case HitscoreColor.TypeEnum.Hitscore:
                    minSlider.slider.maxValue = 115;
                    maxSlider.slider.maxValue = 115;
                    break;
                case HitscoreColor.TypeEnum.Angle:
                    minSlider.slider.maxValue = 100;
                    maxSlider.slider.maxValue = 100;
                    break;
                case HitscoreColor.TypeEnum.Accuracy:
                    minSlider.slider.maxValue = 100;
                    maxSlider.slider.maxValue = 15;
                    break;
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

        [UIValue("notUseMiss")]
        private bool notUseMiss => hitscoreColor.type != HitscoreColor.TypeEnum.Miss;
    }
}
