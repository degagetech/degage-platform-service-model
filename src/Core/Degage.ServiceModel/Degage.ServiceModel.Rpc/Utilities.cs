using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
namespace Degage.ServiceModel.Rpc
{
    public static class Utilities
    {
        private static ConcurrentDictionary<String, Type> _TypeTable;
        private static Object _SyncObject;
        static Utilities()
        {
            _TypeTable = new ConcurrentDictionary<String, Type>();
            _SyncObject = new Object();
        }
        public static String GetOperationPath(String interfaceName, String operationName)
        {
            return interfaceName + "_" + operationName;
        }

        public static String GetOperationPath(ServiceInterfaceSchema interfaceSchema, ServiceOperationSchema operationSchema)
        {
            return GetOperationPath(interfaceSchema.Name, operationSchema.Name);
        }
        /// <summary>
        /// 从当前的程序域中搜寻符合类型完全限定名的信息
        /// </summary>
        /// <param name="typeName">l类型的完全限定名</param>
        /// <returns></returns>
        public static Type GetType(String typeName, Boolean throwError = false)
        {
            Type type = null;
            if (_TypeTable.TryGetValue(typeName, out type))
            {
                return type;
            }
            lock (_SyncObject)
            {
                if (_TypeTable.TryGetValue(typeName, out type))
                {
                    return type;
                }

                type = Type.GetType(typeName);
                if (type != null)
                {
                    _TypeTable.TryAdd(typeName, type);
                    return type;
                }
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = a.GetType(typeName);
                    if (type != null)
                    {
                        _TypeTable.TryAdd(typeName, type);
                        return type;
                    }

                }
            }
            if (throwError)
            {
                throw new TypeLoadException(typeName);
            }
            return type;
        }
    }
}
