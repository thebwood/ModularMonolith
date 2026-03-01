# API Testing Guide - Modular Monolith

Complete testing guide for all modules in the Address Management API.

## Overview

This API includes three modules with **v1 URL versioning**:
- **Addresses** - Managing physical addresses
- **People** - Managing individual persons
- **Businesses** - Managing business entities

**All endpoints use the format**: `/api/v1/{resource}`

## Module: People

### 1. Create a Person

```http
POST https://localhost:7188/api/v1/people
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1-555-0100",
  "dateOfBirth": "1985-03-15"
}
```

**Response (201 Created):**
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "+1-555-0100",
  "dateOfBirth": "1985-03-15",
  "addressIds": [],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

### 2. Get All People

```http
GET https://localhost:7188/api/v1/people
```

### 3. Get Person by Email

```http
GET https://localhost:7188/api/v1/people/email/john.doe@example.com
```

### 4. Add Address to Person

First, create an address (see Addresses section), then:

```http
POST https://localhost:7188/api/v1/people/{personId}/addresses/{addressId}
```

Example:
```http
POST https://localhost:7188/api/v1/people/a1b2c3d4-e5f6-7890-abcd-ef1234567890/addresses/b2c3d4e5-f6a7-8901-bcde-f12345678901
```

## Module: Businesses

### 1. Create a Business

```http
POST https://localhost:7188/api/v1/businesses
Content-Type: application/json

{
  "name": "Acme Corporation",
  "taxId": "12-3456789",
  "email": "info@acme.com",
  "phoneNumber": "+1-555-0200",
  "website": "https://acme.com",
  "type": "Corporation"
}
```

**Business Types**: `SoleProprietorship`, `Partnership`, `Corporation`, `LLC`, `NonProfit`, `Other`

**Response (201 Created):**
```json
{
  "id": "c3d4e5f6-a7b8-9012-cdef-123456789012",
  "name": "Acme Corporation",
  "taxId": "12-3456789",
  "email": "info@acme.com",
  "phoneNumber": "+1-555-0200",
  "website": "https://acme.com",
  "type": "Corporation",
  "addressIds": [],
  "createdAt": "2024-01-15T11:00:00Z",
  "updatedAt": null
}
```

### 2. Get All Businesses

```http
GET https://localhost:7188/api/v1/businesses
```

### 3. Get Business by Tax ID

```http
GET https://localhost:7188/api/v1/businesses/taxid/12-3456789
```

### 4. Get Businesses by Type

```http
GET https://localhost:7188/api/v1/businesses/type/Corporation
```

### 5. Add Address to Business

```http
POST https://localhost:7188/api/v1/businesses/{businessId}/addresses/{addressId}
```

## Module: Addresses

### 1. Create an Address for a Person

```http
POST https://localhost:7188/api/v1/addresses
Content-Type: application/json

{
  "street": "123 Main St",
  "city": "Springfield",
  "state": "IL",
  "zipCode": "62701",
  "country": "USA",
  "type": "Person",
  "ownerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "ownerName": "John Doe"
}
```

### 2. Create an Address for a Business

```http
POST https://localhost:7188/api/v1/addresses
Content-Type: application/json

{
  "street": "456 Business Blvd, Suite 200",
  "city": "Chicago",
  "state": "IL",
  "zipCode": "60601",
  "country": "USA",
  "type": "Business",
  "ownerId": "c3d4e5f6-a7b8-9012-cdef-123456789012",
  "ownerName": "Acme Corporation"
}
```

### 3. Get All Addresses

```http
GET https://localhost:7188/api/v1/addresses
```

### 4. Get Addresses by Owner

```http
GET https://localhost:7188/api/v1/addresses/owner/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

### 5. Get Addresses by Type

```http
GET https://localhost:7188/api/v1/addresses/type/Person
```

**Address Types**: `Person`, `Business`, `Other`

### 6. Update an Address

```http
PUT https://localhost:7000/api/addresses/{id}
Content-Type: application/json

{
  "street": "789 Updated Ave",
  "city": "Springfield",
  "state": "IL",
  "zipCode": "62702",
  "country": "USA",
  "type": "Person",
  "ownerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "ownerName": "John Doe"
}
```

### 7. Delete an Address

```http
DELETE https://localhost:7000/api/addresses/{id}
```

## Complete Workflow Example

### Scenario: Create a person with an address

#### Step 1: Create the Person
```http
POST https://localhost:7000/api/people
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "phoneNumber": "+1-555-0150",
  "dateOfBirth": "1990-07-22"
}
```

Save the returned `id` (e.g., `person-id-here`)

#### Step 2: Create the Address
```http
POST https://localhost:7000/api/addresses
Content-Type: application/json

{
  "street": "789 Oak Avenue",
  "city": "Portland",
  "state": "OR",
  "zipCode": "97201",
  "country": "USA",
  "type": "Person",
  "ownerId": "person-id-here",
  "ownerName": "Jane Smith"
}
```

Save the returned `id` (e.g., `address-id-here`)

#### Step 3: Link Address to Person
```http
POST https://localhost:7000/api/people/person-id-here/addresses/address-id-here
```

#### Step 4: Verify
```http
GET https://localhost:7000/api/people/person-id-here
```

The response should show the address ID in the `addressIds` array.

## Using curl

### Create a Person
```bash
curl -X POST https://localhost:7000/api/people \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1-555-0100",
    "dateOfBirth": "1985-03-15"
  }'
```

### Create a Business
```bash
curl -X POST https://localhost:7000/api/businesses \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Tech Startup LLC",
    "taxId": "98-7654321",
    "email": "hello@techstartup.com",
    "phoneNumber": "+1-555-0300",
    "website": "https://techstartup.com",
    "type": "LLC"
  }'
```

### Create an Address
```bash
curl -X POST https://localhost:7000/api/addresses \
  -H "Content-Type: application/json" \
  -d '{
    "street": "123 Main St",
    "city": "Springfield",
    "state": "IL",
    "zipCode": "62701",
    "country": "USA",
    "type": "Person",
    "ownerId": "your-person-id",
    "ownerName": "John Doe"
  }'
```

### Get All Resources
```bash
curl https://localhost:7000/api/people
curl https://localhost:7000/api/businesses
curl https://localhost:7000/api/addresses
```

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "email": ["The email field is required."]
  }
}
```

### 404 Not Found
```json
{
  "statusCode": 404,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "statusCode": 500,
  "message": "An error occurred while processing your request.",
  "detail": "Error details here"
}
```

## Testing Tips

1. **Get the root endpoint** to see available modules:
   ```http
   GET https://localhost:7000/
   ```

2. **Use OpenAPI** for interactive testing:
   ```http
   GET https://localhost:7000/openapi/v1.json
   ```

3. **Check logs** - All requests are logged with timing information

4. **Data files** - Check `Data/` folder to see the raw JSON storage:
   - `people.json`
   - `businesses.json`
   - `addresses.json`
