using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TextFileEnumerator
{
    private string[] directories;

    public TextFileEnumerator(string[] directories)
    {
        this.directories = directories;
    }

    public IEnumerator<string> GetEnumerator()
    {
        List<string> files_list = new List<string>();
        string[] files = null;

        // поиск файлов текстовых вайлов в указанных директориях
        for (int i = 0; i < directories.Length; ++i) {
            files = System.IO.Directory.GetFiles(Application.streamingAssetsPath + directories[i],
            "*.txt", SearchOption.TopDirectoryOnly);
            if (files != null) files_list.AddRange(files);
        }

        if (files_list.Count == 0) yield break;

        for (int i = 0; i < files_list.Count; ++i) {
            Debug.LogFormat("Recipe file={0}", files_list[i]);
            string json = File.ReadAllText(files_list[i]);

            Debug.LogFormat("json={0}", json);

            yield return json;
        }
    }
}
