namespace Note.Domain.Enum;

/// <summary>
/// Перечисление кодов ошибок.
/// </summary>
public enum ErrorCodes
{
    ReportsNotFound = 0,
    ReportNotFound = 1,
    ReportAlreadyExists = 2,
    ReportAlreadyShared = 3,
    YouCantShareTheReportWithYourself = 4,
    YouDontHaveTheRightsToShareThisReport = 5,
    YouDontHaveTheRrightsToEditThisReport = 6,

    InvalidClientRequest = 9,
    InternalServerError = 10,

    UserNotFound = 11,
    UserAlreadyExists = 12,
    Unauthorized = 13,
    UserAlreadyExistsThisRole = 14,

    PasswordNotEqualPasswordConfirm = 21,
    PasswordIsWrong = 22,
    OldPasswordNotEqualEntryPassword = 23,
    MinimalLengthLoginIsThreeSymbols = 24,

    RoleAlreadyExists = 31,
    RoleNotFound = 32
}