
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace EchoDevGames.EchoSave.Samples.ChronicleLaboratory
{
    /// <summary>
    /// M5-06 sample-owned engineering control panel.
    ///
    /// This is intentionally not production save-menu architecture.
    /// It exists only to make real Chronicle operations human-visible.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EchoSaveRoot))]
    public sealed class ChronicleSaveLaboratoryHarness :
        MonoBehaviour
    {
        private const string ProjectId =
            "echodevgames.chronicle-laboratory";

        private const string ProjectVersion =
            "m5-06";

        private const string BuildId =
            "laboratory";

        private const string OwnershipMarkerFile =
            "m506-laboratory-owned.txt";

        private const string OwnershipMarkerValue =
            "ECHOSAVE-M5-06-LABORATORY";

        private EchoSaveRoot root;
        private IEchoSaveService service;
        private ChronicleSaveLaboratoryState state;
        private ChronicleSaveLaboratoryState lastSavedVisualState;
        private ChronicleSaveLaboratoryParticipant participant;
        private SaveParticipantRegistration registration;
        private PreparedSaveLoad preparedLoad;
        private SaveDeletionPlan pendingDeletionPlan;

        private SaveSlotId selectedSlotId;
        private SaveGenerationId displayedGenerationId;

        private Vector2 scroll;
        private string evidence =
            "BOOTING THE CHRONICLE REACTOR...";
        private bool busy;
        private int slotSerial = 1;

        private async Awaitable Start()
        {
            root =
                GetComponent<EchoSaveRoot>();

            state =
                ChronicleSaveLaboratoryState
                    .CreateKnownBaseline();

            participant =
                new ChronicleSaveLaboratoryParticipant(
                    state);

            await InitializeLaboratoryAsync();
        }

        private void OnDestroy()
        {
            preparedLoad?.Dispose();
            preparedLoad = null;

            registration?.Dispose();
            registration = null;
        }

        private void OnGUI()
        {
            float width =
                Mathf.Min(
                    Mathf.Max(
                        760f,
                        Screen.width - 40f),
                    1120f);

            float height =
                Mathf.Max(
                    420f,
                    Screen.height - 40f);

            GUILayout.BeginArea(
                new Rect(
                    20f,
                    20f,
                    width,
                    height),
                GUI.skin.box);

            scroll =
                GUILayout.BeginScrollView(
                    scroll);

            GUILayout.Label(
                "THE CHRONICLE — SPERK REACTOR CERTIFICATION CONSOLE",
                LargeLabel());

            GUILayout.Label(
                "Ugly reactor panel. Tiny Sperk. Real Chronicle.");

            DrawAuthority();
            DrawSubject();
            DrawNormalOperations();
            DrawCatalog();
            DrawPreparedLoad();
            DrawReset();
            DrawEvidence();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawAuthority()
        {
            Section(
                "REACTOR / AUTHORITY");

            GUILayout.Label(
                "Service: " +
                (root == null
                    ? "NO ROOT"
                    : root.State.ToString()));

            GUILayout.Label(
                "Authoritative Root: " +
                YesNo(
                    root != null &&
                    root.IsAuthoritative));

            GUILayout.Label(
                "Selected Slot: " +
                ValueOrNone(
                    selectedSlotId.Value));

            GUILayout.Label(
                "Displayed Generation: " +
                ValueOrNone(
                    displayedGenerationId.Value));

            GUILayout.Label(
                "Busy: " +
                YesNo(
                    busy));

            if (Button(
                    "REFRESH CATALOG"))
            {
                RefreshCatalog();
            }
        }

        private void DrawSubject()
        {
            Section(
                "SUBJECT: SPERK-001");

            if (state == null)
            {
                GUILayout.Label(
                    "State not constructed.");
                return;
            }

            GUILayout.Label(
                $"Sperk Level: {state.sperkLevel}");

            GUILayout.Label(
                $"Galactic Rupees: {state.galacticRupees}");

            GUILayout.Label(
                $"Anvil Temperature: {state.anvilTemperature}");

            GUILayout.Label(
                "Has Forbidden Key: " +
                YesNo(
                    state.hasForbiddenKey));

            GUILayout.Label(
                $"Reality Damage: {state.realityDamagePercent}%");

            GUILayout.BeginHorizontal();

            if (Button(
                    "+100 GALACTIC RUPEES"))
            {
                state.galacticRupees += 100;
                evidence =
                    "SUBJECT MUTATED: Galactic Rupees increased. Nothing was saved.";
            }

            if (Button(
                    "INCREASE SPERK LEVEL"))
            {
                state.sperkLevel++;
                evidence =
                    "SUBJECT MUTATED: Sperk Level increased. Nothing was saved.";
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (Button(
                    "HEAT THE ANVIL"))
            {
                state.anvilTemperature += 250;
                evidence =
                    "SUBJECT MUTATED: Anvil temperature increased. Nothing was saved.";
            }

            if (Button(
                    "TOGGLE FORBIDDEN KEY"))
            {
                state.hasForbiddenKey =
                    !state.hasForbiddenKey;

                evidence =
                    "SUBJECT MUTATED: Forbidden Key toggled. Nothing was saved.";
            }

            if (Button(
                    "DAMAGE REALITY"))
            {
                state.realityDamagePercent =
                    Mathf.Clamp(
                        state.realityDamagePercent + 7,
                        0,
                        100);

                evidence =
                    "SUBJECT MUTATED: Reality damaged. The paperwork is presumably catastrophic.";
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (Button(
                    "RESET SUBJECT VALUES"))
            {
                state.CopyFrom(
                    ChronicleSaveLaboratoryState
                        .CreateKnownBaseline());

                evidence =
                    "SUBJECT RESET: in-memory values returned to the known baseline. No save occurred.";
            }

            if (Button(
                    "MUTATE VALUES WITHOUT SAVING"))
            {
                state.sperkLevel += 13;
                state.galacticRupees += 777;
                state.anvilTemperature += 111;
                state.hasForbiddenKey =
                    !state.hasForbiddenKey;

                state.realityDamagePercent =
                    Mathf.Clamp(
                        state.realityDamagePercent + 23,
                        0,
                        100);

                evidence =
                    "UNSAVED MUTATION COMPLETE. Press LOAD to prove whether The Chronicle remembers.";
            }

            GUILayout.EndHorizontal();
        }

        private void DrawNormalOperations()
        {
            Section(
                "NORMAL OPERATIONS");

            GUILayout.BeginHorizontal();

            if (Button(
                    "CREATE SLOT"))
            {
                CreateSlot();
            }

            if (Button(
                    "SAVE"))
            {
                Save();
            }

            if (Button(
                    "LOAD & APPLY"))
            {
                LoadAndApply();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (Button(
                    "RENAME SLOT"))
            {
                RenameSelectedSlot();
            }

            if (Button(
                    "DUPLICATE SLOT"))
            {
                DuplicateSelectedSlot();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (Button(
                    "PREVIEW DELETE"))
            {
                PrepareDelete();
            }

            GUI.enabled =
                !busy &&
                pendingDeletionPlan != null &&
                pendingDeletionPlan.Succeeded;

            if (GUILayout.Button(
                    "CONFIRM DELETE"))
            {
                ConfirmDelete();
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            if (pendingDeletionPlan != null)
            {
                GUILayout.Label(
                    "Delete Plan: " +
                    pendingDeletionPlan.Status +
                    " | " +
                    pendingDeletionPlan.Message);
            }
        }

        private void DrawCatalog()
        {
            Section(
                "CATALOG / SLOT SELECTION");

            if (service == null)
            {
                GUILayout.Label(
                    "Service unavailable.");
                return;
            }

            SaveSlotCatalogSnapshot snapshot =
                service.GetCatalogSnapshot();

            GUILayout.Label(
                $"Catalog Slots: {snapshot.Count} | Healthy: {snapshot.HealthyCount} | Degraded: {snapshot.DegradedCount}");

            for (int i = 0;
                 i < snapshot.Entries.Count;
                 i++)
            {
                SaveSlotCatalogEntry entry =
                    snapshot.Entries[i];

                GUILayout.BeginHorizontal(
                    GUI.skin.box);

                GUILayout.Label(
                    $"{entry.DisplayName} | {entry.SlotId.Value} | {entry.Health} | Gen {entry.CurrentGenerationId.Value}",
                    GUILayout.ExpandWidth(true));

                GUI.enabled =
                    !busy &&
                    entry.IsSelectable;

                if (GUILayout.Button(
                        "SELECT",
                        GUILayout.Width(90f)))
                {
                    SelectSlot(
                        entry.SlotId);
                }

                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }

        private void DrawPreparedLoad()
        {
            Section(
                "PREPARED LOAD / HANDLE PROOF");

            GUILayout.Label(
                preparedLoad == null
                    ? "Prepared Handle: NONE"
                    : $"Prepared Handle: {preparedLoad.State} | Slot {preparedLoad.SourceSlotId.Value} | Gen {preparedLoad.SourceGenerationId.Value}");

            GUILayout.BeginHorizontal();

            if (Button(
                    "PREPARE LOAD"))
            {
                PrepareLoad();
            }

            GUI.enabled =
                !busy &&
                preparedLoad != null &&
                preparedLoad.IsValid;

            if (GUILayout.Button(
                    "APPLY PREPARED LOAD"))
            {
                ApplyPreparedLoad();
            }

            if (GUILayout.Button(
                    "DISPOSE PREPARED LOAD"))
            {
                preparedLoad.Dispose();
                preparedLoad = null;

                evidence =
                    "PREPARED LOAD DISPOSED. Disk state was not intentionally mutated by disposal.";
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void DrawReset()
        {
            Section(
                "LABORATORY RESET");

            GUILayout.Label(
                "This deletes only the exact M5-06 Laboratory root after shutdown and ownership-marker verification.");

            if (Button(
                    "DO NOT PRESS UNLESS REALITY IS BACKED UP — RESET LAB"))
            {
                ResetLaboratory();
            }
        }

        private void DrawEvidence()
        {
            Section(
                "EVIDENCE CONSOLE");

            GUILayout.TextArea(
                evidence ?? string.Empty,
                GUILayout.MinHeight(130f));

            if (lastSavedVisualState != null)
            {
                GUILayout.Label(
                    "Last Saved Visual Snapshot: " +
                    Describe(
                        lastSavedVisualState));
            }

            GUILayout.Label(
                "Current Subject: " +
                Describe(
                    state));
        }

        private async Awaitable InitializeLaboratoryAsync()
        {
            if (root == null)
            {
                evidence =
                    "FAIL: EchoSaveRoot component is missing.";
                return;
            }

            busy = true;

            try
            {
                EchoSaveLifecycleResult initialized =
                    await root.InitializeAsync();

                if (!initialized.Succeeded)
                {
                    evidence =
                        "FAIL INITIALIZE: " +
                        initialized.Message;
                    return;
                }

                service =
                    root.Service;

                if (service == null)
                {
                    evidence =
                        "FAIL: authoritative EchoSaveRoot did not expose a service after initialization.";
                    return;
                }

                SaveParticipantRegistrationResult
                    registrationResult =
                        service.RegisterParticipant(
                            participant);

                if (!registrationResult.Succeeded)
                {
                    evidence =
                        "FAIL PARTICIPANT REGISTRATION: " +
                        registrationResult.Message;
                    return;
                }

                registration =
                    registrationResult.Registration;

                EnsureOwnershipMarker();

                SaveSlotCatalogRefreshResult refresh =
                    await service.RefreshCatalogAsync();

                evidence =
                    refresh.Succeeded
                        ? "LAB-001 PASS: direct-scene Chronicle authority initialized. The reactor is online."
                        : "Chronicle initialized, but catalog refresh reported: " +
                          refresh.Message;
            }
            catch (Exception exception)
            {
                evidence =
                    "EXCEPTION DURING LAB INITIALIZATION: " +
                    exception;
            }
            finally
            {
                busy = false;
            }
        }

        private async void RefreshCatalog()
        {
            if (!Ready(
                    "REFRESH CATALOG"))
            {
                return;
            }

            busy = true;

            try
            {
                SaveSlotCatalogRefreshResult result =
                    await service.RefreshCatalogAsync();

                evidence =
                    $"{(result.Succeeded ? "PASS" : "FAIL")} REFRESH CATALOG: {result.Status} | Slots={result.Snapshot.Count} | Cache={result.CacheMaintenanceStatus} | {result.Message}";
            }
            finally
            {
                busy = false;
            }
        }

        private async void CreateSlot()
        {
            if (!Ready(
                    "CREATE SLOT"))
            {
                return;
            }

            busy = true;

            try
            {
                SaveSlotCreateResult result =
                    await service.CreateSlotAsync(
                        new SaveSlotCreateRequest(
                            "Sperk Reactor File " +
                            slotSerial++,
                            ProjectId,
                            ProjectVersion,
                            BuildId));

                if (!result.Succeeded)
                {
                    evidence =
                        "FAIL CREATE SLOT: " +
                        result.Status +
                        " | " +
                        result.Message;
                    return;
                }

                SaveActiveSlotSelectionResult selection =
                    service.SelectSlot(
                        result.SlotId);

                if (!selection.Succeeded)
                {
                    evidence =
                        "SLOT CREATED BUT SELECTION FAILED: " +
                        selection.Message;
                    return;
                }

                selectedSlotId =
                    result.SlotId;

                displayedGenerationId =
                    result.GenerationId;

                pendingDeletionPlan =
                    null;

                evidence =
                    "LAB-003 PASS: slot created and selected.\n" +
                    "Slot: " +
                    selectedSlotId.Value +
                    "\nInitial Generation: " +
                    displayedGenerationId.Value;
            }
            finally
            {
                busy = false;
            }
        }

        private async void Save()
        {
            if (!ReadyWithSlot(
                    "SAVE"))
            {
                return;
            }

            busy = true;

            try
            {
                SaveOperationResult result =
                    await service.SaveAsync(
                        new SaveRequest(
                            ProjectId,
                            ProjectVersion,
                            BuildId));

                if (!result.Succeeded)
                {
                    evidence =
                        "FAIL SAVE: " +
                        result.Status +
                        " | " +
                        result.Message;
                    return;
                }

                lastSavedVisualState =
                    state.Clone();

                displayedGenerationId =
                    result.PublishedGenerationId;

                evidence =
                    "PASS SAVE: verified generation published and head advanced.\n" +
                    "Generation: " +
                    result.PublishedGenerationId.Value +
                    "\nGeneration Published: " +
                    YesNo(
                        result.GenerationPublished) +
                    "\nHead Published: " +
                    YesNo(
                        result.HeadPublished) +
                    "\nCatalog Reconciled: " +
                    YesNo(
                        result.CatalogReconciled) +
                    "\nSaved Subject: " +
                    Describe(
                        lastSavedVisualState);
            }
            finally
            {
                busy = false;
            }
        }

        private async void LoadAndApply()
        {
            if (!ReadyWithSlot(
                    "LOAD"))
            {
                return;
            }

            busy = true;

            try
            {
                SaveLoadResult result =
                    await service.LoadAndApplyAsync(
                        new SaveLoadRequest(
                            selectedSlotId));

                displayedGenerationId =
                    result.SourceGenerationId;

                bool visualMatch =
                    result.Succeeded &&
                    lastSavedVisualState != null &&
                    state.ValueEquals(
                        lastSavedVisualState);

                evidence =
                    result.Succeeded
                        ? "PASS LOAD & APPLY: " +
                          (visualMatch
                              ? "RESULT: THE CHRONICLE REMEMBERS."
                              : "Load succeeded. No same-session visual snapshot was available/matching for comparison.") +
                          "\nSource Generation: " +
                          result.SourceGenerationId.Value +
                          "\nCurrent Subject: " +
                          Describe(
                              state)
                        : "FAIL LOAD & APPLY: " +
                          result.Status +
                          " | " +
                          result.Message;
            }
            finally
            {
                busy = false;
            }
        }

        private async void RenameSelectedSlot()
        {
            if (!ReadyWithSlot(
                    "RENAME SLOT"))
            {
                return;
            }

            busy = true;

            try
            {
                SaveSlotRenameResult result =
                    await service.RenameSlotAsync(
                        new SaveSlotRenameRequest(
                            selectedSlotId,
                            "Sperk File — Certified"));

                if (result.Succeeded)
                {
                    displayedGenerationId =
                        result.PublishedGenerationId;
                }

                evidence =
                    $"{(result.Succeeded ? "LAB-008 PASS" : "FAIL")} RENAME: {result.Status} | {result.Message}";
            }
            finally
            {
                busy = false;
            }
        }

        private async void DuplicateSelectedSlot()
        {
            if (!ReadyWithSlot(
                    "DUPLICATE SLOT"))
            {
                return;
            }

            busy = true;

            try
            {
                SaveSlotDuplicateResult result =
                    await service.DuplicateSlotAsync(
                        new SaveSlotDuplicateRequest(
                            selectedSlotId));

                evidence =
                    result.Succeeded
                        ? "LAB-009 PASS: duplicate published.\nNew Slot: " +
                          result.DuplicateSlotId.Value +
                          "\nGeneration: " +
                          result.DuplicateGenerationId.Value
                        : "FAIL DUPLICATE: " +
                          result.Status +
                          " | " +
                          result.Message;
            }
            finally
            {
                busy = false;
            }
        }

        private async void PrepareDelete()
        {
            if (!ReadyWithSlot(
                    "PREVIEW DELETE"))
            {
                return;
            }

            busy = true;

            try
            {
                pendingDeletionPlan =
                    await service.PrepareDeleteSlotAsync(
                        selectedSlotId);

                evidence =
                    pendingDeletionPlan.Succeeded
                        ? "DELETE PREVIEW READY / ZERO CONFIRMATION YET.\nSlot: " +
                          pendingDeletionPlan.SlotId.Value +
                          "\nGeneration: " +
                          pendingDeletionPlan.CurrentGenerationId.Value +
                          "\nExpires: " +
                          pendingDeletionPlan.ExpiresUtc.ToString("O")
                        : "FAIL DELETE PREVIEW: " +
                          pendingDeletionPlan.Status +
                          " | " +
                          pendingDeletionPlan.Message;
            }
            finally
            {
                busy = false;
            }
        }

        private async void ConfirmDelete()
        {
            if (!Ready(
                    "CONFIRM DELETE") ||
                pendingDeletionPlan == null ||
                !pendingDeletionPlan.Succeeded)
            {
                return;
            }

            busy = true;

            try
            {
                SaveSlotDeleteResult result =
                    await service.ConfirmDeleteSlotAsync(
                        pendingDeletionPlan);

                if (result.Succeeded &&
                    result.ActiveSlotCleared)
                {
                    selectedSlotId =
                        default;

                    displayedGenerationId =
                        default;
                }

                evidence =
                    $"{(result.Succeeded ? "LAB-011 PASS" : "FAIL")} DELETE: {result.Status} | Committed={YesNo(result.DeleteCommitted)} | Catalog={YesNo(result.CatalogReconciled)} | Active Cleared={YesNo(result.ActiveSlotCleared)} | {result.Message}";

                pendingDeletionPlan =
                    null;
            }
            finally
            {
                busy = false;
            }
        }

        private async void PrepareLoad()
        {
            if (!ReadyWithSlot(
                    "PREPARE LOAD"))
            {
                return;
            }

            busy = true;

            try
            {
                preparedLoad?.Dispose();
                preparedLoad =
                    null;

                PreparedLoadCreationResult result =
                    await service.PrepareLoadAsync(
                        new SaveLoadRequest(
                            selectedSlotId));

                if (result.Succeeded)
                {
                    preparedLoad =
                        result.Handle;

                    evidence =
                        "LAB-006 PREPARED: immutable prepared-load handle is live.\n" +
                        "Source Slot: " +
                        preparedLoad.SourceSlotId.Value +
                        "\nSource Generation: " +
                        preparedLoad.SourceGenerationId.Value +
                        "\nExpires: " +
                        preparedLoad.ExpiresUtc.ToString("O");
                }
                else
                {
                    evidence =
                        "FAIL PREPARE LOAD: " +
                        result.Status +
                        " | " +
                        result.Message;
                }
            }
            finally
            {
                busy = false;
            }
        }

        private async void ApplyPreparedLoad()
        {
            if (!Ready(
                    "APPLY PREPARED LOAD") ||
                preparedLoad == null ||
                !preparedLoad.IsValid)
            {
                return;
            }

            busy = true;

            PreparedSaveLoad handle =
                preparedLoad;

            try
            {
                SavePreparedLoadApplyResult result =
                    await service.ApplyPreparedLoadAsync(
                        handle);

                displayedGenerationId =
                    result.SourceGenerationId;

                bool visualMatch =
                    result.Succeeded &&
                    lastSavedVisualState != null &&
                    state.ValueEquals(
                        lastSavedVisualState);

                evidence =
                    result.Succeeded
                        ? "LAB-006 PASS: prepared handle applied after an explicit wait boundary.\n" +
                          (visualMatch
                              ? "RESULT: THE CHRONICLE REMEMBERS."
                              : "Apply succeeded; same-session visual comparison was unavailable/mismatched.") +
                          "\nHandle Consumed: " +
                          YesNo(
                              result.HandleConsumed)
                        : "FAIL APPLY PREPARED LOAD: " +
                          result.Status +
                          " | " +
                          result.Message;
            }
            finally
            {
                preparedLoad =
                    null;

                busy = false;
            }
        }

        private void SelectSlot(
            SaveSlotId slotId)
        {
            if (!Ready(
                    "SELECT SLOT"))
            {
                return;
            }

            SaveActiveSlotSelectionResult result =
                service.SelectSlot(
                    slotId);

            if (result.Succeeded &&
                result.HasActiveSlot)
            {
                selectedSlotId =
                    result.ActiveSlotId;

                SaveSlotCatalogSnapshot snapshot =
                    service.GetCatalogSnapshot();

                if (snapshot.TryGetEntry(
                        selectedSlotId,
                        out SaveSlotCatalogEntry entry))
                {
                    displayedGenerationId =
                        entry.CurrentGenerationId;
                }
            }

            pendingDeletionPlan =
                null;

            evidence =
                $"{(result.Succeeded ? "PASS" : "FAIL")} SELECT SLOT: {result.Status} | {result.Message}";
        }

        private async void ResetLaboratory()
        {
            if (busy)
            {
                return;
            }

            busy = true;

            try
            {
                preparedLoad?.Dispose();
                preparedLoad =
                    null;

                registration?.Dispose();
                registration =
                    null;

                if (root != null &&
                    root.IsAuthoritative &&
                    root.State !=
                        EchoSaveServiceState.Shutdown)
                {
                    await root.ShutdownAsync();
                }

                string rootPath =
                    ResolveLaboratoryRootPath();

                if (!Directory.Exists(
                        rootPath))
                {
                    evidence =
                        "LAB-031 PASS: Laboratory root is already absent. Stop Play Mode and press Play to re-enter.";
                    return;
                }

                string markerPath =
                    Path.Combine(
                        rootPath,
                        OwnershipMarkerFile);

                if (!File.Exists(
                        markerPath) ||
                    !string.Equals(
                        File.ReadAllText(
                            markerPath,
                            Encoding.UTF8).Trim(),
                        OwnershipMarkerValue,
                        StringComparison.Ordinal))
                {
                    evidence =
                        "RESET REFUSED: exact M5-06 ownership marker was not present. No directory was deleted.";
                    return;
                }

                Directory.Delete(
                    rootPath,
                    true);

                evidence =
                    Directory.Exists(
                        rootPath)
                        ? "FAIL RESET: directory deletion returned but the Laboratory root still exists."
                        : "LAB-031 PASS: owned Laboratory root removed and post-cleanup absence verified. Stop Play Mode and press Play to re-enter a fresh reactor.";
            }
            catch (Exception exception)
            {
                evidence =
                    "RESET EXCEPTION: " +
                    exception;
            }
            finally
            {
                busy = false;
            }
        }

        private void EnsureOwnershipMarker()
        {
            string rootPath =
                ResolveLaboratoryRootPath();

            Directory.CreateDirectory(
                rootPath);

            string markerPath =
                Path.Combine(
                    rootPath,
                    OwnershipMarkerFile);

            if (File.Exists(
                    markerPath))
            {
                string existing =
                    File.ReadAllText(
                        markerPath,
                        Encoding.UTF8).Trim();

                if (!string.Equals(
                        existing,
                        OwnershipMarkerValue,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "Chronicle Laboratory root exists with a conflicting ownership marker.");
                }

                return;
            }

            File.WriteAllText(
                markerPath,
                OwnershipMarkerValue +
                Environment.NewLine,
                new UTF8Encoding(
                    false));
        }

        private string ResolveLaboratoryRootPath()
        {
            if (root == null ||
                root.Configuration == null)
            {
                throw new InvalidOperationException(
                    "Chronicle Laboratory configuration is unavailable.");
            }

            string rootName =
                root.Configuration
                    .StorageRootDirectoryName;

            if (!string.Equals(
                    rootName,
                    "EchoSave-M5-06-Laboratory",
                    StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Chronicle Laboratory refuses to operate on any storage root other than EchoSave-M5-06-Laboratory.");
            }

            string persistent =
                Path.GetFullPath(
                    Application.persistentDataPath);

            string candidate =
                Path.GetFullPath(
                    Path.Combine(
                        persistent,
                        rootName));

            string prefix =
                persistent.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            if (!candidate.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Chronicle Laboratory root escaped Application.persistentDataPath.");
            }

            return candidate;
        }

        private bool Ready(
            string operation)
        {
            if (busy)
            {
                evidence =
                    operation +
                    " REFUSED: another Laboratory operation is in progress.";
                return false;
            }

            if (service == null ||
                root == null ||
                root.State !=
                    EchoSaveServiceState.Ready)
            {
                evidence =
                    operation +
                    " REFUSED: Chronicle is not Ready.";
                return false;
            }

            return true;
        }

        private bool ReadyWithSlot(
            string operation)
        {
            if (!Ready(
                    operation))
            {
                return false;
            }

            if (string.IsNullOrEmpty(
                    selectedSlotId.Value))
            {
                evidence =
                    operation +
                    " REFUSED: select or create a slot first.";
                return false;
            }

            return true;
        }

        private bool Button(
            string label)
        {
            GUI.enabled =
                !busy;

            bool pressed =
                GUILayout.Button(
                    label,
                    GUILayout.MinHeight(28f));

            GUI.enabled = true;

            return pressed;
        }

        private static void Section(
            string title)
        {
            GUILayout.Space(12f);
            GUILayout.Label(
                title,
                GUI.skin.box);
        }

        private static GUIStyle LargeLabel()
        {
            GUIStyle style =
                new GUIStyle(
                    GUI.skin.label)
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true
                };

            return style;
        }

        private static string Describe(
            ChronicleSaveLaboratoryState value)
        {
            if (value == null)
            {
                return "NONE";
            }

            return
                $"Level={value.sperkLevel}, Rupees={value.galacticRupees}, Anvil={value.anvilTemperature}, ForbiddenKey={YesNo(value.hasForbiddenKey)}, RealityDamage={value.realityDamagePercent}%";
        }

        private static string YesNo(
            bool value) =>
            value
                ? "YES"
                : "NO";

        private static string ValueOrNone(
            string value) =>
            string.IsNullOrEmpty(
                value)
                ? "NONE"
                : value;
    }
}
