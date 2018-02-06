namespace UiMetadataFrameworkSample.Web
{
	using System;
	using System.Linq;
	using MediatR;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;
	using StructureMap;
	using StructureMap.TypeRules;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMetadataFrameworkSample.Domain.Forms;
	using UiMetadataFrameworkSample.Infrastructure;
	using UiMetadataFrameworkSample.Infrastructure.Metadata;
	using UiMetadataFrameworkSample.Infrastructure.Metadata.Menu;
	using UiMetadataFrameworkSample.Web.Middleware;
	using AppDependencyInjectionContainer = UiMetadataFrameworkSample.Infrastructure.DependencyInjectionContainer;
	using UimfDependencyInjectionContainer = UiMetadataFramework.Core.Binding.DependencyInjectionContainer;

	public class Startup
	{
		public const string CorsAllowAllPolicy = "AllowAll";

		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseMiddleware(typeof(ErrorHandlingMiddleware));
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
			//app.UseSession();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			//services.ConfigureAuthentication();
			services.ConfigureMvc(this.Configuration);

			// Register all assemblies with IRequestHandler.
			services.AddMediatR(typeof(SearchPeople));
			services.AddMediatR(typeof(InvokeForm));
			services.AddMediatR(typeof(MyForms));

			var container = new Container();
			container.Configure(config =>
			{
				config.For<MetadataBinder>().Use(t => GetMetadataBinder(t)).Singleton();
				config.For<FormRegister>().Singleton();
				config.For<MenuRegister>().Singleton();

				config.For<AppDependencyInjectionContainer>().Use(ctx => new AppDependencyInjectionContainer(ctx.GetInstance));
				config.For<UimfDependencyInjectionContainer>().Use(t => new UimfDependencyInjectionContainer(t.GetInstance));

				config.Scan(_ =>
				{
					_.AssembliesFromApplicationBaseDirectory();
					_.AddAllTypesOf<IAssemblyBootstrapper>();
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

			// Run all assembly bootstrappers.
			foreach (var bootstrapper in serviceProvider.GetServices<IAssemblyBootstrapper>().OrderByDescending(t => t.Priority))
		{
				bootstrapper.Start(new AppDependencyInjectionContainer(t => container.GetInstance(t)));
		}

			return serviceProvider;
		}

		private static MetadataBinder GetMetadataBinder(IContext context)
		{
			var binder = new MetadataBinder(context.GetInstance<UimfDependencyInjectionContainer>());
			binder.RegisterAssembly(typeof(StringInputFieldBinding).GetAssembly());
			return binder;
		}
	}
}