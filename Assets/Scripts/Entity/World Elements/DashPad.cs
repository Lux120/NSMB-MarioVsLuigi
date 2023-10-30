using UnityEngine;
using System.Collections;

public class DashPad : MonoBehaviour
{
    public float speedBoostAmount = 5f; // Adjust the boost amount as needed
    public Vector2 boostDirection = Vector2.up; // Default boost direction is up
    public float teleportHeight = 1f; // Adjust the teleport height as needed
    public AudioClip activationSound; // Assign the audio clip in the Inspector

    public LayerMask entityLayer; // Layer containing entities to be affected

    public float joystickResetDelay = 0.5f; // Adjust the delay as needed

    private AudioSource audioSource;
    private bool activated = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && (other.CompareTag("Player") || ((1 << other.gameObject.layer) & entityLayer) != 0))
        {
            StartCoroutine(ActivateDashPad(other.gameObject));

            // Play the activation sound
            if (audioSource != null && activationSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }

            activated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (activated && (other.CompareTag("Player") || ((1 << other.gameObject.layer) & entityLayer) != 0))
        {
            activated = false;
        }
    }

private IEnumerator ActivateDashPad(GameObject entity)
{
    // Teleport the entity a little bit in the air
    Rigidbody2D entityRigidbody = entity.GetComponent<Rigidbody2D>();
    if (entityRigidbody != null)
    {
        Vector2 boostPosition = (Vector2)transform.position + (boostDirection.normalized * teleportHeight);
        entityRigidbody.position = boostPosition;
    }

    Vector2 boostVector = boostDirection.normalized * speedBoostAmount;
    entityRigidbody.velocity += boostVector;

    // Reset the Joystick variable for a brief amount of time if the entity is a player
    PlayerController playerController = entity.GetComponent<PlayerController>();
    if (playerController != null)
    {
        Vector2 originalJoystickValue = playerController.GetJoystickValue();

        playerController.SetJoystickValue(new Vector2(0f, originalJoystickValue.y));

        yield return new WaitForSeconds(joystickResetDelay);

        playerController.SetJoystickValue(originalJoystickValue);
    }

    // Apply the speed boost to the entity

}}