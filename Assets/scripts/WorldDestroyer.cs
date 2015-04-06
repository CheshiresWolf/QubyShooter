using UnityEngine;
using System.Collections;

public class WorldDestroyer : MonoBehaviour {
	public GameObject player;

	public float maxRange = 2.0f;
	public float step = 0.01f;

	private TriangleAnimator[] animators;

	private bool initialAnimation = true;
	private float initialAnimationCoef = 0;

	private class TriangleAnimator {
		GameObject body;
		//source
		Vector3    sPos;
		Quaternion sRot;
		//destination
		Vector3    dPos;
		Quaternion dRot;

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
	    	
	    	//float rotShift = ...
	    	//this.dRot = new Quaternion(...);

			//Debug.Log("WorldDestroyer | TriangleAnimator | sPos : " + sPos + "; dPos : " + dPos);
		}

		//distanse : 0 - sPos ... 1 - dPos 
		public void move(float distanse) {
			this.body.transform.position = new Vector3(
				this.sPos.x + (this.dPos.x - this.sPos.x) * distanse,
				this.sPos.y + (this.dPos.y - this.sPos.y) * distanse,
				this.sPos.z + (this.dPos.z - this.sPos.z) * distanse
			);
		
			//this.body.transform.position.x = this.sPos.x + (this.dPos.x - this.sPos.x) / distanse;
			//this.body.transform.position.y = this.sPos.y + (this.dPos.y - this.sPos.y) / distanse;
			//this.body.transform.position.z = this.sPos.z + (this.dPos.z - this.sPos.z) / distanse;
		}

		public void moveToDestination() {
			this.body.transform.position = new Vector3(
				this.dPos.x,
				this.dPos.y,
				this.dPos.z
			);
		}
	}

	void collapseIcosaedron() {

	}

	public void setTriangles(GameObject[] triangles) {
		animators = new TriangleAnimator[triangles.Length];

		for (int i = 0; i < triangles.Length; i++) {
			animators[i] = new TriangleAnimator(triangles[i], maxRange);
		}

		//new WaitForSeconds(5);
		Debug.Log("WorldDestroyer | Update | start initial animation");
		initialAnimation = false;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!initialAnimation) {
			if (initialAnimationCoef < 1) {
				initialAnimationCoef += step;

				for (int i = 0; i < animators.Length; i++) {
					animators[i].move(initialAnimationCoef);
				}
			} else {
				Debug.Log("WorldDestroyer | Update | initial animation complete");
				initialAnimation = true;

				for (int i = 0; i < animators.Length; i++) {
					animators[i].moveToDestination();
				}
			}
		}
	}
}
