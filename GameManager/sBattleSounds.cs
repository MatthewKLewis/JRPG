using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sBattleSounds : MonoBehaviour
{
    [SerializeField] private AudioClip clipDing;
    [SerializeField] private AudioClip clipBell;
    [SerializeField] private AudioClip clipWoodBlock1;
    [SerializeField] private AudioClip clipWoodBlock2;
    [SerializeField] private AudioClip clipReady;
    [SerializeField] private AudioClip clipHmm;
    [SerializeField] private AudioClip clipHiyaa;

    private AudioSource aS;

    void Start()
    {
        aS = GetComponent<AudioSource>();
    }

    //public void PlayWoodblock()
    //{
    //    aS.PlayOneShot(clipWoodBlock1);
    //}

    //public void PlayWoodblock2()
    //{
    //    aS.PlayOneShot(clipWoodBlock2);
    //}

    public void PlayBell()
    {
        aS.PlayOneShot(clipBell);
    }

    //public void PlayHmm()
    //{
    //    aS.PlayOneShot(clipHmm);
    //}

    //public void PlayHiyaa()
    //{
    //    aS.PlayOneShot(clipHiyaa);
    //}

    //public void PlayReady()
    //{
    //    aS.PlayOneShot(clipReady);
    //}

    public void PlayNoise() { }
}
