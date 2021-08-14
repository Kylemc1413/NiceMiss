using System;
using UnityEngine;
using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using BeatSaberMarkupLanguage.GameplaySetup;
using System.ComponentModel;
using NiceMiss.Configuration;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using BeatSaberMarkupLanguage.Components.Settings;

namespace NiceMiss.UI
{
    internal class ModifierUI : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly GameplaySetupViewController gameplaySetupViewController;
        private readonly HitscoreModal hitscoreModal;
        public event PropertyChangedEventHandler PropertyChanged;

        [UIComponent("multiplierSlider")]
        private SliderSetting multiplierSlider;

        [UIComponent("widthSlider")]
        private SliderSetting widthSlider;

        [UIComponent("leftButton")]
        private RectTransform leftButton;

        [UIComponent("rightButton")]
        private RectTransform rightButton;

        [UIComponent("hitscoreList")]
        public CustomListTableData customListTableData;

        [UIComponent("root")]
        private readonly RectTransform rootTransform;

        public ModifierUI(GameplaySetupViewController gameplaySetupViewController, HitscoreModal hitscoreModal)
        {
            this.gameplaySetupViewController = gameplaySetupViewController;
            this.hitscoreModal = hitscoreModal;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab(nameof(NiceMiss), "NiceMiss.UI.modifierUI.bsml", this);
            selectedIndex = -1;

            hitscoreModal.EntryAdded += OnEntryAdded;
            PluginConfig.Instance.ConfigChanged += UpdateTable;
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab(nameof(NiceMiss));
            hitscoreModal.EntryAdded -= OnEntryAdded;
            PluginConfig.Instance.ConfigChanged -= UpdateTable;
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), multiplierSlider, 0.05f);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), widthSlider, 0.1f);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);

            UpdateTable();
        }

        private void UpdateTable()
        {
            customListTableData.tableView.ClearSelection();
            selectedIndex = -1;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(entrySelected)));

            customListTableData.data.Clear();
            foreach (var hitscoreColor in PluginConfig.Instance.HitscoreColors)
            {
                string colorString = $"#{ColorUtility.ToHtmlStringRGB(hitscoreColor.color)}";
                if (hitscoreColor.type == HitscoreColor.TypeEnum.Miss)
                {
                    customListTableData.data.Add(new CustomListTableData.CustomCellInfo($"Miss (<color={colorString}>{colorString}</color>)"));
                }
                else
                {
                    customListTableData.data.Add(new CustomListTableData.CustomCellInfo($"{hitscoreColor.type}: {hitscoreColor.min}-{hitscoreColor.max} (<color={colorString}>{colorString}</color>)"));
                }
            }
            customListTableData.tableView.ReloadDataKeepingPosition();
        }

        [UIAction("addEntry")]
        private void AddEntry() => hitscoreModal.ShowModal(rootTransform);

        private void OnEntryAdded(HitscoreColor entryToAdd)
        {
            if (entryToAdd.type == HitscoreColor.TypeEnum.Miss)
            {
                entryToAdd.max = 0;
            }

            int duplicateEntryIndex = PluginConfig.Instance.HitscoreColors.FindIndex(x => x.max == entryToAdd.max && x.type == entryToAdd.type);
            if (duplicateEntryIndex != -1)
            {
                PluginConfig.Instance.HitscoreColors[duplicateEntryIndex] = entryToAdd;
            }
            else
            {
                PluginConfig.Instance.HitscoreColors.Add(entryToAdd);
            }

            PluginConfig.Instance.Changed();
        }

        private int selectedIndex;

        [UIValue("entrySelected")]
        private bool entrySelected => selectedIndex >= 0;

        [UIAction("hitscoreSelect")]
        private void HitscoreSelect(TableView _, int selectedIndex)
        {
            this.selectedIndex = selectedIndex;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(entrySelected)));
        }

        [UIAction("moveEntryUp")]
        private void MoveEntryUp()
        {
            if (selectedIndex > 0)
            {
                int tmpIndex = selectedIndex - 1;
                HitscoreColor tmp = PluginConfig.Instance.HitscoreColors[tmpIndex];
                PluginConfig.Instance.HitscoreColors[tmpIndex] = PluginConfig.Instance.HitscoreColors[selectedIndex];
                PluginConfig.Instance.HitscoreColors[selectedIndex] = tmp;
                UpdateTable();
                customListTableData.tableView.SelectCellWithIdx(tmpIndex);
                HitscoreSelect(customListTableData.tableView, tmpIndex);
            }
        }

        [UIAction("moveEntryDown")]
        private void MoveEntryDown()
        {
            if (selectedIndex < PluginConfig.Instance.HitscoreColors.Count - 1)
            {
                int tmpIndex = selectedIndex + 1;
                HitscoreColor tmp = PluginConfig.Instance.HitscoreColors[tmpIndex];
                PluginConfig.Instance.HitscoreColors[tmpIndex] = PluginConfig.Instance.HitscoreColors[selectedIndex];
                PluginConfig.Instance.HitscoreColors[selectedIndex] = tmp;
                UpdateTable();
                customListTableData.tableView.SelectCellWithIdx(tmpIndex);
                HitscoreSelect(customListTableData.tableView, tmpIndex);
            }
        }

        [UIAction("removeEntry")]
        private void RemoveEntry()
        {
            PluginConfig.Instance.HitscoreColors.RemoveAt(selectedIndex);
            UpdateTable();
        }

        [UIAction("modeFormatter")]
        private string ModeFormatter(int modeNum) => ((PluginConfig.ModeEnum)modeNum).ToString();

        [UIValue("enabled")]
        private bool modEnabled
        {
            get => PluginConfig.Instance.Enabled;
            set
            {
                PluginConfig.Instance.Enabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(modEnabled)));
            }
        }

        [UIValue("mode")]
        private int mode
        {
            get => (int)PluginConfig.Instance.Mode;
            set
            {
                PluginConfig.Instance.Mode = (PluginConfig.ModeEnum)value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(mode)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(useMultiplier)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(useOutline)));
            }
        }

        [UIValue("useMultiplier")]
        private bool useMultiplier => mode == 0;

        [UIValue("useOutline")]
        private bool useOutline => mode == 1;

        [UIValue("colorMultiplier")]
        private float colorMultiplier
        {
            get => PluginConfig.Instance.ColorMultiplier;
            set
            {
                PluginConfig.Instance.ColorMultiplier = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(colorMultiplier)));
            }
        }

        [UIValue("outlineWidth")]
        private float outlineWidth
        {
            get => PluginConfig.Instance.OutlineWidth;
            set
            {
                PluginConfig.Instance.OutlineWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(outlineWidth)));
            }
        }
    }
}
