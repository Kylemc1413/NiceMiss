using HarmonyLib;
using UnityEngine;
using NiceMiss.Configuration;
using IPA.Utilities;

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
            if (!NiceMissManager.modActive) return;
            var outline = ____noteController.gameObject.GetComponentInChildren<Outline>();
            if (outline == null)
            {
                outline = ____noteController.gameObject.AddComponent<Outline>();
            }

            Color c = ____colorManager.ColorForType(____noteController.noteData.colorType);
            outline.CheckRenderersValidity();
            outline.enabled = false;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = c;
            outline.OutlineWidth = PluginConfig.Instance.OutlineWidth;
        }

        public static void SetNoteColour(ColorNoteVisuals noteVis, Color c)
        {
            FieldAccessor<ColorNoteVisuals, Color>.Set(ref noteVis, "_noteColor", c);
            SpriteRenderer ____arrowGlowSpriteRenderer = FieldAccessor<ColorNoteVisuals, SpriteRenderer>.Get(ref noteVis, "_arrowGlowSpriteRenderer");
            SpriteRenderer ____circleGlowSpriteRenderer = FieldAccessor<ColorNoteVisuals, SpriteRenderer>.Get(ref noteVis, "_circleGlowSpriteRenderer");
            MaterialPropertyBlockController[] ____materialPropertyBlockController = FieldAccessor<ColorNoteVisuals, MaterialPropertyBlockController[]>.Get(ref noteVis, "_materialPropertyBlockControllers");
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
