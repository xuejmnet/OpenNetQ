using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenNetQ.Exceptions;
using OpenNetQ.Extensions;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.Core
{
    public class OpenNetQServiceContainer
    {
        private OpenNetQServiceContainer()
        {

        }

        private static IServiceProvider? _serviceProvider;

        public static IServiceProvider ServiceProvider
        {
            get { return _serviceProvider ?? throw new OpenNetQException("ServerContainer Not Initialize"); }
        }
        /// <summary>
        /// 静态注入
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            if (_serviceProvider == null)
                _serviceProvider = serviceProvider;
        }

        public static T? GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }
        public static T GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }
        public static IEnumerable<T> GetServices<T>()
        {
            return ServiceProvider.GetServices<T>();
        }
        public static object? GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }
        /// <summary>
        /// 创建一个没有依赖注入的对象,但是对象的构造函数参数是已经可以通过依赖注入获取的
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object? CreateInstance(Type serviceType)
        {
            var constructors
                = serviceType.GetTypeInfo().DeclaredConstructors
                    .Where(c => !c.IsStatic && c.IsPublic)
                    .ToArray();

            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    $"type :[{serviceType}] found more than one  declared constructor ");
            }
            var @params = constructors[0].GetParameters().Select(x => ServiceProvider.GetService(x.ParameterType))
                .ToArray();
            return Activator.CreateInstance(serviceType, @params);
        }
        /// <summary>
        /// 创建一个没有依赖注入的对象,但是对象的构造函数参数是已经可以通过依赖注入获取并且也存在自行传入的参数,优先判断自行传入的参数
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static object? CreateInstanceWithInputParams(Type serviceType, params object[] args)
        {
            var constructors
                = serviceType.GetTypeInfo().DeclaredConstructors
                    .Where(c => !c.IsStatic && c.IsPublic)
                    .ToArray();

            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    $"type :[{serviceType}] found more than one  declared constructor ");
            }

            var argIsNotEmpty = args.IsNotEmpty();
            var @params = constructors[0].GetParameters().Select(x =>
                {
                    if (argIsNotEmpty)
                    {
                        var arg = args.FirstOrDefault(o => o.GetType() == x.ParameterType);
                        if (arg != null)
                            return arg;
                    }
                    return ServiceProvider.GetService(x.ParameterType);
                })
                .ToArray();
            return Activator.CreateInstance(serviceType, @params);
        }
    }
}