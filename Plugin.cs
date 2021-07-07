using IPA;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IPA.Utilities;
using SiraUtil.Zenject;
using NiceMiss.Installers;

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
      //  internal static ColorManager colorManager;
        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            log = logger;
            zenjector.OnMenu<NiceMissMenuInstaller>();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            var harmony = new Harmony("net.kyle1413.nicemiss");
            harmony.PatchAll();
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += BSEvents_gameSceneLoaded;
            
            BS_Utils.Utilities.BSEvents.noteWasMissed += BSEvents_noteWasMissed;
            BS_Utils.Utilities.BSEvents.noteWasCut += BSEvents_noteWasCut;
           SharedCoroutineStarter.instance.StartCoroutine(LoadQuickOutlineMaterials());
        }

        public IEnumerator LoadQuickOutlineMaterials()
        {
            var quickOutlineBundleRequest = AssetBundle.LoadFromStreamAsync(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("NiceMiss.QuickOutline.Resources.outlineBundle"));
            yield return quickOutlineBundleRequest;
            var quickOutlineBundle = quickOutlineBundleRequest.assetBundle;
            if (quickOutlineBundle == null)
            {
                Plugin.log.Error("Failed To load QuickOutline Bundle");
                yield break;
            }
            var fillMatRequest = quickOutlineBundle.LoadAssetAsync<Material>("OutlineFill");
            yield return fillMatRequest;
            Outline.outlineFillMaterialSource = fillMatRequest.asset as Material;
            var maskMatRequest = quickOutlineBundle.LoadAssetAsync<Material>("OutlineMask");
            yield return maskMatRequest;
            Outline.outlineMaskMaterialSource = maskMatRequest.asset as Material;
            Plugin.log.Debug("Loaded QuickOutline Material Assets");
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
            if(modActive)
            {
                var objectmanager = Resources.FindObjectsOfTypeAll<BeatEffectSpawner>().LastOrDefault().GetField<BeatmapObjectManager, BeatEffectSpawner>("_beatmapObjectManager");
        //        colorManager = Resources.FindObjectsOfTypeAll<NoteCutCoreEffectsSpawner>().LastOrDefault().GetField<ColorManager, NoteCutCoreEffectsSpawner>("_colorManager");
                objectmanager.noteDidStartJumpEvent += Objectmanager_noteDidStartJumpEvent;
                objectmanager.noteWasCutEvent += Objectmanager_noteWasCutEvent;
                objectmanager.noteWasMissedEvent += Objectmanager_noteWasMissedEvent;
            }
        }
        
        private void Objectmanager_noteWasMissedEvent(NoteController obj)
        {
            var outline = obj.gameObject.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }

        private void Objectmanager_noteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            var outline = noteController.gameObject.GetComponentInChildren<Outline>();
            if (outline != null)
                outline.enabled = false;
        }

        private void Objectmanager_noteDidStartJumpEvent(NoteController obj)
        {
        //    Color c = colorManager.ColorForType(obj.noteData.colorType);
            var outline = obj.gameObject.GetComponentInChildren<Outline>();
            if (outline == null)
            {
                log.Debug("No Outline");
                return;
            }
            //   Plugin.log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(____noteController.noteData));
            if (Plugin.currentMapMisses.Any(x => ColorNoteVisualsHandleNoteControllerDidInitEvent.NotesEqual(x, obj.noteData)))
            {
                //Plugin.log.Debug($"Coloring Miss");
                Color newC = Config.useMultiplier ? outline.OutlineColor * Config.colorMultiplier :
                obj.noteData.colorType == ColorType.ColorA ? Config.leftMissColor : Config.rightMissColor;
                outline.OutlineColor = newC;
                outline.enabled = true;
                //   SetNoteColour(__instance, newC);
                //   var colorable = ____noteController.gameObject.GetComponent<IColorable>();
                //   if (colorable != null)
                //       colorable.SetColor(newC);
            }
            else
            {

            }
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }

    }
}
