using Godot;
using System;

public interface ICellState
{
    void Calculation(Cell cell);
}

public class Cell
{
    public int x {get; set;}
    public int y {get; set;}
    public int TileNum {get; set;} = 0;
    public Cell[] neighbours {get; set;}
    public ICellState State {get; set;}
    public Cell(ICellState cs)
    {
        State = cs;
    }
    public void Calculation()
    {
        State.Calculation(this);
    }

}
public class WaterCell : ICellState
{
    public float maxFluidValue = 1.0f;
    public float minFluidValue = 0.005f;
    public float fluidQuantity = 0.0f;
    public float maxFlowValue = 4.0f;
    public float minFlowValue = 0.005f;
    public float flowSpeed = 1.0f;
    public void Calculation(Cell cell)
    {
        cell.TileNum = (int)(fluidQuantity*10/8) + 1;
        if (cell.neighbours == null)
        {
            return;
        }
        if (fluidQuantity == 0.0f)
        {
            return;
        }
        if (fluidQuantity < minFluidValue)
        {
            fluidQuantity = 0.0f;
            return;
        }
        foreach (var neighbour in cell.neighbours)
        {
            if (neighbour.State is WaterCell)
            {
                var n = neighbour.State as WaterCell;
                float flow = (fluidQuantity - n.fluidQuantity)/4;
                if (flow > minFlowValue)
                {
                    flow *= flowSpeed;
                }
                flow = Math.Max(flow, 0.0f);
                if (flow > Math.Min(maxFlowValue, fluidQuantity))
                {
                    flow = Math.Min(maxFlowValue, fluidQuantity);
                }
                if (flow != 0.0f)
                {
                    n.fluidQuantity += flow;
                    fluidQuantity -= flow;
                }
            }
        }
    }
}
public class EmptyCell : ICellState
{
    public void Calculation(Cell cell)
    {
        return;
    }
}