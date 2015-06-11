using UnityEngine;
using System.Collections;

public class PositionSynchronizer : MonoBehaviour {

	//[0 .. 1]
	private float step     = 0.1f;
	private float distance = 0.0f;

	private bool startSynchFlag = false;

	Vector3 endPos   = Vector3.zero;
	Vector3 startPos = Vector3.zero;

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		Debug.Log("PositionSynchronizer | OnSerializeNetworkView");
		Vector3 syncPosition = Vector3.zero; // для синхронизации позиции

	    //Debug.Log("PositionSynchronizer | read : " + stream.isReading + "; write : " + stream.isWriting);
	    if (stream.isReading) {
	        // Receiving
	        stream.Serialize(ref syncPosition);

	        endPos   = syncPosition;
	        startPos = gameObject.transform.position;
	        distance = 0;

	        //if (!startSynchFlag)
	        //Debug.Log("PositionSynchronizer | syncPosition : " + syncPosition);
	        startSynchFlag = true;
	    }
	    if (stream.isWriting) {
	    	syncPosition = gameObject.transform.position;
	    	stream.Serialize(ref syncPosition);
	    }
	    
	}

	// Use this for initialization
	void Start () {
		startPos = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (startSynchFlag) {
			if (distance <= 1) {
				gameObject.transform.position = Vector3.Lerp(startPos, endPos, distance);
				distance += step;
			}
		}
	}
}
