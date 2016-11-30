using UnityEngine;
using System.Collections;

public class AirEnemyType2 : MonoBehaviour {
    private const float timeChangeDirection = 9f;
    private const float timeToLaunchMine = 2f;

    public float speedX = 1.5f;
    public float power = 5;
    public float currentLife = 8;
    public float addToScore = 8;
    public GameObject minePrefab;
    public GameObject[] powerUpPrefabs;
    public AudioClip audioShoot;
    public AudioClip audioHurt;
    public AudioClip audioDie;

    private bool isLaunchPowerUp;
    private string stateEnemy;
    private Animator anim;
    private Rigidbody2D rb2d;
    private PauseMenu pauseMenu;
    private float currentTimeChangeDirection;
    private float currentTimeLaunchMine;
    private BossGiantRobot fromBoss = null;

	// Use this for initialization
	void Start () {
        pauseMenu = (PauseMenu) GameObject.FindGameObjectWithTag("MainCamera").GetComponent("PauseMenu");
        isLaunchPowerUp = false;
        currentTimeChangeDirection = 0f;
        currentTimeLaunchMine = 0f;
        anim = gameObject.GetComponent<Animator>();
	    rb2d = gameObject.GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_ENEMY_SHOOT, true);
	}

    public void createdFromBoss(BossGiantRobot boss) {
        fromBoss = boss;
    }

    public void damage(float n) {
        currentLife -= n;
        if (currentLife < 0) {
            currentLife = 0;
        } else if (audioHurt != null) {
            // Insertamos sonido de hurt.
            AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
            if (currentAS.clip != audioHurt) {
                currentAS.clip = audioHurt;
            }
            currentAS.Play();
        }
    }

    void refleshStatus() {
        if (anim.GetBool("isIdle")) {
            stateEnemy = "isIdle";
        } else if (anim.GetBool("isFly")) {
            stateEnemy = "isFly";
        } else if (anim.GetBool("isDie")) {
            stateEnemy = "isDie";
        } else {
            // Si está todo en falso porque ha entrado en un estado extraño, poner en idle.
            anim.SetBool("isIdle", true);
            stateEnemy = "isIdle";
        }

    }
	
    void idle() {
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isIdle", true);
        stateEnemy = "isIdle";
    }

    void move() {
        if (rb2d != null) {
            // Movemos la X y la Y.
            float directionX = gameObject.transform.localScale.x;
            rb2d.velocity = new Vector2(directionX * speedX, 0);
        } else if (!("isDie").Equals(stateEnemy)) {
            die();
        }
    }

    void changeDirection() {
        currentTimeChangeDirection = currentTimeChangeDirection + Time.deltaTime;
        if (currentTimeChangeDirection >= timeChangeDirection) {
            // Cambiamos de dirección.
            gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, 
                gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            currentTimeChangeDirection = 0;
        }
    }
    
    void fly() {
        // Colocar en su dirección correcta y asignar estado walk.
        changeDirection();
        move();
        anim.SetBool(stateEnemy, false);

        // Asignamos el estado para moverse hacia atrás o hacia delante.
        anim.SetBool("isFly", true);
        stateEnemy = "isFly";

    }

    void launchMine() {
        currentTimeLaunchMine = currentTimeLaunchMine + Time.deltaTime;
        if (currentTimeLaunchMine >= timeToLaunchMine) {
            // Lanzamos la mina.
            GameObject goMine = GameObject.Instantiate<GameObject>(minePrefab);
            goMine.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 
                goMine.transform.position.z);

            currentTimeLaunchMine = 0;

            // Insertamos sonido de mina.
            if (audioShoot != null) {
                AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
                if (currentAS.clip != audioShoot) {
                    currentAS.clip = audioShoot;
                }
                currentAS.Play();
            }
        }
    }

    void enemyIA() {
        fly();
        launchMine();
    }

    int probabilityPowerUpByGameMode() {
        int ret = 6; // Modo normal.
        if (UserInterfaceGraphics.MODE_CHOOSE == 0) {
            ret = 4; // Modo fácil.
        } else if (UserInterfaceGraphics.MODE_CHOOSE == 2) {
            ret = 8; // Modo difícil.
        }
        return ret;
    }

    void launchPowerUp() {
        // Soltar power-up. ALEATORIAMENTE.
        if ((!isLaunchPowerUp) && (powerUpPrefabs != null) && powerUpPrefabs.Length != 0) {
            /* Miramos aleatoriamente si soltar o no power-up. Por ejemplo con Random.Range(0, 4), la 
            probabilidad será del 25%, mientras que con Random.Range(0, 2) la probabilidad será del 50%.*/
            int p = Random.Range(0, probabilityPowerUpByGameMode());
            if (p == 0) {
                // Miramos aleatoriamente qué power-up soltar.
                int n = Random.Range(0, powerUpPrefabs.Length);
            
                GameObject puPrefab = powerUpPrefabs[n];
                Transform powerup = GameObject.Instantiate<GameObject>(puPrefab).transform;
                powerup.position = transform.position;
            }
            isLaunchPowerUp = true;
            Player.score = Player.score + addToScore;
        }
        // Destruir enemigo.
        Destroy(this.gameObject, 0.5f);
    }

    void die() {
        // Asignar estado muerte.
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isDie", true);
        stateEnemy = "isDie";
        // Destruyo BoxCollider2D y RigidBody2D, para desactivar física en este estado.
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        if (rb2d != null) {
            Destroy(rb2d);
        }
        // Marco soltar powerUp y a destruir enemigo.
        Invoke("launchPowerUp", 0.2f);

        // Activo sonido de muerte.
        if (audioDie != null) {
            // Insertamos sonido de muerte.
            AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
            if (currentAS.clip != audioDie) {
                currentAS.clip = audioDie;
                currentAS.Play();
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (!pauseMenu.isGameInPause()) {
            refleshStatus();
            if ((currentLife <= 0) || ((fromBoss != null) && ("isDie").Equals(fromBoss.getState()))) {
                die();
            } else {
                enemyIA();
            }
        }
	}

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
