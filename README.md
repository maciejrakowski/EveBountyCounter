# EveBountyCounter

A simple Eve Online bounty counter. It monitors all the characters and counts the bounties by reading log files.

Outputs in the format:
```
2025-07-12 19:36:23: Test Character1: bounty 109,750 ISK; 109750; session total bounty 109,750 ISK'
Date Time: Character name: bounty (user friendly); bounty (for eve-journal bounty field); total bounty for the session (resets on logout)
```

Resets on undock (except session total bounty)

More features are hopefully coming soon
