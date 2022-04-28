using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Avalanche.Message;
using Avalanche.Utilities;
using static System.Console;

class userdatacontainer
{
    public static void Run()
    {
        {
            IUserDataContainer container = new MyClass0();
            container.UserData = new ConcurrentDictionary<string, object?>();
            container.UserData["Hello"] = "World";
            WriteLine(container.UserData["Hello"]); // "World"
        }
        {
            IUserDataContainer container = new MyClass1();
            container.UserData["Hello"] = "World";
            WriteLine(container.UserData["Hello"]); // "World"
        }
        {
            IUserDataContainer container = new MyClass1().SetUserData("Hello", "World");
            WriteLine(container.UserData["Hello"]); // "World"
        }

        {
            IUserDataContainer container = new MyClass2().SetUserData("Hello", "World").SetReadOnly();
            WriteLine(container.UserData["Hello"]); // "World"
        }
        {
            MyClass2 container = new MyClass2();
            IDictionary<string, object?> data = container.UserData;
            container.SetReadOnly();
            try
            {
                data["Hello"] = "World";
            } catch (InvalidOperationException e)
            {
                WriteLine(e);
            }
        }
        {
            try
            {
                IUserDataContainer container = new MyClass2().SetReadOnly().SetUserData("Hello", "World");
            } catch (InvalidOperationException e)
            {
                WriteLine(e);
            }
        }
    }

    public class MyClass0 : UserDataContainerBase { }
    public class MyClass1 : UserDataContainerBase.LazyConstructed { }
    public class MyClass2 : ReadOnlyAssignableClass.UserDataContainerLazyConstructed { }
}

