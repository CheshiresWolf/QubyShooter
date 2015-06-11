using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GearsAnimator : MonoBehaviour {
	public GameObject gear0 = null;
	public GameObject gear1 = null;
	public GameObject gear2 = null;

	int index0 = 0;
	int index1 = 0;
	int index2 = 0;

	Vector3 rot0 = new Vector3(0.0f,   0.0f, 0.0f);
	Vector3 rot1 = new Vector3(0.0f,   0.0f, 0.0f);
	Vector3 rot2 = new Vector3(270.0f, 0.0f, 0.0f);

	Quaternion quat0 = new Quaternion();
	Quaternion quat1 = new Quaternion();
	Quaternion quat2 = new Quaternion();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (index0 >= 0 && index0 < 30) {
			rot0.z += 0.1f;//1.0f;
		} else {
			rot0.z -= 0.01f;
		}

		index0++;
		if (index0 == 30) {
			index0 = -10;
		}

		quat0.eulerAngles = rot0;
		gear0.transform.rotation = quat0;

		if (index1 <= 0 && index1 > -30) {
			rot1.z -= 0.1f;//1.0f;
		} else {
			rot1.z += 0.01f;
		}

		index1--;
		if (index1 == -30) {
			index1 = 10;
		}

		quat1.eulerAngles = rot1;
		gear1.transform.rotation = quat1;

		rot2.y += 1.0f;
		quat2.eulerAngles = rot2;
		gear2.transform.rotation = quat2;
	}
}
