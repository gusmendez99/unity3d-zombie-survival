/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


#define UNITY_5_4_PLUS
#if UNITY_5_3 || UNITY_5_4
#undef UNITY_5_4_PLUS
#endif

using UnityEngine;
using System.Collections;


namespace AdvancedShooterKit
{
    public class Firearms : WeaponBase
    {
        [System.Serializable]
        public sealed class ProjectileSlot
        {
            public int ammoIndex = 0;
            public int currentAmmo = 15;
            public Projectile projectileObject = null;
        }

        public bool infiniteAmmo = false;

        [Range( 1, 100 )]
        public int maxAmmo = 32;
        [Range( -10f, 35f )]
        public float addDamage = 0f;
        [Range( -5f, 15f )]
        public float addSpeed = 0f;
        [Range( 5f, 100f )]
        public float dispersion = 50f;

        [SerializeField]
        private int projectilesPerShot = 1;

        [SerializeField]
        private AudioClip emptySFX = null, reloadSFX = null;

        public float shelloutForce = 25f;
        public Transform shellOuter = null;


        public ProjectileSlot[] projectiles = new ProjectileSlot[ 1 ];
        public ProjectileSlot currentSlot { get; private set; }

        public bool isEmpty { get { return infiniteAmmo ? false : currentSlot.currentAmmo <= 0; } }
        public bool isReloading { get; private set; }


        //
        private int projectileIndex = 0;
        private Vector3 nativeOuterAngles = Vector3.zero;
        private ParticleSystem shotParticle = null;
        private Light shotLight = null;



        // Awake
        protected override void Awake()
        {
            base.Awake();

            projectileIndex = 0;
            currentSlot = projectiles[ projectileIndex ];

            shotParticle = GetComponentInChildren<ParticleSystem>();

            if( shotParticle != null )
            {
#if UNITY_5_4_PLUS
                var main = shotParticle.main;
                main.playOnAwake = false;
                main.loop = false;
#else
		        shotParticle.playOnAwake = shotParticle.loop = false;
#endif
            }

            shotLight = GetComponentInChildren<Light>();
            if( shotLight != null )
                shotLight.enabled = false;

            if( projectileOuter != null )
                nativeOuterAngles = projectileOuter.localEulerAngles;
        }

        // OnEnable
        protected override void OnEnable()
        {
            base.OnEnable();
            isReloading = false;
        }


        // FirstStage Shooting
        protected override void FirstStageShooting()
        {
            if( owner.isPlayer )
            {
                if( isEmpty )
                {
                    if( AmmoBackpack.IsEmpty( currentSlot.ammoIndex ) )
                    {
                        m_Audio.pitch = Time.timeScale;
                        m_Audio.PlayOneShot( emptySFX );
                        StartCoroutine( FireRate( .75f ) );
                    }
                    else
                    {
                        StartReloading();
                    }
                }
                else if( !isEmpty && !isReloading )
                {
                    base.FirstStageShooting();
                }
            }
            else if( owner.isNPC )
            {
                SecondStageShooting();
            }
        }
        // SecondStage Shooting
        protected override void SecondStageShooting()
        {
            if( owner.isPlayer )
            {
                if( !infiniteAmmo )
                    currentSlot.currentAmmo--;

                HudElements.WeaponInformer.UpdateCurrentAmmoInfo( currentSlot.currentAmmo );
            }

            if( shotParticle != null && shotLight != null )
                StartCoroutine( PlayVisualEffects() );

            base.SecondStageShooting();
        }        
        // FinalStage Shooting
        protected override void FinalStageShooting()
        {
            if( currentSlot.projectileObject != null )
            {
                if( projectilesPerShot > 1 )
                {
                    for( int i = 0; i < projectilesPerShot; i++ )
                    {
                        MuzzleToCameraPoint();
                        Projectile.SpawnFromWeapon( this );
                        projectileOuter.localEulerAngles = nativeOuterAngles;
                    }
                }
                else
                {
                    Projectile.SpawnFromWeapon( this );
                    projectileOuter.localEulerAngles = nativeOuterAngles;
                }

                Shell.SpawnFromWeapon( this );                
            }
            else
            {
                Debug.LogError( "ERROR: Projectile is not setup! Error in: " + this.name );                
            }            

            base.FinalStageShooting();

            if( owner.isPlayer && isEmpty && AmmoBackpack.IsEmpty( currentSlot.ammoIndex ) == false )
                StartReloading();
        }

        // Muzzle ToCameraPoint
        protected override void MuzzleToCameraPoint()
        {
            base.MuzzleToCameraPoint();

            float calcX = Random.Range( -dispersion, dispersion ) / 25f;
            float calcY = Random.Range( -dispersion * 1.2f, dispersion * 1.2f ) / 25f;

            projectileOuter.Rotate( calcY, calcX, 0f );
        }

        // Play VisualEffects
        private IEnumerator PlayVisualEffects()
        {
            shotParticle.Stop();
            shotParticle.Play();

            shotLight.enabled = true;

            float elapsed = 0f;

#if UNITY_5_4_PLUS
            float lifetime = shotParticle.main.startLifetime.constant;
#else
		    float lifetime = shotParticle.startLifetime;
#endif

            while( elapsed < lifetime )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            shotLight.enabled = false;
        }

        // Reloading
        internal virtual void StartReloading()
        {
            if( !isReloading && currentSlot.currentAmmo < maxAmmo && !AmmoBackpack.IsEmpty( currentSlot.ammoIndex ) )            
                StartCoroutine( PlayReload() );            
        }

        // Play Reload
        private IEnumerator PlayReload()
        {
            isReloading = true;
            singleShotReady = true;

            FirstPersonWeaponSway.IronsightUnzoom();

            while( FirstPersonWeaponSway.ironsightZooming || FirstPersonWeaponSway.isPlaying )
                yield return null;
            
            float reloadTime = .15f;
            if( fpWeaponSway != null && fpWeaponSway.reloadClip != null )            
                reloadTime = fpWeaponSway.reloadClip.length;
            if( reloadSFX != null )
                reloadTime = reloadSFX.length;
            if( reloadSFX != null && fpWeaponSway != null && fpWeaponSway.reloadClip != null )
                reloadTime = Mathf.Min( reloadSFX.length, fpWeaponSway.reloadClip.length );

            if( fpWeaponSway != null )
                fpWeaponSway.PlayReloadAnimation();

            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot( reloadSFX );            

            float elapsed = 0f;
            while( elapsed < reloadTime )
            {
                elapsed += Time.deltaTime;
                yield return null;
            }            

            int missingAmmo = Mathf.Max( 0, maxAmmo - currentSlot.currentAmmo );
            int invAmmo = AmmoBackpack.GetCurrentAmmo( currentSlot.ammoIndex );
            currentSlot.currentAmmo += Mathf.Min( Mathf.Max( 0, invAmmo ), missingAmmo );
            invAmmo = Mathf.Max( 0, invAmmo -= missingAmmo );

            AmmoBackpack.SetCurrentAmmo( currentSlot.ammoIndex, invAmmo );

            HudElements.WeaponInformer.UpdateCurrentAmmoInfo( currentSlot.currentAmmo );
            HudElements.WeaponInformer.UpdateAllAmmoInfo( invAmmo );

            isReloading = false; 
        }


        // Switch ProjectileType
        internal bool SwitchProjectileType()
        {
            if( projectiles.Length < 2 )
                return false;

            projectileIndex++;

            if( projectileIndex >= projectiles.Length )
                projectileIndex = 0;

            currentSlot = projectiles[ projectileIndex ];
            WeaponsManager.UpdateHud();
            return true;
        }                

        // Check Ammo
        protected override bool checkAmmo { get { return !isEmpty; } }
    }
}