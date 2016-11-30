using UnityEngine;
using System.Collections;

public class PlayerBall : MonoBehaviour {

    public float timeEndFire = 0.025f;

	// Use this for initialization
	void Start () {
	    Destroy(this.gameObject, timeEndFire);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
