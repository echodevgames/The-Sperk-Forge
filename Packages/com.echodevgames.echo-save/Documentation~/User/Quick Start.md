# Quick Start

## Goal

Prove one Chronicle authority can initialize without touching durable storage.

1. Create `EchoSaveConfiguration`.
2. Create a GameObject named `Chronicle`.
3. Add `EchoSaveRoot`.
4. Assign the configuration.
5. Enable **Auto Initialize**, or call `InitializeAsync()` from project startup code.
6. Enter Play Mode.
7. Confirm the root is authoritative and reaches `Ready`.

If another `EchoSaveRoot` appears while the first is authoritative, the duplicate is rejected and disabled before Chronicle initialization.

When the authoritative root shuts down, a later root may claim Chronicle authority.

No real save files are expected in this checkpoint.
