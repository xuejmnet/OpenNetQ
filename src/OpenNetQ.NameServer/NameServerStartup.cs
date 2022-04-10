using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenNetQ.Common.NameSrv;
using OpenNetQ.Core;
using OpenNetQ.Logging;
using OpenNetQ.NameServer.KvConfig;
using OpenNetQ.NameServer.Processor;
using OpenNetQ.NameServer.RouteInfo;
using OpenNetQ.Remoting.Abstractions;
using OpenNetQ.Remoting.Common;
using OpenNetQ.Remoting.Netty;

/*
* @Author: xjm
* @Description:
* @Date: DATE
* @Email: 326308290@qq.com
*/
namespace OpenNetQ.NameServer
{
    public class NameServerStartup
    {
        static async Task Main(string[] args)
        {
            try
            {
                await Main0(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("please press any key to exist");
            Console.ReadLine();
        }

        static async Task Main0(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Option.json")
                .AddJsonFile("RemotingServerOption.json")
                .AddJsonFile("NameServerOption.json");
            var configuration = configurationBuilder.Build();
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging();
            services.Configure<RemotingServerOption>(configuration.GetSection("RemotingServerOption"));
            services.Configure<NameServerOption>(configuration.GetSection("NameServerOption"));
            services.AddSingleton<IRemotingServer>(s =>
            {
                var remotingServerOption = s.GetRequiredService<RemotingServerOption>();
                var routeInfoManager = s.GetRequiredService<IRouteInfoManager>();
                var nettyRemotingServer = new NettyRemotingServer(remotingServerOption);
                nettyRemotingServer.NettyEventExecutor.OnChannelClose += (sender, e) => { routeInfoManager.OnChannelDestory(e.GetRemoteAddr(), e.GetChannel()); };
                nettyRemotingServer.NettyEventExecutor.OnChannelException += (sender, e) => { routeInfoManager.OnChannelDestory(e.GetRemoteAddr(), e.GetChannel()); };
                nettyRemotingServer.NettyEventExecutor.OnChannelIdle += (sender, e) => { routeInfoManager.OnChannelDestory(e.GetRemoteAddr(), e.GetChannel()); };
                return nettyRemotingServer;
            });
            services.AddSingleton<IRouteInfoManager, RouteInfoManager>();
            services.AddSingleton<IKvConfigManager, KvConfigManager>();
            services.AddSingleton<IDefaultProcessor>(s =>
            {
                var nameServerOption = s.GetRequiredService<NameServerOption>();
                if (nameServerOption.IsClusterTest)
                {
                    return new ClusterTestRequestProcessor();
                }
                else
                {
                    return new ClusterTestRequestProcessor();
                }
            });
            services.AddSingleton<NameServerController>();
            IServiceProvider buildServiceProvider = services.BuildServiceProvider();
            OpenNetQServiceContainer.Initialize(buildServiceProvider);
            var loggerFactory = OpenNetQServiceContainer.GetRequiredService<ILoggerFactory>();
            OpenNetQLoggerFactory.DefaultFactory = loggerFactory;
            var nameServerController = buildServiceProvider.GetRequiredService<NameServerController>();
            await nameServerController.Initialize();
            await nameServerController.StartAsync();

            await AppExister.Exist("quit", async () => { await nameServerController.StopAsync(); });
        }
    }
}