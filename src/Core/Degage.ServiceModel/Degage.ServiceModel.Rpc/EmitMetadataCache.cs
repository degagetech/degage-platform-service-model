using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
namespace Degage.ServiceModel.Rpc
{
    /// <summary>
    ///管理并缓存  Emit 操作所需的元数据
    /// </summary>
    public static class EmitMetadataCache
    {
        private static ConcurrentDictionary<Type, Dictionary<String, MethodInfo>> _InterfaceMethodTables;

        private static Object _SyncObject;
        static EmitMetadataCache()
        {
            _SyncObject = new Object();
            _InterfaceMethodTables = new ConcurrentDictionary<Type, Dictionary<String, MethodInfo>>();
        }

        public static MethodInfo GetMethodInfo(Type interfaceType, String methodSignName)
        {
            MethodInfo info = null;
            if (_InterfaceMethodTables.TryGetValue(interfaceType, out var methodInfoTable))
            {
                if (methodInfoTable.ContainsKey(methodSignName))
                {
                    info = methodInfoTable[methodSignName];
                }
            }
            return info;
        }
        public static void Add(Type interfaceType)
        {
            if (!_InterfaceMethodTables.ContainsKey(interfaceType))
            {
                lock (_SyncObject)
                {
                    if (!_InterfaceMethodTables.TryGetValue(interfaceType, out var methodTable))
                    {
                        methodTable = CreateMethodTable(interfaceType);
                        if (!_InterfaceMethodTables.TryAdd(interfaceType, methodTable))
                        {
                            //
                            //throw InternalExceptionManager.CreateException(InternalExceptionManager.EmitMetadataCacheAddFailed);
                        }
                    }
                }
            }

        }
        private static Dictionary<String, MethodInfo> CreateMethodTable(Type interfaceType)
        {
            Dictionary<String, MethodInfo> methodTable = new Dictionary<String, MethodInfo>();
            var methodInfos = interfaceType.GetMethods();
            foreach (var methodInfo in methodInfos)
            {
                methodTable.Add(methodInfo.ToString(), methodInfo);
            }
            return methodTable;
        }
        public static MethodInfo[] GetMethodInfos(Type interfaceType)
        {
            MethodInfo[] infos = null;
            if (_InterfaceMethodTables.TryGetValue(interfaceType, out var methodInfoTable))
            {
                infos = methodInfoTable.Values.ToArray();
            }
            return infos;
        }
    }
}

