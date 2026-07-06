# Flira — Enterprise Project Management Platform

Flira is een moderne enterprise projectmanagementapplicatie ontworpen voor teams die projecten, taken en real-time samenwerking centraal willen organiseren. Het combineert de visuele eenvoud van Kanban-boards met de procesdiepte en configureerbaarheid van Jira. 

Het project is gebouwd als een robuuste, schaalbare full-stack applicatie en dient als een technische showcase voor Clean Architecture, CQRS, en moderne frontend-state-management.

---

## Inhoudsopgave

1. [Over het Project](#1-over-het-project)
2. [Technische Stack](#2-technische-stack)
   - [Backend](#backend)
   - [Frontend](#frontend)
   - [DevOps & Database](#devops--database)
3. [Architectuur & Design Patterns](#3-architectuur--design-patterns) *(Binnenkort)*
4. [Process & State Flows](#4-process--state-flows) *(Binnenkort)*
5. [Testing & Kwaliteit](#5-testing--kwaliteit) *(Binnenkort)*
6. [Installatie & Opstarten](#6-installatie--opstarten) *(Binnenkort)*

---

## 1. Over het Project

Flira ondersteunt teams bij het stroomlijnen van hun dagelijkse werkzaamheden. De belangrijkste functionele peilers van het platform zijn:

* **Multi-tenant data-isolatie:** Gebruikers kunnen lid zijn van meerdere organisaties en teams, met strikte scheiding van data.
* **Flexibel Project- & Boardbeheer:** Projecten bevatten dynamische Kanban-boards met drag-and-drop functionaliteit voor taken.
* **Real-time updates:** Dankzij SignalR-integratie worden board-wijzigingen en notificaties direct naar alle actieve teamleden gepusht.
* **Uitgebreide Taakdetails:** Taken ondersteunen toewijzingen, prioriteiten, deadlines, labels, bestandsuploads en opgemaakte reacties (Markdown) met `@mentions`.
* **Dashboard & Rapportage:** Ingebouwde widgets voor burndown charts, team velocity, open taken en een kalenderweergave.
* **Enterprise Security & Audit:** Rollen- en permissiebeheer (RBAC), JWT authenticatie met refresh tokens en audit logging voor alle kritieke administratieve mutaties.

---

## 2. Technische Stack

Het project maakt gebruik van moderne, stabiele frameworks en libraries in een volledig gecontaineriseerde omgeving.

### Backend
* **Framework:** .NET 10.0 (ASP.NET Core Web API)
* **Architectuur:** Clean Architecture (Domain-Driven Design principes)
* **CQRS-Patroon:** MediatR voor in-memory dispatching van Commands en Queries
* **Validatie & Mapping:** FluentValidation en AutoMapper
* **Database Access:** Entity Framework Core (Code-First)
* **Beveiliging:** ASP.NET Core Identity & JWT Bearer Authentication
* **Logging:** Serilog (gestructureerde logging naar console en bestanden)
* **API Documentatie:** Swashbuckle / OpenAPI (Swagger)

### Frontend
* **Framework:** Angular 22+ (Strict Mode)
* **UI Componenten:** Angular Material & Angular CDK (met Tailwind CSS of custom SCSS variabelen voor dark mode)
* **State Management:** NgRx Signal Store
* **Internationalisatie:** ngx-translate (meertalige ondersteuning)
* **Visualisaties:** Chart.js

### DevOps & Database
* **Database:** PostgreSQL
* **Containerisatie:** Docker & Docker Compose (API, Frontend & Database)
* **CI/CD:** GitHub Actions (Build, Lint & Test Pipelines)

---

## 3. Architectuur & Design Patterns
*(Deze sectie wordt later uitgebreid met o.a. de structuur van Clean Architecture, CQRS workflow, en Repository & Unit of Work patronen.)*

---

## 4. Process & State Flows
*(Deze sectie wordt later uitgebreid met workflowdiagrammen, real-time sync flows via SignalR en state-transities van taken.)*

---

## 5. Testing & Kwaliteit
*(Deze sectie wordt later uitgebreid met de teststrategie, waaronder backend unit- en integratietests met Testcontainers, en frontend E2E-tests met Cypress.)*

---

## 6. Installatie & Opstarten
*(Deze sectie wordt later voorzien van de stappen om het project lokaal op te starten via Docker Compose.)*
