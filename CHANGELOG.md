# Change Log

All notable changes to EveBountyCounter project will be documented in this file.

## [0.2.2] - 2025-07-13

### Changed
- Bounty parsing now supports both , . (e.g. 1,024.69) and . , (e.g. 1.024,69) as thousand and decimal separators
- When a log file is updated, only the file that was updated will be read instead of reading the latest log files for each character

## [0.2.1] - 2025-07-13

### Fixed
- Bounty parsing problem for , as thousands separator

## [0.2.0] - 2025-07-13

### Added
- Added integration with EVEWorkbench API for updating bounties
- This Changelog
