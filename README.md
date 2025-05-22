# Családi Fotóalbum Kezelő

🇭🇺 **Családi felhasználásra készült webalkalmazás fényképek tárolásához és rendszerezéséhez**

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-blue)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-336791)
![Bootstrap](https://img.shields.io/badge/Bootstrap-Frontend-7952B3)

## 📋 Projekt leírása

Ez a projekt egy **ASP.NET MVC** alapú webalkalmazás, amely családi fotóalbumok kezelésére szolgál. Az alkalmazás lehetővé teszi a felhasználók számára, hogy mappákba rendezzék fényképeiket, különböző metaadatokkal (dátum, helyszín, személyek) láthassák el őket, és különböző szűrési lehetőségeket használjanak.

> **⚠️ Megjegyzés**  
> Ez a projekt egy egyetemi beadandó (_Webalkalmazásfejlesztés (C#)_), ami 2024 decemberében készült körülbelül egy hónap alatt. Tudom, hogy számos fejlesztési lehetőség van, de a kódot nem a _jelenlegi_ tudásom reprezentációjának szánom.

## ✨ Főbb funkciók

### 👤 Felhasználói funkciók
- **Mappa kezelés**: Új mappák létrehozása névvel, dátummal, helyszínnel, személyekkel és egyedi színnel
- **Képkezelés**: Képek feltöltése, megjelenítése különböző méretekben, törlés
- **Szűrési lehetőségek**: Mappák szűrése helyszín, dátum és személyek szerint
- **Kedvencek**: Mappák kedvencekhez adása (arany színnel kiemelve)
- **Statisztikák**: Kördiagram a mappákban található képek számáról
- **Export funkció**: Mappák letöltése ZIP formátumban

### 🔧 Admin funkciók
- **Felhasználó kezelés**: Új felhasználók hozzáadása és törlése
- **Statisztikák**: Vonaldiagram a feltöltött képek számának alakulásáról
- **Export**: Mappa adatok XML formátumba mentése

## 🛠️ Technológiai stack

| Komponens | Technológia |
|-----------|-------------|
| **Framework** | ASP.NET Core MVC (.NET 8.0) |
| **Adatbázis** | PostgreSQL |
| **ORM** | Entity Framework Core 7.0.11 |
| **Frontend** | Bootstrap, Chart.js |
| **Egyéb** | Newtonsoft.Json |

## 📦 NuGet csomagok

```xml
<PackageReference Include="Chart.Mvc" />
<PackageReference Include="Microsoft.AspNetCore.EntityFrameworkCore" Version="7.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="7.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="7.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="7.0.11" />
<PackageReference Include="Newtonsoft.Json" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="7.0.11" />
```

## 🚀 Telepítés és futtatás

### Előfeltételek
- **.NET 8.0 SDK**
- **PostgreSQL szerver**
- **Visual Studio 2022** vagy **Visual Studio Code**

### Telepítés

#### 1. Repository klónozása
```bash
git clone [repository-url]
cd [projekt-mappa]
```

#### 2. Adatbázis konfiguráció
Hozz létre egy PostgreSQL adatbázist, majd módosítsd az `appsettings.json` fájlban a connection stringet:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PhotoAlbumDb;Username=your_username;Password=your_password"
  }
}
```

#### 3. Migrációk futtatása
```bash
dotnet ef database update
```

#### 4. Alkalmazás futtatása
```bash
dotnet run
```

#### 5. Böngészőben megnyitás
- `https://localhost:portszám`

## 📁 Projekt struktúra

```
FamilyPhotos/
├── Controllers/          # MVC kontrollerek
├── Models/               # Adatmodellek és ViewModellek
├── Views/                # Razor nézetek
├── Entites/              # Entity Framework DbContext
├── Migrations/           # EF migrációs fájlok
├── Data/           # A felhasználó által feltöltött fényképek
└── wwwroot/
    ├── css/             # Stíluslapok
    ├── js/              # JavaScript fájlok
    └── uploads/         # Feltöltött képek tárolása
    
```

## 🖼️ Támogatott képformátumok

- **PNG**
- **JPG/JPEG**
- *Egyéb formátumok tesztelése szükséges*

## 🔐 Felhasználói rendszer

Az alkalmazás **regisztrációs és bejelentkezési rendszerrel** rendelkezik. 

## ⚠️ Ismert korlátozások

> **Figyelem**: Az alábbi korlátozások tudatos dokumentálása a tanulási folyamat része.

- A projekt **egyetemi beadandóként** készült, ezért vannak hiányosságai
- Az admin **nem látja az összes felhasználó mappáját** 
- A **letöltési funkció minden felhasználó számára elérhető** 
- **Korlátozott hibakezelés** és validáció
- **Biztonsági megfontolások** nem teljes körűek

## 🚧 Fejlesztési lehetőségek

- [ ] Teljes körű **hibakezelés** implementálása
- [ ] **Biztonsági fejlesztések** (fájlfeltöltés validáció)
- [ ] **Képoptimalizálás** és thumbnail generálás
- [ ] **Keresési funkciók** bővítése
- [ ] **Unit és integrációs tesztek** írása

## 📝 Licensz

Ez a projekt **oktatási célokra** készült és **személyes portfólió** részét képezi.


**Készítés ideje**: 2024. december  
**Projekt típusa**: Egyetemi beadandó

---
