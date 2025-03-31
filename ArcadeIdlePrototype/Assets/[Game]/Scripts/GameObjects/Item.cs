using System.Threading;
using Cysharp.Threading.Tasks;
using PoolSystem;
using UnityEngine;

public enum ItemType { RawCopper, BullionCopper }

public class Item : MonoBehaviour, IInitializablePoolable<ItemData>
{
    [SerializeField] private ItemType _type;

    private CancellationTokenSource _cts;

    public ItemType Type => _type;

    public async UniTask MoveCurved(Vector3 pos, Vector3 angle, CurvedMoveOptions options)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        await transform.MoveCurved(pos, angle, options, _cts.Token);
    }

    public void OnCreated()
    {
    }

    public void OnSpawned(ItemData data, params object[] additionalArgs)
    {
        transform.position = data.Position;
    }

    public void OnDespawned()
    {
    }

    public class Pool : Pool<Item, ItemType>
    {
        public Pool(PoolSettings poolSettings) : base(poolSettings)
        {
        }
    }
}