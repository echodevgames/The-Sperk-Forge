using System;
using EchoDevGames.EchoLaunch.Editor.Setup;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal sealed class EchoLaunchValidationRequest :
        IEquatable<EchoLaunchValidationRequest>
    {
        internal EchoLaunchValidationRequest(
            string projectRootPath,
            bool includeInformation = true)
        {
            ProjectRootPath =
                EchoLaunchSetupPathUtility.NormalizeSeparators(
                    projectRootPath);

            IncludeInformation = includeInformation;
        }

        internal string ProjectRootPath { get; }

        internal bool IncludeInformation { get; }

        internal static EchoLaunchValidationRequest CreateDefault()
        {
            return new EchoLaunchValidationRequest(
                EchoLaunchSetupPathSet.DefaultProjectRootPath,
                true);
        }

        public bool Equals(EchoLaunchValidationRequest other)
        {
            return other != null &&
                   string.Equals(
                       ProjectRootPath,
                       other.ProjectRootPath,
                       StringComparison.Ordinal) &&
                   IncludeInformation == other.IncludeInformation;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EchoLaunchValidationRequest);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    ((ProjectRootPath ?? string.Empty).GetHashCode() * 397) ^
                    IncludeInformation.GetHashCode();
            }
        }
    }
}
