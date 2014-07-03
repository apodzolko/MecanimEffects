using UnityEngine;
using System.Collections;

namespace MecanimEffects {
	/// <summary>
	/// Animator state binding connects chosen animator state to an effects list.
	/// </summary>
	[System.Serializable]
	public sealed class AnimatorStateBinding {
		/// <summary>
		/// The name of the state (readonly).
		/// </summary>
		public string stateName;
		/// <summary>
		/// The name for the message sent in the first frame of the bound state (readonly).
		/// </summary>
		public string enterMessage;
		/// <summary>
		/// The name for the message sent in the frame following the last frame of the bound state (readonly).
		/// </summary>
		public string exitMessage;
		/// <summary>
		/// The name for the message sent every frame of the bound state (readonly).
		/// </summary>
		public string updateMessage;
		/// <summary>
		/// The name of the message set when timer reached each one of treshold's values (readonly).
		/// </summary>
		public string timerMessage;
		/// <summary>
		/// The timer tresholds (readonly).
		/// </summary>
		public float timerTreshold;
		/// <summary>
		/// The effects what should be played along with this state (readonly).
		/// </summary>
		public AnimatorEffect[] effects;
		/// <summary>
		/// When last timer was started.
		/// </summary>
		private float timestamp;
		/// <summary>
		/// Reset this instance to default state.
		/// </summary>
		public void Reset() {
			System.Array.ForEach(effects, effect => effect.Reset());
			timestamp = .0f;
		}
		/// <summary>
		/// Plays all effects, bound to the animator's state.
		/// </summary>
		public void Enter(LayerInfo li) {
			li.controller.Trace("AnimatorStateBinding.Enter: {0}", stateName);
			if(!string.IsNullOrEmpty(enterMessage)) {
				li.controller.gameObject.SendMessage(enterMessage, li, SendMessageOptions.RequireReceiver);
			}
			foreach(var effect in effects) {
				effect.Play(li);
			}
			if(timerTreshold != .0f && !string.IsNullOrEmpty(timerMessage)) {
				li.controller.StartCoroutine(TimerCoroutine(li));
			}
		}
		/// <summary>
		/// Checks and sends update and timer messages when needed.
		/// </summary>
		public void Update(LayerInfo li) {
			li.controller.Trace("AnimatorStateBinding.Update: {0}", stateName);
			if(!string.IsNullOrEmpty(updateMessage))
				li.controller.gameObject.SendMessage(updateMessage, li, SendMessageOptions.RequireReceiver);
		}
		/// <summary>
		/// Stops all effects, bound to the animator's state.
		/// </summary>
		public void Exit(LayerInfo li) {
			li.controller.Trace("AnimatorStateBinding.Exit: {0}", stateName);
			if(!string.IsNullOrEmpty(exitMessage)) {
				li.controller.gameObject.SendMessage(exitMessage, li, SendMessageOptions.RequireReceiver);
			}
			foreach(var effect in effects) {
				effect.Stop(li);
			}
		}
		/// <summary>
		/// Sends a timer message if state still active after the timer treshold.
		/// </summary>
		private IEnumerator TimerCoroutine(LayerInfo li) {
			var timestamp = this.timestamp = Time.time;
			yield return new WaitForSeconds(timerTreshold);

			// execution of coroutine will be continued here after some time

			if(timestamp != this.timestamp) yield return null;
			if(!li.state.IsName(stateName)) yield return null;
			li.controller.Trace("AnimatorStateBinding.Timer: {0}", stateName);
			li.controller.gameObject.SendMessage(timerMessage, li, SendMessageOptions.RequireReceiver);
		}
	}
}