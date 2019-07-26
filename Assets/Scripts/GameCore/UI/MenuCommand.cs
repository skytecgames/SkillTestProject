using UnityEngine;
using System.Collections;

public class MenuCommand
{
    public event System.Action cbExecute;
    public event System.Action cbUndo;

    public void Execute()
    {
        if(cbExecute != null) {
            cbExecute();
        }
    }

    public void Undo()
    {
        if(cbUndo != null) {
            cbUndo();
        }
    }
}
