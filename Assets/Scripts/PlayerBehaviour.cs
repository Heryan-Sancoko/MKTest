using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    /*Quick overview of changes in this script:
    1. Removed public variables and made them private
    2. Removed all coroutines
    3. Added player states
    4. Added more comments
    */
    [SerializeField]
    private Joystick mJoystick = null;
    [SerializeField]
    private float jumpTime = 0.5f;
    private float dashTime;
    private float airTime;
    private float slowTime;
    private float killTime = 1;
    private float jumpThresh;
    [SerializeField]
    private float fallSpeed = -20;
    private Rigidbody rbody;
    [SerializeField]
    private GameObject mHitbox = null;
    [SerializeField]
    private float mSpeed = 12;
    private float originalSpeed;
    private int jumpsRemaining;
    private int dashesRemaining;
    [SerializeField]
    private LayerMask mRayLayerMask = ~0;
    [SerializeField]
    private Transform mModel = null;
    private Vector3 originalModelScale;
    private Vector3 originalModelPos;
    [SerializeField]
    private ParticleSystem deathParticle = null;
    [SerializeField]
    private DeathManager mDeathManager = null;
    public ScoreScript mScore;

    //Sound variables ==========================================//

    public AudioSource jumpSound, landSound, slowSound, dashSound, dyingSound, bgm;


    //End Sound variables ======================================//
    //Player States ============================================//

    //Player can either be jumping OR fastfalling
    private enum airState {neutral, jumping, fastFalling};
    private airState mAirstate = airState.neutral;

    //Player can either be dashing OR slowing
    private enum speedState {neutral, dashing, slowing};
    private speedState mSpeedState = speedState.neutral;

    //Player is either grounded or in the air
    private bool isGrounded = false;

    //Player is either alive or dead
    private bool isAlive = true;

    //Displays the latest state the player has been in.
    //This is used to trigger the audio cues and could be used
    //to trigger animations if so desired.
    public enum latestState { neutral, jumping, fastFalling, slowing, dashing, justLanded};
    public latestState myLatestState = latestState.neutral;

    //End Player States ========================================//

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = mSpeed;
        originalModelScale = mModel.transform.localScale;
        originalModelPos = mModel.transform.localPosition;
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            //slooowly ramp up the speed
            originalSpeed += Time.deltaTime * 0.05f;

            //Take the joystick swipe direction and act accordingly
            switch (mJoystick.mySwipeDirection)
            {
                case Joystick.swipeDirection.Up:
                    if (jumpsRemaining > 0)
                    {
                        ResetPlayerState();
                        SetPlayerJump();
                    }
                    break;
                case Joystick.swipeDirection.Down:
                    ResetPlayerState();
                    SetPlayerFastFall();
                    break;
                case Joystick.swipeDirection.Left:
                    if (mSpeedState == speedState.dashing)
                    {
                        EndPlayerDash();
                    }
                    SetPlayerSlow();
                    break;
                case Joystick.swipeDirection.Right:
                    if (dashesRemaining > 0)
                    {
                        ResetPlayerState();
                        SetPlayerDash();
                    }
                    break;
            }

            // Dash or slow depending on the player's state
            switch (mSpeedState)
            {
                case speedState.dashing:
                    PlayerDash();
                    break;
                case speedState.slowing:
                    PlayerSlow();
                    break;
            }

            switch (mAirstate)
            {
                case airState.fastFalling:
                    PlayerFastFall();
                    break;
                case airState.jumping:
                    PlayerJump();
                    break;
            }

            //The model deactivates upon falling offscreen.
        }  //When this happens, wait for 1 second, then activate the death screen.
        else if (!mModel.gameObject.activeSelf)
        {
            if (killTime > 0)
            {
                killTime -= Time.deltaTime;
            }
            else
            {


                mDeathManager.enabled = true;
                gameObject.SetActive(false);
            }
        }
        else
        {
            //Player needs to jump one last time upon running into a rock
            if (mAirstate == airState.jumping)
            {
                PlayerJump();
            }
        }




        //Make the tortoise move forward
        rbody.velocity = new Vector3(mSpeed, rbody.velocity.y, rbody.velocity.z);

        //Rescale model to regular size
        if (mAirstate != airState.fastFalling && mSpeedState != speedState.dashing)
        {
            mModel.localScale = Vector3.Lerp(mModel.localScale, originalModelScale, 0.2f);
        }

        //Death is triggered when the tortoise falls offscreen
        if (Camera.main.WorldToScreenPoint(transform.position).y < 0 && mModel.gameObject.activeSelf)
        {
            KillTortoise();
        }

        //Play sound based on player's latest state
        PlayPlayerSound();
    }

    //Play sound based on player's latest state
    private void PlayPlayerSound()
    {
        switch (myLatestState)
        {
            case latestState.jumping:
                if (isAlive)
                {
                    RandomizePitch(jumpSound);
                    jumpSound.Play();
                }
                else
                {
                    dyingSound.Play();
                }
                myLatestState = latestState.neutral;
                break;
            case latestState.dashing:
                RandomizePitch(dashSound);
                dashSound.Play();
                myLatestState = latestState.neutral;
                break;
            case latestState.slowing:
                RandomizePitch(slowSound);
                slowSound.Play();
                myLatestState = latestState.neutral;
                break;
            case latestState.justLanded:
                RandomizePitch(landSound);
                landSound.Play();
                myLatestState = latestState.neutral;
                break;
        }
    }

    private void RandomizePitch(AudioSource mAudio)
    {
        if (mAudio != dashSound)
            mAudio.pitch = 1 + (Random.Range(-0.5f, 0.5f));
        else
        {
            mAudio.pitch = Random.Range(1f, 2f);
            if (mAudio.pitch > 1.5f)
            {
                mAudio.pitch = 1.5f;
            }
            else
            {
                mAudio.pitch = 1;
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.7f, mRayLayerMask))
        {
            mSpeed = 0;
        }
        else
        {
            mSpeed = Mathf.Lerp(mSpeed, originalSpeed, 0.1f);
        }
    }

    public void KillTortoise() // this happens as soon as the tortoise falls offscreen
    {
        isAlive = false;
        mSpeed = 0;
        originalSpeed = 0;
        deathParticle.transform.parent = null;
        bgm.Stop();
        deathParticle.gameObject.SetActive(true);
        mModel.gameObject.SetActive(false);
    }

    public void TortiseSpinOut() // This happens when the tortoise hits a rock
    {
        GetComponent<Collider>().enabled = false;
        ResetPlayerState();
        jumpsRemaining = 1;
        SetPlayerJump();
    }

    // PLAYER JUMP CODE BELOW ===================================================== PLAYER JUMP CODE BELOW//

    private void SetPlayerJump()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        mAirstate = airState.jumping;
        mModel.localScale = mModel.localScale + (Vector3.up * (mModel.localScale.y * 1.5f));
        jumpsRemaining--;
        airTime = jumpTime;
        jumpThresh = transform.position.y + 4.38f;
        myLatestState = latestState.jumping;
    }

    private void EndPlayerJump()
    {
        rbody.useGravity = true;
        mAirstate = airState.neutral;
        myLatestState = latestState.neutral;
    }

    private void PlayerJump()
    {
        if (airTime > 0)
        {
            rbody.velocity = new Vector3(rbody.velocity.x, 0, rbody.velocity.z);
            rbody.useGravity = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpThresh, transform.position.z), 0.2f);
            airTime -= Time.deltaTime;
        }
        else if (airTime <= 0 && mAirstate == airState.jumping)
        {
            EndPlayerJump();
        }
    }

    // END PLAYER JUMP CODE =====================================================END PLAYER JUMP CODE//
    // PLAYER FASTFALL CODE BELOW ===================================================== PLAYER FASTFALL CODE BELOW//

    private void SetPlayerFastFall()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        rbody.useGravity = true;
        mAirstate = airState.fastFalling;
        rbody.velocity = new Vector3(rbody.velocity.x, fallSpeed, rbody.velocity.z);
        myLatestState = latestState.fastFalling;
    }

    private void EndPlayerFastFall()
    {
        mAirstate = airState.neutral;
        myLatestState = latestState.neutral;
    }

    private void PlayerFastFall()
    {
        if (!isGrounded)
        {
            mModel.localScale = Vector3.Lerp(mModel.localScale, new Vector3 (originalModelScale.x, originalModelScale.y * 2f, originalModelScale.z * 0.5f), 0.25f);
        }
        else if (isGrounded && mAirstate == airState.fastFalling)
        {
            EndPlayerFastFall();
            myLatestState = latestState.justLanded;
        }
    }

    // END PLAYER FASTFALL CODE =====================================================END PLAYER FASTFALL CODE//
    // PLAYER SLOW CODE BELOW ===================================================== PLAYER SLOW CODE BELOW//

    private void SetPlayerSlow()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        mSpeedState = speedState.slowing;
        slowTime = jumpTime;
        myLatestState = latestState.slowing;
    }

    private void EndPlayerSlow()
    {
        mSpeedState = speedState.neutral;
        myLatestState = latestState.neutral;
    }

    private void PlayerSlow()
    {
        if (slowTime > 0)
        {
            mSpeed = originalSpeed * 0.5f;
            slowTime -= Time.deltaTime;
        }
        else if (slowTime <= 0 && mSpeedState == speedState.slowing)
        {
            EndPlayerSlow();
        }
    }

    // END PLAYER SLOW CODE =====================================================END PLAYER SLOW CODE//
    // PLAYER DASH CODE BELOW ===================================================== PLAYER DASH CODE BELOW//

    private void SetPlayerDash()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        dashesRemaining--;
        mSpeedState = speedState.dashing;
        mHitbox.SetActive(true);
        rbody.useGravity = false;
        dashTime = jumpTime;
        myLatestState = latestState.dashing;
    }

    private void EndPlayerDash()
    {
        mSpeedState = speedState.neutral;
        mHitbox.SetActive(false);
        rbody.useGravity = true;
        myLatestState = latestState.neutral;
    }

    private void PlayerDash()
    {

        if (dashTime > 0)
        {
            mModel.localScale = Vector3.Lerp(mModel.localScale, (originalModelScale + (Vector3.forward * (originalModelScale.z * 0.4f)) - Vector3.up * (originalModelScale.y * 0.3f)), 0.3f);
            mModel.localPosition = Vector3.Lerp(mModel.localPosition, new Vector3(originalModelPos.x - (Mathf.Abs(originalModelPos.x * 1)), mModel.localPosition.y, mModel.localPosition.z), 0.3f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.7f, mRayLayerMask))
            {
                mSpeed = 0;
            }
            else
            {
                mSpeed = originalSpeed * 2f;
            }
            rbody.velocity = new Vector3(rbody.velocity.x, 0, rbody.velocity.z);
            dashTime -= Time.deltaTime;
        }
        else if (dashTime <= 0 && mSpeedState == speedState.dashing)
        {
            EndPlayerDash();
        }
    }

    // END PLAYER DASH CODE =====================================================END PLAYER DASH CODE//

    public void ResetPlayerState()
    {
        if (mSpeedState == speedState.dashing)
        {
            EndPlayerDash();
        }

        if (mAirstate == airState.jumping)
        {
            EndPlayerJump();
        }

        if (mAirstate == airState.fastFalling)
        {
            EndPlayerFastFall();
        }

        if (mSpeedState == speedState.slowing)
        {
            EndPlayerSlow();
        }

        myLatestState = latestState.neutral;
        rbody.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.layer)
        {
            case 9:
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.right, out hit, 1, mRayLayerMask))
                {
                    mSpeed = 0;
                }
                else
                {
                    //Recharge jump and dash charges
                    isGrounded = true;
                    myLatestState = latestState.justLanded;
                    jumpsRemaining = 1;
                    dashesRemaining = 1;

                    //Upon landing, squash the model
                    mModel.localScale = (mModel.localScale + (Vector3.forward * (mModel.localScale.z * 0.4f)) - Vector3.up * (mModel.localScale.y * 0.3f));
                }
                break;
            case 11:
                isAlive = false;
                TortiseSpinOut();
                mModel.GetComponent<Animator>().SetBool("isAlive", isAlive);
                break;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.layer)
        {
            case 9:
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.right, out hit, 1, mRayLayerMask))
                {
                    mSpeed = 0;
                }
                else
                {
                    isGrounded = true;
                    dashesRemaining = 1;
                }
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.gameObject.layer)
        {
            case 9:
                    isGrounded = false;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.right*1);
    }
}
