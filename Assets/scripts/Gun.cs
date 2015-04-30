using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

	public GameObject camera;
	public GameObject icosaedron;
	public GameObject player;

	public GameObject pointer;

	private float radius;

	class Bullet {
		public float step = 1.0f;

		public Vector3 destination;
		public GameObject body;

		public Bullet(Vector3 start, Vector3 end) {
			this.body = drawSphere(start);
			this.destination = end;
		}

		public void move() {
			float dist = Vector3.Distance(this.body.transform.position, this.destination);

			if (dist > this.step) {
				this.body.transform.position = new Vector3(
					this.body.transform.position.x + (this.destination.x - this.body.transform.position.x) * this.step / dist,
					this.body.transform.position.y + (this.destination.y - this.body.transform.position.y) * this.step / dist,
					this.body.transform.position.z + (this.destination.z - this.body.transform.position.z) * this.step / dist
				);
			} else {
				//destroy
			}
		}

		GameObject drawSphere(Vector3 pos) {
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			
			sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			sphere.transform.position = pos;

			return sphere;
		}
	}

	void shoot() {
		Bullet bullet = new Bullet(
			camera.transform.position + camera.transform.up,
			collision(
				Vector3.zero,
				radius,
				camera.transform.position,
				camera.transform.position + camera.transform.forward
			)
		);
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
			shoot();
		}
	}
}
