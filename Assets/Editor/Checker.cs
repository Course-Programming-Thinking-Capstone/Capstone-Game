using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Editor
{
	internal static class Checker
	{
		public static void Check()
		{
			CheckInBuildScenes();
			CheckInPrefabs();
			Debug.Log("Check Done");
		}

		private static void CheckInBuildScenes()
		{
			var buildScenes = GetBuildScenes();
			if (buildScenes.Count == 0) return;

			if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

			var previousScenes = GetActiveScenes().Select(scene => scene.path).ToArray();

			CheckScenes(buildScenes);

			try
			{
				var firstScene = previousScenes.FirstOrDefault();
				if (!string.IsNullOrEmpty(firstScene))
				{
					EditorSceneManager.OpenScene(firstScene);
					foreach (var previousScene in previousScenes.Skip(1))
					{
						EditorSceneManager.OpenScene(previousScene, OpenSceneMode.Additive);
					}
				}
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}

		private static void CheckScenes(List<string> scenes)
		{
			for (int i = 0; i < scenes.Count; i++)
			{
				var scenePath = scenes[i];
				var loadedScene = GetActiveScenes().FirstOrDefault(scene => scene.path == scenePath);
				if (loadedScene == default)
				{
					// The scene isn't actually loaded and we have to load it in the editor manually
					if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

					loadedScene = EditorSceneManager.OpenScene(scenePath);
				}
				CheckScene(loadedScene);
			}
		}

		private static void CheckScene(Scene scene)
		{
			var buttons = scene.GetRootGameObjects().SelectMany(obj => obj.GetComponentsInChildren<Button>()).ToArray();
			CheckButtons($"Scene {scene.name}", buttons);

			var toggles = scene.GetRootGameObjects().SelectMany(obj => obj.GetComponentsInChildren<Toggle>()).ToArray();
			CheckToggles($"Scene {scene.name}", toggles);
		}

		private static void CheckInPrefabs()
		{
			var prefabGUIDs = AssetDatabase.FindAssets("t:GameObject");
			for (int i = 0; i < prefabGUIDs.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);

				var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				CheckButtons($"Prefab {prefab.name}", prefab.GetComponentsInChildren<Button>());
				CheckToggles($"Prefab {prefab.name}", prefab.GetComponentsInChildren<Toggle>());
			}
		}

		private static void CheckButtons(string parrent, Button[] buttons)
		{
			var wrongs = new List<GameObject>();
			foreach (var button in buttons)
			{
				var persistentEventCount = button.onClick.GetPersistentEventCount();
				if (persistentEventCount > 0)
				{
					for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
					{
						var objectName = button.onClick.GetPersistentTarget(i);
						if (objectName == null)
						{
							wrongs.Add(button.gameObject);
							break;
						}

						var type = objectName.GetType();
						var methodName = button.onClick.GetPersistentMethodName(i);
						var privateMethodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
						if (privateMethodInfo != null)
						{
							wrongs.Add(button.gameObject);
							break;
						}

						var publicMethodInfo = type.GetMethod(methodName);
						if (publicMethodInfo == null)
						{
							wrongs.Add(button.gameObject);
							break;
						}
					}
				}
				else
				{
					wrongs.Add(button.gameObject);
					continue;
				}
			}

			foreach (var wrong in wrongs)
			{
				Debug.LogWarning($"{parrent}:{GameObjectPath(wrong)} is wrong");
			}
		}

		private static void CheckToggles(string parrent, Toggle[] toggles)
		{
			var wrongs = new List<GameObject>();
			foreach (var toggle in toggles)
			{
				var persistentEventCount = toggle.onValueChanged.GetPersistentEventCount();
				if (persistentEventCount > 0)
				{
					for (int i = 0; i < toggle.onValueChanged.GetPersistentEventCount(); i++)
					{
						var objectName = toggle.onValueChanged.GetPersistentTarget(i);
						if (objectName == null)
						{
							wrongs.Add(toggle.gameObject);
							break;
						}

						var type = objectName.GetType();
						var methodName = toggle.onValueChanged.GetPersistentMethodName(i);
						var privateMethodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
						if (privateMethodInfo != null)
						{
							wrongs.Add(toggle.gameObject);
							break;
						}

						var publicMethodInfo = type.GetMethod(methodName);
						if (publicMethodInfo == null)
						{
							wrongs.Add(toggle.gameObject);
							break;
						}
					}
				}
				else
				{
					wrongs.Add(toggle.gameObject);
					continue;
				}
			}

			foreach (var wrong in wrongs)
			{
				Debug.LogWarning($"{parrent}:{GameObjectPath(wrong)} is wrong");
			}
		}

		private static List<string> GetBuildScenes()
		{
			var scenes = new List<string>();
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
			}
			return scenes;
		}

		private static List<Scene> GetActiveScenes()
		{
			var scenes = new List<Scene>();
			for (int i = 0; i < SceneManager.loadedSceneCount; i++)
			{
				scenes.Add(SceneManager.GetSceneAt(i));
			}
			return scenes;
		}

		private static string GameObjectPath(GameObject obj)
		{
			string path = "/" + obj.name;
			while (obj.transform.parent != null)
			{
				obj = obj.transform.parent.gameObject;
				path = "/" + obj.name + path;
			}
			return path;
		}
	}
}