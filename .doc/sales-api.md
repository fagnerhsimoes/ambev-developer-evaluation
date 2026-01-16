# Sales API Documentation

## Overview

The Sales API provides endpoints for managing sales records, allowing creation, retrieval, modification, and
cancellation of sales.

## Endpoints

### 1. Create Sale

Create a new sale record.

**URL**: `POST /api/sales`
**Description**: Creates a new sale with one or more items.
**Validation**:

- Purchases of 4+ identical items get 10% discount.
- Purchases of 10-20 identical items get 20% discount.
- Maximum 20 items per product.
- Customer must exist.

**Request Body**:

```json
{
  "saleNumber": "SALE-001",
  "date": "2024-01-14T10:00:00Z",
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "branch": "Branch A",
  "items": [
    {
      "productId": "product-uuid",
      "quantity": 5,
      "unitPrice": 100
    }
  ]
}
```

**Response (201 Created)**:

```json
{
  "success": true,
  "message": "Sale created successfully",
  "data": {
    "id": "sale-uuid",
    "saleNumber": "SALE-001",
    "totalAmount": 450.00 
  }
}
```

---

### 2. Get Sale

Retrieve details of a specific sale.

**URL**: `GET /api/sales/{id}`

**Response (200 OK)**:

```json
{
  "success": true,
  "message": "Sale retrieved successfully",
  "data": {
    "id": "sale-uuid",
    "saleNumber": "SALE-001",
    "date": "2024-01-14T10:00:00Z",
    "customerName": "John Doe",
    "totalAmount": 450.00,
    "branch": "Branch A",
    "status": "Pending",
    "isCancelled": false,
    "items": [
      {
        "productName": "Beer",
        "quantity": 5,
        "unitPrice": 100,
        "discount": 10,
        "totalAmount": 450
      }
    ]
  }
}
```

---

### 3. List Sales

Retrieve a paginated list of sales.

**URL**: `GET /api/sales?_page=1&_size=10&_order=date desc`

**Parameters**:

- `_page`: Page number (default: 1)
- `_size`: Page size (default: 10)
- `_order`: Sorting criteria (e.g. "date desc", "totalAmount asc")

**Response (200 OK)**:

```json
{
  "data": [
    {
      "id": "sale-uuid",
      "saleNumber": "SALE-001",
      "totalAmount": 450.00,
      "customerName": "John Doe",
      "status": "Pending"
    }
  ],
  "currentPage": 1,
  "totalPages": 5,
  "totalCount": 50,
  "success": true
}
```

---

### 4. Update Sale

Update an existing sale.
Note: Changing items triggers recalculation of totals and discounts.

**URL**: `PUT /api/sales/{id}`

**Request Body**:

```json
{
  "saleNumber": "SALE-001-UPDATED",
  "isCancelled": true
}
```

---

### 5. Cancel Sale

Cancel a sale (Soft Delete).

**URL**: `DELETE /api/sales/{id}`

**Response (200 OK)**:

```json
{
  "success": true,
  "message": "Sale deleted successfully"
}
```
