using UnityEngine;
using System.Collections;

public class ShadowCharacter : MonoBehaviour {

    public GameObject characterToFollow;
    public float offsetX = 0;

    private float posY;
    private float posZ;

	// Use this for initialization
	void Start () {
        posY = transform.position.y;
        posZ = transform.position.z;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
        if (characterToFollow != null) {
            // La sombra "mira" siempre en la misma dirección que el player.
            if (characterToFollow.transform.localScale.x != transform.localScale.x) {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            // Posición de la sombra (sólo la X, la Y la bloqueo).
            float posXShadow = characterToFollow.transform.position.x + offsetX;
            if (transform.localScale.x < 0) {
                posXShadow = characterToFollow.transform.position.x - offsetX;
            }
            transform.position = new Vector3(posXShadow, posY, posZ);
        } else {
            // El enemigo o player ha muerto, destruir la sombra.
            DestroyImmediate(this.gameObject);
        }
	}
}
