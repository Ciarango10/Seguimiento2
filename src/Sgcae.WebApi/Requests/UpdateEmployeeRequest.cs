namespace Sgcae.WebApi.Requests;

public sealed record UpdateEmployeeRequest(
    string FirstNames,
    string LastNames,
    string Email,
    string JobTitle,
    string Department);
