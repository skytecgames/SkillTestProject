using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding.ZStar
{
    public class NodeCollection_Zone : IPathNodeCollection<Zone>
    {
        private Dictionary<Zone, Node_Zone> zonemap;

        public NodeCollection_Zone(Dictionary<Zone, Node_Zone> zonemap)
        {
            this.zonemap = zonemap;
        }

        public IPathNode<Zone> GetNode(Zone data)
        {
            return zonemap[data];
        }
    }
}