namespace UiMetadataFrameworkSample.Infrastructure.Metadata
{
	using MediatR;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFramework.MediatR;
	using UiMetadataFrameworkSample.Infrastructure.Metadata.Typeahead;

	public interface ITypeaheadRemoteSource<in TRequest, TKey> :
		IForm<TRequest, TypeaheadResponse<TKey>>,
		ITypeaheadRemoteSource
		where TRequest : IRequest<TypeaheadResponse<TKey>>
	{
	}
}