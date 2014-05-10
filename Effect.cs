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
		/// Continious effects does not restart if bound to two or more states.
		/// </summary>
		public bool continious;
		/// <summary>
		/// The play delay.
		/// </summary>
		public float playDelay;
		/// <summary>
		/// The stop delay.
		/// </summary>
		public float stopDelay;
		/// <summary>
		/// Time when play was called.
		/// </summary>
		private float playTimestamp;
		/// <summary>
		/// Time when stop was called.
		/// </summary>
		private float stopTimestamp;
		/// <summary>
		/// Indicates if stop was called after play. Used to cancel play delay when stopped during that delay.
		/// </summary>
		private bool stopTrigger;
		/// <summary>
		/// Indicates if play was called after start. Used to force end stop delay when stopped during that delay.
		/// </summary>
		private bool playTrigger;
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
			//if(instance.activeSelf) return;
			if(playTrigger) return;
			stopTrigger = false;
			playTrigger = true;
			playTimestamp = Time.time;
			PlayOrTail(li);
		}
		/// <summary>
		/// Stops playing the effect.
		/// </summary>
		public void Stop(LayerInfo li) {
			if(instance == null) return;
			//if(!instance.activeSelf) return;
			if(stopTrigger) return;
			stopTrigger = true;
			playTrigger = false;
			stopTimestamp = Time.time;
			StopOrTail(li);
		}
		/// <summary>
		/// Reset the effect to it's initial state - stopped.
		/// </summary>
		public void Reset() {
			playTimestamp = float.NegativeInfinity;
			stopTimestamp = float.NegativeInfinity;
			playTrigger = false;
			stopTrigger = false;
			if(instance != null) {
				instance.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
				instance.SetActive(false);
			}
		}
		/// <summary>
		/// Plays the effect or tails it according to delay.
		/// </summary>
		private void PlayOrTail(LayerInfo li) {
			if(stopTrigger || !playTrigger) return;
			var time = .0f;
			if(li.inTransition) {
				// HACK This may be inaccurate.
				time = li.transitionSeconds - (li.transitionSeconds / li.transition.normalizedTime);
				li.controller.Trace("PlayOrTrail: time = {0} - ({0} / {1})", li.transitionSeconds, li.transition.normalizedTime);
			}
			else {
				time = li.stateSecondsTotal;
				li.controller.Trace("PlayOrTrail: time = {0}", li.stateSecondsTotal);
			}
			if(time >= playDelay) {
				li.controller.Trace("PlayOrTail >> Play {0} {1}", instance, time);
				instance.SetActive(true);
				if(resetOnReplay) {
					instance.SendMessage("Reset", SendMessageOptions.RequireReceiver);
				}
			}
			else {
				li.controller.Trace("PlayOrTail >> Tail {0} {1}", instance, time);
				li.controller.Tail(() => PlayOrTail(li));
			}
		}
		/// <summary>
		/// Plays the effect or tails it according to delay.
		/// </summary>
		private void StopOrTail(LayerInfo li) {
			if(playTrigger || !stopTrigger) return;
			var time = Time.time - stopTimestamp;
			if(time >= stopDelay) {
				li.controller.Trace("StopOrTail >> Stop {0} {1}", instance, time);
				instance.SetActive(false);
			}
			else {
				li.controller.Trace("StopOrTail >> Tail {0} {1}", instance, time);
				li.controller.Tail(() => StopOrTail(li));
			}
		}
	}
}
