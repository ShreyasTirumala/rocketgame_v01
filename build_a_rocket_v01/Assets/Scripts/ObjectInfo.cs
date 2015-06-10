using UnityEngine;
using System.Collections;

public class ObjectInfo : MonoBehaviour {
	public Vector3 lockPosition;
	public bool firstTouch = false;
	private float jumpSize;

	//characteristic values
	public int airResistance = 10;
	public int fuel = 10;
	public int power = 10;
	public int weight = 10;

	//use this for keeping objects visible
	public bool seeMe = false;

	public bool added = false;
	// Use this for initialization
	public void Start () {
		Vector3 piecePosition = gameObject.transform.position;
		lockPosition = piecePosition;
		GameObject objLeft = GameObject.Find ("LeftPanel");
		jumpSize = objLeft.GetComponent<MeshRenderer> ().bounds.size.x;
		if (piecePosition.x < 0) { 
			lockPosition.x = piecePosition.x + jumpSize ; 
		} else {
			lockPosition.x = piecePosition.x - jumpSize ;
		}
	}

	public int newLock (Vector3 newLock) {
		lockPosition = newLock;
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
