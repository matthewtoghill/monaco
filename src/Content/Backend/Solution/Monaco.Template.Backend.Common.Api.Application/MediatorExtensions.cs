using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Monaco.Template.Backend.Common.Application.Commands;
using Monaco.Template.Backend.Common.Application.DTOs;
using Monaco.Template.Backend.Common.Application.Queries;
using Monaco.Template.Backend.Common.Domain.Model;

namespace Monaco.Template.Backend.Common.Api.Application;

public static class MediatorExtensions
{
	extension(ISender sender)
	{
		/// <summary>
		/// Executes the query passed and returns the corresponding response that can be either Ok(result) or a NotFound() result depending on whether the retuned result is null or not
		/// </summary>
		/// <typeparam name="TResult">The type of the records returned by the query</typeparam>
		/// <param name="query"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<Ok<TResult>, NotFound>> ExecuteQueryAsync<TResult>(QueryBase<TResult> query,
																					 CancellationToken cancellationToken = default)
		{
			var result = await sender.Send(query, cancellationToken);
			return result is null
					   ? TypedResults.NotFound()
					   : TypedResults.Ok(result);
		}

		/// <summary>
		/// Executes the paged query passed and returns the corresponding response that can be either Ok(result) or a NotFound() result depending on whether the returned result is null or not
		/// </summary>
		/// <typeparam name="TResult">The type of the records contained in the page returned by the query</typeparam>
		/// <param name="query"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<Ok<Page<TResult>>, NotFound>> ExecuteQueryAsync<TResult>(QueryPagedBase<TResult> query,
																						   CancellationToken cancellationToken = default)
		{
			var result = await sender.Send(query, cancellationToken);
			return result is null
					   ? TypedResults.NotFound()
					   : TypedResults.Ok(result);
		}

		/// <summary>
		/// Executes the query passed and returns the corresponding response that can be either Ok(result) or a NotFound() result depending on whether the returned item is null or not
		/// </summary>
		/// <typeparam name="TResult">The type of the item returned by the query</typeparam>
		/// <param name="query"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<Ok<TResult>, NotFound>> ExecuteQueryAsync<TResult>(QueryByIdBase<TResult> query,
																					 CancellationToken cancellationToken = default)
		{
			var result = await sender.Send(query, cancellationToken);
			return result is null
					   ? TypedResults.NotFound()
					   : TypedResults.Ok(result);
		}

		/// <summary>
		/// Executes the query passed and returns the corresponding response that can be either Ok(result) or a NotFound() result depending on whether the returned item is null or not
		/// </summary>
		/// <typeparam name="TResult">The type of the item returned by the query</typeparam>
		/// <typeparam name="TKey">The type of the key to search the item by</typeparam>
		/// <param name="query"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<Ok<TResult>, NotFound>> ExecuteQueryAsync<TResult, TKey>(QueryByKeyBase<TResult, TKey> query,
																						   CancellationToken cancellationToken = default)
		{
			var item = await sender.Send(query, cancellationToken);
			return item is null
					   ? TypedResults.NotFound()
					   : TypedResults.Ok(item);
		}

		/// <summary>
		/// Executes the query passed and returns a FileStreamResult for allowing download of a file or a NotFound() result depending on whether the returned item is null or not
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="query"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<FileStreamHttpResult, NotFound>> ExecuteFileDownloadAsync<TResult>(QueryBase<TResult?> query,
																									 CancellationToken cancellationToken = default) where TResult : FileDownloadDto
		{
			var item = await sender.Send(query, cancellationToken);
			return GetFileDownload(item);
		}

		/// <summary>
		/// Executes the query passed and returns a FileStreamResult for allowing download of a file or a NotFound() result depending on whether the returned item is null or not
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="query"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<FileStreamHttpResult, NotFound>> ExecuteFileDownloadAsync<TResult>(QueryByIdBase<TResult?> query,
																									 CancellationToken cancellationToken = default) where TResult : FileDownloadDto
		{
			var item = await sender.Send(query, cancellationToken);
			return GetFileDownload(item);
		}

		/// <summary>
		/// Executes the command passed and returns the corresponding response that can be either <see cref="Created{TValue}"/> or a <see cref="NotFound"/> or a <see cref="ValidationProblem"/> depending on the validations and processing
		/// </summary>
		/// <param name="command"></param>
		/// <param name="resultUri">The URI to include in the headers of the Created() response</param>
		/// <param name="cancellationToken"></param>
		/// <param name="uriParams">The parameters (if any) to pass for concatenating into the resultUri</param>
		/// <returns></returns>
		public async Task<Results<Created<CreatedResponse>, NotFound, ValidationProblem>> ExecuteCommandCreatedAsync(CommandBase<Guid> command,
																													 string resultUri,
																													 object[]? uriParams = null,
																													 CancellationToken cancellationToken = default)
		{
			var result = await sender.Send(command, cancellationToken);
			return result switch
				   {
					   { ItemNotFound: true } => TypedResults.NotFound(),
					   { ValidationResult.IsValid: false } => TypedResults.ValidationProblem(result.ValidationResult.ToDictionary()),
					   _ => TypedResults.Created(string.Format(resultUri, [.. uriParams ?? [], result.Result]),
												 new CreatedResponse(result.Result))
				   };
		}

		/// <summary>
		/// Executes the command passed and returns the corresponding response that can be either  <see cref="NoContent"/> or a <see cref="NotFound"/> or a <see cref="ValidationProblem"/> depending on the validations and processing
		/// </summary>
		/// <param name="command"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<NoContent, NotFound, ValidationProblem>> ExecuteCommandNoContentAsync(CommandBase command,
																										CancellationToken cancellationToken = default)
		{
			var result = await sender.Send(command, cancellationToken);
			return result switch
				   {
					   { ItemNotFound: true } => TypedResults.NotFound(),
					   { ValidationResult.IsValid: false } => TypedResults.ValidationProblem(result.ValidationResult.ToDictionary()),
					   _ => TypedResults.NoContent()
				   };
		}

		/// <summary>
		/// Executes the command passed and returns the corresponding response that can be either <see cref="Ok"/> or a <see cref="NotFound"/> or a <see cref="ValidationProblem"/> depending on the validations and processing
		/// </summary>
		/// <param name="command"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<Ok, NotFound, ValidationProblem>> ExecuteCommandOkAsync(CommandBase command,
																						  CancellationToken cancellationToken = default)
		{
			var result = await sender.Send(command, cancellationToken);
			return result switch
				   {
					   { ItemNotFound: true } => TypedResults.NotFound(),
					   { ValidationResult.IsValid: false } => TypedResults.ValidationProblem(result.ValidationResult.ToDictionary()),
					   _ => TypedResults.Ok()
				   };
		}

		/// <summary>
		/// Executes the command passed and returns the corresponding response that can be either <see cref="NotFound"/> or a <see cref="ValidationProblem"/> or a user-defined <see cref="IResult"/> depending on the validations and processing
		/// </summary>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="command"></param>
		/// <param name="response"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<TResponse, NotFound, ValidationProblem>> ExecuteCommandAsync<TResponse>(CommandBase command,
																										  TResponse response,
																										  CancellationToken cancellationToken = default) where TResponse : IResult
		{
			var result = await sender.Send(command, cancellationToken);
			return result switch
				   {
					   { ItemNotFound: true } => TypedResults.NotFound(),
					   { ValidationResult.IsValid: false } => TypedResults.ValidationProblem(result.ValidationResult.ToDictionary()),
					   _ => response
				   };
		}

		/// <summary>
		/// Executes the command passed and returns the corresponding response that can be either <see cref="NotFound"/> or a <see cref="ValidationProblem"/> or an <see cref="IResult"/> calculated based on a function, depending on the validations and processing
		/// </summary>
		/// <typeparam name="TResult">The type of the result returned by the command</typeparam>
		/// <typeparam name="TResponse"></typeparam>
		/// <param name="command"></param>
		/// <param name="func">A function to convert the result to the desired response type</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Results<TResponse, NotFound, ValidationProblem>> ExecuteCommandAsync<TResult, TResponse>(CommandBase<TResult> command,
																												   Func<TResult, TResponse> func,
																												   CancellationToken cancellationToken = default) where TResponse : IResult
		{
			var result = await sender.Send(command, cancellationToken);
			return result switch
				   {
					   { ItemNotFound: true } => TypedResults.NotFound(),
					   { ValidationResult.IsValid: false } => TypedResults.ValidationProblem(result.ValidationResult.ToDictionary()),
					   _ => func(result.Result)
				   };
		}
	}
	
	private static Results<FileStreamHttpResult, NotFound> GetFileDownload<TResult>(TResult? item) where TResult : FileDownloadDto =>
		item is null
			? TypedResults.NotFound()
			: TypedResults.File(item.FileContent,
								item.ContentType,
								item.FileName);
}