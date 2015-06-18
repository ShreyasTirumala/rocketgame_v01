using UnityEngine;
using System.Collections;

public class ObjectInfo : MonoBehaviour {
	public Vector3 lockPosition;
	public Vector3 initialLockPosition;
	public bool firstTouch = false;
	private float jumpSize;

	//characteristic values
	public int airResistance = 10;
	public int fuel = 10;
	public int power = 10;
	public int weight = 10;

	public bool jump = true;

	//use this for keeping objects visible
	public bool seeMe = false;

	public bool added = false;
	// Use this for initialization
	public void Start () {
		Vector3 piecePosition = gameObject.transform.position;
		initialLockPosition = lockPosition = piecePosition;
		GameObject objLeft = GameObject.Find ("LeftPanel");
		jumpSize = objLeft.GetComponent<MeshRenderer> ().bounds.size.x;
		if (jump) {
			if (piecePosition.x < 0) { 
				lockPosition.x = piecePosition.x + jumpSize; 
			} else {
				lockPosition.x = piecePosition.x - jumpSize;
			}
		}
	}

	public void Update() {
		if (gameObject.transform.position == lockPosition && lockPosition != initialLockPosition) {
			GameObject.Find ("GameManager").GetComponent<TouchInputHandler> ().addToRocketPieces (gameObject);
		}
		foreach (GameObject outline in GameObject.Find("GameManager").GetComponent<TouchInputHandler>().selectedOutlinePieces) {
			if (outline.transform.position == lockPosition) {
				outline.GetComponent<OutlineInfo> ().objectLocked = true;
			} 
		}
	}

	public int newLock (Vector3 newLock) {
		foreach (GameObject outline in GameObject.Find("GameManager").GetComponent<TouchInputHandler>().selectedOutlinePieces) {
			if (outline.transform.position == lockPosition) {
				outline.GetComponent<OutlineInfo> ().objectLocked = false;
			} 
		}
		lockPosition = newLock;
		if (lockPosition.x == initialLockPosition.x) {
			initialLockPosition = lockPosition;
		} //fixes jumbling error

		seeMe = true;
		gameObject.transform.parent = null;


		return 0;
	}

	public int reLock () {
		if (firstTouch) {
			gameObject.transform.position = lockPosition;
		}
		return 0;
	}
}
