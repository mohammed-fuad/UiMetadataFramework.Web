namespace UiMetadataFramework.Web
{
	using System.Collections.Generic;
	using UiMetadataFramework.Web.Metadata.Menu;

	public sealed class UserMenus : IMenuContainer
	{
		public const string Main = "System";
		public const string People = "People";
		public const string TopLevel = "";

		public IList<MenuMetadata> GetMenuMetadata()
		{
			return new List<MenuMetadata>
			{
				new MenuMetadata(TopLevel, 100),
				new MenuMetadata(Main, 90),
				new MenuMetadata(People, 100)
			};
		}
	}
}
