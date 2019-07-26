using System;

public partial class InstalledObject : IChangeable<InstalledObject>
{
    //событие - объект был изменен
    private Action<InstalledObject> cbOnChanged;

    public void InvokeOnChanged()
    {
        if(cbOnChanged != null) {
            cbOnChanged(this);
        }
    }

    public void RegisterOnChanged(Action<InstalledObject> callback)
    {
        cbOnChanged += callback;
    }

    public void UnRegisterOnChanged(Action<InstalledObject> callback)
    {
        cbOnChanged -= callback;
    }
}
