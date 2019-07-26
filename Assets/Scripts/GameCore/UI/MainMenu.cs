using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject prefab;

    private List<MenuCommand> commands = new List<MenuCommand>();
    private List<MenuCommand> history = new List<MenuCommand>();

    private void Start()
    {
        //----- Добавляем кнопку меню строительства -----
        BuildMenu build_menu = GameObject.FindObjectOfType<BuildMenu>();
        if(build_menu == null) {
            Debug.LogError("BuildMenu object not found");
        }
        AddCommand("Build", build_menu.Show, build_menu.Hide);
    }

    //Добавялем кнопку совершающую указанные действия
    private void AddCommand(string text, System.Action execute, System.Action undo)
    {
        //Настраиваем команду
        MenuCommand com = new MenuCommand();
        com.cbExecute += execute;
        com.cbUndo += undo;
        commands.Add(com);

        //Добавляем кнопку
        GameObject go = Instantiate(prefab, this.gameObject.transform);
        go.name = string.Format("Command_{0}", text);
        Text txt = go.GetComponentInChildren<Text>();
        txt.text = text;
                
        go.GetComponent<Button>().onClick.AddListener(() => { ExecuteCommand(com); });

        //Вызываем отмену команды, чтобы скрыть связанный с ней экран
        com.Undo();
    }

    //Выполняет указанную команду
    private void ExecuteCommand(MenuCommand command)
    {
        bool need_exec = true;

        //Отменяем все команды в списке истории
        if(history.Count > 0) {            
            for(int i=0;i<history.Count;++i) {                
                history[i].Undo();                

                if(history[i] == command) {
                    need_exec = false;
                }
            }            

            history.Clear();
        }

        //Если нужно, выполняем команду
        if (need_exec) {
            history.Add(command);
            command.Execute();
        }
    }
}
