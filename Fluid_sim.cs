using Godot;
using System;

public class Fluid_sim : Godot.Node
{
    private TileMap _tileMap;
    [Export]
    public int gridX = 128;
    [Export]
    public int gridY = 128;
    [Export]
    private float timeTicRate = 0.08f;
    private float timePassed = 0.0f;
    private Cell[,] _cellBuffer;

    public override void _Ready()
    {
        
    }


}
