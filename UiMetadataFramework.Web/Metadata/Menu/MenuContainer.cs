namespace UiMetadataFramework.Web.Metadata.Menu
{
	using System.Collections.Generic;

	public interface IMenuContainer
	{
		IList<MenuMetadata> GetMenuMetadata();
	}
}
