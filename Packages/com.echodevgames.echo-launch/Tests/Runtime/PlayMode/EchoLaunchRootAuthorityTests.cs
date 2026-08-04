//----- EchoLaunchRootAuthorityTests.cs START -----

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace EchoDevGames.EchoLaunch.Tests.Runtime
{
    public sealed class EchoLaunchRootAuthorityTests
    {
        private GameObject firstObject;
        private GameObject secondObject;

        [SetUp]
        public void SetUp()
        {
            LaunchAuthorityClaim.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyImmediately(firstObject);
            DestroyImmediately(secondObject);

            firstObject = null;
            secondObject = null;

            LaunchAuthorityClaim.Reset();

            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void FirstRootClaimsAuthority()
        {
            EchoLaunchRoot firstRoot = CreateFirstRoot();

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(firstRoot));

            Assert.That(
                firstRoot.IsAuthoritative,
                Is.True);

            Assert.That(
                firstRoot.WasRejectedAsDuplicate,
                Is.False);

            Assert.That(
                firstRoot.enabled,
                Is.True);
        }

		[Test]
		public void SecondRootIsRejectedWithoutReplacingAuthority()
		{
			EchoLaunchRoot firstRoot = CreateFirstRoot();

			ExpectDuplicateWarning();

			EchoLaunchRoot secondRoot = CreateSecondRoot();

			Assert.That(
				EchoLaunchRoot.Current,
				Is.SameAs(firstRoot));

			Assert.That(
				firstRoot.IsAuthoritative,
				Is.True);

			Assert.That(
				secondRoot.IsAuthoritative,
				Is.False);

			Assert.That(
				secondRoot.WasRejectedAsDuplicate,
				Is.True);

			Assert.That(
				secondRoot.enabled,
				Is.False);
		}

        [Test]
        public void DestroyingDuplicateDoesNotReleaseAuthority()
        {
            EchoLaunchRoot firstRoot = CreateFirstRoot();

            ExpectDuplicateWarning();

            EchoLaunchRoot secondRoot = CreateSecondRoot();

            Object.DestroyImmediate(secondObject);
            secondObject = null;

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(firstRoot));

            Assert.That(
                firstRoot.IsAuthoritative,
                Is.True);

            Assert.That(
                secondRoot == null,
                Is.True);
        }

        [Test]
        public void DestroyingAuthorityReleasesClaim()
        {
            CreateFirstRoot();

            Object.DestroyImmediate(firstObject);
            firstObject = null;

            Assert.That(
                EchoLaunchRoot.Current,
                Is.Null);
        }

        [Test]
        public void ResetClearsCurrentAuthority()
        {
            EchoLaunchRoot firstRoot = CreateFirstRoot();

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(firstRoot));

            LaunchAuthorityClaim.Reset();

            Assert.That(
                EchoLaunchRoot.Current,
                Is.Null);

            Assert.That(
                firstRoot.IsAuthoritative,
                Is.False);
        }

        [Test]
        public void FreshRootCanClaimAfterReset()
        {
            EchoLaunchRoot firstRoot = CreateFirstRoot();

            LaunchAuthorityClaim.Reset();

            EchoLaunchRoot secondRoot = CreateSecondRoot();

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(secondRoot));

            Assert.That(
                firstRoot.IsAuthoritative,
                Is.False);

            Assert.That(
                secondRoot.IsAuthoritative,
                Is.True);

            Assert.That(
                secondRoot.WasRejectedAsDuplicate,
                Is.False);
        }

        [UnityTest]
        public IEnumerator DestroyedAuthorityAllowsFreshRootToClaim()
        {
            CreateFirstRoot();

            Object.Destroy(firstObject);
            firstObject = null;

            yield return null;

            Assert.That(
                EchoLaunchRoot.Current,
                Is.Null);

            EchoLaunchRoot secondRoot = CreateSecondRoot();

            Assert.That(
                EchoLaunchRoot.Current,
                Is.SameAs(secondRoot));

            Assert.That(
                secondRoot.IsAuthoritative,
                Is.True);
        }

        private EchoLaunchRoot CreateFirstRoot()
        {
            firstObject =
                new GameObject("First EchoLaunchRoot");

            return firstObject.AddComponent<EchoLaunchRoot>();
        }

        private EchoLaunchRoot CreateSecondRoot()
        {
            secondObject =
                new GameObject("Second EchoLaunchRoot");

            return secondObject.AddComponent<EchoLaunchRoot>();
        }

        private static void ExpectDuplicateWarning()
        {
            LogAssert.Expect(
                LogType.Warning,
                "[ELAUNCH-ROOT-001] " +
                "Duplicate EchoLaunchRoot rejected. " +
                "The first valid root remains authoritative.");
        }

        private static void DestroyImmediately(
            GameObject target)
        {
            if (target != null)
            {
                Object.DestroyImmediate(target);
            }
        }
    }
}

//----- EchoLaunchRootAuthorityTests.cs END -----