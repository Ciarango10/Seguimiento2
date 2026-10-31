using Sgcae.Domain.AccessPoints;
using Sgcae.Domain.Common;

namespace Sgcae.UnitTests.AccessPoints;

public class AccessPointTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 31, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Register_crea_el_punto_habilitado()
    {
        var accessPoint = AccessPoint.Register(
            AccessPointCode.Create("REC-01"), "Recepción", AccessPointKind.Reception, "Piso 1", Now);

        Assert.True(accessPoint.IsEnabled);
        Assert.NotEqual(Guid.Empty, accessPoint.Id);
        Assert.Equal(Now, accessPoint.RegisteredAt);
    }

    [Fact]
    public void Disable_dos_veces_incumple_la_regla()
    {
        var accessPoint = AccessPoint.Register(
            AccessPointCode.Create("BOD-01"), "Bodega", AccessPointKind.Warehouse, "Bloque B", Now);
        accessPoint.Disable();

        var error = Assert.Throws<DomainException>(accessPoint.Disable);

        Assert.Equal("access_point_already_disabled", error.Code);
    }

    [Fact]
    public void Register_rechaza_un_tipo_no_definido()
    {
        Assert.Throws<DomainException>(() => AccessPoint.Register(
            AccessPointCode.Create("REC-02"), "Recepción", (AccessPointKind)42, "Piso 2", Now));
    }

    [Theory]
    [InlineData("rec-01", "REC-01")]
    [InlineData(" parq01 ", "PARQ01")]
    public void AccessPointCode_se_normaliza_a_mayusculas(string input, string expected)
    {
        Assert.Equal(expected, AccessPointCode.Create(input).Value);
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("REC 01")]
    [InlineData("-REC")]
    public void AccessPointCode_rechaza_formatos_invalidos(string input)
    {
        Assert.False(AccessPointCode.TryCreate(input, out _));
    }
}
