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
	}
}