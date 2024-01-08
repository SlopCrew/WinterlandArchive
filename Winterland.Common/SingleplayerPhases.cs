using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterland.Common {
    public static class SingleplayerPhases {
        public static int GetGoalForPhase(int phase) {
            return 1;
        }

        public static int GetLastPhase() {
            return TreeController.Instance.treePhases.Length - 1;
        }
    }
}
