#if !UNITY_EDITOR
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
            var resources = WinterAssets.Instance.PhoneResources;
            m_Unlockables = Array.Empty<AUnlockable>();
            var overlay = AppUtility.CreateAppOverlay("Winterland", resources.AppIcon);
            overlay.SetParent(transform, false);
        }
    }
}
#endif
