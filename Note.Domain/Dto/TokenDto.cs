namespace Note.Domain.Dto;

public class TokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public long? UserId { get; set; }
}

