namespace Note.Domain.Interfaces.Database;

public interface IStateSaveChanges
{
    /// <summary>
    /// Метод сохранения обновления.
    /// </summary>
    public Task<int> SaveChangeAsync();
}
