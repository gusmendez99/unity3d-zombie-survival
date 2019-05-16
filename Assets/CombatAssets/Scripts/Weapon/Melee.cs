/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public class Melee : WeaponBase
    {
        [SerializeField]
        [Range( 45f, 650f )]
        private float damage = 45f;
        [SerializeField]
        [Range( 25f, 150f )]
        private float hitForce = 50f;

        [SerializeField]
        [Range( .1f, 5f )]
        private float swayRange = 1.3f;
        [SerializeField]
        [Range( .01f, 1f )]
        private float swayRadius = .25f;
        
        [SerializeField]
        private HitEffect hitEffect = null;


        // Awake
        protected override void Start()
        {
            base.Start();

            if( owner.isPlayer )
                firingMode = EFiringMode.Single;
        }

        // Shooting
        public override void StartShooting()
        {
            if( owner.isPlayer )
            {
                if( !FirstPersonWeaponSway.isPlaying )
                    base.StartShooting();
            }
            else if( owner.isNPC )
            {
                base.StartShooting();
            }
        }

        // FinalStageShooting
        protected override void FinalStageShooting()
        {
            RaycastHit hitInfo;

            if( Physics.SphereCast( projectileOuter.position, swayRadius, projectileOuter.forward, out hitInfo, swayRange, hitMask ) )
            {
                Collider hitCollider = hitInfo.collider;
                if( hitCollider.isTrigger )
                    return;

                IDamageHandler handler = hitCollider.GetComponent<IDamageHandler>();
                if( handler != null )
                    handler.TakeDamage( new DamageInfo( damage, projectileOuter.root, owner, EDamageType.Melee ) );

                HitEffect.SpawnHitEffect( hitEffect, hitInfo );

                Rigidbody tmpRb = hitCollider.attachedRigidbody;
                if( tmpRb != null && !tmpRb.isKinematic )
                    tmpRb.AddForce( projectileOuter.forward * ( hitForce * 1.75f / ( tmpRb.mass > 1f ? tmpRb.mass : 1f ) ), ForceMode.Impulse );                
            }            

            base.FinalStageShooting();
        }
    }
}