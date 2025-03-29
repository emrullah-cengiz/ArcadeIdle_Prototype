using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyStick
{
    public enum StickType { Fixed, Floating, Dynamic }

    [RequireComponent(typeof(RectTransform))]
    public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private JoyStickData _joyStickData;

        [SerializeField] private StickType _stickType;
        [SerializeField] private float _movementRange = 50f;
        [SerializeField, Range(0f, 1f)] private float _deadZone = 0f;
        [SerializeField] private bool _showOnlyWhenPressed;
        [SerializeField] private bool _showOnStart;
        [SerializeField] private RectTransform _background, _handle;

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (!_showOnStart && _showOnlyWhenPressed)
                _background.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var input = (eventData.position - (Vector2)_background.position) / (_movementRange * _canvas.scaleFactor);
            float magnitude = input.magnitude;

            var data = magnitude < _deadZone ? Vector2.zero : magnitude > 1f ? input.normalized : input;

            _joyStickData.SetData(data);

            if (_stickType == StickType.Dynamic && magnitude > 1f)
                _background.anchoredPosition += _movementRange * (magnitude - 1f) * input.normalized;

            _handle.anchoredPosition = data * _movementRange;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _background.gameObject.SetActive(true);

            if (_stickType != StickType.Fixed)
                _background.localPosition = ScreenToAnchoredPosition(eventData.position);

            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _joyStickData.SetData(Vector2.zero);
            _handle.anchoredPosition = Vector2.zero;

            if (_showOnlyWhenPressed)
                _background.gameObject.SetActive(false);
        }

        private Vector2 ScreenToAnchoredPosition(Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background.parent as RectTransform, screenPos, _canvas.worldCamera, out var localPoint);
            return localPoint;
        }
    }
}