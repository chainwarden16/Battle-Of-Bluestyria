using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaGlobal : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    public void ReproducirMusica()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
    }

    public void PararMusica()
    {
        audioSource.Stop();
    }
}
