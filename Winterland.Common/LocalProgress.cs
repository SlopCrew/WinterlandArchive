using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine;
using CommonAPI;
using UnityEngine.XR.WSA;
using System.Security.Principal;
using Winterland.Common.Challenge;
using SlopCrew.Server.XmasEvent;

namespace Winterland.Common {
    /// <summary>
    /// Keeps track of clientside progress for Winterland.
    /// </summary>
    public class LocalProgress : ILocalProgress {
        public int CurrentPhaseGifts { get; set; }
        public int CurrentPhase { get; set; }
        public WinterObjective Objective { get; set; }
        public int Gifts { get; set; }
        public int FauxJuggleHighScore { get; set; }
        public int ToyLinesCollected => collectedToyLines.Count;
        public bool ArcadeUnlocked { get; set; }
        public TimeOfDayController.TimesOfDay TimeOfDay { get; set; }
        public bool SingleplayerUpdateNew { get; set; }
        private const byte Version = 6;
        private string savePath;
        private Dictionary<Guid, SerializedNPC> npcs;
        private HashSet<Guid> collectedToyLines;
        private Dictionary<string, float> challengeBestTimes;

        public LocalProgress() {
            InitializeNew();
        }

        public void UpdateTree(bool reset = false) {
            if (TreeController.Instance == null) return;
            var isLastPhase = CurrentPhase >= TreeController.Instance.treePhases.Length - 1;
            if (!isLastPhase) {
                var goal = SingleplayerPhases.GetGoalForPhase(CurrentPhase);
                if (CurrentPhaseGifts >= goal) {
                    CurrentPhase++;
                    CurrentPhaseGifts = 0;
                }
            }
            MakeState();
            if (reset)
                TreeController.Instance.ResetToTarget();
        }

        public void MakeState() {
            var packet = new XmasServerEventStatePacket();
            for(var i = 0; i <= CurrentPhase; i++) {
                var phase = new XmasPhase();
                phase.Active = false;
                if (i == CurrentPhase)
                    phase.Active = true;
                else {
                    phase.GiftsCollected = 1;
                }
                phase.GiftsGoal = 1;
                packet.Phases.Add(phase);
            }
            (WinterProgress.Instance.GlobalProgress as SingleplayerGlobalProgress).SetState(packet);
        }

        // Default values for a blank save!
        public void InitializeNew() {
            npcs = new();
            collectedToyLines = new();
            challengeBestTimes = new();
            Gifts = 0;
            FauxJuggleHighScore = 0;
            ArcadeUnlocked = false;
            Objective = ObjectiveDatabase.StartingObjective;
            savePath = Path.Combine(Paths.ConfigPath, "MilleniumWinterland/localprogress.mwp");
            TimeOfDay = TimeOfDayController.TimesOfDay.Night;
            SingleplayerUpdateNew = true;
            CurrentPhase = 0;
            CurrentPhaseGifts = 0;
        }

        public void Save() {
            using (var stream = new MemoryStream()) {
                using (var writer = new BinaryWriter(stream)) {
                    Write(writer);
                }
                // Enqueue async file write.
                CustomStorage.Instance.WriteFile(stream.ToArray(), savePath);
            }
        }

        public void SetNPCDirty(CustomNPC npc) {
            npcs[npc.GUID] = new SerializedNPC(npc);
        }

        public void SetToyLineCollected(Guid guid, bool collected) {
            if (collected)
                collectedToyLines.Add(guid);
            else
                collectedToyLines.Remove(guid);
        }

        public bool IsToyLineCollected(Guid guid) {
            return collectedToyLines.Contains(guid);
        }

        public SerializedNPC GetNPCProgress(CustomNPC npc) {
            if (!npcs.TryGetValue(npc.GUID, out var progress))
                return null;
            return progress;
        }

        public void Load() {
            if (!File.Exists(savePath)) {
                Debug.Log("No Winterland save to load, starting a new game.");
                return;
            }
            using (var stream = File.Open(savePath, FileMode.Open)) {
                using (var reader = new BinaryReader(stream)) {
                    try {
                        Read(reader);
                    }
                    catch(Exception e) {
                        Debug.LogError($"Failed to load Winterland save!{Environment.NewLine}{e}");
                    }
                }
            }
        }

        private void Write(BinaryWriter writer) {
            //version
            writer.Write(Version);
            writer.Write(Objective.name);
            writer.Write(npcs.Count);
            foreach(var npc in npcs.Values) {
                npc.Write(writer);
            }
            writer.Write(Gifts);
            writer.Write(collectedToyLines.Count);
            foreach(var toyLine in collectedToyLines) {
                writer.Write(toyLine.ToString());
            }
            writer.Write(FauxJuggleHighScore);
            writer.Write(ArcadeUnlocked);
            writer.Write(challengeBestTimes.Count);
            foreach(var challenge in challengeBestTimes) {
                writer.Write(challenge.Key);
                writer.Write(challenge.Value);
            }
            writer.Write((int) TimeOfDay);
            writer.Write(SingleplayerUpdateNew);
            writer.Write(CurrentPhase);
            writer.Write(CurrentPhaseGifts);
        }

        private void Read(BinaryReader reader) {
            npcs.Clear();
            var version = reader.ReadByte();
            if (version > Version) {
                Debug.LogError($"Attemped to read a Winterland save that's too new (version {version}), current version is {Version}.");
                return;
            }
            var objectiveName = reader.ReadString();
            var objective = ObjectiveDatabase.GetObjective(objectiveName);
            if (objective != null)
                Objective = objective;
            else
                Objective = ObjectiveDatabase.StartingObjective;
            if (version > 0) {
                var npcCount = reader.ReadInt32();
                for(var i = 0; i < npcCount; i++) {
                    var npc = new SerializedNPC(reader);
                    if (npc.GUID != Guid.Empty) {
                        npcs[npc.GUID] = npc;
                    }
                }
            }
            if (version > 1) {
                Gifts = reader.ReadInt32();
                var collectedToyLineCount = reader.ReadInt32();
                for(var i = 0; i < collectedToyLineCount; i++) {
                    var toyLineGUID = Guid.Parse(reader.ReadString());
                    SetToyLineCollected(toyLineGUID, true);
                }
            }
            if (version > 2) {
                FauxJuggleHighScore = reader.ReadInt32();
            }
            if (version > 3) {
                ArcadeUnlocked = reader.ReadBoolean();
                var challengeCount = reader.ReadInt32();
                for (var i = 0; i < challengeCount; i++) {
                    var guid = reader.ReadString();
                    challengeBestTimes[guid] = reader.ReadSingle();
                }
            }
            if (version > 4) {
                TimeOfDay = (TimeOfDayController.TimesOfDay)reader.ReadInt32();
                SingleplayerUpdateNew = reader.ReadBoolean();
            }
            if (version > 5) {
                CurrentPhase = reader.ReadInt32();
                CurrentPhaseGifts = reader.ReadInt32();
            }
        }

        public void SetChallengeBestTime(ChallengeLevel challenge, float bestTime) {
            challengeBestTimes[challenge.GUID] = bestTime;
        }

        public float GetChallengeBestTime(ChallengeLevel challenge) {
            if (challengeBestTimes.TryGetValue(challenge.GUID, out var result))
                return result;
            return 0f;
        }
    }
}
