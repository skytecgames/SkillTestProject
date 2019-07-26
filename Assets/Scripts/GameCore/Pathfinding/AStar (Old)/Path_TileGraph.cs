using UnityEngine;
using System.Collections.Generic;

public class Path_TileGraph
{
    public Dictionary<Tile, Path_Node<Tile>> nodes;

    public Path_TileGraph(World world)
    {
        nodes = new Dictionary<Tile, Path_Node<Tile>>();

        //Создаем ноды
        for(int x = 0; x < world.width; ++x) {
            for(int y = 0; y < world.height; ++y) {
                Tile t = world[x, y];

                //Создаем ноды только для проходимых тайлов
                if(t.movementCost < Tile.walkabilityMax) {
                    Path_Node<Tile> n = new Path_Node<Tile>();
                    n.data = t;
                    
                    nodes.Add(t, n);
                }                
            }
        }

        //Создаем грани
        foreach (Tile t in nodes.Keys)
        {          
            CalculateEdgesForTileNode(t);
        }
    }

    public void AddTileToGraph(Tile t)
    {
        if(nodes == null) {
            Debug.LogError("AddTileToGraph error: empty nodes map");
            return;
        }

        if (nodes.ContainsKey(t) == false) {
            Path_Node<Tile> n = new Path_Node<Tile>();
            n.data = t;

            nodes.Add(t, n);

            CalculateEdgesForTileNode(t);
        }
    }

    void CalculateEdgesForTileNode(Tile t)
    {
        Path_Node<Tile> n = nodes[t];

        Tile[] neighbours = t.GetNeighbours(true);
        List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>>();

        for (int i = 0; i < neighbours.Length; ++i)
        {
            if (neighbours[i] != null && neighbours[i].movementCost < Tile.walkabilityMax)
            {
                if(IsClippingCorner(t, neighbours[i])) {
                    continue;
                }

                Path_Edge<Tile> edge = new Path_Edge<Tile>();
                edge.cost = neighbours[i].movementCost;
                edge.node = nodes[neighbours[i]];

                edges.Add(edge);
            }
        }

        n.edges = edges.ToArray();
    }

    bool IsClippingCorner(Tile curr, Tile neighbour)
    {
        //если сосед по диагонали
        if(Mathf.Abs(curr.x - neighbour.x) + Mathf.Abs(curr.y - neighbour.y) == 2) {
            int dX = curr.x - neighbour.x;
            int dY = curr.y - neighbour.y;

            if(World.current[curr.x - dX, curr.y].movementCost >= Tile.walkabilityMax) {
                return true;
            }

            if (World.current[curr.x, curr.y - dY].movementCost >= Tile.walkabilityMax) {
                return true;
            }
        }

        return false;
    }
}
