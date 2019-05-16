/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;


namespace AdvancedShooterKit
{
    public sealed class PlayerCharacter : Character
    {
        private static ICharacter instance = null;
        public static ICharacter Instance
        {
            get
            {
                if( instance == null )
                    instance = FindObjectOfType<PlayerCharacter>();

                return instance;
            }
        }


        public override bool isPlayer { get { return true; } }
        public override bool isNPC { get { return false; } }


        // Awake
        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        // Start
        protected override void Start()
        {
            HudElements.HealthBar.UpdateBar( currentHealth, maxHealth );            
            base.Start();
        }

        // OnEnable
        protected override void OnEnable()
        {
            base.OnEnable();
            HudElements.SetActive( GameSettings.ShowHud );
        }

        // OnDisable
        protected override void OnDisable()
        {
            base.OnDisable();
            HudElements.SetActive( false );
        }


        // Take Damage
        public override void TakeDamage( DamageInfo damageInfo )
        {
            HudElements.HealthBar.SetDamageTargetPosition( damageInfo.source.position );

            int finalDamage = CalcDamage( damageInfo.damage );
            float shakeRange = finalDamage / 10f;

            if( finalDamage < damageToPain && HealthInPercent > percentToPain )
            {
                switch( damageInfo.type )
                {
                    case EDamageType.Impact:
                    case EDamageType.Melee:
                        PlayerCamera.ShakeOneShot( Random.Range( shakeRange, shakeRange * 1.65f ) );
                        HudElements.ShowPainScreen();
                        break;

                    default:
                        break;
                }
            }
            else
            {
                PlayerCamera.ShakeOneShot( Random.Range( shakeRange, shakeRange * 1.65f ) );
                HudElements.ShowPainScreen();
            }

            DecrementHealth( finalDamage );
        }

        // DamageModifier ByDifficulty
        protected override float damageModifierByDifficulty
        {
            get
            {
                switch( GameSettings.DifficultyLevel )
                {
                    case EDifficultyLevel.Easy:    return .7f;
                    case EDifficultyLevel.Normal:  return 1f;
                    case EDifficultyLevel.Hard:    return 1.2f;
                    case EDifficultyLevel.Delta:   return 1.35f;
                    case EDifficultyLevel.Extreme: return 1.5f;
                    default: return 1f;
                }
            }
        }


        // Increment Health
        public override bool IncrementHealth( int addАmount )
        {
            bool result = base.IncrementHealth( addАmount );

            if( result )
                HudElements.HealthBar.UpdateBar( currentHealth, maxHealth );

            return result;
        }

        // Decrement Health
        public override bool DecrementHealth( int damage )
        {
            bool result = base.DecrementHealth( damage );

            if( result )
                HudElements.HealthBar.UpdateBar( currentHealth, maxHealth );

            return result;
        }        


        // OnDie
        protected override void OnDie()
        {
            base.OnDie();
            //
            this.enabled = false;
            PlayerCamera.PlayerDie();
            HudElements.PlayerDie();
            FirstPersonController.PlayerDie();

            FirstPersonWeaponSway[] tmpFPWeapSways = this.GetComponentsInChildren<FirstPersonWeaponSway>();
            foreach( FirstPersonWeaponSway fpWSway in tmpFPWeapSways )
                fpWSway.enabled = false;

            GameObject bAnimGO = this.GetComponentInChildren<BodyAnimation>().gameObject;
            if( bAnimGO != null )
                bAnimGO.SetActive( false );

            // Show menu after die.
            ASKInputManager.PlayerDie();
        }
    }
}