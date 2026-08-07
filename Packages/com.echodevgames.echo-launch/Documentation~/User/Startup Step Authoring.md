# First Light Startup Step Authoring

First Light runs project-authored `StartupStepDefinition` assets in the order
stored by a project-owned `StartupSequence`. Each definition creates a fresh,
single-use executor so mutable attempt state never lives in the shared asset.

## Minimal successful step

```csharp
using EchoDevGames.EchoLaunch;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ProjectStartupStep",
    menuName = "My Game/Startup/Project Step")]
public sealed class ProjectStartupStep : StartupStepDefinition
{
    public override IStartupStepExecutor CreateExecutor()
    {
        return new Executor();
    }

    private sealed class Executor : IStartupStepExecutor
    {
        public Awaitable<StartupStepResult> ExecuteAsync(
            StartupStepContext context)
        {
            context.ProgressReporter.Report(
                StartupStepProgress.Determinate(
                    1f,
                    "Project startup step completed."));

            AwaitableCompletionSource<StartupStepResult> source =
                new AwaitableCompletionSource<StartupStepResult>();

            source.SetResult(
                StartupStepResult.Success(
                    "Project startup step completed."));

            return source.Awaitable;
        }
    }
}
```

## Multi-frame work

For asynchronous Unity work, return `Awaitable<StartupStepResult>`, observe
`context.CancellationToken`, and report bounded progress through
`context.ProgressReporter`. Do not retain the context, reporter, token, or
runtime state in the definition asset.

## Add the step

1. Create the custom definition asset from its **Create** menu.
2. Open the project-owned `StartupSequence.asset`.
3. Add an entry, assign the definition, and keep its stable entry ID unique.
4. Choose required/optional intent, failure action, timeout, and cancellation
   metadata deliberately.
5. Run the Simulator for policy-focused proof.
6. Run canonical Boot for full root/presentation/destination proof.

## Failure policy

The MVP supports:

```text
BlockLaunch
ContinueWithWarning
```

Interactive retry, retry counts/backoff, and retry/skip UI are not implemented.
Return structured `StartupStepResult` values; do not use unhandled exceptions as
ordinary control flow.

## Reference implementations

Import the **First Light Standalone Test Lab** and inspect its immediate,
timed-progress, warning, recoverable-failure, and blocking-failure definitions.
The sample uses only public First Light APIs and can be removed afterward.
