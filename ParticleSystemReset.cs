using UnityEngine;
using System.Collections;

namespace MecanimEffects {
	/// <summary>
	/// Hack to reset particle systems.
	/// </summary>
	public class ParticleSystemReset : MonoBehaviour {
		/// <summary>
		/// Resets the particle system.
		/// </summary>
		private void Reset() {
			if(particleSystem == null) return;
			particleSystem.Stop(true);
			particleSystem.Clear(true);
			particleSystem.Play(true);
		}
	}
}