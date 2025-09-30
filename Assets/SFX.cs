using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFX : MonoBehaviour{
    public AudioSource Close;
    public AudioSource Btn;
     
    public void PlayBtn(){
        Btn.Play(); 
    }

    public void PlayClose(){
        Close.Play();
    }
}
