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
		/// Shows if timer notification was already sent in this loop.
		/// </summary>
		private bool timerNotificationSent;
		/// <summary>
		/// Reset this instance to default state.
		/// </summary>
		public void Reset() {
			timerNotificationSent = false;
			// HACK Null can be safely used here because EffectUpdateEventArgs argument is in fact not used.
			// TODO Either stop passing EffectUpdateEventArgs in effects or create special effect stop-on-reset method.
			StopAllEffects(null);
		}
		/// <summary>
		/// When binding enters the active state all effects are started.
		/// </summary>
		public void Enter(EffectUpdateEventArgs e) {
			//Debug.Log("AnimatorStateBindig.Enter: " + stateName);
			if(!string.IsNullOrEmpty(enterMessage))
				e.controller.gameObject.SendMessage(enterMessage, e, SendMessageOptions.RequireReceiver);
			System.Array.ForEach(effects, effect => effect.Play(e));
		}
		/// <summary>
		/// Updates the active binding state.
		/// </summary>
		public void Update(EffectUpdateEventArgs e) {
			//Debug.Log("AnimatorStateBindig.Update: " + stateName);
			if(!string.IsNullOrEmpty(updateMessage))
				e.controller.gameObject.SendMessage(updateMessage, e, SendMessageOptions.RequireReceiver);
			if(string.IsNullOrEmpty(timerMessage)) return;
			if(timerTreshold == .0f) return;
			if(timerNotificationSent && e.controller.layerState[e.layerIndex].stateSeconds <= timerTreshold) {
				timerNotificationSent = false;
			}
			else if(!timerNotificationSent && e.controller.layerState[e.layerIndex].stateSeconds > timerTreshold) {
				e.controller.gameObject.SendMessage(timerMessage, e, SendMessageOptions.RequireReceiver);
				timerNotificationSent = true;
			}
		}
		/// <summary>
		/// When binding exits active state all effects are stopped.
		/// </summary>
		public void Exit(EffectUpdateEventArgs e) {
			//Debug.Log("AnimatorStateBindig.Exit: " + stateName);
			if(!string.IsNullOrEmpty(exitMessage))
				e.controller.gameObject.SendMessage(exitMessage, e, SendMessageOptions.RequireReceiver);
			StopAllEffects(e);
		}
		/// <summary>
		/// Stops all effects.
		/// </summary>
		private void StopAllEffects(EffectUpdateEventArgs e) {
			System.Array.ForEach(effects, effect => effect.Stop(e));
		}
	}
}