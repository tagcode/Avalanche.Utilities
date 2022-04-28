using System.Reflection;
using Avalanche.Utilities;
using Avalanche.Utilities.Record;

class parameterdescription
{
    public static void Run()
    {
        {
            IParameterDescription parameterDescription = new ParameterDescription
            {
                Name = "arg",
                Type = typeof(int),
                Writer = null!,
                Annotations = new object[0],
                Member = null,
                Optional = false
            }.SetReadOnly();
        }
        {
            IParameterDescription parameterDescription =
                new ParameterDescription()
                .SetName("arg")
                .SetType(typeof(int))
                .SetWriter(null)
                .SetAnnotations(new object[0])
                .SetMember(null)
                .SetOptional(false)
                .SetReadOnly();
            IParameterDescription clone = parameterDescription.Clone().SetReadOnly();
        }
        {
            // Get reference
            ParameterInfo pi = typeof(MyClass).GetConstructors()[0].GetParameters()[0];
            // Read
            IParameterDescription parameterDescription =
                new ParameterDescription()
                .Read(pi)
                .SetReadOnly();
        }

    }
    public class MyClass
    {
        public int value;

        public MyClass(int value)
        {
            this.value = value;
        }
    }
}
