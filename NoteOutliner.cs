using NiceMiss.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace NiceMiss
{
    internal class NoteOutliner : IInitializable, IDisposable
    {
        private readonly BeatmapObjectManager objectmanager;
        private HitscoreColor missColor;

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
            missColor = PluginConfig.Instance.Mode == PluginConfig.ModeEnum.Outline ? PluginConfig.Instance.HitscoreColors.Find(x => x.type == HitscoreColor.TypeEnum.Miss) : null;
        }

        public void Dispose()
        {
            objectmanager.noteDidStartJumpEvent -= Objectmanager_noteDidStartJumpEvent;
            objectmanager.noteWasCutEvent -= Objectmanager_noteWasCutEvent;
            objectmanager.noteWasMissedEvent -= Objectmanager_noteWasMissedEvent;
        }

        private void Objectmanager_noteDidStartJumpEvent(NoteController obj)
        {
            var outline = obj.gameObject.GetComponentInChildren<Outline>();
            if (outline == null)
            {
                Plugin.log.Debug("No Outline");
                return;
            }
            var noteRating = NiceMissManager.currentMapData.Where(x => ColorNoteVisualsHandleNoteControllerDidInitEvent.NotesEqual(x.Key, obj.noteData)).FirstOrDefault();
            if (!noteRating.Equals(default(KeyValuePair<NoteData, NoteTracker.Rating>)))
            {
                Color newC = Color.clear;
                if (PluginConfig.Instance.Mode == PluginConfig.ModeEnum.Multiplier && noteRating.Value.missed)
                {
                    newC = outline.OutlineColor * PluginConfig.Instance.ColorMultiplier;
                }
                else if (PluginConfig.Instance.Mode == PluginConfig.ModeEnum.Outline)
                {
                    if (noteRating.Value.missed && missColor != null)
                    {
                        newC = missColor.color;
                    }
                    else
                    {
                        foreach (var hitscoreColor in PluginConfig.Instance.HitscoreColors)
                        {
                            int score = -1;

                            switch (hitscoreColor.type)
                            {
                                case HitscoreColor.TypeEnum.Hitscore:
                                    score = noteRating.Value.hitScore;
                                    break;
                                case HitscoreColor.TypeEnum.Angle:
                                    score = noteRating.Value.angle;
                                    break;
                                case HitscoreColor.TypeEnum.Accuracy:
                                    score = noteRating.Value.accuracy;
                                    break;
                            }

                            if (hitscoreColor.min <= score && score <= hitscoreColor.max)
                            {
                                newC = hitscoreColor.color;
                                break;
                            }
                        }
                    }
                }
                if (newC != Color.clear)
                {
                    outline.OutlineColor = newC;
                    outline.enabled = true;
                }
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
