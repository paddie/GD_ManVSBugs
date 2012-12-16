using UnityEngine;
using System.Collections;
using Pathfinding;

public class Route : MonoBehaviour {
	
	public float lineWidth = 0.5f;
	public float floatHeight = 10f;
	private bool hidden = false;
	public Transform target;
	private Mesh mesh;
	private Material lmat;
	private Point first;
	private Vector3 s;

	void Start () {
        Seeker seeker = GetComponent<Seeker>();
        seeker.pathCallback = OnPathComplete;
		mesh = new Mesh();
		lmat = Camera.main.GetComponent<Controller>().arrowMaterial;
    }
	
	void Update() {
		if (!hidden) {
			Graphics.DrawMesh(mesh, transform.localToWorldMatrix, lmat, 0);
		}
	}
	
	public void HideMesh() {
		hidden = true;
	}
	
	public void ShowMesh() {
		hidden = false;
	}
	
	public void ClearRoute() {
		mesh.Clear();
		first = null;
		s = Vector3.zero;
	}
    
    public void OnPathComplete (Path p) {
		DrawMesh (p);
    }
	/*
	void DrawLineRenderer(Path p) {
		Vector3[] path = p.vectorPath;
        lineRenderer.material = new Material(Shader.Find("Toon/Basic"));
        lineRenderer.SetColors(Color.blue, Color.white);
        lineRenderer.SetWidth(0.8F, 0.2F);
        lineRenderer.SetVertexCount(path.Length);
		int i = 0;
		foreach (Vector3 v in path) {
			lineRenderer.SetPosition(i, v);
			i++;
		}
	}
	*/
	
	void DrawMesh(Path p) {
		mesh.Clear();
		first = null;
		s = Vector3.zero;
        Vector3[] path = p.vectorPath;
		int i = 0;
		foreach (Vector3 v in path) {
			Vector3 e = new Vector3(v.x,floatHeight,v.z);
			if(first == null) {
				first = new Point();
				first.p = transform.InverseTransformPoint(e);
			}
			if(s != Vector3.zero) {
				Vector3 ls = transform.TransformPoint(s);
				AddLine(mesh, MakeQuad(ls, e, lineWidth, (path.Length-i)/path.Length), false);

			}
			s = transform.InverseTransformPoint(e);
			i++;
		}
	}
	
	Vector3[] MakeQuad(Vector3 s, Vector3 e, float w, float scale) {
		w = w / 2;
		Vector3[] q = new Vector3[4];

		Vector3 n = Vector3.Cross(s, e);
		Vector3 l = Vector3.Cross(n, e-s);
		l.Normalize();
		
		q[0] = transform.InverseTransformPoint(s + l * w);
		q[1] = transform.InverseTransformPoint(s + l * -w);
		q[2] = transform.InverseTransformPoint(e);
		q[3] = transform.InverseTransformPoint(e);
		q[0] = new Vector3(q[0].x,floatHeight,q[0].z);
		q[1] = new Vector3(q[1].x,floatHeight,q[1].z);
		q[2] = new Vector3(q[2].x,floatHeight,q[2].z);
		q[3] = new Vector3(q[3].x,floatHeight,q[3].z);
		return q;
	}
	
	void AddLine(Mesh m, Vector3[] quad, bool tmp) {
			int vl = m.vertices.Length;
			
			Vector3[] vs = m.vertices;
			if(!tmp || vl == 0) vs = resizeVertices(vs, 6);
			else vl -= 4;
			
			vs[vl] = quad[0];
			vs[vl+1] = quad[1];
			vs[vl+2] = quad[2];
			vs[vl+3] = quad[3];

        	Vector2[] uvs = new Vector2[vs.Length];
        	int i = 0;
        	while (i < uvs.Length) {
            	uvs[i] = new Vector2(vs[i].x, vs[i].z);
            	i++;
        	}
        	

			int tl = m.triangles.Length;
			int[] ts = m.triangles;
			if(!tmp || tl == 0) ts = resizeTraingles(ts, 6);
			else tl -= 6;
			ts[tl] = vl;
			ts[tl+1] = vl+1;
			ts[tl+2] = vl+2;
			ts[tl+3] = vl+1;
			ts[tl+4] = vl+3;
			ts[tl+5] = vl+2;
			m.vertices = vs;
			m.uv = uvs;
			m.triangles = ts;
			m.RecalculateBounds();
	}
	
	Vector3[] resizeVertices(Vector3[] ovs, int ns) {
		Vector3[] nvs = new Vector3[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
	
	int[] resizeTraingles(int[] ovs, int ns) {
		int[] nvs = new int[ovs.Length + ns];
		for(int i = 0; i < ovs.Length; i++) nvs[i] = ovs[i];
		return nvs;
	}
}