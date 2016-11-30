using UnityEngine;
using System.Collections;

public class EnemyMine : MonoBehaviour {

    public float power = 5f;

	// Use this for initialization
	void Start () {
	    Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_MINE, UserInterfaceGraphics.LAYER_POWER_UP, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_MINE, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_MINE, UserInterfaceGraphics.LAYER_ENEMY_SHOOT, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_MINE, UserInterfaceGraphics.LAYER_SHOOT, true);
	}
	
	void OnCollisionEnter2D(Collision2D col) {
	    switch (col.gameObject.tag)
        {
            case "Player":
                Player p = (Player) (col.gameObject.GetComponent("Player"));
                p.damage(power);
                Destroy(this.gameObject);
                break;
            case "Ground":
                //gameObject.GetComponent<Renderer>().enabled = false;
                Destroy(this.gameObject);
                break;
        }
	}
}
