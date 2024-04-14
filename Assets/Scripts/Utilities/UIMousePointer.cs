using UnityEngine;

namespace Utilities
{
    public class UIMousePointer : HiddenSingleton<UIMousePointer>
    {
        [SerializeField]
        private bool isActive;

        private RectTransform _parentCanvasRectTransform;
        // Start is called before the first frame update
        void Start()
        {
            _parentCanvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive == false)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentCanvasRectTransform, Input.mousePosition,
                null, out var localPoint);
            
            transform.localPosition = localPoint;
        }

        public static void SetActive(bool state)
        {
            Instance.gameObject.SetActive(state);
            Instance.isActive = state;
        }
    }
}
