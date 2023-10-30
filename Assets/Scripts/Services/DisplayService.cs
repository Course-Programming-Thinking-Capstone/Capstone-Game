using UnityEngine;

namespace Services
{
	public class DisplayService
	{
#if UNITY_EDITOR

		public bool IsFake { get; set; }
#endif
		public DisplayService()
		{
			Application.targetFrameRate = 60;
		}
		public bool WideScreen()
		{
			if ((float)Screen.width / (float)Screen.height > 0.58f)
			{
				return true;
			}
			return false;
		}
		public Rect SafeArea()
		{
			Rect safeArea = Screen.safeArea;
			Rect[] cutouts = Cutouts();

			if (safeArea.y == 0)
			{
				float posY = safeArea.height;
				foreach (Rect rect in cutouts)
				{
					if (posY > rect.y)
					{
						posY = rect.y;
					}
				}
				safeArea.y = Screen.height - posY;
			}

#if UNITY_EDITOR
			if (IsFake)
			{
				return new Rect(0f, 90f, Screen.width, Screen.height - 90f);
			}
			else
#endif
			{
				return safeArea;
			}
		}
		public Rect[] Cutouts()
		{
			return Screen.cutouts;
		}
	}
}
