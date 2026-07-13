# Front-end Takenlijst & Sprintplanning — Flira

Dit document bevat de gedetailleerde front-end backlog voor Flira, opgedeeld in Epics, Sprints en individuele taken. Elke taak is uitgeschreven als een User Story met bijbehorende acceptatiecriteria.

---

## Inhoudsopgave
1. [Epic 1: Bootstrap & Layout Core](#epic-1-bootstrap--layout-core) (Sprint 1)
2. [Epic 2: Identity & Authentication Pages](#epic-2-identity--authentication-pages) (Sprint 1 & 2)
3. [Epic 3: Organizations & Teams Workspace](#epic-3-organizations--teams-workspace) (Sprint 2)
4. [Epic 4: Project Management & Kanban Board Workspace](#epic-4-project-management--kanban-board-workspace) (Sprint 3 & 4)
5. [Epic 5: Task View & Details Pane](#epic-5-task-view--details-pane) (Sprint 4 & 5)
6. [Epic 6: Real-time Sync & Notifications Bell](#epic-6-real-time-sync--notifications-bell) (Sprint 5)
7. [Epic 7: Search & Dashboard Visualizations](#epic-7-search--dashboard-visualizations) (Sprint 6)

---

## Epic 1: Bootstrap & Layout Core
**Doel:** Het opzetten van de basis UI, het themasysteem (dark mode) en de routing structuur.

### Sprint 1: Setup & Main Layout
#### Taak 1.1: Angular Material & Dark/Light Mode Theme
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** kunnen wisselen tussen een donker en licht thema
  * **Zodat** ik comfortabel kan werken in verschillende lichtomstandigheden.
* **Acceptatiecriteria:**
  * [x] Angular Material theming is geconfigureerd met SCSS-variabelen voor licht en donker thema.
  * [x] Een global `ThemeService` beheert de actieve theme-klasse op het `<body>` element.
  * [x] Systeem onthoudt de theme-keuze in `localStorage`.
  * [x] De transitie tussen licht en donker verloopt vloeiend (CSS transition).

#### Taak 1.2: Internationalisatie (ngx-translate)
* **User Story:**
  * **Als** Meertalige gebruiker
  * **Wil ik** de taal van de interface kunnen wijzigen (bijv. NL/EN)
  * **Zodat** ik de applicatie in mijn voorkeurstaal kan gebruiken.
* **Acceptatiecriteria:**
  * [x] `ngx-translate` is geïntegreerd in `app.config.ts`.
  * [x] Vertaalbestanden `nl.json` en `en.json` zijn aangemaakt onder `assets/i18n/` of `public/i18n/`.
  * [x] Een global taalschakelaar is toegevoegd in de hoofdnavigatie.
  * [x] Geen enkele hardcoded tekst in HTML-bestanden; alles gebruikt de `translate` pipe.

#### Taak 1.3: Hoofdnavigatie & Layout Componenten
* **User Story:**
  * **Als** Ingelogde gebruiker
  * **Wil ik** een overzichtelijke zijbalk (Sidebar) en bovenbalk (Navbar) zien
  * **Zodat** ik snel kan navigeren tussen projecten, dashboards en instellingen.
* **Acceptatiecriteria:**
  * [x] `MainLayoutComponent` is gemaakt met een responsive Material `MatSidenav`.
  * [x] Zijbalk klapt in op mobiele schermen en toont een hamburger-menu.
  * [x] Navbar bevat het logo, organisatie-switcher, notificatie-bel en profielmenu.

---

## Epic 2: Identity & Authentication Pages
**Doel:** Visuele schermen voor het aanmaken van accounts en inloggen, inclusief sessiebeheer.

### Sprint 1 & 2: Authenticatie & Beveiliging

#### Taak 2.1: Login Pagina & Formulieren
* **User Story:**
  * **Als** Geregistreerde gebruiker
  * **Wil ik** een inlogscherm zien waar ik mijn e-mail en wachtwoord kan invoeren
  * **Zodat** ik veilig toegang krijg tot mijn werkomgeving.
* **Acceptatiecriteria:**
  * [x] `LoginComponent` is ontworpen met Angular Reactive Forms.
  * [x] Validaties: E-mail is verplicht en moet geldig zijn; wachtwoord is verplicht.
  * [x] Toont duidelijke foutmeldingen als de API een 401 of 400 retourneert.
  * [x] Bij succesvolle login wordt het ontvangen JWT en refresh token opgeslagen.

#### Taak 2.2: JWT Interceptor & Route Guards
* **User Story:**
  * **Als** Systeem
  * **Wil ik** automatisch mijn JWT-token meesturen met elke API-call en niet-ingelogde gebruikers weghouden bij interne pagina's
  * **Zodat** de API-aanroepen geautoriseerd zijn en we gegevens beschermen.
* **Acceptatiecriteria:**
  * [x] `AuthInterceptor` voegt de `Authorization: Bearer <token>` header toe aan uitgaande HTTP-requests.
  * [x] `AuthGuard` blokkeert routes als er geen geldig token aanwezig is en stuurt door naar `/login`.
  * [x] Interceptor vangt 401-fouten op en probeert automatisch via de `AuthService` het token te vernieuwen met het refresh token. Als dit faalt, volgt een redirect naar `/login`.

#### Taak 2.3: Registratie Pagina
* **User Story:**
  * **Als** Bezoeker
  * **Wil ik** een registratieformulier kunnen invullen
  * **Zodat** ik direct een account kan registreren.
* **Acceptatiecriteria:**
  * [x] `RegisterComponent` bevat velden: Naam, E-mail, Wachtwoord en Wachtwoord Bevestigen.
  * [x] Wachtwoord-sterkte-indicator toont visueel of het wachtwoord complex genoeg is.
  * [x] Bij succesvolle registratie wordt een vriendelijk scherm getoond waarin wordt gevraagd de e-mail te verifiëren.

#### Taak 2.4: Wachtwoord Vergeten & Herstellen
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** een e-mailherstelformulier kunnen invullen
  * **Zodat** ik mijn wachtwoord kan resetten.
* **Acceptatiecriteria:**
  * [x] `ForgotPasswordComponent` vraagt om e-mailadres en stuurt dit naar de API.
  * [x] `ResetPasswordComponent` (gekoppeld aan route `/reset-password?token=...`) valideert het invoeren van een nieuw wachtwoord en stuurt dit samen met het token naar de API.API.

---

## Epic 3: Organizations & Teams Workspace
**Doel:** Navigatie en beheer van organisaties, teams en rollen.

### Sprint 2: Workspace Inrichting

#### Taak 3.1: Organisatie Switcher in Header
* **User Story:**
  * **Als** Gebruiker met meerdere organisaties
  * **Wil ik** een dropdown in de header zien waarmee ik direct van organisatie kan wisselen
  * **Zodat** ik snel tussen mijn verschillende werkomgevingen kan schakelen.
* **Acceptatiecriteria:**
  * [x] Dropdown toont de lijst van organisaties waar de gebruiker lid van is.
  * [x] Het selecteren van een andere organisatie ververst de actieve state van de applicatie en herlaadt de bijbehorende projecten.
  * [x] De geselecteerde organisatie wordt opgeslagen in `localStorage`.

#### Taak 3.2: Teambeheer Panel
* **User Story:**
  * **Als** Organisatie-beheerder
  * **Wil ik** een overzicht zien van teams en teamleden en nieuwe leden kunnen uitnodigen via e-mail
  * **Zodat** ik de samenstelling van onze teams kan beheren.
* **Acceptatiecriteria:**
  * [x] `TeamManagementComponent` toont een lijst van teams binnen de organisatie.
  * [x] Per team kan een detailweergave geopend worden met alle leden.
  * [x] Formulier om een nieuwe gebruiker uit te nodigen op basis van e-mailadres en rol (RBAC dropdown).

---

## Epic 4: Project Management & Kanban Board Workspace
**Doel:** Project dashboards en interactieve Kanban-boards met drag-and-drop.

### Sprint 3: Projecten & Boards

#### Taak 4.1: Projecten Overzicht & Creatie
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** een dashboard zien met al mijn actieve projecten en een knop om een nieuw project te starten
  * **Zodat** ik direct overzicht heb en nieuwe projecten kan initialiseren.
* **Acceptatiecriteria:**
  * [x] `ProjectListComponent` toont projecten als visuele kaarten (met de gekozen projectkleur en icoon).
  * [x] `CreateProjectDialog` (Material Dialog) bevat formulier voor Naam, Omschrijving, Kleurpicker en Icoonkiezer.
  * [x] Na aanmaken navigeert de app direct naar het board van het nieuwe project.

#### Taak 4.2: Kanban Board met Drag & Drop (Angular CDK)
* **User Story:**
  * **Als** Teamlid
  * **Wil ik** taken visueel kunnen verplaatsen tussen kolommen met drag-and-drop
  * **Zodat** ik mijn werkstatus snel kan bijwerken.
* **Acceptatiecriteria:**
  * [x] `BoardComponent` gebruikt `cdkDropList` en `cdkDrag` uit de Angular CDK.
  * [x] Kolommen tonen het aantal taken dat zich erin bevindt.
  * [x] Slepen triggert een API-call (`MoveTaskCommand` body). Bij een API-fout wordt de taak visueel teruggezet naar zijn oorspronkelijke positie (optimistic UI rendering met rollback).

---

## Epic 5: Task View & Details Pane
**Doel:** Het tonen en bewerken van alle taakdetails en reacties.

### Sprint 4 & 5: Taken & Samenwerking

#### Taak 5.1: Task Detail Dialog (Interactief)
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** op een taakkaart kunnen klikken om een gedetailleerd dialoogvenster te openen
  * **Zodat** ik alle details, bijlagen en reacties kan bekijken en bewerken.
* **Acceptatiecriteria:**
  * [x] `TaskDetailDialogComponent` opent via `MatDialog`.
  * [x] Bevat componenten voor inline-editing van de titel en beschrijving (klik om te bewerken, klik erbuiten of druk op Enter om op te slaan).
  * [x] Status, prioriteit en assignee kunnen aangepast worden via dropdowns die direct API-updates triggeren.

#### Taak 5.2: Comments Sectie met Markdown Weergave
* **User Story:**
  * **Als** Teamlid
  * **Wil ik** reacties onderaan de taak kunnen typen en opgemaakte Markdown kunnen lezen
  * **Zodat** de discussie gestructureerd en leesbaar blijft.
* **Acceptatiecriteria:**
  * [x] Comments sectie toont een tijdlijn van reacties met profielfoto's en timestamps.
  * [x] Gebruikt een Markdown parser component (zoals `ngx-markdown`) om de reactietekst veilig te renderen.
  * [x] Inputveld ondersteunt autocomplete suggesties voor teamleden wanneer de gebruiker `@` typt.

#### Taak 5.3: Attachments Upload & Download Area
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** bestanden kunnen slepen naar de taak om ze te uploaden en ze als thumbnails kunnen zien
  * **Zodat** ik bestanden eenvoudig kan delen en bekijken.
* **Acceptatiecriteria:**
  * [x] Bevat een drag-and-drop zone voor bestanden.
  * [x] Toont een uploadvoortgangsbalk per bestand.
  * [x] Toont geüploade bestanden in een lijst met specifieke iconen op basis van bestandstype en een downloadknop.

---

## Epic 6: Real-time Sync & Notifications Bell
**Doel:** Directe synchronisatie van board-updates en notificaties.

### Sprint 5: Real-time & Notificaties

#### Taak 6.1: SignalR Integratie in State
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** dat wijzigingen die anderen op het board maken direct live worden bijgewerkt in mijn weergave
  * **Zodat** ik nooit met verouderde informatie werk.
* **Acceptatiecriteria:**
  * [x] `SignalRService` beheert de verbinding met de `BoardHub`.
  * [x] Bij binnenkomende evenementen (bijv. `TaskMoved`, `TaskUpdated`) wordt de frontend state (NgRx Signal Store) direct bijgewerkt zonder de hele pagina opnieuw te laden.

#### Taak 6.2: Notificatie-bel in de Header
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** een notificatie-icoon met een teller (badge) in de bovenbalk zien
  * **Zodat** ik weet hoeveel ongelezen notificaties ik heb.
* **Acceptatiecriteria:**
  * [x] Het notificatie-icoon toont een rode badge met het aantal ongelezen notificaties.
  * [x] Klikken op het icoon opent een menu met een scrollbare lijst van recente notificaties.
  * [x] Klikken op een notificatie markeert deze als gelezen en navigeert direct naar de bijbehorende taak.

---

## Epic 7: Search & Dashboard Visualizations
**Doel:** Zoekschermen en grafieken voor rapportage.

### Sprint 6: Dashboard & Search

#### Taak 7.1: Rapportage Dashboard met Chart.js
* **User Story:**
  * **Als** Project Manager
  * **Wil ik** grafieken zien van onze projectvoortgang (Burndown en Velocity)
  * **Zodat** ik direct inzicht heb in onze sprint-prestaties.
* **Acceptatiecriteria:**
  * [x] `DashboardComponent` integreert `Chart.js` (bijv. via `ng2-charts`).
  * [x] Burndown-grafiek toont een ideale lijn versus de werkelijke voortgang.
  * [x] Grafieken passen zich automatisch aan bij het wisselen tussen donker en licht thema (kleuren van assen en rasters veranderen mee).

#### Taak 7.2: Geavanceerd Zoekscherm met Filters
* **User Story:**
  * **Als** Gebruiker
  * **Wil ik** op een centrale pagina kunnen zoeken naar taken over alle projecten heen met diverse filters
  * **Zodat** ik snel een specifiek ticket kan terugvinden.
* **Acceptatiecriteria:**
  * [x] `SearchComponent` bevat een centraal invoerveld met instant search (debounce van 300ms voor het sturen van de API-call).
  * [x] Filters voor Project, Assignee, Status, Prioriteit en Due Date.
  * [x] Zoekresultaten worden overzichtelijk gepagineerd getoond.
