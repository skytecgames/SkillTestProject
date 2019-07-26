using UnityEngine;
using Priority_Queue;

//Класс для управления процессом роста конструкций
public class GrowModel : IProcess
{
    //контейнер с сортировкой для хранения информации о растущих объектах
    private SimplePriorityQueue<IParameters> pQueue;

    //TOFIX: перенести статический вызов в класс менеджер
    private static GrowModel instance = null;

    public GrowModel()
    {
        //инициализируем очередь
        pQueue = new SimplePriorityQueue<IParameters>();

        //создаем статическую ссылку на класс
        instance = this;
    }

    // В этой функции мы смотрим не нужно ли сменить объектам стадию роста.
    // Если нужно, то мы меняем параметры, после чего вызываем InstalledObject.cbOnChanged
    // для того, чтобы объект перерисовался.
    public void Update(float deltaTime)
    {
        //Если ничего не растет, то уходим сразу (оптимизация)
        if (pQueue.Count == 0) return;
        
        //Перебираем пока есть элементы завершившие рост
        while(pQueue.Count > 0 && pQueue.First.GetFloat(ParameterName.grow_stage_time) < GameTime.value) {

            //Обновляем параметры роста объекта и перерисовываем
            //TOTHINK: (проверить) тут вызовется метод UpdatePriority, но отработает он с O(log n), так как елемент для обновления на верху очереди
            pQueue.First.InvokeOnChanged();
        }
    }    

    public void RegisterObject(IParameters obj)
    {        
        if (instance.pQueue.Contains(obj)) {
            Debug.LogError("Error (Process_Grow): try register already registered object");
            return;
        }

        //Если объект может расти, добавляем его в очередь
        if (UpdateObjectParameters(obj)) {            
            obj.RegisterOnChanged(instance.UpdateObject);
            obj.RegisterOnRemoved(instance.UnRegisterObject);

            pQueue.Enqueue(obj, obj.GetFloat(ParameterName.grow_stage_time));
            obj.SetFloat(ParameterName.grow_is_active, 1f);
        }
    }

    //Вызывается при удалении объекта
    private void UnRegisterObject (IParameters obj)
    {
        //Отписываемся от событий конструкта
        obj.UnRegisterOnChanged(UpdateObject);
        obj.UnRegisterOnRemoved(UnRegisterObject);

        //Убираем конструкт из очереди, если он там присутствует
        if (pQueue.Contains(obj)) pQueue.Remove(obj);
    }   

    private void UpdateObject(IParameters obj)
    {
        //TOTHINK: Возможно стоит отказаться от подписки на событие изменения объекта
        //         Это удобно, но не эффективно. Обновлять объект в рамках процесса нужно только если сменились параметры роста        
        bool grow_is_active = obj.GetFloat(ParameterName.grow_is_active) == 1f;

        //Обновляем параметры роста объекта        
        if (UpdateObjectParameters(obj) == false) {
            //Если объект больше не растет
            if (obj.GetFloat(ParameterName.grow_is_active) == 1f) {
                obj.SetFloat(ParameterName.grow_is_active, 0);
                pQueue.Remove(obj);
            }
        } else {
            //Если объект все еще растет (возвращаем или обновляем время)
            if (obj.GetFloat(ParameterName.grow_is_active) == 0) {
                pQueue.Enqueue(obj, obj.GetFloat(ParameterName.grow_stage_time));
            } else {
                pQueue.UpdatePriority(obj, obj.GetFloat(ParameterName.grow_stage_time));
            }

            obj.SetFloat(ParameterName.grow_is_active, 1f);
        }
    }

    //Вызывается при изменении параметров объекта, влияющих на его рост 
    //Возвращает true, если конструкт находится в стадии роста
    private bool UpdateObjectParameters(IParameters obj)
    {
        bool is_growing = true;

        float time = obj.GetFloat(ParameterName.grow_stage_time);

        //Если не настало время сменить стадию роста, то просто уходим
        if (time > GameTime.value) return true;

        int stage = (int)obj.GetFloat(ParameterName.grow_stage);
        int stage_max = (int)obj.GetFloat(ParameterName.grow_stage_max);

        //Если объект в прошлом уже достиг последней стадии роста, уходим
        if (stage >= stage_max) {
            return false;
        }

        float speed = obj.GetFloat(ParameterName.grow_speed);

        //Debug.LogFormat("UpdateGrow IN gameTime={4}, time={0}, stage={1}, max={2}, speed={3}", 
        //    time, stage, stage_max, speed, GameTime.value);        

        //Если это только что созданный объект, то задаем ему время достижения следующей стадии роста
        if (time < 0) {
            time = GameTime.value + speed;
        }

        //Если пришло время смены стадии роста
        if(time <= GameTime.value && stage < stage_max) {
            stage++;
            time += speed;
        }

        //если достигли максимальной стадии роста, то прекращаем дальнейший рост
        if(stage >= stage_max) {
            time = -1;
            is_growing = false;
        }

        //Сохраняем изменения в параметрах
        obj.SetFloat(ParameterName.grow_stage_time, time);
        obj.SetFloat(ParameterName.grow_stage, stage);

        //Debug.LogFormat("UpdateGrow OUT gameTime={4}, time={0}, stage={1}, max={2}, speed={3}", 
        //    time, stage, stage_max, speed, GameTime.value);

        //Так как мы поменяли параметры объекта еще раз вызываем OnChanged
        obj.InvokeOnChanged();

        //Объект растет
        return is_growing;
    }
}
