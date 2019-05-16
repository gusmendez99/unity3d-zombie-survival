/********************************************
 * Copyright(c): 2016 Victor Klepikov       *
 *                                          *
 * Site: 	     http://u3d.as/5Fb		    *
 * Support: 	 http://bit.ly/vk-Support   *
 ********************************************/


using UnityEngine;
using AdvancedShooterKit.Utils;
using AdvancedShooterKit.Events;


namespace AdvancedShooterKit
{
    [RequireComponent( typeof( AudioSource ) )]
    public class Character : Health, ICharacter
    {
        [SerializeField]
        [Range( 1, 50 )]
        protected int damageToPain = 15;
        [SerializeField]
        [Range( 1, 99 )]
        protected int percentToPain = 10;

        [SerializeField]
        private AudioClip deathSound = null;
        [SerializeField]
        private AudioClip[] painSounds = null;

        [SerializeField]
        private int deathLayer = 2;

        [SerializeField]
        protected ASKEvent
            OnPain = null;
        

        public override bool isNPC { get { return true; } }
        public Transform m_Transform { get; protected set; }
        public AudioSource m_Audio { get; protected set; }
        public bool isActive { get { return gameObject.activeSelf; } }

        
        private int nativeLayer = 11;
        


        // Awake
        protected virtual void Awake()
        {
            m_Transform = transform;
            nativeLayer = gameObject.layer;
            m_Audio = GetComponent<AudioSource>();
        }

        // Start
        protected override void Start()
        {
            base.Start();
            m_Audio.SetupForSFX();
        }

        // OnEnable
        protected virtual void OnEnable()
        {
            SetLayerForDamagePoints( nativeLayer );
        }

        // OnDisable
        protected virtual void OnDisable()
        {
            
        }


        // SetActive
        public void SetActive( bool value )
        {
            gameObject.SetActive( value );
        }


        // Spawn
        public ICharacter SpawnCopy( Vector3 position, Quaternion rotation )
        {
            return SpawnCopy( position, rotation );
        }

        // OnRespawn
        protected override void OnRespawn()
        {
            base.OnRespawn();
        }


        // Kill
        public virtual void Kill()
        {
            if( isAlive )
            {
                currentHealth = 0;
                OnDie();
            }
        }

        
        // Take Damage
        public override void TakeDamage( DamageInfo damageInfo )
        {
            base.TakeDamage( damageInfo );
        }

        // DamageModifier ByDifficulty
        protected override float damageModifierByDifficulty
        {
            get
            {
                switch( GameSettings.DifficultyLevel )
                {
                    case EDifficultyLevel.Easy:    return 1.3f;
                    case EDifficultyLevel.Normal:  return 1f;
                    case EDifficultyLevel.Hard:    return .8f;
                    case EDifficultyLevel.Delta:   return .75f;
                    case EDifficultyLevel.Extreme: return .5f;
                    default: return 1f;
                }
            }
        }

        
        // Increment Health
        public override bool IncrementHealth( int addАmount )
        {
            bool result = base.IncrementHealth( addАmount );
            //...
            return result;
        }

        // Decrement Health
        public override bool DecrementHealth( int damage )
        {
            bool result = base.DecrementHealth( damage );
            
            // show pain if
            if( result && ( damage >= damageToPain || HealthInPercent <= percentToPain ) )
                PlayPainEffect();

            return result;
        }

        // Play PainEffect
        public virtual void PlayPainEffect()
        {
            OnPain.Invoke();

            if( painSounds.Length == 0 )
                return;

            m_Audio.outputAudioMixerGroup = GameSettings.VoiceOutput;
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot( Audio.GetRandomClip( painSounds ) );
        }

        // OnDie
        protected override void OnDie()
        {
            SetLayerForDamagePoints( deathLayer );

            m_Audio.Stop();
            m_Audio.outputAudioMixerGroup = GameSettings.VoiceOutput;
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot( deathSound );

            base.OnDie();
        }                

        // SetLayer
        private void SetLayerForDamagePoints( int layer )
        {
            DamagePoint[] points = GetComponentsInChildren<DamagePoint>();

            foreach( DamagePoint point in points )
                point.gameObject.layer = layer;
        }
        //
    }
}