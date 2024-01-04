using Reptile;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Winterland.Common.Phone {
    public class AppButton : MonoBehaviour {
        [Header("Animation")]
        public float AnimationSpeed = 5f;
        public float SelectOffset = 50f;
        public float PressOffset = 10f;
        [Header("Components")]
        public Image Icon;
        public Image ButtonImage;
        public TextMeshProUGUI Label;
        public GameObject RightIndicator;
        [Header("Colors")]
        public Color LabelColorSelected;
        public Color LabelColorUnselected;
        [Header("Sprites")]
        public Sprite ButtonSelected;
        public Sprite ButtonUnselected;
        private Sprite iconSelected;
        private Sprite iconUnselected;
        private bool isSelected = false;
        private float defaultLocalX = 0f;

        public void SetPosition(Vector3 localPosition) {
            transform.localPosition = localPosition;
            defaultLocalX = localPosition.x;
        }

        public void SetOption(AppOption option) {
            Label.text = option.Label;
            iconSelected = option.IconSelected;
            iconUnselected = option.IconUnselected;
            SetSelected(isSelected);
        }

        public void SetSelected(bool selected) {
            isSelected = selected;
            if (selected) {
                ButtonImage.sprite = ButtonSelected;
                Label.faceColor = LabelColorSelected;
                Icon.sprite = iconSelected;
                RightIndicator.SetActive(true);
            } else {
                ButtonImage.sprite = ButtonUnselected;
                Label.faceColor = LabelColorUnselected;
                Icon.sprite = iconUnselected;
                RightIndicator.SetActive(false);
            }
        }

        public void PlayHighlightAnimation() {
            StopAllCoroutines();
            StartCoroutine(AnimationCoroutine(SelectOffset));
        }

        public void PlayPressAnimation() {
            StopAllCoroutines();
            StartCoroutine(AnimationCoroutine(PressOffset));
        }

        public void PlayResetAnimation() {
            StopAllCoroutines();
            StartCoroutine(AnimationCoroutine(0f));
        }

        public IEnumerator AnimationCoroutine(float offset) {
            var targetLocalX = defaultLocalX + offset;
            while(transform.localPosition.x != targetLocalX) {

                var newPos = transform.localPosition.x;

                if (targetLocalX > transform.localPosition.x) {
                    newPos += AnimationSpeed * Core.dt;
                    if (newPos > targetLocalX)
                        newPos = targetLocalX;
                }
                else if (targetLocalX < transform.localPosition.x) {
                    newPos -= AnimationSpeed * Core.dt;
                    if (newPos < targetLocalX)
                        newPos = targetLocalX;
                }

                transform.localPosition = new Vector3(newPos, transform.localPosition.y, transform.localPosition.z);
                yield return null;
            }
        }
    }
}
