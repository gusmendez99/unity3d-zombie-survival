/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public sealed class DamagePoint : DamageHandler
    {
        [SerializeField]
        [Range( 0f, 10f )]
        private float damageModifier = 1f;


        public override DamageInfo lastDamage { get { return checkHealth ? m_Health.lastDamage : null; } }

        public override bool isAlive { get { return checkHealth ? m_Health.isAlive : false; } }

        public override bool isPlayer { get { return checkHealth ? m_Health.isPlayer : false; } }
        public override bool isNPC { get { return checkHealth ? m_Health.isNPC : false; } }


        private IHealth m_Health = null;

        private bool checkHealth
        {
            get
            {
                bool result = ( m_Health != null );

                if( !result )
                    Debug.LogError( "Health componenet is not setup. Error in: " + this.name );

                return result;
            }
        }


        // Start
        void Start()
        {
            m_Health = this.GetComponentInParent<IHealth>();
        }


        // Take Damage
        public override void TakeDamage( DamageInfo damageInfo )
        {
            if( checkHealth )
                m_Health.TakeDamage( damageInfo.GetItByInfo( damageInfo.damage * damageModifier, damageInfo.source, damageInfo.owner, damageInfo.type ) );
        }
    }
}