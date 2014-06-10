using UnityEngine;
using System.Collections;
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
		/// Continious effects does not restart if bound to two or more states (not implemented yet).
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
		/// The timestamp for playing and stopping delayed effects.
		/// </summary>
		private float timestamp;
		/// <summary>
		/// Indicates if effect is playing.
		/// </summary>
		private bool playing;
		/// <summary>
		/// Plays the effect.
		/// </summary>
		public void Play(LayerInfo li) {
			if(instance == null) return;
			if(!playing) {
				timestamp = Time.time;
				playing = true;
				li.controller.StartCoroutine(PlayCoroutine(li));
			}
		}
		/// <summary>
		/// Stops playing the effect.
		/// </summary>
		public void Stop(LayerInfo li) {
			if(instance == null) return;
			if(playing) {
				timestamp = Time.time;
				playing = false;
				li.controller.StartCoroutine(StopCoroutine(li));
			}
		}
		/// <summary>
		/// Reset the effect to it's initial state - stopped.
		/// </summary>
		public void Reset() {
			// TODO Can't trace here because the effect does not know it's controller.
			// li.controller.Trace("Effect.Reset: {0}", instance.name);
			timestamp = Time.time;
			playing = false;
			if(instance != null) {
				instance.SetActive(false);
			}
		}
		/// <summary>
		/// Plays the effect with delay, if specified.
		/// </summary>
		private IEnumerator PlayCoroutine(LayerInfo li) {
			// Thing might have changed over wait inteval, a check to ensure what effect wasn't stopped and, perhaps,
			// played again should be made here. So save the original operation timestamp to check it later.
			var timestamp = this.timestamp;
			if(playDelay != .0f) {
				// Wait one frame for transition's normalizedTime and transtionSeconds to be more than 0 and only then
				// attempt to calculate time.s
				yield return null;
				var time = CalculateTime(li);
				if(time < playDelay) {
					var interval = Mathf.Abs(playDelay - time);
					li.controller.Trace("Effect.PlayCoroutine: {0} delayed for {1} seconds", instance.name, interval);
					yield return new WaitForSeconds(interval);
				}
			}
			if(playing && timestamp == this.timestamp) {
				instance.SetActive(true);
				if(resetOnReplay) {
					instance.SendMessage("Reset", SendMessageOptions.RequireReceiver);
				}
				li.controller.Trace("Effect.PlayCoroutine: {0} activated", instance.name);
			}
			else {
				li.controller.Trace("Effect.PlayCoroutine: {0} canceled", instance.name);
			}
		}
		/// <summary>
		/// Stops the effect with the delay, if specified.
		/// </summary>
		private IEnumerator StopCoroutine(LayerInfo li) {
			// Thing might have changed over wait inteval, a check to ensure what effect wasn't stopped and, perhaps,
			// played again should be made here. So save the original operation timestamp to check it later.
			var timestamp = this.timestamp;
			var time = Time.time - timestamp;
			if(time < stopDelay) {
				li.controller.Trace("Effect.StopCoroutine: {0} delayed for {1} seconds", instance.name, stopDelay);
				yield return new WaitForSeconds(stopDelay);
			}
			if(!playing && timestamp == this.timestamp) {
				instance.SetActive(false);
				li.controller.Trace("Effect.StopCoroutine: {0} deactivated", instance.name);
			}
			else {
				li.controller.Trace("Effect.StopCoroutine: {0} canceled", instance.name);
			}
		}
		/// <summary>
		/// Calculates the time of starting effect from -transition_length to +state_seconds_total.
		/// </summary>
		private float CalculateTime(LayerInfo li) {
			var result = .0f;
			if(li.inTransition) {
				result = li.transitionSeconds - (li.transitionSeconds / li.transition.normalizedTime);
				li.controller.Trace("Effect.CalculateTime: {0} - ({0} / {1}) = {2}", li.transitionSeconds, li.transition.normalizedTime, result);
			}
			else {
				result = li.stateSecondsTotal;
				li.controller.Trace("Effect.CalculateTime: {0}", result);
			}
			return result;
		}
	}
}
