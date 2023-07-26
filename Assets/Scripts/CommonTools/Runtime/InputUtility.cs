using CommonTools.Runtime.TaskManagement;
using UnityEngine;

namespace CommonTools.Runtime
{
    public static class InputUtility
    {
        public static bool IsMouseDown { get; private set; }
        public static bool IsMouseHold { get; private set; }
        public static bool IsMouseUp { get; private set; }

        private static Vector3 mouseDownPosition;
        private static Vector3 mousePrevPosition;
        
        public static bool IsMultiTouchEnabled
        {
            get => Input.multiTouchEnabled;
            set => Input.multiTouchEnabled = value;
        }
        
        
        static InputUtility()
        {
            Updater.Subscribe(Update);
        }

        private static void Update()
        {
            IsMouseDown = Input.GetMouseButtonDown(0);
            IsMouseHold = Input.GetMouseButton(0);
            IsMouseUp = Input.GetMouseButtonUp(0);

            if (IsMouseDown)
            {
                mouseDownPosition = Input.mousePosition;
                mousePrevPosition = mouseDownPosition;
            }
        }
        
        public static int GetSwipe(Vector3 axis, float sensitivity = 10f)
        {
            var drag = GetDrag(mouseDownPosition, axis, sensitivity);
            return (int)Mathf.Clamp(drag, -1.1f, 1.1f);
        }

        public static float GetDrag(Vector3 axis, float sensitivity = 10f) =>
            GetDrag(mousePrevPosition, axis, sensitivity);

        private static float GetDrag(Vector3 mouseAnchorPosition, Vector3 axis, float sensitivity)
        {
            var drag = Vector3.Dot(axis, Input.mousePosition - mouseAnchorPosition) * sensitivity / Screen.width;
            
            mousePrevPosition = Input.mousePosition;

            return drag;
        }
    }
}