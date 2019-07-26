using System;

public partial class Tile : IChangeable<Tile>
{
    //событие для уведомления об изменениях в тайле
    private Action<Tile> cbTileTypeChanged;

    public void RegisterOnChanged(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UnRegisterOnChanged(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public void InvokeOnChanged()
    {
        if (cbTileTypeChanged != null) {
            cbTileTypeChanged(this);
        }
    }
}
