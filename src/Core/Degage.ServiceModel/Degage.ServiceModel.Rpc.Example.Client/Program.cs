using System;

namespace Degage.ServiceModel.Rpc.Example.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ProxySetting setting = new ProxySetting();

            setting.TransferSetting = TransferSettingGetter.Get;

            ProxyFactory factory = new ProxyFactory(setting, typeof(JsonTransferSerializer), typeof(HttpTransmitter));

            var proxy = factory.CreateProxy<IExampleService>();
            ConsoleHelper.ShowTextInfo("Any key to continue:");
            Console.ReadLine();
            var info = proxy.GetPlayerInfo("John Wang");
            ConsoleHelper.ShowPlayerInfo(info);
        }
    }
}
