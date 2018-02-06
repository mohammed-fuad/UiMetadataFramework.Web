namespace UiMetadataFrameworkSample.Domain.Forms.Pickers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFrameworkSample.Infrastructure;

	public class PersonTypeaheadInlineSource : ITypeaheadInlineSource<string>
	{
		public IEnumerable<TypeaheadItem<string>> GetItems()
		{
			var random = new Random(1);
			var people = Enumerable.Range(0, 100)
				.Select(t => SearchPeople.FamilyPerson.RandomFamilyPerson(random.Next(150, 210), random.Next(40, 130)))
				.AsQueryable();

			return people
				.ToList()
				.AsTypeaheadResponse(t => t.FirstName.Label, t => t.FirstName.Label + " (" + t.DateOfBirth?.ToString("dd-MMM-yyyy") + ")").Items;
		}
	}
}