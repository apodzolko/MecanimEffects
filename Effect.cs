using UnityEngine;
using System.Linq;

namespace MecanimEffects {
	/// <summary>
	/// Visual effect options.
	/// </summary>
	[System.Serializable]
	public sealed class AnimatorEffect {
		/// <summary>
		/// The effect's object.
		/// </summary>
		public GameObject instance;
		/// <summary>
		/// Send "Reset" message to the effect object when it played another time.
		/// </summary>
		public bool resetOnReplay;
		/// <summary>
		/// Plays the effect.
		/// </summary>
		public void Play(EffectUpdateEventArgs e) {
			if(instance == null) return;
			if(instance.activeSelf) return;
			instance.SetActive(true);
			if(resetOnReplay)
				instance.SendMessage("Reset", SendMessageOptions.RequireReceiver);
		}
		/// <summary>
		/// Stops playing the effect.
		/// </summary>
		public void Stop(EffectUpdateEventArgs e) {
			if(instance == null) return;
			if(!instance.activeSelf) return;
			instance.SetActive(false);
		}
	}
}
