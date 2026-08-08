# First Light Standalone Test Lab

This Unity Package Manager sample is the isolated acceptance laboratory for
**First Light — Startup and Launch (`EchoLaunch`)**.

## Boundary

The Laboratory demonstrates the existing First Light runtime and Editor
contracts. It does not provide another launch authority, setup system,
simulation engine, or import-time authoring process.

Importing this sample must not automatically:

- enter Play Mode;
- edit Build Settings;
- run Setup Apply or Setup Repair;
- run Validator or Simulator;
- create project content outside the imported sample root;
- modify package source.

## Package-development status

FL-M5-07 is implemented in guarded stages.

Step A establishes:

- the single UPM sample declaration;
- the isolated sample runtime assembly;
- the Laboratory runtime source contracts;
- static tests for the distribution and dependency boundary.

The fully authored scenes, configurations, prefab, splash art, and serialized
reference graph are added in Step B before the sample is manually imported for
acceptance testing.

## Planned acceptance matrix

The completed Laboratory proves:

1. Canonical Boot success.
2. Timed progress.
3. Warning continuation.
4. Missing-configuration block.
5. Blocking-failure stop.
6. Duplicate-authority rejection.
7. Invalid-destination preflight.
8. Direct-scene authority creation.
9. Existing-authority reuse.
10. Splash minimum-duration and skip policy.
11. Sample removal.
12. Setup/Repair repeatability.

## LAB-010 splash skip fixture

The Laboratory readout includes a sample-only IMGUI control named:

```text
Request Splash Skip (LAB-010)
```

Run the Boot Lab with `SuccessConfiguration.asset`. During the authored splash,
click the control while the readout still reports:

```text
Skip permitted now: False
```

The control routes the request through the default First Light uGUI presenter's
public `RequestSplashSkip()` method. The request is intentionally latched by the
existing splash player. The splash must remain visible until the readout reaches
the authored minimum duration and the policy permits the skip.

The readout also displays the live elapsed time, authored minimum duration, and
current `CanSkipNow` state so the manual evidence can be captured without adding
a package-owned keyboard/controller binding.

This control is Laboratory acceptance UI only. It does not become production
input ownership or another launch/presentation authority.

The imported sample is acceptance content only. Package Runtime and Editor
assemblies must remain healthy when it is absent.
