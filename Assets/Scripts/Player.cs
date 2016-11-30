using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    
    private const float FACTOR_DECREASE_VELOCITY_X = 0.75f;
    private const float TIME_IN_IDLE_TO_SHOW_GO = 0.5f;
    private const float TIME_IN_AIR_ATTACK = 1f;
    private const float TIME_TO_REFLESH_TIME_GAME = 1.25f;
    private const float TIME_TO_ATTACK = 0.15f;//0.1f; // Contra mayor sea esta constante, menos bolas lanza por cada milisegundo.
    private const float bulletVelocityX = 10;
    // Para cuando dispara hacia los lados.
    private const float marginBallBulletX = 0.684f;
    private const float marginBallBulletY = 0.15f;
    private const float marginBulletX = 1.617f;
    private const float marginBulletY = 0.109f;
    private const float marginBallBulletZ = -2.013922f;
    private const float marginBulletZ = -2.01392f; // Las balas deben quedar por detrás de la bola.
    // Para cuando dispara hacia arriba.
    private const float marginUpBallBulletX = 0.2728953f;
    private const float marginUpBallBulletY = 0.6722765f;
    private const float marginUpBulletX = 0.32f;
    private const float marginUpBulletY = 1.284443f;
    
    public static float score = 0;
    public static float timeGame = 1000;

    public float maxLife = 150;
    public float maxSpeed = 1f;
    public float speed = 50f;
    public float jumpPower = 300f;
    public TextMesh currentScore;
    public TextMesh currentTime;
    public GameObject goAlertArrow;
    public GameObject ballShootPrefab;
    public GameObject shootHorizontalPrefab;
    public GameObject shootVerticalPrefab;
    public AudioClip audioShoot;
    public AudioClip audioCure;
    public AudioClip audioGO;
    public GameObject objWithAudioSource; // Para que se escuche bien cuando se recoja un HP Recover.

    private float timerAttack;
    private float timeToShowGo;
    private float timeToRefleshTimeGame;
    private bool isLookingToRight;
    private float currentLife;
    private float timeMatrix;
    private float widthCamera;
    private string statePlayer;
    private Rigidbody2D rb2d;
    private Animator anim;
    private GameObject gamecamera;
    private CameraFollow cameraFollow;
    private PauseMenu pauseMenu;

    // Para el hurt (tiempo que dura el hurt, tiempo entre la transición aparecer/desaparecer), y si está en hurt.
    private const float timeInHurt = 0.75f;
    private const float timeBlink = 0.07f;
    private float currentTimeBlink;
    private bool activeBlinkHurt;


    // Use this for initialization
    void Start () {
        Player.score = 0;
        Player.timeGame = 1000;
        gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraFollow = (CameraFollow) gamecamera.GetComponent("CameraFollow");
        pauseMenu = (PauseMenu) gamecamera.GetComponent("PauseMenu");
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        currentLife = maxLife;
        timerAttack = 0;
        timeMatrix = 0;
        timeToRefleshTimeGame = 0;
        timeToShowGo = 0;
        isLookingToRight = true;
        goAlertArrow.GetComponent<Renderer>().enabled = false;
        activeBlinkHurt = false;
        currentTimeBlink = 0;
        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
        // Para cuando muere el player y vuelve a iniciarse el juego.
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_PLAYER, false);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_PLAYER, false);
    }

    public bool isPlayerLookingToRight() {
        return isLookingToRight;
    }

    public Rigidbody2D getRigidBody2D() {
        return rb2d;
    }

    public Vector2 getVelocity() {
        return new Vector2(rb2d.velocity.x, rb2d.velocity.y);
    }

    public string getStatePlayer() {
        return statePlayer;
    }

    public float getCurrentLife() {
        return currentLife;
    }

    public void cure(float n) {
        currentLife += n;
        if (currentLife > maxLife) {
            currentLife = maxLife;
        }

        if (audioCure != null) {
            // Insertamos sonido de cure.
            AudioSource currentAS = (AudioSource) objWithAudioSource.GetComponent<AudioSource>();
            if (currentAS.clip != audioCure) {
                currentAS.clip = audioCure;
            }
            currentAS.Play();
        }
    }

    void callEnableCollisionWithEnemyShoot() {
        activeBlinkHurt = false;
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_PLAYER, false);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_PLAYER, false);
    }

    public void damage(float n) {
        currentLife -= n;
        if (currentLife < 0) {
            currentLife = 0;
        }
        activeBlinkHurt = true;
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_PLAYER, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_PLAYER, true);
        Invoke("callEnableCollisionWithEnemyShoot", timeInHurt);
    }
    
    void refleshStatus() {
        if (anim.GetBool("isAttack1")) {
            statePlayer = "isAttack1";
        } else if (anim.GetBool("isAttack2")) {
            statePlayer = "isAttack2";
        } else if (anim.GetBool("isIdle")) {
            statePlayer = "isIdle";
        } else if (anim.GetBool("isWalk")) {
            statePlayer = "isWalk";
        } else if (anim.GetBool("isJump")) {
            statePlayer = "isJump";
        } else if (anim.GetBool("isJumpAttack1")) {
            statePlayer = "isJumpAttack1";
        } else if (anim.GetBool("isJumpAttack2")) {
            statePlayer = "isJumpAttack2";
        } else if (anim.GetBool("isWalkAttack2")) {
            statePlayer = "isWalkAttack2";
        } else if (anim.GetBool("isWalkAttack1")) {
            statePlayer = "isWalkAttack1";
        } else if (anim.GetBool("isFall")) {
            statePlayer = "isFall";
        } else if (anim.GetBool("isDie")) {
            statePlayer = "isDie";
        } else {
            // Si está todo en falso porque ha entrado en un estado extraño, poner en idle.
            anim.SetBool("isIdle", true);
            statePlayer = "isIdle";
        }

    }

    void launchAttack() {
        if (Input.GetAxis("Vertical") > 0) {
            // Dispara hacia arriba o hacia abajo.
            Vector2 direction = transform.position + new Vector3(marginUpBulletX, marginUpBulletY, marginBulletZ);
        
            Transform bullet = GameObject.Instantiate<GameObject>(shootVerticalPrefab).transform;
            bullet.Rotate(0f, 0f, 90f);
            float bulletVelocityY = (Input.GetAxis("Vertical") > 0) ? bulletVelocityX : -bulletVelocityX;
            if (isLookingToRight) {
                bullet.position = new Vector3(direction.x, direction.y, marginBulletZ);
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, bulletVelocityY), ForceMode2D.Impulse);
            } else {
                bullet.position = new Vector3(transform.position.x - marginUpBulletX, direction.y, marginBulletZ);
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, bulletVelocityY), ForceMode2D.Impulse);
                bullet.localScale = new Vector3(-bullet.localScale.x, bullet.localScale.y, bullet.localScale.x);
            }
            
        } else {
            // Disparamos las líneas.
            Vector2 direction = transform.position + new Vector3(marginBulletX, marginBulletY, marginBulletZ);
            
            Transform bullet = GameObject.Instantiate<GameObject>(shootHorizontalPrefab).transform;
            if (isLookingToRight) {
                bullet.position = new Vector3(direction.x, direction.y, marginBulletZ);
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(bulletVelocityX, 0), ForceMode2D.Impulse);
            } else {
                bullet.position = new Vector3(transform.position.x - marginBulletX, direction.y, marginBulletZ);
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(-bulletVelocityX, 0), ForceMode2D.Impulse);
                bullet.localScale = new Vector3(-bullet.localScale.x, bullet.localScale.y, bullet.localScale.x);
            }
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

    void createAttack() {
        // Creamos la bola de energía.
        if (Input.GetAxis("Vertical") > 0) {
            Vector2 directionBall = transform.position + new Vector3(marginUpBallBulletX, marginUpBallBulletY, marginBallBulletZ);
            Transform ball = GameObject.Instantiate<GameObject>(ballShootPrefab).transform;
            if (isLookingToRight) {
                ball.position = new Vector3(directionBall.x, directionBall.y, marginBallBulletZ);
            } else {
                ball.position = new Vector3(transform.position.x - marginUpBallBulletX, directionBall.y, marginBallBulletZ);
                ball.localScale = new Vector3(-ball.localScale.x, ball.localScale.y, ball.localScale.x);
            }
        } else {
            Vector2 directionBall = transform.position + new Vector3(marginBallBulletX, marginBallBulletY, marginBallBulletZ);
            Transform ball = GameObject.Instantiate<GameObject>(ballShootPrefab).transform;
            if (isLookingToRight) {
                ball.position = new Vector3(directionBall.x, directionBall.y, marginBallBulletZ);
            } else {
                ball.position = new Vector3(transform.position.x - marginBallBulletX, directionBall.y, marginBallBulletZ);
                ball.localScale = new Vector3(-ball.localScale.x, ball.localScale.y, ball.localScale.x);
            }
        }

        // Lanzar el ataque.
        timerAttack = timerAttack + Time.deltaTime;
        if (timerAttack >= TIME_TO_ATTACK) {
            launchAttack();
            timerAttack = 0;
        }
    }


    void attack() {
        // Lanzar ataque.
        createAttack();

        // Asignar estado atacar.
        string attackState = "isAttack1";

        if ((Input.GetAxis("Horizontal") != 0)) {
            // Si camina mientras dispara a los lados...
            attackState = "isWalkAttack1";
        }

        if (Input.GetAxis("Vertical") > 0) {
            // Si dispara arriba...
            if (Input.GetAxis("Horizontal") != 0) {
                // Si está andando mientras dispara arriba...
                attackState = "isWalkAttack2";
            } else {
                // Si no...
                attackState = "isAttack2";
            }
        }

        anim.SetBool(statePlayer, false);
        anim.SetBool(attackState, true);
        statePlayer = attackState;
    }

    
    void jumpAttack() {
        // Lanzar ataque.
        createAttack();

        // Asignar estado atacar.
        string attackState = "isJumpAttack1";
        if (Input.GetAxis("Vertical") > 0) {
            attackState = "isJumpAttack2";
        }
        anim.SetBool(statePlayer, false);
        anim.SetBool(attackState, true);
        statePlayer = attackState;
        
    }

    void idle() {
        anim.SetBool(statePlayer, false);
        anim.SetBool("isIdle", true);
        statePlayer = "isIdle";
    }
    
    void walk(float directionX) {
        // Colocar en su dirección correcta y asignar estado walk.
        float velocityY = rb2d.velocity.y;

        transform.localScale = new Vector3(directionX, 1, 1);
        if (velocityY == 0) {
            anim.SetBool(statePlayer, false);
            anim.SetBool("isWalk", true);
            statePlayer = "isWalk";
        }
    }

    void jump() {
        rb2d.AddForce(Vector2.up * jumpPower);
        anim.SetBool(statePlayer, false);
        anim.SetBool("isJump", true);
        statePlayer = "isJump";
    }

    void fall() {
        anim.SetBool(statePlayer, false);
        anim.SetBool("isFall", true);
        statePlayer = "isFall";
    }

    void gameOver() {
        Application.LoadLevel(5);
    }

    void die() {
        anim.SetBool(statePlayer, false);
        anim.SetBool("isDie", true);
        statePlayer = "isDie";
        // Pasamos a pantalla de Game Over.
        Invoke("gameOver", 0.5f);
    }

    void checkKeys() {
        if ((currentLife == 0) || (timeGame <= 0)) {
            // Si la vitalidad == 0 o tiempo acabado, muere.
            die();
        } else {
            float velocityY = rb2d.velocity.y;
            bool newStateIsIdle = true;
        
            float horizAxis = Input.GetAxis("Horizontal");
            if ((horizAxis < 0)) {
                // Andar hacia la izquierda.
                walk(-1);
                isLookingToRight = false;
                newStateIsIdle = false;
            }

            if (horizAxis > 0) {
                // Andar hacia la derecha.
                walk(1);
                isLookingToRight = true;
                newStateIsIdle = false;
            }

            if ((velocityY == 0) && !("isJump").Equals(statePlayer) && (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.JoystickButton0))) {
                // Si no está saltando y ha pulsado saltar, salta.
                jump();
                timeMatrix = 0f;
                newStateIsIdle = false;
            }
            
            if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.JoystickButton2)) {
                // Si se mantiene pulsado la J, está atacando.
                if (velocityY < 0) {
                    // Hacer caida lenta.
                    jumpAttack();
                    timeMatrix = timeMatrix + Time.deltaTime;
                    bool isJumping = Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.JoystickButton0); //¿está saltando?
                    if ((timeMatrix < TIME_IN_AIR_ATTACK) && isJumping/*Input.GetButton("Jump")*/) {
                        // Si estamos dentro del tiempo ha pasado, caida lenta.
                        rb2d.velocity = new Vector2(0f, 0.1f);
                        velocityY = rb2d.velocity.y;   
                    }
                } else if (velocityY == 0) {
                    // Se mantiene en el suelo.
                    attack();
                } else {
                    // Está saltando y atacando.
                    jumpAttack();
                }
                newStateIsIdle = false;
            }

            // Caída.
            if ((velocityY < 0) && !("isJumpAttack1").Equals(statePlayer) && !("isJumpAttack2").Equals(statePlayer)) {
                fall();
            }

            // Si no ha pasado por todos los estados anteriores, está en idle.
            if (newStateIsIdle && (velocityY == 0) && (horizAxis == 0)) {
                idle();
            }
            
        }
    }

    void refleshTimeGame() {
        if (timeGame > 0) {
            timeToRefleshTimeGame = timeToRefleshTimeGame + Time.deltaTime;
            if (timeToRefleshTimeGame > TIME_TO_REFLESH_TIME_GAME) {
                // Actualizar contador hacia atrás de tiempo.
                timeToRefleshTimeGame = 0;
                timeGame--;
            }
        }
    }

    private const float TimeBlinkGo = 0.7f;
    private float currentTimeBlinkGo = 0;
    void enableBlinkGo() {
        currentTimeBlinkGo = currentTimeBlinkGo + Time.deltaTime;
        if (currentTimeBlinkGo > TimeBlinkGo) {
            currentTimeBlinkGo = 0;
            // Enseñamos u ocultamos GO.
            goAlertArrow.GetComponent<Renderer>().enabled = !goAlertArrow.GetComponent<Renderer>().enabled;
            if (goAlertArrow.GetComponent<Renderer>().enabled) {
                // Si el Go aparecen en pantalla, insertamos sonido de alerta GO.
                if (audioGO != null) {
                    AudioSource currentAS = (AudioSource) goAlertArrow.GetComponent<AudioSource>();
                    if (currentAS.clip != audioGO) {
                        currentAS.clip = audioGO;
                    }
                    currentAS.Play();
                }
            }
        }
    }

    void showGo() {
        if (("isIdle").Equals(statePlayer) && !cameraFollow.isFreezeCamera()) {
            timeToShowGo = timeToShowGo + Time.deltaTime;
            if (timeToShowGo >= TIME_IN_IDLE_TO_SHOW_GO) {
                // Lleva demasiado rato quieto y la cámara no está congelada, mostrar GO.
                enableBlinkGo();
            }
        } else {
            timeToShowGo = 0;
            goAlertArrow.GetComponent<Renderer>().enabled = false;
        }
    }

    void showHurt() {
        Renderer playerRenderer = gameObject.GetComponent<Renderer>();
        if (activeBlinkHurt) {
            currentTimeBlink = currentTimeBlink + Time.deltaTime;
            if (currentTimeBlink > timeBlink) {
                playerRenderer.enabled = !playerRenderer.enabled;
                currentTimeBlink = 0;
            }
        } else {
            if (!playerRenderer.enabled) {
                playerRenderer.enabled = true;
            }
            currentTimeBlink = 0;
        }
    }

    // Update is called once per frame
    void Update () {
        if (!pauseMenu.isGameInPause()) {
            // Actualizar tiempo restante.
            refleshTimeGame();
            // Ver en qué estado está el player.
            refleshStatus();
            // Comprobar si se ha tocado una tecla, y cambiamos de estado en dicho caso.
            checkKeys();
            // Actualizar score en la UI.
            currentScore.text = "Score: " + score;
            //currentTime.text = "Time: " + timeGame;
            currentTime.text = "" + timeGame;
            // Mostrar el Hurt (parpadeo) en caso de que esté.
            showHurt();
            // Enseñar GO cuando el player esté en idle un rato.
            showGo();
        }
    }
    
    bool canMoveInScreen() {
        bool ret = true;
        if (cameraFollow.isFreezeCamera()) {
            // Si la cámara está congelada comprobar si el player está en los límites.
            float leftSideCameraX = gamecamera.transform.position.x - widthCamera;
            float rightSideCameraX = gamecamera.transform.position.x + widthCamera;
            float posX = transform.position.x;

            if ((leftSideCameraX > posX) && (Input.GetAxis("Horizontal") < 0)) {
                ret = false;
            } else if ((posX > rightSideCameraX) && (Input.GetAxis("Horizontal") > 0)) {
                ret = false;
            }
        }
        return ret;
    }

    void FixedUpdate() {
        bool canMoveInScreen = this.canMoveInScreen();
        // Mirar si estamos en uno de los estados de poder moverse.
        bool isWalkInThisState = ("isWalk").Equals(statePlayer) || ("isWalkAttack2").Equals(statePlayer) 
            || ("isWalkAttack1").Equals(statePlayer) || ("isJump").Equals(statePlayer) 
            || ("isJumpAttack1").Equals(statePlayer) || ("isJumpAttack2").Equals(statePlayer) 
            || ("isFall").Equals(statePlayer);
        if (canMoveInScreen && isWalkInThisState) {
            // Incrementar velocidad de andar pero controlando que no se pase de maxSpeed ni se chorre.
            Vector3 easeVelocity = new Vector3();
            easeVelocity.y = rb2d.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x = rb2d.velocity.x * FACTOR_DECREASE_VELOCITY_X;

            // Asignar velocidad con factor de decrecimiento de velocidad si no está en el aire.
            if (rb2d.velocity.y == 0) {
                rb2d.velocity = easeVelocity;
            }

            // Aplicar velocidad en la X.
            float inputH = Input.GetAxis("Horizontal");
            if (inputH > 0) {
                inputH = 1;
            } else if (inputH < 0) {
                inputH = -1;
            }
            rb2d.AddForce(Vector2.right * speed * inputH);

            // Controlamos que no se pase del rango [-maxSpeed, maxSpeed].
            if (rb2d.velocity.x > maxSpeed) {
                rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
            }

            if (rb2d.velocity.x < -maxSpeed)
            {
                rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);
            }
            
        } else {
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
    }
}
