using Sgcae.Domain.Common;

namespace Sgcae.UnitTests.Common;

public class EmailAddressTests
{
    [Fact]
    public void Create_normaliza_a_minusculas_y_quita_espacios()
    {
        var email = EmailAddress.Create("  Ana.Perez@Empresa.COM ");

        Assert.Equal("ana.perez@empresa.com", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sin-arroba")]
    [InlineData("ana@empresa")]
    public void Create_rechaza_correos_invalidos(string value)
    {
        Assert.Throws<DomainException>(() => EmailAddress.Create(value));
    }

    [Fact]
    public void Dos_correos_con_el_mismo_valor_son_iguales()
    {
        Assert.Equal(EmailAddress.Create("ana@empresa.com"), EmailAddress.Create("ANA@empresa.com"));
    }
}
