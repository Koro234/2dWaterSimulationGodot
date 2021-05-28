# First use git. 2d water simulation with cellular automaton in Godot(C#)

Simulation is based on the logic of cellular automata, added state system.

LCM - Water, RCM - Block.

You can improve the speed of work by moving the UpdateMap from PhysicsProcess to Process.

Class WaterCell works stably.
Class WaterCellGravity does not work fine. The axis of gravity is confused, it can be corrected by a sequence of calculations in a cycle (for) or in a collection of cell neighbors.
The physics of the process for class WaterCellGravity was taken from this code: https://github.com/tterrasson/liquid-simulator-godot
