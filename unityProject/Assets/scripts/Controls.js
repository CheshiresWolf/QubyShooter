#pragma strict
var defaultCameraScript : CameraMove;
var spherePlayerScript : PlayerMove;

function Start () {
	var defaultCamera : GameObject = GameObject.Find("Camera");
	defaultCameraScript = defaultCamera.GetComponent("CameraMove");
	var spherePlayer : GameObject = GameObject.Find("SpherePlayer");
	spherePlayerScript = spherePlayer.GetComponent("PlayerMove");
}

function Update () {
	//=======================<MoveCamera>=======================
	
	if (Input.GetKeyDown("w")) {
		defaultCameraScript.moveUp(true);
	}
	if (Input.GetKeyDown("s")) {
		defaultCameraScript.moveDown(true);
	}
	if (Input.GetKeyDown("a")) {
		defaultCameraScript.moveLeft(true);
	}
	if (Input.GetKeyDown("d")) {
		defaultCameraScript.moveRight(true);
	}
	
	if (Input.GetKeyUp("w")) {
		defaultCameraScript.moveUp(false);
	}
	if (Input.GetKeyUp("s")) {
		defaultCameraScript.moveDown(false);
	}
	if (Input.GetKeyUp("a")) {
		defaultCameraScript.moveLeft(false);
	}
	if (Input.GetKeyUp("d")) {
		defaultCameraScript.moveRight(false);
	}
	
	//======================</MoveCamera>=======================
	
	
	//=======================<MovePlayer>=======================
	
	if (Input.GetKeyDown(KeyCode.UpArrow)) {
		spherePlayerScript.moveUp(true);
	}
	if (Input.GetKeyDown(KeyCode.DownArrow)) {
		spherePlayerScript.moveDown(true);
	}
	if (Input.GetKeyDown(KeyCode.LeftArrow)) {
		spherePlayerScript.moveLeft(true);
	}
	if (Input.GetKeyDown(KeyCode.RightArrow)) {
		spherePlayerScript.moveRight(true);
	}
	
	if (Input.GetKeyUp(KeyCode.UpArrow)) {
		spherePlayerScript.moveUp(false);
	}
	if (Input.GetKeyUp(KeyCode.DownArrow)) {
		spherePlayerScript.moveDown(false);
	}
	if (Input.GetKeyUp(KeyCode.LeftArrow)) {
		spherePlayerScript.moveLeft(false);
	}
	if (Input.GetKeyUp(KeyCode.RightArrow)) {
		spherePlayerScript.moveRight(false);
	}
	
	
	//======================</MovePlayer>=======================
	
	Debug.Log("Controls | touchCount = " + Input.touchCount);
	if (Input.touchCount > 0) {
		var touch : Vector2 = Input.GetTouch(0).position;
		spherePlayerScript.setTouchPosition( Camera.main.ScreenToWorldPoint( Vector3 (touch.x, touch.y, 0) ) );
	}
}