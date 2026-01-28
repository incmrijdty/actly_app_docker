# Actly – platforma łącząca wolontariuszy z lokalnymi inicjatywami

## Temat projektu
**Actly** to aplikacja webowa, której celem jest łączenie osób chcących zaangażować się społecznie  
z lokalnymi wydarzeniami wolontariackimi oraz inicjatywami pomocowymi.

Projekt wspiera ideę budowania silniejszych społeczności lokalnych poprzez:
- ułatwienie dostępu do informacji o wydarzeniach wolontariackich,
- zachęcanie do aktywnego udziału w inicjatywach społecznych,
- umożliwienie organizatorom dotarcia do osób gotowych nieść pomoc.

---

## Autor
- **Imię i nazwisko:** Alesia Sichova  
- **Numer albumu:** 52307

---

## Główne funkcjonalności aplikacji
- Rejestracja i logowanie użytkowników
- Autoryzacja oparta o JWT
- Przeglądanie dostępnych wydarzeń wolontariackich
- Zarządzanie wydarzeniami (tworzenie, edycja, usuwanie – po stronie API)
- Podstawowy profil użytkownika z historią aktywności i danymi użytkownika
- Komunikacja frontend–backend poprzez REST API
- Bezpieczne przechowywanie danych użytkowników

---

## Wykorzystane technologie
- **Backend:** ASP.NET Core 9 (Web API)
- **Frontend:** Angular
- **Baza danych:** PostgreSQL
- **Serwer frontendowy:** Nginx
- **Konteneryzacja:** Docker & Docker Compose
- **Autoryzacja:** JWT (JSON Web Token)
- **Integracja z mapą:** Nominatim API
- **Dokumentacja:** Swagger
- **Kontrola wersji:** Git

---

## Uruchomienie projektu

### Wymagania
- Docker
- Docker Compose

---

### Kroki uruchomienia

1. Po skopiowaniu repozytorium utwórz plik z poufnymi danymi .env w katalogu głównym projektu (obok docker-compose.yaml i README.md) w oparciu o następujący przykład:

POSTGRES_DB=nazwa_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=db_haslo

JWT_KEY=dlugi_tajny_klucz_min_32_znaki
 
2. Uruchom aplikację with docker compose:

- docker compose up --build

---

### Dostęp do aplikacji

Po poprawnym uruchomieniu kontenerów:

Frontend (aplikacja):
http://localhost:4201

Backend – Swagger (API):
http://localhost:5127/swagger

Baza danych PostgreSQL (opcjonalnie, ja używałam pgAdmin):
localhost:5433