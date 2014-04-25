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
		/// Print trace messages to console (> 2 every frame).
		/// </summary>
		public bool trace;
		/// <summary>
		/// The state info for all animator's layers.
		/// </summary>
		private LayerInfo[] layerState = new LayerInfo[0];
		/// <summary>
		/// List of effects what asked to last for one more frame.
		/// </summary>
		private List<System.Action> tail = new List<System.Action>();
		/// <summary>
		/// Copy of tail so original array could be modified while the controller is processing it's copy.
		/// </summary>
		private List<System.Action> tailCopy = new List<System.Action>();
		/// <summary>
		/// The active bindings.
		/// </summary>
		private List<AnimatorStateBinding> activeBindings = new List<AnimatorStateBinding>();
		/// <summary>
		/// The frame number for debug purposes.
		/// </summary>
		private int frameNo;
		/// <summary>
		/// Tries to provide good default values and allows reuse of this instance.
		/// </summary>
		private void Reset() {
			if(animator == null) animator = GetComponent<Animator>();
			if(animator != null) {
				layerState = new LayerInfo[animator.layerCount];
				for(var i = 0; i < layerState.Length; i++) layerState[i] = new LayerInfo(this);
				foreach(var binding in bindings) binding.Reset();
			}
			// TODO Following lines will cause a wired exception when called from within acivating bindig or effects being tailed.
			activeBindings.Clear();
			tail.Clear();
			tailCopy.Clear();
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
			// First of all we increment frames counter.

			frameNo++;

			// This is where main MecanimEffects magic is done and it is heavily commented. Do not expect other
			// functions to be documented that much, i'm too lazy for that.

			if(layerState == null || layerState.Length != animator.layerCount) return;
			if(animator == null) return;

			// Tail all effects from previous frame, if any.
			// Any effect cam request to be tailed for one more frame while tailed this frame. That's why array is
			// being copied and cleared before procesing is done.

			tailCopy.Clear();
			tailCopy.AddRange(tail);
			tail.Clear();	// Tail must be cleared exactly here, right after copying and prior to processing.
			tailCopy.ForEach(action => action());

			// Enter, Exit and Update messages shall be properly ordered using this arrays.
			
			var exits = new List<AnimatorStateBinding>();
			var enters = new List<AnimatorStateBinding>();
			
			// Process all the animator's layers and handle their activity changes.

			for(var layer = 0; layer < animator.layerCount; layer++) {

				// Check if animator is in transition and read it's state or transition info correspondingly.

				var inTransition = animator.IsInTransition(layer);
				var stateInfo = inTransition ? animator.GetNextAnimatorStateInfo(layer) : animator.GetCurrentAnimatorStateInfo(layer);

				// Animator state change is counted when animator previously was not in transition and this frame enters
				// a transition or new state (when transition length is zero).

				var changed = !layerState[layer].inTransition && (inTransition || stateInfo.nameHash != layerState[layer].state.nameHash);

				// Update the layer state to reflect changes. Note all checks are made before update.

				UpdateLayerStateInfo(inTransition, layer, changed);

				// Fill in the lists of bindings to be informed of exit, enter and/or update.
				// TODO Index binding by mapping state name hashes to them using Dictionary to avoid double loop.

				foreach(var binding in bindings) {
					if(changed) {
						if(layerState[layer].prevState.IsName(binding.stateName)) {
							activeBindings.Remove(binding);
							exits.Add(binding);
						}
						if(stateInfo.IsName(binding.stateName)) {
							activeBindings.Add(binding);
							enters.Add(binding);
						}
					}
				}

				// Next we send messages to the bindings in proper order (Exit - Enter - Update).

				// FIXME Doing this here is completely wrong from the performance point. I need to figure out how to
				//		 find binding's layer to do all updates by looping through activeBindings later (see comments
				//       below). The BEST SOLUTION would be to send all messages to bindings after the big loop at once.

				foreach(var b in exits) b.Exit(layerState[layer]);
				foreach(var b in enters) b.Enter(layerState[layer]);
				foreach(var b in activeBindings) b.Update(layerState[layer]);

				// Finally clear the lists so they can be reused in next loop iteration.

				exits.Clear();
				enters.Clear();

			}

			// At last, update all active bindings.
			// foreach(var binding in activeBindings) {
			//     WTF HOW TO GET THE LAYER ?!?
			// }

		}
		/// <summary>
		/// Returns the effect controller state information for the animator layer specified.
		/// </summary>
		public LayerInfo GetLayerStateInfo(int layerIndex) {
			if(layerIndex < 0 || layerIndex >= layerState.Length) return null;
			return layerState[layerIndex];
		}
		/// <summary>
		/// Tails the action for one more frame. The action can then tail itself for more if needed.
		/// </summary>
		public void Tail(System.Action tailAction) {
			tail.Add(tailAction);
		}
		/// <summary>
		/// Prints a trace message to console if configured. Use this to trace effects execution only.
		/// </summary>
		public void Trace(string format, params object[] args) {
			if(!trace) return;
			Debug.Log(frameNo + ". " + string.Format(format, args), gameObject);
		}
		/// <summary>
		/// Updates the state of the selected layer with actual data.
		/// </summary>
		private void UpdateLayerStateInfo(bool inTransition, int layer, bool changed) {
			// TODO Refactor this function to be a part of LayerInfo class.
			layerState[layer].inTransition = inTransition;
			if(inTransition) {
				if(layerState[layer].inTransition) {
					layerState[layer].transitionSeconds += Time.deltaTime;
				}
				else {
					// TODO This way transition time is inaccurate, but no other way to get it introduced yet.
					layerState[layer].transitionSeconds = .0f;
				}
				layerState[layer].transition = animator.GetAnimatorTransitionInfo(layer);
			}
			else {
				var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
				layerState[layer].prevState = layerState[layer].state;
				layerState[layer].state = stateInfo;
				layerState[layer].loopCount = Mathf.FloorToInt(stateInfo.normalizedTime);
				layerState[layer].stateSeconds = stateInfo.length * (stateInfo.normalizedTime - layerState[layer].loopCount);
				layerState[layer].stateSecondsTotal = stateInfo.length * layerState[layer].loopCount;
			}
		}
	}
}