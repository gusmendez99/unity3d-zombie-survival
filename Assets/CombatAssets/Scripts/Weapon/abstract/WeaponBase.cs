/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using System.Collections;
using AdvancedShooterKit.Utils;


namespace AdvancedShooterKit
{
    public abstract class WeaponBase : MonoBehaviour, IWeapon
    {
        public LayerMask hitMask = 1;

        [SerializeField]
        protected EFiringMode[] firingModes = null;

        [SerializeField]
        protected float rateOfFire = 250f;

        [SerializeField]
        protected AudioClip shotSFX = null;

        [SerializeField]
        protected float shotDelay = 0f;

        [SerializeField]
        protected float minCameraShake = .05f, maxCameraShake = .15f;

        public bool weaponReady { get; private set; }

        public EFiringMode firingMode { get; protected set; }
        private int firingModeIndex = 0;

        public Transform projectileOuter = null;

        public ICharacter owner { get; private set; }

        protected bool singleShotReady = true;
        protected byte shotsNumber = 0;

        protected AudioSource m_Audio = null;
        protected FirstPersonWeaponSway fpWeaponSway = null;


        // Awake
        protected virtual void Awake()
        {
            if( projectileOuter == null )
                Debug.LogError( "Projectile Outer is not setup! Erron in: " + this.name );            

            weaponReady = true;
        }


        // Start
        protected virtual void Start()
        {
            owner = GetComponentInParent<ICharacter>();
            if( owner == null )
            {
                Debug.LogError( "Weapon Owner is not found! Error in: " + this.name );
                return;
            }

            if( owner.isPlayer )
            {
                m_Audio = GetComponentInParent<AudioSource>();
                fpWeaponSway = GetComponentInChildren<FirstPersonWeaponSway>();

                if( firingModes.Length >= 1 )
                {
                    firingModeIndex = 0;
                    firingMode = firingModes[ firingModeIndex ];
                }
            }
            else if( owner.isNPC )
            {
                firingMode = EFiringMode.Automatic;

                m_Audio = GetComponentInChildren<AudioSource>();
                if( m_Audio == null )
                {
                    m_Audio = owner.m_Audio;
                    if( m_Audio != null )
                        return;

                    Debug.LogWarning( "AudioSource is not found! Warning in " + this.name );
                    m_Audio = gameObject.AddComponent<AudioSource>();
                }
            }

            m_Audio.SetupForSFX();
        }


        // OnEnable
        protected virtual void OnEnable()
        {
            weaponReady = true;
        }
                
                
        // Switch ShootingMode
        internal bool SwitchShootingMode()
        {
            if( firingModes.Length < 2 )
                return false;

            firingModeIndex++;

            if( firingModeIndex >= firingModes.Length )
                firingModeIndex = 0;

            firingMode = firingModes[ firingModeIndex ];
            WeaponsManager.UpdateHud();
            return true;
        }


        // Shooting
        public virtual void StartShooting()
        {
            if( weaponReady == false )
                return;

            if( projectileOuter != null )
                FirstStageShooting();
            else
                Debug.LogError( "Projectile Outer is not setup! Erron in: " + this.name );
        }

        // FirstStage Shooting
        protected virtual void FirstStageShooting()
        {
            if( firingMode == EFiringMode.Automatic )
            {
                SecondStageShooting();
            }
            else
            {
                if( singleShotReady )
                {
                    singleShotReady = false;
                    shotsNumber = ( byte )firingMode;
                    shotsNumber--;
                    SecondStageShooting();
                }
            }
        }
        // SecondStage Shooting
        protected virtual void SecondStageShooting()
        {
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot( shotSFX );

            if( owner.isPlayer && fpWeaponSway != null )
                fpWeaponSway.PlayFireAnimation();

            MuzzleToCameraPoint();

            if( shotDelay > 0f && firingMode == EFiringMode.Single )
                StartCoroutine( ShotWithDelay() );
            else
                FinalStageShooting();

            StartCoroutine( FireRate( 60f / rateOfFire ) );           
        }
        // FinalStage Shooting
        protected virtual void FinalStageShooting()
        {
            if( owner.isPlayer )            
                PlayerCamera.ShakeOneShot( Random.Range( minCameraShake, maxCameraShake ) );
        }

        // Rate Of Fire
        protected IEnumerator FireRate( float rateTime )
        {
            weaponReady = false;

            //yield return new WaitForSeconds( rateTime );

            for( float el = 0f; el < rateTime; el += Time.deltaTime )
                yield return null;

            weaponReady = true;

            if( owner.isPlayer )
            {
                if( checkAmmo && shotsNumber > 0 )
                {
                    SecondStageShooting();
                    shotsNumber--;
                }

                if( checkAmmo == false )
                    WeaponsManager.ChangeEmptyToFirstAvaliable();
            }
        }

        // Check Ammo
        protected virtual bool checkAmmo { get { return true; } } 

        // Muzzle ToCameraPoint
        protected virtual void MuzzleToCameraPoint()
        {
            if( owner.isPlayer )
            {
                PlayerCamera.UpdateHit();

                Transform camTransform = PlayerCamera.m_Transform;

                if( PlayerCamera.isHitted )
                {
                    float toMuzzle = Vector3.Distance( projectileOuter.position, camTransform.position );
                    float toPoint = Vector3.Distance( PlayerCamera.hitInfo.point, camTransform.position );

                    if( toMuzzle < toPoint )
                    {
                        projectileOuter.LookAt( PlayerCamera.hitInfo.point );
                        return;
                    }
                }

                projectileOuter.LookAt( camTransform.position + camTransform.forward * 25f );
            }
        }

        // Shot WithDelay
        private IEnumerator ShotWithDelay()
        {
            float elapsed = 0f;
            while( elapsed < shotDelay )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            FinalStageShooting();
        }
        
        
        // Weapon Reset
        internal void WeaponReset()
        {
            if( !singleShotReady && shotsNumber == 0 ) 
                singleShotReady = true;
        }
    }
}