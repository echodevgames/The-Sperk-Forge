using UnityEngine;

namespace EchoDevGames.EchoLaunch.Samples.StandaloneLab
{
    /// <summary>
    /// Scene-owned hook for the Laboratory's visible runtime readout.
    ///
    /// Step B binds this component to the fully authored sample presentation.
    /// It intentionally owns no launch state or authority.
    /// </summary>
    public sealed class LaboratoryReadout : MonoBehaviour
    {
        [SerializeField]
        private EchoLaunchRoot launchRoot;

        /// <summary>
        /// Gets the authored First Light root observed by this readout.
        /// </summary>
        public EchoLaunchRoot LaunchRoot => launchRoot;
    }
}
