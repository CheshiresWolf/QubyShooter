using UnityEngine;
using System.Collections;

public class WorldDestroyer : MonoBehaviour {
	public GameObject player;

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

		public TriangleAnimator(GameObject triangle) {
			this.body = triangle;
			this.sPos = triangle.transform.position;
	    	this.sRot = triangle.transform.rotation;

	    	float posShift = Random.Range(0.0f, 5.0f);
			this.dPos = new Vector3(
				this.sPos.x + (posShift * ((this.sPos.x < 0) ? -1 : 1) ),
				this.sPos.y + (posShift * ((this.sPos.y < 0) ? -1 : 1) ),
				this.sPos.z + (posShift * ((this.sPos.z < 0) ? -1 : 1) )
			);
	    	
	    	//float rotShift = ...
	    	//this.dRot = new Quaternion(...);
		}

		//distanse : 0 - sPos ... 1 - dPos 
		public void move(float distanse) {
			this.body.transform.position = new Vector3(
				this.sPos.x + (this.dPos.x - this.sPos.x) / distanse,
				this.sPos.y + (this.dPos.y - this.sPos.y) / distanse,
				this.sPos.z + (this.dPos.z - this.sPos.z) / distanse
			);
		
			//this.body.transform.position.x = this.sPos.x + (this.dPos.x - this.sPos.x) / distanse;
			//this.body.transform.position.y = this.sPos.y + (this.dPos.y - this.sPos.y) / distanse;
			//this.body.transform.position.z = this.sPos.z + (this.dPos.z - this.sPos.z) / distanse;
		}
	}

	void collapseIcosaedron() {


		initialAnimation = false;
	}

	public void setTriangles(GameObject[] triangles) {
		animators = new TriangleAnimator[triangles.Length];

		for (int i = 0; i < triangles.Length; i++) {
			animators[i] = new TriangleAnimator(triangles[i]);
		}

		new WaitForSeconds(5);
		initialAnimation = false;
	}

	// Use this for initialization
	void Start () {
		/*
		generator_script = icosaedron.GetComponent<Generator>();
		GameObject[] triangles = generator_script.getTriangles();

		animators = new TriangleAnimator[triangles.Length];
		for (int i = 0; i < triangles.Length; i++) {
			animators[i] = new TriangleAnimator(triangles[i]);
		}
		*/
		//collapseIcosaedron();
	}
	
	// Update is called once per frame
	void Update () {
		if (!initialAnimation) {
			if (initialAnimationCoef < 1) {
				for (int i = 0; i < animators.Length; i++) {
					animators[i].move(initialAnimationCoef);
				}
			
				initialAnimationCoef += 0.01f;
			}
		}
	}
}
