using UnityEngine;
using System.Collections;

public class FloorEnemy : MonoBehaviour {
    private const float FACTOR_DECREASE_VELOCITY_X = 3f;
    private const float TIME_TO_ATTACK = 1.5f;
    private const float TIME_IN_ATTACK = 0.4f; // 0.7f;
    private const float marginBulletX = 1.09f;//0.24f;
    private const float marginBulletY = 0.15f;//0.09f;
    private const float bulletVelocityX = 10;
    
    public float lenghtSight = 10f;
    public float lenghtAttack = 3f;
    public float maxSpeed = 1.5f;
    public float speed = 200f;
    public float power = 3;
    public float jumpPower = 200f;
    public float currentLife = 4;
    public float addToScore = 2;
    public GameObject shootPrefab;
    public GameObject shadowPrefabs;
    public GameObject[] powerUpPrefabs;
    public AudioClip audioShoot;
    public AudioClip audioHurt;
    public AudioClip audioDie;
    
    private bool isLaunchPowerUp = false;
    private bool isLookingToRight;
    private bool isInAttackMode;
    private float timeToAttack;
    private float timeInAttack;
    private float widthCamera;
    private string stateEnemy;
    private GameObject shadow;
    private GameObject goPlayer;
    private Player player;
    private Animator anim;
    private Rigidbody2D rb2d;
    private GameObject gamecamera;
    private CameraFollow cameraFollow;
    private PauseMenu pauseMenu;
    private BossGiantRobot fromBoss = null;

	// Use this for initialization
	void Start () {
        gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraFollow = (CameraFollow) gamecamera.GetComponent("CameraFollow");
        pauseMenu = (PauseMenu) gamecamera.GetComponent("PauseMenu");
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        player = (Player) goPlayer.GetComponent("Player");
        anim = gameObject.GetComponent<Animator>();
	    rb2d = gameObject.GetComponent<Rigidbody2D>();
        isLookingToRight = true;
        isInAttackMode = false;
        timeToAttack = 0;
        timeInAttack = TIME_IN_ATTACK;
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
            isLookingToRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            // Mirar hacia la derecha.
            isLookingToRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void idle() {
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isIdle", true);
        stateEnemy = "isIdle";
    }

    void move() {
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

            float direction = (player.transform.position.x < transform.position.x) ? -1 : 1;
            rb2d.AddForce(Vector2.right * speed * direction);

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

    void walk() {
        // Colocar en su dirección correcta y asignar estado walk.
        putLookingToPlayer();
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isWalk", true);
        stateEnemy = "isWalk";
        move();
    }
    
    void launchProyectile() {
        if (currentLife > 0) {
            // Disparamos las líneas.
            Vector2 direction = transform.position + new Vector3(marginBulletX, marginBulletY, gameObject.transform.position.z);
        
            GameObject goBullet = GameObject.Instantiate<GameObject>(shootPrefab);
            EnemyShoot classBullet = (EnemyShoot) goBullet.GetComponent("EnemyShoot");
            classBullet.enemy = this;
            // Posición.
            Transform bullet = goBullet.transform;
            if (isLookingToRight) {
                bullet.position = new Vector3 (direction.x , direction.y, gameObject.transform.position.z);
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(bulletVelocityX, 0), ForceMode2D.Impulse);
            } else {
                bullet.position = new Vector3(transform.position.x - marginBulletX, direction.y, gameObject.transform.position.z);
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-bulletVelocityX, 0), ForceMode2D.Impulse);
                bullet.localScale = new Vector3(-bullet.localScale.x, bullet.localScale.y, bullet.localScale.x);
            }
            // Insertamos sonido de disparo.
            if (audioShoot != null) {
                AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
                if (currentAS.clip != audioShoot) {
                    currentAS.clip = audioShoot;
                }
                currentAS.Play();
            }
        }
    }

    
    private float countThreeToAttack = 3;

    void attack() {
        if (currentLife > 0) {
            putLookingToPlayer();

            if (anim.GetBool("isAttack")) {
                if (countThreeToAttack == 0) {
                    timeInAttack = timeInAttack + Time.deltaTime;
                    if (timeInAttack < TIME_IN_ATTACK) {
                        // Mientras dure la rafaga, ATACAMOS.
                        launchProyectile();
                        countThreeToAttack = 3;
                    } else {
                        timeToAttack = 0;
                        idle();
                    }
                } else {
                    countThreeToAttack--;
                }
            }

            timeToAttack = timeToAttack + Time.deltaTime;
            if (!anim.GetBool("isAttack") && (timeToAttack >= TIME_TO_ATTACK)) {
                // Preparar la rafaga para atacar.
                timeInAttack = 0;
                countThreeToAttack = 3;
                // Asignar estado atacar.
                anim.SetBool(stateEnemy, false);
                anim.SetBool("isAttack", true);
                stateEnemy = "isAttack";
            }
            
        }
    }

    void jump() {
        float direction = (player.transform.position.x < transform.position.x) ? -1 : 1;
        rb2d.AddForce(new Vector2(speed * 10 * direction, jumpPower));
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isJump", true);
        stateEnemy = "isJump";
    }

    void enemyIA() {
        bool isInAir = rb2d.velocity.y != 0;
        if (!isInAir) {
            int jumpProb = (anim.GetBool("isIdle") || anim.GetBool("isJump"))? 
                1 : Random.Range(0, 300); // Elegimos si saltar hacia el player o hacer otra cosa.
            if (jumpProb == 0) {
                // Aleatoriamente saltamos.
                jump();
            } else {
                // Si está en el suelo.
                float distanceToPlayer = Mathf.Abs(goPlayer.transform.position.x - transform.position.x);
	            if (distanceToPlayer <= lenghtSight) {
                    if (distanceToPlayer <= lenghtAttack) {
                        // Atacar durante un tiempo.
                        isInAttackMode = true;
                    } else {
                        // Andar hacia el player.
                        isInAttackMode = false;
                        timeToAttack = 0;
                        walk();
                    }
                } else {
                    // Mantenerse en idle.
                    isInAttackMode = false;
                    timeToAttack = 0;
                    idle();
                }
            }
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
            if ((currentLife <= 0) || ((fromBoss != null) && ("isDie").Equals(fromBoss.getState()))) {
                timeToAttack = 0;
                timeInAttack = TIME_IN_ATTACK;
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
            // Para evitar errores, hacemos el ataque sólo en FixedUpdate().
            if (isInAttackMode) {
                attack();
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
                    idle();
                    rb2d.velocity = new Vector2(0, 0);
                }
                break;
        }
    }
}
