using System;

namespace Degage.ServiceModel.Rpc.Example.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Listening...");
            HttpServiceHost host = new HttpServiceHost();

            var setting = TransferSettingGetter.Get;
            host.Register(typeof(IExampleService), typeof(ExampleService));

            host.Open(setting);
            Console.WriteLine("Opened");
            Console.ReadKey();
        }
    }
}
