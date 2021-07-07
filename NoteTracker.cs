using System;
using System.Collections.Generic;
using Zenject;

namespace NiceMiss
{
    public class NoteTracker : IInitializable, IDisposable, ISaberSwingRatingCounterDidChangeReceiver, ISaberSwingRatingCounterDidFinishReceiver
    {
        internal static Dictionary<IDifficultyBeatmap, Dictionary<NoteData, Rating>> mapData = new Dictionary<IDifficultyBeatmap, Dictionary<NoteData, Rating>>();

        private readonly IDifficultyBeatmap difficultyBeatmap;
        private readonly ScoreController scoreController;
        private Dictionary<NoteData, Rating> currentMapData;
        private Dictionary<ISaberSwingRatingCounter, NoteCutInfo> swingCounterCutInfo;
        private Dictionary<NoteCutInfo, NoteData> noteCutInfoData;

        public NoteTracker(IDifficultyBeatmap difficultyBeatmap, ScoreController scoreController)
        {
            this.difficultyBeatmap = difficultyBeatmap;
            this.scoreController = scoreController;
        }

        public void Initialize()
        {
            scoreController.noteWasMissedEvent += ScoreController_noteWasMissedEvent;
            scoreController.noteWasCutEvent += ScoreController_noteWasCutEvent;
            currentMapData = new Dictionary<NoteData, Rating>();
            swingCounterCutInfo = new Dictionary<ISaberSwingRatingCounter, NoteCutInfo>();
            noteCutInfoData = new Dictionary<NoteCutInfo, NoteData>();
        }

        public void Dispose()
        {
            scoreController.noteWasMissedEvent -= ScoreController_noteWasMissedEvent;
            scoreController.noteWasCutEvent -= ScoreController_noteWasCutEvent;
            mapData[difficultyBeatmap] = currentMapData;
            Plugin.log.Debug(currentMapData.ToString());
        }

        private void ScoreController_noteWasMissedEvent(NoteData noteData, int _)
        {
            currentMapData[noteData] = new Rating(0, true);
        }

        private void ScoreController_noteWasCutEvent(NoteData noteData, in NoteCutInfo noteCutInfo, int multiplier)
        {
            if (noteData.colorType != ColorType.None && !noteCutInfo.allIsOK)
            {
                currentMapData[noteData] = new Rating(0, true);
            }
            else
            {
                swingCounterCutInfo.Add(noteCutInfo.swingRatingCounter, noteCutInfo);
                noteCutInfoData.Add(noteCutInfo, noteData);
                noteCutInfo.swingRatingCounter.RegisterDidChangeReceiver(this);
                noteCutInfo.swingRatingCounter.RegisterDidFinishReceiver(this);
                int beforeCutRawScore, afterCutRawScore, cutDistanceRawScore;
                ScoreModel.RawScoreWithoutMultiplier(noteCutInfo.swingRatingCounter, noteCutInfo.cutDistanceToCenter, out beforeCutRawScore, out afterCutRawScore, out cutDistanceRawScore);
                int totalScore = beforeCutRawScore + afterCutRawScore + cutDistanceRawScore;
                currentMapData[noteData] = new Rating(totalScore, false);
            }
        }

        public void HandleSaberSwingRatingCounterDidChange(ISaberSwingRatingCounter saberSwingRatingCounter, float rating)
        {
            int beforeCutRawScore, afterCutRawScore, cutDistanceRawScore;
            NoteCutInfo noteCutInfo;
            if (swingCounterCutInfo.TryGetValue(saberSwingRatingCounter, out noteCutInfo))
            {
                ScoreModel.RawScoreWithoutMultiplier(saberSwingRatingCounter, noteCutInfo.cutDistanceToCenter, out beforeCutRawScore, out afterCutRawScore, out cutDistanceRawScore);
                int totalScore = beforeCutRawScore + afterCutRawScore + cutDistanceRawScore;
                NoteData noteData;
                if (noteCutInfoData.TryGetValue(noteCutInfo, out noteData))
                {
                    currentMapData[noteData] = new Rating(totalScore, false);
                }
            }
        }

        public void HandleSaberSwingRatingCounterDidFinish(ISaberSwingRatingCounter saberSwingRatingCounter)
        {
            swingCounterCutInfo.Remove(saberSwingRatingCounter);
            saberSwingRatingCounter.UnregisterDidChangeReceiver(this);
            saberSwingRatingCounter.UnregisterDidFinishReceiver(this);
        }

        public struct Rating
        {
            public Rating(int hitScore, bool missed)
            {
                this.hitScore = hitScore;
                this.missed = missed;
            }

            public int hitScore;
            public bool missed;
        }
    }
}
