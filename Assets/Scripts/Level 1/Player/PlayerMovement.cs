using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
	/********* Variables *********/

	// Direction
	private enum direction
	{
		LEFT = -1,
		NEUTRAL = 0,
		RIGHT = 1
	};

	// Speed
	public bool boolRunning = false;
	public Rigidbody2D body;
	public float horizontalMovementInput = 0f;
	[SerializeField] public float speed_multiplier = 0f;
	private float default_speed = 0f;
	Vector3 old_pos, new_pos;
	public GameManager gameManager;

	// Jump
	[SerializeField] private bool boolGrounded = true;
	[SerializeField] private float playerJumpForce = 0f;
	public GameObject ground;
	const float DISTANCE_TO_GROUND = 1.3f;
	private Gyroscope gyro;
	private Quaternion rotation;
	private bool gyroActive;
	float old_rotation = 0f, new_rotation = 0f;
	int old_time = 0, new_time = 0;

	// Energy
	public static float playerEnergy = 1f;
	[SerializeField] public Slider energySlider;
	[SerializeField] private float energyConsumptionPerSprint = 0f;
	[SerializeField] private float energyConsumptionPerJump = 0f;
	[SerializeField] public float startRegeneratingEnergyTime = 0f;
	public float timeUntilStartRegeneratingEnergy = 0f;
	public TextMeshProUGUI notEnoughEnergyError;
	public Vector3 position_to_player;
	[SerializeField] private float CarrotEnergy;

	// Animations
	private SpriteRenderer playerSprite;
	private Animator playerAnimator;

	// Score
	public int playerScore = 0;

	// Win
	public bool is_at_the_exit = false;

	// Lose
	public bool isDead = false;

	// Sounds
	private AudioSource chew_sound;

	// Get components
	private void Awake()
	{
		EnableGyro();
		body = gameObject.GetComponent<Rigidbody2D>();
		playerSprite = gameObject.GetComponent<SpriteRenderer>();
		playerAnimator = gameObject.GetComponent<Animator>();
		playerEnergy = 1f;
		energySlider.value = playerEnergy;
		default_speed = speed_multiplier;
		notEnoughEnergyError.alpha = 0f;
		boolGrounded = true;
		chew_sound = GetComponent<AudioSource>();
	}

	// Face the walking direction
	private void maintainDirection()
	{
		if (horizontalMovementInput < -0.3f)
		{ // face right
			playerSprite.flipX = false;
			boolRunning = true;
			playerEnergy -= energyConsumptionPerSprint * Time.deltaTime;
		}
		else if (horizontalMovementInput > 0.3f)
		{ // face left
			playerSprite.flipX = true;
			boolRunning = true;
			playerEnergy -= energyConsumptionPerSprint * Time.deltaTime;
		}
		else
		{ // face ahead
			boolRunning = false;
		}
	}

	public void EnableGyro()
	{
		// Already activated
		if (gyroActive)
			return;
		if (SystemInfo.supportsGyroscope)
		{
			gyro = Input.gyro;
			gyro.enabled = true;
			gyroActive = gyro.enabled;
		}
		else
		{
			Debug.Log("System doesn't support Gyroscope");
		}
	}

	// Move based on input
	private void moveToInput()
	{
		// Remove energy per jump
		if (boolGrounded == false && playerEnergy > 0f)
		{
			if (boolRunning == true)
			{
				timeUntilStartRegeneratingEnergy = 0f;
				playerEnergy -= energyConsumptionPerSprint * Time.deltaTime;
				if (playerEnergy < 0f)
					playerEnergy = 0f;

				Jump();
				Debug.Log("THIS CASE 1/1;");
			}
			else if (boolRunning == false)
			{
				Jump();
				Debug.Log("THIS CASE 2/1;");
			}
		}
		else if (boolGrounded == true && boolRunning == true && playerEnergy > 0f)
		{
			Debug.Log("THIS CASE 2;");
			timeUntilStartRegeneratingEnergy = 0f;
			playerEnergy -= energyConsumptionPerSprint * Time.deltaTime;
			if (playerEnergy < 0f)
				playerEnergy = 0f;
		}

		if (playerEnergy < 0f)
			playerEnergy = 0f;

		if (boolRunning == false || (boolRunning == true && playerEnergy == 0f))
		{
			timeUntilStartRegeneratingEnergy += Time.deltaTime;
			if (timeUntilStartRegeneratingEnergy > startRegeneratingEnergyTime)
			{
				playerEnergy += energyConsumptionPerSprint * Time.deltaTime;
				if (playerEnergy > 1f)
					playerEnergy = 1f;
			}
		}


		// Get input
		horizontalMovementInput = Input.acceleration.x;
		if ((horizontalMovementInput > 0.3f || horizontalMovementInput <= -0.3f) && playerEnergy > 0f)
		{
			body.velocity = new Vector2(horizontalMovementInput * speed_multiplier, body.velocity.y);
			boolRunning = true;
			timeUntilStartRegeneratingEnergy = 0f;
		}
		else
		{
			boolRunning = false;
		}


		// Display no energy
		if (playerEnergy <= 0f && boolRunning == true)
		{
			StartCoroutine(not_enough_energy("Run"));
		}

		// Change energy slider
		energySlider.value = playerEnergy;
	}

	private void Jump()
	{
		if (playerEnergy <= energyConsumptionPerJump)
		{
			StartCoroutine(not_enough_energy("Jump"));
			boolGrounded = true;
		}
		else
		{
			body.velocity = new Vector2(body.velocity.x, playerJumpForce);
			boolGrounded = false;
			playerEnergy -= energyConsumptionPerJump;
		}
	}

	// No enough energy error
	IEnumerator not_enough_energy(string type_of_action)
	{
		notEnoughEnergyError.alpha = 1;
		notEnoughEnergyError.text = "Not enough energy to " + type_of_action.ToLower() + "!";

		yield return new WaitForSeconds(2);
		notEnoughEnergyError.alpha = 0;
	}

	// Update color to reflect tiredness
	private void update_moving_color()
	{
		playerSprite.color = new Color(255f - energySlider.value, energySlider.value * 255f, energySlider.value * 255f);
	}

	// Movement & facing direction
	private void movement()
	{
		maintainDirection();
		moveToInput();
		update_moving_color();
	}

	// Execute animations
	private void animations()
	{
		playerAnimator.SetBool("Run", boolRunning);
		playerAnimator.SetBool("Grounded", boolGrounded);
	}

	// Check ground collision && exit collision
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Ground")
		{
			boolGrounded = true;
		}
		else if (collision.gameObject.tag == "ExitLevel")
		{
			is_at_the_exit = true;
		}
	}

	// Check if not touching exit
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "ExitLevel")
		{
			is_at_the_exit = false;
		}
	}

	// Triggers
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Carrot")
		{
			Destroy(collision.gameObject);
			++playerScore;
			playerEnergy = (playerEnergy + CarrotEnergy);
			chew_sound.Play();
			if (playerEnergy > 1f)
				playerEnergy = 1f;
		}
		else if (collision.gameObject.tag == "ExitLevel")
		{
			is_at_the_exit = true;
		}
		else if (collision.gameObject.tag == "Boulder")
		{
			Destroy(this.gameObject);
			isDead = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "ExitLevel")
		{
			is_at_the_exit = false;
		}
	}

	// Move body on input
	private void Update()
	{
		movement();
		animations();
	}
}
