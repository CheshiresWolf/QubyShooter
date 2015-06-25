using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldDestroyer : MonoBehaviour {
	public List<GameObject> players = new List<GameObject>();

	public float destroyCoeff = 2.0f;
	public float step = 0.01f;
	public float playerReactRange = 1.0f;

	private TriangleAnimator[] animators;

	private bool animationStart = false;

	private class TriangleAnimator {
		public GameObject body;
		//source
		public Vector3    sPos;
		public Quaternion sRot;
		//destination
		public Vector3    dPos;
		public Quaternion dRot;

		float trackPos  = 0.0f;
		float trackStep = 0.05f;

		public TriangleAnimator(GameObject triangle, float range) {
			this.body = triangle;
			this.sPos = triangle.transform.position;
	    	this.sRot = triangle.transform.rotation;

			float posShift = Random.Range(1.0f, range);
			this.dPos = new Vector3(
				this.sPos.x * posShift,
				this.sPos.y * posShift,
				this.sPos.z * posShift
			);
	    	
	    	float rotShift = Random.Range(-1.0f, 1.0f);
	    	this.dRot = new Quaternion(
	    		this.sRot.x,
	    		this.sRot.y,
	    		this.sRot.z,
	    		this.sRot.w + rotShift
	    	);
		}

		//distanse : 0 - sPos ... 1 - dPos 
		public void move(float distanse) {
			float velocity = Mathf.Pow(distanse, 2.0f);

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

		animationStart = true;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (animationStart) {
			foreach (TriangleAnimator animator in animators) {
				if (isInRange(animator)) {
					animator.moveToSource();
				} else {
					animator.moveToDestination();
				}
			}
		}
	}
}
