using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterMoveController : MonoBehaviour
{


    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    
    [Header("Jump")]
    public float jumpAccel;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;
    private float lastPositionX;

    [Header("UI")]
    public TextMeshProUGUI UiScore;
    public TextMeshProUGUI UiHighscore;


    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    private bool isJumping;
    private bool isOnGround;


    private Rigidbody2D rig;
    private Animator anim;
    private CharacterSoundScript sound;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // read input
        if (Input.GetAxis("Jump") == 1)
        {
            Debug.Log("Jumped");
            if (isOnGround)
            {
                isJumping = true;
            }
        }
        anim.SetBool("isOnGround", isOnGround);

        // calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }
        UiScore.text =ScoreController.GetCurrentScore().ToString();
        UiHighscore.text = ScoreData.highScore.ToString();

        if (transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocityVector = rig.velocity;
        // raycast ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(.5f, 0, 0), Vector2.down, groundRaycastDistance, groundLayerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(.5f, 0, 0), Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit || hit2)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }

        // calculate velocity vector

        if (isJumping)
        {
            velocityVector.y += jumpAccel; 
            sound.PlayJump();

            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;
    }
    public void GameOver()
    {
        // set high score
        score.FinishScoring();

        // stop camera movement
        gameCamera.enabled = false;

        // show gameover
        gameOverScreen.SetActive(true);

        //stop the player & animation
        rig.velocity = new Vector2(0,0);
        anim.enabled = false;
        
        // disable this too
        this.enabled = false;
    }


    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position - new Vector3(.5f , 0,0), transform.position + (Vector3.down * groundRaycastDistance) - new Vector3(.5f, 0, 0), Color.white);
        Debug.DrawLine(transform.position + new Vector3(.5f, 0, 0), transform.position + (Vector3.down * groundRaycastDistance) + new Vector3(.5f, 0, 0), Color.white);
    }
}
