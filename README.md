# IdőKocka - Pomodoro + Teendőkezelő

## Funkciók
- 25 perc munka + 5 perc szünet (automatikus váltás, hangjelzés)
- Teendők: hozzáadás, szerkesztés, törlés, prioritás
- Lokális adatmentés (SQLite)

## Technológia
- .NET MAUI
- MVVM + CommunityToolkit.Mvvm
- SQLite (sqlite-net-pcl)
- Data binding mindenhol

## Futtatás
1. Nyissa meg a `.sln` fájlt Visual Studio 2022-ben
2. Válasszon platformot (Android/iOS/Windows)
3. Futtassa (F5)

## Adatbázis
- `idokocka.db3` → `FileSystem.AppDataDirectory`