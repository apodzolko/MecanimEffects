using UnityEngine;
using System.Collections;

namespace MecanimEffects {
	/// <summary>
	/// Contains information about animator's layer state, required to run effects.
	/// </summary>
	public class LayerInfo {
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
		/// <summary>
		/// Transition timer.
		/// </summary>
		public float transitionSeconds;
		/// <summary>
		/// True if a state change happend this frame.
		/// </summary>
		public bool stateChanged;
		/// <summary>
		/// The animator state info from the previous frame the animator wasn't in transition.
		/// </summary>
		public AnimatorStateInfo prevState;
		/// <summary>
		/// The controller what owns this state.
		/// </summary>
		public EffectsController controller { get; private set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="MecanimEffects.LayerInfo"/> class.
		/// </summary>
		public LayerInfo(EffectsController controller) {
			this.controller = controller;
		}
	}
}