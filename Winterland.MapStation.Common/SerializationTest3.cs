using System;
using System.Collections.Generic;
using UnityEngine;

namespace Winterland.MapStation.Common {
    public class SerializationTest3 : MonoBehaviour {
        [SerializeReference]
        private Data data = new ();
        [Serializable]
        public class Data {
            // Store all inspectable, configuration stuff on this object

            public string Foo;
            public string Bar;
            public List<string> Baz;
            public GameObject ReferencedObject;
            [SerializeReference]
            public SList_Item Other = new();
        }
        public class SList_Item : SList<Item, Node_Item> {}
        [Serializable]
        public class Item {
            public string Name;
        }
        public class Node_Item : Node<Item, Node_Item> {}

        private void OnValidate() {
            data.Other.syncToLinkedList();
            // if(data.Other == null) data.Next = new Data();
            // for(int i = 0; i < data.Length; i++) {
            //     if(data[i] == null) {
            //         data[i] = new Data();
            //     }
            //     var d = data[i];
            //     for(int i2 = 0; i2 < d.other.Length; i2++) {
            //         if(d.other[i2] == null) {
            //             d.other[i2] = new OtherData();
            //         }
            //     }
            // }
        }

        private void Awake() {
            data.Other.syncFromLinkedList();
            var a = data.Foo;
            var b = data.Bar;
            var c = data.Baz != null ? data.Baz[0] : "<no list>";
            var d = data.ReferencedObject.name;
            // var e = data.other.Length;
            // var f = data.other?[0]?.Name;
            var e = data.Other.list[0].Name;
            Debug.Log(String.Format(
                "SerializationTest3: Serialized data: foo={0}, bar={1}, Baz[0]={2}, ReferencedObject.name={3}, {4}",
                a, b, c, d, e
            ));
        }
    }
}

namespace Winterland.MapStation.Common {
    [Serializable]
    public class SList<T, NodeT> // TODO IMPLEMENT ISerializationCallbackReceiver
        where T : new()
        where NodeT : Node<T, NodeT>, new() {
        // [HideInInspector]
        [SerializeReference]
        private NodeT first;
        public List<T> list;
        public void syncToLinkedList() {
            if(list.Count == 0) {
                first = null;
                return;
            }
            first ??= new NodeT();
            var n = first;
            foreach(var item in list) {
                n.value = item;
                n.next ??= new NodeT();
                n = n.next;
            }
            n.next = null;
        }
        public void syncFromLinkedList() {
            if(list != null) list.Clear();
            else list = new();
            for(var n = first; n != null; n = n.next) {
                list.Add(n.value);
            }
        }
    }
    [Serializable]
    public class Node<T, NodeT> where NodeT : Node<T, NodeT> {
        [SerializeReference]
        public T value;
        [SerializeReference]
        public NodeT next;
    }
}