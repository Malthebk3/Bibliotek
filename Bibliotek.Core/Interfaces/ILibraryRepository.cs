using Bibliotek.Core.Models;

namespace Bibliotek.Core.Interfaces;

public interface ILibraryRepository
{
    void Save(Library library);
    Library Load();
}