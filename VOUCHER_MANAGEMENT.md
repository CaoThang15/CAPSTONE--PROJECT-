# Voucher Management API

This document demonstrates the voucher management system implementation for SMarket.

## Features Implemented

### 1. **Voucher CRUD Operations (Admin Only)**

- Create new vouchers with validation
- Update existing vouchers
- Delete vouchers (soft delete)
- Get all vouchers
- Get voucher by ID or code

### 2. **Voucher Types Support**

- **Percentage Discount**: e.g., 10% off
- **Fixed Amount Discount**: e.g., $50 off

### 3. **Voucher Validation**

- Date range validation (start/end dates)
- Usage limit tracking
- Status management (Active/Inactive)
- Unique code enforcement

### 4. **User Voucher Management**

- Get user's assigned vouchers
- Assign vouchers to users (Admin)
- Remove vouchers from users (Admin)

### 5. **Voucher Application System**

- Validate voucher codes
- Apply vouchers to orders
- Calculate discounts automatically
- Usage count tracking

## API Endpoints

### Admin Endpoints

```http
# Get all vouchers
GET /api/vouchers
Authorization: Bearer {admin_token}

# Create new voucher
POST /api/vouchers
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "code": "SAVE20",
    "description": "20% discount on all items",
    "discountType": "Percentage",
    "discountAmount": 20,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z",
    "usageLimit": 1000,
    "statusId": 1
}

# Update voucher
PATCH /api/vouchers/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "description": "Updated description",
    "discountAmount": 25
}

# Delete voucher
DELETE /api/vouchers/{id}
Authorization: Bearer {admin_token}

# Assign voucher to user
POST /api/vouchers/assign/{voucherId}
Authorization: Bearer {admin_token}
Content-Type: application/json

123  // userId

# Remove voucher from user
DELETE /api/vouchers/assign/{voucherId}
Authorization: Bearer {admin_token}
Content-Type: application/json

123  // userId

# Get voucher statuses
GET /api/vouchers/statuses
Authorization: Bearer {admin_token}
```

### User Endpoints

```http
# Get active vouchers
GET /api/vouchers/active
Authorization: Bearer {user_token}

# Get my assigned vouchers
GET /api/vouchers/my-vouchers
Authorization: Bearer {user_token}

# Get voucher by code
GET /api/vouchers/code/{code}
Authorization: Bearer {user_token}

# Validate voucher (without applying)
POST /api/vouchers/validate
Authorization: Bearer {user_token}
Content-Type: application/json

{
    "code": "SAVE20",
    "orderTotal": 100.00
}

# Apply voucher to order
POST /api/vouchers/apply
Authorization: Bearer {user_token}
Content-Type: application/json

{
    "code": "SAVE20",
    "orderTotal": 100.00
}
```

## Response Examples

### Successful Voucher Creation

```json
{
  "message": "Voucher created successfully.",
  "data": {
    "id": 1,
    "code": "SAVE20",
    "description": "20% discount on all items",
    "discountType": "Percentage",
    "discountAmount": 20,
    "startDate": "2024-01-01T00:00:00Z",
    "endDate": "2024-12-31T23:59:59Z",
    "usageLimit": 1000,
    "usageCount": 0,
    "statusId": 1,
    "statusName": "Active",
    "createdAt": "2024-10-03T10:00:00Z",
    "updatedAt": null,
    "isActive": true
  }
}
```

### Voucher Application Result

```json
{
  "message": "Voucher applied successfully",
  "data": {
    "isValid": true,
    "message": "Voucher applied successfully",
    "discountAmount": 20.0,
    "finalTotal": 80.0,
    "voucher": {
      "id": 1,
      "code": "SAVE20",
      "discountType": "Percentage",
      "discountAmount": 20,
      "isActive": true
    }
  }
}
```

## Business Logic

### Voucher Validation Rules

1. **Code Uniqueness**: Each voucher code must be unique
2. **Date Validation**: End date must be after start date
3. **Discount Validation**:
   - Percentage discounts cannot exceed 100%
   - Fixed discounts must be positive
4. **Usage Limits**: Tracks usage count and prevents over-usage
5. **Status Check**: Only active vouchers can be applied
6. **Date Range**: Vouchers only work within their date range

### Discount Calculation

- **Percentage**: `orderTotal * (discountAmount / 100)`
- **Fixed**: `discountAmount` (capped at order total)
- **Final Total**: `orderTotal - discountAmount`

## Database Structure

### Voucher Table

- Id, Code, Description, DiscountType, DiscountAmount
- StartDate, EndDate, UsageLimit, UsageCount
- StatusId, CreatedAt, UpdatedAt, IsDeleted

### UserVoucher Table (Junction)

- UserId, VoucherId (for user-specific voucher assignments)

### VoucherStatus Table

- Id, Name (Active, Inactive, etc.)

## Security Features

- **Admin-only endpoints** for voucher management
- **User authentication** required for all endpoints
- **Input validation** on all DTOs
- **Soft delete** for data integrity
- **Usage tracking** to prevent abuse

## Usage Notes

1. **Admin Role Required**: Voucher creation, updates, and user assignment require admin privileges
2. **Automatic Usage Tracking**: When vouchers are applied, usage count increments automatically
3. **Flexible Assignment**: Vouchers can be assigned to specific users or made available to all
4. **Real-time Validation**: Vouchers are validated in real-time during application
5. **Date-aware**: System automatically checks voucher validity based on current date

This voucher management system provides a complete solution for discount code functionality in the e-commerce platform.
