# Gate of Tribute

A compact medieval logistics puzzle game about building production networks that deliver the right goods to the city gate.

Gate of Tribute was designed and developed as a solo project in 10 days for the [GameDev.tv Game Jam 2026](https://itch.io/jam/gamedevtv-jam-2026). The project focuses on systemic problem-solving: players place buildings and roads, establish supply chains, and diagnose bottlenecks until the required tribute reaches the gate.

[Play the Windows build on itch.io](https://emberlinestudios.itch.io/gate-of-tribute)

## Project Overview

- **Role:** Solo developer
- **Development period:** 10 days
- **Platform:** Windows
- **Engine:** Unreal Engine 5
- **Primary language:** C++
- **Genre:** Logistics / city-building puzzle
- **Jam:** GameDev.tv Game Jam 2026

## Gameplay

Each level presents a compact space, a limited set of building nodes, and a specific tribute required by the gate. Players must construct a working logistics network that produces and transports every requested resource.

The production chains include resources such as planks, stone blocks, food, ingots, rum, tools, and trade goods. Completing a level requires more than placing the correct buildings: roads and connections must allow workers to move resources through the complete supply chain without creating bottlenecks.

There is no combat or countdown timer. The challenge comes from understanding the system, using the available space efficiently, and debugging the network when goods do not reach their destination.

## Key Systems Implemented

- Grid-based placement and deletion of buildings and roads
- Placement validation and continuous road construction
- Node inputs, outputs, and production requirements
- Multi-stage resource-production chains
- Connection-driven resource routing
- Worker hauling between connected nodes
- Gate requirements and level-completion logic
- Hover-based inspection of node production, requirements, and connections
- Build-mode, delete-mode, and contextual user-interface states
- Level progression and player feedback

## Technical Focus

The project was deliberately scoped around a small number of interacting systems that could be completed and tested within the jam deadline. The main engineering challenge was making the logistics network understandable to the player while keeping the underlying placement, connection, production, and delivery rules consistent.

Particular attention was given to:

- Keeping placement and connection behaviour predictable
- Providing enough visual feedback to diagnose broken supply chains
- Separating node responsibilities so that different producers and processors could share common behaviour
- Supporting rapid iteration on resource requirements and level layouts
- Prioritising a complete, playable build over features that could not be finished reliably within the time limit

## Controls

- Use the **Build** menu to select buildings or roads.
- Click a valid tile to place the selected item.
- Hold **Shift** to place multiple items of the same type.
- Hold and drag while placing roads to extend them continuously.
- Drag from a source node, across connected road tiles, to a target node to create a connection.
- Hover over a node to inspect its inputs, outputs, and existing connections.
- Use **Delete** mode to remove buildings or road tiles.

## Running the Project from Source

This is a source-focused portfolio repository rather than a byte-for-byte copy of the original jam project. Large and non-essential assets are intentionally omitted, so the repository may not reproduce the published build exactly. The complete playable Windows build remains available on [itch.io](https://emberlinestudios.itch.io/gate-of-tribute).

1. Clone the repository:

   ```bash
   git clone https://github.com/lukasz-dziedziczak/GDTV_GameJam2026_Connections.git
   ```

2. Install the Unreal Engine 5 version associated with the `.uproject` file.
3. Install Visual Studio 2022 with the **Game development with C++** workload.
4. Open the `.uproject` file. If prompted, allow Unreal Engine to rebuild the project modules.
5. Open the project in the Unreal Editor and run the main game map.

If Unreal cannot build the modules automatically, generate Visual Studio project files from the `.uproject`, open the generated solution, and build the Editor target in the **Development Editor** configuration. Missing asset references may appear where large or third-party content has been excluded; these do not affect review of the original gameplay source code.

## Repository Scope

This repository is intended to present the original gameplay code and project structure created for the jam while keeping the download practical and respecting third-party licences.

The repository focuses on:

- Original C++ gameplay source
- Unreal project and configuration files
- Small project-owned assets required to understand the implementation, where practical

The following are intentionally excluded:

- Large `.wav` files and other high-resolution audio
- Large graphics and other non-essential binary content
- Unused marketplace or asset-store packs imported during the initial jam setup
- Third-party models, animations, audio, fonts, and other assets that cannot be redistributed
- Generated Unreal Engine folders such as `Binaries`, `DerivedDataCache`, `Intermediate`, and `Saved`

The downloadable build may contain third-party assets obtained under their respective licences. Those assets are not covered by this repository's GPL licence and remain the property of their original creators.

## Development Context

Game jams reward disciplined scope, fast technical decision-making, and the ability to deliver a functioning product under a fixed deadline. Gate of Tribute represents the submitted project and the engineering decisions made within that constraint; it is not intended to be presented as a production-scale codebase.

## About the Developer

Gate of Tribute was created by [Łukasz Dziedziczak](https://emberlinestudios.com.au/), founder and solo developer at Emberline Studios.

- [Emberline Studios](https://emberlinestudios.com.au/)
- [Game portfolio on itch.io](https://emberlinestudios.itch.io/)
- [Gate of Tribute playable build](https://emberlinestudios.itch.io/gate-of-tribute)

## Licence

The original source code in this repository is licensed under the [GNU General Public License, version 2](https://www.gnu.org/licenses/gpl2.html) (`GPL-2.0-only`). See the repository's `LICENSE` file for the complete licence text.

Copyright © 2026 Łukasz Dziedziczak.

This licence applies only to original source code and other material for which Łukasz Dziedziczak owns the copyright. Third-party assets and components retain their respective licences and are not relicensed under the GPL.
