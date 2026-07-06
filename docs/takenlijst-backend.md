# Back-end Takenlijst & Sprintplanning — Flira

Dit document bevat de gedetailleerde back-end backlog voor Flira, opgedeeld in Epics, Sprints en individuele taken. Elke taak is uitgeschreven als een User Story met bijbehorende acceptatiecriteria.

---

## Inhoudsopgave
1. [Epic 1: Fundament & DevOps](#epic-1-fundament--devops) (Sprint 1)
2. [Epic 2: Identity & Authentication](#epic-2-identity--authentication) (Sprint 1 & 2)
3. [Epic 3: Multi-Tenancy & Access Control](#epic-3-multi-tenancy--access-control) (Sprint 2)
4. [Epic 4: Project Management & Board Core](#epic-4-project-management--board-core) (Sprint 3 & 4)
5. [Epic 5: Real-time Collaboration & Storage](#epic-5-real-time-collaboration--storage) (Sprint 5)
6. [Epic 6: Reporting, Search & Audit](#epic-6-reporting-search--audit) (Sprint 6)
7. [Epic 7: Hardening & Testing](#epic-7-hardening--testing) (Sprint 7)

---

## Epic 1: Fundament & DevOps
**Doel:** Het opzetten en configureren van de ontwikkelingsomgeving, CI/CD-pipeline en database-inrichting.

### Sprint 1: Bootstrap & DevOps

#### Taak 1.1: GitHub Actions CI/CD Pipeline opzetten
* **User Story:**
  * **Als** DevOps Engineer
  * **Wil ik** een geautomatiseerde GitHub Actions workflow inrichten
  * **Zodat** elke code push naar `main` of `develop` automatisch wordt gebouwd, gelint en getest.
* **Acceptatiecriteria:**
  * [x] GitHub Actions workflowbestand `.github/workflows/ci-backend.yml` is aangemaakt.
  * [x] De workflow triggert bij Pull Requests en pushes naar `main` en `develop`.
  * [x] Stappen bevatten: dotnet restore, dotnet build (met warnings as errors ingeschakeld), en dotnet test.
  * [x] De pipeline faalt als de build of de tests falen.

#### Taak 1.2: Database Mutatie en Seeding Framework
* **User Story:**
  * **Als** Systeem
  * **Wil ik** bij het opstarten automatisch ontbrekende database-migraties uitvoeren en optioneel basis seed data invoegen
  * **Zodat** de database altijd up-to-date is zonder handmatige interventie.
* **Acceptatiecriteria:**
  * [x] Er is een robuust migratiemechanisme in `Program.cs` ingebouwd dat faalt zonder de applicatie te crashen als de database nog niet klaar is.
  * [x] Er is een seed helper die basisrollen (Admin, Manager, User) en systeemconfiguraties in de database zet als deze leeg is.

---

## Epic 2: Identity & Authentication
**Doel:** Veilige gebruikersregistratie en JWT-token-gebaseerde sessieafhandeling.

### Sprint 1 & 2: Authenticatie & Beveiliging

#### Taak 2.1: Registreren Endpoint (CQRS)
* **User Story:**
  * **Als** nieuwe bezoeker
  * **Wil ik** een account kunnen aanmaken met mijn e-mailadres en een wachtwoord
  * **Zodat** ik toegang krijg tot het platform.
* **Acceptatiecriteria:**
  * [x] Command `RegisterUserCommand` en handler geïmplementeerd.
  * [x] FluentValidation dwingt af: geldig e-mailformaat, e-mailadres moet uniek zijn, en wachtwoord voldoet aan complexiteitseisen.
  * [x] Wachtwoord wordt veilig gehasht via ASP.NET Identity `UserManager`.
  * [x] Endpoint retourneert een gestandaardiseerd `Result`-object (succes of validatiefouten).

#### Taak 2.2: Login & JWT Uitgifte
* **User Story:**
  * **Als** bestaande gebruiker
  * **Wil ik** inloggen met mijn e-mailadres en wachtwoord
  * **Zodat** ik een JWT-toegangstoken ontvang om geautoriseerde API-calls te doen.
* **Acceptatiecriteria:**
  * [x] Query `LoginQuery` en handler geïmplementeerd.
  * [x] Valideert de credentials tegen de ASP.NET Identity database.
  * [x] Genereert een JWT-token met claims: User ID, Email, en Roles.
  * [x] Het token heeft een verlooptermijn (bijv. 15 of 60 minuten) geconfigureerd via `appsettings.json`.

#### Taak 2.3: Refresh Tokens
* **User Story:**
  * **Als** ingelogde API-client
  * **Wil ik** mijn verlopen JWT-token vernieuwen met een refresh token
  * **Zodat** ik ingelogd kan blijven zonder opnieuw mijn wachtwoord in te voeren.
* **Acceptatiecriteria:**
  * [x] Er is een tabel `RefreshToken` gekoppeld aan de `User`.
  * [x] Endpoint `POST /api/auth/refresh` accepteert het verlopen JWT en het actieve refresh token.
  * [x] Refresh token moet cryptografisch veilig gegenereerd worden en een langere verloopdatum hebben.
  * [x] Gebruikte of verlopen refresh tokens worden ongeldig verklaard (rotation policy).

#### Taak 2.4: Forgot Password & Wachtwoordherstel
* **User Story:**
  * **Als** gebruiker die zijn wachtwoord is vergeten
  * **Wil ik** een wachtwoordherstel-link kunnen aanvragen via e-mail
  * **Zodat** ik op een veilige manier een nieuw wachtwoord kan instellen.
* **Acceptatiecriteria:**
  * [x] Endpoint `POST /api/auth/forgot-password` genereert een unieke, tijdelijke password reset token.
  * [x] E-mail met token wordt verzonden (stub/abstrahering in Infrastructure).
  * [x] Endpoint `POST /api/auth/reset-password` valideert het token en wijzigt het wachtwoord via de `UserManager`.

#### Taak 2.5: E-mailverificatie
* **User Story:**
  * **Als** nieuw geregistreerde gebruiker
  * **Wil ik** een e-mail ontvangen met een verificatielink
  * **Zodat** ik mijn e-mailadres kan bevestigen voordat ik de applicatie volledig gebruik.
* **Acceptatiecriteria:**
  * [x] Na registratie wordt de status van de gebruiker op `EmailConfirmed = false` gezet.
  * [ ] Er wordt een e-mail verificatietoken gegenereerd en verzonden.
  * [x] Endpoint `POST /api/auth/confirm-email` (of GET) valideert het token en zet `EmailConfirmed = true`.
  * [ ] JWT-generatie blokkeert toegang als e-mailverificatie verplicht is en nog niet is uitgevoerd.

---

## Epic 3: Multi-Tenancy & Access Control
**Doel:** Afbakening van organisaties, teams en role-based / permission-based access control.

### Sprint 2: Data-isolatie & Autorisatie

#### Taak 3.1: Organisatiebeheer & Multi-Tenancy
* **User Story:**
  * **Als** Organisatiebeheerder
  * **Wil ik** een organisatie kunnen aanmaken en gebruikers hieraan koppelen
  * **Zodat** al onze projecten en data strikt geïsoleerd blijven van andere organisaties.
* **Acceptatiecriteria:**
  * [ ] `Organization` entiteit aangemaakt in `Domain`.
  * [ ] Elk CRUD endpoint voor organisaties controleert of de gebruiker de juiste rechten binnen die organisatie heeft.
  * [ ] Er is een Tenant-ID of Organisatie-ID filter op database-niveau (EF Core Global Query Filters) om data-lekken te voorkomen.

#### Taak 3.2: Teams & Teamleden
* **User Story:**
  * **Als** Project Manager
  * **Wil ik** teams kunnen aanmaken binnen mijn organisatie en leden toewijzen
  * **Zodat** we taken specifiek aan teams kunnen toewijzen.
* **Acceptatiecriteria:**
  * [ ] `Team` entiteit aangemaakt met een veel-op-veel relatie naar `User` en gekoppeld aan een `Organization`.
  * [ ] Endpoints om leden toe te voegen/verwijderen aan/uit een team.

#### Taak 3.3: Role- & Permission-Based Access Control (RBAC/PBAC)
* **User Story:**
  * **Als** Systeembeheerder
  * **Wil ik** rollen (bijv. Admin, Manager, Member, Guest) en specifieke permissies kunnen toekennen aan gebruikers
  * **Zodat** we nauwkeurig kunnen bepalen wie projecten mag aanmaken, taken mag verwijderen of instellingen mag wijzigen.
  * Acceptatiecriteria:
  * [ ] Custom autorisatie-attribuut of middleware `[HasPermission(Permissions.ProjectDelete)]` geschreven.
  * [ ] Permissies worden bijgehouden in de database en gekoppeld aan rollen.
  * [ ] Claims in de JWT-token bevatten de geaggregeerde permissies van de actieve gebruiker binnen de geselecteerde organisatie.

---

## Epic 4: Project Management & Board Core
**Doel:** De kernfunctionaliteit voor projecten, Kanban-boards en taken.

### Sprint 3: Projecten & Boards

#### Taak 4.1: Project CRUD
* **User Story:**
  * **Als** Manager
  * **Wil ik** een project kunnen aanmaken, wijzigen en verwijderen met een naam, omschrijving, icoon en kleur
  * **Zodat** ik werkzaamheden logisch kan groeperen en visueel kan onderscheiden.
* **Acceptatiecriteria:**
  * [ ] CRUD CQRS handlers geïmplementeerd in de `Application` laag.
  * [ ] Project is verplicht gekoppeld aan een `Organization`.
  * [ ] Validatie controleert of de naam niet leeg is en uniek is binnen de organisatie.
  * [ ] Verwijderen van een project ondersteunt Soft Delete (`IsDeleted` vlag).

#### Taak 4.2: Board- & Kolombeheer
* **User Story:**
  * **Als** Scrum Master / Manager
  * **Wil ik** de Kanban-kolommen (statusstappen) van een projectboard kunnen configureren en sorteren
  * **Zodat** de workflow overeenkomt met ons bedrijfsproces.
* **Acceptatiecriteria:**
  * [ ] Entiteiten `Board` en `BoardColumn` gedefinieerd.
  * [ ] Bij het aanmaken van een project wordt automatisch een standaardboard gegenereerd met kolommen: *Backlog, Todo, In Progress, Review, Done*.
  * [ ] Kolommen bevatten een `Position` (volgorde-index).
  * [ ] Endpoint `PUT /api/boards/{id}/columns/order` herordent de kolommen op basis van een nieuwe indexlijst.

### Sprint 4: Taken (Task Engine)

#### Taak 4.3: Taak CRUD & CQRS Handlers
* **User Story:**
  * **Als** Teamlid
  * **Wil ik** taken kunnen aanmaken, bewerken en verwijderen met attributen (titel, beschrijving, prioriteit, assignee, reporter, due date, estimated hours)
  * **Zodat** ik mijn dagelijks werk gedetailleerd kan plannen en bijhouden.
* **Acceptatiecriteria:**
  * [ ] `Task` entiteit gedefinieerd met alle benodigde relaties (Assignee, Reporter, BoardColumn).
  * [ ] CQRS Command/Query structuren opgezet voor Create, Read, Update, Delete.
  * [ ] Validatie: Titel mag niet leeg zijn en mag maximaal 200 tekens bevatten; `DueDate` mag niet in het verleden liggen bij aanmaak.

#### Taak 4.4: Taakstatus Wijzigen (Drag & Drop support)
* **User Story:**
  * **Als** Ontwikkelaar
  * **Wil ik** de status (kolom) en positie van een taak binnen een board kunnen aanpassen
  * **Zodat** iedereen direct ziet in welke fase van het proces de taak zich bevindt.
* **Acceptatiecriteria:**
  * [ ] Endpoint `PUT /api/tasks/{id}/move` accepteert `TargetColumnId` en `NewPosition`.
  * [ ] Logica herberekent de posities van de overige taken in zowel de bron- als de doelkolom.
  * [ ] Database-update gebeurt binnen één transactie.

---

## Epic 5: Real-time Collaboration & Storage
**Doel:** Samenwerkingsfuncties zoals realtime boardupdates, reacties en bijlagen.

### Sprint 5: Real-time & Attachments

#### Taak 5.1: SignalR Hub voor Real-time Boardupdates
* **User Story:**
  * **Als** Actief teamlid
  * **Wil ik** dat statuswijzigingen van taken door anderen direct op mijn scherm verschijnen zonder de pagina te verversen
  * **Zodat** we altijd met actuele data werken en conflicten voorkomen.
* **Acceptatiecriteria:**
  * [ ] `BoardHub : Hub` opgezet in `Flira.Api`.
  * [ ] Zodra een taak wordt verplaatst (`MoveTaskCommand`), stuurt de handler een bericht naar de SignalR-groep van dat specifieke bord.
  * [ ] Verbindingen worden geautoriseerd via het JWT-token (SignalR query parameter handeling).

#### Taak 5.2: Comments met Markdown & Mentions
* **User Story:**
  * **Als** Teamlid
  * **Wil ik** reacties kunnen achterlaten op een taak en collega's kunnen noemen met `@naam`
  * **Zodat** we inhoudelijk kunnen overleggen bij de taak.
* **Acceptatiecriteria:**
  * [ ] `Comment` entiteit gekoppeld aan `Task` en `User`.
  * [ ] Comments ondersteunen opslaan van ruwe Markdown tekst.
  * [ ] Regex parser identificeert `@username` vermeldingen in de comment-tekst en triggert een notificatie-event.

#### Taak 5.3: Notificatiesysteem (Event-driven)
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** een in-app notificatie ontvangen wanneer ik word toegewezen aan een taak of genoemd in een comment
  * **Zodat** ik direct op de hoogte ben van acties die mijn aandacht vereisen.
* **Acceptatiecriteria:**
  * [ ] Notificaties worden ontkoppeld verwerkt via MediatR Domain Events of een in-memory event-bus.
  * [ ] `Notification` entiteit opgeslagen in de database met vlag `IsRead`.
  * [ ] Actieve gebruikers ontvangen de notificatie real-time via een SignalR `NotificationHub`.

#### Taak 5.4: Bestandsuploads (Storage-abstractie)
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** bijlagen kunnen uploaden naar taken en comments
  * **Zodat** ik specificaties, ontwerpen en screenshots kan delen.
* **Acceptatiecriteria:**
  * [ ] `IStorageService` interface gedefinieerd in `Application` met methoden `UploadAsync` en `DeleteAsync`.
  * [ ] Twee implementaties in `Infrastructure`: `LocalStorageService` (ontwikkeling) en `AzureBlobStorageService` (productie/stage).
  * [ ] Bestandsmetadata (naam, grootte, extensie, URL) opgeslagen in tabel `Attachment`.
  * [ ] Validatie: Blokkeer onveilige bestandstypen (zoals `.exe`, `.bat`, `.sh`) en beperk de maximale bestandsgrootte (bijv. 10MB).

---

## Epic 6: Reporting, Search & Audit
**Doel:** Dashboards, geavanceerd zoeken en enterprise audit logging.

### Sprint 6: Dashboard & Search

#### Taak 6.1: Dashboard Data Endpoints (Read-Optimized)
* **User Story:**
  * **Als** Project Manager
  * **Wil ik** prestatiegegevens kunnen opvragen (zoals burndown-data, open/gesloten taken ratio, en team velocity)
  * **Zodat** ik de voortgang en prestaties van het team kan analyseren.
* **Acceptatiecriteria:**
  * [ ] SQL-queries of EF Core queries zijn read-optimized (met `.AsNoTracking()`).
  * [ ] Endpoints voor dashboard widgets leveren geaggregeerde data over specifieke tijdsperiodes.

#### Taak 6.2: Globale Zoekfunctie & Filtering
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** globaal kunnen zoeken naar taken, projecten en labels met filters op status en prioriteit
  * **Zodat** ik snel de informatie vind die ik nodig heb.
* **Acceptatiecriteria:**
  * [ ] Query-endpoint accepteert optionele parameters: `SearchTerm`, `ProjectId`, `AssigneeId`, `Status`, `Priority`, `Labels`.
  * [ ] Paginering (`PageNumber`, `PageSize`) en sortering zijn verplicht.
  * [ ] Zoekactie maakt gebruik van database-indices op veelgebruikte zoekvelden (Titel, Beschrijving, Labels).

#### Taak 6.3: Audit Logging
* **User Story:**
  * **Als** Security Compliance Officer
  * **Wil ik** dat alle belangrijke mutaties (zoals projectverwijderingen, permissiewijzigingen en mislukte inlogpogingen) worden gelogd in een database
  * **Zodat** we achteraf kunnen auditeren wie welke actie heeft uitgevoerd.
* **Acceptatiecriteria:**
  * [ ] Entiteit `AuditLog` bevat: `UserId`, `Action`, `EntityName`, `EntityId`, `OldValues`, `NewValues`, en `Timestamp`.
  * [ ] DbContext overschrijft `SaveChanges/SaveChangesAsync` om automatisch wijzigingen in geauditeerde entiteiten vast te leggen.

---

## Epic 7: Hardening & Testing
**Doel:** Testdekking verhogen, beveiliging aanscherpen en performance optimalisaties.

### Sprint 7: Hardening & Testing

#### Taak 7.1: Backend Unit Tests (MediatR & Handlers)
* **User Story:**
  * **Als** Ontwikkelaar
  * **Wil ik** unit tests schrijven voor alle CQRS command- en queryhandlers
  * **Zodat** ik er zeker van ben dat de business logica correct functioneert en er geen regressie optreedt bij wijzigingen.
* **Acceptatiecriteria:**
  * [ ] xUnit testproject `Flira.Application.UnitTests` aangemaakt.
  * [ ] Moq wordt gebruikt om repository- en storage-interfaces te mocken.
  * [ ] Minimale testdekking op de handlers is 80%.

#### Taak 7.2: Integratietests met Testcontainers
* **User Story:**
  * **Als** Lead Developer
  * **Wil ik** integratietests draaien tegen een echte, tijdelijke PostgreSQL database in Docker
  * **Zodat** we database-beperkingen, migraties en complexe SQL-queries betrouwbaar kunnen testen.
* **Acceptatiecriteria:**
  * [ ] Testproject `Flira.Api.IntegrationTests` maakt gebruik van de `Testcontainers.PostgreSql` NuGet-package.
  * [ ] Tests draaien migraties op de tijdelijke database, voeren API-requests uit via `HttpClient` (in-memory test host) en controleren de database-status.
