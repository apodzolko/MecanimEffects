using UnityEngine;
using System.Collections;

namespace MecanimEffects {
	/// <summary>
	/// Hack to reset particle systems.
	/// </summary>
	public class ParticleSystemReset : MonoBehaviour {
		/// <summary>
		/// The particle system to reset.
		/// </summary>
		public ParticleSystem particleSystem;
		/// <summary>
		/// Resets the particle system.
		/// </summary>
		private void Reset() {
			if(particleSystem == null) {
				particleSystem = GetComponent<ParticleSystem>();
			}
			if(particleSystem == null) return;
			particleSystem.Stop(true);
			particleSystem.Clear(true);
			particleSystem.Play(true);
		}
	}
}