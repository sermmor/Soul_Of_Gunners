using UnityEngine;
using System.Collections;

public class FloorEnemyType2 : MonoBehaviour {
    private const float FACTOR_DECREASE_VELOCITY_X = 3f;
    private const float timeInAssault = 0.3f;
    private const int numberJumpUntilAssault = 3;
    
    public float lenghtSight = 100f;
    public float lenghtAttack = 5f;
    public float maxSpeed = 1.5f;
    public float speed = 200f;
    public float assaultSpeed = 10000f;
    public float power = 6;
    public float jumpPower = 20000f;
    public float currentLife = 8;
    public float addToScore = 10;
    public GameObject shadowPrefabs;
    public GameObject[] powerUpPrefabs;
    public AudioClip audioHurt;
    public AudioClip audioDie;
    
    private float currentTimeInAssault;
    private int countJumpUntilAssault;
    private bool isInAssault;
    private bool firstMovementToAssault;
    private bool isTouchFloor;
    private bool isLaunchPowerUp;
    private float widthCamera;
    private string stateEnemy;
    private GameObject shadow;
    private GameObject goPlayer;
    private Animator anim;
    private Rigidbody2D rb2d;
    private GameObject gamecamera;
    private CameraFollow cameraFollow;
    private PauseMenu pauseMenu;

	// Use this for initialization
	void Start () {
        gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraFollow = (CameraFollow) gamecamera.GetComponent("CameraFollow");
        pauseMenu = (PauseMenu) gamecamera.GetComponent("PauseMenu");
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        anim = gameObject.GetComponent<Animator>();
	    rb2d = gameObject.GetComponent<Rigidbody2D>();
        currentTimeInAssault = 0f;
        countJumpUntilAssault = 0;
        isInAssault = false;
        firstMovementToAssault = false;
        isTouchFloor = true;
        isLaunchPowerUp = false;
        //------------- Colisiones a ignorar.
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_ENEMY, true);
        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
        //------------- Creo la sombra del enemigo.
        shadow = GameObject.Instantiate<GameObject>(shadowPrefabs);
        ShadowCharacter sc = (ShadowCharacter) shadow.GetComponent("ShadowCharacter");
        sc.characterToFollow = gameObject;
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
	
    public string getState() {
        return stateEnemy;
    }

    void refleshStatus() {
        if (anim.GetBool("isAttack")) {
            stateEnemy = "isAttack";
        } else if (anim.GetBool("isIdle")) {
            stateEnemy = "isIdle";
        } else if (anim.GetBool("isWalk")) {
            stateEnemy = "isWalk";
        } else if (anim.GetBool("isJump")) {
            stateEnemy = "isJump";
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
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            // Mirar hacia la derecha.
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void moveAssault() {
        if (rb2d != null) {
            // Incrementar velocidad de andar pero controlando que no se pase de maxSpeed ni se chorre.
            Vector3 easeVelocity = new Vector3();
            easeVelocity.y = rb2d.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x = rb2d.velocity.x * FACTOR_DECREASE_VELOCITY_X;

            // Asignar velocidad con factor de decrecimiento de velocidad si no está en el aire.
            if (rb2d.velocity.y == 0) {
                rb2d.velocity = easeVelocity;
            }

            float direction = transform.localScale.x;
            rb2d.AddForce(Vector2.right * assaultSpeed * direction);

            // Controlamos que no se pase del rango [-maxSpeed, maxSpeed].
            if (rb2d.velocity.x > maxSpeed) {
                rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
            }

            if (rb2d.velocity.x < -maxSpeed)
            {
                rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);
            }
        } else if (!("isDie").Equals(stateEnemy)) {
            die();
        }
    }
    
    void assault() {
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isJump", true);
        stateEnemy = "isJump";
    }
    
    void jump() {
        float direction = transform.localScale.x;
        rb2d.AddForce(new Vector2(speed * 10 * direction, jumpPower));
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isJump", true);
        stateEnemy = "isJump";
    }
    
    bool isInAGameModeThatCanAssault() {
        // En modo fácil este enemigo no hace assault.
        return UserInterfaceGraphics.MODE_CHOOSE != 0;
    }

    void enemyIA() {
        bool isInFloor = rb2d.velocity.y == 0;
        if ((isInFloor && isTouchFloor) || isInAssault) {
            // Si estamos en el suelo, saltamos.
            if (countJumpUntilAssault < numberJumpUntilAssault) {
                // Salto.
                putLookingToPlayer();
                jump();
                isTouchFloor = false;
                if (isInAGameModeThatCanAssault()) {
                    // Sólo si estamos en un modo de juego que permite que haga asaltos, los hacemos.
                    countJumpUntilAssault++;
                }
            } else {
                // Asalto.
                if (firstMovementToAssault) {
                    putLookingToPlayer();
                    firstMovementToAssault = false;
                }
                isInAssault = true;
                currentTimeInAssault = currentTimeInAssault + Time.deltaTime;
                assault();
                if (currentTimeInAssault > timeInAssault) {
                    // Se acabó el assault.
                    rb2d.velocity = new Vector2(0, 0);
                    countJumpUntilAssault = 0;
                    currentTimeInAssault = 0f;
                    isInAssault = false;
                    firstMovementToAssault = true;
                }
            }
        } else if (isInFloor) {
            isTouchFloor = true;
        }
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
        // Destruyo la sombra.
        Destroy(shadow);
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
            if (currentLife <= 0) {
                die();
            } else {
                enemyIA();
            }
        }
	}

    bool canMoveInScreen() {
        bool ret = true;
        if (cameraFollow.isFreezeCamera()) {
            // Si la cámara está congelada comprobar si el enemigo está en los límites.
            float leftSideCameraX = gamecamera.transform.position.x - widthCamera;
            float rightSideCameraX = gamecamera.transform.position.x + widthCamera;
            float posX = transform.position.x;

            if ((leftSideCameraX > posX) && (Input.GetAxis("Horizontal") < 0)) {
                transform.position = new Vector3(leftSideCameraX, transform.position.y, transform.position.z);
                ret = false;
            } else if ((posX > rightSideCameraX) && (Input.GetAxis("Horizontal") > 0)) {
                transform.position = new Vector3(rightSideCameraX, transform.position.y, transform.position.z);
                ret = false;
            }
        }
        return ret;
    }

    void FixedUpdate() {
        if (!pauseMenu.isGameInPause()) {
            // Si estamos en asalto, moverse.
            if (isInAssault) {
                moveAssault();
            }

            // Evitar que el enemigo se salga de los límites.
            if (!canMoveInScreen() && (rb2d != null)) {
                rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
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
            case "Ground":
                if (anim.GetBool("isJump")) {
                    isTouchFloor = true;
                    rb2d.velocity = new Vector2(0, 0);
                }
                break;
        }
    }
}
