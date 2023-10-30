using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public static class Tools
	{
		[MenuItem("Tools/OneChain/Clear Data", false, 1)]
		private static void ClearData()
		{
			PlayerPrefs.DeleteAll();

			/*
			var thumbnailsPath = Path.Combine(Application.persistentDataPath, "thumbnails");
			if (Directory.Exists(thumbnailsPath)) Directory.Delete(thumbnailsPath, true);
			*/

			Debug.Log("Clear data Done!");
		}

		[MenuItem("Tools/OneChain/Reveal in Finder", false, 2)]
		private static void RevealInFinder() => EditorUtility.RevealInFinder(Application.persistentDataPath);

		[MenuItem("Tools/OneChain/Check Events", false, 1)]
		private static void Check() => Checker.Check();
	}
}
