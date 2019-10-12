using System;
using System.Collections.Generic;
using System.Text;

namespace Degage.ServiceModel.Rpc.Example
{
    public interface IExampleService
    {
        PlayerInfo GetPlayerInfo(String name);
        Boolean AddPlayerInfo(PlayerInfo info);
    }
    public class PlayerInfo
    {
        public String Name { get; set; }
        public Int32 Age { get; set; }
    }

    public static class TransferSettingGetter
    {
        public static TransferSetting Get
        {
            get
            {
                return new TransferSetting
                {
                    Address = "http://127.0.0.1:20018/",
                    Timeout=new TimeSpan(0,0,15)
                };
            }
        }
    }
}
