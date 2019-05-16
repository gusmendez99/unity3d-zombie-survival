/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public abstract class DamageHandler : MonoBehaviour, IDamageHandler
    {
        [SerializeField]
        private EArmorType armorType = EArmorType.None;
        public EArmorType ArmorType
        {
            get { return armorType; }
            set { armorType = value; }
        }

        [SerializeField]
        protected int surfaceIndex = 0;
        public int SurfaceIndex { get { return surfaceIndex; } }

        public virtual DamageInfo lastDamage { get; protected set; }

        public abstract bool isAlive { get; }

        public virtual bool isPlayer { get { return false; } }
        public virtual bool isNPC { get { return false; } }


        // Take Damage
        public virtual void TakeDamage( DamageInfo damageInfo )
        {
            if( GameSettings.DamageIndication == EDamageIndication.ForAll )
            {
                if( damageInfo.owner.isPlayer )
                    HudElements.ShowDamegeIndicator();
            }
            else if( GameSettings.DamageIndication == EDamageIndication.OnlyCharacters )
            {
                if( damageInfo.owner.isPlayer && isNPC )
                    HudElements.ShowDamegeIndicator();
            }

            lastDamage = damageInfo;
        }

        // Calc Damage
        protected int CalcDamage( float damage )
        {
            return Mathf.RoundToInt( damage * getHardness * damageModifierByDifficulty );
        }

        // DamageModifier ByDifficulty
        protected virtual float damageModifierByDifficulty { get { return 1f; } }


        // Get Hardness
        private float getHardness
        {
            get
            {
                switch( armorType )
                {
                    case EArmorType.None: return    1f;
                    case EArmorType.Lite: return   .8f;
                    case EArmorType.Medium: return .65f;
                    case EArmorType.Heavy: return  .5f;
                    case EArmorType.Ultra: return  .35f;

                    default: Debug.LogError( "Invalid ArmorType in " + this.name ); return 1f;
                }
            }
        }
        //
    }
}