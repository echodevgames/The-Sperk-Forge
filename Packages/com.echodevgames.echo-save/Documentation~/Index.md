# The Chronicle Documentation

## User

- [[User/Installation|Installation]]
- [[User/Quick Start|Quick Start]]

## Developer

- [[Developer/Current Notes|Current Notes]]
- [[Developer/Checkpoints/ESV-M1-01_Chronicle_Installable_Skeleton_and_Duplicate-Safe_Authority_Claim|ESV-M1-01 Closeout]]
- [[Developer/Test Reports/ESV-M1-01_Chronicle_Installable_Skeleton_and_Duplicate-Safe_Authority_Claim_Test_Report|ESV-M1-01 Test Report]]
- [[Developer/Checkpoints/ESV-M2-01_Chronicle_Storage_Root_Path_Safety_and_Local_Backend_Foundation|ESV-M2-01 Closeout]]
- [[Developer/Test Reports/ESV-M2-01_Chronicle_Storage_Root_Path_Safety_and_Local_Backend_Foundation_Test_Report|ESV-M2-01 Test Report]]
- [[Developer/Checkpoints/ESV-M2-02_Chronicle_Document_Contracts_and_Unity_JSON_Serializer_Foundation|ESV-M2-02 Closeout]]
- [[Developer/Test Reports/ESV-M2-02_Chronicle_Document_Contracts_and_Unity_JSON_Serializer_Foundation_Test_Report|ESV-M2-02 Test Report]]
- [[Developer/Checkpoints/ESV-M2-03_Chronicle_Generation_Identity_Integrity_and_Commit-Document_Foundation|ESV-M2-03 Closeout]]
- [[Developer/Test Reports/ESV-M2-03_Chronicle_Generation_Identity_Integrity_and_Commit-Document_Foundation_Test_Report|ESV-M2-03 Test Report]]
- [[Developer/Checkpoints/ESV-M2-04_Chronicle_Immutable_Generation_Publication_and_Head-Last_Commit_Foundation|ESV-M2-04 Closeout]]
- [[Developer/Test Reports/ESV-M2-04_Chronicle_Immutable_Generation_Publication_and_Head-Last_Commit_Foundation_Test_Report|ESV-M2-04 Test Report]]
- [[Developer/Checkpoints/ESV-M3-01_Chronicle_Participant_Contracts_Descriptor_Validation_and_Duplicate-Safe_Registry_Foundation|ESV-M3-01 Closeout]]
- [[Developer/Test Reports/ESV-M3-01_Chronicle_Participant_Contracts_Descriptor_Validation_and_Duplicate-Safe_Registry_Foundation_Test_Report|ESV-M3-01 Test Report]]
- [[Developer/Checkpoints/ESV-M3-02_Chronicle_Detached_Participant_Capture_Runtime_Type_Routing_and_Payload-Entry_Construction_Foundation|ESV-M3-02 Closeout]]
- [[Developer/Test Reports/ESV-M3-02_Chronicle_Detached_Participant_Capture_Runtime_Type_Routing_and_Payload-Entry_Construction_Foundation_Test_Report|ESV-M3-02 Test Report]]
- [[Developer/Checkpoints/ESV-M3-03_Chronicle_Participant-Backed_Generation_Publication_and_Head-Last_Integration_Foundation|ESV-M3-03 Closeout]]
- [[Developer/Test Reports/ESV-M3-03_Chronicle_Participant-Backed_Generation_Publication_and_Head-Last_Integration_Foundation_Test_Report|ESV-M3-03 Test Report]]

## Completed checkpoint

`ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation`

Chronicle can durably publish verified participant-backed generations while preserving the previous known-good head across injected failures at **197 / 197** focused Chronicle Editor tests.

## Current checkpoint

`ESV-M3-04 — Current-Generation Read, Opaque Unknown-Payload Preservation, and Session Store Foundation`

This checkpoint may read/validate the current generation and preserve unclaimed participant entries as inert opaque session data. It does not yet merge those entries into the next save.
