using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5;

    public Transform arrow;

    public GameObject bulletPrefab;

    public UnityEvent StartGame;
    public UnityEvent BulletHit;


    private Rigidbody rb;

    private Coroutine aimC;
    private bool aiming = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        aimC = StartCoroutine(AimBullet());
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector3 moveVector = context.ReadValue<Vector2>();

        moveVector.z = moveVector.y;
        moveVector.y = 0;

        rb.velocity = moveVector * moveSpeed;
    }

    public void Shoot()
    {
        if(aiming)
        {
            aiming = false;

            // stop coroutine
            StopCoroutine(aimC);

            // disable arrow
            arrow.gameObject.SetActive(false);

            // move/rotate bullet
            GameObject b = Instantiate(bulletPrefab, transform.position + arrow.forward + Vector3.up, arrow.rotation);

            StartGame.Invoke();
        }
    }

    private IEnumerator AimBullet()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        Camera mainCam = Camera.main;
        while(true)
        {
            yield return wait;
            // get mouse position
            RaycastHit hit;
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(mousePos);

            if(Physics.Raycast(ray, out hit))
            {
                //get direction from player
                Vector3 point = hit.point;
                point.y = 0;
                Vector3 dir = point - transform.position;

                // rotate arrow towards position
                arrow.transform.rotation = Quaternion.LookRotation(dir);
            }


            

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Bullet"))
        {
            BulletHit.Invoke();
            collision.rigidbody.velocity = Vector3.zero;
        }
    }
}
