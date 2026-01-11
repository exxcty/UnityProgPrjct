using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera cam;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 mousePos;

    private GameControls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (cam == null) cam = Camera.main;

        controls = new GameControls();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        movementInput = controls.Player.Move.ReadValue<Vector2>();

        Vector2 screenMousePos = controls.Player.Look.ReadValue<Vector2>();
        mousePos = cam.ScreenToWorldPoint(screenMousePos);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput.normalized * moveSpeed;
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }
    public void ApplySpeedBoost(float amount, float duration)
    {
        StartCoroutine(SpeedBoostRoutine(amount, duration));
    }

    private IEnumerator SpeedBoostRoutine(float amount, float duration)
    {
        moveSpeed += amount;
        Debug.Log("Speed Boost Activated!");

        yield return new WaitForSeconds(duration);

        moveSpeed -= amount;
        Debug.Log("Speed Boost Ended.");
    }

    public bool IsMoving => movementInput.sqrMagnitude > 0.01f;

    public void IncreaseSpeed(float amount) => moveSpeed += amount;

    public GameControls InputControls => controls;
}