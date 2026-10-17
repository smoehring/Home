namespace Smoehring.Home.Application.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public sealed class NotFoundException(string what) : Exception($"{what} was not found.");
