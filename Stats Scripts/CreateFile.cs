using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CreateFile : MonoBehaviour
{
    [SerializeField] private string _filePath;
    [SerializeField] private string _scriptName;
    [SerializeField] private string _filler;
   
    [ExecuteInEditMode]
    [ContextMenu("Create Script")]
    public void CreateScript()
    {
        string savePath = Application.dataPath + _filePath + _scriptName + ".cs";
        using (StreamWriter newWriter = new StreamWriter( savePath, false))
        { 
            //The rest of this writes what you want your file to contain
            newWriter.WriteLine("using UnityEngine;");

            //This is an empty line because I like having spaces :D
            newWriter.WriteLine();
            newWriter.WriteLine("public class " + _scriptName + " : MonoBehaviour");
            newWriter.Write("{");
            newWriter.WriteLine("// this is a new script");
            newWriter.WriteLine($"{_filler}");
            newWriter.WriteLine("}");
        }
        print($"file saved at {savePath}");
    }
}
