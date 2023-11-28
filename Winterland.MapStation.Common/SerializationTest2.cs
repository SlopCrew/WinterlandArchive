using System;
using System.Collections.Generic;
using UnityEngine;

namespace Winterland.MapStation.Common {
    public class SerializationTest2 : MonoBehaviour {
        [SerializeField]
        private Data data = new ();
        public class Data : ScriptableObject {
            // Store all inspectable, configuration stuff on this object

            public string Foo;
            public string Bar;
            public List<string> Baz;
            public GameObject ReferencedObject;
            // public List<Data> Recursive;
        }

        void OnValidate() {
            if(data == null) {
                data = ScriptableObject.CreateInstance<Data>();
            }
        }

        private void Awake() {
            Debug.Log(String.Format(
                "SerializationTest2: Serialized data: foo={0}, bar={1}, Baz[0]={2}, ReferencedObject.name={3}",
                data.Foo, data.Bar, data.Baz[0], data.ReferencedObject != null ? data.ReferencedObject.name : "<null>"
            ));
        }
    }
}