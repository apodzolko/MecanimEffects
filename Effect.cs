using UnityEngine;
using System.Linq;

namespace MecanimEffects {
	/// <summary>
	/// Visual and sound effect represented as object.
	/// </summary>
	[System.Serializable]
	public sealed class AnimatorEffect {
		/// <summary>
		/// The parent object for instantiating a prefab and playing a sound.
		/// </summary>
		public GameObject subject;
		/// <summary>
		/// The effect's prefab.
		/// </summary>
		public GameObject prefab;
		/// <summary>
		/// Send "Reset" message to the effect object when it played another time.
		/// </summary>
		public bool resetOnReplay;
		/// <summary>
		/// The cached effect instance.
		/// </summary>
		private GameObject cachedInstance;
		/// <summary>
		/// If this effect is currently playing.
		/// </summary>
		private bool playing;
		/// <summary>
		/// The playing subject.
		/// </summary>
		private GameObject playingSubject;
		/// <summary>
		/// The playing source.
		/// </summary>
		private AudioSource playingSource;
		/// <summary>
		/// Plays the effect if additional conditions are met.
		/// </summary>
		public void Play(EffectUpdateEventArgs e) {
			if(prefab == null) return;
			if(playing) return;
			playing = true;
			playingSubject = this.subject == null ? e.controller.gameObject : this.subject;
			if(cachedInstance == null) {
				var position = playingSubject.transform.position;
				var rotation = playingSubject.transform.rotation;
				cachedInstance = GameObject.Instantiate(prefab, position, rotation) as GameObject;
				cachedInstance.transform.parent = playingSubject.transform;
			}
			else {
				cachedInstance.SetActive(true);
				cachedInstance.transform.parent = playingSubject.transform;
				if(resetOnReplay)
					cachedInstance.SendMessage("Reset", SendMessageOptions.RequireReceiver);
			}
		}
		/// <summary>
		/// Stops playing effect.
		/// </summary>
		public void Stop(EffectUpdateEventArgs e) {
			if(cachedInstance == null) return;
			if(!playing) return;
			playing = false;
			cachedInstance.SetActive(false);
		}
	}
}