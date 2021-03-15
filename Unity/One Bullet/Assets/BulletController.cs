using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    private float speed = 15f;
    [SerializeField]
    private float splitTime = 5f;

    private Rigidbody rb;

    public GameObject bulletPrefab;
    public Transform split1;
    public Transform split2;
    public int generation = 0;
    public int generationLimit = 5;

    public ParticleSystem particles;

    [Header("Audio")]
    public AudioSource bounceSound;
    public AudioSource explodeSound;
    public AudioClip explosion;

    private float startTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        //rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bounceSound.Play();
    }

    private void Update()
    {
        if(Time.time - startTime >= splitTime)
        {
            if (generation >= generationLimit) return;
            Split();
        }
    }

    public void Split()
    {
        GameObject s1 = Instantiate(bulletPrefab, transform.position, split1.rotation);
        s1.GetComponent<BulletController>().generation = generation + 1;
        GameObject s2 = Instantiate(bulletPrefab, transform.position, split2.rotation);
        s2.GetComponent<BulletController>().generation = generation + 1;
        Destroy(gameObject);
    }

    public void Die()
    {
        // disable visual
        GetComponent<Renderer>().enabled = false;
        // play particles
        particles.Play();
        // audio
        //bounceSound.clip = explosion;
        explodeSound.Play();

        // screen shake
        GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        // gameObject.SetActive(false);
    }


}
