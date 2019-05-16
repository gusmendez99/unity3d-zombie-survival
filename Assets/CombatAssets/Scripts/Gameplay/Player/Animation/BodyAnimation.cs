/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    [RequireComponent( typeof( Animation ) )]
    public class BodyAnimation : MonoBehaviour
    {
        [SerializeField]
        private AnimationClip
            idle = null, walk = null, run = null,
            strafeLeft = null, strafeRight = null,
            crouch = null, crouchWalk = null,
            crouchStrafeLeft = null, crouchStrafeRight = null,
            falling = null;

        [SerializeField]
        private float walkSpeed = 1.2f;
        [SerializeField]
        private float backwardsSpeed = .72f;
        [SerializeField]
        private float runSpeed = 1.15f;
        [SerializeField]
        private float strafeSpeed = .8f;
        [SerializeField]
        private float crouchWalkSpeed = .75f;
        [SerializeField]
        private float crouchBackwardsSpeed = .45f;
        [SerializeField]
        private float crouchStrafeSpeed = .5f;

        [SerializeField]
        private float halfStrafeAngle = 35f;
        [SerializeField]
        private float hSAngleSmooth = 7f;

        private Animation m_Animation = null;
        private Transform m_Transform = null;
        private float smoothAngle = 0f;

        
        // Awake
        void Awake()
        {
            m_Transform = transform;
            
            m_Animation = this.GetComponent<Animation>();
            m_Animation.clip = null;
            m_Animation.wrapMode = WrapMode.Loop;
            m_Animation.playAutomatically = false;
            m_Animation.animatePhysics = false;
            m_Animation.cullingType = AnimationCullingType.AlwaysAnimate;
        }

        // Update
        void LateUpdate()
        {
            float moveVertical = ASKInputManager.moveVertical;
            float moveHorizontal = ASKInputManager.moveHorizontal;

            //
            if( FirstPersonController.isGrounded )
            {
                if( FirstPersonController.isCrouched )
                {
                    if( FirstPersonController.isMoving )
                    {
                        if( moveVertical == 0f && moveHorizontal != 0f )
                        {
                            if( moveHorizontal > 0f )
                            {
                                m_Animation[ crouchStrafeRight.name ].speed = crouchStrafeSpeed * moveHorizontal;
                                m_Animation.CrossFade( crouchStrafeRight.name, .33f );
                            }
                            else
                            {
                                m_Animation[ crouchStrafeLeft.name ].speed = crouchStrafeSpeed * -moveHorizontal;
                                m_Animation.CrossFade( crouchStrafeLeft.name, .33f );
                            }
                        }
                        else
                        {
                            m_Animation[ crouchWalk.name ].speed = moveVertical > 0f ? 
                                crouchWalkSpeed * moveVertical : crouchBackwardsSpeed * moveVertical;
                            m_Animation.CrossFade( crouchWalk.name, .33f );

                            StrafeRotation();
                        }
                    }
                    else
                    {
                        m_Animation.CrossFade( crouch.name, .33f );
                    }
                }
                else
                {
                    if( FirstPersonController.isMoving )
                    {
                        if ( FirstPersonController.isRunning )
                        {
                            m_Animation[ run.name ].speed = runSpeed;
                            m_Animation.CrossFade( run.name, .33f );

                            StrafeRotation();
                        }
                        else
                        {
                            if( moveVertical == 0f && moveHorizontal != 0f )
                            {
                                if( moveHorizontal > 0f )
                                {
                                    m_Animation[ strafeRight.name ].speed = strafeSpeed * moveHorizontal;
                                    m_Animation.CrossFade( strafeRight.name, .33f );
                                }
                                else
                                {
                                    m_Animation[ strafeLeft.name ].speed = strafeSpeed * -moveHorizontal;
                                    m_Animation.CrossFade( strafeLeft.name, .33f );
                                }
                            }
                            else
                            {
                                m_Animation[ walk.name ].speed = moveVertical > 0f ? 
                                    walkSpeed * moveVertical : backwardsSpeed * moveVertical;
                                m_Animation.CrossFade( walk.name, .33f );

                                StrafeRotation();
                            }
                        }
                    }
                    else
                    {
                        m_Animation[ idle.name ].speed = 1f;
                        m_Animation.CrossFade( idle.name, .33f );

                        ResetRotation();
                    }
                }
            }
            else
            {
                m_Animation[ falling.name ].speed = 1f;
                m_Animation.CrossFade( falling.name, .33f );

                ResetRotation();
            }
        }

        
        // StrafeRotation
        private void StrafeRotation()
        {
            float smoothTime = hSAngleSmooth * Time.deltaTime;
            float moveVertical = ASKInputManager.moveVertical;
            float moveHorizontal = ASKInputManager.moveHorizontal;

            if( moveVertical != 0f && moveHorizontal != 0f )
            {
                if( moveVertical > 0f )
                    smoothAngle = Mathf.Lerp( smoothAngle, moveHorizontal > 0f ? halfStrafeAngle : -halfStrafeAngle, smoothTime );
                else
                    smoothAngle = Mathf.Lerp( smoothAngle, moveHorizontal > 0f ? -halfStrafeAngle : halfStrafeAngle, smoothTime );
            }
            else
            {
                smoothAngle = Mathf.Lerp( smoothAngle, 0f, smoothTime );
            }

            m_Transform.localEulerAngles = Vector3.up * smoothAngle;
        }

        // ResetRotation
        private void ResetRotation()
        {
            smoothAngle = Mathf.Lerp( smoothAngle, 0f, Time.deltaTime * hSAngleSmooth );
            m_Transform.localEulerAngles = Vector3.up * smoothAngle;
        }
    }
}