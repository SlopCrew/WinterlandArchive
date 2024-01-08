using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlopCrew.Server.XmasEvent;

namespace Winterland.Common {
    /// <summary>
    /// Local global progress.
    /// </summary>
    public class SingleplayerGlobalProgress : WritableGlobalProgress {
        public SingleplayerGlobalProgress() {
            var defaultState = new XmasServerEventStatePacket();
            var phase = new XmasPhase();
            phase.Active = true;
            defaultState.Phases.Add(phase);
            SetState(defaultState);
        }
    }
}
