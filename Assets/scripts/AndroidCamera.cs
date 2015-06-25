using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AndroidCamera : MonoBehaviour {

	public GameObject camera;
	public GameObject icosaedron;
	public GameObject player;

	private float radius;

	private bool isInitAnimationFinised = false;

	Vector3 currentRotation = new Vector3(0, 0, 0);
	Vector3 position = Vector3.zero;
	Quaternion rotation = Quaternion.identity;

	RaycastHit hit = new RaycastHit();
    Ray ray;

    ArrowsUI moveArrows;
    ArrowsUI lookArrows;
    ArrowsUI strafeArrows;
	
	GameObject moveArrowsPanel;
    GameObject lookArrowsPanel;

    static Text logTextField;
    static Logger Log;

    List<MyOwnTouch> touchList = new List<MyOwnTouch>();
    List<MyOwnTouch> newTouchList;

    Quaternion plusOneDegree, minusOneDegree;
    float cameraOrbitAngle = Mathf.PI / 90.0f;
    float stepAngle = Mathf.PI / 480.0f;

    float cameraLookAtOffset = 1.3f;

    float lookAngleCounter = 0;
    float minLookAngle = -20.0f * Mathf.PI / 90.0f;
    float maxLookAngle =  20.0f * Mathf.PI / 90.0f;

    //reset Look At vertical position
    bool resetLA = false;

    KeyMap keyMap;

    //=======<Debug>=======

    static bool DEBUG = true;
    bool leftMouseButtonDebug = true;

    //======</Debug>=======

    class Logger {
    	Text textField;
    	GameObject textPanel;

    	int length;
    	string[] messages;

		int index = 0;

    	public Logger() {
			this.length = 7; //because i can

			this.messages = new string[this.length];
			for (int i = 0; i < this.length; i++) {
				this.messages[i] = "";
			}

    		this.textPanel = GameObject.Find("log_image");
    		this.textField = GameObject.Find("log_text").GetComponent<Text>();
    	}

    	public void print(string message) {
			this.index++;

    		for (int i = this.length - 1; i > 0; i--) {
    			this.messages[i] = this.messages[i - 1];
    		}
    		this.messages[0] = message;

    		this.textField.text = formOutput();
		}

		public void hide() {
			this.textPanel.SetActive(false);
		}

		string formOutput() {
			string res = "";

			for (int i = 0; i < length; i++) {
				if (messages[i] != "") {
					res += (this.index - i) + ") " + messages[i] + "\n";
				} else {
					res += "";
				}
			}

			return res;
		}
    }

    class MyOwnTouch {
    	public int fingerId;

    	public Vector3 startPos;
    	public Vector3 currentPos;

    	public bool up    = false;
    	public bool right = false;
    	public bool down  = false;
    	public bool left  = false;

		public string moveType = "";

		public MyOwnTouch(Touch touch, GameObject moveAP, GameObject moveLP) {
    		this.fingerId = touch.fingerId;

    		this.startPos   = touch.position;
    		this.currentPos = touch.position;

    		this.moveType = "none";

			if (isIn(touch.position, moveAP)) {
				this.moveType = "move";
			}

			if (isIn(touch.position, moveLP)) {
				this.moveType = "look";
			}
    	}

    	//only for debug
    	public MyOwnTouch(Vector3 pos, GameObject moveAP, GameObject moveLP) {
			this.fingerId = 10;

    		this.startPos   = pos;
    		this.currentPos = pos;

    		this.moveType = "none";

			if (isIn(pos, moveAP)) {
				this.moveType = "move";
			}

			if (isIn(pos, moveLP)) {
				this.moveType = "look";
			}
    	}

    	public void move(Vector3 newPos) {
    		this.currentPos = newPos;

    		float distance = Vector3.Distance(this.startPos, this.currentPos);

    		if (distance > 1.0f) {
    			this.up    = (this.currentPos.y - this.startPos.y   > 15.0f);
    			this.down  = (this.startPos.y   - this.currentPos.y > 15.0f);
    			this.left  = (this.startPos.x   - this.currentPos.x > 15.0f);
    			this.right = (this.currentPos.x - this.startPos.x   > 15.0f);
    		} else {
    			this.up    = false;
    			this.down  = false;
    			this.left  = false;
    			this.right = false;
    		}
    	}

    	private bool isIn(Vector3 dot, GameObject polygon) {
    		bool res = false;

    		Vector3 polyPos  = polygon.transform.position;
			Rect    polySize = polygon.GetComponent<RectTransform>().rect;

			polyPos.x += polySize.x;
			polyPos.y += polySize.y;

			if ( dot.x > polyPos.x && dot.x < (polyPos.x + polySize.width ) &&
			     dot.y > polyPos.y && dot.y < (polyPos.y + polySize.height) ) {
				res = true;
			}

			if (DEBUG) Log.print("AndroidCamera | MyOwnTouch | isIn | dot : " + dot + "; polygon(" + polygon.name + ") pos : " + polyPos + "; size : " + polySize + "; res : " + res);

    		return res;
    	}
    }

    class ArrowsUI {
    	public Image up    = null;
    	public Image right = null;
    	public Image down  = null;
    	public Image left  = null;

    	public ArrowsUI(string type) {
    		GameObject bufUp = GameObject.Find(type + "_t_arrow");
    		GameObject bufBottom = GameObject.Find(type + "_b_arrow");

    		this.up    = (bufUp) ? bufUp.GetComponent<Image>() : null;
    		this.right = GameObject.Find(type + "_r_arrow").GetComponent<Image>();
    		this.down  = (bufBottom) ? bufBottom.GetComponent<Image>() : null;
    		this.left  = GameObject.Find(type + "_l_arrow").GetComponent<Image>();
    	}

    	public void setDirection(bool up, bool right, bool down, bool left) {
			this.up.color    = up    ? Color.red : Color.black;
			this.right.color = right ? Color.red : Color.black;
			this.down.color  = down  ? Color.red : Color.black;
			this.left.color  = left  ? Color.red : Color.black;
    	}

    	public void setDirection(bool right, bool left) {
			this.right.color = right ? Color.red : Color.black;
			this.left.color  = left  ? Color.red : Color.black;
    	}
    }

    class KeyMap {
    	public bool moveUp, moveRight, moveDown, moveLeft;
		public bool lookUp, lookRight, lookDown, lookLeft;
		public bool strafeLeft, strafeRight;

		public KeyMap() {
			this.reset();
		}

		public void reset() {
			this.moveUp    = false;
			this.moveRight = false;
			this.moveDown  = false;
			this.moveLeft  = false;

			this.lookUp    = false;
			this.lookRight = false;
			this.lookDown  = false;
			this.lookLeft  = false;

			this.strafeLeft  = false;
			this.strafeRight = false;
		}
    }

	void moveCamera() {
		foreach (MyOwnTouch ownTouch in touchList) {
			if (ownTouch.moveType == "move") {
				keyMap.moveUp    = keyMap.moveUp    || ownTouch.up;
				keyMap.moveDown  = keyMap.moveDown  || ownTouch.down;
				keyMap.moveRight = keyMap.moveRight || ownTouch.right;
				keyMap.moveLeft  = keyMap.moveLeft  || ownTouch.left;
			} else if (ownTouch.moveType == "look") {
				keyMap.lookUp    = keyMap.lookUp    || ownTouch.up;
				keyMap.lookDown  = keyMap.lookDown  || ownTouch.down;
				keyMap.lookRight = keyMap.lookRight || ownTouch.right;
				keyMap.lookLeft  = keyMap.lookLeft  || ownTouch.left;
			}
		}

		if (resetLA) {
			if (lookAngleCounter > cameraOrbitAngle) {
				keyMap.lookDown = true;
			} else if (lookAngleCounter < -cameraOrbitAngle) {
				keyMap.lookUp = true;
			} else {
				resetLA = false;
			}
		}
		
		if (keyMap.moveUp) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Quaternion newRot = rotateAroundVector(normal, -stepAngle);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			lookAtPlayer();
		}

		if (keyMap.moveDown) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Quaternion newRot = rotateAroundVector(normal, stepAngle);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			lookAtPlayer();
		}

		if (keyMap.moveRight) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Vector3 contrNormal = Vector3.Cross(Vector3.zero - normal, Vector3.zero - player.transform.position);

			Quaternion newRot = rotateAroundVector(contrNormal, -stepAngle);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			lookAtPlayer();
		}
		if (keyMap.moveLeft) {
			Vector3 normal = Vector3.Cross(Vector3.zero - player.transform.position, Vector3.zero - camera.transform.position);
			Vector3 contrNormal = Vector3.Cross(Vector3.zero - normal, Vector3.zero - player.transform.position);

			Quaternion newRot = rotateAroundVector(contrNormal, stepAngle);
			player.transform.position = newRot * player.transform.position;
			camera.transform.position = newRot * camera.transform.position;

			lookAtPlayer();
		}

		if (keyMap.lookUp) {			
			if (lookAngleCounter < maxLookAngle) {
				Vector3 pivot = getPivot(player.transform.position, cameraLookAtOffset);
				camera.transform.position = rotatePointAroundPivot(camera.transform.position, pivot, cameraOrbitAngle);

				lookAtPlayer();

				lookAngleCounter += cameraOrbitAngle;
			}
		}
		if (keyMap.lookDown) {
			if (lookAngleCounter > minLookAngle) {
				Vector3 pivot = getPivot(player.transform.position, cameraLookAtOffset);
				camera.transform.position = rotatePointAroundPivot(camera.transform.position, pivot, -cameraOrbitAngle);
				
				lookAtPlayer();

				lookAngleCounter -= cameraOrbitAngle;
			}
		}

		if (keyMap.lookLeft) {
			camera.transform.position = rotateAroundVector(player.transform.position,  cameraOrbitAngle) * camera.transform.position;
			lookAtPlayer();
		}
		if (keyMap.lookRight) {
			camera.transform.position = rotateAroundVector(player.transform.position,  -cameraOrbitAngle) * camera.transform.position; 
			lookAtPlayer();
		}

		camera.transform.rotation = rotation;

		moveArrows.setDirection(keyMap.moveUp, keyMap.moveRight, keyMap.moveDown, keyMap.moveLeft);
		lookArrows.setDirection(keyMap.lookUp, keyMap.lookRight, keyMap.lookDown, keyMap.lookLeft);

		keyMap.reset();
	}

	void lookAtPlayer() {
		Vector3 pivot = getPivot(player.transform.position, cameraLookAtOffset);

		rotation.SetLookRotation(pivot - camera.transform.position, Vector3.zero - camera.transform.position);
	}

	Quaternion rotateAroundVector(Vector3 vec, float angle) {
		float distance = Vector3.Distance(Vector3.zero, vec);

		//normalize
		Vector3 nPoint = new Vector3(
			(vec.x != 0) ? (vec.x / distance) : 0,
			(vec.y != 0) ? (vec.y / distance) : 0,
			(vec.z != 0) ? (vec.z / distance) : 0
		);

		return new Quaternion(
			nPoint.x * Mathf.Sin(angle / 2.0f),
			nPoint.y * Mathf.Sin(angle / 2.0f),
			nPoint.z * Mathf.Sin(angle / 2.0f),
			           Mathf.Cos(angle / 2.0f)
		);
	}

	Vector3 getPivot(Vector3 point, float distance) {
		float centerDistance = Vector3.Distance(Vector3.zero, point);

		return new Vector3(
			point.x - ( (point.x != 0) ? (point.x * distance / centerDistance) : 0 ),
			point.y - ( (point.y != 0) ? (point.y * distance / centerDistance) : 0 ),
			point.z - ( (point.z != 0) ? (point.z * distance / centerDistance) : 0 )
		);
	}

	Vector3 rotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle) {
		Vector3 normal = Vector3.Cross(player.transform.position - pivot, point - pivot).normalized;

		Quaternion bufQuat =  new Quaternion(
			normal.x * Mathf.Sin(angle / 2.0f),
			normal.y * Mathf.Sin(angle / 2.0f),
			normal.z * Mathf.Sin(angle / 2.0f),
			           Mathf.Cos(angle / 2.0f)
		);

		return bufQuat * (point - pivot) + pivot;
	}

	Vector3 placeOnSphere(Vector3 point) {
		float centerDistance = Vector3.Distance(Vector3.zero, player.transform.position);

		return new Vector3(
			point.x * radius / centerDistance,
			point.y * radius / centerDistance,
			point.z * radius / centerDistance
		);
	}

	public void resetLookAt() {
		if (isInitAnimationFinised) {
			resetLA = true;
		}
	}

	// Use this for initialization
	void Start () {
		radius = icosaedron.GetComponent<Generator>().radius;

		player.transform.position = new Vector3( 0,        0, radius - 0.5f );
		camera.transform.position = new Vector3( 0, -3.5355f, -3.5355f      );

		moveArrowsPanel = GameObject.Find("move_panel");
    	lookArrowsPanel = GameObject.Find("look_panel");

		moveArrows = new ArrowsUI("move");
		lookArrows = new ArrowsUI("look");

		Log = new Logger();

		keyMap = new KeyMap();
		
		if (DEBUG) Log.print("AndroidCamera | Start");
		if (!DEBUG) {
			Log.hide();

			moveArrowsPanel.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
			lookArrowsPanel.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isInitAnimationFinised) {
			Vector3 newCameraPos = camera.transform.position;
			newCameraPos.z += radius / 150;
			camera.transform.position = newCameraPos;

			lookAtPlayer();
			camera.transform.rotation = rotation;

			if (newCameraPos.z >= (radius - 3.5355f)) {
				isInitAnimationFinised = true;
			}
		} else {
			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Began) {
					touchList.Add(new MyOwnTouch(touch, moveArrowsPanel, lookArrowsPanel));
				}
				if (touch.phase == TouchPhase.Moved) {
					foreach (MyOwnTouch ownTouch in touchList) {
						if (ownTouch.fingerId == touch.fingerId) {
							ownTouch.move(touch.position);
							break;
						}
					}
				}
				if (touch.phase == TouchPhase.Ended) {
					newTouchList = new List<MyOwnTouch>();

					foreach (MyOwnTouch ownTouch in touchList) {
						if (ownTouch.fingerId != touch.fingerId) {
							newTouchList.Add(ownTouch);
						}
					}

					touchList = newTouchList;
				}
			}

			if (DEBUG) {
				//only for debug
				if (Input.GetMouseButtonDown(0)) {
					if (leftMouseButtonDebug) {
						touchList.Add(new MyOwnTouch(Input.mousePosition, moveArrowsPanel, lookArrowsPanel));
						leftMouseButtonDebug = false;
					}
				}
				if (Input.GetMouseButton(0)) {
					if (!leftMouseButtonDebug) {
						touchList[0].move(Input.mousePosition);
					}
				}
				if (Input.GetMouseButtonUp(0)) {
					leftMouseButtonDebug = true;
					touchList = new List<MyOwnTouch>();
				}
			}

			moveCamera();
		}
	}
}
