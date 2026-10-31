using Sgcae.Domain.Common;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.UnitTests.Credentials;

public class CredentialTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 31, 8, 0, 0, TimeSpan.Zero);

    private static Employee NewEmployee() => Employee.Register(
        DocumentNumber.Create("71234567"),
        PersonName.Create("Andrés", "Ríos"),
        EmailAddress.Create("andres@logistica.com"),
        "Coordinador",
        "Seguridad",
        Now);

    private static Credential NewCredential() =>
        Credential.IssueFor(NewEmployee(), CredentialCode.Generate(), Now);

    [Fact]
    public void IssueFor_crea_una_credencial_activa_del_empleado()
    {
        var employee = NewEmployee();

        var credential = Credential.IssueFor(employee, CredentialCode.Generate(), Now);

        Assert.Equal(employee.Id, credential.EmployeeId);
        Assert.Equal(CredentialStatus.Active, credential.Status);
        Assert.True(credential.IsUsable);
    }

    [Fact]
    public void IssueFor_rechaza_empleados_inactivos()
    {
        var employee = NewEmployee();
        employee.Deactivate(Now);

        var error = Assert.Throws<DomainException>(() => Credential.IssueFor(employee, CredentialCode.Generate(), Now));

        Assert.Equal("employee_inactive", error.Code);
    }

    [Fact]
    public void Block_y_Unblock_cambian_el_estado_y_guardan_el_motivo()
    {
        var credential = NewCredential();

        credential.Block("Reporte de pérdida", Now);
        Assert.Equal(CredentialStatus.Blocked, credential.Status);
        Assert.Equal("Reporte de pérdida", credential.StatusReason);
        Assert.False(credential.IsUsable);
        Assert.True(credential.IsCurrent);

        credential.Unblock(Now.AddHours(1));
        Assert.Equal(CredentialStatus.Active, credential.Status);
        Assert.Null(credential.StatusReason);
        Assert.Equal(Now.AddHours(1), credential.StatusChangedAt);
    }

    [Fact]
    public void Block_exige_un_motivo()
    {
        Assert.Throws<DomainException>(() => NewCredential().Block("", Now));
    }

    [Fact]
    public void Una_credencial_revocada_no_admite_cambios()
    {
        var credential = NewCredential();
        credential.Revoke("Retiro", Now);

        Assert.False(credential.IsCurrent);
        Assert.Equal("credential_revoked", Assert.Throws<DomainException>(() => credential.Unblock(Now)).Code);
        Assert.Equal("credential_revoked", Assert.Throws<DomainException>(() => credential.Block("x", Now)).Code);
        Assert.Equal("credential_revoked", Assert.Throws<DomainException>(() => credential.Revoke("x", Now)).Code);
    }

    [Fact]
    public void Unblock_de_una_credencial_activa_incumple_la_regla()
    {
        Assert.Equal("credential_not_blocked", Assert.Throws<DomainException>(() => NewCredential().Unblock(Now)).Code);
    }

    [Fact]
    public void Generate_produce_codigos_validos_y_distintos()
    {
        var codes = Enumerable.Range(0, 200).Select(_ => CredentialCode.Generate().Value).ToList();

        Assert.All(codes, code => Assert.True(CredentialCode.TryCreate(code, out _)));
        Assert.Equal(codes.Count, codes.Distinct().Count());
    }

    [Theory]
    [InlineData("SGC-ABCD-1234")]
    [InlineData("SGC-ABCD-EFG")]
    [InlineData("CRD-ABCD-EFGH")]
    public void CredentialCode_rechaza_formatos_invalidos(string input)
    {
        Assert.False(CredentialCode.TryCreate(input, out _));
    }

    [Fact]
    public void CredentialCode_acepta_minusculas()
    {
        Assert.Equal("SGC-ABCD-EFGH", CredentialCode.Create("sgc-abcd-efgh").Value);
    }
}
