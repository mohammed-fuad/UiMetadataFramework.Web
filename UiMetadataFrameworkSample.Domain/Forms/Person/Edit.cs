namespace UiMetadataFrameworkSample.Domain.Forms.Person
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UiMetadataFramework.Basic.EventHandlers;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFrameworkSample.Domain.Forms.Pickers;
	using UiMetadataFrameworkSample.Infrastructure;
	using UiMetadataFrameworkSample.Infrastructure.Metadata;
	using UiMetadataFrameworkSample.Infrastructure.Metadata.ClientFunctions;
	using UiMetadataFrameworkSample.Infrastructure.Metadata.EventHandlers;
	using UiMetadataFrameworkSample.Infrastructure.Metadata.Record;

	[MyForm(Id = "EditPerson", PostOnLoad = true, SubmitButtonLabel = "Save changes")]
	[LogToConsole(FormEvents.FormLoaded)]
	public class Edit : IMyForm<Edit.Request, Edit.Response>
	{
		public Response Handle(Request message)
		{
			var person = SearchPeople.FamilyPerson.RandomFamilyPerson(message.Name);

			if (message.Operation.Value == RecordRequestOperation.Post)
			{
				person.DateOfBirth = message.DateOfBirth;

				if (message.Height == null)
				{
					throw new ArgumentNullException(nameof(message.Height));
				}

				if (message.Weight == null)
				{
					throw new ArgumentNullException(nameof(message.Weight));
				}

				person.Height = message.Height.Value;
				person.Weight = message.Weight.Value;
			}

			var response = new Response
			{
				Name = person.FirstName.Label,
				DateOfBirth = person.DateOfBirth,
				Height = person.Height,
				Weight = person.Weight,
				Spouse = person.Relatives.Select(t => t.FirstName.Label).AsMultiSelect(),
				Metadata = new MyFormResponseMetadata
				{
					Title = person.FirstName.Label
				}
			};

			if (message.Operation.Value == RecordRequestOperation.Post)
			{
				response.Metadata.FunctionsToRun = new List<ClientFunctionMetadata>
				{
					new GrowlNotification("Changes to the user record were saved.", "success").GetClientFunctionMetadata()
				};
			}

			return response;
		}

		public static FormLink FormLink(string personName, string label)
		{
			return new FormLink
			{
				Label = label,
				Form = typeof(Edit).GetFormId(),
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Name), personName },
					{ nameof(Request.Operation), new DropdownValue<RecordRequestOperation>(RecordRequestOperation.Get) }
				}
			};
		}

		public class Response : RecordResponse
		{
			[NotField]
			public DateTime? DateOfBirth { get; set; }

			[NotField]
			public int Height { get; set; }

			[NotField]
			[LogToConsole(FormEvents.ResponseHandled)]
			public string Name { get; set; }

			[NotField]
			public MultiSelect<string> Spouse { get; set; }

			[NotField]
			public decimal Weight { get; set; }
		}

		public class Request : RecordRequest<Response>
		{
			[BindToOutput(nameof(Response.DateOfBirth))]
			public DateTime? DateOfBirth { get; set; }

			[BindToOutput(nameof(Response.Height))]
			public int? Height { get; set; }

			[BindToOutput(nameof(Response.Name))]
			[LogToConsole(FormEvents.FormLoaded)]
			public string Name { get; set; }

			[BindToOutput(nameof(Response.Spouse))]
			//[TypeaheadInputField(typeof(PersonTypeaheadInlineSource))]
			[TypeaheadInputField(typeof(PersonTypeaheadInlineSource))]
			public MultiSelect<string> Spouse { get; set; }

			[BindToOutput(nameof(Response.Weight))]
			public int? Weight { get; set; }
		}
	}
}