/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using System.Collections;


#pragma warning disable 169

namespace AdvancedShooterKit
{    
    public class FirstPersonWeaponSway : MonoBehaviour
    {
        // Sway
        [SerializeField]
        private bool useSway = true;
        [SerializeField]
        private float swaySmoothing = 2.2f;
        [SerializeField]
        private float borderSize = 25f;
        [SerializeField]
        private float fpMoveSpeed = 1f;
        internal static float moveSpeed { get; set; }

        // Animation
        [SerializeField]
        private AnimationClip fireClip = null;
        [SerializeField]
        private float fireAnimSpeed = 1f;
        public AnimationClip reloadClip = null;
        [SerializeField]
        private float reloadAnimSpeed = 1f;
        [SerializeField]
        private float dropoutSmoothing = 4.5f;
        [SerializeField]
        private Vector3 dropoutRotation;
        public Vector3 runOffset;
        internal static bool isChanging { get; private set; }
        internal static bool isPlaying { get { return instance.m_Animation.isPlaying; } }

        // Ironsighting
        [SerializeField]
        private bool useIronsighting = true;        
        [SerializeField]
        private float ironsightSmoothing = 3.5f;
        [SerializeField]
        private float ironsightDispersion = .5f;
        [SerializeField]
        private float addMove = .85f;
        [SerializeField]
        private float addLook = .4f;
        [SerializeField]
        private float addRunAndInAir = 1.15f;
        [SerializeField]
        private float crouched = .55f;
        [SerializeField]
        private float zoomFOV = 20f;
        [SerializeField]
        private float ironsightMoveSpeed = 1f;
        [SerializeField]
        private Vector3 zoomPos = Vector3.zero;        
        internal static bool ironsightZoomed { get; private set; }
        internal static bool ironsightZooming { get; private set; }

        // Crosshair
        public ECrosshairView crosshairView = ECrosshairView.All;

        // Pivot
        public Vector3 pivotPosition = Vector3.zero;

        private static FirstPersonWeaponSway instance = null;


        // Sway  
        private Transform m_Parent, m_Transform, cameraTransform;
        private Vector3 nativePosition, noisePosition;
        private Vector3 prevRotation, prevVelocity;
        private float halfBorderSize, fullBorderSize;
        private float divider = 400f;
        private float vel01, vel02, vel03, magVel;
        private float idleX, idleY;
        private float multiplier = 1f;

        // Animation
        private Animation m_Animation = null;
        private Vector3 dropinRotation = Vector3.zero;
        private Vector3 velRotation = Vector3.zero;
        
        // Ironsighting
        private bool meleeWeapon = false;
        private float mainCamNativeFOV, secondCamNativeFOV;
        private Vector3 zoomVel = Vector3.zero;
        private float camFovVel = 0f;
        private Firearms m_Firearm = null;
        private float nativeDispersion = 0f;
        private const int ironsightingDigits = 4;

        // Pivot
        private Vector3 parentPosition, prevPivotPosition;

                
        // Awake
        void Awake()
        {
            isChanging = false;
            ironsightZoomed = ironsightZooming = false;

            halfBorderSize = borderSize / 2.75f;
            fullBorderSize = borderSize;

            m_Transform = transform;
            m_Parent = m_Transform.parent;

            m_Animation = GetComponentInChildren<Animation>();

            dropinRotation = m_Transform.localEulerAngles;

            if( fireClip != null )
                m_Animation[ fireClip.name ].speed = fireAnimSpeed;

            if( reloadClip != null )
                m_Animation[ reloadClip.name ].speed = reloadAnimSpeed; 

            parentPosition = m_Parent.localPosition;

            m_Firearm = GetComponentInParent<Firearms>();

            if( m_Firearm != null )
            {
                nativeDispersion = m_Firearm.dispersion;
            }
            else
            {
                meleeWeapon = true;
                useIronsighting = false;                
            }

            SetPivot();

            m_Parent.localPosition = noisePosition = nativePosition;
        }

        // Start
        void Start()
        {
            cameraTransform = PlayerCamera.m_Transform;
            mainCamNativeFOV = PlayerCamera.mainCamera.fieldOfView;
            secondCamNativeFOV = PlayerCamera.secondCamera.fieldOfView;
        }

        // OnEnable
        void OnEnable()
        {
            moveSpeed = useSway ? fpMoveSpeed : 1f;
            instance = this;
        }


        // Update
        void Update()
        {
            if( useIronsighting && meleeWeapon == false && isPlaying == false && isChanging == false )
                Ironsighting();

            if( prevPivotPosition != pivotPosition )
                SetPivot();

            //isPlaying = m_Animation.isPlaying;
        }
        
        // Late Update
        void LateUpdate()
        {
            Sway();
            //            

            if( meleeWeapon || useIronsighting == false )
                return;

            float newMultiplier = 1f;

            if( FirstPersonController.isMoving ) // AddMove
                newMultiplier += addMove;

            if( ( ASKInputManager.lookHorizontal != 0 ) || ( ASKInputManager.lookVertical != 0 ) ) // AddLook
                newMultiplier += addLook;

            if( FirstPersonController.isRunning || FirstPersonController.isGrounded == false ) // AddRunningAndInAir
                newMultiplier += addRunAndInAir;

            newMultiplier = FirstPersonController.isCrouched ? newMultiplier * crouched : newMultiplier; // Crouched

            multiplier = Mathf.SmoothDamp( multiplier, newMultiplier, ref magVel, Time.smoothDeltaTime * 5f );            
            
            if( ironsightZoomed )
                m_Firearm.dispersion = nativeDispersion * ironsightDispersion * multiplier;
            else
                m_Firearm.dispersion = nativeDispersion * multiplier;

            HudElements.Crosshair.UpdatePosition( m_Firearm.dispersion ); 
        }

        
        // Sway
        private void Sway()
        {
            if( useSway )
            {
                idleX = Mathf.Sin( Time.time * 1.25f ) + CameraHeadBob.xPos * 50f;
                idleY = Mathf.Sin( Time.time * 1.5f ) + CameraHeadBob.yPos * 50f;

                float dividerDeltaTime = Time.smoothDeltaTime * 1000f;

                if( ironsightZoomed )
                {
                    if( divider < 1600f )
                        divider += dividerDeltaTime;

                    idleX /= divider;
                    idleY /= divider;

                    if( ironsightZooming == false )
                        noisePosition = zoomPos;
                }
                else
                {
                    if( FirstPersonController.isMoving )
                    {
                        if( divider < 800f )
                            divider += dividerDeltaTime;
                        else if( divider > 800f )
                            divider -= dividerDeltaTime;

                        idleX /= divider;
                        idleY /= divider;
                    }
                    else
                    {
                        if( divider > 400f )
                            divider -= dividerDeltaTime;

                        idleX /= divider;
                        idleY /= divider;
                    }

                    if( ironsightZooming == false )
                        noisePosition = nativePosition;
                }
            }
            else
            {
                idleX = idleY = 0f;
            }

            if( ironsightZooming == false )
            {
                noisePosition.x += idleX;
                noisePosition.y += idleY;
                m_Parent.localPosition = noisePosition;
            }

            if( useSway ) 
            {
                Vector3 localEulerAngles = PlayerCharacter.Instance.m_Transform.localEulerAngles + cameraTransform.localEulerAngles;
                Vector3 velocity = ( localEulerAngles - prevRotation ) / Time.fixedDeltaTime;
                Vector3 velocityChange = velocity + prevVelocity;
                prevRotation = localEulerAngles;
                prevVelocity = velocity;
                velocityChange *= -Time.fixedDeltaTime;

                float smoothTime = Time.smoothDeltaTime;

                if( smoothTime == 0f )
                    smoothTime = Time.deltaTime;

                float smoothing = swaySmoothing * Time.smoothDeltaTime;
                
                float eulerAnglesX = Mathf.SmoothDampAngle( m_Parent.localEulerAngles.x, velocityChange.x, ref vel01, smoothing, Mathf.Infinity, smoothTime );
                float eulerAnglesY = Mathf.SmoothDampAngle( m_Parent.localEulerAngles.y, velocityChange.y, ref vel02, smoothing, Mathf.Infinity, smoothTime );
                float eulerAnglesZ = Mathf.SmoothDampAngle( m_Parent.localEulerAngles.z, velocityChange.z, ref vel03, smoothing, Mathf.Infinity, smoothTime );

                if( FirstPersonController.isRunning )
                {
                    IronsightUnzoom();

                    float offsetX = eulerAnglesX - runOffset.x - CameraHeadBob.yPos * 10f;
                    float offsetY = eulerAnglesY - runOffset.y + CameraHeadBob.xPos * 15f;
                    float offsetZ = eulerAnglesZ + runOffset.z;
                    m_Parent.localRotation = Quaternion.Slerp( m_Parent.localRotation, Quaternion.Euler( offsetX, offsetY, offsetZ ), Time.smoothDeltaTime * 25f );
                }
                else
                {
                    m_Parent.localRotation = Quaternion.Slerp( m_Parent.localRotation, Quaternion.Euler( eulerAnglesX, eulerAnglesY, eulerAnglesZ ), Time.smoothDeltaTime * 25f );
                }

                if( Vector3.Angle( m_Parent.forward, cameraTransform.forward ) > borderSize && FirstPersonController.isRunning == false )
                {
                    float angle = Mathf.Sign( ASKInputManager.lookHorizontal ) * -borderSize;
                    m_Parent.rotation = Quaternion.Slerp( m_Parent.rotation, Quaternion.LookRotation( Quaternion.AngleAxis( angle, Vector3.up ) * cameraTransform.forward ),
                        Time.smoothDeltaTime * 20f );
                }
            }
        }
                
        // Play FireAnimation
        internal void PlayFireAnimation()
        {
            if( fireClip != null )
            {
                string animName = fireClip.name;
                m_Animation[ animName ].speed = fireAnimSpeed;
                m_Animation.Rewind( animName );
                m_Animation.Play( animName, PlayMode.StopAll );
            }
        }

        // Play ReloadAnimation
        internal void PlayReloadAnimation()
        {
            if( reloadClip != null )
            {
                string animName = reloadClip.name;
                m_Animation[ animName ].speed = reloadAnimSpeed;
                m_Animation.Rewind( animName );
                m_Animation.CrossFade( animName, .33f, PlayMode.StopAll ); //.33f
            }
        }

        // Dropin Animation
        internal void DropinAnimation()
        {
            isChanging = true;
            StartCoroutine( StartDropinAnimation() );
        }

        // Dropout Animation
        internal void DropoutAnimation()
        {
            isChanging = true;
            StartCoroutine( StartDropoutAnimation() );
        }

        // Start DropinAnimation
        private IEnumerator StartDropinAnimation()
        {
            m_Transform.localEulerAngles = dropoutRotation;

            while( DropAnimationPlay( ref dropinRotation ) )
                yield return null;            

            m_Transform.localEulerAngles = dropinRotation;
            isChanging = false;
        }

        // Start DropoutAnimation
        private IEnumerator StartDropoutAnimation()
        {
            m_Transform.localEulerAngles = dropinRotation;

            while( DropAnimationPlay( ref dropoutRotation ) )
                yield return null;            

            m_Transform.localEulerAngles = dropoutRotation;
            isChanging = false;
        }

        // DropAnimation Play
        private bool DropAnimationPlay( ref Vector3 targetRotation )
        {
            m_Transform.localEulerAngles = Vector3.SmoothDamp( m_Transform.localEulerAngles, targetRotation, ref velRotation, dropoutSmoothing * Time.smoothDeltaTime );
            
            const int digits = 2;
            double eaMag = System.Math.Round( ( double )m_Transform.localEulerAngles.magnitude, digits );
            double drMag = System.Math.Round( ( double )targetRotation.magnitude, digits );
            
            return ( eaMag != drMag );
        }



        // Ironsighting
        private void Ironsighting()
        {
            if( FirstPersonController.isRunning || ironsightZooming )
                return;

            bool zoomActionDown = ASKInputManager.zoomActionDown;
            bool zoomAction = ASKInputManager.zoomAction;

            switch( WeaponsManager.IronsightingMode )
            {
                case EIronsightingMode.Click:
                    if( zoomActionDown && ironsightZoomed )
                        StartUnzoom();
                    else if( zoomActionDown && ironsightZoomed == false )
                        StartZoom();
                    break;
                case EIronsightingMode.Press:
                    if( zoomAction && ironsightZoomed == false )
                        StartZoom();                    
                    else if( zoomAction == false && ironsightZoomed )
                        StartUnzoom();                    
                    break;
                case EIronsightingMode.Mixed:
                    if( zoomActionDown && ironsightZoomed == false )
                        StartZoom();
                    else if( ( zoomActionDown || ASKInputManager.zoomActionUp ) && ironsightZoomed )
                        StartUnzoom();
                    break;
            }
        }

        // Start Zoom
        private void StartZoom()
        {
            HudElements.Crosshair.SetActive( ECrosshairView.None );

            StopCoroutine( "IronsightUnzoomming" );
            StartCoroutine( "IronsightZoomming" );
        }
        // Start Unzoom
        private void StartUnzoom()
        {
            StopCoroutine( "IronsightZoomming" );
            StartCoroutine( "IronsightUnzoomming" );
        }

        // Ironsight Zoomming
        private IEnumerator IronsightZoomming()
        {
            ironsightZooming = true;

            while( ironsightZooming )
            {
                float smoothing = ironsightSmoothing * Time.smoothDeltaTime;

                noisePosition = zoomPos;
                noisePosition.x += idleX;
                noisePosition.y += idleY;
                PlayerCamera.mainCamera.fieldOfView = Mathf.SmoothDamp( PlayerCamera.mainCamera.fieldOfView, mainCamNativeFOV - zoomFOV, ref camFovVel, smoothing );
                PlayerCamera.secondCamera.fieldOfView = Mathf.SmoothDamp( PlayerCamera.secondCamera.fieldOfView, secondCamNativeFOV - zoomFOV, ref camFovVel, smoothing );
                m_Parent.localPosition = Vector3.SmoothDamp( m_Parent.localPosition, noisePosition, ref zoomVel, smoothing );

                double lpMag = System.Math.Round( ( double )m_Parent.localPosition.magnitude, ironsightingDigits );
                double npMag = System.Math.Round( ( double )noisePosition.magnitude, ironsightingDigits );

                if( lpMag == npMag )
                {
                    ironsightZooming = false;
                    ironsightZoomed = true;

                    m_Parent.localPosition = noisePosition;
                    PlayerCamera.mainCamera.fieldOfView = mainCamNativeFOV - zoomFOV;
                    PlayerCamera.secondCamera.fieldOfView = secondCamNativeFOV - zoomFOV;
                    borderSize = halfBorderSize;
                    m_Firearm.dispersion = ironsightDispersion;
                    moveSpeed = fpMoveSpeed * ironsightMoveSpeed;
                }

                yield return null;
            }
        }
        // Ironsight Unzoomming
        private IEnumerator IronsightUnzoomming()
        {
            ironsightZooming = true;

            while( ironsightZooming )
            {
                float smoothing = ironsightSmoothing * Time.smoothDeltaTime;

                noisePosition = nativePosition;
                noisePosition.x += idleX;
                noisePosition.y += idleY;
                PlayerCamera.mainCamera.fieldOfView = Mathf.SmoothDamp( PlayerCamera.mainCamera.fieldOfView, mainCamNativeFOV, ref camFovVel, smoothing );
                PlayerCamera.secondCamera.fieldOfView = Mathf.SmoothDamp( PlayerCamera.secondCamera.fieldOfView, secondCamNativeFOV, ref camFovVel, smoothing );
                m_Parent.localPosition = Vector3.SmoothDamp( m_Parent.localPosition, noisePosition, ref zoomVel, smoothing );

                double lpMag = System.Math.Round( ( double )m_Parent.localPosition.magnitude, ironsightingDigits );
                double npMag = System.Math.Round( ( double )noisePosition.magnitude, ironsightingDigits );

                if( lpMag == npMag )
                {
                    ironsightZooming = false;
                    ironsightZoomed = false;

                    m_Parent.localPosition = noisePosition;
                    PlayerCamera.mainCamera.fieldOfView = mainCamNativeFOV;
                    PlayerCamera.secondCamera.fieldOfView = secondCamNativeFOV;
                    borderSize = fullBorderSize;
                    m_Firearm.dispersion = nativeDispersion;
                    moveSpeed = fpMoveSpeed;
                    HudElements.Crosshair.SetActive( ( GameSettings.ShowCrosshair ) ? crosshairView : ECrosshairView.None );
                }

                yield return null;
            }
        }
                

        // Ironsight Unzoom
        internal static void IronsightUnzoom()
        {
            if( ironsightZoomed && ironsightZooming == false )
            {
                instance.StopCoroutine( "IronsightZoomming" );
                instance.StartCoroutine( "IronsightUnzoomming" );
            }
        }


        // Full Reset
        internal static void FullReset()
        {
            if( instance == null )
                return;

            instance.StopAllCoroutines();

            if( ironsightZoomed || ironsightZooming )
                instance.m_Parent.localPosition = instance.nativePosition;

            if( isChanging )
                instance.m_Transform.localEulerAngles = instance.dropoutRotation;

            if( isPlaying )
                instance.m_Animation.Stop();

            ironsightZoomed = ironsightZooming = isChanging = false;
        }


        // Set Pivot
        private void SetPivot()
        {
            m_Transform.localPosition = parentPosition - pivotPosition;
            m_Parent.localPosition = pivotPosition;
            nativePosition = m_Parent.localPosition;
            prevPivotPosition = pivotPosition;            
        }
    };
}