using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterland.Common {
    public class WinterProgress {
        public static WinterProgress Instance { get; private set; }
        public ILocalProgress LocalProgress = null;
        public IGlobalProgress GlobalProgress => WritableGlobalProgress;
        public WritableGlobalProgress WritableGlobalProgress = null;
        public WinterProgress() {
            Instance = this;
            var localProgress = new LocalProgress();
            LocalProgress = localProgress;


#if WINTER_DEBUG
            if (!WinterConfig.Instance.ResetLocalSaveValue)
#endif
                LocalProgress.Load();
            WritableGlobalProgress = new SingleplayerGlobalProgress();
            localProgress.MakeState();
        }
    }
}
