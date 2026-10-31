using Sgcae.Application.AccessHistory;
using Sgcae.Domain.AccessControl;

namespace Sgcae.UnitTests.AccessHistory;

public class GetAccessHistoryValidatorTests
{
    private readonly GetAccessHistoryValidator _validator = new();

    [Fact]
    public void Acepta_una_consulta_sin_filtros()
    {
        Assert.True(_validator.Validate(new GetAccessHistoryQuery(null, null, null, null, null)).IsValid);
    }

    [Fact]
    public void Acepta_todos_los_filtros_combinados()
    {
        var query = new GetAccessHistoryQuery(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 10, 31, 0, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            AccessResult.Denied,
            Page: 2,
            PageSize: 50);

        Assert.True(_validator.Validate(query).IsValid);
    }

    [Fact]
    public void Rechaza_un_rango_de_fechas_invertido()
    {
        var query = new GetAccessHistoryQuery(
            null,
            new DateTimeOffset(2026, 12, 31, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            null,
            null);

        var result = _validator.Validate(query);

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetAccessHistoryQuery.To));
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public void Rechaza_paginacion_fuera_de_rango(int page, int pageSize)
    {
        var query = new GetAccessHistoryQuery(null, null, null, null, null, page, pageSize);

        Assert.False(_validator.Validate(query).IsValid);
    }
}
