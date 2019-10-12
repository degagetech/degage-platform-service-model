using System;
using System.Collections.Generic;
using System.Text;

namespace Degage.ServiceModel.Rpc.Example.Server
{
    public class ExampleService : IExampleService
    {
        public Boolean AddPlayerInfo(PlayerInfo info)
        {
            ConsoleHelper.NewLine();
            ConsoleHelper.ShowTextInfo("Invoked method:" + nameof(AddPlayerInfo));
            ConsoleHelper.ShowTextInfo("parameter:" );
            ConsoleHelper.ShowPlayerInfo(info);
            ConsoleHelper.ShowTextInfo("Current Object Hash:" + this.GetHashCode().ToString());

            return true;
        }

        public PlayerInfo GetPlayerInfo(String name)
        {
            ConsoleHelper.NewLine();
            ConsoleHelper.ShowTextInfo("Invoked method:" + nameof(GetPlayerInfo));
            ConsoleHelper.ShowTextInfo("parameter: " + name);
            ConsoleHelper.ShowTextInfo("Current Object Hash:" + this.GetHashCode().ToString());

            return new PlayerInfo
            {
                Name = name,
                Age = 18
            };
        }
    }
}
