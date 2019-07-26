//механика LinkToNeighbour для констркутов
public partial class InstalledObject 
{
    public void LinkToNeighbour()
    {
        if (this.parameters.GetFloat(ParameterName.links_to_neighbour) == 1f) {
            Tile t;
            int x = this.tile.x;
            int y = this.tile.y;

            //TOFIX: переделать на x+dx, y+dy в цикле

            t = World.current[x, y + 1];
            if (t != null && t.installedObject != null && t.installedObject.objectType == this.objectType) {
                t.installedObject.InvokeOnChanged();
            }

            t = World.current[x + 1, y];
            if (t != null && t.installedObject != null && t.installedObject.objectType == this.objectType) {
                t.installedObject.InvokeOnChanged();
            }

            t = World.current[x, y - 1];
            if (t != null && t.installedObject != null && t.installedObject.objectType == this.objectType) {
                t.installedObject.InvokeOnChanged();
            }

            t = World.current[x - 1, y];
            if (t != null && t.installedObject != null && t.installedObject.objectType == this.objectType) {
                t.installedObject.InvokeOnChanged();
            }
        }
    }
}
