#pragma strict

var index : int;
var points : Vector3[];

function Start () {
	index = 0;
	
	var t = (1.0 + Mathf.Sqrt(5.0)) / 2.0;

	points = new Vector3[12];
	
	points[0]  = addSphere(-1,  t,  0);
	points[1]  = addSphere( 1,  t,  0);
	points[2]  = addSphere(-1, -t,  0);
	points[3]  = addSphere( 1, -t,  0);
	points[4]  = addSphere( 0, -1,  t);
	points[5]  = addSphere( 0,  1,  t);
	points[6]  = addSphere( 0, -1, -t);
	points[7]  = addSphere( 0,  1, -t);
	points[8]  = addSphere( t,  0, -1);
	points[9]  = addSphere( t,  0,  1);
	points[10] = addSphere(-t,  0, -1);
	points[11] = addSphere(-t,  0,  1);
	
	getMiddlePoint(0, 1);
}

function Update () {

}

function addVertex(pos : Vector3) {
	var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			
	body.transform.localScale = Vector3(0.2, 0.2, 0.2);
	body.transform.position = pos;
}

function addSphere(x : float, y : float, z : float) : Vector3 {
	var length : float = Mathf.Sqrt(x * x + y * y + z * z);
	var pos : Vector3 = new Vector3(x / length, y / length, z / length);
	
	addVertex(pos);
	index++;
	
	return pos;
}

function createFaces() {
	var faces : TriangleIndices[] = new TriangleIndices[20];

	// 5 faces around point 0
	faces[0] = new TriangleIndices(0, 11, 5 );
	faces[1] = new TriangleIndices(0, 5,  1 );
	faces[2] = new TriangleIndices(0, 1,  7 );
	faces[3] = new TriangleIndices(0, 7,  10);
	faces[4] = new TriangleIndices(0, 10, 11);

	// 5 adjacent faces 
	faces[5] = new TriangleIndices(1,  5,  9);
	faces[6] = new TriangleIndices(5,  11, 4);
	faces[7] = new TriangleIndices(11, 10, 2);
	faces[8] = new TriangleIndices(10, 7,  6);
	faces[9] = new TriangleIndices(7,  1,  8);

	// 5 faces around point 3
	faces[10] = new TriangleIndices(3, 9, 4);
	faces[11] = new TriangleIndices(3, 4, 2);
	faces[12] = new TriangleIndices(3, 2, 6);
	faces[13] = new TriangleIndices(3, 6, 8);
	faces[14] = new TriangleIndices(3, 8, 9);

	// 5 adjacent faces 
	faces[15] = new TriangleIndices(4, 9, 5 );
	faces[16] = new TriangleIndices(2, 4, 11);
	faces[17] = new TriangleIndices(6, 2, 10);
	faces[18] = new TriangleIndices(8, 6, 7 );
	faces[19] = new TriangleIndices(9, 8, 1 );
}

function getMiddlePoint(p1 : int, p2 : int) : Vector3 {
	var point1 : Vector3 = points[p1];
	var point2 : Vector3 = points[p2];
	
	var mx : float = (point1.x + point2.x) / 2.0;
	var my : float = (point1.y + point2.y) / 2.0; 
	var mz : float = (point1.z + point2.z) / 2.0;

	//makes sure point is on unit sphere
	return addSphere(mx, my, mz);
}

public class TriangleIndices {
	public var v1 : int;
	public var v2 : int;
	public var v3 : int;

	public function TriangleIndices(v1 : int, v2 : int, v3 : int) {
	    this.v1 = v1;
	    this.v2 = v2;
	    this.v3 = v3;
	}
}

/*
// refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            var faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2);
                int b = getMiddlePoint(tri.v2, tri.v3);
                int c = getMiddlePoint(tri.v3, tri.v1);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        // done, now add triangles to mesh
        foreach (var tri in faces)
        {
            this.geometry.TriangleIndices.Add(tri.v1);
            this.geometry.TriangleIndices.Add(tri.v2);
            this.geometry.TriangleIndices.Add(tri.v3);
        }

        return this.geometry;     
*/