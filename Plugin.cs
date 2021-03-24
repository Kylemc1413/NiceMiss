using IPA;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace NiceMiss
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }
        public static bool modActive = false;
        public static List<NoteData> currentMapMisses = new List<NoteData>();
        public static List<NoteData> previousNoteMisses = new List<NoteData>();
        public static string lastLevelID = "";
        [Init]
        public Plugin(IPALogger logger)
        {
            Instance = this;
            log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            var harmony = new Harmony("net.kyle1413.nicemiss");
            harmony.PatchAll();
            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("NiceMiss", "NiceMiss.UI.modifierUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Solo);
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;
            BS_Utils.Utilities.BSEvents.noteWasMissed += BSEvents_noteWasMissed;
            BS_Utils.Utilities.BSEvents.noteWasCut += BSEvents_noteWasCut;
        }

        private void BSEvents_noteWasCut(NoteData arg1, NoteCutInfo arg2, int arg3)
        {
            if (!modActive) return;
            if(arg1.colorType != ColorType.None && !arg2.allIsOK)
                previousNoteMisses.Add(arg1);
        }

        private void BSEvents_noteWasMissed(NoteData arg1, int arg2)
        {
            if (!modActive) return;
            previousNoteMisses.Add(arg1);

        }

        private void BSEvents_gameSceneLoaded()
        {
            if (Config.enabled && BS_Utils.Plugin.LevelData.IsSet && BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Standard)
                modActive = true;
            else
            {
                modActive = false;
                currentMapMisses.Clear();
                previousNoteMisses.Clear();
                lastLevelID = "";
                return;
            }
            string levelID = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID;
            if (levelID == lastLevelID)
            {
                currentMapMisses.Clear();
                currentMapMisses.AddRange(previousNoteMisses);
                previousNoteMisses.Clear();

            }
            else
            {
                currentMapMisses.Clear();
                previousNoteMisses.Clear();
                lastLevelID = levelID;
            }
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }

    }
}
