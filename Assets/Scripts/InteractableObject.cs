using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private Transform mParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalRotation;
    private Rigidbody rbody;
    private bool isGravityEnabled;
    public ParticleSystem mParticle;

    // Start is called before the first frame update
    void Awake()
    {
        mParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalRotation = transform.rotation;
        rbody = GetComponent<Rigidbody>();
        isGravityEnabled = rbody.useGravity;
    }

    private void OnEnable()
    {
        ResetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mParent.gameObject.activeSelf)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        transform.parent = mParent;
        transform.localPosition = originalLocalPosition;
        transform.rotation = originalRotation;
        rbody.useGravity = isGravityEnabled;
        rbody.angularVelocity = Vector3.zero;
        rbody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12 && gameObject.layer != 11)
        {
            collision.gameObject.GetComponent<PlayerBehaviour>().mScore.watermelonBonus += 0.1f;
            mParticle.Play();
            transform.position = Vector3.down * 50;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (rbody.isKinematic && other.gameObject.layer == 8)
        {
            if (gameObject.layer != 11)
            {
                other.transform.parent.GetComponent<PlayerBehaviour>().mScore.watermelonBonus += 0.1f;
            }
            mParticle.Play();
            transform.position = Vector3.down * 50;
        }
    }
}
