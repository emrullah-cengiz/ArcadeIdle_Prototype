using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    public async UniTask GoToDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);

        while (!_agent.pathPending && _agent.remainingDistance > _agent.stoppingDistance)
            await UniTask.Yield();
    }
}