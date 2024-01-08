using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CommonAPI;
using Reptile;
using System.Collections;

namespace Winterland.Common {
    public class RespawnToyLinesSequenceAction : SequenceAction {
        [Header("Name of action to jump to if the gift collection is successful.")]
        public string SuccessTarget = "";
        [Header("Name of action to jump to if SlopCrew rejects collection.")]
        public string RejectedTarget = "";
        [Header("Name of action to jump to if can't reach SlopCrew")]
        public string NoConnectionTarget = "";
        private bool immediate;
        private const float TimeOutSeconds = 5f;

#if !UNITY_EDITOR
        public override void Run(bool immediate) {
            base.Run(immediate);
            this.immediate = immediate;
            RunSuccess(immediate);
        }

        private void RunSuccess(bool immediate) {
            StopAllCoroutines();
            if (Sequence.Sequence.Skippable)
                CustomSequenceHandler.instance.skipTextActiveState = SequenceHandler.SkipState.IDLE;
            var progress = WinterProgress.Instance.LocalProgress;
            ToyLineManager.Instance.RespawnAllToyLines();
            progress.Gifts++;
            progress.CurrentPhaseGifts++;
            progress.Save();
            GiftPileManager.Instance.UpdatePiles();
            var action = Sequence.Sequence.GetActionByName(SuccessTarget);
            if (action != null)
                action.Run(immediate);
            else
                Finish(immediate);
        }
#endif
    }
}
