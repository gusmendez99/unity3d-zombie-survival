using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public Slider MusicVolume;
    private AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        musicSource.volume = MusicVolume.value;
    }
    public void OnButtonPress()
    {
        musicSource.PlayOneShot(musicSource.clip);
    }
}
