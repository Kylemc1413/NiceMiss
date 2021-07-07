using NiceMiss.Configuration;
using System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace NiceMiss
{
    internal class NoteOutliner : IInitializable, IDisposable
    {
        private readonly BeatmapObjectManager objectmanager;

        public NoteOutliner(BeatmapObjectManager objectmanager)
        {
            this.objectmanager = objectmanager;
        }

        public void Initialize()
        {
            if (NiceMissManager.modActive)
            {
                objectmanager.noteDidStartJumpEvent += Objectmanager_noteDidStartJumpEvent;
                objectmanager.noteWasCutEvent += Objectmanager_noteWasCutEvent;
                objectmanager.noteWasMissedEvent += Objectmanager_noteWasMissedEvent;
            }
        }

        public void Dispose()
        {
            objectmanager.noteDidStartJumpEvent -= Objectmanager_noteDidStartJumpEvent;
            objectmanager.noteWasCutEvent -= Objectmanager_noteWasCutEvent;
            objectmanager.noteWasMissedEvent -= Objectmanager_noteWasMissedEvent;
        }

        private void Objectmanager_noteDidStartJumpEvent(NoteController obj)
        {
            //    Color c = colorManager.ColorForType(obj.noteData.colorType);
            var outline = obj.gameObject.GetComponentInChildren<Outline>();
            if (outline == null)
            {
                Plugin.log.Debug("No Outline");
                return;
            }
            //   Plugin.log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(____noteController.noteData));
            if (NiceMissManager.currentMapData.Any(x => ColorNoteVisualsHandleNoteControllerDidInitEvent.NotesEqual(x.Key, obj.noteData) && x.Value.missed))
            {
                //Plugin.log.Debug($"Coloring Miss");
                Color newC = PluginConfig.Instance.UseMultiplier ? outline.OutlineColor * PluginConfig.Instance.ColorMultiplier :
                obj.noteData.colorType == ColorType.ColorA ? PluginConfig.Instance.LeftMissColor : PluginConfig.Instance.RightMissColor;
                outline.OutlineColor = newC;
                outline.enabled = true;
                //   SetNoteColour(__instance, newC);
                //   var colorable = ____noteController.gameObject.GetComponent<IColorable>();
                //   if (colorable != null)
                //       colorable.SetColor(newC);
            }
        }

        private void Objectmanager_noteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            var outline = noteController.gameObject.GetComponentInChildren<Outline>();
            if (outline != null)
                outline.enabled = false;
        }

        private void Objectmanager_noteWasMissedEvent(NoteController obj)
        {
            var outline = obj.gameObject.GetComponentInChildren<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }
}
