using System;
using UnityEngine;

namespace Vibro
{
	/// <summary>
	/// Class that operates the vibrator on the device.
	/// If your process exits, any vibration you started will stop.
	/// </summary>
	public sealed class AndroidVibrator : IDisposable
	{
		public enum EffectId
		{
			Click = 0,
			DoubleClick = 1,
			Tick = 2,
			HeavyClick = 5
		}

		private static AndroidVibrator _instance;
		private readonly AndroidJavaObject _vibrator;

		/// <summary>
		/// Get vibrator instance.
		/// </summary>
		/// <returns>Vibrator instance or <c>null</c></returns>
		public static AndroidVibrator Build()
		{
#if UNITY_EDITOR
			_instance = null;
#elif UNITY_ANDROID
			if (_instance == null)
			{
				using (var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					using (var activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
					{
						var vibratorBuilder = new AndroidJavaClass("com.volgmartin.plugins.Vibro");
						var vibrator = vibratorBuilder.CallStatic<AndroidJavaObject>("make", activity);

						if (vibrator != null)
						{
							_instance = new AndroidVibrator(vibrator);
						}
					}
				}
			}
#endif
			return _instance;
		}

		private AndroidVibrator(AndroidJavaObject vibrator)
		{
			_vibrator = vibrator;
		}

		/// <summary>
		/// Check whether the hardware has a vibrator.
		/// <para>True if the hardware has a vibrator, else false.</para>
		/// </summary>
		public bool HasVibrator
		{
			get
			{
				return _vibrator.Call<bool>("hasVibrator");
			}
		}

		/// <summary>
		/// Check whether the vibrator has amplitude control.
		/// <para>True if the hardware can control the amplitude of the vibrations, otherwise false.</para>
		/// </summary>
		public bool HasAmplitudeControl
		{
			get
			{
				return _vibrator.Call<bool>("hasAmplitudeControl");
			}
		}

		/// <summary>
		/// Vibrate constantly for the specified period of time.
		/// </summary>
		/// <param name="milliseconds">The number of milliseconds to vibrate.</param>
		public void Vibrate(long milliseconds)
		{
			_vibrator.Call("vibrate", milliseconds);
		}

		/// <summary>
		/// Vibrate constantly for the specified period of time.
		/// </summary>
		/// <param name="milliseconds">The number of milliseconds to vibrate.</param>
		/// <param name="amplitude">The strength of the vibration. This must be a value between 1 and 255, or default value -1.</param>
		public void Vibrate(long milliseconds, int amplitude = -1)
		{
			_vibrator.Call("vibrate", milliseconds, amplitude);
		}

		/// <summary>
		/// Create a waveform vibration.
		/// Pass in an array of ints that are the durations for which to turn on or off the vibrator in milliseconds.
		/// The first value indicates the number of milliseconds to wait before turning the vibrator on.
		/// The next value indicates the number of milliseconds for which to keep the vibrator on before turning it off.
		/// Subsequent values alternate between durations in milliseconds to turn the vibrator off or to turn the vibrator on. 
		/// To cause the pattern to repeat, pass the index into the timings array at which to start the repetition, or -1 to disable repeating.
		/// </summary>
		/// <param name="timings">An array of longs of times for which to turn the vibrator on or off. Timing values of 0 will cause the pair to be ignored.</param>
		/// <param name="repeat">The index into the timings array at which to repeat, or -1 if you you don't want to repeat.</param>
		public void Vibrate(long[] timings, int repeat = -1)
		{
			_vibrator.Call("vibrate", timings, repeat);
		}

		/// <summary>
		/// Create a waveform vibration.
		/// Waveform vibrations are a potentially repeating series of timing and amplitude pairs.
		/// For each pair, the value in the amplitude array determines the strength of the vibration and the value in the timing array determines how long it vibrates for, in milliseconds.
		/// Amplitude values must be between 0 and 255, and an amplitude of 0 implies no vibration (i.e. off). Any pairs with a timing value of 0 will be ignored.
		/// To cause the pattern to repeat, pass the index into the timings array at which to start the repetition, or -1 to disable repeating.
		/// </summary>
		/// <param name="timings">The timing values, in milliseconds, of the timing / amplitude pairs. Timing values of 0 will cause the pair to be ignored.</param>
		/// <param name="repeat">The amplitude values of the timing / amplitude pairs. Amplitude values must be between 0 and 255, or equal to default value -1. An amplitude value of 0 implies the motor is off.</param>
		public void Vibrate(long[] timings, int[] amplitudes, int repeat = -1)
		{
			_vibrator.Call("vibrate", timings, amplitudes, repeat);
		}

		/// <summary>
		/// Create a predefined vibration effect.
		/// This will fallback to a generic pattern if one exists and there does not exist a hardware-specific implementation of the effect.
		/// </summary>
		/// <param name="effectId"></param>
		public void Vibrate(EffectId effectId)
		{
			int id = (int)effectId;
			_vibrator.Call("vibrate", id);
		}

		/// <summary>
		/// Turn the vibrator off.
		/// </summary>
		public void Cancel()
		{
			_vibrator.Call("cancel");
		}

		public void Dispose()
		{
			_vibrator.Dispose();
		}
	}
}