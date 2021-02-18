using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronRuby;
using System;
using Microsoft.Scripting;
using IronRuby.Builtins;

namespace VirtualPhenix.IrnRuby
{
    public class VP_IronRubyTest : MonoBehaviour
    {
        [SerializeField] string m_scriptFolder;
        [SerializeField] string m_scriptPath = "";

        void Reset()
        {
            m_scriptFolder = Application.dataPath + "/Ruby/Scripts/";
            m_scriptPath = m_scriptFolder + "Test.rb";
        }

        // Start is called before the first frame update
        void Start()
        {
            m_scriptFolder = Application.dataPath + "/Ruby/Scripts/";
            m_scriptPath = m_scriptFolder + "CrimsonMarch_PDA.rb";

            CallClassMethod();
        }

        public void CallClassMethodWithAll()
        {
            var engine = IronRuby.Ruby.CreateEngine();
            var scope = engine.CreateScope();

            var scripts = System.IO.Directory.GetFiles(m_scriptFolder);
            foreach (var s in scripts)
            {
                try
                {
                    var src = engine.CreateScriptSourceFromFile(s);
                    var compiled = src.Compile();
                    compiled.Execute(scope);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }

            dynamic ruby = engine.Runtime.Globals;
            dynamic script = ruby.PokeDispositivoAvanzado.@new();
            Debug.Log(script.pbStartScene2());
        }

        public void CallClassMethod()
        {
            var engine = IronRuby.Ruby.CreateEngine();
            var scope = engine.CreateScope();

            var src = engine.CreateScriptSourceFromFile(m_scriptPath);
            var compiled = src.Compile();
            compiled.Execute(scope);

            dynamic ruby = engine.Runtime.Globals;
            dynamic script = ruby.PokeDispositivoAvanzado.@new();
            Debug.Log(script.pbStartScene2());
        }

        public void CheckParameters()
        {
            var engine = IronRuby.Ruby.CreateEngine();
            var scope = engine.CreateScope();
            var src = engine.CreateScriptSourceFromString(System.IO.File.ReadAllText(m_scriptPath), SourceCodeKind.Statements);
            var compiled = src.Compile();
            compiled.Execute(scope);

            object rubyClass;
            if (engine.Runtime.Globals.TryGetVariable("Test", out rubyClass))
            {
                RubyClass instance = engine.Runtime.Globals.GetVariable("Test");

                var method = instance.GetMethod("pbPlus");
                var param = method.GetRubyParameterArray();

                if (param.Count == 2)
                {
                    // OK, try to execute (see below)
                    Debug.Log("H");
                }
            }

        }

        public void CallNonClassMethod()
        {
            try
            {
                var ironRubyRuntime = Ruby.CreateRuntime();
                dynamic loadIRuby = ironRubyRuntime.UseFile(m_scriptPath);
                Debug.Log("Sum" + loadIRuby.add(100, 200));
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.Log(ex.Message);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

