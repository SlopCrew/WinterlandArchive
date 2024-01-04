#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using Reptile.Phone;
using UnityEngine;

namespace Winterland.Common.Phone {
    public class WinterlandApp : App {

        private AppOption[] options;
        private AppButton[] buttons;
        private int selectedOption = 0;

        public override void OnAppInit() {
            var resources = WinterAssets.Instance.PhoneResources;
            m_Unlockables = Array.Empty<AUnlockable>();
            var overlay = AppUtility.CreateAppOverlay("Winterland", resources.AppIcon);
            overlay.SetParent(transform, false);

            var timeOfDayOption = new AppOption("Time of Day", resources.TimeOfDaySpriteSelected, resources.TimeOfDaySpriteUnselected);

            var resetToysOption = new AppOption("Reset Toys", resources.ResetToysSpriteSelected, resources.ResetToysSpriteUnselected);
            resetToysOption.OnClicked += () => {
                var localPlayer = WinterPlayer.GetLocal();
                localPlayer.DropCurrentToyLine();
                ToyLineManager.Instance.RespawnAllToyLines();
            };

            options = [
                timeOfDayOption,
                resetToysOption
            ];

            PopulateButtons(resources);

            buttons[selectedOption].PlayHighlightAnimation();
        }

        public override void OnAppEnable() {
            buttons[selectedOption].PlayHighlightAnimation();
        }

        public override void OnPressRight() {
            buttons[selectedOption].PlayPressAnimation();
        }

        public override void OnReleaseRight() {
            buttons[selectedOption].PlayHighlightAnimation();
            m_AudioManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Confirm, 0f);
            options[selectedOption].OnClicked?.Invoke();
        }

        public override void OnPressDown() {
            buttons[selectedOption].SetSelected(false);
            buttons[selectedOption].PlayResetAnimation();
            selectedOption++;
            if (selectedOption >= options.Length)
                selectedOption = options.Length - 1;
            buttons[selectedOption].SetSelected(true);
            buttons[selectedOption].PlayHighlightAnimation();
            m_AudioManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select, 0f);
        }

        public override void OnPressUp() {
            buttons[selectedOption].SetSelected(false);
            buttons[selectedOption].PlayResetAnimation();
            selectedOption--;
            if (selectedOption < 0)
                selectedOption = 0;
            buttons[selectedOption].SetSelected(true);
            buttons[selectedOption].PlayHighlightAnimation();
            m_AudioManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select, 0f);
        }

        private void PopulateButtons(PhoneResources resources) {
            var buttonStartHeight = 450f;
            var buttonRightOffset = 150f;
            var buttonHeight = 350f;
            var appFont = AppUtility.GetAppFont();
            buttons = new AppButton[options.Length];

            for(var i = 0; i < options.Length; i++) {
                var xPosition = buttonRightOffset;
                var yPosition = buttonStartHeight - (buttonHeight * i);
                var button = Instantiate(resources.Button);
                button.transform.SetParent(Content, false);
                var appButton = button.GetComponent<AppButton>();
                appButton.Label.font = appFont;
                appButton.SetOption(options[i]);
                appButton.SetPosition(new Vector3(xPosition, yPosition));
                if (i == selectedOption) {
                    appButton.SetSelected(true);
                } else {
                    appButton.SetSelected(false);
                }
                buttons[i] = appButton;
            }
        }
    }
}
#endif
