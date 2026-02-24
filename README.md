# 🧠 Auto Battler Strategy Prototype

A shop-based auto battler prototype built in Unity 2022.3.21f1.

This project combines strategic team building, merge-based leveling, tier progression, and automated combat driven by creature attributes and unique abilities.

---

## 📌 Overview

This prototype focuses on strategic composition through a dynamic shop system and automated combat resolution.

Each creature has:

- Base stats (Attack, Defense, Intelligence)
- Level progression via merging
- A unique ability that influences combat or team dynamics

Players must build a team before combat begins, then watch automated battles unfold based on stats, decision timing, and abilities.

---

## 🛠 Engine Version

- Unity 2022.3.21f1 (LTS)
- C#
- ScriptableObjects for data-driven design

---

## ⚙️ Core Systems

### 🛒 Shop System

- Randomized creature pool
- Tier-based progression per turn
- Refresh mechanic
- Slot freezing system
- Purchase & sell logic
- Merge-based leveling
- Experience tracking per creature

As turns advance, higher-tier creatures become available.

---

### 🧬 Creature System

Each creature contains:

- Attack
- Defense
- Intelligence (decision speed)
- Level
- Experience
- Unique Ability

Creatures are defined using ScriptableObjects to ensure scalability and easy expansion.

---

### ⭐ Unique Ability System

Each creature has a distinct ability that can trigger based on specific conditions, such as:

- On battle start
- On attack
- On taking damage
- On elimination
- On ally interaction
- On level up

The ability system is designed to be modular and extensible, allowing new behaviors to be added without modifying core combat logic.

This creates strategic depth and synergy potential between creatures.

---

### 🧠 Intelligence-Based Decision System

Intelligence determines how quickly a creature acts during battle.

When a creature's decision timer completes, it chooses between:

- Attack 1
- Attack 2
- Dodge (forward or backward reposition)

This creates asynchronous combat flow rather than rigid turn order.

---

### ⚔️ Combat System

- Creatures move freely on the board
- Automated decision-based combat
- Attack vs Defense damage calculation
- Position shifting via dodge actions
- Ability triggers integrated into the combat loop

Combat is deterministic in logic but dynamic due to timing differences and ability interactions.

---

## 🔄 Turn Progression & Scaling

- Shop tier increases over turns
- Higher-tier creatures introduce stronger stats and more impactful abilities
- Merge system increases creature power and can amplify ability effects

This creates a natural difficulty and strategy curve.

---

## 🔍 QA-Oriented Testing Notes

The following scenarios were tested:

- Merge logic correctness
- Ability trigger validation
- Ability stacking edge cases
- Selling creatures with active effects
- Shop freeze persistence across refresh
- Intelligence timing conflicts
- Simultaneous ability activation
- Dodge boundary constraints
- Tier progression consistency

---

## 🚧 Pending Features

Planned but not yet implemented:

- Win/Loss tracking system
- Session end condition
- Full run lifecycle
- Dynamic opponent team assembly
- Variable enemy compositions
- Persistent progression system

Currently, the opponent team is static.

---

## 🧪 Performance Considerations

- Controlled shop randomization
- Data-driven creature and ability definitions
- Minimal runtime allocations
- Modular ability architecture
- Deterministic combat loop

---

## 🚀 Possible Improvements

- Trait/synergy system between creatures
- Procedural enemy generation
- Ability animation feedback
- Combat log system
- Replay system
- Save system
- AI-driven enemy drafting

---

## 📂 Project Structure
```
Assets/
 ├── Scripts/
 │    ├── Shop/
 │    ├── Creatures/
 │    ├── Abilities/
 │    ├── Combat/
 │    ├── Systems/
 │    └── UI/
 ├── ScriptableObjects/
 ├── Prefabs/
 ├── Scenes/
ProjectSettings/
Packages/
```
---

## 🎯 Learning Goals

This project explores:

- Shop economy systems
- Merge-based leveling mechanics
- Ability-driven gameplay design
- AI decision timing systems
- Data-driven architecture
- Scalable combat systems
- Strategic composition mechanics

---

## 📜 License

This project is for educational and portfolio purposes.