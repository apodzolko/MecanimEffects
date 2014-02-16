using UnityEngine;
using System.Collections;

namespace MecanimEffects {
	/// <summary>
	/// Contains information about animator's layer state, required to run effects.
	/// </summary>
	public struct LayerInfo {
		/// <summary>
		/// Animator state descriptor.
		/// </summary>
		public AnimatorStateInfo state;
		/// <summary>
		/// Animator transition descriptor.
		/// </summary>
		public AnimatorTransitionInfo transition;
		/// <summary>
		/// If animator is in transition.
		/// </summary>
		public bool inTransition;
		/// <summary>
		/// State timer resets every time state gets looped.
		/// </summary>
		public float stateSeconds;
		/// <summary>
		/// State loops counter.
		/// </summary>
		public int loopCount;
		/// <summary>
		/// State time including loops.
		/// </summary>
		public float stateSecondsTotal;
	}
}