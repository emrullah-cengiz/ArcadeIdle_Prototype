using JoyStick;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private JoyStickData _joyStickData;
    [SerializeField] private NavMeshAgent _agent;

    private void Start()
    {
        // _agent.updateRotation = false;
    }

    private void Update()
    {
        Move(_joyStickData.Data);
    }

    private void Look(Vector2 data)
    {
    }

    private void Move(Vector2 input)
    {
        Vector3 movementVector = new(input.x, 0, input.y);
        _agent.velocity = _agent.speed * movementVector;
    }
}