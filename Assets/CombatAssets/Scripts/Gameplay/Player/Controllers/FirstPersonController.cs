/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using System.Collections;


namespace AdvancedShooterKit
{
    [RequireComponent( typeof( CameraHeadBob ) )]
    [RequireComponent( typeof( FootstepSFXManager ) )]
    [RequireComponent( typeof( CharacterController ) )]
    public class FirstPersonController : MonoBehaviour
    {
        // Fields for parameters 
        [SerializeField]
        private bool canWalk = true;
        [SerializeField]
        private float walkSpeed = 4.25f;
        [SerializeField]
        private float backwardsSpeed = .6f;
        [SerializeField]
        private float sidewaysSpeed = .7f;
        [SerializeField]
        private float inAirSpeed = .35f;

        [SerializeField]
        private bool canRun = true;
        [SerializeField]
        private float runSpeed = 8.75f;

        [SerializeField]
        private bool canCrouch = true;
        [SerializeField]
        private float crouchSpeed = .45f;
        [SerializeField]
        private float crouchHeight = 1.25f;

        [SerializeField]
        private bool canJump = true;
        [SerializeField]
        private float jumpForce = 5f;

        [SerializeField]
        private bool canClimb = true;
        [SerializeField]
        private float climbingSpeed = .8f;

        [SerializeField]
        private bool useHeadBob = true;
        [SerializeField]
        private float posForce = .65f;
        [SerializeField]
        private float tiltForce = .85f;

        [SerializeField]
        private float gravityMultiplier = 2f;
        [SerializeField]
        private float fallingDistanceToDamage = 3f;
        [SerializeField]
        private float fallingDamageMultiplier = 3.5f;

        [SerializeField]
        private float stepInterval = .5f;

        [SerializeField]
        private float lookSmooth = .721f;
        [SerializeField]
        private float maxLookAngleY = 65f;
        [SerializeField]
        private Vector3 cameraOffset = Vector3.up;


        public static bool isGrounded { get; private set; }
        public static bool isClimbing { get; private set; }
        public static bool isMoving { get; private set; }
        public static bool isRunning { get; private set; }
        public static bool isCrouched { get; private set; }
        public static bool isJumping { get; private set; }
        public static bool isFalling { get; private set; }


        public static RaycastHit floorHit { get { return hitInfo;  } }
        private static RaycastHit hitInfo;


        // Fields for move calculation
        private bool prevGrounded, jump, crouching;

        private CharacterController m_Controller = null;
        private CollisionFlags collisionFlags = CollisionFlags.None;
        private Transform m_Transform, cameraTransform;
        private Vector3 moveDirection, crouchVelVec;
        private float nextStep, nativeCapsuleHeight, crouchVel, fallingStartPos, fallingDist;
        private ILadder currentLadder = null;
        private IFootstepSFXManager m_FootstepSFXManager = null;
        private static FirstPersonController instance = null;

        // Fields for look calculation
        private float rotationX, rotationY;
        private Quaternion nativeRotation = Quaternion.identity;
        

        // Awake
        void Awake()
        {
            instance = this;
            m_Controller = this.GetComponent<CharacterController>();
            m_Controller.center = Vector3.up;
            nativeCapsuleHeight = m_Controller.height;
            m_FootstepSFXManager = this.GetComponent<IFootstepSFXManager>();
        }

        // Start
        void Start()
        {
            m_Transform = transform;
            cameraTransform = PlayerCamera.m_Transform;

            nativeRotation = cameraTransform.localRotation;
            nativeRotation.eulerAngles = Vector3.up * cameraTransform.localEulerAngles.y;
        }

        // OnEnable
        void OnEnable()
        {
            isMoving = isGrounded = isClimbing = isCrouched = false;
        }

        // FixedUpdate
        void FixedUpdate()
        {
            isGrounded = m_Controller.isGrounded;
            if( isGrounded )
            {
                if( isFalling )
                {
                    isFalling = false;

                    if( fallingDist > fallingDistanceToDamage )
                    {
                        int damage = Mathf.RoundToInt( fallingDist * fallingDamageMultiplier );
                        HudElements.HealthBar.SetDamageTargetPosition( Vector3.zero );
                        PlayerCharacter.Instance.DecrementHealth( damage );
                    }

                    fallingDist = 0f;
                }
            }
            else
            {
                if( isFalling )
                {
                    fallingDist = fallingStartPos - m_Transform.position.y;
                }
                else
                {
                    if( isClimbing == false )
                    {
                        isFalling = true;
                        fallingStartPos = m_Transform.position.y;
                    }
                }
            }



            //CameraLook();
            Movement();
            PlayFootStepAudio();

            if( isClimbing == false && isGrounded == false && isJumping == false && prevGrounded )
                moveDirection.y = 0f;

            prevGrounded = isGrounded;
        }

        void Update()
        {
            CameraLook();
        }


        // Jump
        internal static void Jump()
        {          
            if( isClimbing || instance.crouching || isGrounded == false )
                return;

            if( isCrouched )
            {
                Crouch();
                return;
            }

            if( instance.canJump && instance.canWalk && instance.jump == false && isJumping == false )
                instance.jump = true;
        }

        // Crouch
        internal static void Crouch()
        {
            if( instance.canCrouch == false || isClimbing || instance.crouching || isGrounded == false )
                return;

            instance.crouching = true;

            if( isCrouched )
            {
                if( Physics.SphereCast( instance.m_Transform.position + Vector3.up * .75f, instance.m_Controller.radius, Vector3.up, out hitInfo, instance.nativeCapsuleHeight * .25f ) )
                {
                    //Debug.Log( "StandUp return" );
                    instance.crouching = false;                    
                    return;
                }

                //Debug.Log( "StandUp RUN" );
                instance.StartCoroutine( instance.StandUp() );
            }
            else
            {
                instance.StartCoroutine( instance.SitDown() );
            }
        }
        
        // Movement
        private void Movement()
        {
            float horizontal = ASKInputManager.moveHorizontal * Time.timeScale; // move Left/Right
            float vertical = ASKInputManager.moveVertical * Time.timeScale;     // move Forward/Backward

            bool moveForward = ( vertical > 0f );

            vertical *= ( moveForward ? 1f : backwardsSpeed );
            horizontal *= sidewaysSpeed;            

            Quaternion screenMovementSpace = Quaternion.Euler( 0f, cameraTransform.eulerAngles.y, 0f );
            Vector3 forwardVector = screenMovementSpace * Vector3.forward * vertical;
            Vector3 rightVector = screenMovementSpace * Vector3.right * horizontal;
            Vector3 moveVector = forwardVector + rightVector;

            if( isClimbing )
            {
                bool lookUp = cameraTransform.forward.y > -.4f;

                if( moveForward )
                {
                    forwardVector = currentLadder.m_Transform.up * vertical;
                    forwardVector *= lookUp ? 1f : -1f;
                }

                moveVector = forwardVector + rightVector;

                if( isGrounded )
                {
                    if( moveForward && lookUp == false )
                        moveVector += screenMovementSpace * Vector3.forward;
                }
                else
                {
                    if( moveForward && lookUp )
                        moveVector += screenMovementSpace * Vector3.forward;
                }

                moveDirection = moveVector * GetSpeed( moveForward );
            }
            else
            {
                if( isGrounded )
                {
                    Physics.SphereCast( m_Transform.position + m_Controller.center, m_Controller.radius, Vector3.down, out hitInfo, m_Controller.height * .5f );

                    moveDirection = moveVector * GetSpeed( moveForward );
                    moveDirection.y = -10f;

                    if( jump )
                    {
                        m_FootstepSFXManager.PlayJumpingSound( hitInfo );
                        isJumping = true;
                        jump = false;
                        moveDirection.y = jumpForce;
                    }
                }
                else
                {
                    moveDirection += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
                }
            }

            if( canWalk )
                collisionFlags = m_Controller.Move( moveDirection * Time.fixedDeltaTime );

            bool accelerated = ( m_Controller.velocity.magnitude > .01f );
            isMoving = isClimbing ? accelerated : isGrounded && accelerated;
        }
        // Get Speed
        private float GetSpeed( bool inForward )
        {
            bool runReady = ( canRun && isCrouched == false && inForward && FirstPersonWeaponSway.ironsightZoomed == false && FirstPersonWeaponSway.ironsightZooming == false );
            isRunning = ( ASKInputManager.runAction && runReady );

            /*
            if( ASKInputManager.GetButtonDown( runBtnName ) && runReady )            
                HudElements.Crosshair.SetActive( ECrosshairView.None );            
            if( ASKInputManager.GetButtonUp( runBtnName ) )            
                WeaponsManager.UpdateHud( true );            
            
            if( isRunning == false && ASKInputManager.GetButtonDown( runBtnName ) && runReady )
                isRunning = true;
            if( isRunning && isMoving == false )
                isRunning = false;
            */

            float speed = isRunning ? runSpeed : walkSpeed;
            speed = isCrouched ? speed * crouchSpeed : speed;
            speed = ( isGrounded || ( isJumping && jump == false ) ) ? speed : speed * inAirSpeed;
            speed = isClimbing ? speed * climbingSpeed : speed;
            return speed * FirstPersonWeaponSway.moveSpeed;
        }
        
        // Camera Look
        private void CameraLook()
        {
            rotationX += ASKInputManager.lookHorizontal * Time.timeScale;
            rotationY += ASKInputManager.lookVertical * Time.timeScale;

            rotationY = Mathf.Clamp( rotationY, -maxLookAngleY, maxLookAngleY );

            Quaternion camTargetRotation = nativeRotation * Quaternion.Euler( -1f * rotationY + ( useHeadBob ? CameraHeadBob.xTilt * tiltForce : 0f ), 0f, 0f );
            Quaternion bodyTargetRotation = nativeRotation * Quaternion.Euler( 0f, rotationX + ( useHeadBob ? CameraHeadBob.yTilt * tiltForce : 0f ), 0f );            

            float smoothRotation = lookSmooth * ( Time.deltaTime * 50f );
            cameraTransform.localRotation = Quaternion.Slerp( cameraTransform.localRotation, camTargetRotation, smoothRotation );
            m_Transform.localRotation = Quaternion.Slerp( m_Transform.localRotation, bodyTargetRotation, smoothRotation );

            Vector3 newCameraPosition = Vector3.zero;
            newCameraPosition.x = m_Controller.center.x + cameraOffset.x + ( useHeadBob ? CameraHeadBob.xPos * posForce : 0f ); //xPos
            newCameraPosition.y = ( m_Controller.center.y * 2f ) + cameraOffset.y + ( useHeadBob ? CameraHeadBob.yPos * posForce : 0f ); //yPos
            newCameraPosition.z = m_Controller.center.z + cameraOffset.z;
            cameraTransform.localPosition = newCameraPosition;
        }


        // StandUp
        private IEnumerator StandUp()
        {
            Vector3 targetCenter = Vector3.up;

            isCrouched = false;

            while( PlayCrouchAnimation( ref targetCenter, ref nativeCapsuleHeight ) )
                yield return null;

            m_Controller.height = nativeCapsuleHeight;
            m_Controller.center = targetCenter;

            crouching = false;
        }
        // SitDown
        private IEnumerator SitDown()
        {
            Vector3 targetCenter = Vector3.up * ( crouchHeight * .5f );

            isCrouched = true;

            while( PlayCrouchAnimation( ref targetCenter, ref crouchHeight ) )
                yield return null;

            m_Controller.height = crouchHeight;
            m_Controller.center = targetCenter;
            
            crouching = false;
        }
        // Play CrouchAnimation
        private bool PlayCrouchAnimation( ref Vector3 targetCenter, ref float targetHeight )
        {
            m_Controller.height = Mathf.SmoothDamp( m_Controller.height, targetHeight, ref crouchVel, Time.fixedDeltaTime * 5f );
            m_Controller.center = Vector3.SmoothDamp( m_Controller.center, targetCenter, ref crouchVelVec, Time.fixedDeltaTime * 5f );

            const int digits = 3;
            double cMag = System.Math.Round( ( double )m_Controller.center.magnitude, digits );
            double tMag = System.Math.Round( ( double )targetCenter.magnitude, digits );

            return ( cMag != tMag );
        }


        // Play FootStepAudio 
        private void PlayFootStepAudio()
        {
            if( prevGrounded == false && isGrounded )
            {
                m_FootstepSFXManager.PlayLandingSound( hitInfo );
                nextStep = CameraHeadBob.headBobCycle + stepInterval;
                isJumping = false;
                moveDirection.y = 0f;
                return;
            }

            if( CameraHeadBob.headBobCycle > nextStep )
            {
                nextStep = CameraHeadBob.headBobCycle + stepInterval;

                if( isGrounded )
                    m_FootstepSFXManager.PlayFootStepSound( hitInfo );
                else if( isClimbing )
                    currentLadder.PlayLadderFootstepSound();
            }
        }


        // Player Die
        internal static void PlayerDie()
        {
            instance.enabled = false;
            instance.m_Controller.height = .1f;
            instance.m_Controller.radius = .1f;
            //instance.GetComponent<CameraHeadBob>().enabled = false;
        }
               

        // OnController ColliderHit            
        void OnControllerColliderHit( ControllerColliderHit hit )
        {
            if( collisionFlags != CollisionFlags.Below )
            {
                Rigidbody tmpRb = hit.collider.attachedRigidbody;

                if( tmpRb != null && tmpRb.isKinematic == false )
                    tmpRb.AddForceAtPosition( hit.moveDirection * m_Controller.velocity.magnitude * 3f / tmpRb.mass, hit.point, ForceMode.VelocityChange );
            }
        }

        // OnTrigger Enter
        void OnTriggerEnter( Collider collider )
        {
            if( canClimb )
            {
                currentLadder = collider.GetComponent<ILadder>();
                if( currentLadder != null )
                {
                    if( isCrouched )
                        Crouch();

                    currentLadder.AssignAudioSource( GetComponent<AudioSource>() );
                    moveDirection = Vector3.zero;
                    isClimbing = true;
                }
            }
        }

        // OnTrigger Exit
        void OnTriggerExit( Collider collider )
        {
            if( isClimbing )
            {
                isClimbing = false;
                currentLadder = null;
            }
        }
        //
    }
}