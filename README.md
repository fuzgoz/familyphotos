Családi Fotóalbum Kezelő
🇭🇺 Családi felhasználásra készült webalkalmazás fényképek tárolásához és rendszerezéséhez
📋 Projekt leírása
Ez a projekt egy ASP.NET MVC alapú webalkalmazás, amely családi fotóalbumok kezelésére szolgál. Az alkalmazás lehetővé teszi a felhasználók számára, hogy mappákba rendezzék fényképeiket, különböző metaadatokkal (dátum, helyszín, személyek) láthassák el őket, és különböző szűrési lehetőségeket használjanak.

⚠️ Fontos megjegyzés: Ez a projekt 2024 elején készült egyetemi beadandóként, körülbelül egy hónap alatt. A kód nem reprezentálja a jelenlegi tudásszintemet, és tisztában vagyok vele, hogy számos fejlesztési lehetőség van benne.

✨ Főbb funkciók
👤 Felhasználói funkciók

Mappa kezelés: Új mappák létrehozása névvel, dátummal, helyszínnel, személyekkel és egyedi színnel
Képkezelés: Képek feltöltése, megjelenítése különböző méretekben, törlés
Szűrési lehetőségek: Mappák szűrése helyszín, dátum és személyek szerint
Kedvencek: Mappák kedvencekhez adása (arany színnel kiemelve)
Statisztikák: Kördiagram a mappákban található képek számáról
Export funkció: Mappák letöltése ZIP formátumban

🔧 Admin funkciók

Felhasználó kezelés: Új felhasználók hozzáadása és törlése
Statisztikák: Vonaldiagram a feltöltött képek számának alakulásáról
Export: Mappa adatok XML formátumba exportálása

🛠️ Technológiai stack

Framework: ASP.NET Core MVC (.NET 8.0)
Adatbázis: PostgreSQL
ORM: Entity Framework Core 7.0.11
Frontend: Bootstrap, Chart.js
Egyéb: Newtonsoft.Json

📦 NuGet csomagok
xml<PackageReference Include="Chart.Mvc" />
<PackageReference Include="Microsoft.AspNetCore.EntityFrameworkCore" Version="7.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="7.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="7.0.11" />
<PackageReference Include="Newtonsoft.Json" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="7.0.11" />
🚀 Telepítés és futtatás
Előfeltételek

.NET 8.0 SDK
PostgreSQL
Visual Studio 2022 vagy Visual Studio Code

Lépések

Repository klónozása
bashgit clone [repository-url]
cd [projekt-mappa]

Adatbázis konfiguráció

Hozz létre egy PostgreSQL adatbázist
Módosítsd a appsettings.json fájlban a connection stringet:

json{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PhotoAlbumDb;Username=your_username;Password=your_password"
  }
}

Migrációk futtatása
bashdotnet ef database update

Alkalmazás indítása
bashdotnet run

Böngészőben megnyitás

Navigálj a https://localhost:5001 vagy http://localhost:5000 címre



📁 Projekt struktúra
├── Controllers/        # MVC kontrollerek
├── Models/            # Adatmodellek és ViewModellek
├── Views/             # Razor nézetek
├── Data/              # Entity Framework DbContext
├── Migrations/        # EF migrációs fájlok
└── wwwroot/
    ├── css/          # Stíluslapok
    ├── js/           # JavaScript fájlok
    └── uploads/      # Feltöltött képek tárolása
🖼️ Támogatott képformátumok

PNG
JPG/JPEG
(Egyéb formátumok tesztelése szükséges)

🔐 Felhasználói rendszer
Az alkalmazás beépített regisztrációs és bejelentkezési rendszerrel rendelkezik. Az első regisztrált felhasználó automatikusan admin jogosultságot kap.
⚠️ Ismert korlátozások

A projekt egyetemi beadandóként készült, ezért vannak hiányosságai
Az admin nem látja az összes felhasználó mappáját (a specifikációtól eltérően)
A letöltési funkció minden felhasználó számára elérhető (nem csak admin)
Korlátozott hibakezelés és validáció
Biztonsági megfontolások nem teljes körűek

🚧 Fejlesztési lehetőségek

Teljes körű hibakezelés implementálása
Biztonsági fejlesztések (fájlfeltöltés validáció, SQL injection védelem)
Responsive design javítása
Képoptimalizálás és thumbnail generálás
Keresési funkciók bővítése
API végpontok hozzáadása
Unit és integrációs tesztek írása

📝 Licensz
Ez a projekt oktatási célokra készült és személyes portfólió részét képezi.
👨‍💻 Szerző
Készítés ideje: 2024. december
Projekt típusa: Egyetemi beadandó

Ez a projekt egy korai munkám, amely bemutatja az ASP.NET MVC, Entity Framework és PostgreSQL használatának alapjait. A jelenlegi tudásszintem jelentősen meghaladja az itt bemutatott megoldásokat.
