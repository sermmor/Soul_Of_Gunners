using UnityEngine;
using System.Collections;

public class BlockCamera : MonoBehaviour {

    public bool blockInLeftSide = false;

    private bool isBlockCamera;
    private bool isActive;
    private GameObject gamecamera;
    private CameraFollow cameraFollow;
    private float widthCamera;

	// Use this for initialization
	void Start () {
	    gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraFollow = (CameraFollow) gamecamera.GetComponent("CameraFollow");
        isBlockCamera = true;
        isActive = false;

        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
        
	}
	
    public bool isBlockCameraInThisBlock() {
        return isActive;
    }

    public void unblockCamera() {
        isBlockCamera = false;
        cameraFollow.setFreezeCamera(false);
    }

	// Update is called once per frame
	void Update () {
        if (isBlockCamera && !cameraFollow.isFreezeCamera()) {
            // Si la cámara ha llegado a dónde está este objeto, la congelamos en este punto.
            
            if (blockInLeftSide) {
                float leftSideCameraX = gamecamera.transform.position.x - widthCamera;
                if (leftSideCameraX >= transform.position.x) {
                    isActive = true;
                    cameraFollow.setFreezeCamera(true);
                }
            } else {
                float rightSideCameraX = gamecamera.transform.position.x + widthCamera;
                if (rightSideCameraX >= transform.position.x) {
                    isActive = true;
                    cameraFollow.setFreezeCamera(true);
                }
            }
        }
	}
}
