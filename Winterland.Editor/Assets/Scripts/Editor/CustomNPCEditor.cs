using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Winterland.Common;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(CustomNPC))]
public class CustomNPCEditor : Editor {
    private bool showBranches = false;
    public override void OnInspectorGUI() {
        var npc = serializedObject.targetObject as CustomNPC;
        var dialogBranches = DialogueBranch.GetComponentsOrdered<DialogueBranch>(npc.gameObject);
        if (dialogBranches.Length <= 0)
            EditorGUILayout.HelpBox("No dialogue branches - Nothing will happen when you interact with this NPC.", MessageType.Warning);
        if (npc.PlacePlayerAtSnapPosition && npc.transform.Find("PlayerSnapPosition") == null)
            EditorGUILayout.HelpBox("Please create an empty child GameObject named PlayerSnapPosition positioned where you would like the player to get placed at.", MessageType.Warning);
        
        if (GUILayout.Button("Generate GUID")) {
            npc.GUID = Guid.NewGuid();
            EditorUtility.SetDirty(serializedObject.targetObject);
        }

        DrawDefaultInspector();

        EditorGUILayout.Space();

        GUILayout.BeginVertical("window");
        showBranches = GUILayout.Toggle(showBranches, $"Dialogue Branches ({dialogBranches.Length})", "DropDownButton");
        if (showBranches) {
            
            EditorGUILayout.HelpBox("Dialogue branch priority is from top to bottom.", MessageType.Info);
            for(var i=0;i<dialogBranches.Length;i++) {
                EditorGUILayout.Space();
                var branch = dialogBranches[i];
                GUILayout.BeginVertical($"Branch #{i}", "window");
                var editor = Editor.CreateEditor(branch);

                editor.OnInspectorGUI();
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove")) {
                    DestroyImmediate(branch);
                }
                if (GUILayout.Button("Move Up")) {
                    EditorHelper.MoveUp(editor.serializedObject);
                }
                if (GUILayout.Button("Move Down")) {
                    EditorHelper.MoveDown(editor.serializedObject);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Branch")) {
                var newBranch = npc.gameObject.AddComponent<DialogueBranch>();
                newBranch.hideFlags = HideFlags.HideInInspector;
            }
            
        }
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        //EditorGUILayout.Space();
    }
}
