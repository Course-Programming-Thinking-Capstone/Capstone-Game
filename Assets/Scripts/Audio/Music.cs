using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Audio
{

	public class Music : MonoBehaviour
	{
		[SerializeField] private List<Sound> musics;

		private AudioService audioService;

		// Cache
		private readonly Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
		private Dictionary<string, float> musicVolumes = new Dictionary<string, float>();
        /// <summary>
        /// Initiate audiosource corresponding to music in list music. Cache audio source with name and cache music volume with name.
        /// </summary>
        private void Awake()
		{
			foreach (var music in musics)
			{
				var audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.clip = music.AudioClip;
				audioSource.playOnAwake = false;

				audioSources.Add(music.Name, audioSource);
				musicVolumes.Add(music.Name, music.Volume);
			}
		}
        /// <summary>
        /// Set volume of music by game volume and default volume.
        /// </summary>
        private void Start()
		{
			foreach (var music in audioSources)
			{
				music.Value.volume = audioService.MusicVolume * musicVolumes[music.Key];
			}
		}

		private void OnDisable()
		{
			audioService.OnSoundChanged -= AudioService_OnMusicChanged;
			audioService.OnSoundVolumeChanged -= AudioService_OnMusicVolumeChanged;
		}
		/// <summary>
		/// Stop all music when music is off.
		/// </summary>
		/// <param name="isOn"></param>
		private void AudioService_OnMusicChanged(bool isOn)
		{
			if (isOn == false)
			{
				foreach (var music in audioSources)
				{
					music.Value.Stop();
				}
			}
		}
		/// <summary>
		/// Tuning music volume when game music change.
		/// </summary>
		/// <param name="volume"></param>
		private void AudioService_OnMusicVolumeChanged(float volume)
		{
			foreach (var music in audioSources)
			{
				music.Value.volume = volume * musicVolumes[music.Key];
				if(IsMusicPlaying(music.Key) == false && music.Value.volume > 0f)
				{
					PlayMusic(music.Key);
					music.Value.loop = true;
				}
			}
		}
		/// <summary>
		/// Play music by name.
		/// </summary>
		/// <param name="name"></param>
		public void PlayMusic(string name)
		{
			if (audioSources.ContainsKey(name))
			{
				audioSources[name].Play();
				audioSources[name].loop = true;
			}
			else
			{
				Logger.Warning($"Music: {name} not found!");
			}
		}
		/// <summary>
		/// Fade music by name in certain time.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="time"></param>
		public void FadeMusic(string name, float time)
		{
			if (audioSources.ContainsKey(name))
			{
				AudioSource audioSource = audioSources[name];
				if (audioSource.isPlaying == false)
				{
					audioSource.Play();
				}
				StartCoroutine(FadeMusicCoroutine(audioSource, time));
			}
			else
			{
				Logger.Warning($"Music: {name} not found!");
			}
		}
		private IEnumerator FadeMusicCoroutine(AudioSource audioSource, float time)
		{
			float deltaT = 0.02f;
			float volumeTemp = audioSource.volume;
			float step = time / deltaT;
			float stepVolume = volumeTemp / step;

			for (int i = 0; i < 150; i++)
			{
				yield return new WaitForSeconds(0.01f);
				audioSource.volume -= stepVolume;
			}

			audioSource.Stop();
			audioSource.volume = volumeTemp;
		}
		/// <summary>
		/// Stop music by name
		/// </summary>
		/// <param name="name"></param>
		public void StopMusic(string name)
		{
			if (audioSources.ContainsKey(name))
			{
				audioSources[name].Stop();
			}
			else
			{
				Logger.Warning($"Music: {name} not found!");
			}
		}
		/// <summary>
		/// Return true if music is playing.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool IsMusicPlaying(string name)
		{
			if (audioSources.ContainsKey(name))
			{
				if (audioSources[name].isPlaying == true)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			Logger.Warning($"Music: {name} not found!");
			return false;
		}
		/// <summary>
		/// Assign audio service to music.
		/// </summary>
		/// <param name="audioService"></param>
		public void Initialized(AudioService audioService)
		{
			this.audioService = audioService;

			audioService.OnMusicChanged += AudioService_OnMusicChanged;
			audioService.OnMusicVolumeChanged += AudioService_OnMusicVolumeChanged;
		}
	}
}
