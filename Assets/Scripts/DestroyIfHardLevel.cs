using UnityEngine;
using System.Collections;

public class DestroyIfHardLevel : MonoBehaviour {

	void Start () {
        if (UserInterfaceGraphics.MODE_CHOOSE == 2) {
            Destroy(gameObject);
        }
	}
}
