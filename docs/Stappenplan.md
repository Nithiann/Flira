# Flira — Van Projectplan naar Uitvoering

Dit document vertaalt het projectplan naar drie concrete onderdelen: de opstartstappen (initiële push), een volledige taken-/backlogstructuur, en een aanpak om het project daadwerkelijk te realiseren.

---

## 1. Opstartstappen — De initiële push

Dit zijn de stappen om van niets naar een werkende, gestructureerde repository te komen, in logische volgorde.

### 1.1 Repository & tooling

1. Maak een nieuwe Git-repository aan op GitHub (`jira-dark`), inclusief `.gitignore` (Visual Studio/.NET + Angular/Node template) en `README.md`.
2. Richt branch-strategie in (bijv. `main` + `develop`, of trunk-based met feature branches en PR-verplichting).
3. Voeg een `LICENSE`-bestand toe indien relevant.
4. Configureer basis GitHub-instellingen: branch protection op `main`, verplichte PR-reviews.

### 1.2 Backend-skelet opzetten

1. Maak de .NET 10 solution aan (`Flira.sln`).
2. Voeg de projecten toe volgens Clean Architecture:
   - `Flira.Domain`
   - `Flira.Application`
   - `Flira.Infrastructure`
   - `Flira.Persistence`
   - `Flira.Shared`
   - `Flira.Api`
3. Leg projectreferenties correct vast (Domain heeft geen dependencies; Application → Domain + Shared; Infrastructure/Persistence → Application; Api → Application + Infrastructure + Persistence).
4. Installeer basis NuGet-packages: EF Core, MediatR, FluentValidation, AutoMapper, Serilog, Swashbuckle (Swagger), JWT-bearer packages, ASP.NET Identity.
5. Zet een `Result`-pattern en basis-abstracties op in `Shared` (bijv. `Result<T>`, `Error`, `PagedList<T>`).
6. Configureer Serilog voor gestructureerde logging (console + file/sink) inclusief correlation-id middleware.

### 1.3 Database & Identity

1. Richt PostgreSQL in (lokaal via Docker) en configureer de connection string via `appsettings.Development.json` + user-secrets.
2. Configureer `DbContext` in `Persistence` en koppel ASP.NET Identity aan de PostgreSQL-store.
3. Maak de eerste EF Core-migratie (Identity-tabellen) en verifieer dat deze succesvol draait.
4. Configureer JWT-authenticatie (issuer, audience, signing key, refresh-token opzet) in de API-laag.

### 1.4 Frontend-skelet opzetten

1. Genereer de Angular 20+ workspace met routing en SCSS.
2. Voeg Angular Material toe en configureer een basis thema (incl. dark mode-variabelen).
3. Richt de modulaire mappenstructuur in: `core`, `shared`, `features/<domein>`, `state`.
4. Configureer de JWT-interceptor en een basis `AuthService` (nog zonder echte endpoints, als stub).
5. Voeg `ngx-translate` toe met een basis taalbestand (bijv. NL/EN).
6. Configureer NgRx Signal Store of RxJS-services als state-aanpak (keuze vastleggen in een ADR, zie 3.2).

### 1.5 Containerisatie & CI

1. Schrijf een `Dockerfile` voor de API en een voor de Angular-app (multi-stage build).
2. Schrijf `docker-compose.yml` met services: `api`, `frontend`, `postgres` (en later `pgadmin` optioneel).
3. Verifieer dat `docker compose up` een werkend, leeg skelet oplevert (API health-endpoint + Angular welkomstpagina).
4. Zet een eerste GitHub Actions workflow op: build + restore backend, build frontend, linting.
5. Voeg Swagger/OpenAPI toe aan de API en verifieer dat de documentatie zichtbaar is op `/swagger`.

### 1.6 Eerste commit & oplevering

1. Commit het volledige skelet als "initial scaffold" met een duidelijke commit message en PR-template.
2. Documenteer in de README: hoe lokaal te starten (Docker Compose), hoe migraties te draaien, hoe tests te draaien.
3. Maak een projectbord aan (bijv. GitHub Projects) met de kolommen uit paragraaf 2 hieronder, en zet de backlog erin.

Na deze stappen heb je een leeg maar volledig werkend fundament: draaiende API met Swagger, draaiende Angular-app, PostgreSQL-verbinding, Identity/JWT-basis, Docker Compose, en een CI-pipeline die bouwt en test.

---

## 2. Volledige takenlijst (backlog)

Onderstaande lijst is direct afgeleid uit de tekst en gegroepeerd per domein/fase, zodat die 1-op-1 in een backlog (Jira/GitHub Projects) kan worden gezet.

### Fase 1 — Fundament

- [ ] Solution-structuur opzetten (Clean Architecture-lagen)
- [ ] Domain/Application/Infrastructure/Persistence/Shared/API scheiden
- [ ] ASP.NET Identity configureren
- [ ] JWT-authenticatie + refresh tokens configureren
- [ ] PostgreSQL + EF Core inrichten
- [ ] Basis CI/CD-pipeline (GitHub Actions)
- [ ] Docker + Docker Compose opzetten
- [ ] Swagger/OpenAPI-basisdocumentatie

### Fase 2 — Kernfunctionaliteit

**Authenticatie & identiteit**

- [ ] Registreren
- [ ] Login
- [ ] Refresh tokens
- [ ] Forgot password
- [ ] Email-verificatie
- [ ] MFA (optioneel, instelbaar)

**Organisaties & teams**

- [ ] Meerdere organisaties per gebruiker
- [ ] Teams binnen organisaties
- [ ] Gebruikersbeheer per organisatie
- [ ] Rollen en permissions (role- en permission-based access control)
- [ ] Data-isolatie/multi-tenant afbakening op organisatieniveau

**Projecten**

- [ ] Project aanmaken, wijzigen, verwijderen
- [ ] Project-attributen: naam, omschrijving, kleur, icoon
- [ ] Koppeling project ↔ organisatie

**Boards & kolommen**

- [ ] Boardbeheer per project
- [ ] Standaard Kanban-board met kolommen (Backlog, Todo, In Progress, Review, Done)
- [ ] Kolommen beheren (aanmaken/wijzigen/volgorde)
- [ ] Drag & drop van taken tussen kolommen

**Taken**

- [ ] Taken aanmaken, bewerken, verwijderen
- [ ] Velden: titel, beschrijving, labels, priority, status, assignee, reporter, due date, estimated hours, attachments
- [ ] Labels en prioriteit voor filtering/triage
- [ ] Koppeling taakstatus ↔ boardkolom

**CQRS & validatie**

- [ ] Command/Query-objecten per use case
- [ ] MediatR-handlers per command/query
- [ ] FluentValidation-validators per command/query
- [ ] Result Pattern consistent toepassen
- [ ] AutoMapper-profielen (domain ↔ entity ↔ DTO)

### Fase 3 — Samenwerking

- [ ] SignalR-hub voor realtime boardupdates
- [ ] Realtime notificaties (in-app push)
- [ ] Notificatietriggers: nieuwe taken, assignments, comments
- [ ] Losgekoppelde notificatie-architectuur (nieuwe triggers makkelijk toevoegen)
- [ ] Comments op taken
- [ ] Mentions in comments (met notificatie)
- [ ] Markdown-ondersteuning in comments
- [ ] Bestandsuploads (attachments) op taken en comments
- [ ] Storage-abstractie (Azure Blob Storage / Local Storage inwisselbaar)
- [ ] Bestandsmetadata opslaan in database

### Fase 4 — Rapportage & admin

- [ ] Dashboard: Open Tasks widget
- [ ] Dashboard: Completed Tasks widget
- [ ] Dashboard: Burndown chart
- [ ] Dashboard: Team Velocity chart
- [ ] Dashboard: Calendar view
- [ ] Read-geoptimaliseerde query-endpoints voor dashboard-data
- [ ] Globale zoekfunctie (projecten, taken, labels, gebruikers)
- [ ] Filteropties: labels, users, project, status
- [ ] Pagination en sortering in zoekresultaten
- [ ] Adminmodule: gebruikersbeheer
- [ ] Adminmodule: rollen en permissions
- [ ] Audit logs (mutaties, autorisatie-issues, beheerdersacties)

### Fase 5 — Hardening

- [ ] Backend unit tests uitbreiden (xUnit + Moq)
- [ ] Integratietests met Testcontainers (echte PostgreSQL)
- [ ] Frontend unit tests (Jasmine/Karma)
- [ ] E2E-tests (Cypress) voor kritieke paden: login, board-interacties, taakbeheer, zoekfunctie
- [ ] Security-review: password policies, token expiry, rate limiting, veilige file-uploadregels
- [ ] Performance-optimalisatie (queries, indices, caching waar relevant)
- [ ] UX-polish en consistentie (Angular Material theming, dark mode afronden)
- [ ] Correlation IDs / request tracing verfijnen
- [ ] Seed data voor development/demo (organisaties, teams, users, projecten, boards, kolommen, taken, labels)

### Doorlopend / niet-fasegebonden

- [ ] Database-indexering op veelgebruikte queryvelden (project, status, assignee, due date, organisatie-id)
- [ ] Soft delete waar relevant
- [ ] Repository & Unit of Work-pattern
- [ ] Specification Pattern voor flexibele filtering
- [ ] Strategy/Factory-patterns waar variabele businesslogica dat vraagt
- [ ] Dependency Injection consistent toepassen
- [ ] CI/CD uitbreiden: linting, unit tests, integration tests, build verification, artefactpublicatie
- [ ] Internationalisatie (ngx-translate) volledig doorvoeren
- [ ] Documentatie actueel houden (Swagger, README, ADR's)

---

## 3. Hoe dit project gerealiseerd kan worden

### 3.1 Aanpak en volgorde

Het plan is zelf al gefaseerd (fase 1 t/m 5), en dat is ook de meest logische uitvoeringsvolgorde: eerst het architecturale fundament (lagen, auth, database, CI/CD) — pas daarna functionaliteit bouwen. Reden: alle latere features (taken, boards, notificaties) hangen af van correcte Identity/JWT, database-schema en CQRS-basis. Binnen elke fase is het verstandig verticaal te werken (een volledige "slice" per feature: entity → command/query → handler → validator → endpoint → frontend-component → test) in plaats van horizontaal per laag, zodat er telkens een werkend, demonstreerbaar stukje functionaliteit ontstaat.

Een praktische volgorde binnen Fase 2, bijvoorbeeld:

1. Organisaties (eerste bounded context, want alles hangt hieronder)
2. Teams en rollen
3. Projecten
4. Boards en kolommen
5. Taken (grootste en belangrijkste entiteit)
6. Comments

### 3.2 Werkwijze en governance

- **Methodiek:** een lichte Scrum/Kanban-hybride past goed bij de eigen fasering — sprints van 1-2 weken per fase-onderdeel, met een board dat de kolommen uit dit document weerspiegelt.
- **Architecture Decision Records (ADR's):** leg keuzes vast (bijv. NgRx Signal Store vs RxJS, Azure Blob vs Local Storage) zodat de showcase-functie van het project (referentie-implementatie) ook echt navolgbaar is.
- **Definition of Done per feature:** command/query + handler + validator + endpoint + Swagger-doc + unit test + (waar relevant) integratietest + frontend-koppeling.
- **Code review verplicht** via pull requests, met CI die moet slagen (build, lint, tests) voordat een merge mogelijk is.

### 3.3 Risicobeheersing tijdens uitvoering

Zoals het plan zelf al aangeeft, zijn de grootste risico's over-engineering van CQRS en onduidelijke laaggrenzen. Concreet:

- Voeg patterns (Specification, Strategy, Factory) pas toe zodra een concreet scenario erom vraagt — niet vooraf "voor de zekerheid".
- Houd de eerste doorloop van elke fase bewust minimaal (MVP per fase), en voeg verfijning toe in Fase 5 (Hardening) of latere iteraties.
- Test realtime-functionaliteit (SignalR) vroeg met meerdere gelijktijdige clients, aangezien dit een technisch risicogebied is.
- Bewaak zoekperformance vanaf het begin met indexering; een relationele aanpak is voor v1 voldoende, full-text search is expliciet uitgesteld.

### 3.4 Team en rolverdeling (indicatief)

Afhankelijk van teamgrootte:

- **Backend-ontwikkelaar(s):** Clean Architecture-lagen, CQRS-handlers, EF Core, SignalR-hub, security.
- **Frontend-ontwikkelaar(s):** Angular-modules, state management, boards met drag & drop, dashboards met Chart.js.
- **Eén persoon (of rol) verantwoordelijk voor DevOps:** Docker, Docker Compose, GitHub Actions, omgevingen.
- **QA/testfocus:** kan verdeeld worden over het team, maar met name Cypress-E2E-scenario's verdienen een aparte, expliciete eigenaar omdat ze cross-cutting zijn.

Bij een klein team (1-2 developers) is de fasering vooral een volgordelijst; bij een groter team kunnen fase 2 en 3 gedeeltelijk parallel lopen zodra het fundament (fase 1) staat, mits de bounded contexts (Identity, Organization, Project Management, Notifications) goed ontkoppeld zijn — wat de architectuur uit paragraaf 6 van het plan ook expliciet mogelijk maakt.

### 3.5 Opleverbaar resultaat per fase

- **Na Fase 1:** een leeg maar volledig werkend, gecontaineriseerd fundament met auth-basis.
- **Na Fase 2:** een bruikbare kern-applicatie (organisaties → projecten → boards → taken), zonder realtime of rapportage.
- **Na Fase 3:** een samenwerkingsplatform met realtime updates, notificaties en bestanden.
- **Na Fase 4:** een enterprise-ready platform met dashboards, zoekfunctie en beheer.
- **Na Fase 5:** een geharde, geteste, performante en gepolijste v1 — het punt waarop de acceptatiecriteria uit het plan (§22) volledig worden gehaald.
