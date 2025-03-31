using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimatorController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speedDampTime = 3f;

    private readonly int MOVE_SPEED_RATIO_PARAM = Animator.StringToHash("MoveSpeedRatio");

    private void Update() => SetMovementSpeed();

    private void SetMovementSpeed()
    {
        var moveSpeedRatio = _agent.velocity.magnitude / _agent.speed;

        _animator.SetFloat(MOVE_SPEED_RATIO_PARAM, moveSpeedRatio, _speedDampTime, Time.deltaTime);
    }
}