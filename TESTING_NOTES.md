# Testing Notes

## 1. Test Results

- **Total Tests:** 20
- **Passed:** 20
- **Failed:** 0

## 2. Line Coverage

| Project | Coverage |
|---|---:|
| Domain | 100% |
| Application | 95.8% |
| Overall | 90.5% |

## 3. Testing Libraries

- **Moq:** Used to mock dependencies such as inventory, order repository, and notification services.
- **AutoFixture:** Used to generate orders with valid order items through a custom customization.
- **Bogus:** Used to generate realistic customer and order data with a fixed seed.

## 4. Pricing Fairness

No, it is unfair because a customer paying 999.99 gets no discount, while a customer paying 1,000 gets a 10% discount.

## 5. Bugs Found

None.