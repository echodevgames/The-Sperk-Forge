using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace EchoDevGames.EchoUI.Samples
{
    [DefaultExecutionOrder(10000)]
    [DisallowMultipleComponent]
    public sealed class LaboratoryMotifProof : MonoBehaviour
    {
        private const string DefaultId = "motif.lab.graphite";
        private const string SecondId = "motif.lab.signal";
        private const string UnknownId = "motif.lab.unknown";
        private const string ColorId = "color.surface";
        private const string SelectableId = "selectable.action";
        private const string SpriteId = "sprite.badge";
        private const string NumberId = "number.opacity";
        private const string MissingId = "color.unavailable";

        private static readonly Color DefaultColor = new Color(0.12f, 0.16f, 0.22f, 1f);
        private static readonly Color SecondColor = new Color(0.55f, 0.16f, 0.48f, 1f);
        private static readonly Color LocalColor = new Color(0.94f, 0.58f, 0.12f, 1f);
        private static readonly Color SafeColor = new Color(0.18f, 0.72f, 0.34f, 1f);

        private EchoUIRoot root;
        private UIMotifDefinition first;
        private UIMotifDefinition second;
        private UIMotifCatalog catalog;
        private Texture2D firstTexture;
        private Texture2D secondTexture;
        private Sprite firstSprite;
        private Sprite secondSprite;
        private string firstBaseline;
        private string secondBaseline;
        private LaboratoryMotifTarget targetA;
        private LaboratoryMotifTarget targetB;
        private UIMotifRegistrationHandle handleA;
        private UIMotifRegistrationHandle handleB;
        private bool prepared;
        private bool ready;
        private bool busy;
        private Vector2 scroll;
        private string state = "Bootstrapping M4-03 Motifs...";
        private string observed = "<not run>";
        private string idle = "<not run>";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            LaboratoryUIContextDriver[] drivers = Resources.FindObjectsOfTypeAll<LaboratoryUIContextDriver>();
            for (int i = 0; i < drivers.Length; i++)
            {
                LaboratoryUIContextDriver driver = drivers[i];
                if (driver == null || !driver.gameObject.scene.IsValid() || !driver.gameObject.scene.isLoaded)
                    continue;

                LaboratoryMotifProof proof = driver.GetComponent<LaboratoryMotifProof>();
                if (proof == null) proof = driver.gameObject.AddComponent<LaboratoryMotifProof>();
                proof.Prepare(driver.GetComponent<EchoUIRoot>());
                return;
            }
        }

        private void Prepare(EchoUIRoot value)
        {
            if (prepared) return;
            prepared = true;
            root = value;
            if (root == null)
            {
                state = "FAIL: EchoUIRoot not found.";
                return;
            }
            if (root.IsInitialized)
            {
                state = "FAIL: Root initialized before Motif sample bootstrap. Re-enter Play Mode.";
                return;
            }

            CreateAssets();
            FieldInfo field = typeof(EchoUIRoot).GetField("motifCatalog", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                state = "FAIL: EchoUIRoot.motifCatalog not found.";
                return;
            }
            field.SetValue(root, catalog);
            state = "Transient sample Motif catalog armed before EchoUIRoot.Start().";
        }

        private IEnumerator Start()
        {
            for (int i = 0; root != null && !root.IsInitialized && i < 120; i++) yield return null;
            if (root == null || !root.IsInitialized || !root.IsMotifLifecycleInitialized)
            {
                state = "FAIL: Motif lifecycle did not initialize.";
                yield break;
            }

            targetA = CreateTarget("M4-03 Target A", UIMotifBindingMode.UseMotif, Color.white, new Vector2(-315f, 90f), ColorId);
            targetB = CreateTarget("M4-03 Target B LOCAL", UIMotifBindingMode.KeepLocal, LocalColor, new Vector2(-315f, -45f), ColorId);
            handleA = root.RegisterMotifTarget(targetA, targetA);
            handleB = root.RegisterMotifTarget(targetB, targetB);
            ready = handleA.Result.Succeeded && handleB.Result.Succeeded;
            state = ready ? "M4-03 Motif proof READY. Run checks 1-6 in order." : "FAIL: primary target registration.";
        }

        private void OnGUI()
        {
            Color prior = GUI.contentColor;
            GUI.contentColor = new Color32(255, 45, 214, 255);
            const float w = 430f, retained = 470f, margin = 20f, gap = 12f;
            float h = Mathf.Min(Screen.height - margin * 2f, 820f);
            float left = Mathf.Max(margin, Screen.width - retained - w - gap - margin);
            GUILayout.BeginArea(new Rect(left, margin, w, h), "M4-03 MOTIFS - REAL RUNTIME PROOF", GUI.skin.window);
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label(state);
            if (root != null) GUILayout.Label("effective: " + root.EffectiveMotifId.Value + " | targets: " + root.RegisteredMotifTargetCount);
            GUILayout.Label("Observed: " + observed);
            GUILayout.Label("Idle: " + idle);
            GUILayout.Space(8f);
            bool priorEnabled = GUI.enabled;
            GUI.enabled = priorEnabled && ready && !busy;
            if (GUILayout.Button("Run Check 1: Default + Typed Tokens")) Check1();
            if (GUILayout.Button("Run Check 2: Switch + Keep Local")) Check2();
            if (GUILayout.Button("Run Check 3: Missing Token Safety")) Check3();
            if (GUILayout.Button("Run Check 4: Unknown ID Fallback")) Check4();
            if (GUILayout.Button("Run Check 5: Failure + Stale + Reset")) StartCoroutine(Check5());
            if (GUILayout.Button("Run Check 6: 180-Frame Idle")) StartCoroutine(Check6());
            GUI.enabled = priorEnabled;
            GUILayout.Space(8f);
            GUILayout.Label("Check 5 intentionally logs one target exception. It is the isolation proof.");
            GUILayout.Label("After Check 6, smoke M4-02 through M1 in the retained console.");
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUI.contentColor = prior;
        }

        private void Check1()
        {
            UIMotifSwitchResult r = root.ResetMotif();
            bool pass = r.Succeeded && root.EffectiveMotifId.Value == DefaultId &&
                Near(targetA.SurfaceColor, DefaultColor) && Near(targetB.SurfaceColor, LocalColor) &&
                targetA.BadgeSprite == firstSprite && targetB.BadgeSprite == firstSprite &&
                Mathf.Approximately(targetA.Opacity, 0.82f) &&
                targetA.LastResult.AppliedBindingCount == 4 && targetB.LastResult.KeptLocalBindingCount == 1 && AssetsStable();
            observed = Result(pass) + " | default typed tokens + local preservation | immutable=" + AssetsStable();
        }

        private void Check2()
        {
            UIMotifSwitchResult r = root.SwitchMotif(new UIMotifId(SecondId));
            bool pass = r.Succeeded && root.EffectiveMotifId.Value == SecondId &&
                Near(targetA.SurfaceColor, SecondColor) && Near(targetB.SurfaceColor, LocalColor) &&
                targetA.BadgeSprite == secondSprite && targetB.BadgeSprite == secondSprite &&
                Mathf.Approximately(targetA.Opacity, 0.56f) && targetB.LastResult.KeptLocalBindingCount == 1 && AssetsStable();
            observed = Result(pass) + " | status=" + r.Status + " | KeepLocal=" + targetB.LastResult.KeptLocalBindingCount;
        }

        private void Check3()
        {
            LaboratoryMotifTarget t = CreateTarget("M4-03 Missing Token", UIMotifBindingMode.UseMotif, SafeColor, new Vector2(-315f, -180f), MissingId);
            Color before = t.SurfaceColor;
            UIMotifRegistrationHandle h = root.RegisterMotifTarget(t, t);
            bool pass = h.Result.Status == UIMotifRegistrationStatus.Registered &&
                h.Result.ApplyResult.Status == UIMotifTargetApplyStatus.Partial && h.Result.ApplyResult.FailedBindingCount == 1 &&
                Near(t.SurfaceColor, before) && root.EffectiveMotifId.Value == SecondId;
            observed = Result(pass) + " | registration=" + h.Result.Status + " | apply=" + h.Result.ApplyResult.Status + " | safe prior preserved=" + Near(t.SurfaceColor, before);
            h.Release();
            Destroy(t.gameObject);
        }

        private void Check4()
        {
            UIMotifSwitchResult reset = root.ResetMotif();
            UIMotifId requested = new UIMotifId(UnknownId);
            UIMotifSwitchResult r = root.SwitchMotif(requested);
            bool pass = reset.Succeeded && r.Status == UIMotifSwitchStatus.FallbackApplied && r.RequestedMotifId == requested &&
                r.EffectiveMotifId.Value == SecondId && root.EffectiveMotifId.Value == SecondId;
            observed = Result(pass) + " | reset=" + reset.Status + " | requested=" + r.RequestedMotifId + " | effective=" + r.EffectiveMotifId;
        }

        private IEnumerator Check5()
        {
            busy = true;
            root.ResetMotif();
            LaboratoryMotifTarget broken = CreateTarget("M4-03 Broken Target", UIMotifBindingMode.UseMotif, Color.white, new Vector2(-315f, -180f), ColorId);
            broken.SetThrowOnApply(true);
            UIMotifRegistrationHandle old = root.RegisterMotifTarget(broken, broken);
            int healthyBefore = targetA.ApplicationCount;
            UIMotifSwitchResult switched = root.SwitchMotif(new UIMotifId(SecondId));
            long oldGeneration = old.Generation;
            Destroy(broken.gameObject);
            yield return null;
            int prunedCount = root.RegisteredMotifTargetCount;
            UIMotifRegistrationReleaseResult stale = old.Release();
            LaboratoryMotifTarget replacement = CreateTarget("M4-03 Replacement", UIMotifBindingMode.UseMotif, Color.white, new Vector2(-315f, -180f), ColorId);
            UIMotifRegistrationHandle fresh = root.RegisterMotifTarget(replacement, replacement);
            UIMotifSwitchResult reset = root.ResetMotif();
            bool pass = old.Result.Status == UIMotifRegistrationStatus.RegisteredWithApplyFailure && switched.FailedTargetCount == 1 &&
                targetA.ApplicationCount > healthyBefore && prunedCount == 2 && stale.Status == UIMotifRegistrationReleaseStatus.Stale &&
                fresh.Result.Succeeded && fresh.Generation > oldGeneration && reset.Succeeded && root.EffectiveMotifId.Value == DefaultId && AssetsStable();
            observed = Result(pass) + " | failed=" + switched.FailedTargetCount + " | pruned=" + prunedCount + " | stale=" + stale.Status + " | reset=" + reset.Status;
            fresh.Release();
            Destroy(replacement.gameObject);
            busy = false;
        }

        private IEnumerator Check6()
        {
            busy = true;
            root.ResetMotif();
            root.TryGetMotifSnapshot(out UIMotifServiceSnapshot before);
            int count = root.RegisteredMotifTargetCount;
            int a = targetA.ApplicationCount;
            int b = targetB.ApplicationCount;
            for (int frame = 0; frame < 180; frame++) yield return null;
            bool hasAfter = root.TryGetMotifSnapshot(out UIMotifServiceSnapshot after);
            bool pass = hasAfter && before.State == after.State && before.EffectiveMotifId == after.EffectiveMotifId &&
                before.Revision == after.Revision && count == root.RegisteredMotifTargetCount &&
                a == targetA.ApplicationCount && b == targetB.ApplicationCount && AssetsStable();
            idle = Result(pass) + " | revision " + before.Revision + "->" + after.Revision + " | targets " + count + "->" + root.RegisteredMotifTargetCount + " | applies A " + a + "->" + targetA.ApplicationCount;
            busy = false;
        }

        private void CreateAssets()
        {
            firstTexture = Texture("M4-03 Graphite", new Color(0.3f, 0.76f, 0.94f, 1f));
            secondTexture = Texture("M4-03 Signal", new Color(0.95f, 0.32f, 0.64f, 1f));
            firstSprite = SpriteFor("M4-03 Graphite Badge", firstTexture);
            secondSprite = SpriteFor("M4-03 Signal Badge", secondTexture);
            first = Definition(DefaultId, DefaultColor, firstSprite, 0.82f, new Color(0.22f, 0.74f, 0.88f, 1f));
            second = Definition(SecondId, SecondColor, secondSprite, 0.56f, new Color(0.92f, 0.62f, 0.18f, 1f));
            catalog = UIMotifCatalog.CreateTransient(DefaultId, SecondId, new[] { first, second });
            firstBaseline = JsonUtility.ToJson(first);
            secondBaseline = JsonUtility.ToJson(second);
        }

        private static UIMotifDefinition Definition(string id, Color color, Sprite sprite, float opacity, Color selectable) =>
            UIMotifDefinition.CreateTransient(id,
                new[] { new UIMotifColorToken(ColorId, color) },
                new[] { new UIMotifSelectableColorsToken(SelectableId, Block(selectable)) },
                new[] { new UIMotifSpriteToken(SpriteId, sprite) },
                new[] { new UIMotifNumberToken(NumberId, opacity) });

        private LaboratoryMotifTarget CreateTarget(string name, UIMotifBindingMode mode, Color local, Vector2 position, string colorToken)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(CanvasGroup), typeof(LaboratoryMotifTarget));
            go.hideFlags = HideFlags.DontSave;
            go.transform.SetParent(transform, false);
            RectTransform rect = (RectTransform)go.transform;
            rect.sizeDelta = new Vector2(250f, 110f);
            rect.anchoredPosition = position;
            Image surface = go.GetComponent<Image>();
            CanvasGroup group = go.GetComponent<CanvasGroup>();
            GameObject buttonGo = new GameObject("Selectable", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonGo.hideFlags = HideFlags.DontSave;
            buttonGo.transform.SetParent(go.transform, false);
            GameObject badgeGo = new GameObject("Badge", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            badgeGo.hideFlags = HideFlags.DontSave;
            badgeGo.transform.SetParent(go.transform, false);
            LaboratoryMotifTarget target = go.GetComponent<LaboratoryMotifTarget>();
            target.Configure(surface, buttonGo.GetComponent<Button>(), badgeGo.GetComponent<Image>(), group, mode, local, colorToken);
            return target;
        }

        private bool AssetsStable() => first != null && second != null && firstBaseline == JsonUtility.ToJson(first) && secondBaseline == JsonUtility.ToJson(second);

        private static Texture2D Texture(string name, Color color)
        {
            Texture2D t = new Texture2D(2, 2, TextureFormat.RGBA32, false) { name = name, hideFlags = HideFlags.DontSave };
            t.SetPixels(new[] { color, color, color, color });
            t.Apply();
            return t;
        }

        private static Sprite SpriteFor(string name, Texture2D texture)
        {
            Sprite s = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);
            s.name = name;
            s.hideFlags = HideFlags.DontSave;
            return s;
        }

        private static ColorBlock Block(Color normal)
        {
            ColorBlock b = ColorBlock.defaultColorBlock;
            b.normalColor = normal;
            b.highlightedColor = Color.Lerp(normal, Color.white, 0.15f);
            b.pressedColor = Color.Lerp(normal, Color.black, 0.22f);
            b.selectedColor = Color.Lerp(normal, Color.white, 0.08f);
            b.disabledColor = new Color(normal.r, normal.g, normal.b, 0.35f);
            b.colorMultiplier = 1f;
            b.fadeDuration = 0.08f;
            return b;
        }

        private static bool Near(Color a, Color b) => Mathf.Abs(a.r - b.r) < 0.001f && Mathf.Abs(a.g - b.g) < 0.001f && Mathf.Abs(a.b - b.b) < 0.001f && Mathf.Abs(a.a - b.a) < 0.001f;
        private static string Result(bool pass) => pass ? "PASS" : "FAIL";

        private void OnDestroy()
        {
            if (handleA != null) handleA.Release();
            if (handleB != null) handleB.Release();
            Dispose(first);
            Dispose(second);
            Dispose(catalog);
            Dispose(firstSprite);
            Dispose(secondSprite);
            Dispose(firstTexture);
            Dispose(secondTexture);
        }

        private static void Dispose(Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) Destroy(value); else DestroyImmediate(value);
        }
    }
}
