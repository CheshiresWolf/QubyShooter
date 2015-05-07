using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Gun : MonoBehaviour {

	public GameObject camera;
	public GameObject icosaedron;
	public GameObject player;

	public GameObject pointer;

	private float radius;

	private List<Bullet> bulletStore = new List<Bullet>();
	private List<Bullet> newBulletStore;

	private List<Shards> shardsStore = new List<Shards>();
	private List<Shards> newShardsStore;

	class Bullet {
		public float step = 1.0f;

		public Vector3 start;
		public Vector3 destination;
		public GameObject body;

		private int traectoryMode = 0;

		public Bullet(Vector3 start, Vector3 end, GameObject parent, int traectoryMode) {
			this.start = start;
			this.destination = end;
			this.body = drawSphere(start, parent);

			this.traectoryMode = traectoryMode;
		}

		public bool move() {
			float dist = Vector3.Distance(this.body.transform.position, this.destination);

			if (dist > this.step) {
				if (traectoryMode == 0) {
					this.body.transform.position = linearTraectory(this.body.transform.position, this.destination, dist);
				} else {
					this.body.transform.position = parabolicTraectory(this.body.transform.position, this.destination, dist);
				}

				return true;
			} else {
				Destroy(this.body);

				return false;
			}
		}

		private Vector3 linearTraectory(Vector3 bodyP, Vector3 destP, float distance) {
			return new Vector3(
				bodyP.x + (destP.x - bodyP.x) * this.step / distance,
				bodyP.y + (destP.y - bodyP.y) * this.step / distance,
				bodyP.z + (destP.z - bodyP.z) * this.step / distance
			);
		}

		private Vector3 parabolicTraectory(Vector3 bodyP, Vector3 destP, float distance) {
			float fullDistance = Vector3.Distance(this.start, this.destination);

			Vector3 stepForvard = new Vector3(
				bodyP.x + (destP.x - bodyP.x) * this.step / distance,
				bodyP.y + (destP.y - bodyP.y) * this.step / distance,
				bodyP.z + (destP.z - bodyP.z) * this.step / distance
			);

			Debug.Log("Gun | parabolicTraectory | distance : " + distance + "; fullDistance : " + fullDistance);

			if (distance > fullDistance / 2) {
				return stepCloser(Vector3.zero, stepForvard, 0.1f);
			} else {
				return stepForvard;//stepCloser(stepForvard, Vector3.zero, -0.1f);
			}
		}

		private Vector3 stepCloser(Vector3 start, Vector3 end, float step) {
			float distance = Vector3.Distance(start, end);

			return new Vector3(
				start.x + (end.x * (distance - step) / distance),
				start.y + (end.y * (distance - step) / distance),
				start.z + (end.z * (distance - step) / distance)
			);
		}
	}

	class Shards {
		public float step   = 0.4f;
		public float radius = 5.0f;

		public Vector3 start;

		private GameObject[] pieces = new GameObject[26];//6

		public Shards(Vector3 start, GameObject parent) {
			this.start  = start;

			this.pieces[0] = drawSphere(start + new Vector3( 1.0f,  0.0f,  0.0f), parent);
			this.pieces[1] = drawSphere(start + new Vector3( 0.0f,  1.0f,  0.0f), parent);
			this.pieces[2] = drawSphere(start + new Vector3( 0.0f,  0.0f,  1.0f), parent);
			this.pieces[3] = drawSphere(start + new Vector3(-1.0f,  0.0f,  0.0f), parent);
			this.pieces[4] = drawSphere(start + new Vector3( 0.0f, -1.0f,  0.0f), parent);
			this.pieces[5] = drawSphere(start + new Vector3( 0.0f,  0.0f, -1.0f), parent);

			//because i can

			this.pieces[6]  = drawSphere(start + new Vector3( 1.0f,  1.0f,  0.0f), parent);
			this.pieces[7]  = drawSphere(start + new Vector3( 0.0f,  1.0f,  1.0f), parent);
			this.pieces[8]  = drawSphere(start + new Vector3( 1.0f,  0.0f,  1.0f), parent);
			this.pieces[9]  = drawSphere(start + new Vector3( 1.0f,  1.0f,  1.0f), parent);

			this.pieces[10] = drawSphere(start + new Vector3(-1.0f, -1.0f,  0.0f), parent);
			this.pieces[11] = drawSphere(start + new Vector3( 0.0f, -1.0f, -1.0f), parent);
			this.pieces[12] = drawSphere(start + new Vector3(-1.0f,  0.0f, -1.0f), parent);
			this.pieces[13] = drawSphere(start + new Vector3(-1.0f, -1.0f, -1.0f), parent);

			this.pieces[14] = drawSphere(start + new Vector3(-1.0f,  1.0f,  0.0f), parent);
			this.pieces[15] = drawSphere(start + new Vector3( 1.0f, -1.0f,  0.0f), parent);
			this.pieces[16] = drawSphere(start + new Vector3( 0.0f, -1.0f,  1.0f), parent);
			this.pieces[17] = drawSphere(start + new Vector3( 0.0f,  1.0f, -1.0f), parent);
			this.pieces[18] = drawSphere(start + new Vector3(-1.0f,  0.0f,  1.0f), parent);
			this.pieces[19] = drawSphere(start + new Vector3( 1.0f,  0.0f, -1.0f), parent);

			this.pieces[20] = drawSphere(start + new Vector3(-1.0f,  1.0f,  1.0f), parent);
			this.pieces[21] = drawSphere(start + new Vector3( 1.0f, -1.0f,  1.0f), parent);
			this.pieces[22] = drawSphere(start + new Vector3( 1.0f,  1.0f, -1.0f), parent);
			this.pieces[23] = drawSphere(start + new Vector3(-1.0f, -1.0f,  1.0f), parent);
			this.pieces[24] = drawSphere(start + new Vector3(-1.0f,  1.0f, -1.0f), parent);
			this.pieces[25] = drawSphere(start + new Vector3( 1.0f, -1.0f, -1.0f), parent);

			foreach (GameObject piece in this.pieces) {
				setColor(piece, Color.red);
			}
		}

		public bool move() {
			foreach (GameObject piece in this.pieces) {
				float dist = Vector3.Distance(start, piece.transform.position);

				if (dist < this.radius) {
					piece.transform.position = new Vector3(
						this.start.x + (piece.transform.position.x - this.start.x) * (1 + this.step / dist),
						this.start.y + (piece.transform.position.y - this.start.y) * (1 + this.step / dist),
						this.start.z + (piece.transform.position.z - this.start.z) * (1 + this.step / dist)
					);
				} else {
					return false;
				}
			}

			return true;
		}

		public void clean() {
			foreach (GameObject piece in this.pieces) {
				Destroy(piece);
			}
		}

		void setColor(GameObject body, Color color) {
			MeshRenderer gameObjectRenderer = body.GetComponent("MeshRenderer") as MeshRenderer;
			gameObjectRenderer.material.color = color;
		}
	}

	public static GameObject drawSphere(Vector3 pos, GameObject parent) {
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		
		sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		sphere.transform.position = pos;
		sphere.transform.SetParent(parent.transform);

		return sphere;
	}

	void shoot(int mode) {
		bulletStore.Add(new Bullet(
			camera.transform.position + camera.transform.up,
			collision(
				Vector3.zero,
				radius,
				camera.transform.position,
				camera.transform.position + camera.transform.forward
			),
			pointer,
			mode
		));
	}

	//пересечение шара и прямой
	//centre_sphere - центр сферы
	//radius_sphere - радиус сферы 
	//line_p0 и line_p1 - точки прямой
	//p0 и p1 - точки пересечения ( причём p0 всегда по направлению line_p1 - line_p0 )
	Vector3 collision(Vector3 centre_sphere, float radius_sphere, Vector3 line_p0, Vector3 line_p1) {
		Vector3 p0 = Vector3.zero;
		//парамметрическое ур-е прямой
		//p = line_p0 + Dir * t 
		//x = line_p0.x + Dir.x * t
		//...
		//получаю направление
		Vector3 dir = line_p1 - line_p0;

		//ур-е сферы
		//( x - x0 ) ^ 2 + ( y - y0 ) ^ 2 + ( z - z0 ) ^ 2 = R ^ 2
		//( line_p0.x + Dir.x * t - centre_sphere.x ) ^ 2 + ... = R ^ 2  
		//замена констант
		//VecConst.x = line_p0.x - centre_sphere.x
		//...
		Vector3 VecConst = new Vector3(
			line_p0.x - centre_sphere.x,
			line_p0.y - centre_sphere.y,
			line_p0.z - centre_sphere.z
		);

		//( VecConst.x + Dir.x * t ) ^ 2 + ... = R ^ 2
		//раскрывая скобки получаем квадратное ур-е вида A * t ^ 2 + B * t + C = 0

		float A = dir.sqrMagnitude;
		float B = 2.0f * (VecConst.x * dir.x + VecConst.y * dir.y + VecConst.z * dir.z);
		float C = VecConst.sqrMagnitude - Mathf.Pow(radius_sphere, 2.0f);

		//дискриминант
		float D = Mathf.Pow(B, 2.0f) - 4.0f * A * C;

		//if (D < 0.0f) return false;

		//корни ур-я
		float T0 = (-B + Mathf.Sqrt(D)) / (2.0f * A);
		float T1 = (-B - Mathf.Sqrt(D)) / (2.0f * A);

		//вычисление точек
		if ( T0 > T1 ) {
			p0 = line_p0 + dir * T0 ;
			//p1 = line_p0 + dir * T1 ;
		} else {
			p0 = line_p0 + dir * T1 ;
			//p1 = line_p0 + dir * T0 ;
		}

		return p0;
	}

	// Use this for initialization
	void Start () {
		radius = icosaedron.GetComponent<Generator>().radius;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F)) {
			shoot(0);
		}
		if (Input.GetKeyDown(KeyCode.G)) {
			shoot(1);
		}


		if (bulletStore.Count > 0) {
			newBulletStore = new List<Bullet>();
			foreach (Bullet buf in bulletStore) {
				if (buf.move()) {
					newBulletStore.Add(buf);
				} else {
					shardsStore.Add(new Shards(
						buf.destination,
						pointer
					));
				}
			}
			bulletStore = newBulletStore;
		}

		if (shardsStore.Count > 0) {
			newShardsStore = new List<Shards>();
			foreach (Shards buf in shardsStore) {
				if (buf.move()) {
					newShardsStore.Add(buf);
				} else {
					buf.clean();
				}
			}
			shardsStore = newShardsStore;
		}
	}
}
