// https://github.com/SlopCrew/SlopCrew/blob/f356897d0e480673fd6a0715c6f3a6bec6338255/SlopCrew.Plugin/UI/Phone/AppUtility.cs

using Reptile;
using Reptile.Phone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Winterland.Common.UI;

namespace Winterland.Common.Phone;

public static class AppUtility {
    // Width = Full width, there's nothing in the way of an app horizontally
    // Height = Phone screen height - the status at the top (160 in size)
    public static readonly Vector2 AppSize = new Vector2(1070, 1740);

    public static T CreateApp<T>(string name, Transform root) where T : App {
        var appObject = new GameObject(name);
        appObject.layer = Layers.Phone;

        var appRect = appObject.AddComponent<RectTransform>();
        appRect.SetParent(root, false);
        // We need to set the size manually as the app parent is of size 0
        appRect.sizeDelta = AppSize;
        // Align the app to the top
        // We don't have to compensate for the status bar here because the app parent already does this
        appRect.SetAnchorAndPivot(0.5f, 1.0f);

        var contentObject = new GameObject("Content");
        contentObject.layer = Layers.Phone;
        contentObject.transform.SetParent(appRect, false);

        var contentRect = contentObject.AddComponent<RectTransform>();
        contentRect.StretchToFillParent();

        var app = appObject.AddComponent<T>();

        return app;
    }

    public static RectTransform CreateAppOverlay(string title, Sprite icon, float fontSize = 80f) {
        var player = WorldHandler.instance.GetCurrentPlayer();
        var phone = player.phone;
        var sourceApp = phone.GetAppInstance<AppGraffiti>();
        var overlay = sourceApp.transform.Find("Overlay");
        var newOverlay = GameObject.Instantiate(overlay);
        var icons = newOverlay.transform.Find("Icons");
        icons.Find("GraffitiIcon").GetComponent<Image>().sprite = icon;
        var header = icons.Find("HeaderLabel");
        Component.Destroy(header.GetComponent<TMProLocalizationAddOn>());
        var tmpro = header.GetComponent<TextMeshProUGUI>();
        tmpro.text = title;
        tmpro.fontSize = fontSize;
        tmpro.fontSizeMax = fontSize;
        tmpro.fontSizeMin = fontSize;
        return newOverlay.RectTransform();
    }
}
