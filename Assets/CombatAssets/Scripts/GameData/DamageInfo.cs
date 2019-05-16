/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public sealed class DamageInfo
    {
        public float damage { get; private set; }
        public Transform source { get; private set; }
        public ICharacter owner { get; private set; }
        public EDamageType type { get; private set; }


        // Damage Info
        public DamageInfo()
        {
            SetInfo( 0f, null, null, EDamageType.Unknown );
        }
        // Damage Info
        public DamageInfo( float damage, Transform source, ICharacter owner, EDamageType type = EDamageType.Unknown )
        {
            SetInfo( damage, source, owner, type );
        }

        // GetIt ByInfo
        public DamageInfo GetItByInfo( float damage, Transform source, ICharacter owner, EDamageType type = EDamageType.Unknown )
        {
            SetInfo( damage, source, owner, type );
            return this;
        }

        // Set Info
        public void SetInfo( float damage, Transform source, ICharacter owner, EDamageType type = EDamageType.Unknown )
        {
            this.damage = damage;
            this.source = source;
            this.owner = owner;
            this.type = type;
        }
    };
}