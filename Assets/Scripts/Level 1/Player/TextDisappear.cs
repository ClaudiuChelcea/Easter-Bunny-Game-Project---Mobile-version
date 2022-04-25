using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisappear : MonoBehaviour
{
        private float startTime = 0f;
        private float endTime = 2f;
        public GameManager GM;
        
        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
                startTime += Time.deltaTime;
                if (startTime > endTime)
                        Destroy(this.gameObject);

                if (GM.player.isDead == true)
                        Destroy(this.gameObject);
    }
}
