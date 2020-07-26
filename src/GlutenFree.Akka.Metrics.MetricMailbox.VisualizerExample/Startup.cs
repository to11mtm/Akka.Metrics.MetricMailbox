using System;
using Akka.Actor;
using Akka.Routing;
using Akka.Streams.Util;
using GlutenFree.Akka.Metrics.MetricMailbox.RedisPubSubListener;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisInside;
using StackExchange.Redis;

namespace GlutenFree.Akka.Metrics.MetricMailbox.VisualizerExample
{
    public class Startup
    {
        private static Redis _redis;
        private ActorSystem _actorSystem;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _redis = new Redis(conf=>conf.Port(9001));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServerSentEvents();
            services
                .AddServerSentEvents<IMailboxEventService, MailboxEventService
                >();
            services.AddControllersWithViews();
            var conf = new ConfigurationOptions();
            conf.EndPoints.Add("localhost",9001);
            services.AddSingleton<IMetricPayloadSerializer,MessagePackMetricPayloadSerializer>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(conf));
            services.AddSingleton<ObservableMetricSubscriber>(sp =>
                new ObservableMetricSubscriber(sp.GetService<IConnectionMultiplexer>()
                    ,new MessagePackMetricPayloadSerializer(),"sample"));
            services.AddSingleton<SseRedisAsyncMetricSubscriber>(sp =>
                new SseRedisAsyncMetricSubscriber(
                    sp.GetService<IMailboxEventService>(),
                    sp.GetService<IConnectionMultiplexer>(),
                    sp.GetService<IMetricPayloadSerializer>(), "sample"));
            services.AddSingleton<IMetricMeasuringWriter>(sp =>
                new MetricPublisher(sp.GetService<IConnectionMultiplexer>(),
                    new MessagePackMetricPayloadSerializer(), "sample"));
            _actorSystem = ActorSystem.Create("sample", MetricMailboxConfig.Config());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        private void DoBroadcasts(IActorRef recipient, IActorRef sender,
            int numiters)
        {
            for (int i=0; i<numiters; i++)
            {
                _actorSystem.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromSeconds(0.1), recipient,
                    new Broadcast( new Echo(i)), sender);    
            }
        }
        private void DoTells(IActorRef recipient, IActorRef sender,
            int numiters)
        {
            for (int i=0; i<numiters; i++)
            {
                _actorSystem.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromSeconds(0.1), recipient,
                     new Echo(i), sender);    
            }
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var echoRef = _actorSystem.ActorOf(Props
                .Create(() => new EchoActor(false))
                .WithDefaultMetricMailbox().WithRouter(new ConsistentHashingPool(10,
                    msg =>
                    {
                        if (msg is Echo e)
                        {
                            return e.Id;
                        }

                        return msg;
                    })),"consistentHashing");
            var otherEchoRef = _actorSystem.ActorOf(Props
                .Create(() => new EchoActor(true)).WithDefaultMetricMailbox().WithRouter(new RoundRobinPool(1)),"roundRobin");
            _actorSystem.RegisterMailboxMetricListener(app.ApplicationServices
                .GetService<IMetricMeasuringWriter>());
            app.ApplicationServices.GetService<SseRedisAsyncMetricSubscriber>();
            DoTells(echoRef,otherEchoRef,50);
            DoBroadcasts(otherEchoRef,echoRef,5);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapServerSentEvents<MailboxEventService>("/api/events");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}