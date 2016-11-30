using UnityEngine;
using System.Collections;

public class PlayerShoot : MonoBehaviour {

    // Tiempo en el que acaba la rafaga, dependiendo de si es menor o mayor, acaba o no el disparo.
    public float timeEndFire = 0.15f;//0.2f;
    public float power = 2f;

	// Use this for initialization
	void Start () {
	    Destroy(this.gameObject, timeEndFire);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_SHOOT, UserInterfaceGraphics.LAYER_POWER_UP, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_SHOOT, UserInterfaceGraphics.LAYER_WALL, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_SHOOT, UserInterfaceGraphics.LAYER_PLAYER, true);
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
    
    void resistanceAgainstBulletImpulse(GameObject c) {
        // Corregir impulso bala.
        Rigidbody2D rb2d = c.GetComponent<Rigidbody2D>();
        if (rb2d != null) {
            if (c.gameObject.transform.position.x >= 0) {
                rb2d.AddForce(Vector2.left * 1000f);
            } else {
                rb2d.AddForce(Vector2.right * 1000f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        switch (col.gameObject.tag) {
            case "FloorEnemy":
                // Quitar vida al enemigo.
                FloorEnemy c = (FloorEnemy) col.gameObject.GetComponent("FloorEnemy");
                if (c == null) {
                    // Si no se trata del FloorEnemy, con toda seguridad se trata del FloorEnemyType2.
                    FloorEnemyType2 ct2 = (FloorEnemyType2) col.gameObject.GetComponent("FloorEnemyType2");
                    ct2.damage(power);
                    resistanceAgainstBulletImpulse(ct2.gameObject);
                } else {
                    c.damage(power);
                    resistanceAgainstBulletImpulse(c.gameObject);
                }
                // Eliminar bala.
                Destroy(this.gameObject);
                break;
            case "Enemy":
                AirEnemy c1 =(AirEnemy) col.gameObject.GetComponent("AirEnemy");
                if (c1 == null) {
                    // Si no se trata del AirEnemy, con toda seguridad se trata del AirEnemyType2.
                    AirEnemyType2 c1t2 = (AirEnemyType2) col.gameObject.GetComponent("AirEnemyType2");
                    c1t2.damage(power);
                } else {
                    c1.damage(power);
                }
                Destroy(this.gameObject);
                break;
            case "Boss":
                BossGiantRobot b = (BossGiantRobot) col.gameObject.GetComponent("BossGiantRobot");
                b.damage(power);
                Destroy(this.gameObject);
                break;
        }
    }

}
