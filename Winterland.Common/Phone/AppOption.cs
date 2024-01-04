using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Winterland.Common.Phone {
    public class AppOption {
        public Sprite IconSelected;
        public Sprite IconUnselected;
        public string Label;
        public Action OnClicked;

        public AppOption(string title, Sprite iconSelected = null, Sprite iconUnselected = null) {
            Label = title;
            IconSelected = iconSelected;
            IconUnselected = iconUnselected;
        }
    }
}
