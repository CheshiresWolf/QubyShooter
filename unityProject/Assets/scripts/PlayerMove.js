#pragma strict

var spherePlayer : GameObject;
var levelObj : LoadFile;

var up : boolean;
var down : boolean;
var left : boolean;
var right : boolean;

var step : float;
var playerRadius : float;

var touchPosition : Vector3;

function Start () {
	spherePlayer = GameObject.Find("SpherePlayer");
	levelObj = (GameObject.Find("LevelsStore")).GetComponent("LoadFile");
	
	touchPosition = spherePlayer.transform.position;
	
	step = 0.1;
	playerRadius = 0.5;
	
	up = false;
	down = false;
	left = false;
	right = false;
}

function Update () {
	var pos : Vector3 = spherePlayer.transform.position;
	
	if (up) {
		if ( levelObj.isQubyAtPos(pos.x + playerRadius, pos.z) ) pos.x += step;
	}
	if (down) {
		if ( levelObj.isQubyAtPos(pos.x - playerRadius, pos.z) ) pos.x -= step;
	}
	if (left) {
		if ( levelObj.isQubyAtPos(pos.x, pos.z + playerRadius) ) pos.z += step;
	}
	if (right) {
		if ( levelObj.isQubyAtPos(pos.x, pos.z - playerRadius) ) pos.z -= step;
	}
	/*
	
	if (distanseToTouch(1) > 1) {
		if (touchPosition.x > pos.x) pos.x += step;
		if (touchPosition.x < pos.x) pos.x -= step;
		if (touchPosition.z > pos.z) pos.z += step;
		if (touchPosition.z < pos.z) pos.z -= step;
	}
	*/
	spherePlayer.transform.position = pos;
}

public function moveUp(flag : boolean) {
	up = flag;
}

public function moveDown(flag : boolean) {
	down = flag;
}

public function moveLeft(flag : boolean) {
	left = flag;
}

public function moveRight(flag : boolean) {
	right = flag;
}

public function setTouchPosition(pos : Vector3) {
	touchPosition = pos;
	Debug.Log("PlayerMove | setTouchPosition | pos = " + pos);
}

function distanseToTouch(pow : int) : float {
	var n : float = Mathf.Pow(10.0, pow);
	
	return Mathf.Round( Mathf.Sqrt(
		Mathf.Pow(touchPosition.x - spherePlayer.transform.position.x, 2) + Mathf.Pow(touchPosition.z - spherePlayer.transform.position.z, 2)
	) * n ) / n;
}