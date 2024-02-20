using UnityEngine;
using System.Collections;

public class BuildingTimer : MonoBehaviour {

    public Material material;
    //private Material _material;

    public float minY = 0;
    public float maxY = 2;
    public float duration = 5;

	// Use this for initialization
	void Start () {
        // Copy the material
        //_material = new Material(material);
        //GetComponent<Renderer>().material = _material;
	}
	
	// Update is called once per frame
	void Update () {
        float y = Mathf.Lerp(minY, maxY, Time.time / duration);
        material.SetFloat("_ConstructY", y);
        //_material.SetFloat("_ConstructY", y);
    }
}
