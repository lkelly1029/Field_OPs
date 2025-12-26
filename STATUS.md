# Sovereign State — Current Status

Last updated: (Gemini fills this)

## Sources of Truth
- control/sovereign_state_control.yaml
- control/SovereignState_Build_Reference.md
- control/SovereignState_Unity_Placement_Reference.md
- control/SovereignState_Dev_Onboarding_Checklist.md
- control/SovereignState_First_Vertical_Replication_Guide.md
- README.md

## Repo Health
- Guardrails: UNKNOWN (run: .\run_guardrails.ps1)
- Unity compile: UNKNOWN
- Engine build: UNKNOWN

## Current Milestone
Power Vertical MVP

## Power Vertical — Implementation Checklist
### Simulation Core
- [ ] Tick loop (deterministic)
- [ ] Ledger core (units, ownership, pricing hooks)
- [ ] State machine for Plots (Vacant → Surveyed → Serviced → Built → Active → Decay)

### Power Sector
- [ ] Power producer (e.g., Nuclear Plant) generates MW per tick
- [ ] Grid loss / transport math
- [ ] Delivery to consumer (House)
- [ ] Settlement/payment (House pays Plant)
- [ ] AI Global Market import fallback (high price)
- [ ] Player export listing to Global Exchange (displaces AI)

### Testing
- [ ] PowerAcceptance scenario runs headless
- [ ] Acceptance assertions implemented (Dependency → Substitution → Export displacement)

### Unity Visibility
- [ ] Dev Console overlay displays tick + key ledger values
- [ ] ISimDebugProvider is wired from a SimRunner/Bridge MonoBehaviour

## Next 5 Concrete Steps
1.
2.
3.
4.
5.

## Risks / Decisions Needed
- None logged yet.

## Notes
- Keep Unity presentation-only; keep engine headless.
