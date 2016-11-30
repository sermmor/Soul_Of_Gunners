using UnityEngine;
using System.Collections;

public class ReturnToTutorialIndex : MonoBehaviour {

    private GameObject goIndexTutorial;
    
	void Start () {
	    goIndexTutorial = GameObject.FindGameObjectWithTag("IndexTutorial");
        goIndexTutorial.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            goIndexTutorial.SetActive(true);
            Destroy(gameObject);
        }
	}
}
