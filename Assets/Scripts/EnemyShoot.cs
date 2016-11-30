using UnityEngine;
using System.Collections;

public class EnemyShoot : MonoBehaviour {

    // Tiempo en el que acaba la rafaga, dependiendo de si es menor o mayor, acaba o no el disparo.
    public float timeEndFire = 0.15f;
    public float power = 3f;
    public FloorEnemy enemy;
    public BossGiantRobot boss;

	// Use this for initialization
	void Start () {
	    Destroy(this.gameObject, timeEndFire);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_POWER_UP, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_WALL, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_ENEMY_SHOOT, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_SHOOT, true);
	}

    void FixedUpdate() {
        if (enemy != null) {
            if (("isDead").Equals(enemy.getState())) {
                DestroyImmediate(this.gameObject);
            }
        }
        if (boss != null) {
            if (("isDead").Equals(boss.getState())) {
                DestroyImmediate(this.gameObject);
            }
        }
    }
	
	void OnCollisionEnter2D(Collision2D col) {
	    switch (col.gameObject.tag)
        {
            case "Player":
                Player p = (Player) (col.gameObject.GetComponent("Player"));
                p.damage(power);
                Destroy(this.gameObject);
                break;
        }
	}

}
