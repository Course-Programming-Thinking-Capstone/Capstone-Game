using System.Collections.Generic;
using UnityEngine;
using System;
using Audio;

namespace Services
{
    public class AudioService
    {
        /// <summary>
        /// Action sound, sound volume change.
        /// </summary>
        public event Action<bool> OnSoundChanged;
        public event Action<float> OnSoundVolumeChanged;
        /// <summary>
        /// Action music, music volume change.
        /// </summary>
        public event Action<bool> OnMusicChanged;
        public event Action<float> OnMusicVolumeChanged;

        private bool soundOn;
        private bool musicOn;
        private float soundVolume = 1f;
        private float musicVolume = 1f;

        private bool vibrateOn;

        private Music music;
        private Dictionary<string, AudioSource> soundAudioSources;

        // Cache
        private Dictionary<string, float> soundVolumes = new();

        private const string soundKey = "soundk";
        private const string musicKey = "musick";

        /// <summary>
        /// Initiate audio service.
        /// </summary>
        /// <param name="music">class music from entry scene</param>
        /// <param name="sounds">list sound from entry scene.</param>
        /// <param name="soundObject">object that attached audio source</param>
        public AudioService(Music music, List<Sound> sounds, GameObject soundObject)
        {
            this.music = music;
            this.music.Initialized(this);

            soundAudioSources = new Dictionary<string, AudioSource>();
            foreach (var sound in sounds)
            {
                AudioSource soundSource = soundObject.AddComponent<AudioSource>();
                soundSource.clip = sound.AudioClip;
                soundSource.volume = sound.Volume;
                soundSource.playOnAwake = false;
                soundAudioSources.Add(sound.Name, soundSource);
            }

            foreach (var audioSource in soundAudioSources)
            {
                soundVolumes.Add(audioSource.Key, audioSource.Value.volume);
            }

            soundOn = PlayerPrefs.GetInt(soundKey, 1) == 1;
            musicOn = PlayerPrefs.GetInt(musicKey, 1) == 1;
        }


        // Play Music
        public void PlayMusic()
        {
            if (musicOn == true && musicVolume > 0.0f)
            {
                music.PlayMusic("music");
            }
        }

        // Fade Music
        public void FadeMusic(float time)
        {
            if (musicOn == true && musicVolume > 0.0f)
            {
                music.FadeMusic("music", time);
            }
        }

        // End
        /// <summary>
        /// Stop all sound.
        /// </summary>
        public void StopAllSound()
        {
            foreach (var audioSource in soundAudioSources)
            {
                audioSource.Value.Stop();
            }
        }

        /// <summary>
        /// Stop music with name.
        /// </summary>
        public void StopMusic()
        {
            music.StopMusic("music");
        }

        /// <summary>
        /// Return true if music is playing.
        /// </summary>
        /// <returns></returns>
        public bool IsMusicPlaying()
        {
            return music.IsMusicPlaying("music");
        }

        /// <summary>
        /// Set volume of music.
        /// </summary>
        /// <param name="volume"></param>
        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
        }

        /// <summary>
        /// Set volume of sound.
        /// </summary>
        /// <param name="volume"></param>
        public void SetSoundVolume(float volume)
        {
            SoundVolume = volume;
        }

        /// <summary>
        /// Set vibrate.
        /// </summary>
        /// <param name="isOn"></param>
        public void SetVibrate(bool isOn)
        {
            VibrateOn = isOn;
        }

        /// <summary>
        /// Play vibrate
        /// </summary>
        public void Vibrate()
        {
            if (vibrateOn == true)
            {
                Handheld.Vibrate();
            }
        }

        // GET - SET
        public float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                soundVolume = value;
                foreach (var audioSource in soundAudioSources)
                {
                    audioSource.Value.volume = soundVolume * soundVolumes[audioSource.Key];
                }

                OnSoundVolumeChanged?.Invoke(soundVolume);
            }
        }

        public float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                musicVolume = value;
                OnMusicVolumeChanged?.Invoke(musicVolume);
            }
        }

        public bool SoundOn
        {
            get { return soundOn; }
            set
            {
                soundOn = value;
                if (soundOn == false)
                {
                    StopAllSound();
                    PlayerPrefs.SetInt(soundKey, 0); // set to 0
                }
                else
                {
                    PlayerPrefs.SetInt(soundKey, 1); // set to 1
                }

                OnSoundChanged?.Invoke(soundOn);
            }
        }

        public bool MusicOn
        {
            get { return musicOn; }
            set
            {
                musicOn = value;
                if (musicOn)
                {
                    PlayerPrefs.SetInt(musicKey, 1);
                    music.PlayMusic("music");
                }
                else
                {
                    PlayerPrefs.SetInt(musicKey, 0);
                    music.StopMusic("music");
                }

                OnMusicChanged?.Invoke(musicOn);
            }
        }

        public bool VibrateOn
        {
            get { return vibrateOn; }
            set { vibrateOn = value; }
        }
        
        
        // AUDIO PLAY
        
        public void PlaySound(SoundToPlay nameSound)
        {
            if (!soundOn)
            {
                return;
            }
            soundAudioSources[nameSound.ToString()].Play();
        }
    }

    public enum SoundToPlay
    {
        Button,
        GameOver,
        GotSkin,
        HitTorus,
        Lost,
        PerfectScore,
        Scored,
        Tap
    }
}