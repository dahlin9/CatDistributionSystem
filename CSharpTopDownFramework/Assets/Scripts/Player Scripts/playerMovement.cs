using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// A class to control the top down character.
/// Implements the player controls for moving and shooting.
/// Updates the player animator so the character animates based on input.
/// </summary>
public class playerMovement : MonoBehaviour
{
    #region Variables

    //The inputs that we need to retrieve from the input system.
    private InputAction m_moveAction;
    private InputAction m_attackAction;

    //The components that we need to edit to make the player move smoothly.
    private Animator m_animator;
    private Rigidbody2D m_rigidbody;
    
    //The direction that the player is moving in.
    private Vector2 m_playerDirection;

    //Reference to Teleport value
    public VectorValue startingPosition;

    //Reference to walkingSFX
    private AudioSource walkingsfx;

    //Make sure walkingSFX doesn't spam play
    private bool canHearWalking;
   

    [Header("Movement parameters")]
    //The speed at which the player moves
    [SerializeField] private float m_playerSpeed = 200f;
    //The maximum speed the player can move
    [SerializeField] private float m_playerMaxSpeed = 1000f;

    #endregion

    /// <summary>
    /// When the script first initialises this gets called.
    /// Use this for grabbing components and setting up input bindings.
    /// </summary>
    private void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_attackAction = InputSystem.actions.FindAction("Attack");

        m_moveAction.performed += OnMove;
        m_moveAction.canceled += StopMove;
        //m_attackAction.performed += OnAttack;


        //get components from Character game object so that we can use them later.
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Called after Awake(), and is used to initialize variables e.g. set values on the player
    /// </summary>
    void Start()
    {
        transform.position = startingPosition.initialValue;
        walkingsfx = GetComponent<AudioSource>();
    }

    /// <summary>
    /// When a fixed update loop is called, it runs at a constant rate, regardless of pc performance.
    /// This ensures that physics are calculated properly.
    /// </summary>
    private void FixedUpdate()
    {
        //clamp the speed to the maximum speed for if the speed has been changed in code.
        float speed = m_playerSpeed > m_playerMaxSpeed ? m_playerMaxSpeed : m_playerSpeed;
        
        //apply the movement to the character using the clamped speed value.
        m_rigidbody.linearVelocity = m_playerDirection * (speed * Time.fixedDeltaTime);
    }

    #region Movement Handler Functions
    
    public void OnMove(InputAction.CallbackContext context)
    {
        m_playerDirection = context.ReadValue<Vector2>();

        HandleAnimOnMove();
    }

    public void StopMove(InputAction.CallbackContext context)
    {
        m_playerDirection = Vector2.zero;

        HandleAnimOnMove();
    }

    private void HandleAnimOnMove()
    {
        m_animator.SetFloat("Speed", m_playerDirection.magnitude);

        if (m_playerDirection.magnitude > 0)
        {
            m_animator.SetFloat("Horizontal", m_playerDirection.x);
            m_animator.SetFloat("Vertical", m_playerDirection.y);
        }
    }

    #endregion
    void Update()
    {

        if (m_moveAction.IsPressed() && !walkingsfx.isPlaying)
        {
            walkingsfx.Play();
        }
        else if (!m_moveAction.IsPressed() && walkingsfx.isPlaying)
        {
            walkingsfx.Stop();
        }
    }
}
