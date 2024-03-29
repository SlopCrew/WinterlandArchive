using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class ProjectChangeListener
{
    static ProjectChangeListener() {
        EditorApplication.projectChanged -= Recompile;
        EditorApplication.projectChanged += Recompile;
    }
    private static void Recompile() {
        if (!PluginEditor.IsPluginOutOfDate())
            return;
        if (!BuildAssets.IsGameOpen())
        {
            var rebuildProcess = PluginEditor.RebuildPlugin();
            rebuildProcess.WaitForExit();
        }
    }
}
