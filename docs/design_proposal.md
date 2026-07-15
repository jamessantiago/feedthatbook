# Find That Book — Design Proposal

## Problem

The core problem is identifying the correct book from "messy, plain-text blob (title and/or author and/or keywords). Assuming ambiguous human queries, rigid database queries may fail to return a relevant result. Given the PRD example of "extra noise" the query "mark huckleberry, twilight meyer, tolkien hobbit illustrated deluxe 1937" against OpenLibrary returns [no results](https://openlibrary.org/search?q=tolkien+hobbit+illustrated+deluxe+1937).

## Constraints

1. The product development commitment should be short, 4-6 hrs
1. Product should be cost constrained (conversely, unconstrained SLA)

## Solution

### 1. Architecture Overview

- Solution structure:
  - `Web` - ASP.NET Core 10 Web API
  - `Core` - domain models, interfaces, business logic, infrastructure
  - `Tests` - unit + integration tests
  - `Client` -

Given a larger project we'd likely want to follow a domain driven design and separate out our
infrastructure layer and breakup the domain models into separate libraries. Given a large enough
domain c# aspects such as EF Core's db context or runtime construction of services can suffer cold
starts.

Security

Authentication

Authorization

Frontend
