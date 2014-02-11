using UnityEngine;
using System.Collections;

namespace MecanimEffects {
	/// <summary>
	/// Effect update message arguments.
	/// </summary>
	public class EffectUpdateEventArgs : System.EventArgs {
		public EffectsController controller;
		public int layerIndex;
	}
}