namespace Monaco.Template.Backend.Common.Application.Commands.Contracts;

/// <summary>
/// Marker interface that indicates a command is safe to retry on
/// <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/>.
/// </summary>
/// <remarks>
/// Commands that implement this interface are retried by the
/// <see cref="Commands.Behaviors.ConcurrencyExceptionBehavior{TCommand}"/> using a resilience pipeline
/// when a <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/> occurs.
/// The change tracker is cleared between attempts to reset tracked entity states.
/// If all retries are exhausted, a <see cref="CommandResult"/> with a concurrency-conflicted status is returned.
/// <para>
/// Commands that do not implement this interface receive the safe default: no retry is attempted and
/// a concurrency-conflicted <see cref="CommandResult"/> is returned immediately on the first occurrence,
/// allowing the caller to handle the conflict explicitly.
/// </para>
/// </remarks>
public interface IConcurrencyRetriable;