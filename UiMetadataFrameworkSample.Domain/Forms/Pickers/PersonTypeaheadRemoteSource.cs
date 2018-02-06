namespace UiMetadataFrameworkSample.Domain.Forms.Pickers
{
	using System;
	using System.Linq;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFrameworkSample.Infrastructure;
	using UiMetadataFrameworkSample.Infrastructure.Metadata;
	using UiMetadataFrameworkSample.Infrastructure.Metadata.Typeahead;

	[Form]
	public class PersonTypeaheadRemoteSource : ITypeaheadRemoteSource<PersonTypeaheadRemoteSource.Request, string>
	{
		public TypeaheadResponse<string> Handle(Request message)
		{
			var random = new Random(1);
			var people = Queryable.AsQueryable(Enumerable.Select(Enumerable.Range(0, 100),
				t => SearchPeople.FamilyPerson.RandomFamilyPerson(random.Next(150, 210), random.Next(40, 130))));

			return people.GetForTypeahead(
				message,
				t => new TypeaheadItem<string>
				{
					Label = t.FirstName.Label + " (" + t.DateOfBirth.Value.ToString("dd-MMM-yyyy") + ")",
					Value = t.FirstName.Label
				},
				t => message.Ids.Items.Contains(t.FirstName.Label),
				t => t.FirstName.Label.Contains(message.Query, StringComparison.OrdinalIgnoreCase));
		}

		public class Request : TypeaheadRequest<string>
		{
		}
	}
}