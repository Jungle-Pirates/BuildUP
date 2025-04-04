using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformMove : MonoBehaviour
{
    private GameObject currentOneWayPlatform;

    [SerializeField] private Collider2D playerCollider;
    [Tooltip("The time during which the player ignores the platform collider when pressing the down key.")]
    [SerializeField] private float ignoreCollisionTime = .5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentOneWayPlatform != null)
            {
                StartCoroutine(IE_DisablePlatformCollision());
            }
        }
    }

    private IEnumerator IE_DisablePlatformCollision()
    {
        BoxCollider2D platformCollidir = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollidir);
        yield return new WaitForSeconds(ignoreCollisionTime);
        Physics2D.IgnoreCollision(playerCollider, platformCollidir, false);
    }
}
