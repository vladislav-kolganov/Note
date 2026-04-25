namespace Note.Domain.Result;

/// <summary>
/// Базовая модель для ответа.
/// </summary>
public class BaseResult
{
    public bool IsSuccess => ErrorMessage == null;

    public string ErrorMessage { get; set; }

    public int? ErrorCode { get; set; }
}

public class BaseResult<T> : BaseResult
{
    public T Data { get; set; }

    public BaseResult() { }

    public BaseResult(string errorMassage, int? errorCode, T data)
    {
        ErrorMessage = errorMassage;
        ErrorCode = errorCode;
        Data = data;
    }
}