using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class TweenExtensions
{
    public static async UniTask MoveCurved(this Transform transform, Vector3 target, Vector3? angle, CurvedMoveOptions curvedMoveOptions,
                                           CancellationToken cancellationToken = default)
    {
        float distance = Vector3.Distance(transform.position, target);
        float duration = Mathf.Clamp(distance / curvedMoveOptions.Speed, curvedMoveOptions.MinDuration, curvedMoveOptions.MaxDuration);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            var position = Vector3.Lerp(transform.position, target, t);
            position.y = Mathf.Lerp(transform.position.y, target.y, t) + curvedMoveOptions.YDistance * (4 * t * (1 - t));

            transform.position = position;
            if (angle != null)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angle.Value), t);

            await UniTask.Yield(cancellationToken, cancelImmediately: true);
        }

        transform.position = target;
    }
}

[System.Serializable]
public class CurvedMoveOptions
{
    public float Speed = 10f;
    public float MaxDuration = 1.5f;
    public float MinDuration = 0.2f;
    public float YDistance = 1f;
}