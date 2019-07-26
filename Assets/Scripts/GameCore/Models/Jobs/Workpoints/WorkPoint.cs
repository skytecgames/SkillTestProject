using UnityEngine;
using System.Collections;

//Базовый класс для точки проведения работы
//хранит материалы для работы, количество работы до завершения
//TOTHINK: этот же компонент должен уведомлять о своем состоянии JobSpriteController чтобы его кореектно визуализировать
public class WorkPoint
{
    //TOFIX: работу можно проводить как непосредственно на точке так и в радиусе от нее или в определенной области

    //Место проведения работ (куда должен пойти персонаж для совершения работы)
    public Tile tile;

    //Место спавна для продукции (где появится результат работы)
    public Tile spawnPoint;

    //Количество оставшенейся работы до завершения
    public float workAmount;

    //контейнер для материалов
    public MaterialContainer materials;

    //Событие - уведомление о завершении работы
    public event System.Action<WorkPoint> cbWorkComplete;

    //уведомление об изменении в работе
    public event System.Action<WorkPoint> cbWorkChanged;
    public event System.Action<WorkPoint> cbWorkStart;
    public event System.Action<WorkPoint> cbWorkCanceled;

    //Расчитать 
    public void DoWork(Character character, float deltaTime)
    {
        if(materials.HasAllMaterials() == false) {
            Debug.LogError("character applyWork to site without all materials!!!");
            return;
        }

        if(character.currTile != tile) {
            Debug.LogError("character try to work from distance");
            return;
        }

        //TOFIX: в будующем тут должна быть зависимость от характеристик персонажа
        workAmount -= deltaTime;

        //Уведомляем в случае если работа завершена
        if(workAmount <= 0) {
            Complete();
        } else {
            Change();
        }
    }    

    //функция добавления материала для работы
    public void AddMaterial(Item item)
    {        
        materials.AddMaterial(item);

        //TODO: нужно ли здесь следить за тем что предмет стал пустым (полностью потрачен на материалы)

        //уведомляем об изменении точки проведения работ 
        //(чтобы мы могли анимировать добавление материалов там где это нужно)
        Change();
    }

    //Запуск работы
    public void Start()
    {
        if (cbWorkStart != null) {
            cbWorkStart(this);
        }
    }

    //разрушает точку работы освобождая все материалы    
    public void Cancel()
    {
        if(cbWorkCanceled != null) {
            cbWorkCanceled(this);
        }
    }

    //Уведомляем о завершении работы
    private void Complete()
    {        
        //выставляем количество работы в 0
        workAmount = 0;

        //Тратим материалы
        materials.Spend();

        //уведомляем о завершении производства
        if(cbWorkComplete != null) {
            cbWorkComplete(this);
        }
    }

    //Уведомляем об изменении работы
    private void Change()
    {
        if(cbWorkChanged != null) {
            cbWorkChanged(this);
        }
    }
}
