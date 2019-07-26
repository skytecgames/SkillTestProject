using UnityEngine;
using System.Collections;

namespace Pathfinding.ZStar
{
    public class NodeConnector_ZoneTile : INodeConnector<Tile, Zone>
    {
        Graph_ZoneData data;

        public NodeConnector_ZoneTile(Graph_ZoneData data)
        {
            this.data = data;
        }

        public Zone GetNode(Tile t)
        {
            return data.tilemap[t.x, t.y].zone;
        }
    }
}