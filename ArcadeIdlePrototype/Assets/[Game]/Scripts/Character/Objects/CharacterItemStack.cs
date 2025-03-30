using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterItemStack : ItemStorage
{
    [Title("Stack Options")] [SerializeField]
    private Transform _stackOriginPoint;

    [SerializeField] private float _damping;

    public override ItemType? ItemType
    {
        get
        {
            if (!HasItem)
                return null;

            var item = Items[0];

            return item.Type;
        }
    }

    private void Update() => TransformUpdate();

    private void TransformUpdate()
    {
        var (lastPos, lastRot) = (_stackOriginPoint.position, _stackOriginPoint.rotation);

        int i = 0;
        foreach (var item in Items)
        {
            var pos = GetItemPosition(i);
            var targetPos = lastPos;
            targetPos.y = _stackOriginPoint.position.y + pos.y;

            var targetRot = lastRot;

            var t = _damping * Time.deltaTime;

            item.transform.SetPositionAndRotation(
                lastPos = Vector3.Lerp(item.transform.position, targetPos, t),
                lastRot = Quaternion.Lerp(item.transform.rotation, targetRot, t * .6f)
            );
            i++;
        }
    }
}