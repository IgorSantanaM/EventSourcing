# 📦 EventSourcing

A study and reference project exploring **Event Sourcing** and **Projections** patterns in .NET. This repository demonstrates how to capture, persist, and query state changes as a sequence of immutable domain events rather than overwriting records in place.

---

## 📚 Table of Contents

- [What is Event Sourcing?](#what-is-event-sourcing)
- [Core Concepts](#core-concepts)
  - [Events](#events)
  - [Event Store](#event-store)
  - [Aggregates](#aggregates)
  - [Commands](#commands)
  - [Streams](#streams)
- [Projections](#projections)
  - [What is a Projection?](#what-is-a-projection)
  - [Types of Projections](#types-of-projections)
  - [Inline vs Async Projections](#inline-vs-async-projections)
- [CQRS and Event Sourcing](#cqrs-and-event-sourcing)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Key Patterns Demonstrated](#key-patterns-demonstrated)
- [Technologies](#technologies)
- [References](#references)

---

## What is Event Sourcing?

In a traditional CRUD application, when an entity's state changes, you simply overwrite the previous value in the database. This is simple, but it comes with a fundamental trade-off: **you lose history**.

**Event Sourcing** flips this model. Instead of storing *current state*, you store the full **sequence of events** that led to that state. Each event is an immutable record of something that happened in the past.

```
Traditional CRUD:
  Account { Balance: 500 }   ← you only see the final state

Event Sourcing:
  AccountOpened      { InitialDeposit: 1000 }
  MoneyWithdrawn     { Amount: 200 }
  MoneyDeposited     { Amount: 100 }
  MoneyWithdrawn     { Amount: 400 }
  ─────────────────────────────────────────
  Replayed Balance:  500     ← same result, full history retained
```

### Why use Event Sourcing?

| Benefit | Description |
|---|---|
| **Full audit trail** | Every change is recorded. You can answer "what happened and when?" |
| **Time travel** | Replay events up to any point to see past state |
| **Event-driven integration** | Events are a natural integration boundary between services |
| **Debugging** | Reproduce bugs by replaying the exact sequence of events |
| **Retroactive projections** | Build new read models from historical data at any time |

---

## Core Concepts

### Events

An **event** represents something that happened in the domain — a fact in the past. Events are:

- **Immutable** — once written, they never change
- **Named in past tense** — `OrderPlaced`, `ItemAddedToCart`, `PaymentProcessed`
- **Self-contained** — they carry all the data relevant to what happened

```csharp
public record ItemAddedToCart(
    Guid CartId,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
);

public record ItemRemovedFromCart(
    Guid CartId,
    Guid ProductId
);

public record CartCheckedOut(
    Guid CartId,
    DateTimeOffset CheckedOutAt
);
```

---

### Event Store

The **Event Store** is the database for your events. Unlike a relational database that stores the latest state of a row, the event store stores an append-only log of events grouped into **streams**.

Key characteristics:

- **Append-only** — events are never updated or deleted
- **Ordered** — events within a stream have a sequential version number
- **Queryable** — you can retrieve all events for a given stream, or from a given position

```csharp
// Appending events
await session.Events.AppendToStream(cartId, events);
await session.SaveChangesAsync();

// Loading events back
var events = await session.Events.FetchStreamAsync(cartId);
```

---

### Aggregates

An **aggregate** is a domain object whose state is reconstructed by replaying its event stream. It defines the business rules and decides which events to emit in response to commands.

```csharp
public class ShoppingCart
{
    public Guid Id { get; private set; }
    public List<CartItem> Items { get; private set; } = [];
    public CartStatus Status { get; private set; }

    // Reconstruct state from events
    public void Apply(ItemAddedToCart @event)
    {
        Items.Add(new CartItem(@event.ProductId, @event.ProductName, @event.Quantity, @event.UnitPrice));
    }

    public void Apply(ItemRemovedFromCart @event)
    {
        Items.RemoveAll(i => i.ProductId == @event.ProductId);
    }

    public void Apply(CartCheckedOut @event)
    {
        Status = CartStatus.CheckedOut;
    }
}
```

The aggregate's current state is always derived by replaying events from the beginning (or from a snapshot).

---

### Commands

A **command** expresses the *intent* to change state. Commands are validated by the aggregate, which then emits zero or more events if the command is accepted.

```
Command → Aggregate validates → Event(s) emitted → Stored → State updated
```

```csharp
public record AddItemToCart(Guid CartId, Guid ProductId, string ProductName, decimal Price, int Qty);
public record RemoveItemFromCart(Guid CartId, Guid ProductId);
public record CheckoutCart(Guid CartId);
```

---

### Streams

A **stream** is the sequence of events for a single aggregate instance. Each stream is identified by a unique ID (typically the aggregate's ID) and each event in the stream has a **version number** that enables optimistic concurrency control.

```
Stream: Cart-3fa85f64-5717-4562-b3fc-2c963f66afa6
  Version 1 → CartOpened
  Version 2 → ItemAddedToCart  { ProductId: "abc", Qty: 2 }
  Version 3 → ItemAddedToCart  { ProductId: "xyz", Qty: 1 }
  Version 4 → CartCheckedOut
```

---

## Projections

### What is a Projection?

A **projection** (also called a *read model* or *view*) is a data structure derived from the event stream, optimized for querying. While the event store is the **source of truth**, projections are purpose-built **query-optimized views** of that data.

```
Events (source of truth)
        │
        ▼
   Projection logic  ←  subscribes to events
        │
        ▼
  Read Model / View  ←  fast queries
```

Projections answer the fundamental question: *"What does my data look like right now?"* — without requiring you to replay every event on every read.

---

### Types of Projections

#### 1. Single-Stream Projection

Processes events from **one aggregate stream** to produce one read document per stream. The most common type.

```csharp
public class CartSummaryProjection : SingleStreamProjection<CartSummary>
{
    public CartSummary Create(CartOpened @event)
        => new CartSummary { Id = @event.CartId, Status = "Open" };

    public void Apply(CartSummary summary, ItemAddedToCart @event)
    {
        summary.Items.Add(new CartItemView(@event.ProductName, @event.Quantity, @event.UnitPrice));
        summary.TotalPrice = summary.Items.Sum(i => i.Quantity * i.UnitPrice);
        summary.ItemCount  = summary.Items.Sum(i => i.Quantity);
    }

    public void Apply(CartSummary summary, ItemRemovedFromCart @event)
    {
        summary.Items.RemoveAll(i => i.ProductId == @event.ProductId);
        summary.TotalPrice = summary.Items.Sum(i => i.Quantity * i.UnitPrice);
    }

    public void Apply(CartSummary summary, CartCheckedOut @event)
        => summary.Status = "CheckedOut";
}
```

#### 2. Multi-Stream / Cross-Stream Projection

Aggregates data **across multiple streams**. Useful for analytics, dashboards, or any read model that spans many aggregates.

```csharp
// Example: which products appear most often across all carts?
public class ProductPopularityProjection : MultiStreamProjection<ProductPopularity, Guid>
{
    public ProductPopularityProjection()
    {
        Identity<ItemAddedToCart>(e => e.ProductId);
    }

    public void Apply(ProductPopularity view, ItemAddedToCart @event)
        => view.TimesAdded++;

    public void Apply(ProductPopularity view, ItemRemovedFromCart @event)
        => view.TimesRemoved++;
}
```

#### 3. Event Transformation Projection

Produces a new stream of transformed events from an existing one. Useful for versioning and event migration.

---

### Inline vs Async Projections

| Mode | When it updates | Use case |
|---|---|---|
| **Inline** | Synchronously, within the same transaction as the event append | Strong consistency — read immediately after write |
| **Async** | Asynchronously, via a background daemon | High-throughput writes; eventual consistency is acceptable |
| **Live** | On demand, by replaying events at query time | Debugging, one-off queries, small streams |

```csharp
// Registering projections (example with Marten)
options.Projections.Add<CartSummaryProjection>(ProjectionLifecycle.Inline);
options.Projections.Add<ProductPopularityProjection>(ProjectionLifecycle.Async);
```

**Inline projections** guarantee that your read model is always up to date when you query it, at the cost of slightly slower writes.

**Async projections** decouple writes from read model updates, giving better write throughput at the cost of a short lag before queries reflect the latest events.

---

## CQRS and Event Sourcing

Event Sourcing is a natural partner for **CQRS (Command Query Responsibility Segregation)**.

```
┌─────────────────────────────────────────────────────┐
│                  Application                        │
│                                                     │
│  Commands ──► Command Handlers ──► Aggregate        │
│                                        │            │
│                                    Events           │
│                                        │            │
│                                   Event Store       │
│                                        │            │
│                               Projection Engine     │
│                                        │            │
│  Queries ◄─── Read Models ◄────────────┘            │
└─────────────────────────────────────────────────────┘
```

- The **write side** handles commands, validates business rules, and appends events to the store.
- The **read side** subscribes to the event stream and maintains optimized read models (projections).
- Read models can be freely rebuilt from scratch at any time by replaying history.

---

## Project Structure

```
EventSourcing/
│
├── src/
│   ├── Domain/
│   │   ├── Events/             # Domain event records
│   │   └── Aggregates/         # Aggregate classes with Apply() methods
│   │
│   ├── Application/
│   │   ├── Commands/           # Command records and handlers
│   │   └── Queries/            # Query handlers against projections
│   │
│   ├── Projections/
│   │   ├── CartSummary/        # Single-stream projection example
│   │   └── ProductPopularity/  # Cross-stream projection example
│   │
│   └── Infrastructure/
│       └── EventStore/         # Event store configuration and setup
│
└── tests/
    ├── Domain.Tests/
    ├── Projection.Tests/
    └── Integration.Tests/
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL via Docker Compose)

### Running locally

```bash
# 1. Clone the repository
git clone https://github.com/IgorSantanaM/EventSourcing.git
cd EventSourcing

# 2. Start PostgreSQL (used as event store)
docker-compose up -d

# 3. Restore dependencies
dotnet restore

# 4. Run the application
dotnet run --project src/EventSourcing.Api

# 5. Run tests
dotnet test
```

---

## Key Patterns Demonstrated

| Pattern | Description |
|---|---|
| **Append-only event log** | All state changes stored as immutable events |
| **Aggregate reconstitution** | Replaying events to rebuild aggregate state |
| **Single-stream projection** | One read document per aggregate stream |
| **Cross-stream projection** | Analytics views spanning many aggregates |
| **Inline projection** | Synchronous read model updates for strong consistency |
| **Async projection** | Background daemon for high-throughput eventual consistency |
| **Optimistic concurrency** | Version-based conflict detection on stream appends |
| **Snapshot** | Periodic full-state capture to speed up aggregate loading |

---

## Technologies

- **.NET 8** — application runtime
- **Marten** — PostgreSQL-backed document database and event store for .NET
- **PostgreSQL** — underlying database (used by Marten as event store)
- **xUnit** — testing framework

---

## References

- [Martin Fowler — Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)
- [Marten Documentation — Event Sourcing](https://martendb.io/events/)
- [Marten — Projections](https://martendb.io/events/projections/)
- [Greg Young — CQRS and Event Sourcing](https://cqrs.files.wordpress.com/2010/11/cqrs_documents.pdf)
- [Oskar Dudycz — EventSourcing.NetCore](https://github.com/oskardudycz/EventSourcing.NetCore)

---

> **Note:** This repository is a learning project. The goal is to explore and document Event Sourcing patterns hands-on in .NET.
