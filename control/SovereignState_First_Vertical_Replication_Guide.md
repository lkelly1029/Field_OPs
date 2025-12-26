# First Vertical Replication Guide  
## Power → Water

This guide explains how to add a second resource **without adding new systems**.

If you need new engine logic, the architecture has failed.

---

## 1. Purpose of This Guide

- Power is the **reference vertical**
- Water must reuse:
  - Ledger
  - Orders
  - Resolver
  - Market
  - Settlement
  - Plot state machine

Only **data + tuning** should change.

---

## 2. Define Water as Data (No Code)

### Resource Definition (example)

```yaml
resource:
  name: Water
  unit: Liter
  critical: true
  transport:
    loss_model: leakage_rate
    fee_model: pump_energy_cost
```

---

## 3. Producers (Data-Only)

Examples:
- Well
- Reservoir
- Desalination Plant

Each producer defines:
- production_per_tick
- operating_cost
- resource_type = Water

---

## 4. Consumers (Reuse Existing Logic)

Examples:
- House
- Industry
- Agriculture

Each consumer:
- Emits ResourceRequest(Water, qty)
- Fails if unmet (affects stability)

No new consumer logic is added.

---

## 5. Transport Model (Plug-In)

Water transport differs only in parameters:
- Loss: leakage per distance
- Fee: pumping energy translated to cost

Resolver still selects:
> **Lowest delivered total cost**

---

## 6. Market Behavior (Identical)

- AI Global Market supplies Water at premium
- Player surplus lists on Global Exchange
- New universes import cheapest offer

No special cases.

---

## 7. Plot Impact Rules

- Water is marked `critical = true`
- Chronic shortage:
  - stability decay
  - Slum → Abandoned

Reuse existing thresholds.

---

## 8. Validation Checklist

Before Water is “done”:

- [ ] No new resolver logic added
- [ ] No Unity-side economics
- [ ] Ledger reconciles Water like Power
- [ ] AI Water supply is displaceable
- [ ] Scenario test proves:
  - Dependency → Substitution → Export

---

## 9. Success Criteria

If Water:
- Can bankrupt a careless player
- Can be monopolized by a smart one
- Can replace AI supply for new universes

Then the vertical is complete.

---

## 10. Repeat for All Future Resources

Power → Water → Food → Steel → Automotive

Same pattern.  
Same systems.  
Different data.

That is the entire game.
