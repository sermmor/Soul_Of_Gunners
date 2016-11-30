using UnityEngine;
using System.Collections;

public class ParallaxScrollElement : MonoBehaviour {

    public bool isFar = false;
    public bool isMiddle = false;
    public bool isNear = true;

    // Contra mayor sea el offset de una capa, menor será su desplazamiento.
    private float offset;
    private float firstCameraPosX;
    
    private Player player;
    private GameObject goPlayer;
    private CameraFollow cameraFollow;
    private float firstThisPosX;

    void Start() {
        float farOffset = 10f;
        float middleOffset = 5f;
        float nearOffset = 2f;

        firstThisPosX = gameObject.transform.position.x;
        firstCameraPosX = Camera.main.transform.position.x;
        cameraFollow = (CameraFollow) GameObject.FindGameObjectWithTag("MainCamera").GetComponent("CameraFollow");
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        player = (Player) GameObject.FindGameObjectWithTag("Player").GetComponent("Player");
        if (isFar) {
            offset = farOffset;
        } else if (isMiddle) {
            offset = middleOffset;
        } else if (isNear) {
           offset = nearOffset;
        }
    }

    void Update() {
        if (!cameraFollow.isFreezeCamera()) {
            float playerPosX = goPlayer.transform.position.x;
            // La clave de esto es: ¿a qué distancía me encuentro de la cámara?
            if (!player.isPlayerLookingToRight()) {
                float posXStickInCamara = Camera.main.transform.position.x - (firstCameraPosX - firstThisPosX);
                float backFactor = playerPosX/offset;
                transform.position = new Vector3(posXStickInCamara - backFactor, 
                    transform.position.y, transform.position.z);
            } else {
                float posXStickInCamara = Camera.main.transform.position.x + (firstThisPosX - firstCameraPosX);
                float backFactor = playerPosX/offset;
                transform.position = new Vector3(posXStickInCamara - backFactor, 
                    transform.position.y, transform.position.z);
            }
        }
    }

}
