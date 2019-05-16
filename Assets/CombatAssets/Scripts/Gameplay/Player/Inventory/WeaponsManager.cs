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
    public class WeaponsManager : MonoBehaviour
    {
        [System.Serializable]
        public sealed class WeaponSlot
        {
            public bool available = false;
            public EWeaponType type = EWeaponType.Standart;
            public WeaponBase weapon, subWeapon;
            public Rigidbody dropout = null;
        }

        [SerializeField]
        [Range( 1, 10 )]
        private int maxWeapons = 2;

        [SerializeField]
        private AudioClip 
            switchWeapon = null, 
            switchShootingMode = null, 
            subWeaponOnOff = null, 
            switchProjectile = null;

        [SerializeField]
        private EIronsightingMode ironsightingMode = EIronsightingMode.Mixed;
        internal static EIronsightingMode IronsightingMode { get{ return m_Instance.ironsightingMode; } }

        [ SerializeField]
        private WeaponSlot[] weapons = null;

        // instance
        private static WeaponsManager instance = null;
        // m_Instance
        private static WeaponsManager m_Instance
        {
            get
            {
                if( instance == null )
                    instance = FindObjectOfType<WeaponsManager>();

                return instance;
            }
        }

        //        
        private static GameObject weaponGo = null;
        private static WeaponBase m_Weapon = null;
        private static Firearms m_Firearm = null;

        private static int weaponIndex = -1, weaponsCount = 0;        
        private static bool isSubweapon = false;
        private static bool isMelee { get { return ( m_Firearm == null ); } }
        private static AudioSource m_Audio = null;


        // Awake
        void Awake()
        {
            weaponIndex = -1;
            weaponsCount = 0;
            isSubweapon = false;

            instance = this;

            maxWeapons = Mathf.Clamp( 1, maxWeapons, weapons.Length );

            m_Audio = this.GetComponent<AudioSource>();
            m_Audio.outputAudioMixerGroup = GameSettings.SFXOutput;

            m_Weapon = weapons[ 0 ].weapon;
            weaponGo = m_Weapon.gameObject;
            m_Firearm = m_Weapon as Firearms;
        }

        // Start
        void Start()
        {
            for( int i = 0; i < weapons.Length; i++ )
            {
                WeaponSlot currentSlot = weapons[ i ];
                GameObject currentGO = currentSlot.weapon.gameObject;

                currentGO.SetActive( true );

                if( currentSlot.type == EWeaponType.Thrown )
                {
                    Firearms tmpFA = currentSlot.weapon as Firearms;

                    if( !tmpFA.isEmpty || !AmmoBackpack.IsEmpty( tmpFA.currentSlot.ammoIndex ) )
                    {
                        currentSlot.available = true;
                    }
                    else if( AmmoBackpack.IsEmpty( tmpFA.currentSlot.ammoIndex ) && tmpFA.isEmpty )
                    {
                        currentSlot.available = false;
                    }
                }

                if( weaponsCount >= maxWeapons && currentSlot.type == EWeaponType.Standart )
                {
                    DropWeapon( i );
                }
                else if( currentSlot.available && currentSlot.type == EWeaponType.Standart )
                {
                    weaponsCount++;
                }

                currentGO.SetActive( false );
            }

            for( int i = weaponIndex; i < weapons.Length; i++ )
                if( i != weaponIndex && weapons[ i ].available )
                {
                    weaponIndex = i;
                    StartCoroutine( ChangeWeapon() );
                    break;
                }

            UpdateHud();
            UpdateInformerAndCrosshair();
        }

        // OnDisable
        void OnDisable()
        {
            instance.StopCoroutine( "ChangeWeapon" );
            FirstPersonWeaponSway.FullReset();
        }


        // bespoken
        private static bool bespoken
        {
            get
            {
                return ( ( m_Weapon == null )
                    || ( !isMelee && m_Firearm.isReloading )
                    || !PlayerCharacter.Instance.isAlive
                    || FirstPersonWeaponSway.ironsightZooming
                    || FirstPersonWeaponSway.isChanging );
            }
        }

        // weapons array length
        public static int size { get { return m_Instance.weapons.Length; } }

        // Weapon Fire
        internal static void WeaponFire()
        {
            if( bespoken || weaponIndex < 0 )
                return;

            m_Weapon.StartShooting();
        }

        // Weapon Reset
        internal static void WeaponReset()
        {
            if( bespoken || weaponIndex < 0 )
                return;

            m_Weapon.WeaponReset();
        }


        // Drop Current Weapon
        internal static void DropCurrentWeapon()
        {
            if( bespoken )
                return;

            int newInd = -1;

            for( int i = 0; i < size; i++ )
                if( i != weaponIndex && m_Instance.weapons[ i ].available )
                {
                    newInd = i;
                    break;
                }

            DropWeapon( weaponIndex, newInd );
        }

        // Reload Weapon
        internal static void ReloadWeapon()
        {
            if( bespoken || weaponIndex < 0 || isMelee )
                return;

            m_Firearm.StartReloading();
        }

        // Switch To SubWeapon
        internal static void SwitchToSubWeapon()
        {
            if( bespoken || weaponIndex < 0 || isMelee || m_Instance.weapons[ weaponIndex ].subWeapon == null )
                return;

            if( isSubweapon )
            {
                m_Weapon = m_Instance.weapons[ weaponIndex ].weapon;
                isSubweapon = false;
            }
            else
            {
                m_Weapon = m_Instance.weapons[ weaponIndex ].subWeapon;
                isSubweapon = true;
            }

            m_Firearm = m_Weapon as Firearms;
            m_Audio.PlayOneShot( m_Instance.subWeaponOnOff );
            UpdateHud();
        }

        // Switch to Next Firemode
        internal static void SwitchFiremode()
        {
            if( !bespoken && weaponIndex >= 0 && !isMelee && m_Weapon.SwitchShootingMode() )
                m_Audio.PlayOneShot( m_Instance.switchShootingMode );
        }

        // Switch to Next AmmoType
        internal static void SwitchAmmotype()
        {
            if( !bespoken && weaponIndex >= 0 && !isMelee && m_Firearm.SwitchProjectileType() )
                m_Audio.PlayOneShot( m_Instance.switchProjectile );
        }


        // Select Weapon ByIndex
        internal static void SelectWeaponByIndex( int index )
        {
            if( bespoken
                || ( weaponIndex == index )
                || !m_Weapon.weaponReady
                || ( index < 0 )
                || ( index > size - 1 ) )
                return;

            if( m_Instance.weapons[ index ].available )
            {
                weaponIndex = index;
                m_Instance.StartCoroutine( ChangeWeapon() );
            }
        }

        // Select Previous Weapon
        internal static void SelectPreviousWeapon()
        {
            if( bespoken || !m_Weapon.weaponReady )
                return;

            // Find Previous Weapon
            for( int i = weaponIndex; i > -1; i-- )
                if( i != weaponIndex && m_Instance.weapons[ i ].available )
                {
                    weaponIndex = i;
                    m_Instance.StartCoroutine( ChangeWeapon() );
                    return;
                }

            // Find Last Weapon
            for( int i = size - 1; i > -1; i-- )            
                if( i != weaponIndex && m_Instance.weapons[ i ].available )
                {
                    weaponIndex = i;
                    m_Instance.StartCoroutine( ChangeWeapon() );
                    break;
                }            
        }

        // Select Next Weapon
        internal static void SelectNextWeapon()
        {
            if( bespoken || !m_Weapon.weaponReady )
                return;

            // Find Next Weapon
            for( int i = weaponIndex; i < size; i++ )
                if( i != weaponIndex && m_Instance.weapons[ i ].available )
                {
                    weaponIndex = i;
                    m_Instance.StartCoroutine( ChangeWeapon() );
                    return;
                }

            // Find First Weapon
            for( int i = 0; i < size; i++ )
                if( i != weaponIndex && m_Instance.weapons[ i ].available )
                {
                    weaponIndex = i;
                    m_Instance.StartCoroutine( ChangeWeapon() );
                    break;
                }
        }        

        // ChangeEmpty to FirstAvaliable
        internal static void ChangeEmptyToFirstAvaliable()
        {
            if( !FirstPersonWeaponSway.isChanging && AmmoBackpack.IsEmpty( m_Firearm.currentSlot.ammoIndex ) )
            {
                if( m_Instance.weapons[ weaponIndex ].type == EWeaponType.Thrown )
                {
                    m_Instance.weapons[ weaponIndex ].available = false;
                    FindFirstAvailableWeapon();
                    CalculateWeaponsCount();
                    m_Instance.StartCoroutine( ChangeWeapon() );
                }
                else
                {
                    if( FindFirstAvailableWeapon() )
                        m_Instance.StartCoroutine( ChangeWeapon() );
                }
            }
        }

        // Change Weapon
        private static IEnumerator ChangeWeapon()
        {
            while( FirstPersonWeaponSway.isPlaying )
                yield return null;

            if( FirstPersonWeaponSway.ironsightZoomed )
            {
                FirstPersonWeaponSway.IronsightUnzoom();

                while( FirstPersonWeaponSway.ironsightZooming )
                    yield return null;
            }            

            if( weaponGo.activeSelf )
            {
                if( weaponsCount > 0 )
                    m_Audio.PlayOneShot( m_Instance.switchWeapon );

                weaponGo.GetComponentInChildren<FirstPersonWeaponSway>().DropoutAnimation();
            }

            while( FirstPersonWeaponSway.isChanging )
                yield return null;

            weaponGo.SetActive( false );

            if( weaponIndex >= 0 )
            {
                isSubweapon = false;

                m_Weapon = m_Instance.weapons[ weaponIndex ].weapon;
                weaponGo = m_Weapon.gameObject;

                m_Firearm = m_Weapon as Firearms;

                weaponGo.SetActive( true );
                weaponGo.GetComponentInChildren<FirstPersonWeaponSway>().DropinAnimation();
            }

            UpdateHud( true );
        }       
        

        // Pickup Weapon
        internal static bool PickupWeapon( int index, bool isFArms = false, int ammoAmount = 0 )
        {
            WeaponSlot currentSlot = m_Instance.weapons[ index ];

            if( currentSlot.available )
                return false;

            currentSlot.available = true;

            if( isFArms )
            {
                Firearms tmpFA = currentSlot.weapon as Firearms;
                tmpFA.currentSlot.currentAmmo = Mathf.Min( tmpFA.maxAmmo, ammoAmount );
            }

            if( weaponsCount < m_Instance.maxWeapons )
            {
                if( weaponIndex < 0 )
                {
                    weaponIndex = index;
                    m_Instance.StartCoroutine( ChangeWeapon() );
                }

                CalculateWeaponsCount();
            }
            else
            {
                if( currentSlot.type == EWeaponType.Standart )
                    DropWeapon( weaponIndex, index );
            }

            return true;
        }

        // Drop Weapon
        private static void DropWeapon( int index, int newIndex = -1 )
        {
            if( index < 0 )
            {
                for( int i = 0; i < size; i++ )
                    if( newIndex != i && m_Instance.weapons[ i ].available && m_Instance.weapons[ i ].type == EWeaponType.Standart )
                    {
                        DropWeapon( i, newIndex );
                        return;
                    }

                for( int i = 0; i < size; i++ )
                    if( m_Instance.weapons[ i ].available && m_Instance.weapons[ i ].type == EWeaponType.Standart )
                    {
                        DropWeapon( i );
                        return;
                    }
                return;
            }


            WeaponSlot currentSlot = m_Instance.weapons[ index ];

            if( weaponsCount < 1 || !currentSlot.available )            
                return;

            if( currentSlot.type != EWeaponType.Standart )
            {
                for( int i = 0; i < size; i++ )
                    if( newIndex != i && m_Instance.weapons[ i ].available && m_Instance.weapons[ i ].type == EWeaponType.Standart )
                    {
                        DropWeapon( i );
                        return;
                    }

                for( int i = 0; i < size; i++ )
                    if( m_Instance.weapons[ i ].available && m_Instance.weapons[ i ].type == EWeaponType.Standart )
                    {
                        DropWeapon( i );
                        return;
                    }
            }

            if( currentSlot.dropout != null )
            {
                Transform playerTR = PlayerCharacter.Instance.m_Transform;
                Rigidbody droppedWeapon = currentSlot.dropout.SpawnCopy( playerTR.forward * .2f + PlayerCamera.m_Transform.position + Vector3.up * .1f, Random.rotation );
                droppedWeapon.AddForce( playerTR.forward * ( 4f * droppedWeapon.mass ), ForceMode.Impulse );
                droppedWeapon.AddRelativeTorque( Vector3.up * ( Random.value > .5f ? 15f : -15f ), ForceMode.Impulse );

                Firearms tmpFA = currentSlot.weapon as Firearms;
                if( tmpFA != null )
                {
                    IPickup tmpPickup = droppedWeapon.GetComponent<IPickup>();
                    if( tmpPickup != null )
                    {
                        tmpPickup.Amount = tmpFA.currentSlot.currentAmmo;
                    }
                    else
                    {
                        tmpPickup = droppedWeapon.gameObject.AddComponent<Pickup>();
                        tmpPickup.Amount = tmpFA.currentSlot.currentAmmo;
                        tmpPickup.WeaponIndex = index;
                        tmpPickup.AmmoIndex = tmpFA.currentSlot.ammoIndex;
                        tmpPickup.PickupType = EPickupType.Firearms;
                    }
                }
            }
            else
            {
                Debug.LogError( "ERROR: Dropout item is null! Item index in weapons: " + index );
            }


            currentSlot.weapon.gameObject.SetActive( false );
            currentSlot.available = false;

            if( newIndex > -1 )
            {
                weaponIndex = newIndex;
                m_Instance.StartCoroutine( ChangeWeapon() );
            }            

            CalculateWeaponsCount();

            if( weaponsCount < m_Instance.maxWeapons )
                m_Audio.PlayOneShot( m_Instance.switchWeapon );

            UpdateHud( true );
        }

        // Find FirstAvailable Weapon
        private static bool FindFirstAvailableWeapon()
        {
            for( int i = 0; i < size; i++ )
                if( i != weaponIndex && m_Instance.weapons[ i ].available )
                {
                    weaponIndex = i;
                    return true;
                }

            return false;
        }


        // Calculate Weapons Count
        private static void CalculateWeaponsCount()
        {
            weaponsCount = 0;

            for( int i = 0; i < size; i++ )
                if( m_Instance.weapons[ i ].available && m_Instance.weapons[ i ].type == EWeaponType.Standart )
                    weaponsCount++;

            if( weaponsCount < 1 && !m_Instance.weapons[ weaponIndex ].available )
                weaponIndex = -1;

            UpdateHud();
        }


        // Weapon isAvailable
        internal static bool WeaponIsAvailable( int index )
        {
            return m_Instance.weapons[ index ].available;
        }

        // Crowded
        internal static bool crowded
        {
            get { return weaponsCount >= m_Instance.maxWeapons; }
        }

        // WeaponType isStandart
        internal static bool WeaponTypeIsStandart( int index )
        {
            return m_Instance.weapons[ index ].type == EWeaponType.Standart;
        }


        // Update Hud
        public static void UpdateHud( bool updateAll = false )
        {
            if( !PlayerCharacter.Instance.isAlive )
                return;

            if( !isMelee )
            {
                HudElements.Crosshair.UpdatePosition( m_Firearm.dispersion );
                //
                HudElements.WeaponInformer.UpdateAmmoIcon( AmmoBackpack.GetHudIcon( m_Firearm.currentSlot.ammoIndex ) );
                HudElements.WeaponInformer.UpdateCurrentAmmoInfo( m_Firearm.currentSlot.currentAmmo );
                HudElements.WeaponInformer.UpdateAllAmmoInfo( AmmoBackpack.GetCurrentAmmo( m_Firearm.currentSlot.ammoIndex ) );
                HudElements.WeaponInformer.UpdateShootingModeIcon( m_Weapon.firingMode );
            }

            if( updateAll )
                UpdateInformerAndCrosshair();
        }

        // Update InformerAndCrosshair
        private static void UpdateInformerAndCrosshair()
        {
            ECrosshairView tmpView = ECrosshairView.None;

            if( weaponIndex >= 0 )
            {
                if( GameSettings.ShowCrosshair && !FirstPersonWeaponSway.ironsightZoomed && !FirstPersonWeaponSway.ironsightZooming )
                    tmpView = weaponGo.GetComponentInChildren<FirstPersonWeaponSway>().crosshairView;
            }
            else
            {
                FirstPersonWeaponSway.moveSpeed = 1f;
            }

            HudElements.Crosshair.SetActive( tmpView );
            HudElements.WeaponInformer.SetActive( weaponIndex >= 0 && !isMelee );
        }
    }
}