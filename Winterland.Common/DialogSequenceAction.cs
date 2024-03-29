using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;
using CommonAPI;
using Reptile;
using UnityEngine;
using Winterland.Common.Challenge;

namespace Winterland.Common {
    public class DialogSequenceAction : CameraSequenceAction {
        public enum DialogType {
            Normal,
            YesNah
        }
        [Header("Type of dialog. Can make a branching question.")]
        public DialogType Type = DialogType.Normal;
        [HideInInspector]
        public string YesTarget = "";
        [HideInInspector]
        public string NahTarget = "";
        [Header("Random clips to play when the character starts talking.")]
        public AudioClip[] AudioClips;
        public override void Run(bool immediate) {
            base.Run(immediate);
            if (immediate)
                return;

            if (AudioClips != null && AudioClips.Length > 0) {
                var audioManager = Core.Instance.AudioManager;
                var clip = AudioClips[UnityEngine.Random.Range(0, AudioClips.Length)];
                audioManager.PlayNonloopingSfx(audioManager.audioSources[5], clip, audioManager.mixerGroups[5], 0f);
            }

            var dialogs = DialogBlock.GetComponentsOrdered<DialogBlock>(gameObject).Where((block) => block.Owner == this).ToArray();
            var customDialogs = new List<CustomDialogue>();
            for(var i=0;i<dialogs.Length;i++) {
                var dialog = dialogs[i];

                var parsedText = dialog.Text;
                var toyLinesLeft = ToyLineManager.Instance.ToyLines.Count - WinterProgress.Instance.LocalProgress.ToyLinesCollected;
                parsedText = parsedText.Replace("$TOYS_LEFT", toyLinesLeft.ToString());
                var playerName = BepInEx.Bootstrap.Chainloader.PluginInfos["SlopCrew.Plugin"].Instance.Config["General", "Username"] as ConfigEntry<string>;
                if (playerName != null)
                    parsedText = parsedText.Replace("$PLAYERNAME", playerName.Value);
                parsedText = parsedText.Replace("$LOCALGIFTS", WinterProgress.Instance.LocalProgress.Gifts.ToString());
                parsedText = parsedText.Replace("$GLOBALGIFTS", TreeController.Instance.TargetProgress.totalGiftsCollected.ToString());
                if (NPC != null) {
                    if (NPC.Challenge != null) {
                        var challengeTime = ChallengeUI.SecondsToMMSS(NPC.Challenge.Timer);
                        var challengeBestTime = ChallengeUI.SecondsToMMSS(NPC.Challenge.BestTime);
                        if (NPC.Challenge.BestTime == 0f)
                            challengeBestTime = "Not set";
                        parsedText = parsedText.Replace("$ARCADE_LASTTIME", challengeTime);
                        parsedText = parsedText.Replace("$ARCADE_BESTTIME", challengeBestTime);
                    }
                }
                var customDialog = new CustomDialogue(dialog.SpeakerName, parsedText);
                customDialogs.Add(customDialog);

                if (dialog.Speaker == DialogBlock.SpeakerMode.None)
                    customDialog.CharacterName = "";
                if (dialog.Speaker == DialogBlock.SpeakerMode.NPC)
                    customDialog.CharacterName = NPC.Name;

                if (i > 0) {
                    var prevDialogue = customDialogs[i - 1];
                    if (prevDialogue != null)
                        prevDialogue.NextDialogue = customDialog;
                }

                customDialog.OnDialogueBegin += () => {
                    if (dialog.AudioClips != null && dialog.AudioClips.Length > 0) {
                        var audioManager = Core.Instance.AudioManager;
                        var clip = dialog.AudioClips[UnityEngine.Random.Range(0, dialog.AudioClips.Length)];
                        audioManager.PlayNonloopingSfx(audioManager.audioSources[5], clip, audioManager.mixerGroups[5], 0f);
                    }
                };

                if (i >= dialogs.Length - 1) {
                    customDialog.OnDialogueBegin += () => {
                        if (Type == DialogType.YesNah)
                            Sequence.RequestYesNoPrompt();
                    };
                    customDialog.OnDialogueEnd += () => {
                        if (Type == DialogType.YesNah) {
                            var yesTarget = Sequence.Sequence.GetActionByName(YesTarget);
                            var noTarget = Sequence.Sequence.GetActionByName(NahTarget);
                            if (customDialog.AnsweredYes) {
                                if (yesTarget == null)
                                    Finish(immediate);
                                else
                                    yesTarget.Run(immediate);
                            }
                            else {
                                if (noTarget == null)
                                    Finish(immediate);
                                else
                                    noTarget.Run(immediate);
                            }
                        }
                        else
                            Finish(immediate);
                    };
                }
            }

            if (customDialogs.Count > 0)
                Sequence.StartDialogue(customDialogs[0]);
        }
    }
}
