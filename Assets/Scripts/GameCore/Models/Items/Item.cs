using UnityEngine;
using System.Collections;

public class Item
{
    //Тип предмета
    public string objectType;

    //Размеры кучи предметов    
    public int stackSize;    

    //максимальное количество предметов в куче
    public int maxStackSize;        

    public Item()
    {
    }

    public Item(string objectType, int stackSize, int maxStackSize)
    {
        this.objectType = objectType;
        this.stackSize = stackSize;
        this.maxStackSize = maxStackSize;
    }

    //конструктор копии
    protected Item(Item other)
    {
        this.objectType = other.objectType;

        this.stackSize = other.stackSize;
        this.maxStackSize = other.maxStackSize;
    }

    //создать копию предмета
    public Item Clone()
    {
        return new Item(this);
    }    
}
