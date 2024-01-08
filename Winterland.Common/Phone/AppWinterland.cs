#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonAPI.Phone;
using Reptile;

namespace Winterland.Common.Phone {
    public class AppWinterland : CustomApp {
        public override bool Available => Core.Instance.BaseModule.CurrentStage == Stage.square;
        public override void OnAppInit() {
            base.OnAppInit();
            var resources = WinterAssets.Instance.PhoneResources;
            CreateTitleBar("Winterland", resources.AppIcon);
            ScrollView = PhoneScrollView.Create(this);

            var button = PhoneUIUtility.CreateSimpleButton("Switch Time of Day");
            button.OnConfirm = SwitchTimeOfDay;
            ScrollView.AddButton(button);

            button = PhoneUIUtility.CreateSimpleButton("Restart Tree Construction");
            button.OnConfirm = RestartTree;
            ScrollView.AddButton(button);

            button = PhoneUIUtility.CreateSimpleButton("Finish Tree Construction");
            button.OnConfirm = FinishTree;
            ScrollView.AddButton(button);
        }

        private void WarnTooCloseToTree() {
            Core.Instance.UIManager.ShowNotification("You're too close to the tree - please back up a little and try again.");
        }

        private void RestartTree() {
            if (TreeController.Instance.ConstructionBlockers.Count > 0) {
                WarnTooCloseToTree();
                return;
            }
            ResetToys();
            SetTreePhase(0);
        }

        private void FinishTree() {
            if (TreeController.Instance.ConstructionBlockers.Count > 0) {
                WarnTooCloseToTree();
                return;
            }
            ResetToys();
            SetTreePhase(SingleplayerPhases.GetLastPhase());
        }

        private void SetTreePhase(int phase) {
            var localProgress = WinterProgress.Instance.LocalProgress;
            localProgress.CurrentPhase = phase;
            localProgress.CurrentPhaseGifts = 0;
            localProgress.UpdateTree(true);
            localProgress.Save();
        }

        private void ResetToys() {
            var localPlayer = WinterPlayer.GetLocal();
            localPlayer.DropCurrentToyLine();
            ToyLineManager.Instance.RespawnAllToyLines();
        }

        private void SwitchTimeOfDay() {
            var timeOfDay = TimeOfDayController.Instance;
            if (timeOfDay == null) return;
            var targetTimeOfDay = TimeOfDayController.TimesOfDay.Night;
            if (timeOfDay.CurrentTimeOfDay == TimeOfDayController.TimesOfDay.Night)
                targetTimeOfDay = TimeOfDayController.TimesOfDay.Day;
            timeOfDay.TransitionToTimeOfDay(targetTimeOfDay);
        }
    }
}
#endif
