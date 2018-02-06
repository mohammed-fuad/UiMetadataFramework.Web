namespace UiMetadataFrameworkSample.Domain.Forms
{
	using MediatR;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;

	[Form(Id = "Redirect", Label = "Redirect handler", PostOnLoad = false)]
	public class Redirect : IForm<Redirect.Request, Redirect.Response>
	{
		public Response Handle(Request message)
		{
			return new Response
			{
				Form = message.Form
			};
		}

		public class Request : IRequest<Response>
		{
			[InputField(Label = "Id of the form to redirect to", Required = true)]
			public string Form { get; set; }
		}

		public class Response : RedirectResponse
		{
		}
	}
}