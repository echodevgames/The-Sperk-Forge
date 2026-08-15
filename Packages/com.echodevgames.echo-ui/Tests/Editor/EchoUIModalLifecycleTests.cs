using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.EchoUI.Tests.Editor
{
    public sealed class EchoUIModalLifecycleTests
    {
        private GameObject rootObject;
        private EchoUIRoot root;
        private EchoUIRoot previousActiveRoot;
        private readonly List<GameObject> externalObjects =
            new List<GameObject>();

        private static readonly FieldInfo ActiveRootField =
            typeof(EchoUIRoot).GetField(
                "active",
                BindingFlags.Static |
                BindingFlags.NonPublic);

        private static readonly MethodInfo TryClaimAuthorityMethod =
            typeof(EchoUIRoot).GetMethod(
                "TryClaimAuthority",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        private static readonly MethodInfo RootOnDestroyMethod =
            typeof(EchoUIRoot).GetMethod(
                "OnDestroy",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        [SetUp]
        public void SetUp()
        {
            previousActiveRoot =
                EchoUIRoot.Active;

            SetActiveRootForTest(
                null);

            rootObject =
                new GameObject(
                    "Canvas_MasterCanvas");

            root =
                rootObject.AddComponent<EchoUIRoot>();

            ClaimAuthorityForTest(
                root);
        }

        [TearDown]
        public void TearDown()
        {
            if (rootObject != null)
            {
                UnityEngine.Object.DestroyImmediate(
                    rootObject);
            }

            for (int index = 0;
                 index < externalObjects.Count;
                 index++)
            {
                if (externalObjects[index] != null)
                {
                    UnityEngine.Object.DestroyImmediate(
                        externalObjects[index]);
                }
            }

            externalObjects.Clear();

            SetActiveRootForTest(
                previousActiveRoot);
        }

        [Test]
        public void ModalAndResultIdsAreStableProjectDefinedValues()
        {
            UIModalId first =
                new UIModalId(
                    " delete-character ");

            UIModalId second =
                new UIModalId(
                    "delete-character");

            UIModalResultId result =
                new UIModalResultId(
                    " confirm ");

            Assert.That(
                first.IsValid,
                Is.True);

            Assert.That(
                first,
                Is.EqualTo(second));

            Assert.That(
                first.Value,
                Is.EqualTo("delete-character"));

            Assert.That(
                result.Value,
                Is.EqualTo("confirm"));

            Assert.That(
                new UIModalId(" ").IsValid,
                Is.False);

            Assert.That(
                new UIModalResultId(null).IsValid,
                Is.False);
        }

        [Test]
        public void OpenOneBlockingModalCreatesActiveTopEntry()
        {
            UISurface modal =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Confirm",
                    "confirm",
                    UISurfaceRole.Modal,
                    string.Empty,
                    false);

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            Assert.That(
                handle.Accepted,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.False);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(1));

            Assert.That(
                root.TopModalId,
                Is.EqualTo("confirm"));

            Assert.That(
                modal.IsVisible,
                Is.True);
        }

        [Test]
        public void BlockingModalDisablesLowerLookingGlassInteractionAndRaycasts()
        {
            UISurface modal =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Confirm",
                    "confirm",
                    UISurfaceRole.Modal,
                    string.Empty,
                    false);

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            CanvasGroup mainGroup =
                fixture.Main.GetComponent<CanvasGroup>();

            CanvasGroup modalGroup =
                modal.GetComponent<CanvasGroup>();

            Assert.That(
                mainGroup.interactable,
                Is.True);

            Assert.That(
                mainGroup.blocksRaycasts,
                Is.True);

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            Assert.That(
                fixture.Main.IsInteractable,
                Is.False);

            Assert.That(
                mainGroup.blocksRaycasts,
                Is.False);

            Assert.That(
                modal.IsInteractable,
                Is.True);

            Assert.That(
                modalGroup.blocksRaycasts,
                Is.True);

            Assert.That(
                root.CompleteModal(
                    handle,
                    "confirm").Succeeded,
                Is.True);

            Assert.That(
                fixture.Main.IsInteractable,
                Is.True);

            Assert.That(
                mainGroup.blocksRaycasts,
                Is.True);
        }

        [Test]
        public void NestedModalsAllowOnlyTopModalInteraction()
        {
            UISurface lower =
                CreateModalSurface(
                    "lower");

            UISurface upper =
                CreateModalSurface(
                    "upper");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "lower",
                    lower),
                SceneModalDefinition(
                    "upper",
                    upper));

            UIModalHandle lowerHandle =
                root.OpenModal(
                    "lower");

            UIModalHandle upperHandle =
                root.OpenModal(
                    "upper");

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(2));

            Assert.That(
                lower.IsVisible,
                Is.True);

            Assert.That(
                lower.IsInteractable,
                Is.False);

            Assert.That(
                upper.IsInteractable,
                Is.True);

            Assert.That(
                root.CompleteModal(
                    upperHandle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                lowerHandle.IsCompleted,
                Is.False);

            Assert.That(
                lower.IsInteractable,
                Is.True);
        }

        [Test]
        public void SceneOwnedModalNeverDestroysSceneView()
        {
            UISurface modal =
                CreateModalSurface(
                    "scene-modal");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "scene-modal",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "scene-modal");

            Assert.That(
                root.CompleteModal(
                    handle,
                    "close").Succeeded,
                Is.True);

            Assert.That(
                modal,
                Is.Not.Null);

            Assert.That(
                modal.gameObject,
                Is.Not.Null);

            Assert.That(
                modal.IsVisible,
                Is.False);
        }

        [Test]
        public void ExternalOwnedModalNeverDestroysExternalView()
        {
            BaseFixture fixture =
                InitializeBase();

            UIModalDefinition definition =
                new UIModalDefinition(
                    "external-modal",
                    fixture.Layer.LayerId.Value,
                    UIScreenOwnershipMode.ExternalOwned);

            AssertModalInitialization(
                fixture.Layer,
                definition);

            UISurface external =
                CreateExternalModalSurface(
                    "external-modal");

            Assert.That(
                root.RegisterExternalModalView(
                    "external-modal",
                    external).Succeeded,
                Is.True);

            UIModalHandle handle =
                root.OpenModal(
                    "external-modal");

            Assert.That(
                root.CompleteModal(
                    handle,
                    "close").Succeeded,
                Is.True);

            Assert.That(
                external,
                Is.Not.Null);

            Assert.That(
                external.gameObject,
                Is.Not.Null);

            Assert.That(
                external.IsVisible,
                Is.False);
        }

        [Test]
        public void RootOwnedModalCreatesAndReleasesOwnedView()
        {
            BaseFixture fixture =
                InitializeBase();

            GameObject prefab =
                CreateModalPrefab(
                    "root-modal");

            UIModalDefinition definition =
                new UIModalDefinition(
                    "root-modal",
                    fixture.Layer.LayerId.Value,
                    UIScreenOwnershipMode.RootOwned,
                    rootOwnedPrefab: prefab);

            AssertModalInitialization(
                fixture.Layer,
                definition);

            int before =
                root.RegisteredSurfaceCount;

            UIModalHandle handle =
                root.OpenModal(
                    "root-modal");

            Assert.That(
                handle.Accepted,
                Is.True);

            Assert.That(
                fixture.Layer.ContentRoot.childCount,
                Is.EqualTo(1));

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(before + 1));

            Assert.That(
                root.CompleteModal(
                    handle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                fixture.Layer.ContentRoot.childCount,
                Is.EqualTo(0));

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(before));
        }

        [Test]
        public void ActiveCapacityOverflowRejectsBeforePartialMutation()
        {
            UISurface first =
                CreateModalSurface(
                    "first");

            UISurface second =
                CreateModalSurface(
                    "second");

            BaseFixture fixture =
                InitializeBase();

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        SceneModalDefinition(
                            "first",
                            first),
                        SceneModalDefinition(
                            "second",
                            second)
                    },
                    activeCapacity: 1).Succeeded,
                Is.True);

            UIModalHandle accepted =
                root.OpenModal(
                    "first");

            UIModalHandle rejected =
                root.OpenModal(
                    "second");

            Assert.That(
                accepted.Accepted,
                Is.True);

            Assert.That(
                rejected.Accepted,
                Is.False);

            Assert.That(
                rejected.IsCompleted,
                Is.True);

            Assert.That(
                rejected.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Rejected));

            Assert.That(
                second.IsVisible,
                Is.False);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(1));
        }

        [Test]
        public void SemanticCompletionReportsExactProjectDefinedResultId()
        {
            UISurface modal =
                CreateModalSurface(
                    "difficulty");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "difficulty",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "difficulty");

            Assert.That(
                root.CompleteModal(
                    handle,
                    "hard").Succeeded,
                Is.True);

            Assert.That(
                handle.IsCompleted,
                Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Completed));

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("hard"));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.None));

            Assert.That(
                handle.Completion.IsCompleted,
                Is.True);
        }

        [Test]
        public void RepeatedCompletionIsHarmlessAndFirstResultWinsExactlyOnce()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            int completionCount = 0;

            handle.Completed +=
                _ => completionCount++;

            UIModalCompletionAttemptResult first =
                root.CompleteModal(
                    handle,
                    "yes");

            UIModalCompletionAttemptResult second =
                root.CompleteModal(
                    handle,
                    "no");

            Assert.That(
                first.Succeeded,
                Is.True);

            Assert.That(
                second.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("yes"));

            Assert.That(
                completionCount,
                Is.EqualTo(1));
        }

        [Test]
        public void CompletionCallbackCannotRaceSecondTerminalResultIntoSameGeneration()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            UIModalCompletionAttemptResult competing =
                default;

            handle.Completed +=
                _ =>
                    competing =
                        root.CompleteModal(
                            handle,
                            "second");

            UIModalCompletionAttemptResult first =
                root.CompleteModal(
                    handle,
                    "first");

            Assert.That(
                first.Succeeded,
                Is.True);

            Assert.That(
                competing.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("first"));

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(0));
        }

        [Test]
        public void StructuralAbortIsDistinctFromSemanticCancel()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            Assert.That(
                root.AbortModal(
                    handle,
                    UIModalAbortReason.ExplicitAbort).Succeeded,
                Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.ExplicitAbort));

            Assert.That(
                handle.Result.ResultId.IsValid,
                Is.False);
        }

        [Test]
        public void LowerModalMayAbortOutOfOrderWithoutStealingTopInteraction()
        {
            UISurface lower =
                CreateModalSurface(
                    "lower");

            UISurface upper =
                CreateModalSurface(
                    "upper");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "lower",
                    lower),
                SceneModalDefinition(
                    "upper",
                    upper));

            UIModalHandle lowerHandle =
                root.OpenModal(
                    "lower");

            UIModalHandle upperHandle =
                root.OpenModal(
                    "upper");

            Assert.That(
                root.AbortModal(
                    lowerHandle,
                    UIModalAbortReason.OwnerLost).Succeeded,
                Is.True);

            Assert.That(
                lowerHandle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                upperHandle.IsCompleted,
                Is.False);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(1));

            Assert.That(
                root.TopModalId,
                Is.EqualTo("upper"));

            Assert.That(
                upper.IsInteractable,
                Is.True);
        }

        [Test]
        public void ReopeningModalCreatesFreshHandleAndGeneration()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            UIModalHandle first =
                root.OpenModal(
                    "confirm");

            Assert.That(
                root.CompleteModal(
                    first,
                    "done").Succeeded,
                Is.True);

            UIModalHandle second =
                root.OpenModal(
                    "confirm");

            Assert.That(
                second.Accepted,
                Is.True);

            Assert.That(
                second.Generation,
                Is.GreaterThan(
                    first.Generation));

            Assert.That(
                second,
                Is.Not.SameAs(first));

            Assert.That(
                second.Completion,
                Is.Not.SameAs(first.Completion));
        }

        [Test]
        public void StaleCompletedHandleCannotCompleteLaterReopening()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            UIModalHandle first =
                root.OpenModal(
                    "confirm");

            Assert.That(
                root.CompleteModal(
                    first,
                    "first").Succeeded,
                Is.True);

            UIModalHandle second =
                root.OpenModal(
                    "confirm");

            UIModalCompletionAttemptResult stale =
                root.CompleteModal(
                    first,
                    "stale");

            Assert.That(
                stale.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.AlreadyCompleted));

            Assert.That(
                second.IsCompleted,
                Is.False);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(1));
        }

        [Test]
        public void BackDisabledLeavesTopModalActive()
        {
            UISurface modal =
                CreateModalSurface(
                    "required-choice");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "required-choice",
                    modal,
                    new UIModalBackPolicy(
                        UIModalBackBehavior.Disabled)));

            UIModalHandle handle =
                root.OpenModal(
                    "required-choice");

            UIModalCompletionAttemptResult result =
                root.HandleModalBack();

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UIModalCompletionStatus.BackDisabled));

            Assert.That(
                handle.IsCompleted,
                Is.False);

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(1));
        }

        [Test]
        public void BackMayCompleteTopModalUsingConfiguredStableResultId()
        {
            UISurface modal =
                CreateModalSurface(
                    "dismissible");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "dismissible",
                    modal,
                    new UIModalBackPolicy(
                        UIModalBackBehavior.CompleteWithResultId,
                        "cancel")));

            UIModalHandle handle =
                root.OpenModal(
                    "dismissible");

            UIModalCompletionAttemptResult result =
                root.HandleModalBack();

            Assert.That(
                result.Succeeded,
                Is.True);

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("cancel"));
        }

        [Test]
        public void HighLevelBackRoutesToModalBeforeScreenHistory()
        {
            UISurface modal =
                CreateModalSurface(
                    "dismissible");

            BaseFixture fixture =
                InitializeBase(
                    includeSettings: true);

            Assert.That(
                root.PushScreen(
                    "settings").Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "dismissible",
                    modal,
                    new UIModalBackPolicy(
                        UIModalBackBehavior.CompleteWithResultId,
                        "back")));

            UIModalHandle handle =
                root.OpenModal(
                    "dismissible");

            int depth =
                root.GetScreenHistoryDepth(
                    "frontend");

            UISurfaceOperationResult back =
                root.Back(
                    "frontend");

            Assert.That(
                back.Succeeded,
                Is.True);

            Assert.That(
                handle.Result.ResultId.Value,
                Is.EqualTo("back"));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(depth));
        }

        [Test]
        public void RejectPolicyBlocksScreenMutationWithoutHistoryChange()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase(
                    includeSettings: true);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        SceneModalDefinition(
                            "confirm",
                            modal)
                    },
                    screenMutationPolicy:
                        UIModalScreenMutationPolicy.Reject).Succeeded,
                Is.True);

            UIModalHandle modalHandle =
                root.OpenModal(
                    "confirm");

            UIScreenHandle screen =
                root.PushScreen(
                    "settings");

            Assert.That(
                screen.Accepted,
                Is.False);

            Assert.That(
                screen.IsCompleted,
                Is.True);

            Assert.That(
                screen.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.BlockedByModal));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(1));

            Assert.That(
                modalHandle.IsCompleted,
                Is.False);
        }

        [Test]
        public void DeferPolicyExecutesScreenMutationAfterModalStackClears()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase(
                    includeSettings: true);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        SceneModalDefinition(
                            "confirm",
                            modal)
                    },
                    screenMutationPolicy:
                        UIModalScreenMutationPolicy.DeferUntilModalStackClears).Succeeded,
                Is.True);

            UIModalHandle modalHandle =
                root.OpenModal(
                    "confirm");

            UIScreenHandle deferred =
                root.PushScreen(
                    "settings");

            Assert.That(
                deferred.Accepted,
                Is.True);

            Assert.That(
                deferred.IsCompleted,
                Is.False);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));

            Assert.That(
                root.CompleteModal(
                    modalHandle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                deferred.IsCompleted,
                Is.True);

            Assert.That(
                deferred.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
        }

        [Test]
        public void LegacySynchronousNavigationDoesNotHideDeferredMutationBehindSurfaceResult()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase(
                    includeSettings: true);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        SceneModalDefinition(
                            "confirm",
                            modal)
                    },
                    screenMutationPolicy:
                        UIModalScreenMutationPolicy.DeferUntilModalStackClears).Succeeded,
                Is.True);

            UIModalHandle modalHandle =
                root.OpenModal(
                    "confirm");

            UISurfaceOperationResult result =
                root.NavigateTo(
                    "settings");

            Assert.That(
                result.Status,
                Is.EqualTo(
                    UISurfaceOperationStatus.BlockedByModal));

            Assert.That(
                root.DeferredScreenOperationQueueDepth,
                Is.EqualTo(0));

            Assert.That(
                root.CompleteModal(
                    modalHandle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("main-menu"));
        }

        [Test]
        public void MultipleDeferredScreenMutationsPreserveOriginalFifoOrder()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase(
                    includeSettings: true,
                    includeCredits: true);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        SceneModalDefinition(
                            "confirm",
                            modal)
                    },
                    screenMutationPolicy:
                        UIModalScreenMutationPolicy.DeferUntilModalStackClears).Succeeded,
                Is.True);

            UIModalHandle modalHandle =
                root.OpenModal(
                    "confirm");

            UIScreenHandle first =
                root.PushScreen(
                    "settings");

            UIScreenHandle second =
                root.ReplaceScreen(
                    "credits");

            Assert.That(
                first.IsCompleted,
                Is.False);

            Assert.That(
                second.IsCompleted,
                Is.False);

            Assert.That(
                root.CompleteModal(
                    modalHandle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                first.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                second.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                first.Request.Sequence,
                Is.LessThan(
                    second.Request.Sequence));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("credits"));

            Assert.That(
                root.GetScreenHistoryDepth("frontend"),
                Is.EqualTo(2));
        }

        [Test]
        public void DeferredCapacityOverflowIsExplicitAndDoesNotDisturbAcceptedRequest()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            BaseFixture fixture =
                InitializeBase(
                    includeSettings: true,
                    includeCredits: true);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        SceneModalDefinition(
                            "confirm",
                            modal)
                    },
                    screenMutationPolicy:
                        UIModalScreenMutationPolicy.DeferUntilModalStackClears,
                    deferredCapacity: 1).Succeeded,
                Is.True);

            UIModalHandle modalHandle =
                root.OpenModal(
                    "confirm");

            UIScreenHandle accepted =
                root.PushScreen(
                    "settings");

            UIScreenHandle rejected =
                root.ReplaceScreen(
                    "credits");

            Assert.That(
                accepted.Accepted,
                Is.True);

            Assert.That(
                accepted.IsCompleted,
                Is.False);

            Assert.That(
                rejected.Accepted,
                Is.False);

            Assert.That(
                rejected.IsCompleted,
                Is.True);

            Assert.That(
                root.DeferredScreenOperationQueueDepth,
                Is.EqualTo(1));

            Assert.That(
                root.CompleteModal(
                    modalHandle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                accepted.Result.Status,
                Is.EqualTo(
                    UIScreenOperationStatus.Succeeded));

            Assert.That(
                root.GetCurrentScreenId("frontend"),
                Is.EqualTo("settings"));
        }

        [Test]
        public void UnregisteringActiveExternalOwnerAbortsWithoutDestroyingView()
        {
            BaseFixture fixture =
                InitializeBase();

            UIModalDefinition definition =
                new UIModalDefinition(
                    "external-modal",
                    fixture.Layer.LayerId.Value,
                    UIScreenOwnershipMode.ExternalOwned);

            AssertModalInitialization(
                fixture.Layer,
                definition);

            UISurface external =
                CreateExternalModalSurface(
                    "external-modal");

            Assert.That(
                root.RegisterExternalModalView(
                    "external-modal",
                    external).Succeeded,
                Is.True);

            UIModalHandle handle =
                root.OpenModal(
                    "external-modal");

            Assert.That(
                root.UnregisterExternalModalView(
                    "external-modal").Succeeded,
                Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.OwnerLost));

            Assert.That(
                external,
                Is.Not.Null);

            Assert.That(
                external.gameObject,
                Is.Not.Null);
        }

        [Test]
        public void LostSceneViewAbortsGenerationAndClearsBlocking()
        {
            UISurface modal =
                CreateModalSurface(
                    "scene-modal");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "scene-modal",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "scene-modal");

            UnityEngine.Object.DestroyImmediate(
                modal.gameObject);

            root.RefreshModalLifecycle();

            Assert.That(
                handle.IsCompleted,
                Is.True);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.ViewLost));

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(0));

            Assert.That(
                root.RegisteredSurfaceCount,
                Is.EqualTo(1));

            Assert.That(
                fixture.Main.IsInteractable,
                Is.True);
        }

        [Test]
        public void RootShutdownAbortsAdmittedModalExactlyOnce()
        {
            UISurface modal =
                CreateModalSurface(
                    "scene-modal");

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "scene-modal",
                    modal));

            UIModalHandle handle =
                root.OpenModal(
                    "scene-modal");

            Assert.That(
                RootOnDestroyMethod,
                Is.Not.Null);

            RootOnDestroyMethod.Invoke(
                root,
                null);

            Assert.That(
                handle.IsCompleted,
                Is.True);

            UnityEngine.Object.DestroyImmediate(
                rootObject);

            rootObject = null;
            root = null;

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Aborted));

            Assert.That(
                handle.Result.AbortReason,
                Is.EqualTo(
                    UIModalAbortReason.RootShutdown));
        }

        [Test]
        public void FactoryFailureLeavesNoLiveEntryOrBlockingResidue()
        {
            BaseFixture fixture =
                InitializeBase();

            GameObject prefab =
                CreateModalPrefab(
                    "failing");

            UIModalDefinition definition =
                new UIModalDefinition(
                    "failing",
                    fixture.Layer.LayerId.Value,
                    UIScreenOwnershipMode.RootOwned,
                    rootOwnedPrefab: prefab);

            Assert.That(
                root.InitializeModalLifecycle(
                    new[]
                    {
                        definition
                    },
                    new AlwaysFailModalFactory()).Succeeded,
                Is.True);

            UIModalHandle handle =
                root.OpenModal(
                    "failing");

            Assert.That(
                handle.Accepted,
                Is.False);

            Assert.That(
                handle.Result.Outcome,
                Is.EqualTo(
                    UIModalOutcome.Rejected));

            Assert.That(
                root.ActiveModalCount,
                Is.EqualTo(0));

            Assert.That(
                fixture.Main.IsInteractable,
                Is.True);
        }

        [Test]
        public void ModalSettlementRestoresIndependentWindowInteraction()
        {
            UISurface modal =
                CreateModalSurface(
                    "confirm");

            UISurface window =
                CreateSurface(
                    rootObject.transform,
                    "Panel_Inventory",
                    "inventory",
                    UISurfaceRole.Window,
                    string.Empty,
                    true);

            BaseFixture fixture =
                InitializeBase();

            AssertModalInitialization(
                fixture.Layer,
                SceneModalDefinition(
                    "confirm",
                    modal));

            Assert.That(
                window.IsInteractable,
                Is.True);

            UIModalHandle handle =
                root.OpenModal(
                    "confirm");

            Assert.That(
                window.IsVisible,
                Is.True);

            Assert.That(
                window.IsInteractable,
                Is.False);

            Assert.That(
                root.CompleteModal(
                    handle,
                    "done").Succeeded,
                Is.True);

            Assert.That(
                window.IsVisible,
                Is.True);

            Assert.That(
                window.IsInteractable,
                Is.True);
        }

        private BaseFixture InitializeBase(
            bool includeSettings = false,
            bool includeCredits = false)
        {
            UILayerHost layer =
                CreateLayer(
                    "primary-ui",
                    20);

            UISurface main =
                CreateSurface(
                    rootObject.transform,
                    "Panel_MainMenu",
                    "main-menu",
                    UISurfaceRole.Screen,
                    "frontend",
                    true);

            UISurface settings =
                includeSettings
                    ? CreateSurface(
                        rootObject.transform,
                        "Panel_Settings",
                        "settings",
                        UISurfaceRole.Screen,
                        "frontend",
                        false)
                    : null;

            UISurface credits =
                includeCredits
                    ? CreateSurface(
                        rootObject.transform,
                        "Panel_Credits",
                        "credits",
                        UISurfaceRole.Screen,
                        "frontend",
                        false)
                    : null;

            Assert.That(
                root.Initialize().Succeeded,
                Is.True);

            List<UIScreenDefinition> definitions =
                new List<UIScreenDefinition>
                {
                    SceneScreenDefinition(
                        "main-menu",
                        main)
                };

            if (settings != null)
            {
                definitions.Add(
                    SceneScreenDefinition(
                        "settings",
                        settings));
            }

            if (credits != null)
            {
                definitions.Add(
                    SceneScreenDefinition(
                        "credits",
                        credits));
            }

            Assert.That(
                root.InitializeScreenLifecycle(
                    new[]
                    {
                        layer
                    },
                    definitions).Succeeded,
                Is.True);

            return new BaseFixture(
                layer,
                main,
                settings,
                credits);
        }

        private void AssertModalInitialization(
            UILayerHost layer,
            params UIModalDefinition[] definitions)
        {
            Assert.That(
                layer,
                Is.Not.Null);

            UISurfaceOperationResult result =
                root.InitializeModalLifecycle(
                    definitions);

            Assert.That(
                result.Succeeded,
                Is.True,
                result.Message);

            Assert.That(
                root.IsModalLifecycleInitialized,
                Is.True);
        }

        private UISurface CreateModalSurface(
            string modalId) =>
            CreateSurface(
                rootObject.transform,
                "Panel_" + modalId,
                modalId,
                UISurfaceRole.Modal,
                string.Empty,
                false);

        private UISurface CreateExternalModalSurface(
            string modalId)
        {
            GameObject external =
                new GameObject(
                    "External_" + modalId);

            externalObjects.Add(
                external);

            external.AddComponent<CanvasGroup>();

            return ConfigureSurface(
                external.AddComponent<UISurface>(),
                modalId,
                UISurfaceRole.Modal,
                string.Empty,
                false);
        }

        private GameObject CreateModalPrefab(
            string modalId)
        {
            GameObject prefab =
                new GameObject(
                    "PF_" + modalId);

            externalObjects.Add(
                prefab);

            prefab.AddComponent<CanvasGroup>();

            ConfigureSurface(
                prefab.AddComponent<UISurface>(),
                modalId,
                UISurfaceRole.Modal,
                string.Empty,
                false);

            return prefab;
        }

        private UISurface CreateSurface(
            Transform parent,
            string objectName,
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            GameObject child =
                new GameObject(
                    objectName);

            child.transform.SetParent(
                parent,
                false);

            child.AddComponent<CanvasGroup>();

            return ConfigureSurface(
                child.AddComponent<UISurface>(),
                surfaceId,
                role,
                scopeId,
                startVisible);
        }

        private UILayerHost CreateLayer(
            string id,
            int order)
        {
            GameObject layerObject =
                new GameObject(
                    "Layer_" + id);

            layerObject.transform.SetParent(
                rootObject.transform,
                false);

            UILayerHost host =
                layerObject.AddComponent<UILayerHost>();

            SerializedObject serialized =
                new SerializedObject(
                    host);

            SerializedProperty definition =
                serialized.FindProperty(
                    "definition");

            definition.FindPropertyRelative(
                    "layerId")
                .stringValue = id;

            definition.FindPropertyRelative(
                    "displayLabel")
                .stringValue = id;

            definition.FindPropertyRelative(
                    "order")
                .intValue = order;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return host;
        }

        private static UISurface ConfigureSurface(
            UISurface surface,
            string surfaceId,
            UISurfaceRole role,
            string scopeId,
            bool startVisible)
        {
            SerializedObject serialized =
                new SerializedObject(
                    surface);

            serialized.FindProperty(
                    "surfaceId")
                .stringValue = surfaceId;

            serialized.FindProperty(
                    "displayLabel")
                .stringValue = surface.gameObject.name;

            serialized.FindProperty(
                    "role")
                .enumValueIndex = (int)role;

            serialized.FindProperty(
                    "navigationScopeId")
                .stringValue = scopeId;

            serialized.FindProperty(
                    "startVisible")
                .boolValue = startVisible;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return surface;
        }

        private UIModalDefinition SceneModalDefinition(
            string modalId,
            UISurface view,
            UIModalBackPolicy backPolicy = null) =>
            new UIModalDefinition(
                modalId,
                "primary-ui",
                UIScreenOwnershipMode.SceneOwned,
                sceneOwnedView: view,
                backPolicy: backPolicy);

        private static UIScreenDefinition SceneScreenDefinition(
            string screenId,
            UISurface view) =>
            new UIScreenDefinition(
                screenId,
                "frontend",
                "primary-ui",
                UIScreenOwnershipMode.SceneOwned,
                UIScreenSuspensionVisibility.Hidden,
                sceneOwnedView: view);

        private static void SetActiveRootForTest(
            EchoUIRoot value)
        {
            Assert.That(
                ActiveRootField,
                Is.Not.Null);

            ActiveRootField.SetValue(
                null,
                value);
        }

        private static void ClaimAuthorityForTest(
            EchoUIRoot value)
        {
            Assert.That(
                TryClaimAuthorityMethod,
                Is.Not.Null);

            TryClaimAuthorityMethod.Invoke(
                value,
                null);

            Assert.That(
                value.IsAuthoritative,
                Is.True);
        }

        private sealed class AlwaysFailModalFactory :
            IUIModalFactory
        {
            public bool TryCreate(
                UIModalDefinition definition,
                UILayerHost layerHost,
                out UISurface surface,
                out string error)
            {
                surface = null;
                error = "Injected Modal factory failure.";
                return false;
            }

            public void Release(
                UISurface surface)
            {
            }
        }

        private readonly struct BaseFixture
        {
            public BaseFixture(
                UILayerHost layer,
                UISurface main,
                UISurface settings,
                UISurface credits)
            {
                Layer = layer;
                Main = main;
                Settings = settings;
                Credits = credits;
            }

            public UILayerHost Layer { get; }

            public UISurface Main { get; }

            public UISurface Settings { get; }

            public UISurface Credits { get; }
        }
    }
}
