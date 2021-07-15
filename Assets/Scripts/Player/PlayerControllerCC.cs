using Tools;
using UnityEngine;
using static DebugTools.DebugHelpers;

namespace PlayerMovement
{
    /// <summary>
    /// This script handles Quake III CPM(A) mod style player movement logic.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerControllerCC : MonoBehaviour, EventListener<GameEvent>
    {
        /// <summary>
        /// Event Handling
        /// </summary>

        public void OnEvent(GameEvent eventType)
        {
            //Nothing currently
        }

        private void OnEnable()
        {
            this.EventStartListening<GameEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameEvent>();
        }

        [System.Serializable]
        public class MovementSettings
        {
            public float MaxSpeed;
            public float Acceleration;
            public float Deceleration;

            public MovementSettings(float maxSpeed, float accel, float decel)
            {
                MaxSpeed = maxSpeed;
                Acceleration = accel;
                Deceleration = decel;
            }
        }

        //Player State Machine
        private PlayerState myStateMachine;

        [Header("Movement")]
        [SerializeField] private float m_Friction = 6;
        [SerializeField] private float m_Gravity = 20;
        private float savedGravity;
        [SerializeField] private float m_JumpForce = 8;
        [Tooltip("Automatically jump when holding jump button")]
        [SerializeField] private bool m_AutoBunnyHop = false;
        [Tooltip("How precise air control is")]
        [SerializeField] private float m_AirControl = 0.3f;
        [SerializeField] private MovementSettings m_GroundSettings = new MovementSettings(7, 14, 10);
        [SerializeField] private MovementSettings m_AirSettings = new MovementSettings(7, 2, 2);
        [SerializeField] private MovementSettings m_StrafeSettings = new MovementSettings(1, 50, 50);

        /// <summary>
        /// Returns player's current speed.
        /// </summary>
        public float Speed { get { return m_Character.velocity.magnitude; } }

        private CharacterController m_Character;
        private Vector3 m_MoveDirectionNorm = Vector3.zero;
        public Vector3 m_PlayerVelocity = Vector3.zero;

        
        
        private float height;
        
        [Header("Slope Handling")]
        private float heightWithPadding;
        private RaycastHit hitInfo;
        [SerializeField] private float heightPadding;
        [SerializeField] private LayerMask ground;
        [HideInInspector] public bool onSlope;
        [HideInInspector] public float groundAngle;
        [HideInInspector] public bool grounded;
        

        [Header("Debug")]
        [SerializeField] private bool drawDebugCast;

        // Used to queue the next jump just before hitting the ground.
        private bool m_JumpQueued = false;

        // Used to display real time friction values.
        private float m_PlayerFriction = 0;

        private Vector3 m_MoveInput;
        PlayerCameraController playerCameraController;

        private void Start()
        {
            playerCameraController = GetComponent<PlayerCameraController>();
            m_Character = GetComponent<CharacterController>();
            height = m_Character.height/2 + m_Character.skinWidth;
            heightWithPadding = heightPadding + height;
            myStateMachine = GetComponent<PlayerState>();
            savedGravity = m_Gravity;
        }

        private void Update()
        {
            m_MoveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            playerCameraController.m_MouseLook.UpdateCursorLock(); //Whether we show the cursor or not
            QueueJump();

            //Checking For Slope
            CheckGround();
            CalculateGroundAngle();

            // Set movement state.
            if (m_Character.isGrounded)
            {
                GroundMove();
            }
            else
            {
                AirMove();
            }

            // Rotate the character and camera.
            playerCameraController.m_MouseLook.LookRotation(playerCameraController.m_Tran, playerCameraController.m_CamTran);

            // Move the character.
            m_Character.Move(m_PlayerVelocity * Time.deltaTime);

            // Compensate for Downward Slope
            StickToSlope();
        }

        private void StickToSlope()
        {
            if(onSlope && m_Character.velocity.y <= 0)
            {
                //Lerp the characters position
                playerCameraController.m_Tran.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hitInfo.point.y + height, transform.position.z), 100 * Time.deltaTime);
            }
        }

        private void CheckGround()
        {
            //if(Physics.Raycast(transform.position, -Vector3.up, out hitInfo, height, ground))
            hitInfo = DebugRayCast(transform.position, -Vector3.up, heightWithPadding, ground, Color.blue, drawDebugCast);
            if (hitInfo.collider != null)
            {
                grounded = true; 
            }
            else
            {
                grounded = false;
            }
        }

        void CalculateGroundAngle()
        {
            if (grounded)
                groundAngle = Vector3.Angle(hitInfo.normal, transform.forward);
            else
                groundAngle = 90f;
            //Check if angle is less than 88 or greater than 92
            onSlope = (groundAngle <= 88f || groundAngle >= 92f);
        }

        // Queues the next jump.
        private void QueueJump()
        {
            if (m_AutoBunnyHop)
            {
                m_JumpQueued = Input.GetButton("Jump");
                return;
            }

            if (Input.GetButtonDown("Jump") && !m_JumpQueued)
            {
                m_JumpQueued = true;
            }

            if (Input.GetButtonUp("Jump"))
            {
                m_JumpQueued = false;
            }
        }

        
        public void DisableGravity()
        {
            m_Gravity = 0;
        }

        public void EnableGravity()
        {
            m_Gravity = savedGravity;
        }

        // Handle air movement.
        private void AirMove()
        {
            float accel;
            float mouseX = Input.GetAxis("Mouse X");
            if(mouseX != 0)
            {
                if (mouseX > 0.02f)
                    mouseX = 1;
                else if (mouseX < -0.02f)
                    mouseX = -1;
                else
                    mouseX = 0;
            }
            //var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z); 

            var wishdir = new Vector3(mouseX, 0, Mathf.Clamp(m_MoveInput.z, -1, 0)); // Gets the mouse X inputs, zero the forward input
            
            wishdir = playerCameraController.m_Tran.TransformDirection(wishdir); // get the local to world space for the players current transform to the requested new position 
            
            float wishspeed = wishdir.magnitude;
            wishspeed *= m_AirSettings.MaxSpeed;

            wishdir.Normalize();
            m_MoveDirectionNorm = wishdir;

            // CPM Air control.
            float wishspeed2 = wishspeed;
            if (Vector3.Dot(m_PlayerVelocity, wishdir) < 0)
            {
                accel = m_AirSettings.Deceleration;
            }
            else
            {
                accel = m_AirSettings.Acceleration;
            }

            // If the player is ONLY strafing left or right 
            // Disabled to make air strafing automatic
            if (true/*m_MoveInput.z == 0 && m_MoveInput.x != 0*/)
            {
                if (wishspeed > m_StrafeSettings.MaxSpeed)
                {
                    wishspeed = m_StrafeSettings.MaxSpeed;
                }

                accel = m_StrafeSettings.Acceleration;
            }

            Accelerate(wishdir, wishspeed, accel);
            
            if (m_AirControl > 0)
            {
                AirControl(wishdir, wishspeed2);
            }
            
            // Apply gravity
            m_PlayerVelocity.y -= m_Gravity * Time.deltaTime;
        }

        // Air control occurs when the player is in the air, it allows players to move side 
        // to side much faster rather than being 'sluggish' when it comes to cornering.
        private void AirControl(Vector3 targetDir /*wishDir*/, float targetSpeed)
        {
            // Only control air movement when moving backward
            
            if (Mathf.Abs(m_MoveInput.z) < 0.001 || Mathf.Abs(targetSpeed) < 0.001 || m_MoveInput.z > 0)
            {
                return;
            }

            float zSpeed = m_PlayerVelocity.y;
            m_PlayerVelocity.y = 0;
            /* Next two lines are equivalent to idTech's VectorNormalize() */
            float speed = m_PlayerVelocity.magnitude;
            m_PlayerVelocity.Normalize();

            float dot = Vector3.Dot(m_PlayerVelocity, targetDir);
            float k = 32;
            k *= m_AirControl * dot * dot * Time.deltaTime;

            // Change direction while slowing down.
            if (dot > 0)
            {
                m_PlayerVelocity.x *= speed + targetDir.x * k;
                m_PlayerVelocity.y *= speed + targetDir.y * k;
                m_PlayerVelocity.z *= speed + targetDir.z * k;

                m_PlayerVelocity.Normalize();
                m_MoveDirectionNorm = m_PlayerVelocity;
            }

            m_PlayerVelocity.x *= speed;
            m_PlayerVelocity.y = zSpeed; // Note this line
            m_PlayerVelocity.z *= speed;
        }

        // Handle ground movement.
        private void GroundMove()
        {
            // Do not apply friction if the player is queueing up the next jump
            if (!m_JumpQueued)
            {
                ApplyFriction(1.0f);
            }
            else
            {
                ApplyFriction(0);
            }

            var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
            wishdir = playerCameraController.m_Tran.TransformDirection(wishdir);
            wishdir.Normalize();
            m_MoveDirectionNorm = wishdir;

            var wishspeed = wishdir.magnitude;
            wishspeed *= m_GroundSettings.MaxSpeed;

            Accelerate(wishdir, wishspeed, m_GroundSettings.Acceleration);

            // Reset the gravity velocity
            m_PlayerVelocity.y = -m_Gravity * Time.deltaTime;

            if (m_JumpQueued)
            {
                m_PlayerVelocity.y = m_JumpForce;
                m_JumpQueued = false;
            }
        }

        private void ApplyFriction(float t)
        {
            // Equivalent to VectorCopy();
            Vector3 vec = m_PlayerVelocity; 
            vec.y = 0;
            float speed = vec.magnitude;
            float drop = 0;

            // Only apply friction when grounded.
            if (m_Character.isGrounded)
            {
                float control = speed < m_GroundSettings.Deceleration ? m_GroundSettings.Deceleration : speed;
                drop = control * m_Friction * Time.deltaTime * t;
            }

            float newSpeed = speed - drop;
            m_PlayerFriction = newSpeed;
            if (newSpeed < 0)
            {
                newSpeed = 0;
            }

            if (speed > 0)
            {
                newSpeed /= speed;
            }

            m_PlayerVelocity.x *= newSpeed;
            // playerVelocity.y *= newSpeed;
            m_PlayerVelocity.z *= newSpeed;
        }

        // Calculates acceleration based on desired speed and direction.
        private void Accelerate(Vector3 targetDir, float targetSpeed, float accel)
        {
            float currentspeed = Vector3.Dot(m_PlayerVelocity, targetDir);
            float addspeed = targetSpeed - currentspeed;
            if (addspeed <= 0)
            {
                return;
            }

            float accelspeed = accel * Time.deltaTime * targetSpeed;
            if (accelspeed > addspeed)
            {
                accelspeed = addspeed;
            }

            m_PlayerVelocity.x += accelspeed * targetDir.x;
            m_PlayerVelocity.z += accelspeed * targetDir.z;
        }

        
    }
}