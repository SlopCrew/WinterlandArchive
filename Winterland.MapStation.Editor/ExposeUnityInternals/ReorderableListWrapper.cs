using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Winterland.MapStation.Editor.ExposedUnityInternals {
    public class ReorderableListWrapper {
        private ReorderableListWrapper wrapped;
        public ReorderableListWrapper(SerializedProperty property, GUIContent label, bool reorderable = true) {
            wrapped = new ReorderableListWrapper(property, label, reorderable);
        }

        public void Draw(GUIContent label, Rect r, Rect visibleArea, string tooltip, bool includeChildren) {
            wrapped.Draw(label, r, visibleArea, tooltip, includeChildren);
            ReorderableList foo = null;
        }
    }
}