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
		/// The active bindings.
		/// </summary>
		private List<AnimatorStateBinding> activeBindings = new List<AnimatorStateBinding>();
		/// <summary>
		/// Tries to provide good default values and allows reuse of this instance.
		/// </summary>
		private void Reset() {
			if(animator == null) animator = GetComponent<Animator>();
			if(animator != null) {
				layerState = new LayerInfo[animator.layerCount];
				for(var i = 0; i < layerState.Length; i++) {
					layerState[i] = new LayerInfo(this, i);
				}
				foreach(var binding in bindings) {
					binding.Reset();
				}
			}
			// WTF is the next line comment? Can't remember what it means. Should not write such comments in the future.
			// TODO Following lines will cause a wired exception when called from within acivating bindig or effects being tailed.
			activeBindings.Clear();
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
			// This is where main MecanimEffects magic is done and it is heavily commented. Do not expect other
			// functions to be documented that much, i'm too lazy for that.

			if(layerState == null || layerState.Length != animator.layerCount) return;
			if(animator == null) return;

			// This arrays are required to ensure sending all exit messages before all enter messages.
			
			var exits = new List<AnimatorStateBinding>();
			var enters = new List<AnimatorStateBinding>();
			
			// Process all the animator's layers and handle their activity changes.

			for(var layer = 0; layer < animator.layerCount; layer++) {

				// Clear the lists so they can be reused from previous layer's states if any.
				
				exits.Clear();
				enters.Clear();

				// Update the layer state info to reflect the state machine changes.

				layerState[layer].Update();

				// Handle the state change if any.

				if(layerState[layer].stateChanged) {

					// FIXME Without prev and next state names this info is not very useful here.

					Trace("EffectsController.Update: state has changed");

					// Fill in the lists of bindings to be informed of exit, enter and/or update.
					// TODO Index binding by mapping state name hashes to them using Dictionary to avoid double loop.

					foreach(var binding in bindings) {
						if(layerState[layer].prevState.IsName(binding.stateName)) {
							exits.Add(binding);
							activeBindings.Remove(binding);
						}
						if(layerState[layer].state.IsName(binding.stateName)) {
							activeBindings.Add(binding);
							enters.Add(binding);
						}
					}

					// Invoke exit and enter on the bindings found.
					
					foreach(var binding in exits) {
						binding.Exit(layerState[layer]);
					}
					foreach(var binding in enters) {
						binding.Enter(layerState[layer]);
					}
				}

				// Update all active bindings.

				foreach(var binding in activeBindings) {
					binding.Update(layerState[layer]);
				}
			}
		}
		/// <summary>
		/// Returns the effect controller state information for the animator layer specified.
		/// </summary>
		public LayerInfo GetLayerStateInfo(int layerIndex) {
			if(layerIndex < 0 || layerIndex >= layerState.Length) return null;
			return layerState[layerIndex];
		}
		/// <summary>
		/// Prints a trace message to console if configured. Use this to trace effects execution only.
		/// </summary>
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void Trace(string format, params object[] args) {
			if(!trace) return;
			Debug.Log(Time.frameCount + ". " + string.Format(format, args), gameObject);
		}
	}
}