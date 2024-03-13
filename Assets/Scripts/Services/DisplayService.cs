using UnityEngine;

namespace Services
{
    public class DisplayService : MonoBehaviour
    {
        [SerializeField] private GameObject changeOrientationGo;
        private void Awake()
        {
            Application.targetFrameRate = 60;
            changeOrientationGo.SetActive(false);
        }
        public void SetOrientation(int orient)
        {
            ScreenOrientation orientation = (ScreenOrientation)orient;
            //the 'if' is obviously unnecessary. I'm just testing if the comparisons are working as expected. It's an example after all, might as well be thorough.
            if (orientation == ScreenOrientation.Portrait
                || orientation == ScreenOrientation.PortraitUpsideDown)
            {
                changeOrientationGo.SetActive(false);
            }
            else
            {
                if (Application.isMobilePlatform)
                {
                    changeOrientationGo.SetActive(true);
                }
                else
                {
                    changeOrientationGo.SetActive(false);
                }
            }
        }
    }
}