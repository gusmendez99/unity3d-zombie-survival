/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using m_IEnumerator = System.Collections.IEnumerator;

namespace AdvancedShooterKit
{
    [RequireComponent( typeof( Camera ) )]
    public sealed class PlayerCamera : MonoBehaviour
    {
        private static RaycastHit hit;        
        private static Transform m_transform = null;
        private static PlayerCamera instance = null;
        

        //
        public LayerMask hitMask = 1;
        public Camera m_secondCamera = null;
        //        

        public static Transform m_Transform
        {
            get
            {
                if( m_transform == null )
                {
                    PlayerCamera PC = FindObjectOfType<PlayerCamera>();
                    m_transform = ( PC != null ) ? PC.transform : null;
                }                    

                return m_transform;
            }
        }
        public static AudioSource m_Audio { get; private set; }

        public static Camera mainCamera { get; private set; }
        public static Camera secondCamera { get { return instance.m_secondCamera; } }

        public static bool isShaking { get; private set; }
        public static bool isHitted { get; private set; }

        public static RaycastHit hitInfo { get { return hit; } }


        // need for calculation         
        private static bool dirtHair, inputUse;
        private static Collider hitCollider, prevHitCollider;
        private static IPickup tmpPickup = null;
        private static ECrosshairMode crosshairMode = ECrosshairMode.None;


        // Awake
        void Awake()
        {
            instance = this;
            m_transform = transform;
            mainCamera = this.GetComponent<Camera>();
            m_Audio = this.GetComponentInChildren<AudioSource>();
        }
        
        // Fixed Update
        void FixedUpdate()
        {
            UpdateHit();
        }

        // Update Hit
        public static void UpdateHit()
        {
            isHitted = Physics.Raycast( m_transform.position, m_transform.forward, out hit, Mathf.Infinity, instance.hitMask );
            if( !isHitted )
            {
                ResetPickup();
                return;
            }                

            hitCollider = hit.collider;
            //
            if( TagsManager.IsPickup( hitCollider.tag ) )
            {
                dirtHair = true;
                PickupActivity();                
            }
            else
            {
                SetCrosshairColor();
                ResetPickup();
            }
            //
            prevHitCollider = hitCollider;
            // Debug.DrawLine( m_transform.position, hit.point );
        }


        // Set CrosshairColor
        private static void SetCrosshairColor()
        {
            if( GameSettings.DamageIndication == EDamageIndication.OFF || hitCollider == prevHitCollider )
                return;

            IDamageHandler handler = hitCollider.GetComponent<IDamageHandler>();

            if( GameSettings.DamageIndication == EDamageIndication.ForAll )
            {
                if( handler != null && !handler.isPlayer && handler.isAlive )
                    HudElements.Crosshair.SetColor( ECrosshairColor.Damager );
                else
                    HudElements.Crosshair.SetColor( ECrosshairColor.Normal );
            }
            else if( GameSettings.DamageIndication == EDamageIndication.OnlyCharacters )
            {
                if( handler != null && handler.isNPC && handler.isAlive )
                    HudElements.Crosshair.SetColor( ECrosshairColor.Damager );
                else
                    HudElements.Crosshair.SetColor( ECrosshairColor.Normal );
            }
        }


        // Use Item
        internal static void UseItem()
        {
            if( inputUse 
                || crosshairMode == ECrosshairMode.Cancel
                || !PlayerCharacter.Instance.isAlive 
                || FirstPersonWeaponSway.ironsightZooming 
                || FirstPersonWeaponSway.isChanging 
                || FirstPersonWeaponSway.isPlaying )
                return;

            if( FirstPersonWeaponSway.ironsightZoomed )
            {
                FirstPersonWeaponSway.IronsightUnzoom();
                return;
            }                

            inputUse = true;
        }

        // Pickup Activity
        private static void PickupActivity()
        {
            if( inputUse )
            {
                if( checkPickup )
                    tmpPickup.PickupItem();

                inputUse = false;
                ResetPickup();
            }
            else
            {
                if( hitCollider != prevHitCollider || tmpPickup == null )
                {
                    tmpPickup = hitCollider.GetComponent<IPickup>();
                    HudElements.Crosshair.SetColor( ECrosshairColor.Normal );
                }                                

                if( !checkPickup )
                    return;

                switch( tmpPickup.PickupType )
                {
                    case EPickupType.Melee:
                    case EPickupType.Firearms:
                    case EPickupType.Thrown:
                        if( WeaponsManager.WeaponIsAvailable( tmpPickup.WeaponIndex ) )
                        {
                            if( ( tmpPickup.PickupType != EPickupType.Melee ) && ( tmpPickup.Amount > 0 ) && !AmmoBackpack.IsFull( tmpPickup.AmmoIndex ) )
                                crosshairMode = ECrosshairMode.Ammo;
                            else
                                crosshairMode = ECrosshairMode.Cancel;
                        }
                        else if( WeaponsManager.WeaponTypeIsStandart( tmpPickup.WeaponIndex ) && WeaponsManager.crowded )
                            crosshairMode = ECrosshairMode.Swap;
                        else
                            crosshairMode = ECrosshairMode.Hand;
                        break;

                    case EPickupType.Ammo:
                        if( AmmoBackpack.IsFull( tmpPickup.AmmoIndex ) )
                            crosshairMode = ECrosshairMode.Cancel;
                        else
                            crosshairMode = ECrosshairMode.Ammo;
                        break;

                    case EPickupType.Health:
                        if( PlayerCharacter.Instance.isFull )
                            crosshairMode = ECrosshairMode.Cancel;
                        else
                            crosshairMode = ECrosshairMode.Health;
                        break;

                    default:
                        crosshairMode = ECrosshairMode.Hand;
                        break;
                }

                //crosshairMode
                HudElements.Crosshair.SetPointSprite( crosshairMode );
            }
        }

        // Check Pickup
        private static bool checkPickup { get { return ( tmpPickup != null && tmpPickup.CheckDistance ); } }


        // Reset Pickup
        private static void ResetPickup()
        {
            if( dirtHair )
            {
                dirtHair = false;
                tmpPickup = null;
                prevHitCollider = null;
                crosshairMode = ECrosshairMode.Point;
                HudElements.Crosshair.SetPointSprite( crosshairMode );
            }
        }
        

        // Shake
        public static void Shake( float duration, float intensity )
        {
            if( isShaking ) 
                return;

            isShaking = true;
            instance.StopCoroutine( "StartShake" );
            instance.StartCoroutine( StartShake( duration, intensity * 250f ) );
        }
        // Shake OneShot
        public static void ShakeOneShot( float intensity )
        {
            instance.StopCoroutine( "StartShake" );
            instance.StartCoroutine( StartShake( .1f, intensity * 50f ) );
        }

        // Private Shake
        private static m_IEnumerator StartShake( float duration, float intensity )
        {
            Vector3 originalPos =  m_transform.localPosition;
            Quaternion originalRot = m_transform.localRotation;
            originalRot.eulerAngles = Vector3.up * m_transform.localEulerAngles.y;
            
            float elapsed = 0f;
            while( elapsed < duration )
            {
                elapsed += Time.deltaTime;

                float percentComplete = elapsed / duration;
                float damper = 1f - Mathf.Clamp01( 4f * percentComplete - 3f );
                float shakeRange = damper * Random.Range( -intensity, intensity );

                Vector3 shakePos = originalPos + Random.insideUnitSphere * shakeRange * .035f;
                Quaternion shakeRot = originalRot * Quaternion.Euler( -shakeRange * .15f, 0f, shakeRange * 1.75f );
                m_transform.localPosition = Vector3.Lerp( m_transform.localPosition, shakePos, Time.smoothDeltaTime * 2f );
                m_transform.localRotation = Quaternion.Slerp( m_transform.localRotation, shakeRot, Time.smoothDeltaTime * 2f );

                yield return null;
            }

            isShaking = false;

            if( !PlayerCharacter.Instance.isAlive )
                PlayerDie();
        }
        


        // Player Die
        internal static void PlayerDie()
        {
            m_transform.localEulerAngles = Vector3.forward * 64f;
            secondCamera.enabled = false;
        }
    }
}