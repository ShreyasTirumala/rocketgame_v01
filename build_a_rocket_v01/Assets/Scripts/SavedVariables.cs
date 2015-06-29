using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SavedVariables : MonoBehaviour {

	public int d1 = -1;
	public int d2 = -1;
	public int d3 = -1;
	public int d4 = -1;
	public int d5 = -1;

	public List<string> previousTrialRocketPieceNames = new List<string> ();
	public List<Vector3> previousTrialRocektPiecePositions = new List<Vector3> ();

	void Awake() {
		DontDestroyOnLoad (this);
	}
}
