/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using AdvancedShooterKit.Events;


namespace AdvancedShooterKit
{
    public class Pickup : MonoBehaviour, IPickup
    {
        [SerializeField, Range( 1, 100 )]
        private int dropChance = 75;
        

        [SerializeField]
        private EPickupType pickupType = EPickupType.Health;
        public EPickupType PickupType
        {
            get { return pickupType; }
            set { pickupType = value; }
        }

        [SerializeField]
        [Range( .1f, 3f )]
        private float pickupDistance = 1.5f;
        [SerializeField]
        private AudioClip pickupSound = null;

        [SerializeField]
        private int amount = 10;
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        [SerializeField]
        private int ammoIndex = 0;
        public int AmmoIndex
        {
            get { return ammoIndex; }
            set { ammoIndex = value; }
        }

        [SerializeField]
        private int weaponIndex = 0;
        public int WeaponIndex
        {
            get { return weaponIndex; }
            set { weaponIndex = value; }
        }


        [SerializeField]
        private ASKEvent Pickuped = null;

        private Transform m_Transform = null;
        private int nativeAmount = 0;


        // Awake
        void Awake()
        {
            m_Transform = transform;
            nativeAmount = amount;
        }

        // OnRespawn
        void OnRespawn()
        {
            amount = nativeAmount;
        }


        void OnSpawn()
        {
            bool isLuck = ( Random.Range( 1, 101 ) <= dropChance );

            if( isLuck == false )
                Destroy( gameObject );
        }



        // Check Distance
        public bool CheckDistance { get { return ( Vector3.Distance( PlayerCharacter.Instance.m_Transform.position, m_Transform.position ) <= pickupDistance ); } }

        // Pickup Item
        public void PickupItem()
        {
            Pickuped.Invoke();

            switch( pickupType )
            {
                case EPickupType.Health:
                    if( PlayerCharacter.Instance.IncrementHealth( amount ) )
                        DestroyIt();
                    break;

                case EPickupType.Melee:
                case EPickupType.Firearms:
                    if( WeaponsManager.PickupWeapon( weaponIndex, ( pickupType == EPickupType.Firearms ), amount ) )
                        DestroyIt();
                    else
                        PickupAmmo();
                    break;

                case EPickupType.Ammo:
                    PickupAmmo();   
                    break;

                case EPickupType.Thrown:
                    if( WeaponsManager.PickupWeapon( weaponIndex, true, amount ) )
                    {
                        amount--;

                        if( amount > 0 )
                            PickupAmmo();
                        else
                            DestroyIt();
                    }
                    else
                    {
                        PickupAmmo();
                    }
                    break;
            }
        }

        // Ammo Pickup
        private void PickupAmmo()
        {
            if( AmmoBackpack.AddAmmo( ammoIndex, ref amount ) )
            {
                Utils.Audio.PlayClipAtPoint( pickupSound, m_Transform.position );

                if( pickupType == EPickupType.Ammo || pickupType == EPickupType.Thrown )
                    DestroyIt( false );
            }
        }


        // Destroy It
        private void DestroyIt( bool playSound = true )
        {
            if( playSound )
                Utils.Audio.PlayClipAtPoint( pickupSound, m_Transform.position );
            
            Respawner.StartRespawn( gameObject );
        }
    }
}