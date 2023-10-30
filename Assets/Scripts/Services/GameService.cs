using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services
{
	public class GameService
	{
		// Const
		private const string FirstDayShowRate = "fds";
		private const string IsRateUsKey = "iru";
		private const string TimesRate = "tru";

		// list of day that will show rate us.
		private readonly List<int> dayShowRate = new() { 0, 2, 10, 30 };
		private DateTime firstTimeShowRate;

		// Link
		private readonly string tosURL;
		private readonly string privacyURL;
		private readonly string rateURL;
		public GameService(string tos, string privacy, string rate)
		{
			this.tosURL = tos;
			this.privacyURL = privacy;
			this.rateURL = rate;
		}

		public void Rate()
		{
#if UNITY_ANDROID
            Application.OpenURL(rateURL);
#elif UNITY_IOS
            UnityEngine.iOS.Device.RequestStoreReview();
#endif
		}

		public void TOS()
		{
			Application.OpenURL(tosURL);
		}

		public void Privacy()
		{
			Application.OpenURL(privacyURL);
		}
		/// <summary>
		/// Save when user rate game.
		/// </summary>
		public void SetRate()
		{
			PlayerPrefs.SetInt(IsRateUsKey, 1);
		}
		/// <summary>
		/// Return true if user rated game, false if not.
		/// </summary>
		/// <returns></returns>
		public bool GetRate()
		{
			int first = PlayerPrefs.GetInt(IsRateUsKey, 0);

			if (first == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		/// <summary>
		/// Return true if reach time that can show rate game, return false if not. Once call this function, time can show rate will set to next day in list show rate.
		/// </summary>
		/// <returns></returns>
		public bool CanShowRate()
		{
			if (GetRate() == true)
			{
				return false;
			}

			int timesRate = PlayerPrefs.GetInt(TimesRate, 0);

			if (timesRate == 0)
			{
				firstTimeShowRate = DateTime.Today;
				PlayerPrefs.SetString(FirstDayShowRate, firstTimeShowRate.ToString());
				PlayerPrefs.SetInt(TimesRate, timesRate + 1);
				return true;
			}
			if (timesRate == 1)
			{
				DateTime firstTime = DateTime.Parse(PlayerPrefs.GetString(FirstDayShowRate, DateTime.Today.ToString()));
				if ((DateTime.Today - firstTime).TotalDays >= dayShowRate[1])
				{
					// Show Rate
					PlayerPrefs.SetInt(TimesRate, timesRate + 1);
					return true;
				}
				return false;
			}
			if (timesRate == 2)
			{
				DateTime firstTime = DateTime.Parse(PlayerPrefs.GetString(FirstDayShowRate, DateTime.Today.ToString()));
				if ((DateTime.Today - firstTime).TotalDays >= dayShowRate[2])
				{
					// Show Rate
					PlayerPrefs.SetInt(TimesRate, timesRate + 1);
					return true;
				}
				return false;
			}
			if (timesRate == 3)
			{
				DateTime firstTime = DateTime.Parse(PlayerPrefs.GetString(FirstDayShowRate, DateTime.Today.ToString()));
				if ((DateTime.Today - firstTime).TotalDays >= dayShowRate[3])
				{
					// Show Rate
					PlayerPrefs.SetInt(TimesRate, timesRate + 1);
					return true;
				}
				return false;
			}
			return false;
		}
	}
}
