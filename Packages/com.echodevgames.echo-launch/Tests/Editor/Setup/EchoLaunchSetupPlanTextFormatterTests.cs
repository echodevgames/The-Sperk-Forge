
using EchoDevGames.EchoLaunch.Editor.Setup;
using NUnit.Framework;

namespace EchoDevGames.EchoLaunch.Tests.Editor.Setup
{
    public sealed class EchoLaunchSetupPlanTextFormatterTests
    {
        [Test]
        public void ReportIncludesStatus()
        {
            EchoLaunchSetupPlan plan = CreatePlan();

            Assert.That(
                new EchoLaunchSetupPlanTextFormatter().Format(plan),
                Does.Contain("Status: " + plan.Status));
        }

        [Test]
        public void ReportIncludesPreviewWarning()
        {
            Assert.That(
                new EchoLaunchSetupPlanTextFormatter().Format(CreatePlan()),
                Does.Contain("Preview only"));
        }

        [Test]
        public void ReportIncludesOperations()
        {
            string report =
                new EchoLaunchSetupPlanTextFormatter().Format(CreatePlan());

            Assert.That(report, Does.Contain("Operations:"));
            Assert.That(report, Does.Contain("ResolveConfiguration"));
        }

        [Test]
        public void ReportIncludesDiagnostics()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest(
                    destinationPath: string.Empty);

            EchoLaunchSetupPlan plan =
                new EchoLaunchSetupPlanner().CreatePlan(
                    request,
                    EchoLaunchSetupTestFactory.CreateSnapshot());

            Assert.That(
                new EchoLaunchSetupPlanTextFormatter().Format(plan),
                Does.Contain(EchoLaunchSetupDiagnosticCodes.InvalidRequest));
        }

        [Test]
        public void ReportIsDeterministic()
        {
            EchoLaunchSetupPlan plan = CreatePlan();
            EchoLaunchSetupPlanTextFormatter formatter =
                new EchoLaunchSetupPlanTextFormatter();

            Assert.That(
                formatter.Format(plan),
                Is.EqualTo(formatter.Format(plan)));
        }

        [Test]
        public void NullPlanFormatsAsEmpty()
        {
            Assert.That(
                new EchoLaunchSetupPlanTextFormatter().Format(null),
                Is.Empty);
        }

        private static EchoLaunchSetupPlan CreatePlan()
        {
            EchoLaunchSetupRequest request =
                EchoLaunchSetupTestFactory.CreateRequest();

            return new EchoLaunchSetupPlanner().CreatePlan(
                request,
                EchoLaunchSetupTestFactory.CreateSnapshot());
        }
    }
}
