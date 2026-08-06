using System;
using System.Collections.Generic;
using System.Threading;

namespace EchoDevGames.EchoLaunch.Editor.Validation
{
    internal sealed class EchoLaunchValidationService
    {
        private static int activeValidation;

        private readonly IEchoLaunchValidationEvidenceSource evidenceSource;

        internal EchoLaunchValidationService()
            : this(new EchoLaunchValidationEvidenceCollector())
        {
        }

        internal EchoLaunchValidationService(
            IEchoLaunchValidationEvidenceSource evidenceSource)
        {
            this.evidenceSource =
                evidenceSource ??
                throw new ArgumentNullException(nameof(evidenceSource));
        }

        internal static bool IsValidationActive =>
            Interlocked.CompareExchange(
                ref activeValidation,
                0,
                0) != 0;

        internal EchoLaunchValidationReport Validate(
            EchoLaunchValidationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string requestFingerprint =
                EchoLaunchValidationFingerprint.ForRequest(request);

            if (Interlocked.CompareExchange(
                    ref activeValidation,
                    1,
                    0) != 0)
            {
                return CreateAlreadyRunningReport(
                    request,
                    requestFingerprint);
            }

            try
            {
                EchoLaunchValidationEvidence evidence;

                try
                {
                    evidence = evidenceSource.Collect(request);
                }
                catch (Exception exception)
                {
                    return CreateEvidenceFailureReport(
                        request,
                        requestFingerprint,
                        exception);
                }

                List<EchoLaunchValidationFinding> findings =
                    new List<EchoLaunchValidationFinding>();

                try
                {
                    findings.AddRange(
                        EchoLaunchValidationRuleCatalog.Evaluate(evidence));
                }
                catch (Exception exception)
                {
                    findings.Add(
                        CreateContainedFailureFinding(
                            request.ProjectRootPath,
                            exception));
                }

                if (!request.IncludeInformation)
                {
                    findings.RemoveAll(
                        finding =>
                            finding.Severity ==
                            EchoLaunchValidationSeverity.Information);
                }

                return new EchoLaunchValidationReport(
                    request,
                    requestFingerprint,
                    evidence.EvidenceFingerprint,
                    findings);
            }
            finally
            {
                Interlocked.Exchange(ref activeValidation, 0);
            }
        }

        internal static void SetValidationActiveForTests(bool active)
        {
            Interlocked.Exchange(
                ref activeValidation,
                active ? 1 : 0);
        }

        private static EchoLaunchValidationReport
            CreateAlreadyRunningReport(
                EchoLaunchValidationRequest request,
                string requestFingerprint)
        {
            return new EchoLaunchValidationReport(
                request,
                requestFingerprint,
                string.Empty,
                new[]
                {
                    new EchoLaunchValidationFinding(
                        EchoLaunchValidationDiagnosticCodes.AlreadyRunning,
                        EchoLaunchValidationSeverity.Warning,
                        "Validation is already running",
                        "A second First Light validation scan was not started.",
                        request.ProjectRootPath,
                        "One validation run is already active.",
                        "Wait for the current validation run to finish.")
                });
        }

        private static EchoLaunchValidationReport
            CreateEvidenceFailureReport(
                EchoLaunchValidationRequest request,
                string requestFingerprint,
                Exception exception)
        {
            return new EchoLaunchValidationReport(
                request,
                requestFingerprint,
                string.Empty,
                new[]
                {
                    CreateContainedFailureFinding(
                        request.ProjectRootPath,
                        exception)
                });
        }

        private static EchoLaunchValidationFinding
            CreateContainedFailureFinding(
                string projectPath,
                Exception exception)
        {
            string exceptionType =
                exception == null
                    ? "UnknownException"
                    : exception.GetType().Name;

            return new EchoLaunchValidationFinding(
                EchoLaunchValidationDiagnosticCodes.EvidenceUnavailable,
                EchoLaunchValidationSeverity.Blocker,
                "Validation evidence is unavailable",
                "A required read-only validation operation failed safely.",
                projectPath,
                "ExceptionType=" + exceptionType + ".",
                "Resolve the Editor, scene, asset, or import condition and validate again.");
        }
    }
}
