using Godot;
using System;

public class Fluid_sim : Godot.Node2D
{
    private TileMap _tileMap;
    [Export]
    public int gridX = 128;
    [Export]
    public int gridY = 75;
    [Export]
    private float timeTicRate = 0.08f;
    private float timePassed = 0.0f;
    private Cell[,] _cellBuffer;

    public override void _Ready()
    {
        _tileMap = GetNode<TileMap>("TileMap");
        _cellBuffer = new Cell[gridX, gridY];
        CellCreater();
        SetNeighbors();
    }
    private void Inject()
    {
        if (Input.IsMouseButtonPressed(1))
        {
            var waterCell = (Cell)_cellBuffer[(int)(GetGlobalMousePosition().x/8), (int)(GetGlobalMousePosition().y/8)];
            if (waterCell.State is EmptyCell)
            {
                waterCell.State = new WaterCell();
            }
            var wc = waterCell.State as WaterCell; 
            wc.fluidQuantity += 10f;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        Inject();
        timePassed += delta;
        if (timePassed > timeTicRate)
        {
            UpdateSimulation();
            UpdateSimulation();
            UpdateSimulation();
            UpdateSimulation();
            timePassed = 0f;
        }
    }

    private void UpdateSimulation()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                _cellBuffer[i, j].Calculation();
                UpdateMap(_cellBuffer[i, j]);
            }
        }
    }
    private void UpdateMap(Cell c)
    {
        _tileMap.SetCell(c.x, c.y, c.TileNum);
    }
    private void CellCreater()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                Cell c = new Cell(new WaterCell());
                c.x = i;
                c.y = j;
                _cellBuffer[i,j] = c;
            }
        }
    }
    private void SetNeighbors()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                if (i > 0 && i < (gridX - 1) && j > 0 && j < (gridY-1))
                {
                    _cellBuffer[i, j].neighbours = PackagingNeighbours(i, j);
                }
            }
        }
    }
    private Cell[] PackagingNeighbours(int x, int y)
    {
        Cell[] package = new Cell[4];
        package[0] = _cellBuffer[x, y - 1];
        package[1] = _cellBuffer[x + 1, y];
        package[2] = _cellBuffer[x, y + 1];
        package[3] = _cellBuffer[x - 1, y];

        return package;
    } 
}
