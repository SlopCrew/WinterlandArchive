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
#if UNITY_EDITOR
public class PlayTimelineOnObjective : MonoBehaviour {
#else
    public class PlayTimelineOnObjective : GameplayEvent {
#endif
        public WinterObjective RequiredObjective = null;
        public WinterObjective ObjectiveToSet = null;
        public PlayableDirector Director = null;
        public string Music = "MusicTrack_Hwbouths";

#if !UNITY_EDITOR
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
                SequenceHandler.instance.StartEnteringSequence(Director, this, true, true, true, true, true, null, true, false, null, false);
            }
            else
                ShowNewStuffNotification();
        }

        public override void Awake() {
            StageManager.OnStagePostInitialization += OnPostInitialization;
        }

        public override void ExitSequence(PlayableDirector sequence, bool invokeEvent = true, bool restartCurrentEncounter = false) {
            base.ExitSequence(sequence, invokeEvent, restartCurrentEncounter);
            ShowNewStuffNotification();
        }

        private void ShowNewStuffNotification() {
            var localProgress = WinterProgress.Instance.LocalProgress;
            if (localProgress.SingleplayerUpdateNew) {
                Core.Instance.UIManager.ShowNotification("Check out the new Winterland app on your phone to set things up to your liking!");
                localProgress.SingleplayerUpdateNew = false;
                localProgress.Save();
            }
        }

        public override void OnDestroy() {
            StageManager.OnStagePostInitialization -= OnPostInitialization;
        }
#endif
    }
}
