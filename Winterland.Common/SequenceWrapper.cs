using CommonAPI;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterland.Common {
    public class SequenceWrapper : CustomSequence {
        public Sequence Sequence;
        public CustomNPC NPC;
        public SequenceAction CurrentAction;
        public string CurrentActionToRunOnEnd;

        public SequenceWrapper(Sequence sequence) {
            Sequence = sequence;
        }

        public override void Play() {
            base.Play();

            if (NPC != null) {
                NPC.CurrentDialogueLevel += Sequence.DialogueLevelToAdd;
                WinterProgress.Instance.LocalProgress.SetNPCDirty(NPC);
                WinterProgress.Instance.LocalProgress.Save();
            }

            CurrentActionToRunOnEnd = Sequence.RunActionOnEnd;

            if (Sequence.StandDownEnemies)
                WantedManager.instance.StandDownEnemies();

            var actions = Sequence.GetActions();
            if (actions.Length > 0)
                actions[0].Run(false);
        }

        public override void Stop() {
            base.Stop();
            CurrentAction?.Stop();
            CurrentAction = null;
            if (Sequence.SetObjectiveOnEnd != null) {
                WinterProgress.Instance.LocalProgress.Objective = Sequence.SetObjectiveOnEnd;
                WinterProgress.Instance.LocalProgress.Save();
            }

            if (!string.IsNullOrEmpty(CurrentActionToRunOnEnd)) {
                var action = Sequence.GetActionByName(CurrentActionToRunOnEnd);
                if (action != null) {
                    action.Run(true);
                }
            }
        }
    }
}
