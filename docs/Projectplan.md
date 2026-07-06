# Projectplan — Flira

## 1. Inleiding

Flira is een moderne enterprise projectmanagementapplicatie voor teams die projecten, taken en samenwerking centraal willen organiseren. Het product combineert de visuele eenvoud van Trello met de procesdiepte van Jira, maar blijft bewust modulair, schaalbaar en onderhoudbaar.

Het project is opgezet als een professioneel platform met een sterke nadruk op architectuur, testbaarheid, security, real-time samenwerking en codekwaliteit. De oplossing moet geschikt zijn voor groeiende teams en organisaties met meerdere projecten, rollen en permissies.

## 2. Doel van het project

Het primaire doel is het ontwikkelen van een robuuste projectmanagementapplicatie waarin gebruikers projecten kunnen beheren, taken kunnen aanmaken, teamleden kunnen toewijzen en statuswijzigingen in real time kunnen volgen. Daarnaast moet het platform enterprise-ready zijn met ondersteuning voor authenticatie, autorisatie, audit logging, notificaties en schaalbare infrastructuur.

De applicatie moet niet alleen functioneel compleet zijn, maar ook als referentie-implementatie dienen voor Clean Architecture, CQRS, gedisciplineerde API-ontwikkeling en hoogwaardige frontend-architectuur. Daarmee is het project evenzeer een product als een technische showcase.

## 3. Scope

De scope omvat een volledige webapplicatie met backend API, frontend, database, authenticatie, notificaties, dashboards, administratieve functies en testautomatisering. Ook deployment via Docker en CI/CD via GitHub Actions vallen binnen de scope.

Buiten scope vallen mobiele apps, uitgebreide enterprise-integraties met externe identity providers, complexe time tracking suites en geavanceerde portfolio management features. Deze kunnen later als uitbreidingen worden toegevoegd zonder het kernontwerp te verstoren.

## 4. Productvisie

Flira moet een duidelijke, snelle en professionele gebruikerservaring bieden waarin teams in één oogopslag hun werk kunnen plannen en opvolgen. De interface moet overzichtelijk zijn voor dagelijkse gebruikers, maar krachtig genoeg voor beheerders en projectleiders.

Het product moet zich onderscheiden door real-time samenwerking, strakke informatiearchitectuur, sterke zoekmogelijkheden en een clean, modern design. De technische onderlaag moet bewust generiek worden opgezet zodat toekomstige features zonder grote herstructurering kunnen worden toegevoegd.

## 5. Technische stack

### Backend

- ASP.NET Core 9 Web API.
- C#.
- Entity Framework Core.
- PostgreSQL.
- JWT Authentication.
- ASP.NET Identity.
- FluentValidation.
- AutoMapper.
- MediatR voor CQRS.
- SignalR.
- Serilog.
- Swagger / OpenAPI.
- Docker en Docker Compose.

### Front-end

- Angular 20+.
- Angular Material.
- NgRx Signal Store of RxJS.
- Angular Router.
- SCSS.
- Chart.js.
- ngx-translate.
- JWT interceptor.

### Testing

- Backend: xUnit, Moq, Testcontainers.
- Frontend: Jasmine, Karma, Cypress.

### Development en delivery

- Visual Studio of Rider.
- VSCode.
- Postman.
- GitHub.
- GitHub Actions.

## 6. Architectuur

De applicatie gebruikt Clean Architecture met duidelijke lagen en een strikte afhankelijkheidsrichting. Domain staat centraal en mag niet afhankelijk zijn van infrastructuur of presentatie. Application bevat use cases, interfaces, DTO’s, CQRS handlers en validatie. Infrastructure levert implementaties voor persistence, email, storage, logging en externe services. API fungeert als thin presentation layer. Shared bevat gedeelde bouwstenen zoals result types, constants, contracts en basisabstraheringen.

De beoogde lagen zijn:

- API.
- Application.
- Domain.
- Infrastructure.
- Persistence.
- Shared.

Deze scheiding ondersteunt testbaarheid, onderhoudbaarheid en vervangbaarheid van componenten. Het maakt ook duidelijke bounded contexts mogelijk, bijvoorbeeld voor Identity, Organization Management, Project Management en Notifications.

## 7. CQRS-aanpak

Alle endpoint-logica wordt georganiseerd volgens CQRS. Elke use case wordt opgesplitst in een command of query, een handler en een validator. Dit leidt tot kleine, expliciete en goed testbare componenten.

Het patroon is als volgt:

- Command of Query definieert de intentie.
- Handler voert de businesslogica uit.
- Validator controleert invoerregels.
- Result Pattern standaardiseert succes- en foutafhandeling.

Waar nodig kunnen read-modellen worden geoptimaliseerd voor dashboard- en zoekscenario’s, terwijl write-modellen gericht blijven op integriteit en transactiecontrole. Daarmee ontstaat een heldere scheiding tussen schrijf- en leespaden zonder onnodige complexiteit.

## 8. Belangrijkste domeinen

### Authenticatie en identiteit

Gebruikers kunnen zich registreren, inloggen, hun token vernieuwen en hun wachtwoord herstellen. Emailverificatie is standaard onderdeel van het onboardingproces, terwijl MFA optioneel kan worden ingeschakeld voor extra beveiliging.

ASP.NET Identity beheert user accounts, password hashing, security stamps, claims en herstelstromen. JWT tokens worden gebruikt voor API-toegang en refresh tokens voor sessiebeheer.

### Organisaties

Een gebruiker kan lid zijn van meerdere organisaties. Binnen een organisatie bestaan teams, gebruikers en rollen, zodat toegang en zichtbaarheid per organisatorische context kunnen worden geregeld.

Organisaties vormen het primaire afbakeningsniveau voor data-isolatie en autorisatie. Dit maakt multi-tenant gedrag mogelijk zonder dat de applicatie per se fysiek gescheiden deployments nodig heeft.

### Projecten

Een project bevat minimaal naam, omschrijving, kleur en icoon. Projecten zijn gekoppeld aan een organisatie en vormen de container voor boards, taken en teamactiviteit.

Projecten moeten voldoende flexibel zijn om verschillende werkwijzen te ondersteunen, van eenvoudige kanban-flow tot meer gestructureerde teamworkflows. Metadata zoals kleur en icoon helpen bij visuele herkenning in lijst- en dashboardweergaven.

### Boards

Elk project kan een of meerdere boards hebben. Het standaard board is een Kanban Board met kolommen zoals Backlog, Todo, In Progress, Review en Done.

Taken kunnen realtime worden versleept tussen kolommen via SignalR, zodat meerdere gebruikers live dezelfde boardstatus zien. Dit versterkt samenwerking en voorkomt conflicterende bewerkingen.

### Taken

Een taak bevat titel, beschrijving, labels, priority, status, assignee, reporter, due date, estimated hours en attachments. Taken vormen de kern van de applicatie en moeten zorgvuldig worden gevalideerd, geauditeerd en gevisualiseerd.

Taakstatussen worden gekoppeld aan boardkolommen, terwijl labels en prioriteit gebruikt worden voor filtering en triage. Assignee en reporter ondersteunen eigenaarschap en traceerbaarheid.

### Comments en samenwerking

Taken ondersteunen comments, mentions en markdown-opmaak. Gebruikers kunnen elkaar noemen in reacties, waardoor notificaties en contextuele samenwerking ontstaan.

Commenting moet licht en snel aanvoelen, maar tegelijkertijd auditbaar zijn. Markdown biedt flexibiliteit voor opsommingen, codefragmenten en gestructureerde toelichting.

### Dashboard

Het dashboard bevat widgets voor Open Tasks, Completed Tasks, Burndown, Team Velocity en Calendar. Hiermee krijgen gebruikers direct zicht op voortgang, workload en planning.

De dashboardlaag moet data uit meerdere bronnen samenbrengen zonder de backend te overbelasten. Daarom zijn read-optimizations en gerichte query-endpoints belangrijk.

### Notificaties

Notificaties zijn realtime en moeten gebeurtenissen zoals nieuwe taken, assignments en comments ondersteunen. Deze meldingen kunnen zowel in-app als via toekomstige uitbreidingen naar e-mail of andere kanalen worden verstuurd.

SignalR is geschikt voor directe push naar verbonden clients. De notificatie-architectuur moet losgekoppeld blijven van de brongebeurtenissen zodat nieuwe triggers eenvoudig kunnen worden toegevoegd.

### Bestanden

Bestanden kunnen worden geüpload en opgeslagen in Azure Blob Storage of Local Storage. De implementatie moet storage-abstracties gebruiken zodat de infrastructuur later kan worden gewisseld zonder domeinwijzigingen.

Attachments worden gekoppeld aan taken en eventueel comments. Bestandsmetadata moet worden opgeslagen in de database, terwijl de inhoud extern of lokaal wordt bewaard.

### Zoekfunctie

De globale zoekfunctie moet projecten, taken, labels en gebruikers kunnen vinden. Filteropties omvatten labels, users, project en status.

Zoekfunctionaliteit moet snel en voorspelbaar zijn, met aandacht voor pagination, sortering en query performance. Voor de eerste versie volstaat een relationele aanpak, later kan eventueel full-text search worden toegevoegd.

### Admin

Het admingedeelte bevat gebruikersbeheer, rollen, permissions en audit logs. Beheerders moeten inzicht hebben in wie toegang heeft tot wat, en welke acties zijn uitgevoerd.

Audit logs zijn essentieel voor enterprisegebruik en troubleshooting. Ze registreren belangrijke mutaties, autorisatie-issues en beheerdersacties.

## 9. Functionele eisen

### Authenticatie

- Registreren.
- Login.
- Refresh tokens.
- Forgot password.
- Email verification.
- MFA optioneel.

### Organisaties en teams

- Meerdere organisaties per gebruiker.
- Teams binnen organisaties.
- Gebruikersbeheer per organisatie.
- Rollen en permissions.

### Projecten en boards

- Project aanmaken, wijzigen en verwijderen.
- Boardbeheer per project.
- Kolommen beheren.
- Drag & drop van taken.
- Realtime updates.

### Taken en samenwerking

- Taken aanmaken, bewerken en verwijderen.
- Labels, prioriteit, status en assignee instellen.
- Comments met mentions en markdown.
- Due dates en estimated hours.
- Attachments toevoegen.

### Dashboard en rapportage

- Open Tasks widget.
- Completed Tasks widget.
- Burndown chart.
- Team Velocity chart.
- Calendar view.

### Notificaties en audit

- Realtime notificaties.
- Notificaties voor nieuwe taken, assignment en comments.
- Audit logs voor beheer en belangrijke acties.

## 10. Databaseontwerp

De database draait op PostgreSQL en bevat een relationeel model met duidelijke foreign keys, indices en audit-vriendelijke kolommen. Tabellen omvatten Users, Organizations, Teams, Projects, Boards, Columns, Tasks, Comments, Labels, Attachments, Notifications, ActivityLogs en RefreshTokens.

Belangrijke ontwerpprincipes zijn normalisatie, referentiële integriteit, soft delete waar nodig en indexering op veelgebruikte queryvelden zoals project, status, assignee, due date en organisatie-id. Voor schaalbaarheid moeten read queries en write transacties zorgvuldig gescheiden blijven.

## 11. Design patterns

Het project gebruikt meerdere patronen om complexiteit beheersbaar te houden. Repository en Unit of Work structureren toegang tot data, Specification Pattern ondersteunt flexibele filtering, Strategy en Factory helpen bij variabele businesslogica, en Result Pattern zorgt voor consistente uitkomsten.

Dependency Injection is overal de standaard voor losgekoppelde componenten. CQRS en MediatR vormen de basis voor use-case-gedreven ontwikkeling, terwijl AutoMapper de mapping tussen domein, entity en DTO vereenvoudigt.

## 12. Security en compliance

Security is een kernonderdeel van het ontwerp. JWT, refresh tokens en ASP.NET Identity beschermen toegang, terwijl role- en permission-based access control de autorisatie verfijnt.

Aanvullend moeten password policies, email verification, token expiry, rate limiting en veilige file-uploadregels worden voorzien. Audit logging en gestructureerde logging via Serilog helpen bij detectie, monitoring en incidentanalyse.

## 13. Realtime en UX

SignalR ondersteunt realtime boardupdates, notificaties en collaboratieve interactie. Hierdoor zien meerdere gebruikers wijzigingen direct zonder handmatige refresh.

De frontend moet responsive, snel en intuïtief zijn, met Angular Material als visuele basis en SCSS voor theme-aanpassingen. Dark mode moet worden ondersteund, zodat gebruikers hun werkruimte kunnen afstemmen op voorkeur of omgeving.

## 14. Frontend-architectuur

De Angular-applicatie wordt modulair opgebouwd met routering per domein en duidelijke scheiding tussen pages, components, services, state en shared UI. Statebeheer kan met NgRx Signal Store of RxJS worden ingericht, afhankelijk van de gekozen complexiteitsbalans.

De frontend gebruikt een JWT interceptor voor veilige API-calls en ngx-translate voor internationalisatie. Chart.js verzorgt visualisaties op het dashboard, terwijl drag & drop de boardervaring soepel maakt.

## 15. API-ontwerp

Elke endpoint volgt een consistente structuur met request, response, validatie en gestandaardiseerde foutafhandeling. Endpointnamen moeten voorspelbaar zijn en logisch gegroepeerd per resource en use case.

Voorbeeldcategorieën zijn auth, organizations, teams, projects, boards, columns, tasks, comments, notifications, admin en search. Swagger documenteert de API en maakt test- en integratiewerk eenvoudiger.

## 16. Testingstrategie

De backend wordt getest met xUnit en Moq, aangevuld met Testcontainers voor realistische database- en integratietests. Daarmee kan de applicatie worden gevalideerd tegen een echte PostgreSQL-omgeving tijdens CI en lokale ontwikkeling.

De frontend wordt getest met Jasmine en Karma voor unit tests en Cypress voor end-to-end scenario’s. Kritieke paden zoals login, board interacties, taakbeheer en zoekfunctionaliteit moeten hoge testdekking krijgen.

## 17. DevOps en levering

De applicatie moet volledig containeriseerbaar zijn met Docker en Docker Compose. Dat vereenvoudigt lokale ontwikkeling, integratietests en deployment naar test- of productieomgevingen.

GitHub Actions verzorgt build, test en eventueel deployment workflows. Een CI/CD-pipeline moet kwaliteitschecks uitvoeren zoals linting, unit tests, integration tests, build verification en artefactpublicatie.

## 18. Logging en observability

Serilog levert gestructureerde logging voor API-requests, domeinacties, errors en uitzonderlijke flows. Logs moeten voldoende context bevatten om problemen snel te herleiden, zonder onnodige gevoelige data op te slaan.

Aanvullend zijn correlation ids, request tracing en audit events belangrijk voor support en debugging. Dit is vooral relevant bij realtime functies en multi-user interacties.

## 19. Seed data

Seed data ondersteunt directe bruikbaarheid van de applicatie in development en demo-omgevingen. Denk aan voorbeeldorganisaties, teams, users, projecten, boards, kolommen, taken en labels.

Goede seed data maakt het makkelijker om features te demonstreren, tests te draaien en schermen te beoordelen. De data moet realistisch zijn en verschillende status- en autorisatiescenario’s afdekken.

## 20. Projectfasering

### Fase 1: Fundament

- Solution structuur opzetten.
- Clean Architecture scheiden.
- Identity en JWT configureren.
- PostgreSQL en EF Core inrichten.
- Basis CI/CD en Docker toevoegen.

### Fase 2: Kernfunctionaliteit

- Organisaties, teams en rollen.
- Projecten, boards en kolommen.
- Taken en comments.
- Validatie, mapping en CQRS handlers.

### Fase 3: Samenwerking

- SignalR realtime updates.
- Notificaties.
- Mentions.
- Bestandsuploads.

### Fase 4: Rapportage en admin

- Dashboard widgets.
- Zoekfunctie.
- Adminbeheer.
- Audit logs.

### Fase 5: Hardening

- Testdekking uitbreiden.
- Security verfijnen.
- Performance optimaliseren.
- UX en polish.

## 21. Risico’s en mitigatie

De grootste technische risico’s zitten in te veel complexiteit, onduidelijke grenzen tussen lagen en over-engineering van CQRS. Dit wordt gemitigeerd door de eerste versie compact te houden en alleen patronen toe te passen waar ze daadwerkelijk waarde toevoegen.

Daarnaast vormen realtime synchronisatie, permissiemodellen en zoekperformance aandachtspunten. Deze risico’s worden beheerst met duidelijke contracts, integratietests, observability en iteratieve oplevering.

## 22. Acceptatiecriteria

De eerste versie is klaar wanneer gebruikers veilig kunnen registreren en inloggen, organisaties en projecten kunnen beheren, taken realtime kunnen verplaatsen en dashboards, notificaties en adminfuncties werken zoals afgesproken. Ook moet de applicatie draaien via Docker, getest zijn met geautomatiseerde tests en gedocumenteerd zijn via Swagger.

Daarnaast moet de codebase voldoen aan Clean Architecture-principes, CQRS-structuur, consistente logging en een professionele frontend-ervaring. Het systeem moet stabiel genoeg zijn voor demonstratie en verdere doorontwikkeling.

## 23. Eindresultaat

Het eindresultaat is een volwassen projectmanagementplatform met enterprise-karakter, gebouwd op een moderne .NET- en Angular-stack. De oplossing is technisch sterk, uitbreidbaar en ontworpen voor langdurig onderhoud.

Door architectuur, testbaarheid en real-time samenwerking centraal te zetten, ontstaat een applicatie die zowel functioneel nuttig als technisch overtuigend is.
