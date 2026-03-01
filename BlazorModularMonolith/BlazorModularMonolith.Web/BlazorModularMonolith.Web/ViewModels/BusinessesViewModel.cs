using BlazorModularMonolith.Web.Models;
using BlazorModularMonolith.Web.Services;

namespace BlazorModularMonolith.Web.ViewModels;

public class BusinessesViewModel : ViewModelBase
{
    private readonly IBusinessApiService _businessService;
    private readonly IAddressApiService _addressService;
    private readonly ILogger<BusinessesViewModel> _logger;

    public BusinessesViewModel(
        IBusinessApiService businessService,
        IAddressApiService addressService,
        ILogger<BusinessesViewModel> logger)
    {
        _businessService = businessService;
        _addressService = addressService;
        _logger = logger;
    }

    private List<BusinessModel> _businesses = new();
    public List<BusinessModel> Businesses
    {
        get => _businesses;
        set => SetProperty(ref _businesses, value);
    }

    private BusinessModel? _selectedBusiness;
    public BusinessModel? SelectedBusiness
    {
        get => _selectedBusiness;
        set => SetProperty(ref _selectedBusiness, value);
    }

    private bool _isCreating;
    public bool IsCreating
    {
        get => _isCreating;
        set => SetProperty(ref _isCreating, value);
    }

    private bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public CreateBusinessModel NewBusiness { get; set; } = new();
    public CreateBusinessModel EditBusiness { get; set; } = new();

    public async Task LoadBusinessesAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();
            Businesses = await _businessService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading businesses");
            SetError("Failed to load businesses. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> CreateBusinessAsync()
    {
        try
        {
            IsBusy = true;
            ClearError();
            var created = await _businessService.CreateAsync(NewBusiness);
            Businesses.Add(created);
            Businesses = new List<BusinessModel>(Businesses);
            NewBusiness = new CreateBusinessModel();
            IsCreating = false;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating business");
            SetError("Failed to create business. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void StartEdit(BusinessModel business)
    {
        SelectedBusiness = business;
        EditBusiness = new CreateBusinessModel
        {
            Name = business.Name,
            TaxId = business.TaxId,
            Email = business.Email,
            PhoneNumber = business.PhoneNumber,
            Website = business.Website,
            Type = business.Type
        };
        IsEditing = true;
    }

    public async Task<bool> UpdateBusinessAsync()
    {
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();
            var updated = await _businessService.UpdateAsync(SelectedBusiness.Id, EditBusiness);
            if (updated != null)
            {
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = updated;
                    Businesses = new List<BusinessModel>(Businesses);
                }
                IsEditing = false;
                SelectedBusiness = null;
                return true;
            }
            SetError("Business not found.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating business");
            SetError("Failed to update business. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> DeleteBusinessAsync(Guid id)
    {
        try
        {
            IsBusy = true;
            ClearError();
            var success = await _businessService.DeleteAsync(id);
            if (success)
            {
                Businesses = Businesses.Where(b => b.Id != id).ToList();
                return true;
            }
            SetError("Failed to delete business.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting business");
            SetError("Failed to delete business. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void CancelEdit()
    {
        IsEditing = false;
        IsCreating = false;
        SelectedBusiness = null;
        EditBusiness = new CreateBusinessModel();
        NewBusiness = new CreateBusinessModel();
        ClearError();
    }

    // Address Management Properties
    private List<AddressModel> _businessAddresses = new();
    public List<AddressModel> BusinessAddresses
    {
        get => _businessAddresses;
        set => SetProperty(ref _businessAddresses, value);
    }

    private List<AddressModel> _availableAddresses = new();
    public List<AddressModel> AvailableAddresses
    {
        get => _availableAddresses;
        set => SetProperty(ref _availableAddresses, value);
    }

    private bool _isManagingAddresses;
    public bool IsManagingAddresses
    {
        get => _isManagingAddresses;
        set => SetProperty(ref _isManagingAddresses, value);
    }

    private bool _isCreatingAddress;
    public bool IsCreatingAddress
    {
        get => _isCreatingAddress;
        set => SetProperty(ref _isCreatingAddress, value);
    }

    public CreateAddressModel NewAddress { get; set; } = new();

    // Address Management Methods
    public async Task StartManageAddressesAsync(BusinessModel business)
    {
        try
        {
            SelectedBusiness = business;
            IsBusy = true;
            ClearError();

            // Load business's addresses
            BusinessAddresses = await _addressService.GetByOwnerIdAsync(business.Id);

            // Load all addresses to show available ones
            var allAddresses = await _addressService.GetAllAsync();
            AvailableAddresses = allAddresses
                .Where(a => !business.AddressIds.Contains(a.Id))
                .ToList();

            IsManagingAddresses = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading addresses for business {BusinessId}", business.Id);
            SetError("Failed to load addresses. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> AddExistingAddressAsync(Guid addressId)
    {
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            var updated = await _businessService.AddAddressAsync(SelectedBusiness.Id, addressId);
            if (updated != null)
            {
                // Update the business in the list
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = updated;
                    Businesses = new List<BusinessModel>(Businesses);
                }

                // Refresh addresses
                await StartManageAddressesAsync(updated);
                return true;
            }

            SetError("Failed to add address to business.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address to business");
            SetError("Failed to add address. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<bool> RemoveAddressAsync(Guid addressId)
    {
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            var updated = await _businessService.RemoveAddressAsync(SelectedBusiness.Id, addressId);
            if (updated != null)
            {
                // Update the business in the list
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = updated;
                    Businesses = new List<BusinessModel>(Businesses);
                }

                // Refresh addresses
                await StartManageAddressesAsync(updated);
                return true;
            }

            SetError("Failed to remove address from business.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing address from business");
            SetError("Failed to remove address. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void StartCreateAddress()
    {
        if (SelectedBusiness == null) return;

        NewAddress = new CreateAddressModel
        {
            Type = AddressType.Business,
            OwnerId = SelectedBusiness.Id,
            OwnerName = SelectedBusiness.Name
        };
        IsCreatingAddress = true;
    }

    public async Task<bool> CreateAndAddAddressAsync()
    {
        if (SelectedBusiness == null) return false;

        try
        {
            IsBusy = true;
            ClearError();

            // Ensure the address is correctly configured
            NewAddress.Type = AddressType.Business;
            NewAddress.OwnerId = SelectedBusiness.Id;
            NewAddress.OwnerName = SelectedBusiness.Name;

            // Create the address
            var createdAddress = await _addressService.CreateAsync(NewAddress);

            // Add it to the business
            var updated = await _businessService.AddAddressAsync(SelectedBusiness.Id, createdAddress.Id);
            if (updated != null)
            {
                // Update the business in the list
                var index = Businesses.FindIndex(b => b.Id == SelectedBusiness.Id);
                if (index >= 0)
                {
                    Businesses[index] = updated;
                    Businesses = new List<BusinessModel>(Businesses);
                }

                // Reset form and refresh
                NewAddress = new CreateAddressModel();
                IsCreatingAddress = false;
                await StartManageAddressesAsync(updated);
                return true;
            }

            SetError("Failed to add newly created address to business.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating and adding address");
            SetError("Failed to create address. Please try again.");
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void CancelAddressManagement()
    {
        IsManagingAddresses = false;
        IsCreatingAddress = false;
        SelectedBusiness = null;
        BusinessAddresses = new List<AddressModel>();
        AvailableAddresses = new List<AddressModel>();
        NewAddress = new CreateAddressModel();
        ClearError();
    }
}
