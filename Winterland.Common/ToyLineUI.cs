using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Winterland.Common {
    public class ToyLineUI : MonoBehaviour {
        public bool Visible {
            get {
                return gameObject.activeSelf;
            }

            set {
                if (value && !gameObject.activeSelf)
                    gameObject.SetActive(true);
                else if (!value && gameObject.activeSelf)
                    gameObject.SetActive(false);
            }
        }

        public void SetCounter(int currentAmount, int maxAmount) {
            toyLineLabel.SetCounter(currentAmount, maxAmount);
        }

        public void SetToy(Toys toy) {
            switch (toy) {
                case Toys.Inferno:
                    toyPartImage.sprite = InfernoIcon;
                    break;
                case Toys.Fuguman:
                    toyPartImage.sprite = FugumanIcon;
                    break;
                case Toys.Bullied:
                    toyPartImage.sprite = BulliedIcon;
                    break;
                case Toys.Brain:
                    toyPartImage.sprite = BrainIcon;
                    break;
                case Toys.Cars:
                    toyPartImage.sprite = CarsIcon;
                    break;
                case Toys.Petemeat:
                    toyPartImage.sprite = PetemeatIcon;
                    break;
                case Toys.Polo:
                    toyPartImage.sprite = PoloIcon;
                    break;
                default:
                case Toys.Spacey:
                    toyPartImage.sprite = SpaceyIcon;
                    break;

            }
        }

        private void Awake() {
            Visible = false;
        }

        [SerializeField]
        private ToyLineLabel toyLineLabel = null;
        [SerializeField]
        private Image toyPartImage = null;
        [Header("Icons")]
        public Sprite BrainIcon = null;
        public Sprite BulliedIcon = null;
        public Sprite CarsIcon = null;
        public Sprite FugumanIcon = null;
        public Sprite InfernoIcon = null;
        public Sprite PetemeatIcon = null;
        public Sprite PoloIcon = null;
        public Sprite SpaceyIcon = null;
    }
}
