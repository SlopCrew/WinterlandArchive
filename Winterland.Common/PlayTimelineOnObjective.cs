using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;
using UnityEngine.Playables;
using UnityEngine.Assertions;

namespace Winterland.Common {
    public class PlayTimelineOnObjective : MonoBehaviour {
        public WinterObjective RequiredObjective = null;
        public WinterObjective ObjectiveToSet = null;
        public PlayableDirector Director = null;
        public string Music = "MusicTrack_Hwbouths";
        private void Awake() {
            StageManager.OnStagePostInitialization += OnPostInitialization;
        }

        private void OnPostInitialization() {
            StageManager.OnStagePostInitialization -= OnPostInitialization;
            var localProgress = WinterProgress.Instance.LocalProgress;
            if (localProgress.Objective.name == RequiredObjective.name) {
                localProgress.Objective = ObjectiveToSet;
                localProgress.Save();
                if (!string.IsNullOrEmpty(Music)) {
                    var track = Core.Instance.Assets.LoadAssetFromBundle<MusicTrack>("coreassets", Music);
                    if (track != null) {
                        var player = Core.Instance.BaseModule.StageManager.musicPlayer;
                        player.AddAndBufferAsCurrentTrack(track);
                        player.AttemptStartCurrentTrack();
                    }
                }
                SequenceHandler.instance.StartEnteringSequence(Director, null, true, true, true, true, true, null, true, false, null, false);
            }
        }

        private void OnDestroy() {
            StageManager.OnStagePostInitialization -= OnPostInitialization;
        }
    }
}
