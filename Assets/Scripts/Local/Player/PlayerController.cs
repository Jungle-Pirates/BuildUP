using UnityEngine;
using Mirror;
using TMPro;
using Steamworks;
using Cinemachine;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SyncVar(hook = nameof(OnDisplayNameChanged))]
    private string displayName;

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] float m_climbSpeed = 4.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    [SyncVar(hook = nameof(OnFacingDirectionChanged))]
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private float vertical;
    private PlayerColliderController playerColliderController;

    [Header("???? ???")]
    public GameObject attackPoint;  //???? ???? ?????? ??????? 
    //?¥ê??? ????? InventoryManager
    //private bool isEquipped = false; //??? ??? ???? ????


    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer && SteamManager.Initialized)
        {
            // ?? ????? ??????? ?????? ????
            string myName = SteamFriends.GetPersonaName();
            CmdSetDisplayName(myName);
            // ?? ???? ?????
            virtualCamera.gameObject.SetActive(true);
            //?????? ?ÄØ???? +1
            GetComponent<SpriteRenderer>().sortingOrder += 1;
        }
        playerColliderController = transform.Find("PlayerCollider").GetComponent<PlayerColliderController>();

        transform.position = new Vector3(Random.Range(-10, 10), 1, 0);

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        attackPoint = transform.Find("AttackPoint").gameObject;
        //attackPoint.SetActive(false); //?????? Use()???? Collider2D ????????? ???? ????? 
    }

    /// <summary>
    /// ?????? ????? ????????? ?????? Command
    /// </summary>
    [Command]
    private void CmdSetDisplayName(string myName)
    {
        // ???????? ??? ???? (SyncVar?? ???? ??? ????????? ?????)
        displayName = myName;
    }

    /// <summary>
    /// ????? ????? ?? ????? Hook ???
    /// </summary>
    private void OnDisplayNameChanged(string oldName, string newName)
    {
        playerName.text = newName;
    }

    /// <summary>
    /// ?????? ?¡À???? ???? ?????? ????????? ?????? Command
    /// </summary>
    /// <param name="direction"></param>
    [Command]
    private void CmdSetFacingDirection(int direction)
    {
        m_facingDirection = direction;
    }

    /// <summary>
    /// ?????? ????? ?? ????? Hook ???
    /// </summary>
    private void OnFacingDirectionChanged(int oldDirection, int newDirection)
    {
        GetComponent<SpriteRenderer>().flipX = newDirection == -1;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
            CmdSetFacingDirection(m_facingDirection);
            //flipX of attackPoint
            attackPoint.transform.localPosition = new Vector3(1.0f, 0.7f, 0.0f);
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            CmdSetFacingDirection(m_facingDirection);
            //flipX of attackPoint
            attackPoint.transform.localPosition = new Vector3(-1.0f, 0.7f, 0.0f);
        }

        // Move
        if (!m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        //??? ?????????? ??? > ??? ?????????? ??? ???????? Use???
        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            if (IsHandEquipped())  // ??? ??? ???? ???? ???
            {
                InventoryManager.Instance.slots[0].inventoryItem?.Use(this);  // ????? Use() ???


                m_currentAttack++;
                // Loop back to one after third attack
                if (m_currentAttack > 3)
                    m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                    m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);

                // Reset timer
                m_timeSinceAttack = 0.0f;
            }
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }


        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
        //Climb
        if (playerColliderController.OverlappingLadderCount > 0)
        {
            vertical = Input.GetAxisRaw("Vertical"); // W, S ??? ??, ?? ? ????
        }
    }
    void FixedUpdate()
    {
        if (playerColliderController.OverlappingLadderCount > 0)
        {
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, vertical * m_climbSpeed);
            m_body2d.gravityScale = 0f; // ??? ???? (???????? ?¥å÷é?? ???)
        }
        else
        {
            m_body2d.gravityScale = 1f; // ??? ???? ??? ????
        }
    }

    /// <summary>
    /// ?????? ??? ???
    /// </summary>
    private bool IsHandEquipped()
    {
        var item = InventoryManager.Instance.slots[0].inventoryItem;
        return item != null && item.equipable == Equipable.Hand;
    }


    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    /// <summary>
    /// ???? ???? ???? ?????? ??? : ??? ????????? ???
    /// </summary>
    public Item GetEquippedItem()
    {
        Slot handSlot = InventoryManager.Instance.slots[0];
        if (handSlot != null && handSlot.inventoryItem != null &&
            handSlot.inventoryItem.equipable == Equipable.Hand)
        {
            return handSlot.inventoryItem;
        }

        return null;
    }
    /// <summary>
    /// ???????? ???????. ???? ???????? ???? ???????? ???? ?????? ?????? ???????
    /// ?¥ê??????? ??????? ?? InventoryManager???? ???
    /// </summary>
    [Command (requiresAuthority = false)]
    public void ThrowItem(string itemCode)
    {
        // ?????? ???
        NetworkItemManager.Instance.SpawnItem(itemCode, transform.position, false);
    }

    /// <summary>
    /// ??? ???? ????
    /// </summary>
    /*
    private IEnumerator AttackPointEnable()
    {
        attackPoint.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackPoint.SetActive(false);
    }
    */
}