using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SavedPieceInfo {
	public Vector3 initialLockPosition;
	public int pieceType; // 0 - cone, 1 - body, 2 - booster, 3 - fin
	public int pos;
	public Vector3 vectorPos;

	public SavedPieceInfo(Vector3 initialLockPosition, int pieceType, int pos, Vector3 vectorPos)
	{
		this.initialLockPosition = initialLockPosition;
		this.pieceType = pieceType;
		this.pos = pos;
		this.vectorPos = vectorPos;
	}
}

public class SavedVariables : MonoBehaviour {

	public int d1 = -1;
	public int d2 = -1;
	public int d3 = -1;
	public int d4 = -1;
	public int d5 = -1;

	public List<SavedPieceInfo> previousTrialRocketPieces = new List<SavedPieceInfo> ();

	void Awake() {
		DontDestroyOnLoad (this);
	}
}
