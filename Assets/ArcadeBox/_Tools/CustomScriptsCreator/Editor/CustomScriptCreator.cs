/*
*  Author: Rutvij
*  Created On: 17-05-2021 23:54:20
* 
*  Copyright (c) 2021 ArcadeBox
*/

using System.IO;
using UnityEngine;
using UnityEditor;

namespace ArcadeBox.Core
{
    public class CreateCSharpScripts
    {
        private const string TEMPLATE_PATH = "Assets/ArcadeBox/_Tools/CustomScriptsCreator/Editor/Templates/";
        private const string MONO_SCRIPT_TEMPLATE = "CSharpMonobehaviourScriptTemplate.cs.txt";
        private const string CLASS_SCRIPT_TEMPLATE = "CSharpScriptTemplate.cs.txt";
        private const string SINGLETON_SCRIPT_TEMPLATE = "CSharpSingletonScriptTemplate.cs.txt";
        private const string EDITOR_SCRIPT_TEMPLATE = "CSharpEditorWindowScriptTemplate.cs.txt";
        private const string STATIC_SCRIPT_TEMPLATE = "CSharpStaticScriptTemplate.cs.txt";
        private const string JSON_TEMPLATE = "templateJSON.json.txt";
        private const string SO_TEMPLATE = "CSharpScriptableObjectsTemplate.cs.txt";
        private const string INTERFACE_TEMPLATE = "InterfaceTemplate.cs.txt";


        [MenuItem(itemName: "Assets/Create/Scripts/New C# Monobehaviour Script", isValidateFunction: false, priority: 51)]
        public static void CreateMonobehaviourFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + MONO_SCRIPT_TEMPLATE, "NewCSharpMonobehaviourScript.cs");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New C# Class Script", isValidateFunction: false, priority: 52)]
        public static void CreateClassFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + CLASS_SCRIPT_TEMPLATE, "NewCSharpScript.cs");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New C# Singleton Script", isValidateFunction: false, priority: 53)]
        public static void CreateSingletonFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + SINGLETON_SCRIPT_TEMPLATE, "NewCSharpSingletonScript.cs");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New C# Editor Script", isValidateFunction: false, priority: 54)]
        public static void CreateEditorScriptFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + EDITOR_SCRIPT_TEMPLATE, "NewCSharpEditorWindowScript.cs");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New C# Static Script", isValidateFunction: false, priority: 55)]
        public static void CreateStaticScriptFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + STATIC_SCRIPT_TEMPLATE, "NewCSharpStaticScript.cs");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New JSON", isValidateFunction: false, priority: 56)]
        public static void CreateJSONFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + JSON_TEMPLATE, "NewJSON.json");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New Scriptable Object", isValidateFunction: false, priority: 57)]
        public static void CreateScriptableObjectFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + SO_TEMPLATE, "NewScriptableObject.cs");
        }

        [MenuItem(itemName: "Assets/Create/Scripts/New Interface", isValidateFunction: false, priority: 58)]
        public static void CreateInterfaceFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(TEMPLATE_PATH + INTERFACE_TEMPLATE, "NewInterface.cs");
        }
    }


    public class ScriptGenerationHelper : UnityEditor.AssetModificationProcessor
    {
        private const string AUTHOR_NAME = "#AUTHOR#";
        private const string COMPANY_NAME = "#COMPANYNAME#";
        private const string DATE_NAME = "#CREATIONDATE#";
        private const string PROJECT_NAME = "#PROJECTNAME#";
        private const string YEAR = "#YEAR#";

        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            int index = path.LastIndexOf(".");
            if (index < 0)
                return;

            string fileExtension = path.Substring(index);
            if (fileExtension != ".cs" && fileExtension != ".json")
                return;

            index = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, index) + path;
            if (File.Exists(path))
            {
                string file = System.IO.File.ReadAllText(path);

                if(fileExtension == ".cs")
                {
                    file = file.Replace(COMPANY_NAME, PlayerSettings.companyName.Replace(" ", ""));
                    file = file.Replace(AUTHOR_NAME, System.Environment.UserName + "");
                    file = file.Replace(DATE_NAME, System.DateTime.Now + "");
                    file = file.Replace(YEAR, System.DateTime.Now.Year + "");
                    file = file.Replace(PROJECT_NAME, PlayerSettings.productName.Replace(" ",""));
                }
                //file = file.Replace("#SMARTDEVELOPERS#", PlayerSettings.companyName);

                System.IO.File.WriteAllText(path, file);
                AssetDatabase.Refresh();
            }
        }
    }
}
