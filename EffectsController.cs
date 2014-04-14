using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MecanimEffects {
	/// <summary>
	/// Allows for visual and sound effects represented as GameObjects to be bound to mecanim states of given animator.
	/// </summary>
	public sealed class EffectsController : MonoBehaviour {
		/// <summary>
		/// The animator shortcut.
		/// </summary>
		public Animator animator;
		/// <summary>
		/// Effects are bound to mecanim states with bindings.
		/// </summary>
		public AnimatorStateBinding[] bindings;
		/// <summary>
		/// The state info for all animator's layers.
		/// </summary>
		[System.NonSerialized]
		public LayerInfo[] layerState = new LayerInfo[0];
		/// <summary>
		/// Arguments for updating effects.
		/// </summary>
		private EffectUpdateEventArgs updateEventArgs = new EffectUpdateEventArgs();
		/// <summary>
		/// Previous state effects still active.
		/// </summary>
		private bool effectsActive;
		/// <summary>
		/// Tries to provide good default values and allows reuse of this instance.
		/// </summary>
		private void Reset() {
			if(animator == null) animator = GetComponent<Animator>();
			if(animator != null) {
				layerState = new LayerInfo[animator.layerCount];
				foreach(var binding in bindings)
					binding.Reset();
			}
			updateEventArgs.controller = this;
		}
		/// <summary>
		/// Awakes this instance.
		/// </summary>
		private void Awake() {
			Reset();
		}
		/// <summary>
		/// Updates effects bindings for this instance.
		/// </summary>
		private void Update() {
			if(layerState == null || layerState.Length != animator.layerCount) return;
			if(animator == null) return;
			var enter = false;
			var update = false;
			var exit = false;
			for(var i = 0; i < animator.layerCount; i++) {
				updateEventArgs.layerIndex = i;
				enter = update = exit = false;
				UpdateLayerState(i, ref enter, ref update, ref exit);
				UpdateLayerStateBindings(i, enter, update, exit);
			}
		}
		/// <summary>
		/// Updates state information for the particular layer.
		/// </summary>
		private void UpdateLayerState(int index, ref bool enter, ref bool update, ref bool exit) {
			if(animator.IsInTransition(index)) {
				exit = !layerState[index].inTransition;
				if(exit) {
					effectsActive = false;
				}
				layerState[index].transition = animator.GetAnimatorTransitionInfo(index);
				layerState[index].inTransition = true;
			}
			else {
				var stateInfo = animator.GetCurrentAnimatorStateInfo(index);
				enter = layerState[index].inTransition || layerState[index].state.nameHash != stateInfo.nameHash;
				update = enter || layerState[index].state.nameHash == stateInfo.nameHash;
				// HACK This is wrong way to tell prev state bindings to exit in case when transition length = 0.
				if(enter && effectsActive) {
					UpdateLayerStateBindings(index, false, false, true);
				}
				if(enter) {
					effectsActive = true;
				}
				layerState[index].state = stateInfo;
				layerState[index].inTransition = false;
				layerState[index].loopCount = Mathf.FloorToInt(stateInfo.normalizedTime);
				layerState[index].stateSeconds = stateInfo.length * (stateInfo.normalizedTime - layerState[index].loopCount);
				layerState[index].stateSecondsTotal = stateInfo.length * layerState[index].loopCount;
			}
		}
		/// <summary>
		/// Updates the layer state bindings.
		/// </summary>
		private void UpdateLayerStateBindings(int index, bool enter, bool update, bool exit) {
			foreach(var binding in EnumerateBindings(index)) {
				if(enter) binding.Enter(updateEventArgs);
				if(update) binding.Update(updateEventArgs);
				if(exit) binding.Exit(updateEventArgs);
			}
		}
		/// <summary>
		/// Enumerates the animator state bindings bound to particular state.
		/// </summary>
		private IEnumerable<AnimatorStateBinding> EnumerateBindings(int layerIndex) {
			return bindings.Where(b => layerState[layerIndex].state.IsName(b.stateName));
		}
	}
}