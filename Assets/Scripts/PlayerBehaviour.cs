using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Joystick mJoystick;
    public float jumpTime;
    public float fallSpeed;
    private Rigidbody rbody;
    public float falVel;
    public GameObject mHitbox;
    public bool isDashing = false;
    public bool isSlowing = false;
    public float mSpeed;
    private float originalSpeed;
    public int jumpsRemaining;
    public int dashesRemaining;
    public LayerMask mRayLayerMask;
    public Transform mModel;
    private Vector3 originalModelScale;
    private Vector3 originalModelPos;
    private bool isGrounded = false;
    private bool isAlive = true;
    public ParticleSystem deathParticle;
    public DeathManager mDeathManager;
    public ScoreScript mScore;
    private bool isHittingWall = false;


    private IEnumerator jump = null;
    private IEnumerator fall = null;
    private IEnumerator slow = null;
    private IEnumerator dash = null;


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
            switch (mJoystick.mySwipeDirection)
            {
                case Joystick.swipeDirection.Up:
                    if (jumpsRemaining > 0)
                    {
                        mModel.localScale = mModel.localScale + (Vector3.up * (mModel.localScale.y * 1.5f));
                        ResetCoroutines();
                        jump = PlayerJump();
                        StartCoroutine(jump);
                    }
                    break;
                case Joystick.swipeDirection.Down:
                    ResetCoroutines();
                    fall = PlayerFastFall();
                    StartCoroutine(fall);
                    break;
                case Joystick.swipeDirection.Left:
                    if (dash != null)
                    {
                        StopCoroutine(dash);
                        mHitbox.SetActive(false);
                        rbody.useGravity = true;
                        isDashing = false;
                    }
                    slow = PlayerSlow();
                    StartCoroutine(slow);
                    break;
                case Joystick.swipeDirection.Right:
                    if (dashesRemaining > 0)
                    {
                        ResetCoroutines();
                        dash = PlayerDash();
                        StartCoroutine(dash);
                    }
                    break;
            }
        }

        falVel = rbody.velocity.y;
        rbody.velocity = new Vector3(mSpeed, rbody.velocity.y, rbody.velocity.z);
        if (fall == null)
        {
            mModel.localScale = Vector3.Lerp(mModel.localScale, originalModelScale, 0.2f);
        }
        mModel.localPosition = Vector3.Lerp(mModel.localPosition, originalModelPos, 0.2f);

        //on death
        if (Camera.main.WorldToScreenPoint(transform.position).y < 0)
        {
            StartCoroutine(KillTortoise());
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

    public IEnumerator KillTortoise()
    {
        isAlive = false;
        mSpeed = 0;
        originalSpeed = 0;
        deathParticle.transform.parent = null;
        deathParticle.gameObject.SetActive(true);
        mModel.gameObject.SetActive(false);
        float killTimer = 1;
        while (killTimer > 0)
        {
            killTimer -= Time.deltaTime;
            yield return null;
        }

        mDeathManager.enabled = true;
        yield return null;
    }

    public IEnumerator TortiseSpinOut()
    {
        GetComponent<Collider>().enabled = false;
        ResetCoroutines();
        jump = PlayerJump();
        StartCoroutine(jump);
        yield return null;
    }

    private IEnumerator PlayerJump()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        jumpsRemaining--;
        float airTime = jumpTime;
        float jumpThresh = transform.position.y + 4.38f;
        while (airTime > 0)
        {
            rbody.velocity = new Vector3(rbody.velocity.x, 0, rbody.velocity.z);
            rbody.useGravity = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, jumpThresh, transform.position.z), 0.2f);
            airTime -= Time.deltaTime;
            yield return null;
        }
        rbody.useGravity = true;
        jump = null;
        yield return null;
    }

    private IEnumerator PlayerFastFall()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        rbody.useGravity = true;
        rbody.velocity = new Vector3(rbody.velocity.x, fallSpeed, rbody.velocity.z);
        while (!isGrounded)
        {
            mModel.localScale = Vector3.Lerp(mModel.localScale, originalModelScale + (Vector3.up * (mModel.localScale.y * 0.3f)), 0.2f);
            yield return null;
        }
        fall = null;
        yield return null;
    }

    private IEnumerator PlayerSlow()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        isSlowing = true;
        float slowTime = jumpTime;
        while (slowTime > 0)
        {
            mSpeed = originalSpeed * 0.5f;
            slowTime -= Time.deltaTime;
            yield return null;
        }
        isSlowing = false;
        slow = null;
        yield return null;
    }

    private IEnumerator PlayerDash()
    {
        mJoystick.mySwipeDirection = Joystick.swipeDirection.None;
        dashesRemaining--;
        isDashing = true;
        mHitbox.SetActive(true);
        float dashTime = jumpTime;
        while (dashTime > 0)
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
            rbody.useGravity = false;
            dashTime -= Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        mHitbox.SetActive(false);
        rbody.useGravity = true;
        dash = null;
        yield return null;
    }

    public void ResetCoroutines()
    {
        if (dash != null)
        {
            StopCoroutine(dash);
            mHitbox.SetActive(false);
            isDashing = false;
            dash = null;
        }

        if (slow != null)
        {
            StopCoroutine(slow);
            isSlowing = false;
            slow = null;
        }

        if (jump != null)
        {
            StopCoroutine(jump);
            jump = null;
        }

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
                    isGrounded = true;
                    jumpsRemaining = 1;
                    dashesRemaining = 1;
                    mModel.localScale = (mModel.localScale + (Vector3.forward * (mModel.localScale.z * 0.4f)) - Vector3.up * (mModel.localScale.y * 0.3f));
                }
                break;
            case 11:
                isAlive = false;
                StartCoroutine(TortiseSpinOut());
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
