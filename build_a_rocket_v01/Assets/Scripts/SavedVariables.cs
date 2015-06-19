using UnityEngine;
using System.Collections;

public class SavedVariables : MonoBehaviour {

	public int d1 = -1;
		public int d2 = -1;
			public int d3 = -1;
			public int d4 = -1;
			public int d5 = -1;

	void Awake() {
		DontDestroyOnLoad (this);
	}
}
