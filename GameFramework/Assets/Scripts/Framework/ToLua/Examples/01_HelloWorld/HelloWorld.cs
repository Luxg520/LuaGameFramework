using UnityEngine;
using LuaInterface;
using System;

public class HelloWorld : MonoBehaviour
{
    void Awake()
    {
        LuaState lua = new LuaState();
        lua.Start();
        string hello =
            @"                
                print('hello tolua#')                                  
            ";
        
        lua.DoString(hello, "HelloWorld.cs");

		string test = 
			@"
				print('lua test');
			";

		lua.DoString(test,"test.cs");

        lua.CheckTop();
        lua.Dispose();
        lua = null;
    }
}
