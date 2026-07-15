# Find That Book — Design Proposal

## Problem

The core problem is identifying the correct book from "messy, plain-text blob (title and/or author and/or keywords)". Assuming ambiguous human queries, rigid database queries may fail to return a relevant result. Given the PRD example of "extra noise" the query "mark huckleberry, twilight meyer, tolkien hobbit illustrated deluxe 1937" against OpenLibrary returns [no results](https://openlibrary.org/search?q=tolkien+hobbit+illustrated+deluxe+1937).

## Constraints

1. The product development commitment should be short, 4-6 hrs
1. Product should be cost constrained (conversely, unconstrained SLA)
1. Tech stack is limited to a "simple modern UI backed by a .NET 8 Web API", the .net 8 portion is likely interchangeable with .net latest

## Solution

A minimal .net api that leverages Gemini's LLM api and an internally hosted LLM Tool (the openlibrary API) to resolve messy book queries. The frontend will be react with mantine UI to provide a modern and easily extensible design while also being simple enough that it gives that library catalog feel (as apposed to something like material design which I think is a bit bloated for this project).
