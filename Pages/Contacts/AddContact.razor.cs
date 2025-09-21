using AddressBookManagement.Models;
using AddressBookManagement.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace AddressBookManagement.Pages.Contacts
{
    public partial class AddContact
    {
        //Private fields
        private bool isInitialized = false;
        //Context to check validation state
        private EditContext? editContext;
        private ClaimsPrincipal? userContext;
        private int userId;

        // Image upload related fields
        private IBrowserFile? selectedImageFile;
        private string? imagePreviewUrl;
        private bool isImageUploading = false;
        private const int MaxImageSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] allowedImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        //Inject Services
        [Inject]
        private IContactService ContactService { get; set; } = null!;
        [Inject]
        private IOrganizationService OrganizationService { get; set; } = null!;
        [Inject]
        private IMasterService MasterService { get; set; } = null!;
        [Inject]
        private IToastService ToastService { get; set; } = null!;
        [Inject]
        private ILogger<AddContact> Logger { get; set; } = null!;
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private IWebHostEnvironment WebHostEnvironment { get; set; } = null!;

        [Parameter]
        //Contact Id from route
        public int? ContactId { get; set; }

        [SupplyParameterFromForm]
        //Contact to make operations on
        public Contact contact { get; set; } = new();

        private List<Organization> organizations = new();
        private List<Master> masters = new();
        //MasterMap after grouping all masters
        private Dictionary<string, List<Master>>? MasterMap;

        protected override async Task OnInitializedAsync()
        {
            //Initialization code
            //Get user in authen state
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            userContext = authState.User;
            userId = int.Parse(userContext.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            masters = await MasterService.GetAllAsync();
            GroupMasters();
            organizations = await OrganizationService.GetAllAsync();

            if (ContactId.HasValue)
            {
                contact = await ContactService.GetByIdAsync(ContactId.Value);
                contact.UpdatedAt = DateTime.UtcNow;
                // Set the preview URL for existing image
                if (!string.IsNullOrEmpty(contact.Image))
                {
                    imagePreviewUrl = contact.Image;
                }
            }
            else
            {
                contact.AppUserId = userId;
                contact.CreatedAt = DateTime.UtcNow;
            }
            editContext = new EditContext(contact!);
            //Finish initializing
            isInitialized = true;
        }

        private void GroupMasters()
        {
            MasterMap = masters
                .GroupBy(m => m.TypeName)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // Image upload methods
        private async Task OnImageSelected(InputFileChangeEventArgs e)
        {
            selectedImageFile = e.File;

            if (selectedImageFile != null)
            {
                // Validate file type
                if (!allowedImageTypes.Contains(selectedImageFile.ContentType.ToLower()))
                {
                    ToastService.ShowError("Please select a valid image file (JPEG, PNG, GIF).");
                    selectedImageFile = null;
                    return;
                }

                // Validate file size
                if (selectedImageFile.Size > MaxImageSize)
                {
                    ToastService.ShowError("Image size must be less than 5MB.");
                    selectedImageFile = null;
                    return;
                }

                try
                {
                    isImageUploading = true;
                    StateHasChanged();

                    // Resize image to reasonable size for preview (e.g., 300x300 max)
                    var resizedFile = await selectedImageFile.RequestImageFileAsync(
                        selectedImageFile.ContentType,
                        300, 300);

                    // Process the smaller resized image
                    const int bufferSize = 4096;
                    using var stream = resizedFile.OpenReadStream();
                    using var memoryStream = new MemoryStream();

                    var buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await memoryStream.WriteAsync(buffer, 0, bytesRead);
                        await Task.Yield(); // Keep UI responsive
                    }

                    var imageBytes = memoryStream.ToArray();
                    imagePreviewUrl = $"data:{resizedFile.ContentType};base64,{Convert.ToBase64String(imageBytes)}";
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing image preview");
                    ToastService.ShowError("Error processing image. Please try again.");
                    selectedImageFile = null;
                    imagePreviewUrl = null;
                }
                finally
                {
                    isImageUploading = false;
                    StateHasChanged();
                }
            }
        }

        //Save image 
        private async Task<string?> SaveImageAsync()
        {
            if (selectedImageFile == null)
                return contact.Image; // Return existing image path if no new image selected

            try
            {
                isImageUploading = true;
                StateHasChanged();

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(WebHostEnvironment.WebRootPath, "uploads", "contacts");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var fileExtension = Path.GetExtension(selectedImageFile.Name);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                // Save file
                await using var fileStream = new FileStream(fullPath, FileMode.Create);
                await selectedImageFile.OpenReadStream(MaxImageSize).CopyToAsync(fileStream);

                // Return relative path for database storage
                return $"/uploads/contacts/{fileName}";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving image file");
                ToastService.ShowError("Error uploading image. Please try again.");
                return contact.Image; // Return existing image path on error
            }
            finally
            {
                isImageUploading = false;
                StateHasChanged();
            }
        }

        private void RemoveImage()
        {
            selectedImageFile = null;
            imagePreviewUrl = null;
            contact.Image = null;
            StateHasChanged();
        }

        private void DeleteExistingImage()
        {
            if (!string.IsNullOrEmpty(contact.Image))
            {
                try
                {
                    var fullPath = Path.Combine(WebHostEnvironment.WebRootPath, contact.Image.TrimStart('/'));
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error deleting existing image file: {ImagePath}", contact.Image);
                }
            }
        }

        //Custom validation
        private async Task<bool> ValidateCustomRules()
        {
            var isValid = true;
            var validationMessages = new List<string>();

            // Custom validation: Check if at least one email is provided
            if (string.IsNullOrEmpty(contact.PersonalEmail) && string.IsNullOrEmpty(contact.WorkEmail))
            {
                validationMessages.Add("At least one email address is required.");
                isValid = false;
            }

            // Custom validation: Check if phone numbers are unique
            if (contact.Phones?.Count > 0)
            {
                var duplicateNumbers = contact.Phones
                    .GroupBy(p => p.Number)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicateNumbers.Any())
                {
                    validationMessages.Add($"Duplicate phone numbers found: {string.Join(", ", duplicateNumbers)}");
                    isValid = false;
                }
            }

            // Display custom validation messages
            if (validationMessages.Any())
            {
                foreach (var message in validationMessages)
                {
                    ToastService.ShowError(message);
                }
            }

            return isValid;
        }

        //Save Contact into database
        private async Task SaveAsync()
        {
            // Validate the form first
            if (editContext != null && !editContext.Validate())
            {
                ToastService.ShowError("Please fix the validation errors before saving.");
                return;
            }

            try
            {
                // Additional custom validation if needed
                if (await ValidateCustomRules())
                {
                    // Save image if selected
                    if (selectedImageFile != null)
                    {
                        // Delete old image if updating
                        if (ContactId.HasValue && !string.IsNullOrEmpty(contact.Image))
                        {
                            DeleteExistingImage();
                        }

                        contact.Image = await SaveImageAsync();
                    }

                    if (!ContactId.HasValue && contact != null)
                    {
                        await ContactService.AddAsync(contact);
                        ToastService.ShowSuccess("Contact added successfully!");
                    }
                    else
                    {
                        await ContactService.UpdateAsync(contact!);
                        ToastService.ShowSuccess("Contact updated successfully!");
                    }

                    // Navigate to contact list after saved
                    NavigationManager.NavigateTo("contacts");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving contact");
                ToastService.ShowError($"Error saving contact: {ex.Message}");
            }
        }

        private bool IsPhoneAdded(Master phoneType)
        {
            return contact?.Phones?.FirstOrDefault(p => p.PhoneType == phoneType.TypeKey) != null;
        }

        private void AddPhoneWithType(Master type)
        {
            if (contact?.Phones == null)
            {
                contact.Phones = new List<Phone>();
            }
            contact?.Phones?.Add(new Phone()
            {
                Number = "",
                PhoneType = type.TypeKey,
                ContactId = contact.Id,
                Contact = contact
            });
        }

        private void RemovePhone(Phone phone)
        {
            contact?.Phones?.Remove(phone);
        }

        private bool IsWebsiteAdded(Master websiteType)
        {
            return contact?.Websites?.FirstOrDefault(w => w.WebsiteType == websiteType.TypeKey) != null;
        }

        private void AddWebsiteWithType(Master type)
        {
            if (contact?.Websites == null)
            {
                contact.Websites = new List<Website>();
            }

            contact?.Websites?.Add(new Website()
            {
                Url = "",
                ContactId = contact.Id,
                WebsiteType = type.TypeKey,
                Contact = contact
            });
        }

        private void RemoveWebsite(Website website)
        {
            contact?.Websites?.Remove(website);
        }

        private void BackToList(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("contacts");
        }
    }
}