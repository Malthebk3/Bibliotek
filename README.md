# 📚 Bibliotek – Biblioteksstyringssystem

Gennemgående opgave for både OOP og GUI. Løsningen er bygget i C#/.NET med **Avalonia UI** efter MVVM-mønsteret og består af tre projekter:

- **Bibliotek.Core** – forretningslogik (klasser, interfaces, persistens). Har ingen afhængigheder til UI.
- **Bibliotek.Avalonia** – grafisk brugergrænseflade (Views, ViewModels og Converters).
- **Bibliotek.Tests** – automatiserede unit tests (xUnit).

[![.NET CI](https://github.com/malthebk3/bibliotek/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/malthebk3/bibliotek/actions)

## 🚀 Kom i gang

**Krav:** .NET 8 SDK

```bash
# Kør appen
dotnet run --project Bibliotek.Avalonia

# Kør alle unit tests
dotnet test
```

Ved første opstart seedes appen automatisk med 6 bøger og 3 brugere (heraf én Premium-bruger) inklusive forud-udlånte bøger, så UI'en straks viser både "Tilgængelig"- og "Udlånt"-tilstande. Data gemmes i `library_data.json` og indlæses automatisk igen ved næste opstart.

## 🗂️ Projektstruktur

```
Bibliotek/
├── .github/workflows/dotnet-ci.yml   # CI/CD: build + tests ved hvert push
├── Bibliotek.Core/                   # Forretningslogik (ingen UI-afhængighed)
│   ├── Interfaces/                   # IBook, IUser, ILibrary, ILibraryRepository
│   ├── Models/                       # Book, User, PremiumUser, Library
│   └── Data/                         # JsonLibraryRepository (persistens)
├── Bibliotek.Avalonia/               # GUI (MVVM)
│   ├── Views/                        # MainWindow, UserDetailWindow, BookDetailWindow
│   ├── ViewModels/                   # MainViewModel, UserDetailViewModel, BookDetailViewModel
│   └── Converters/                   # BoolToUserTypeConverter, BoolToIsAvailableConverter
├── Bibliotek.Tests/                  # xUnit-tests
│   ├── BookTests.cs
│   ├── UserTests.cs
│   ├── LibraryTests.cs
│   └── JsonLibraryRepositoryTests.cs
└── Bibliotek.sln
```

## ✅ Opgavekrav (PDF)

| Krav | Løsning |
| --- | --- |
| `Book` med Title, Author, ISBN, IsAvailable | `Core/Models/Book.cs` |
| `User` med Name, UserId, BorrowedBooks | `Core/Models/User.cs` |
| `PremiumUser` arver fra `User`, må låne 1 ekstra bog | `Core/Models/PremiumUser.cs` (5 bøger mod 4) |
| `Library` med Books, Users og alle metoder | `Core/Models/Library.cs` |
| `BorrowBook`, `ReturnBook`, `AddBook`, `RemoveBook`, `RegisterUser`, `FindBookByISBN` | Implementeret i `User`/`Library` |

### Indkapsling
`IsAvailable` har en `private set` og kan **kun** ændres gennem låne-/returneringsmetoderne:

```csharp
public bool IsAvailable { get; private set; }
```

### Polymorfi
`PremiumUser` overskriver `BorrowBook` og låneloftet, så en premium-bruger kan låne 5 bøger, mens en almindelig bruger stoppes ved 4. Verificeret med unit tests i `UserTests.cs`.

### Forretningsregler (håndhæves i Core)
- En **udlånt bog kan ikke slettes** (`RemoveBook` kaster `InvalidOperationException`).
- En **bruger med udlånte bøger kan ikke slettes** (`RemoveUser` kaster `InvalidOperationException`).
- Almindelig bruger: maks. 4 bøger. Premium-bruger: maks. 5 bøger.

## 🧑‍🏫 Ekstra krav

### 1) Interfaces (løs kobling)
Al kommunikation sker gennem interfaces: `IBook`, `IUser`, `ILibrary` og `ILibraryRepository`. ViewModels og persistens-laget koder mod abstraktioner, ikke konkrete klasser – hvilket også muliggør en fake repository i tests.

### 2) Function pointers / delegates
- `Library.FindBooks(Func<IBook, bool> predicate)` – generisk søgning vha. delegate. Bruges bl.a. i søgefeltet i bruger-detaljevinduet:
  ```csharp
  _library.FindBooks(b => b.IsAvailable && b.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
  ```
- `Action`-callbacks bruges til at lukke detaljevinduer, og `Closed`-events subscribes med lambdas, f.eks. `(s, e) => RefreshTables()`.

### 3) Automatiserede unit tests
29 xUnit-tests dækker forretningslogik og persistens: lånelofter, indkapsling, polymorfi, ID-generering, søgning med delegates, forretningsregler for sletning samt save/load-roundtrip.

### 4) CI/CD
GitHub Actions-workflow (`.github/workflows/dotnet-ci.yml`) kører automatisk **build + alle tests** ved hvert push og pull request. Grøn check = koden virker.

### 5) Persistens
`JsonLibraryRepository` gemmer/indlæser hele biblioteket som JSON. Designet er **relationelt**: brugere gemmer kun ISBN som "foreign key" på lånte bøger, præcis som i en SQL-database – hvilket gør en fremtidig migration til database ligetil.

## 🖥️ Sådan bruges appen

**Hovedvindue (2×2 grid):**
- Tilføj bøger og brugere via formularerne (bruger-ID genereres automatisk, f.eks. `U0004`).
- Scrollbare tabeller med fastgjorte knapper; statuslinje nederst med feedback (f.eks. *"Registreret: Anders (ID: U0004)"*).
- Tabellerne bruger **value converters**, så der står "Tilgængelig"/"Udlånt" og "Standard"/"Premium" i stedet for True/False.
- Markér en række og klik *Vis bog detaljer* / *Vis bruger detaljer*.

**Bog-detaljer:** Redigér titel/forfatter, slet bogen (blokeres af Core hvis udlånt), og se hvem der har lånt den.

**Bruger-detaljer:** Redigér navn, slet brugeren (blokeres af Core ved udlånte bøger), returnér lånte bøger, og lån nye bøger via søgebaren (søgningen bruger function pointers).

## 🏗️ Designbeslutninger

- **MVVM med CommunityToolkit.Mvvm** – `[ObservableProperty]` og `[RelayCommand]` holder ViewModels rene og testbare.
- **Tynde ViewModels** – forretningsreglerne bor i Core (`RemoveBook`/`RemoveUser` kaster exceptions ved regelbrud), mens ViewModels kun står for præsentationsvalidering (tomme felter) og at vise statusbeskeder. Reglerne kan dermed ikke omgås af en anden UI.
- **DTO-mønster til persistens** – domæneklasserne bevarer deres strenge indkapsling, mens en simpel datastruktur (`LibrarySaveData`) står for JSON-serialiseringen.
- **Auto-genererede bruger-ID'er** beregnes ud fra eksisterende data (ikke en static counter), så ID'er aldrig kolliderer efter genindlæsning.
- **Value converters** (`BoolToUserTypeConverter`, `BoolToIsAvailableConverter`) oversætter bools til læsbar tekst i UI'en.

## 🧪 Tests & CI

```bash
dotnet test          # lokalt
git push             # udløser automatisk CI-pipeline på GitHub
```