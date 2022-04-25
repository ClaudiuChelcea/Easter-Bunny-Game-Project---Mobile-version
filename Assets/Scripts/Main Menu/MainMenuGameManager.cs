using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGameManager : MonoBehaviour
{
        // Get objects
        public AudioSource lvl_music;
        public AudioClip music_clip;

        // Start is called before the first frame update
        void Start()
        {
                music_clip = GetComponentInChildren<AudioClip>();
                lvl_music.PlayOneShot(music_clip);
        }

        // Update is called once per frame
        void Update()
        {

        }
}
