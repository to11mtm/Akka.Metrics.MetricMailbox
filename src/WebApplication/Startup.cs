using System;
using System.Linq;
using Akka.Actor;
using Akka.Mailbox.Visualizer;
using Akka.Metrics.MetricMailbox;
using Akka.Metrics.MetricMailbox.RedisPubSubListener;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RedisInside;
using StackExchange.Redis;

namespace WebApplication
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
            services.AddSingleton<IMetricSerializer,MessagePackMetricSerializer>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(conf));
            services.AddSingleton<ObservableMetricSubscriber>(sp =>
                new ObservableMetricSubscriber(sp.GetService<IConnectionMultiplexer>()
                    ,new MessagePackMetricSerializer(),"sample"));
            services.AddSingleton<SSERedisMetricSubscriber>(sp =>
                new SSERedisMetricSubscriber(
                    sp.GetService<IMailboxEventService>(),
                    sp.GetService<IConnectionMultiplexer>(),
                    sp.GetService<IMetricSerializer>(), "sample"));
            services.AddSingleton<IMetricMeasuringWriter>(sp =>
                new MetricPublisher(sp.GetService<IConnectionMultiplexer>(),
                    new MessagePackMetricSerializer(), "sample"));
            _actorSystem = ActorSystem.Create("sample", MetricMailboxConfig.Config());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var echoRef = _actorSystem.ActorOf(Props
                .Create(() => new EchoActor())
                .WithDefaultMetricMailbox());
            var otherEchoRef = _actorSystem.ActorOf(Props
                .Create(() => new EchoActor()).WithDefaultMetricMailbox());
            _actorSystem.RegisterMailboxMetricListener(app.ApplicationServices
                .GetService<IMetricMeasuringWriter>());

            app.ApplicationServices.GetService<SSERedisMetricSubscriber>();
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            /*_actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);
            _actorSystem.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(0.1), echoRef,
                new Echo(), otherEchoRef);*/
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