namespace UiMetadataFrameworkSample.Web
{
	using StructureMap.TypeRules;
	using UiMetadataFrameworkSample.Infrastructure;

	public class Bootstrap : IAssemblyBootstrapper
	{
		public int Priority { get; } = 0;

		public void Start(DependencyInjectionContainer dependencyInjectionContainer)
		{
			dependencyInjectionContainer.RegisterUiMetadata(this.GetType().GetAssembly());
		}
	}
}