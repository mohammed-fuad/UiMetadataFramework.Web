namespace UiMetadataFramework.Web.Metadata
{
	using System.Collections.Generic;
	using System.Linq;
	using global::MediatR;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.MediatR;
	using UiMetadataFramework.Web.Metadata.Menu;

	/// <summary>
	///     Gets all forms available to the current user.
	/// </summary>
	public class MyForms : IRequestHandler<MyForms.Request, MyForms.Response>
	{
		private readonly FormRegister formRegister;
		private readonly MenuRegister menuRegister;

		public MyForms(FormRegister formRegister, MenuRegister menuRegister)
		{
			this.formRegister = formRegister;
			this.menuRegister = menuRegister;
		}

		public Response Handle(Request message)
		{
			return new Response
			{
				Forms = this.formRegister.RegisteredForms.Select(t => t.Metadata).ToList(),
				Menus = this.menuRegister.RegisteredMenus
			};
		}

		public class Request : IRequest<Response>
		{
		}

		public class Response
		{
			public IList<FormMetadata> Forms { get; set; }
			public IList<MenuMetadata> Menus { get; set; }
		}
	}
}