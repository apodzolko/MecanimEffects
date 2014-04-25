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
		/// The play delay.
		/// </summary>
		public float playDelay;
		/// <summary>
		/// The stop delay.
		/// </summary>
		public float stopDelay;
		/// <summary>
		/// The stop timestamp.
		/// </summary>
		private float stopTimestamp;
		/// <summary>
		/// Checks instance for particular components and creates special effects scripts.
		/// </summary>
		public void Prepare() {
			// HACK Not all kind of possible effects suports Reset method, but often it is essential to reset an
			//      effect to it's default state before using it again. This hack seeks for such known components in
			//		the effect's hierarchy and adds the specialized reset script if found.
		}
		/// <summary>
		/// Plays the effect.
		/// </summary>
		public void Play(LayerInfo li) {
			if(instance == null) return;
			if(instance.activeSelf) return;
			instance.SetActive(true);
			if(resetOnReplay)
				instance.SendMessage("Reset", SendMessageOptions.RequireReceiver);
		}
		/// <summary>
		/// Stops playing the effect.
		/// </summary>
		public void Stop(LayerInfo li) {
			if(instance == null) return;
			if(!instance.activeSelf) return;
			instance.SetActive(false);
		}
	}
}
