using UnityEngine;
using System.Collections;

public class BossGiantRobotHands : MonoBehaviour {

    public float power = 10f;
    	
	// Colisiones contra el enemigo.
    void OnCollisionEnter2D(Collision2D col) {
        switch (col.gameObject.tag)
        {
            case "Player":
                ((Player) (col.gameObject.GetComponent("Player"))).damage(power);
                break;
        }
    }
}
