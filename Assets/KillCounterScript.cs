using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounterScript : MonoBehaviour
{
    public GameObject player;
    private GameObject fPSCharacter;
    private GameObject weaponHolder;
    private GameObject glockGO;
    private GameObject mp5kGO;
    private GameObject m870GO;
    private GameObject akmGO;

    private WeaponBase wbGLOCK;
    private WeaponBase wbMP5K;
    private WeaponBase wbM870;
    private WeaponBase wbAKM;

    public int totalKills;
    public int killsToMake;
    public GameObject sceneChanger;



    // Start is called before the first frame update
    void Start()
    {
        fPSCharacter = player.transform.Find("FirstPersonCharacter").gameObject;
        weaponHolder = fPSCharacter.transform.Find("WeaponHolder").gameObject;

        glockGO = weaponHolder.transform.Find("Glock").gameObject;
        mp5kGO = weaponHolder.transform.Find("MP5K").gameObject;
        m870GO = weaponHolder.transform.Find("M870").gameObject;
        akmGO = weaponHolder.transform.Find("AKM").gameObject;

        wbGLOCK = glockGO.transform.GetComponent<WeaponBase>();
        wbMP5K = mp5kGO.transform.GetComponent<WeaponBase>();
        wbM870 = m870GO.transform.GetComponent<WeaponBase>();
        wbAKM = akmGO.transform.GetComponent<WeaponBase>();
        totalKills = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            //Get total kills and verify when player has killed all the enemies in the level
            totalKills = wbGLOCK.GetEnemiesKilled() + wbMP5K.GetEnemiesKilled() + wbM870.GetEnemiesKilled() + wbAKM.GetEnemiesKilled();
        }
           
        Debug.Log("TOTAL KILLS IS: " + totalKills.ToString());
        Debug.Log("KILL TO MAKE IS: " + killsToMake.ToString());
        if (killsToMake <= totalKills)
        {
            if (sceneChanger)
            {
                sceneChanger.SetActive(true);
                
            }
        }
    }
}
