using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldDestroyer : MonoBehaviour {
	public List<GameObject> players = new List<GameObject>();

	public float destroyCoeff = 2.0f;
	public float step = 0.01f;
	public float playerReactRange = 1.0f;

	private TriangleAnimator[] animators;

	/*
	 * 0 - idle
	 * 1 - move to initial position
	 * 2 - react on user moves
	 */
	private int   animationMode        = 0;
	private float initialAnimationCoef = 0;

	private class TriangleAnimator {
		public GameObject body;
		//source
		public Vector3    sPos;
		public Quaternion sRot;
		//destination
		public Vector3    dPos;
		public Quaternion dRot;

		float trackPos  = 1.0f;
		float trackStep = 0.05f;

		public TriangleAnimator(GameObject triangle, float range) {
			this.body = triangle;
			this.sPos = triangle.transform.position;
	    	this.sRot = triangle.transform.rotation;

			float posShift = Random.Range(1.0f, range);
			this.dPos = new Vector3(
				this.sPos.x * posShift,//+ (posShift * ((this.sPos.x < 0) ? -1 : 1) ),
				this.sPos.y * posShift,//+ (posShift * ((this.sPos.y < 0) ? -1 : 1) ),
				this.sPos.z * posShift //+ (posShift * ((this.sPos.z < 0) ? -1 : 1) )
			);
	    	
	    	float rotShift = Random.Range(-1.0f, 1.0f);
	    	this.dRot = new Quaternion(
	    		this.sRot.x,
	    		this.sRot.y,
	    		this.sRot.z,
	    		this.sRot.w + rotShift
	    	);

			//Debug.Log("WorldDestroyer | TriangleAnimator | sPos : " + sPos + "; dPos : " + dPos);
		}

		//distanse : 0 - sPos ... 1 - dPos 
		public void move(float distanse) {
			//if (distanse < 0) {
			//	distanse = 0;
				//Debug.Log("WorldDestroyer | TriangleAnimator | move | distanse : " + distanse + "; velocity : " + velocity);
			//}
			float velocity = Mathf.Pow(distanse, 2.0f);
			//Mathf.Sqrt(distanse);

			this.body.transform.position = new Vector3(
				this.sPos.x + (this.dPos.x - this.sPos.x) * velocity,
				this.sPos.y + (this.dPos.y - this.sPos.y) * velocity,
				this.sPos.z + (this.dPos.z - this.sPos.z) * velocity
			);

			this.body.transform.rotation = new Quaternion(
				this.sRot.x,
	    		this.sRot.y,
	    		this.sRot.z,
	    		this.sRot.w + (this.dRot.w - this.sRot.w) * distanse
			);
		}

		public void moveToDestination() {
			if (this.trackPos < 1.0f) {
				this.trackPos += this.trackStep;
				this.move(this.trackPos);
			}
		}

		public void moveToSource() {
			if (this.trackPos > 0.0f) {
				this.trackPos -= this.trackStep;
				this.move(this.trackPos);
			}
		}
	}

	bool isInRange(TriangleAnimator animator) {
		bool res = false;

		foreach(GameObject target in players) {
			if (Vector3.Distance(target.transform.position, animator.sPos) <= playerReactRange) {
				res = true;
				break;
			}
		}

		return res;
	}

	void collapseIcosaedron() {

	}

	public void setTriangles(GameObject[] triangles) {
		animators = new TriangleAnimator[triangles.Length];

		for (int i = 0; i < triangles.Length; i++) {
			animators[i] = new TriangleAnimator(triangles[i], destroyCoeff);
		}

		//new WaitForSeconds(5);
		Debug.Log("WorldDestroyer | Update | start initial animation");
		animationMode = 1;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (animationMode == 1) {
			if (initialAnimationCoef < 1) {
				initialAnimationCoef += step;

				for (int i = 0; i < animators.Length; i++) {
					animators[i].move(initialAnimationCoef);
				}
			} else {
				Debug.Log("WorldDestroyer | Update | initial animation complete");
				animationMode = 2;
			}
		}
		if (animationMode == 2) {
			for (int i = 0; i < animators.Length; i++) {
				if (isInRange(animators[i])) {
					animators[i].moveToSource();
				} else {
					animators[i].moveToDestination();
				}
			}
		}
	}
}
