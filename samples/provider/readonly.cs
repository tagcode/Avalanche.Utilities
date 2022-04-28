using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

class provider_readonly
{
    public static void Run()
    {
        {
            // Create provider
            IProvider<string, MyRecord> provider =
                Providers.Func((string name) => new MyRecord { Name = name })
                .AsReadOnly();
            // Create record from read-only provider
            MyRecord record = provider["Hello"];
        }
    }

    public record MyRecord : ReadOnlyAssignableRecord
    {
        string name = null!;
        public string Name { get => name; set => this.AssertWritable().name = value; }
    }


}
