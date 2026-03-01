# Address Management Feature

## Overview

This document describes the CRUD address management functionality that has been added to both the **People** and **Businesses** modules in the Blazor Web application.

## Features Implemented

### 1. People Module - Address Management

**UI Enhancements:**
- Added "Addresses" button in the People table for each person
- Modal dialog for managing addresses associated with a person
- Split view showing:
  - **Current Addresses**: Addresses already assigned to the person
  - **Available Addresses**: Existing addresses that can be assigned
- Create new address functionality directly from the modal
- Remove address functionality with confirmation

**ViewModel Updates (`PeopleViewModel.cs`):**
- Injected `IAddressApiService` for address operations
- Added properties:
  - `PersonAddresses` - List of addresses for the selected person
  - `AvailableAddresses` - List of addresses that can be assigned
  - `IsManagingAddresses` - Flag for showing the address management modal
  - `IsCreatingAddress` - Flag for showing the address creation form
  - `NewAddress` - Model for creating new addresses

- Added methods:
  - `StartManageAddressesAsync()` - Loads addresses when modal opens
  - `AddExistingAddressAsync()` - Assigns an existing address to the person
  - `RemoveAddressAsync()` - Removes an address from the person
  - `StartCreateAddress()` - Initializes address creation
  - `CreateAndAddAddressAsync()` - Creates a new address and assigns it to the person
  - `CancelAddressManagement()` - Closes modal and cleans up state

### 2. Businesses Module - Address Management

**UI Enhancements:**
- Added "Addresses" button in the Businesses table for each business
- Modal dialog for managing addresses associated with a business
- Split view showing:
  - **Current Addresses**: Addresses already assigned to the business
  - **Available Addresses**: Existing addresses that can be assigned
- Create new address functionality directly from the modal
- Remove address functionality with confirmation

**ViewModel Updates (`BusinessesViewModel.cs`):**
- Injected `IAddressApiService` for address operations
- Added properties (same structure as PeopleViewModel):
  - `BusinessAddresses`
  - `AvailableAddresses`
  - `IsManagingAddresses`
  - `IsCreatingAddress`
  - `NewAddress`

- Added methods (same functionality as PeopleViewModel):
  - `StartManageAddressesAsync()`
  - `AddExistingAddressAsync()`
  - `RemoveAddressAsync()`
  - `StartCreateAddress()`
  - `CreateAndAddAddressAsync()`
  - `CancelAddressManagement()`

## API Endpoints Used

### People Address Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/people/{id}/addresses/{addressId}` | Add an existing address to a person |
| DELETE | `/api/v1/people/{id}/addresses/{addressId}` | Remove an address from a person |

### Business Address Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/businesses/{id}/addresses/{addressId}` | Add an existing address to a business |
| DELETE | `/api/v1/businesses/{id}/addresses/{addressId}` | Remove an address from a business |

### Address CRUD (Used for creating new addresses)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/addresses` | Get all addresses (for available list) |
| GET | `/api/v1/addresses/owner/{ownerId}` | Get addresses by owner ID |
| POST | `/api/v1/addresses` | Create a new address |

## User Flow

### Managing Addresses for a Person/Business

1. User clicks the **"Addresses"** button next to a person or business
2. Modal opens showing:
   - Left side: Current addresses (with Remove button)
   - Right side: Available addresses (with Add button)
   - Top: "Create New Address" button
3. User can:
   - **Add existing address**: Click "Add" on any available address
   - **Remove address**: Click "Remove" on current address (with confirmation)
   - **Create new address**: Click "Create New Address" button

### Creating a New Address

1. User clicks **"Create New Address"** button
2. Address form appears with fields:
   - Street
   - City
   - State
   - Zip Code
   - Country
3. Form automatically sets:
   - `Type` = Person or Business (based on context)
   - `OwnerId` = Selected person/business ID
   - `OwnerName` = Person's full name or business name
4. User fills out form and clicks **"Create & Add"**
5. Address is created and automatically assigned
6. Modal refreshes to show the new address in "Current Addresses"

## Technical Details

### Data Flow

```
Blazor Component (People.razor/Businesses.razor)
    ↓
ViewModel (PeopleViewModel/BusinessesViewModel)
    ↓
API Service (IPersonApiService/IBusinessApiService + IAddressApiService)
    ↓
HTTP Client
    ↓
API Endpoints (PersonEndpoints/BusinessEndpoints + AddressEndpoints)
    ↓
Application Service (PersonService/BusinessService + AddressService)
    ↓
Repository (FilePersonRepository/FileBusinessRepository + FileAddressRepository)
    ↓
JSON File Storage
```

### Address Types

Addresses are typed based on their owner:
- `AddressType.Person` - For people
- `AddressType.Business` - For businesses
- `AddressType.Other` - Generic addresses

### State Management

- ViewModels maintain state for address management
- `IsBusy` flag prevents concurrent operations
- Error handling with user-friendly messages
- Automatic refresh after add/remove operations
- Confirmation dialogs for destructive actions (Remove)

## UI Components

### Modal Structure

```
┌─────────────────────────────────────────────────────────┐
│ Manage Addresses - [Person/Business Name]          [X] │
├─────────────────────────────────────────────────────────┤
│ [+ Create New Address]                                  │
│                                                          │
│ [Create Address Form - if IsCreatingAddress = true]    │
│                                                          │
│ ┌────────────────────────┬──────────────────────────┐  │
│ │ Current Addresses (N)  │ Available Addresses (N)  │  │
│ │                        │                          │  │
│ │ • Address 1 [Remove]   │ • Address A [Add]        │  │
│ │ • Address 2 [Remove]   │ • Address B [Add]        │  │
│ │ • Address 3 [Remove]   │ • Address C [Add]        │  │
│ └────────────────────────┴──────────────────────────┘  │
│                                                          │
│                                       [Close]            │
└─────────────────────────────────────────────────────────┘
```

## Files Modified

### Blazor Web Application

1. **ViewModels**
   - `BlazorModularMonolith.Web\ViewModels\PeopleViewModel.cs`
   - `BlazorModularMonolith.Web\ViewModels\BusinessesViewModel.cs`

2. **Pages**
   - `BlazorModularMonolith.Web\Components\Pages\People.razor`
   - `BlazorModularMonolith.Web\Components\Pages\Businesses.razor`

3. **Program.cs**
   - Removed obsolete reference to `BlazorModularMonolith.Web.Client.Pages`

### Cleanup

4. **Removed Files**
   - `BlazorModularMonolith.Web\Components\Pages\Weather.razor` (template demo)
   - `BlazorModularMonolith.Web.Client\Pages\Counter.razor` (template demo)

5. **Updated Navigation**
   - `BlazorModularMonolith.Web\Components\Layout\NavMenu.razor` (removed Weather link)

## API Support

The API already had full support for address management:

✅ **PersonEndpoints.cs** - Has AddAddress and RemoveAddress endpoints
✅ **BusinessEndpoints.cs** - Has AddAddress and RemoveAddress endpoints  
✅ **PersonService.cs** - Has AddAddressToPerson and RemoveAddressFromPerson methods
✅ **BusinessService.cs** - Has AddAddressToBusiness and RemoveAddressFromBusiness methods
✅ **AddressEndpoints.cs** - Full CRUD for addresses

**No API changes were required!**

## Testing

To test the address management functionality:

1. **Start both projects**:
   ```bash
   # Terminal 1 - API
   cd BlazorModularMonolith.Api
   dotnet run

   # Terminal 2 - Web
   cd BlazorModularMonolith.Web\BlazorModularMonolith.Web
   dotnet run
   ```

2. **Test People Address Management**:
   - Navigate to `/people`
   - Create a person if none exist
   - Click "Addresses" button
   - Create a new address
   - Verify it appears in "Current Addresses"
   - Create another address
   - Test removing an address

3. **Test Business Address Management**:
   - Navigate to `/businesses`
   - Create a business if none exist
   - Click "Addresses" button
   - Create a new address
   - Verify it appears in "Current Addresses"
   - Test adding existing addresses
   - Test removing addresses

4. **Test Address Sharing** (if desired):
   - Create an address for Person A
   - Go to Person B's addresses
   - Add the same address to Person B
   - Verify both people show the address

## Benefits

1. **User Experience**
   - Intuitive modal interface
   - Clear separation of current vs available addresses
   - In-place address creation
   - Confirmation dialogs for safety

2. **Code Organization**
   - Clean separation of concerns
   - Reusable patterns between People and Businesses
   - Consistent error handling
   - Type-safe operations

3. **Maintainability**
   - Centralized state management in ViewModels
   - Service abstraction for API calls
   - Easy to extend with additional address operations

4. **Scalability**
   - Ready for additional features (edit address, address validation, etc.)
   - Can easily add address types or categories
   - Supports future enhancements (bulk operations, address search, etc.)

## Future Enhancements

Potential improvements:
- Address validation (validate zip codes, states, etc.)
- Address editing capability
- Address search/filter in available list
- Bulk address operations
- Address history/audit trail
- Geolocation integration
- Address autocomplete using Google Maps API
- Primary/secondary address designation
- Address verification services

## Conclusion

The address management feature is now fully functional for both People and Businesses modules. Users can create, assign, and remove addresses through an intuitive modal interface. The implementation follows clean architecture principles and maintains consistency with the existing codebase.
