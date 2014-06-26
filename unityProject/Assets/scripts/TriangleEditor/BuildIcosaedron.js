#pragma strict

var faces : TriangleIndices[];
var radius : float;

var parent : GameObject;

function Start () {
	parent = GameObject.Find("Isocaedron");
	radius = 4.0;
	
	createFaces();
	
	subdivide();
	subdivide();
	subdivide();
	subdivide();
		
	drawFaces();
	
	//placeTriangles();
}

function Update () {

}

function getFaceCenter(faceIndex : int) : Vector3 {
	var face : TriangleIndices = faces[faceIndex];
	
	var mp : Vector3 = getMiddlePoint(face.v1, face.v2);
	
	return new Vector3(
		mp.x + (face.v3.x - mp.x) / 3,
		mp.y + (face.v3.y - mp.y) / 3,
		mp.z + (face.v3.z - mp.z) / 3
	);
}

function placeOnSphere(vectorIn : Vector3) : Vector3 {
	return placeOnSphere(vectorIn.x, vectorIn.y, vectorIn.z);
}

function placeOnSphere(x : float, y : float, z : float) : Vector3 {
	var length : float = Mathf.Sqrt(x * x + y * y + z * z);
	var pos : Vector3 = new Vector3(x * radius / length, y * radius / length, z * radius / length);
	
	return pos;
}

function createFaces() {
	var t = (1.0 + Mathf.Sqrt(5.0)) / 2.0;

	var points = new Vector3[12];
	
	points[0]  = placeOnSphere(-1,  t,  0);
	points[1]  = placeOnSphere( 1,  t,  0);
	points[2]  = placeOnSphere(-1, -t,  0);
	points[3]  = placeOnSphere( 1, -t,  0);
	points[4]  = placeOnSphere( 0, -1,  t);
	points[5]  = placeOnSphere( 0,  1,  t);
	points[6]  = placeOnSphere( 0, -1, -t);
	points[7]  = placeOnSphere( 0,  1, -t);
	points[8]  = placeOnSphere( t,  0, -1);
	points[9]  = placeOnSphere( t,  0,  1);
	points[10] = placeOnSphere(-t,  0, -1);
	points[11] = placeOnSphere(-t,  0,  1);
	
	faces = new TriangleIndices[20];

	// 5 faces around point 0
	faces[0] = new TriangleIndices(0, 11, 5 , points);
	faces[1] = new TriangleIndices(0, 5,  1 , points);
	faces[2] = new TriangleIndices(0, 1,  7 , points);
	faces[3] = new TriangleIndices(0, 7,  10, points);
	faces[4] = new TriangleIndices(0, 10, 11, points);

	// 5 adjacent faces 
	faces[5] = new TriangleIndices(1,  5,  9, points);
	faces[6] = new TriangleIndices(5,  11, 4, points);
	faces[7] = new TriangleIndices(11, 10, 2, points);
	faces[8] = new TriangleIndices(10, 7,  6, points);
	faces[9] = new TriangleIndices(7,  1,  8, points);

	// 5 faces around point 3
	faces[10] = new TriangleIndices(3, 9, 4, points);
	faces[11] = new TriangleIndices(3, 4, 2, points);
	faces[12] = new TriangleIndices(3, 2, 6, points);
	faces[13] = new TriangleIndices(3, 6, 8, points);
	faces[14] = new TriangleIndices(3, 8, 9, points);

	// 5 adjacent faces 
	faces[15] = new TriangleIndices(4, 9, 5 , points);
	faces[16] = new TriangleIndices(2, 4, 11, points);
	faces[17] = new TriangleIndices(6, 2, 10, points);
	faces[18] = new TriangleIndices(8, 6, 7 , points);
	faces[19] = new TriangleIndices(9, 8, 1 , points);
}

function getMiddlePoint(p1 : Vector3, p2 : Vector3) : Vector3 {		
	return new Vector3(
		(p1.x + p2.x) / 2.0,
		(p1.y + p2.y) / 2.0,
		(p1.z + p2.z) / 2.0
	);
}

function setColor(body : GameObject, color : Color) {
	var gameObjectRenderer : MeshRenderer = body.GetComponent("MeshRenderer");
	gameObjectRenderer.material.color = color;
}

function drawSphere(pos : Vector3) : GameObject {
	var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			
	body.transform.localScale = Vector3(0.1, 0.1, 0.1);
	body.transform.position = pos;
	body.transform.parent = parent.transform;
	
	return body;
}

function drawFaces() {
	for (var i : int = 0; i < faces.length; i++) {
		drawSphere(faces[i].v1);
		drawSphere(faces[i].v2);
		drawSphere(faces[i].v3);
	}
}

function placeTriangles() {

	for (var i : int = 0; i < faces.length; i++) {
		setColor(drawSphere(getFaceCenter(i)), Color.red);
	}
}

function subdivide() {
    var faces2 = new TriangleIndices[faces.length * 4];
    
    for (var i : int = 0; i < faces.length; i++) {
        var a : Vector3 = placeOnSphere(getMiddlePoint(faces[i].v1, faces[i].v2));
        var b : Vector3 = placeOnSphere(getMiddlePoint(faces[i].v2, faces[i].v3));
        var c : Vector3 = placeOnSphere(getMiddlePoint(faces[i].v3, faces[i].v1));

        // replace triangle by 4 triangles
        faces2[i * 4 + 0] = new TriangleIndices(faces[i].v1, a, c);	
        faces2[i * 4 + 1] = new TriangleIndices(faces[i].v2, b, a);
        faces2[i * 4 + 2] = new TriangleIndices(faces[i].v3, c, b);
        faces2[i * 4 + 3] = new TriangleIndices(		  a, b, c);
    }
    
    faces = faces2;    
}

public class TriangleIndices {
	public var v1 : Vector3;
	public var v2 : Vector3;
	public var v3 : Vector3;

	public function TriangleIndices(v1 : Vector3, v2 : Vector3, v3 : Vector3) {
	    this.v1 = v1;
	    this.v2 = v2;
	    this.v3 = v3;
	}
	
	public function TriangleIndices(v1 : int, v2 : int, v3 : int, points : Vector3[]) {
	    this.v1 = points[v1];
	    this.v2 = points[v2];
	    this.v3 = points[v3];
	}
}