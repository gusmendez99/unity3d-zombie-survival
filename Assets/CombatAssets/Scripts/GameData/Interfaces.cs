/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    // DamageHandler
    public interface IDamageHandler
    {
        EArmorType ArmorType { get; set; }

        bool isAlive { get; }

        bool isPlayer { get; }
        bool isNPC { get; }

        int SurfaceIndex { get; }

        DamageInfo lastDamage { get; }

        void TakeDamage( DamageInfo damageInfo );
    };

    // IHealth
    public interface IHealth : IDamageHandler
    {
        bool Immortal { get; set; }

        int MaxHealth { get; set; }
        int CurrentHealth { get; }

        bool Regeneration { get; set; }
        int RegAmount { get; set; }
        float RegDelay { get; set; }
        float RegInterval { get; set; }

        int HealthInPercent { get; }

        bool isFull { get; }

        bool IncrementHealth( int addАmount );
        bool DecrementHealth( int damage );
    };

    // ICharacter
    public interface ICharacter : IHealth
    {
        Transform m_Transform { get; }
        AudioSource m_Audio { get; }
        bool isActive { get; }

        void SetActive( bool value );
        void Kill();
        void PlayPainEffect();
    };

    // IFootstepSFXManager
    public interface IFootstepSFXManager
    {
        void PlayJumpingSound( RaycastHit hit );
        void PlayLandingSound( RaycastHit hit );
        void PlayFootStepSound( RaycastHit hit );
    };


    // IPickup
    public interface IPickup
    {
        EPickupType PickupType { get; set; }

        int Amount { get; set; }
        int AmmoIndex { get; set; }
        int WeaponIndex { get; set; }

        bool CheckDistance { get; }

        void PickupItem();
    };


    // IWeapon
    public interface IWeapon
    {
        void StartShooting();
    };

    // IExplosion
    public interface IExplosion
    {
        void SetHitMask( LayerMask mask );

        void StartExplode();
        void StartExplode( ICharacter owner );
        void StartExplode( ICharacter owner, float delay );
        void StartExplode( float delay );
    };

    // ILadder
    public interface ILadder
    {
        Transform m_Transform { get; }

        void AssignAudioSource( AudioSource m_Audio );

        void PlayLadderFootstepSound();
    };
}
