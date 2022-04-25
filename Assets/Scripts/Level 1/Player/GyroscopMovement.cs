using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopMovement : MonoBehaviour
{
        private Rigidbody2D rb;
	public float speed_multiplier = 0f;
	public PlayerMovement playerMovement;

	// Use this for initiatization
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		playerMovement = GetComponent<PlayerMovement>();
	}

	/*
	private void Update()
	{
		dirx = Input.acceleration.x * movespeed;
		this.transform.position = new Vector2(Mathf.Clamp(transform.position.x, -7.5f, -7.5f), transform.position.y);
	}

	private void FixedUpdate()
	{
		rb.velocity = new Vector2(dirx, 0f);
	}*/


	private void Update()
	{
		rb.velocity = new Vector2(Input.acceleration.x * speed_multiplier, 0f);
	}
}