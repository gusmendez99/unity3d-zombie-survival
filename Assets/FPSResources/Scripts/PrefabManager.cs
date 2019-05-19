using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager: MonoBehaviour {
	[Serializable]
	public struct PrefabItem { 
		public string key; //the key of the prefab to instantiate
		public GameObject prefab; //the prefab to return
	}

	public PrefabItem[] prefabs; //array that contains all prefabs

    // returns a prefab with a given key, found in the prefabs array
	public GameObject GetPrefab(string key) {
		GameObject foundPrefab = null;

		foreach(PrefabItem prefabItem in prefabs) {
			if(prefabItem.key == key) {
				foundPrefab = prefabItem.prefab;
				break;
			}
		}

		return foundPrefab;
	}

	public static PrefabManager GetInstance() {
		return GameObject.Find("PrefabManager").GetComponent<PrefabManager>();
	}
}