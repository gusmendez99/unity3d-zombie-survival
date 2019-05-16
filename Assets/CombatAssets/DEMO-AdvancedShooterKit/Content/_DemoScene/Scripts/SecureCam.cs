using UnityEngine;
using AdvancedShooterKit;


public class SecureCam : MonoBehaviour
{
    private Transform body, player;
    private IWeapon m_Weapon;
    private Animation anim;
    private Animation animZombie;

    private bool alive = true;

    
    // Start
    void Start()
    {        
        body = GetComponentInChildren<Collider>().transform;
        player = PlayerCharacter.Instance.m_Transform;        

        m_Weapon = GetComponentInChildren<IWeapon>();

        anim = GetComponentInChildren<Animation>();
        animZombie = GetComponentInParent<Animation>();
        anim.enabled = true;
        anim.playAutomatically = true;


    }


    // Update
    void Update()
    {
        if( alive == false )
            return;

        if( PlayerCharacter.Instance.isAlive && Vector3.Distance( body.position, player.position ) < 10f )
        {
            if( anim.enabled )            
                anim.enabled = false;            

            body.rotation = Quaternion.Lerp( body.rotation, Quaternion.LookRotation( player.position - body.position ), 2f * Time.deltaTime );

            if( Vector3.Angle( player.position - body.position, body.forward ) < 0.5f )
                m_Weapon.StartShooting();
        }
        else
        {
            if( anim.enabled == false )
            {
                anim.enabled = true;
                body.localEulerAngles = Vector3.right * 32f;
            }
        }
    }


    // OnRespawn
    void OnRespawn()
    {
        alive = true;
    }

    // ExplodeCam
    public void ExplodeCam()
    {
        if( alive )
        {
            alive = false;
            anim.enabled = false;

            ParticleSystem camParticle = GetComponentInChildren<ParticleSystem>();
            camParticle.playOnAwake = false;
            camParticle.Stop();
            camParticle.Play();

            Destroy(transform.root.gameObject);

        }
    }
}
