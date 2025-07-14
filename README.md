# EveBountyCounter

A simple Eve Online bounty counter. It monitors all the characters and counts the bounties by reading log files.

Outputs in the format:
```
2025-07-12 19:36:23: Test Character1: bounty 109,750 ISK; 109750; session total bounty 109,750 ISK'
Date Time: Character name: bounty (user friendly); bounty (for eve-journal bounty field); total bounty for the session (resets on logout)
```

Resets bounty counter on undock (except session total bounty) or by pressing R.  
In case of more than one character with positive bounty a menu will appear allowing to choose character for that operation.

More features are hopefully coming soon.

## API Integration

There is possibility to send bounties directly to Eve Journal.  
API key is required. It can be create on https://evejournal.com/my-account/personal-access-tokens.  
Then in EveBountyCounter, press C and follow the steps.  
Once done, the current bounty value can be send to open run page by pressing S

## Help menu

Help menu is accessible by pressing key H. It will show available shortcuts:

```
EVE Bounty Counter
H - Help (this screen)
R - Reset character bounty
C - Add API key
S - Submit bounty
L - Update Logs Directory
Q or ESC - Quit
```

Example output:
```
2025-07-12 19:34:01: No configuration found. Please enter the logs directory path  (most likely: C:\Users\your_user_name\Documents\EVE\logs\Gamelogs):
2025-07-12 19:34:01: Enter logs directory path: C:\Users\your_user_name\Documents\EVE\logs\Gamelogs
2025-07-12 19:34:06: Path validated: C:\Users\your_user_name\Documents\EVE\logs\Gamelogs
2025-07-12 19:34:06: Configuration saved.
2025-07-12 19:34:07: character_name_1: tracking
2025-07-12 19:34:07: character_name_2: tracking
2025-07-12 19:35:23: character_name_2: bounty 40,000 ISK; 40000; session total bounty 40,000 ISK
2025-07-12 19:35:36: character_name_2: bounty 47,500 ISK; 47500; session total bounty 47,500 ISK
2025-07-12 19:35:50: character_name_2: bounty 55,750 ISK; 55750; session total bounty 55,750 ISK
2025-07-12 19:35:51: character_name_2: bounty 63,625 ISK; 63625; session total bounty 63,625 ISK
2025-07-12 19:36:04: character_name_2: bounty 78,250 ISK; 78250; session total bounty 78,250 ISK
2025-07-12 19:36:15: character_name_2: bounty 94,000 ISK; 94000; session total bounty 94,000 ISK
2025-07-12 19:36:23: character_name_2: bounty 109,750 ISK; 109750; session total bounty 109,750 ISK
2025-07-12 19:36:52: character_name_2: bounty 149,750 ISK; 149750; session total bounty 149,750 ISK
2025-07-12 19:37:01: character_name_2: bounty 165,500 ISK; 165500; session total bounty 165,500 ISK
2025-07-12 19:37:20: character_name_2: bounty 180,125 ISK; 180125; session total bounty 180,125 ISK
2025-07-12 19:37:42: character_name_2: bounty 223,250 ISK; 223250; session total bounty 223,250 ISK
2025-07-12 19:37:58: character_name_2: bounty 266,375 ISK; 266375; session total bounty 266,375 ISK
2025-07-12 19:38:10: character_name_2: bounty 274,625 ISK; 274625; session total bounty 274,625 ISK
2025-07-12 19:38:31: character_name_2: bounty 312,125 ISK; 312125; session total bounty 312,125 ISK
2025-07-12 19:38:40: character_name_2: bounty 320,000 ISK; 320000; session total bounty 320,000 ISK
2025-07-12 19:39:12: character_name_2: bounty 363,125 ISK; 363125; session total bounty 363,125 ISK
2025-07-12 19:39:30: character_name_2: bounty 400,625 ISK; 400625; session total bounty 400,625 ISK
2025-07-12 19:40:08: character_name_2: bounty 452,188 ISK; 452188; session total bounty 452,188 ISK
2025-07-12 19:40:17: character_name_2: bounty 466,813 ISK; 466813; session total bounty 466,813 ISK
2025-07-12 19:40:28: character_name_2: bounty 478,626 ISK; 478626; session total bounty 478,626 ISK
2025-07-12 19:41:01: character_name_2: bounty 530,189 ISK; 530189; session total bounty 530,189 ISK
2025-07-12 19:41:48: character_name_2: bounty 593,002 ISK; 593002; session total bounty 593,002 ISK
2025-07-12 19:41:57: character_name_2: bounty 607,627 ISK; 607627; session total bounty 607,627 ISK
```
