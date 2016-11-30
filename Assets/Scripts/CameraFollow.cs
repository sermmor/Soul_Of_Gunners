using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    private Vector2 velocity = new Vector2();

    // Retrasos al seguir a la cámara para crear un efecto bonito.
    public float smoothTimeY;
    public float smoothTimeX;
    public bool bounds;
    public Vector3 minCameraPos;
    public Vector3 maxCameraPos;
    public GameObject player;
    
    private bool freezeCamera;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        freezeCamera = false;
    }

    public void setFreezeCamera(bool f) {
        freezeCamera = f;
    }

    public bool isFreezeCamera() {
        return freezeCamera;
    }

    void FixedUpdate()
    {
        if (!freezeCamera) {
            // Hacemos que la cámara siga al player.
            float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

            transform.position = new Vector3(posX, posY, transform.position.z);

            // Bordes por los que no se debe pasar la cámara (aunque siga al player no hay que ver los límites de la plataforma).
            if (bounds) {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x),
                    Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y),
                    Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
            }
        }
    }

    public void SetMinCamPosition() {
        minCameraPos = gameObject.transform.position;
    }

    public void SetMaxCamPosition() {
        maxCameraPos = gameObject.transform.position;
    }
}
