using IPA;
using IPALogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using SiraUtil.Zenject;
using NiceMiss.Installers;
using IPA.Config;
using IPA.Config.Stores;

namespace NiceMiss
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            log = logger;
            zenjector.OnMenu<NiceMissMenuInstaller>();
            zenjector.OnGame<NiceMissGameInstaller>(false);
        }

        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Plugin.log?.Debug("Config loaded");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            var harmony = new Harmony("net.kyle1413.nicemiss");
            harmony.PatchAll();            
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

        [OnExit]
        public void OnApplicationQuit()
        {

        }
    }
}
