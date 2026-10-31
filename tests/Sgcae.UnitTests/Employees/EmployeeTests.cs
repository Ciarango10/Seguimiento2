using Sgcae.Domain.Common;
using Sgcae.Domain.Employees;

namespace Sgcae.UnitTests.Employees;

public class EmployeeTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 31, 8, 0, 0, TimeSpan.Zero);

    private static Employee NewEmployee() => Employee.Register(
        DocumentNumber.Create("1036789456"),
        PersonName.Create("Laura", "Gómez"),
        EmailAddress.Create("laura@logistica.com"),
        "Analista",
        "Bodega",
        Now);

    [Fact]
    public void Register_crea_un_empleado_activo()
    {
        var employee = NewEmployee();

        Assert.True(employee.IsActive);
        Assert.Equal("Laura Gómez", employee.Name.FullName);
        Assert.Null(employee.DeactivatedAt);
    }

    [Fact]
    public void Deactivate_marca_la_fecha_y_no_se_puede_repetir()
    {
        var employee = NewEmployee();

        employee.Deactivate(Now);

        Assert.Equal(EmploymentStatus.Inactive, employee.Status);
        Assert.Equal(Now, employee.DeactivatedAt);
        Assert.Equal("employee_already_inactive", Assert.Throws<DomainException>(() => employee.Deactivate(Now)).Code);
    }

    [Fact]
    public void UpdateProfile_no_se_permite_si_el_empleado_esta_inactivo()
    {
        var employee = NewEmployee();
        employee.Deactivate(Now);

        var error = Assert.Throws<DomainException>(() => employee.UpdateProfile(
            PersonName.Create("Laura", "Gómez"), EmailAddress.Create("laura@logistica.com"), "Líder", "Bodega"));

        Assert.Equal("employee_inactive", error.Code);
    }

    [Fact]
    public void Reactivate_limpia_la_fecha_de_desactivacion()
    {
        var employee = NewEmployee();
        employee.Deactivate(Now);

        employee.Reactivate();

        Assert.True(employee.IsActive);
        Assert.Null(employee.DeactivatedAt);
    }

    [Fact]
    public void Register_exige_cargo_y_area()
    {
        Assert.Throws<DomainException>(() => Employee.Register(
            DocumentNumber.Create("1036789456"),
            PersonName.Create("Laura", "Gómez"),
            EmailAddress.Create("laura@logistica.com"),
            " ",
            "Bodega",
            Now));
    }

    [Theory]
    [InlineData("ab-123", "AB-123")]
    [InlineData(" 71234567 ", "71234567")]
    public void DocumentNumber_se_normaliza(string input, string expected)
    {
        Assert.Equal(expected, DocumentNumber.Create(input).Value);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12 345 678")]
    public void DocumentNumber_rechaza_formatos_invalidos(string input)
    {
        Assert.False(DocumentNumber.TryCreate(input, out _));
    }
}
