using System;

public partial class InstalledObject : IRemoveable<InstalledObject>
{
    //событие - объект был удален
    private Action<InstalledObject> cbOnRemove;

    public void RegisterOnRemoved(Action<InstalledObject> callback)
    {
        cbOnRemove += callback;
    }

    public void UnRegisterOnRemoved(Action<InstalledObject> callback)
    {
        cbOnRemove -= callback;
    }
}