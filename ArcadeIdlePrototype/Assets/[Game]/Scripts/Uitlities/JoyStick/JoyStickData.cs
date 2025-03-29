using UnityEngine;

namespace JoyStick
{
    [CreateAssetMenu(fileName = nameof(JoyStickData), menuName = "Data/" + nameof(JoyStickData), order = 0)]
    public class JoyStickData : ScriptableObject
    {
        public Vector2 Data { get; private set; }

        public void SetData(Vector2 data) => Data = data;
    }
}