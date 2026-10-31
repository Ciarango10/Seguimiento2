using Sgcae.Domain.AccessControl;
using Sgcae.Domain.AccessPoints;
using Sgcae.Domain.Common;
using Sgcae.Domain.Credentials;
using Sgcae.Domain.Employees;

namespace Sgcae.UnitTests.AccessControl;

public class AccessGateTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 31, 8, 0, 0, TimeSpan.Zero);

    private readonly Employee _employee = Employee.Register(
        DocumentNumber.Create("1036789456"),
        PersonName.Create("Laura", "Gómez"),
        EmailAddress.Create("laura@logistica.com"),
        "Analista",
        "Bodega",
        Now);

    private readonly AccessPoint _reception = AccessPoint.Register(
        AccessPointCode.Create("REC-01"), "Recepción", AccessPointKind.Reception, "Piso 1", Now);

    private readonly Credential _credential;

    public AccessGateTests()
    {
        _credential = Credential.IssueFor(_employee, CredentialCode.Generate(), Now);
    }

    private AccessAttempt Attempt(
        MovementDirection direction = MovementDirection.Entry,
        MovementDirection? lastGranted = null) =>
        new(_credential.Code.Value, "REC-01", direction, _credential, _employee, _reception, lastGranted);

    [Fact]
    public void Autoriza_la_entrada_de_un_empleado_activo_con_credencial_activa()
    {
        var decision = AccessGate.Evaluate(Attempt());

        Assert.True(decision.IsGranted);
        Assert.Null(decision.Reason);
    }

    [Fact]
    public void Autoriza_la_salida_despues_de_una_entrada()
    {
        Assert.True(AccessGate.Evaluate(Attempt(MovementDirection.Exit, MovementDirection.Entry)).IsGranted);
    }

    [Fact]
    public void Rechaza_una_credencial_desconocida()
    {
        var attempt = Attempt() with { Credential = null, Employee = null };

        Assert.Equal(DenialReason.CredentialNotFound, AccessGate.Evaluate(attempt).Reason);
    }

    [Fact]
    public void Rechaza_una_credencial_bloqueada()
    {
        _credential.Block("Pérdida", Now);

        Assert.Equal(DenialReason.CredentialBlocked, AccessGate.Evaluate(Attempt()).Reason);
    }

    [Fact]
    public void Rechaza_una_credencial_revocada()
    {
        _credential.Revoke("Retiro", Now);

        Assert.Equal(DenialReason.CredentialRevoked, AccessGate.Evaluate(Attempt()).Reason);
    }

    [Fact]
    public void Rechaza_a_un_empleado_inactivo_aunque_la_credencial_siga_activa()
    {
        _employee.Deactivate(Now);

        Assert.Equal(DenialReason.EmployeeInactive, AccessGate.Evaluate(Attempt()).Reason);
    }

    [Fact]
    public void Rechaza_un_punto_de_acceso_inexistente_o_deshabilitado()
    {
        Assert.Equal(DenialReason.AccessPointNotFound, AccessGate.Evaluate(Attempt() with { AccessPoint = null }).Reason);

        _reception.Disable();
        Assert.Equal(DenialReason.AccessPointDisabled, AccessGate.Evaluate(Attempt()).Reason);
    }

    [Fact]
    public void Rechaza_dos_entradas_seguidas()
    {
        Assert.Equal(DenialReason.AlreadyInside, AccessGate.Evaluate(Attempt(MovementDirection.Entry, MovementDirection.Entry)).Reason);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(MovementDirection.Exit)]
    public void Rechaza_una_salida_sin_entrada_previa(MovementDirection? lastGranted)
    {
        Assert.Equal(DenialReason.NotInside, AccessGate.Evaluate(Attempt(MovementDirection.Exit, lastGranted)).Reason);
    }

    [Fact]
    public void Record_guarda_el_resultado_y_los_codigos_presentados()
    {
        var attempt = Attempt() with { PresentedAccessPointCode = " rec-01 " };
        var decision = AccessGate.Evaluate(attempt);

        var accessEvent = AccessEvent.Record(attempt, decision, Now);

        Assert.Equal(AccessResult.Granted, accessEvent.Result);
        Assert.Equal("REC-01", accessEvent.PresentedAccessPointCode);
        Assert.Equal(_employee.Id, accessEvent.EmployeeId);
        Assert.Equal(_reception.Id, accessEvent.AccessPointId);
        Assert.Equal(Now, accessEvent.OccurredAt);
    }
}
