using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Winterland.Common.Phone {
    [CreateAssetMenu(menuName = "Winterland/Phone Resources")]
    public class PhoneResources : ScriptableObject {
        [Header("General")]
        public Sprite AppIcon = null;
        public GameObject Button = null;
        [Header("Time Of Day")]
        public Sprite TimeOfDaySpriteUnselected = null;
        public Sprite TimeOfDaySpriteSelected = null;
        [Header("Reset Toys")]
        public Sprite ResetToysSpriteUnselected = null;
        public Sprite ResetToysSpriteSelected = null;
    }
}
