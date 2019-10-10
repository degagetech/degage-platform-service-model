#define ENABLE_SIMULATE_
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
#if !NETSTANDRAD2_0
using System.Reflection.Emit;
#endif
using System.Threading;
using System.Collections.Concurrent;
using System.Collections;
namespace Degage.ServiceModel.Rpc
{
    public partial class ProxyFactory
    {
        /// <summary>
        ///Key 为接口类型，Value 为动态创建的代理类的类型
        /// </summary>
        private static ConcurrentDictionary<Type, Type> _InterfaceProxyTypeTable;
        /// <summary>
        ///Key 为接口类型，Value 为动态创建的代理类的对象的委托 
        /// </summary>
        private static ConcurrentDictionary<Type, Func<IInvokeHandler, Type, Object>> _InterfaceProxyDelegateTable;
#if !NETSTANDRAD2_0
        private static readonly Dictionary<Type, OpCode> _TypeLdOpCodeTable;
#endif
        private static Object _SyncObject;

        ///// <summary>
        ///// 默认的序列化器实例生成工厂，此静态属性的设置不保证线程安全性
        ///// </summary>
        //public static Func<ISerializer> DefaultSerializerFactory { get; set; }

        ///// <summary>
        ///// 默认的数据传输器，此静态属性的设置不证线程安全性保
        ///// </summary>
        //public static Func<ITransmitter> DefaultTransmitterFactory { get; set; }

        /// <summary>
        /// 默认的代理设置
        /// </summary>
        public static ProxySetting DefaultProxySetting
        {
            get
            {
                return _DefaultProxySetting;
            }
            set
            {
                lock (_SyncObject)
                {
                    _DefaultProxySetting = value;
                }
            }
        }
        private volatile static ProxySetting _DefaultProxySetting;


        static ProxyFactory()
        {

            _InterfaceProxyTypeTable = new ConcurrentDictionary<Type, Type>();
            _SyncObject = new Object();
            _InterfaceProxyDelegateTable = new ConcurrentDictionary<Type, Func<IInvokeHandler, Type, Object>>();
#if !NETSTANDRAD2_0
            _TypeLdOpCodeTable = new Dictionary<Type, OpCode>();


            _TypeLdOpCodeTable.Add(typeof(System.Boolean), OpCodes.Ldind_I1);
            _TypeLdOpCodeTable.Add(typeof(System.Int16), OpCodes.Ldind_I2);
            _TypeLdOpCodeTable.Add(typeof(System.Int32), OpCodes.Ldind_I4);
            _TypeLdOpCodeTable.Add(typeof(System.Int64), OpCodes.Ldind_I8);
            _TypeLdOpCodeTable.Add(typeof(System.Double), OpCodes.Ldind_R8);
            _TypeLdOpCodeTable.Add(typeof(System.Single), OpCodes.Ldind_R4);
            _TypeLdOpCodeTable.Add(typeof(System.UInt16), OpCodes.Ldind_U2);
            _TypeLdOpCodeTable.Add(typeof(System.UInt32), OpCodes.Ldind_U4);
#endif

        }




        /// <summary>
        /// 代理设置
        /// </summary>
        public ProxySetting ProxySetting { get; internal set; }
        public Func<ITransferSerializer> SerializerFactory { get; internal set; }
        public Func<ITransmitter> TransmitterFactory { get; set; }

        public IInvokeHandler InvokeHandlerFactory
        {
            get
            {
                return new RemoteInvokeHandler(
                    this.ProxySetting,
                    this.SerializerFactory.Invoke(),
                    this.TransmitterFactory.Invoke());
            }
        }


        public ProxyFactory(ProxySetting setting, Func<ITransferSerializer> serializerFactory, Func<ITransmitter> transmitterFactory)
        {
            this.ProxySetting = setting;
            this.SerializerFactory = serializerFactory;
            this.TransmitterFactory = transmitterFactory;
        }

        public void DisposeProxy(Object proxy)
        {
            try
            {
                ((IDisposable)proxy).Dispose();
            }
            catch
            {

            }
        }
        public ProxyFactory(ProxySetting setting, Type serializerType, Type transmitterType)
        {
            if (serializerType == null)
            {
                throw new ArgumentNullException(nameof(serializerType));
            }
            if (transmitterType == null)
            {
                throw new ArgumentNullException(nameof(transmitterType));
            }

            this.ProxySetting = setting;


            Func<ITransferSerializer> serializerFactory = null;
#if NET40
            NewExpression newSerializerExp = Expression.New(serializerType);
            serializerFactory = Expression.Lambda<Func<ITransferSerializer>>(newSerializerExp).Compile();          
#elif NETSTANDRAD2_0
            serializerFactory = () =>
              {
                  return (ITransferSerializer)Activator.CreateInstance(serializerType);
              };

#endif
            this.SerializerFactory = serializerFactory;


            Func<ITransmitter> transmitterFactory = null;
#if NET40
            NewExpression newTransmitterExp = Expression.New(transmitterType);
            transmitterFactory = Expression.Lambda<Func<ITransmitter>>(newTransmitterExp).Compile();
#elif NETSTANDRAD2_0
            transmitterFactory = () =>
            {
                return (ITransmitter)Activator.CreateInstance(transmitterType);
            };
#endif
            this.TransmitterFactory = transmitterFactory;



        }


        public Object CreateProxy(Type interfaceType)
        {
            Object proxy = null;
            Start:
            Func<IInvokeHandler, Type, Object> proxyFunc = GetProxyCreateDelegate(interfaceType);
            if (proxyFunc == null)
            {
                lock (_SyncObject)
                {
                    if (proxyFunc == null)
                    {
                        Type proxyType = GetProxyType(interfaceType);
                        if (proxyType == null)
                        {
                            EmitMetadataCache.Add(interfaceType);
                            proxyType = DynamicallyCreateProxyType(interfaceType);
                            AddProxyTypeCache(interfaceType, proxyType);
                        }
                        //public DynamicProxy(IInvokeHandler handler,Type interfaceType)
                        //Object proxy=(Object)(new ProxyType(handler,interfaceType));
                        var proxyTypeConstructor = proxyType.GetConstructor(new Type[] { typeof(IInvokeHandler), typeof(Type) });

                        var handlerParameterExpression = Expression.Parameter(typeof(IInvokeHandler));
                        var typeParameterExpression = Expression.Parameter(typeof(Type));
                        var newExpression = Expression.New(proxyTypeConstructor, new Expression[] {
                     handlerParameterExpression,typeParameterExpression
                      });
                        Expression castExpression = Expression.Convert(newExpression, typeof(Object));
                        proxyFunc = Expression.Lambda<Func<IInvokeHandler, Type, Object>>(
                            newExpression,
                            handlerParameterExpression,
                            typeParameterExpression).Compile();
                        AddProxyCreateDelegateCache(interfaceType, proxyFunc);
                    }
                    else
                    {
                        goto Start;
                    }
                }
            }
            var invokeHandler = this.InvokeHandlerFactory;
            invokeHandler.Initialize();

            proxy = proxyFunc.Invoke(invokeHandler, interfaceType);

            return proxy;
        }

        public T CreateProxy<T>()
        {
            T proxy = (T)CreateProxy(typeof(T));
            return proxy;
        }



        /// <summary>
        /// 若未找到返回 null
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static Func<IInvokeHandler, Type, Object> GetProxyCreateDelegate(Type interfaceType)
        {
            Func<IInvokeHandler, Type, Object> func = null;
            if (_InterfaceProxyDelegateTable.TryGetValue(interfaceType, out func))
            {
                //
            }
            return func;
        }

        private static void AddProxyCreateDelegateCache(Type interfaceType, Func<IInvokeHandler, Type, Object> func)
        {
            if (!_InterfaceProxyDelegateTable.TryAdd(interfaceType, func))
            {
                //代理创建委托添加失败时
            }
        }

        /// <summary>
        ///若未找到返回  null
        /// </summary>
        /// <param name="interfaceType">接口的类型</param>
        /// <returns></returns>
        private static Type GetProxyType(Type interfaceType)
        {
            Type proxyType = null;
            if (_InterfaceProxyTypeTable.TryGetValue(interfaceType, out proxyType))
            {
                //
            }
            return proxyType;
        }

        private static void AddProxyTypeCache(Type interfaceType, Type proxyType)
        {
            if (!_InterfaceProxyTypeTable.TryAdd(interfaceType, proxyType))
            {
                // 代理类型的缓存条件失败时
            }
        }
        #region 代理类代码模拟
#if ENABLE_SIMULATE
        public class DynamicProxyClass
        {
            public DynamicProxyClass(IInvokeHandler handler, Type interfaceType) : base()
            {
                this._handler = handler;
                this._interfaceType = interfaceType;
            }
            /// <summary>
            /// //处理调用的对象
            /// </summary>
            public IInvokeHandler _handler;
            /// <summary>
            /// 代理接口的类型
            /// </summary>
            public Type _interfaceType;
            private void ProxyMethodTest(Int32 test)
            {
                //获取当前方法的签名字符串，注意！此处之所以不直接用获取到的方法信息，
                //是因为当前获取到的是动态生成的代理类的接口的方法的实现，并没有继承接口方法的特性
                //详情请查看单元测试项目 Biobank.Function.TestUnit/ServiceModel/ServiceModelTest
                String methodSignName = System.Reflection.MethodBase.GetCurrentMethod().ToString();

                //获取接口方法信息
                var methodInfo = EmitMetadataCache.GetMethodInfo(_interfaceType, methodSignName);
                //  _handler.InovkeHandle(this, _interfaceType, methodInfo,);
            }
        }
#endif
        #endregion
#if !NETSTANDRAD2_0
        /// <summary>
        /// 为指定接口动态创建代理类型
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private static Type DynamicallyCreateProxyType(Type interfaceType)
        {
            var methodInfo = MethodBase.GetCurrentMethod();
            String methodName = methodInfo.Name;
            Type objectType = typeof(Object);


            String assemblyName = "DynamicAssembly";
            String moudleName = "DynamicMoudle";
            String typeName = interfaceType.FullName + ".DynamicProxy";
            String field_handler_Name = "_handler";
            String field_interfaceType_Name = "_interfaceType";


            //在当前程序域中构建程序集信息
            AppDomain domain = AppDomain.CurrentDomain;

            //构建一个程序集
            AssemblyName assemblyNameObj = new AssemblyName(assemblyName);
            assemblyNameObj.Version = new Version(1, 0, 0, 0);
            assemblyNameObj.CultureInfo = null;
#if NETSTANDARD2_0
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyNameObj,
                AssemblyBuilderAccess.RunAndCollect);
#else
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(
               assemblyNameObj,
               AssemblyBuilderAccess.RunAndCollect);
#endif
            //在程序集中构建基本模块
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moudleName);

            //在模块中构建类型
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
                objectType,
                new Type[] { interfaceType, typeof(IDisposable) });

            //为类型构建字段成员
            FieldBuilder handlerField = typeBuilder.DefineField(
                field_handler_Name,
                typeof(IInvokeHandler),
                FieldAttributes.Private);

            FieldBuilder interfaceTypeField = typeBuilder.DefineField(
                field_interfaceType_Name,
                interfaceType,
                FieldAttributes.Private);

            //为类型构建构造函数信息
            ConstructorInfo objectConstructorInfo = objectType.GetConstructor(new Type[] { });

            //public DynamicProxy(IDynamicProxyHandler handler,Type interfaceType)
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(IInvokeHandler), typeof(Type) });

            ILGenerator iLGenerator = constructorBuilder.GetILGenerator();

            //在构造函数中使用参数为私有字赋值

            //this._handler=handler;
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, handlerField);

            //this._interfaceType=interfaceType;
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_2);
            iLGenerator.Emit(OpCodes.Stfld, interfaceTypeField);

            //调用基类的构造函数
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, objectConstructorInfo);
            iLGenerator.Emit(OpCodes.Ret);


            //为类型构建方法成员
            DynamicallyCreateProxyTypeMethod(interfaceType, typeBuilder, handlerField, interfaceTypeField);
            DynamicallyCreateProxyTypeMethod(typeof(IDisposable), typeBuilder, handlerField, interfaceTypeField);

#if NETSTANDARD2_0
            return typeBuilder.CreateTypeInfo();

#else
            return typeBuilder.CreateType();
#endif
        }
#endif
#if !NETSTANDRAD2_0
        private static void DynamicallyCreateProxyTypeMethod(Type interfaceType,
            TypeBuilder proxyTypeBuilder,
            FieldBuilder handlerField,
            FieldBuilder interfaceTypeField)
        {
            if (interfaceType == typeof(IDisposable))
            {
                //调用IInvokeHandler 的 void Dispose(); 方法
                MethodInfo disposeMethodInfo = typeof(IDisposable).GetMethods().First();
                MethodBuilder methodBuilder = proxyTypeBuilder.DefineMethod(
                     disposeMethodInfo.Name,
                     MethodAttributes.Public | MethodAttributes.Virtual,
                     CallingConventions.Standard,
                     disposeMethodInfo.ReturnType,
                     null
                    );
                var ilGenerator = methodBuilder.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, handlerField);
                ilGenerator.Emit(OpCodes.Call, disposeMethodInfo);
                ilGenerator.Emit(OpCodes.Ret);
                return;
            }
            var methodInfos = interfaceType.GetMethods();

            MethodInfo methodInfo = null;
            for (Int32 i = 0; i < methodInfos.Length; ++i)
            {
                methodInfo = methodInfos[i];
                var parameterInfos = methodInfo.GetParameters();
                var parameterCount = parameterInfos.Length;

                Type[] parameterTypeInfos = new Type[parameterInfos.Length];

                for (Int32 j = 0; j < parameterCount; ++j)
                {
                    parameterTypeInfos[j] = parameterInfos[j].ParameterType;
                }

                //根据接口的方法声明为动态类声明方法
                MethodBuilder methodBuilder = proxyTypeBuilder.DefineMethod(
                      methodInfo.Name,
                      MethodAttributes.Public | MethodAttributes.Virtual,
                      CallingConventions.Standard,
                      methodInfo.ReturnType,
                      parameterTypeInfos
                      );

                var cacheMethodInfo = typeof(EmitMetadataCache).GetMethod(
                 nameof(EmitMetadataCache.GetMethodInfo),
                 BindingFlags.Public | BindingFlags.Static);

                var handleMethodInfo = typeof(IInvokeHandler).GetMethod(
                    nameof(IInvokeHandler.InovkeHandle),
                    BindingFlags.Public | BindingFlags.Instance);

                String methodSignName = methodInfo.ToString();
                ILGenerator iLGenerator = methodBuilder.GetILGenerator();

                //声明参数对象列表
                LocalBuilder parameterObjects = null;
                if (parameterCount > 0)
                {
                    parameterObjects = iLGenerator.DeclareLocal(typeof(System.Object[]));
                }


                //加载 handler 对象
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, handlerField);

                //加载对象本身
                iLGenerator.Emit(OpCodes.Ldarg_0);

                //加载接口类型
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, interfaceTypeField);

                //加载接口方法对象
                // EmitMetadataCache.GetMethodInfo(interfaceType,methodSignName);
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Ldfld, interfaceTypeField);
                iLGenerator.Emit(OpCodes.Ldstr, methodSignName);
                iLGenerator.Emit(OpCodes.Call, cacheMethodInfo);

                //加载 parameters 参数至栈顶
                if (parameterObjects != null)
                {
                    iLGenerator.Emit(OpCodes.Ldc_I4, parameterCount);
                    iLGenerator.Emit(OpCodes.Newarr, typeof(Object));
                    iLGenerator.Emit(OpCodes.Stloc, parameterObjects.LocalIndex);
                    for (int k = 0; k < parameterCount; k++)
                    {
                        //推送数组引用到堆栈上
                        iLGenerator.Emit(OpCodes.Ldloc, parameterObjects.LocalIndex);
                        //将数组索引推送到堆栈上
                        iLGenerator.Emit(OpCodes.Ldc_I4, k);
                        //第一个参数为对象本身，所以该用 k+1
                        iLGenerator.Emit(OpCodes.Ldarg, k + 1);
                        if (parameterTypeInfos[k].IsValueType)
                        {
                            iLGenerator.Emit(OpCodes.Box, parameterTypeInfos[k]);
                        }
                        iLGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    iLGenerator.Emit(OpCodes.Ldloc, parameterObjects.LocalIndex);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Ldnull);
                }

                //调用 InovkeHandle 方法
                iLGenerator.Emit(OpCodes.Call, handleMethodInfo);
                //Object InovkeHandle(Object proxy, Type interfaceType, MethodInfo interfaceInvokeMethod, Object[] parameters);

                if (!methodInfo.ReturnType.Equals(typeof(void)))
                {
                    if (methodInfo.ReturnType.IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Unbox, methodInfo.ReturnType);
                        if (methodInfo.ReturnType.IsEnum)
                        {
                            iLGenerator.Emit(OpCodes.Ldind_I4);
                        }
                        else if (!methodInfo.ReturnType.IsPrimitive)
                        {
                            iLGenerator.Emit(OpCodes.Ldobj, methodInfo.ReturnType);
                        }
                        else
                        {
                            iLGenerator.Emit(_TypeLdOpCodeTable[methodInfo.ReturnType]);
                        }
                    }
                }
                else
                {
                    //如果方法本身没有返回值，此时应该清除栈顶的数据
                    iLGenerator.Emit(OpCodes.Pop);
                }
                iLGenerator.Emit(OpCodes.Ret);
            }

            var parentInterfaces = interfaceType.GetInterfaces();
            if (parentInterfaces.Length > 0)
            {
                foreach (var parentInterfaceType in parentInterfaces)
                {
                    //避免重复实现
                    if (parentInterfaceType == typeof(IDisposable)) continue;
                    DynamicallyCreateProxyTypeMethod(parentInterfaceType, proxyTypeBuilder, handlerField, interfaceTypeField);
                }
            }

        }
#endif
    }
}
