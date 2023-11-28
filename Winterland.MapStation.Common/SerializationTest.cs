using System;
using System.Collections.Generic;
using UnityEngine;

namespace Winterland.MapStation.Common {
    public interface ISupportsBepInExSerializationWorkaround<T>
        where T : class
    {
        public T SerializedData {get;}
    }

    public class SerializationTest : MonoBehaviour, ISerializationCallbackReceiver, ISupportsBepInExSerializationWorkaround<SerializationTest.Data> {
        // Serialize state as JSON
        [SerializeField]
        private string json = "";
        Data ISupportsBepInExSerializationWorkaround<Data>.SerializedData => data;
        // Parse json to/from this sub-object
        private Data data = new ();
        [Serializable]
        public class Data {
            // Store all inspectable, configuration stuff on this object

            public string Foo;
            public string Bar;
            public List<string> Baz;
            public GameObject ReferencedObject;
        }

        private void Awake() {
            Debug.Log(String.Format(
                "Serialized data: foo={0}, bar={1}, Baz[0]={2}, ReferencedObject.name={3}",
                data.Foo, data.Bar, data.Baz[0], data.ReferencedObject != null ? data.ReferencedObject.name : "<null>"
            ));
        }

        public void OnBeforeSerialize() {
            try {
                json = JsonUtility.ToJson(data);
            } catch(Exception e) {
                Debug.Log("Error serializing to JSON: " + e.Message);
            }
        }

        public void OnAfterDeserialize() {
            Debug.Log(json);
            try {
                JsonUtility.FromJsonOverwrite(json, data);
            } catch(Exception e) {
                Debug.Log("Error deserializing from JSON: " + e.Message);
            }
        }
    }
}