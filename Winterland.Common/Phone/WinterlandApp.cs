using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using Reptile.Phone;

namespace Winterland.Common.Phone {
    public class WinterlandApp : App {
        public override void OnAppInit() {
            m_Unlockables = Array.Empty<AUnlockable>();
            var overlay = AppUtility.CreateAppOverlay("Winterland", null);
            overlay.SetParent(transform, false);
        }
    }
}
