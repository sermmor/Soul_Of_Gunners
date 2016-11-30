using UnityEngine;
using System.Collections;

public class AirEnemy : MonoBehaviour {
    private const float FACTOR_FROM_RENDERER_TO_POINTS = 50f;
    private const float FACTOR_DECREASE_VELOCITY = 3f;

    public float lenghtSight = 10f;
    public float speedX = 1.5f;
    public float speedY = 1.5f;
    public float power = 5;
    public float currentLife = 4;
    public float addToScore = 4;
    public GameObject[] powerUpPrefabs;
    public AudioClip audioHurt;
    public AudioClip audioDie;

    bool isLaunchPowerUp = false;
    private float lastDirectionX;
    private string stateEnemy;
    private GameObject goPlayer;
    private Player player;
    private Animator anim;
    private Rigidbody2D rb2d;
    private PauseMenu pauseMenu;
    private BossGiantRobot fromBoss = null;
    
    // Variables para controlar lo de cuando se choca contra el player, volver un rato hacia atrás.
    private const float TimeReverseMode = 1f;
    private bool reverseMode = false;
    private float currentTimeReverseMode;

	// Use this for initialization
	void Start () {
        pauseMenu = (PauseMenu) GameObject.FindGameObjectWithTag("MainCamera").GetComponent("PauseMenu");
	    goPlayer = GameObject.FindGameObjectWithTag("Player");
        player = (Player) goPlayer.GetComponent("Player");
        anim = gameObject.GetComponent<Animator>();
	    rb2d = gameObject.GetComponent<Rigidbody2D>();
        lastDirectionX = 1;
        reverseMode = false;
        currentTimeReverseMode = 0f;
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
	
    void putLookingToPlayer() {
        if (goPlayer.transform.position.x < transform.position.x) {
            // Mirar hacia la izquierda.
            if (lastDirectionX == -1) {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        } else {
            // Mirar hacia la derecha.
            if (lastDirectionX == 1) {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void idle() {
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isIdle", true);
        stateEnemy = "isIdle";
    }

    void move() {
        if (rb2d != null) {
            // Preparamos para mover la X.
            float directionX;
            Vector3 sizeEnemy = GetComponent<Renderer>().bounds.size;
            if ((player.transform.position.x + sizeEnemy.x < transform.position.x)) {
                directionX = -1;
            } else if ((player.transform.position.x - sizeEnemy.x > transform.position.x)) {
                directionX = 1;
            } else {
                // Si estamos encima del player usar dirección actual.
                directionX = lastDirectionX;
            }
            lastDirectionX = directionX;
            // Preparamos para mover la Y
            float directionY;
            if ((player.transform.position.y + sizeEnemy.y/2 < transform.position.y)) {
                directionY = -1;
            } else if ((player.transform.position.y - sizeEnemy.y/2 > transform.position.y)) {
                directionY = 1;
            } else {
                // Si estamos a la altura del player, ir hacia arriba.
                directionY = 1;
            }
        
            // Movemos la X y la Y.
            if (reverseMode) {
                // A pesar de estar en reverse mode, el enemigo debe de tratar de subir siempre.
                rb2d.velocity = new Vector2(-directionX * speedX, speedY);
            } else {
                // Si estamos en modo normal, avanzamos.
                rb2d.velocity = new Vector2(directionX * speedX, directionY * speedY);
            }
        } else if (!("isDie").Equals(stateEnemy)) {
            die();
        }
    }

    void fly() {
        // Colocar en su dirección correcta y asignar estado walk.
        move();
        putLookingToPlayer();
        anim.SetBool(stateEnemy, false);

        // Miramos si seguimos en reverseMode.
        currentTimeReverseMode = currentTimeReverseMode + Time.deltaTime;
        if (reverseMode && (currentTimeReverseMode > TimeReverseMode)) {
            reverseMode = false;
        }

        // Asignamos el estado para moverse hacia atrás o hacia delante.
        if (reverseMode) {
            anim.SetBool("isIdle", true);
            stateEnemy = "isIdle";
        } else {
            anim.SetBool("isFly", true);
            stateEnemy = "isFly";
        }

    }

    void enemyIA() {
        float distanceToPlayer = Mathf.Abs(goPlayer.transform.position.x - transform.position.x);
	    if (distanceToPlayer <= lenghtSight) {
            fly();
        } else {
            // Mantenerse en idle.
            idle();
        }
    }

    void launchPowerUp() {
        // Soltar power-up. ALEATORIAMENTE.
        if ((!isLaunchPowerUp) && (powerUpPrefabs != null) && powerUpPrefabs.Length != 0) {
            /* Miramos aleatoriamente si soltar o no power-up. Por ejemplo con Random.Range(0, 4), la 
            probabilidad será del 25%, mientras que con Random.Range(0, 2) la probabilidad será del 50%.*/
            int p = Random.Range(0, 8);
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
                reverseMode = true;
                currentTimeReverseMode = 0f;
                break;
        }
    }
}
