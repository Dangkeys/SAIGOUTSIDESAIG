using UnityEngine;

namespace Utils
{
    public static class RaycastHelper
    {
        private static Camera _mainCamera;

        /// <summary>
        /// Cached main camera to avoid repeated Camera.main calls
        /// </summary>
        public static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                    _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        /// <summary>
        /// Clears the cached camera (call when camera changes)
        /// </summary>
        public static void ClearCameraCache() => _mainCamera = null;

        /// <summary>
        /// Gets a ray from the center of the screen
        /// </summary>
        public static Ray GetCenterScreenRay()
        {
            if (MainCamera == null)
                return default;

            return MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        }

        /// <summary>
        /// Performs a raycast from center screen and returns hit info
        /// </summary>
        public static bool TryCenterScreenRaycast(float maxDistance, out RaycastHit hit)
        {
            hit = default;
            if (MainCamera == null)
                return false;

            Ray ray = GetCenterScreenRay();
            return Physics.Raycast(ray, out hit, maxDistance);
        }

        /// <summary>
        /// Performs a raycast from center screen with layer mask
        /// </summary>
        public static bool TryCenterScreenRaycast(float maxDistance, LayerMask layerMask, out RaycastHit hit)
        {
            hit = default;
            if (MainCamera == null)
                return false;

            Ray ray = GetCenterScreenRay();
            return Physics.Raycast(ray, out hit, maxDistance, layerMask);
        }

        /// <summary>
        /// Tries to get a component from a raycast hit at center screen
        /// </summary>
        public static bool TryGetComponentFromCenterRaycast<T>(float maxDistance, out T component, out RaycastHit hit) where T : class
        {
            component = null;
            if (!TryCenterScreenRaycast(maxDistance, out hit))
                return false;

            return hit.collider.TryGetComponent(out component);
        }

        /// <summary>
        /// Tries to get a component from a raycast hit at center screen with layer mask
        /// </summary>
        public static bool TryGetComponentFromCenterRaycast<T>(float maxDistance, LayerMask layerMask, out T component, out RaycastHit hit) where T : class
        {
            component = null;
            if (!TryCenterScreenRaycast(maxDistance, layerMask, out hit))
                return false;

            return hit.collider.TryGetComponent(out component);
        }

        /// <summary>
        /// Tries to get a component from a raycast hit (ignoring specified layers)
        /// </summary>
        public static bool TryGetComponentFromCenterRaycastIgnore<T>(float maxDistance, LayerMask ignoreMask, out T component, out RaycastHit hit) where T : class
        {
            return TryGetComponentFromCenterRaycast(maxDistance, ~ignoreMask, out component, out hit);
        }
    }
}
