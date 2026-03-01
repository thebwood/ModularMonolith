using BlazorModularMonolith.Web.Models;
using BlazorModularMonolith.Web.Services;

namespace BlazorModularMonolith.Web.ViewModels;

public class BusinessesViewModel : ViewModelBase
{
    private readonly IBusinessApiService _businessService;
    private readonly ILogger<BusinessesViewModel> _logger;

    public BusinessesViewModel(IBusinessApiService businessService, ILogger<BusinessesViewModel> logger)
    {
        _businessService = businessService;
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
}
