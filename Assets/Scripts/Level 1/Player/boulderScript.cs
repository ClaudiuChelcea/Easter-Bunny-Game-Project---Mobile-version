using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boulderScript : MonoBehaviour
{
        private float start;
        public float startAfter = 0f;
        public float boulderSpeed = 0f;
        public float acceleration = 0f;
        private float increase_speed = 0f;

        private Rigidbody2D rigidbody;
        // Start is called before the first frame update
        void Start()
        {
                rigidbody = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
                increase_speed += Time.deltaTime;
                start += Time.deltaTime;
                if(start < 1f)
		{
		}
                if (start > startAfter)
                {
                        this.transform.RotateAroundLocal(-Vector3.forward, 1f * Time.deltaTime);
                        this.rigidbody.velocity = new Vector2(boulderSpeed + increase_speed, rigidbody.velocity.y);
                }
        }
}
