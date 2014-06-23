#pragma strict

function Start () {

}

function Update () {

}
/*
public class Quby {
	public var body : GameObject;
	public var flag : boolean;
	public var edge : int;
	
	public function Quby(edge : int, position : Vector3, rotation : Quaternion, type : int) {
		if (type == 1) {
		
			body = GameObject.CreatePrimitive(PrimitiveType.Cube);
			
			body.transform.localScale = Vector3(0.95, 0.2, 0.95);
			body.transform.position = position;
			body.transform.rotation = rotation;
			
		} else {
		
			body = GameObject.Instantiate(GameObject.Find( "wall" + (type - 1) ), position, Quaternion.identity) as GameObject;//copy(GameObject.Find("wall1"), position);
			
			body.transform.localScale = Vector3(0.48, 1, 0.48);
			
		}
		
		this.edge = edge;
		flag = true;
	}
	
	public function move() {
		if (body.transform.position.y < 0) {
			body.transform.position.y += 0.1;
		} else {
			body.AddComponent("QubyMover");
			flag = false;
		}
	}
}

public class Level {
	var W : int;
	var H : int;
	
	var qubyArray : Quby[];
	
	private var parent : GameObject;
	
	public function Level(number : int, parent : GameObject) {
		W = number;
		H = number;
		
		this.parent = parent;
	}
	
	// edges:
	//      ---
	//     | 5 |
	//     |---|
	//     | 4 |
	//  -----------
	// | 1 | 0 | 3 |
	//  -----------
	//     | 2 |
	//      ---
	public function dravLevel() {
		var i : float;
		var j : float;
		
		var cube : Quby;
		var arrayIterator : int = 0;
		
		qubyArray = new Quby[W * H * 6];
		
		//edge 0
		i = 0 - W / 2;
		j = 0 - H / 2;
		for (i; i < W / 2; i += 1) {
			for (j; j < H / 2; j += 1) {
				cube = new Quby(0, Vector3(j, -H / 2, i), Quaterion.Euler(0, 0, 0), 1);
				cube.body.transform.parent = this.parent.transform;
				
				qubyArray[arrayIterator] = cube;
				arrayIterator++;
			}
		}
		
		//edge 1
		i = 0 - W / 2;
		j = 0 - H / 2;
		for (i; i < W / 2; i += 1) {
			for (j; j < H / 2; j += 1) {
				cube = new Quby(0, Vector3(j, i, H / 2), Quaterion.Euler(90, 0, 0), 1);
				cube.body.transform.parent = this.parent.transform;
				
				qubyArray[arrayIterator] = cube;
				arrayIterator++;
			}
		}
		
		//edge 3
		i = 0 - W / 2;
		j = 0 - H / 2;
		for (i; i < W / 2; i += 1) {
			for (j; j < H / 2; j += 1) {
				cube = new Quby(0, Vector3(j, i, -H / 2), Quaterion.Euler(90, 0, 0), 1);
				cube.body.transform.parent = this.parent.transform;
				
				qubyArray[arrayIterator] = cube;
				arrayIterator++;
			}
		}
		
		//resize and free unused space
		if (w * h * 6 != arrayIterator) System.Array.Resize.<Quby>(qubyArray, arrayIterator);
		
		return arrayIterator;
	}
}
*/