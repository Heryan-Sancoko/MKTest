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
    public AudioSource mAudio;

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

    private void OnTriggerEnter(Collider other)
    {
        if (rbody.isKinematic && other.gameObject.layer == 8 || rbody.isKinematic && other.gameObject.layer == 12)
        {
            if (gameObject.layer != 11)
            {
                if (other.gameObject.layer == 8)
                {
                    if (other.transform.parent && other.transform.parent.GetComponent<PlayerBehaviour>())
                        other.transform.parent.GetComponent<PlayerBehaviour>().mScore.watermelonBonus += 0.1f;
                    else
                        Debug.Log("PlayerBehaviour script or parent does not exist on the trigger i've just entered");
                }
                else if (other.gameObject.layer == 12)
                {
                    if (other.GetComponent<PlayerBehaviour>())
                        other.GetComponent<PlayerBehaviour>().mScore.watermelonBonus += 0.1f;
                    else
                        Debug.Log("PlayerBehaviour script does not exist on the trigger i've just entered");
                }
                if (mAudio != null)
                    mAudio.Play();
                else
                    Debug.Log("No audio on this prefab");

                if (mParticle != null)
                    mParticle.Play();
                else
                    Debug.Log("No particle on this prefab");

                transform.position = Vector3.down * 50;
            }
            else if (gameObject.layer == 11 && other.gameObject.layer == 8)
            {
                mAudio.Play();
                mParticle.Play();
                transform.position = Vector3.down * 50;
            }
        }
    }
}
