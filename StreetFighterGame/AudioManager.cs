using System;
using System.IO;
using NAudio.Wave;

namespace StreetFighterGame
{
    public static class AudioManager
    {
        private static WaveOutEvent output;
        private static AudioFileReader reader;
        private static string musicPath;

        public static bool IsEnabled { get; private set; }

        public static void StartLoop(string path)
        {
            Stop();

            if (string.IsNullOrWhiteSpace(path))
            {
                IsEnabled = false;
                return;
            }

            string fullPath = Path.IsPathRooted(path)
                ? path
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            if (!File.Exists(fullPath))
            {
                IsEnabled = false;
                return;
            }

            try
            {
                musicPath = fullPath;
                reader = new AudioFileReader(fullPath);
                output = new WaveOutEvent();
                output.PlaybackStopped += PlaybackStopped;
                output.Init(reader);
                IsEnabled = true;
                output.Play();
            }
            catch
            {
                Stop();
            }
        }

        public static void Toggle()
        {
            if (output == null)
            {
                if (!string.IsNullOrEmpty(musicPath))
                {
                    StartLoop(musicPath);
                }
                return;
            }

            if (IsEnabled)
            {
                IsEnabled = false;
                try
                {
                    output.Pause();
                }
                catch
                {
                    Stop();
                }
            }
            else
            {
                IsEnabled = true;
                try
                {
                    output.Play();
                }
                catch
                {
                    Stop();
                }
            }
        }

        public static void Stop()
        {
            IsEnabled = false;

            if (output != null)
            {
                output.PlaybackStopped -= PlaybackStopped;

                try
                {
                    output.Stop();
                }
                catch
                {
                }

                output.Dispose();
                output = null;
            }

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
        }

        private static void PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (!IsEnabled || reader == null || output == null)
            {
                return;
            }

            try
            {
                reader.Position = 0;
                output.Play();
            }
            catch
            {
                IsEnabled = false;
            }
        }
    }
}
