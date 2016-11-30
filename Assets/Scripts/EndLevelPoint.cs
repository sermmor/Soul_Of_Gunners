using UnityEngine;
using System.Collections;

public class EndLevelPoint : MonoBehaviour {
    
    private GameObject gamecamera;
    private float widthCamera;

	// Use this for initialization
	void Start () {
	    gamecamera = GameObject.FindGameObjectWithTag("MainCamera");

        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
	}
	
	// Update is called once per frame
	void Update () {
	    float rightSideCameraX = gamecamera.transform.position.x + widthCamera;
        if (rightSideCameraX >= transform.position.x) {
            // Si la cámara (por el lateral derecho) ha llegado a este punto, pasamos a la pantalla de youwin.
            Application.LoadLevel(6);
        }
	}
}
