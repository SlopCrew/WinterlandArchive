using Reptile;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Winterland.Common {
    public class TimeOfDayController : MonoBehaviour {
        public static TimeOfDayController Instance { get; private set; }
        [HideInInspector]
        public bool InTransition = false;
        [Header("Daytime Lighting")]
        public Transform DayTransform;
        public Texture DaySkybox;
        public Color DayLightColor;
        public Color DayShadowColor;
        [Header("Nighttime Lighting")]
        public Transform NightTransform;
        public Texture NightSkybox;
        public Color NightLightColor;
        public Color NightShadowColor;
        [Header("General")]
        public AmbientOverride AmbientOverride = null;
        public Moon Moon = null;
        public enum TimesOfDay {
            Day,
            Night
        }
        [HideInInspector]
        public TimesOfDay CurrentTimeOfDay = TimesOfDay.Night;
        public float FadeDuration = 0.5f;
        public float BlackScreenDuration = 0.1f;

        public bool TransitionToTimeOfDay(TimesOfDay setTimeOfDay) {
            if (InTransition)
                return false;
            StartCoroutine(TransitionCoroutine(setTimeOfDay));
            return true;
        }

        private void Awake() {
            Instance = this;
            SetTimeOfDay(WinterProgress.Instance.LocalProgress.TimeOfDay);
        }

        private void SetTimeOfDay(TimesOfDay timeOfDay) {
            if (timeOfDay == TimesOfDay.Day)
                SetDayLighting();
            else
                SetNightLighting();
            CurrentTimeOfDay = timeOfDay;
            var localProgress = WinterProgress.Instance.LocalProgress;
            localProgress.TimeOfDay = CurrentTimeOfDay;
            localProgress.Save();
        }

        private void SetDayLighting() {
            AmbientOverride.Night = false;
            AmbientOverride.LightColor = DayLightColor;
            AmbientOverride.ShadowColor = DayShadowColor;
            AmbientOverride.Skybox = DaySkybox;
            AmbientOverride.transform.rotation = DayTransform.rotation;
            Moon.gameObject.SetActive(false);
            AmbientOverride.Refresh();
        }

        private void SetNightLighting() {
            AmbientOverride.Night = true;
            AmbientOverride.LightColor = NightLightColor;
            AmbientOverride.ShadowColor = NightShadowColor;
            AmbientOverride.Skybox = NightSkybox;
            AmbientOverride.transform.rotation = NightTransform.rotation;
            Moon.gameObject.SetActive(true);    
            AmbientOverride.Refresh();
        }

        private IEnumerator TransitionCoroutine(TimesOfDay timeOfDay) {
            InTransition = true;
            var effects = Core.Instance.UIManager.effects;
            effects.FadeToBlack(FadeDuration);
            yield return new WaitForSeconds(FadeDuration);
            SetTimeOfDay(timeOfDay);
            yield return new WaitForSeconds(BlackScreenDuration);
            effects.FadeOpen(FadeDuration);
            yield return new WaitForSeconds(FadeDuration);
            InTransition = false;
        }
    }
}
