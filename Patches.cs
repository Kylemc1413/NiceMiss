using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Collections;
using UnityEngine;
using IPA;
using BS_Utils.Utilities;
using SiraUtil.Interfaces;
namespace NiceMiss
{
    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(ColorNoteVisuals))]
    [HarmonyPatch("HandleNoteControllerDidInit")]
    public class ColorNoteVisualsHandleNoteControllerDidInitEvent
    {
        static readonly int colorID = Shader.PropertyToID("_Color");
        static void Postfix(ColorNoteVisuals __instance, NoteController ____noteController, MaterialPropertyBlockController[] ____materialPropertyBlockControllers, int ____colorId, ref ColorManager ____colorManager)
        {
            if (!Plugin.modActive) return;
            //    Debug.Log("ColorNoteVis Init");
            var outline = ____noteController.gameObject.GetComponentInChildren<Outline>();
            if (outline == null)
            {
          //      Debug.Log("Outline Not Found");
                outline = ____noteController.gameObject.AddComponent<Outline>();
            }

            Color c = ____colorManager.ColorForType(____noteController.noteData.colorType);
            outline.CheckRenderersValidity();
            outline.enabled = false;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = c;
            outline.OutlineWidth = 4f;
            return;
                
            //   Plugin.log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(____noteController.noteData));
            if (Plugin.currentMapMisses.Any(x => NotesEqual(x, ____noteController.noteData)))
            {
                //Plugin.log.Debug($"Coloring Miss");
                Color newC = Config.useMultiplier? c * Config.colorMultiplier :
                    ____noteController.noteData.colorType == ColorType.ColorA? Config.leftMissColor : Config.rightMissColor;
                outline.OutlineColor = newC;
                outline.enabled = true;
             //   SetNoteColour(__instance, newC);
             //   var colorable = ____noteController.gameObject.GetComponent<IColorable>();
             //   if (colorable != null)
             //       colorable.SetColor(newC);
            }
            else
            {
          //      Plugin.log.Debug($"No Matching Miss");
            }

        }

        public static void SetNoteColour(ColorNoteVisuals noteVis, Color c)
        {
            noteVis.SetField("_noteColor", c);
            SpriteRenderer ____arrowGlowSpriteRenderer = noteVis.GetField<SpriteRenderer>("_arrowGlowSpriteRenderer");
            SpriteRenderer ____circleGlowSpriteRenderer = noteVis.GetField<SpriteRenderer>("_circleGlowSpriteRenderer");
            MaterialPropertyBlockController[] ____materialPropertyBlockController = noteVis.GetField<MaterialPropertyBlockController[]>("_materialPropertyBlockControllers");
            if (____arrowGlowSpriteRenderer != null) ____arrowGlowSpriteRenderer.color = c;
            if (____circleGlowSpriteRenderer != null) ____circleGlowSpriteRenderer.color = c;
            foreach (var block in ____materialPropertyBlockController)
            {
                block.materialPropertyBlock.SetColor(colorID, c);
                block.ApplyChanges();
            }

        }

        public static bool NotesEqual(NoteData one, NoteData other)
        {
            return one.colorType == other.colorType && one.cutDirection == other.cutDirection && one.flipYSide == other.flipYSide
                && one.flipLineIndex == other.flipLineIndex && one.lineIndex == other.lineIndex && one.noteLineLayer == other.noteLineLayer
                && one.time == other.time;
        }
    }
}
