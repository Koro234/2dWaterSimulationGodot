using Godot;
using System;

public class Fluid_sim : Godot.Node2D
{
    private TileMap _tileMap;
    private Button _gravity;
    [Export]
    public int gridX = 128;
    [Export]
    public int gridY = 75;
    [Export]
    private float timeTicRate = 0.008f;
    private float timePassed = 0.0f;
    private bool StopTic = false;
    private bool GravityOnOff = false;
    private Cell[,] _cellBuffer;

    public override void _Ready()
    {
        _tileMap = GetNode<TileMap>("TileMap");
        _gravity = GetNode<Button>("Button");
        _cellBuffer = new Cell[gridX, gridY];
        CellCreater();
        SetNeighbors();
    }
    private void Inject()
    {
        var tempCell = (Cell)_cellBuffer[(int)(GetGlobalMousePosition().x/8), (int)(GetGlobalMousePosition().y/8)];
        if (Input.IsMouseButtonPressed(1))
        {
            if (GravityOnOff == true)
            {
                tempCell.State = new WaterCellGravity();
                var wc = tempCell.State as WaterCellGravity; 
                wc.fluidQuantity += 10f;
            }
            else
            {
                tempCell.State = new WaterCell();
                var wc = tempCell.State as WaterCell; 
                wc.fluidQuantity += 10f;
            }
        }
        else if (Input.IsMouseButtonPressed(2))
        {
            tempCell.State = new BlockCell();
        }
    }
    public override void _PhysicsProcess(float delta)
    {
        Inject();
        if (StopTic == true)
        {
            return;
        }
        timePassed += delta;
        if (timePassed > timeTicRate)
        {
            UpdateSimulation();
            timePassed = 0f;
        }
    }
    private void UpdateSimulation()
    {
        for (int i = 0; i < gridX; i++)
        {
            for (int j = gridY - 1; j >= 0; j--)
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
                Cell c = new Cell(new EmptyCell());
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
        package[0] = _cellBuffer[x, y + 1];
        package[1] = _cellBuffer[x + 1, y];
        package[2] = _cellBuffer[x - 1, y];
        package[3] = _cellBuffer[x, y - 1];
        return package;
    }
    private void _on_GravityButton_pressed()
    {
        StopTic = true;
        GravityOnOff =! GravityOnOff;
        if (GravityOnOff == true)
        {
            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    var tempCell = _cellBuffer[i, j].State;
                    if (tempCell is WaterCell)
                    {
                        var tempWC = tempCell as WaterCell;
                        var tempFluid = tempWC.fluidQuantity;
                        _cellBuffer[i, j].State = new WaterCellGravity();
                        var newCell = _cellBuffer[i, j].State as WaterCellGravity;
                        newCell.fluidQuantity = tempFluid;
                    }
                }
            }
            _gravity.Text = "true";
        }
        else
        {
            for (int i = 0; i < gridX; i++)
            {
                for (int j = 0; j < gridY; j++)
                {
                    var tempCell = _cellBuffer[i, j].State;
                    if (tempCell is WaterCellGravity)
                    {
                        var tempWC = tempCell as WaterCellGravity;
                        var tempFluid = tempWC.fluidQuantity;
                        _cellBuffer[i, j].State = new WaterCell();
                        var newCell = _cellBuffer[i, j].State as WaterCell;
                        newCell.fluidQuantity = tempFluid;
                    }
                }
            }
            _gravity.Text = "false";
        }
        StopTic = false; 
        
    }
}
