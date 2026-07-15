# Find That Book — Design Proposal

## Problem

The core problem is identifying the correct book from "messy, plain-text blob (title and/or author and/or keywords)". Assuming ambiguous human queries, rigid database queries may fail to return a relevant result. Given the PRD example of "extra noise" the query "mark huckleberry, twilight meyer, tolkien hobbit illustrated deluxe 1937" against OpenLibrary returns [no results](https://openlibrary.org/search?q=tolkien+hobbit+illustrated+deluxe+1937).

## Constraints

1. The product development commitment should be short, 4-6 hrs
1. Product should be cost constrained (conversely, unconstrained SLA)
1. Tech stack is limited to a "simple modern UI backed by a .NET 8 Web API", the .net 8 portion is likely interchangeable with .net latest

## Solution

A minimal .net api that leverages Gemini's LLM api and an internally hosted LLM Tool (the openlibrary API) to resolve messy book queries. The frontend will be react with mantine UI to provide a modern and easily extensible design while also being simple enough that it gives that library catalog feel (as apposed to something like material design which I think is a bit bloated for this project).

--- save all this for the readme:

### 1. Backend Tech stack and library choice

- .net 10 minimal web api
  - a lot of this is interchangeable with a full mvc deployment
  - specifically non-aot to avoid anything that can't be "trimmed" and require a library replacement or backtrack on implementation
- openapi for grabbing an endpoint/dto schema, will likely hand crank the frontend though since the schema should be minimal, but if the api grows large enough we can import the openapi schema into the frontend to keep our api and consumer in sync
- GeminiDotnet - this is our Microsoft AI compliant implementation of a LLM chat client
- Microsoft.Extensions.AI - we'll set this up as a service layer between our domain and the LLM provider to allow for us to swap out gemini for something else such as anthropic, openai, etc...

### 2. Client Tech stack and library choice

- react with mantine

### 1. Architecture Overview

- Solution structure:
  - `Web` - ASP.NET Core 10 Web API
  - `Core` - domain models, interfaces, business logic, infrastructure
  - `Tests` - unit + integration tests
  - `Client` - react app with mantine UI

Given a larger project we'd likely want to follow a domain driven design and separate out our
infrastructure layer and breakup the domain models into separate libraries. Given a large enough
domain, aspects of c# such as EF Core's db context or runtime construction of services can suffer cold
starts.

### 2. Backend Overview

Security

Authentication

Authorization

Frontend
