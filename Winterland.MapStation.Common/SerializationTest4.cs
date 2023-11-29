using System;
using System.Collections.Generic;
using UnityEngine;

namespace Winterland.MapStation.Common {
    public class SerializationTest4 : MonoBehaviour {
        [SerializeReference]
        public SList_Item items = new SList_Item();

        [Serializable]
        public class SList_Item : SList<Item> {}
        [Serializable]
        public class Item {
            public string Name;
        }

        private void Awake() {
            Debug.LogFormat(
                "SerializationTest4: items.Count={0} items[0].Name={1} items[1].Name={2}",
                items?.Count, items?[0]?.Name, items?[1]?.Name
            );
        }
    }

    [Serializable]
    internal class Node {
        [SerializeReference]
        public object value;
        [SerializeReference]
        public Node next;
    }

    [Serializable]
    public class SList<T> : List<T>, ISerializationCallbackReceiver
        where T : new()
    {

        // [HideInInspector]
        [SerializeReference]
        private Node first;

        public void OnAfterDeserialize() {
            copyFromLinkedList();
        }

        public void OnBeforeSerialize() {
            InstantiateNullSlots();
            copyToLinkedList();
        }
        private void InstantiateNullSlots() {
            for(int i = 0, l = Count; i < l; i++) {
                if (this[i] == null) this[i] = new T();
            }
        }

        private void copyToLinkedList() {
            if(Count == 0) {
                first = null;
            } else {
                first ??= new Node();
                var n = first;
                var l = Count - 1;
                for(int i = 0; i < l; i++) {
                    n.value = this[i];
                    n.next ??= new Node();
                    n = n.next;
                }
                n.value = this[l];
            }
        }
        private void copyFromLinkedList() {
            Clear();
            for(var n = first; n != null; n = n.next) {
                Add((T)n.value);
            }
        }
    }
}