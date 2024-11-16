namespace Note.Domain.Enum
{
    public enum ErrorCodes
    {
        ReportsNotFound = 0,
        ReportNotFound = 1,
        ReportAlreadyExists = 2,


        InternalServerError = 10,

        UserNotFound = 11,
        UserAlreadyExists = 12,
        Unauthorized = 13,
        UserAlreadyExistsThisRole = 14,

        PasswordNotEqualPasswordConfirm = 21,
        PasswordIsWrong = 22,

        RoleAlreadyExists = 31,
        RoleNotFound = 32
    }
}
