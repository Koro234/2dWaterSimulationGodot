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
            cell.State = new EmptyCell();
            return;
        }
        if (fluidQuantity < minFluidValue)
        {
            fluidQuantity = 0.0f;
            cell.State = new EmptyCell();
            return;
        }
        if (fluidQuantity < maxFluidValue)
        {
            return;
        }
        foreach (var neighbour in cell.neighbours)
        {
            if (neighbour.State is EmptyCell)
            {
                neighbour.State = new WaterCell();
            }
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
public class WaterCellGravity : ICellState
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
            cell.State = new EmptyCell();
            return;
        }
        if (fluidQuantity < minFluidValue)
        {
            fluidQuantity = 0.0f;
            cell.State = new EmptyCell();
            return;
        }
        for (int i = 0; i < cell.neighbours.Length; i++)
        {
            var neighbour = cell.neighbours[i];
            if (neighbour.State is EmptyCell)
            {
                neighbour.State = new WaterCellGravity();
            }
            if (neighbour.State is WaterCellGravity)
            {
                var n = neighbour.State as WaterCellGravity;
                float flow = 0f;
                if (i == 0)
                {
                    flow = VerticalFlow(fluidQuantity, n.fluidQuantity) - n.fluidQuantity;
                }
                else if (i == 2)
                {
                    flow = VerticalFlow(fluidQuantity, n.fluidQuantity);
                    
                }
                else
                {
                    flow = (fluidQuantity - n.fluidQuantity)/3;
                }
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
        if (fluidQuantity < maxFluidValue)
        {
            return;
        }
    }
    private float VerticalFlow(float sQuantity, float oQuantity)
    {
        float sum = sQuantity + oQuantity;
        float value = 0.0f;
        if (sum <= maxFluidValue)
        {
            value = maxFluidValue;
        }
        else if (sum < (2 * maxFluidValue + 0.25f))
        {
            value = (maxFluidValue * maxFluidValue + sum * 0.25f) / (maxFluidValue + 0.25f);
        }
        else
        {
            value = (sum + 0.25f) / 2.0f;
        }
        return value;
    }
}
public class EmptyCell : ICellState
{
    public void Calculation(Cell cell)
    {
        cell.TileNum = 1;
        return;
    }
}
public class BlockCell : ICellState
{
    public void Calculation(Cell cell)
    {
        cell.TileNum = 0;
        return;
    }
}