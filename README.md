# Mini Checkout Engine — Unit Testing 2

**Student Name:** Omar Fathy  
**Group ID:** Group 3

---

## Project Overview

The **Mini Checkout Engine** is a .NET application that simulates an order checkout process.

It validates customer orders, checks product availability, calculates order totals and discounts, saves successful orders, and sends order confirmations.

This repository focuses on practicing unit testing, test data generation, mocking dependencies, snapshot testing, architecture testing, and code coverage.

## Solution Architecture

The solution is organized into three main layers:

```text
Simulation.Checkout
│
├── src
│   ├── Simulation.Checkout.Domain
│   ├── Simulation.Checkout.Application
│   └── Simulation.Checkout.Infrastructure
│
└── tests
    └── Simulation.Checkout.Tests
```

### Domain

Contains the core business models and rules, including:

- `Order`
- `OrderItem`
- `CheckoutReceipt`
- `ReceiptLine`
- Checkout-related exceptions

### Application

Contains the application logic.

The main component is `CheckoutService`, which handles the checkout workflow.

### Infrastructure

Contains implementations of external dependencies, such as `InMemoryOrderRepository`.

### Dependency Direction

The architecture follows this dependency direction:

```text
Infrastructure → Application → Domain
```

- Infrastructure can depend on Application and Domain.
- Application can depend on Domain, but not Infrastructure.
- Domain does not depend on Application or Infrastructure.

---

## Checkout Process

The checkout process follows these main steps:

1. Validate that the order contains items.
2. Validate item quantities and prices.
3. Check product availability through the inventory service.
4. Calculate the order subtotal.
5. Apply the discount according to the business rules.
6. Calculate the final total.
7. Save the successful order.
8. Send an order confirmation containing the receipt.

If checkout fails, the order must not be saved and no confirmation should be sent.

## Business Rules

| Rule | Description |
|---|---|
| BR-01 | An empty order is not allowed. |
| BR-02 | Item quantity must be greater than zero. |
| BR-03 | Item price cannot be negative. |
| BR-04 | All items must be available in inventory. |
| BR-05 | Subtotal is calculated from unit price × quantity. |
| BR-06 | Orders below 1,000 receive no discount; orders of 1,000 or more receive a 10% discount, capped at 100. |
| BR-07 | Final total = subtotal − discount. |
| BR-08 | Successful checkout saves the order and sends one confirmation. |
| BR-09 | Failed checkout must not save the order or send a confirmation. |

---

## Testing Implementation

The test project uses the following libraries:

| Library | Purpose |
|---|---|
| xUnit | Writing and running unit tests. |
| FluentAssertions | Expressive assertions. |
| Moq | Mocking dependencies and verifying interactions. |
| AutoFixture | Generating test objects. |
| Bogus | Generating realistic test data. |
| Verify.Xunit | Snapshot testing. |
| NetArchTest.Rules | Testing architectural rules. |
| Coverlet | Collecting code coverage data. |
| ReportGenerator | Generating coverage reports. |

## Completed Tasks

### Task 1 — Business Rules

Implemented unit tests for:

- Empty order validation.
- Invalid quantities, including zero and negative values.
- Negative and zero prices.
- Discount boundaries and maximum discount.
- Subtotal, discount, and final total calculations.

### Task 2 — Mocking with Moq

Used mocks to isolate `CheckoutService` from its dependencies.

Tested that:

- Successful checkout saves the order exactly once.
- Successful checkout sends one confirmation.
- Inventory failure prevents saving and sending confirmation.
- Validation failure prevents inventory checks, saving, and notification.

### Task 3 — Test Data Generation

**AutoFixture:**

Created a custom `ValidOrderCustomization` to generate orders with valid item quantities and prices.

**Bogus:**

Created an `OrderFaker` helper to generate realistic customer information and order data using a fixed seed.

Verified that generated customer names and emails are preserved in the checkout receipt.

### Task 4 — Snapshot Testing

Used Verify.Xunit to create a snapshot test for the checkout receipt using Example C.

The snapshot verifies important receipt information, including:

- Customer details.
- Order items.
- Subtotal.
- Discount.
- Final total.

Dynamic values such as GUIDs and timestamps are scrubbed so they do not cause unnecessary snapshot failures.

### Task 5 — Architecture Testing

Used NetArchTest.Rules to enforce architectural and naming conventions.

Implemented the following rules:

1. Domain must not depend on Application or Infrastructure.
2. Application must not depend on Infrastructure.
3. All interfaces must start with `I`.
4. Classes implementing `IOrderRepository` must end with `Repository`.

### Task 6 — Code Coverage

Used Coverlet to collect code coverage and ReportGenerator to generate HTML and text reports.

The required minimum was **80% line coverage for Domain and Application**.

---

## Test Results

**Latest test run:**

| Metric | Result |
|---|---:|
| Total Tests | 20 |
| Passed | 20 |
| Failed | 0 |
| Skipped | 0 |

## Code Coverage Results

| Project | Line Coverage |
|---|---:|
| Simulation.Checkout.Domain | 100% |
| Simulation.Checkout.Application | 95.8% |
| Simulation.Checkout.Infrastructure | 0% |
| **Overall** | **90.5%** |

Both Domain and Application exceed the required 80% line coverage.

The Infrastructure project is not part of the required coverage target.

---

## How to Run the Tests

Run the following command from the solution root:

```bash
dotnet test
```

## How to Generate Code Coverage

Collect coverage data using Coverlet:

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

Install ReportGenerator if it is not already installed:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Generate the HTML and text summary reports:

```bash
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:"Html;TextSummary"
```

Open the generated report:

```text
coverage-report/index.html
```

---

## Important Notes

- Tests focus on verifying existing production behavior.
- Mocks are used to isolate dependencies and verify interactions.
- High code coverage alone does not guarantee good tests; meaningful assertions are essential.
- The generated `coverage-report/` directory is included in the repository.

