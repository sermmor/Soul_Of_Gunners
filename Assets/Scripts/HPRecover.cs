using UnityEngine;
using System.Collections;

public class HPRecover : MonoBehaviour {

    public float pointsToCure = 20f;

    private Player player;

	// Use this for initialization
	void Start () {
	    player = (Player) GameObject.FindGameObjectWithTag("Player").GetComponent("Player");
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_POWER_UP, UserInterfaceGraphics.LAYER_SHOOT, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_POWER_UP, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_POWER_UP, UserInterfaceGraphics.LAYER_ENEMY_SHOOT, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_POWER_UP, UserInterfaceGraphics.LAYER_ENEMY_MINE, true);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnCollisionEnter2D(Collision2D col) {
        switch (col.gameObject.tag)
        {
            case "Player":
                player.cure(pointsToCure);
                Destroy(this.gameObject);
                break;
        }
    }
}
