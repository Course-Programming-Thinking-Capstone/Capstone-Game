using UnityEngine;

namespace Utilities
{
    public class BackgroundFitCamera : MonoBehaviour
    {
        [SerializeField] private float width;
        [SerializeField] private float heigh;
        [SerializeField] private Transform bgTf;
        [SerializeField] private Transform minSide;

        private void Start()
        {
            if ((Camera.main.orthographicSize * 2.0f * Camera.main.aspect) < minSide.position.x * 2.0f)
            {
                Camera.main.orthographicSize *= (minSide.position.x * 2.0f) /
                                                (Camera.main.orthographicSize * 2.0f * Camera.main.aspect);
            }

            float cameraHeight = Camera.main.orthographicSize * 2.0f;
            float cameraWidth = cameraHeight * Screen.width / Screen.height;

            float heightRatio = cameraHeight / heigh;
            float widthRatio = cameraWidth / width;

            bgTf.localScale = Vector3.one * Mathf.Max(heightRatio, widthRatio);
        }

        public void Fit()
        {
            float cameraHeight = Camera.main.orthographicSize * 2.0f;
            float cameraWidth = cameraHeight * Screen.currentResolution.width / Screen.currentResolution.height;

            float heightRatio = cameraHeight / heigh;
            float widthRatio = cameraWidth / width;

            bgTf.localScale = Vector3.one * Mathf.Max(heightRatio, widthRatio);
        }
    }
}