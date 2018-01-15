﻿namespace UiMetadataFramework.Web
{
	using System;
	using global::MediatR;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using StructureMap;
	using StructureMap.TypeRules;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMetadataFramework.Web.Forms;
	using UiMetadataFramework.Web.Metadata.Menu;
	using UiMetadataFramework.Web.Middleware;
	using AppDependencyInjectionContainer = DependencyInjectionContainer;
	using UimfDependencyInjectionContainer = UiMetadataFramework.Core.Binding.DependencyInjectionContainer;

	public class Startup
	{
		public const string CorsAllowAllPolicy = "AllowAll";

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			this.Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware(typeof(ErrorHandlingMiddleware));
			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			app.UseMvc();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddCors(o => o.AddPolicy(CorsAllowAllPolicy, builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			}));

			// Add framework services.
			services.AddMvc().AddJsonOptions(options =>
			{
				options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
				options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
			});

			services.AddMediatR(typeof(SearchPeople));
			services.AddMediatR(typeof(InvokeForm));

			var container = new Container();
			container.Configure(config =>
			{
				config.For<MetadataBinder>().Use(t => GetMetadataBinder(t)).Singleton();
				config.For<FormRegister>().Use(t => GetFormRegister(t)).Singleton();
				config.For<AppDependencyInjectionContainer>().Use(ctx => new AppDependencyInjectionContainer(ctx.GetInstance));
				config.For<MenuRegister>().Use(t => GetMenuRegister(t.GetInstance<AppDependencyInjectionContainer>())).Singleton();

				config.Scan(_ =>
				{
					_.AssembliesFromApplicationBaseDirectory();
					_.WithDefaultConventions();
				});
			});

			// Populate the container using the service collection.
			// This will register all services from the collection
			// into the container with the appropriate lifetime.
			container.Populate(services);

			// Finally, make sure we return an IServiceProvider. This makes
			// ASP.NET use the StructureMap container to resolve its services.


			var serviceProvider = container.GetInstance<IServiceProvider>();

			//container.GetInstance<RequestHandlerGuardRegister>().RegisterAssembly(typeof(InvokeForm).Assembly);
			container.GetInstance<MenuRegister>().RegisterAssembly(typeof(InvokeForm).Assembly);

			return serviceProvider;
		}

		private static FormRegister GetFormRegister(IContext context)
		{
			var register = new FormRegister(context.GetInstance<MetadataBinder>());
			register.RegisterAssembly(typeof(SearchPeople).GetAssembly());
			return register;
		}

		private static MenuRegister GetMenuRegister(AppDependencyInjectionContainer context)
		{

			var register = new MenuRegister(context);
			register.RegisterAssembly(typeof(SearchPeople).GetAssembly());
			return register;
		}

		private static MetadataBinder GetMetadataBinder(IContext context)
		{
			var binder = new MetadataBinder(new UimfDependencyInjectionContainer(context.GetInstance));
			binder.RegisterAssembly(typeof(StringInputFieldBinding).GetAssembly());
			binder.RegisterAssembly(typeof(SearchPeople).GetAssembly());
			return binder;
		}
	}
}