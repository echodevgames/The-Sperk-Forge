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

## Completed checkpoint

`ESV-M3-02 — Detached Participant Capture, Runtime Type Routing, and Payload-Entry Construction Foundation`

Chronicle can convert active open-ended participants into one deterministic verified all-or-nothing in-memory transport batch at **171 / 171** focused Chronicle Editor tests.

## Current checkpoint

`ESV-M3-03 — Participant-Backed Generation Publication and Head-Last Integration Foundation`

This checkpoint may join the successful participant batch to the M2 immutable-generation transaction. It still does not expose production `SaveAsync` or load/apply behavior.
