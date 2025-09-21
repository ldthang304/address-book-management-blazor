using address_book_backend.Commons.Utils;
using AddressBookManagement.Commons.Constants;
using AddressBookManagement.Models;
using AddressBookManagement.Services;
using Blazored.Toast.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MimeKit;

namespace AddressBookManagement.Shared.Components
{
    public partial class ActivityPanel
    {
        //private fields
        private List<TodoTask> taskItems = new();
        private List<TodoTask> completedTasks = new();
        private List<Master> masterTaskTypes = new();
        private List<Note> notes = new();
        private TodoTask NewTask = new();
        private Note NewNote = new();

        //Store reminded task
        private HashSet<int> remindedTaskIds = new();


        //Get contact id from parent component
        [Parameter]
        public Contact Contact { get; set; } = null!;

        [Inject]
        private ITodoTaskService TodoTaskService { get; set; } = default!;
        [Inject]
        private ILogger<ActivityPanel> Logger { get; set; } = null!;
        [Inject] 
        private IJSRuntime JS { get; set; } = default!;
        [Inject]
        private IMasterService MasterService { get; set; } = default!;
     
        [Inject]
        private IToastService ToastService { get; set; } = default!;
        
        [Inject]
        private INoteService NoteService { get; set; } = default!;
        //Task reminder
        [Inject]
        private ReminderWatcher ReminderWatcher { get; set; } = default!;
        

        protected override async Task OnInitializedAsync()
        {
            //Initialize new task in modal
            ResetNewTask();
            masterTaskTypes = await MasterService.GetByTypeNameAsync("TaskType");
            var allTasks = await TodoTaskService.GetByContactIdAsync(Contact.Id);
            notes = await NoteService.GetByContactIdAsync(Contact.Id);

            taskItems = allTasks.Where(t => !t.IsCompleted).ToList();
            completedTasks = allTasks.Where(t => t.IsCompleted).ToList();

            //Task reminder
            ReminderWatcher.OnReminderTriggered += HandleReminderTriggered;
        }

        private async Task ToggleTaskCompletionAsync(TodoTask task, bool isCompleted)
        {
            task.IsCompleted = isCompleted;
            await TodoTaskService.UpdateAsync(task); // Update vào DB

            if (isCompleted)
            {
                taskItems.Remove(task);
                completedTasks.Add(task);
            }
            else
            {
                completedTasks.Remove(task);
                taskItems.Add(task);
            }
            StateHasChanged();
        }

        private async Task HandleSubmitTask()
        {
            Logger.LogInformation("This is called");
            // Add new task
            if (NewTask.Id == 0)
            {
                await TodoTaskService.AddAsync(NewTask);
            }
            else
            {
                await TodoTaskService.UpdateAsync(NewTask);
            }
            // Get all task after update
            var allTasks = await TodoTaskService.GetByContactIdAsync(Contact.Id);

            // Rerender taskItems and completedTask
            taskItems = allTasks.Where(t => !t.IsCompleted).ToList();
            completedTasks = allTasks.Where(t => t.IsCompleted).ToList();

            // Hide modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#taskModal");
            await modalInstance.InvokeVoidAsync("hide");

            // Reset form
            ResetNewTask();
        }


        //Handle task reminder 
        private void HandleReminderTriggered(List<TodoTask> tasks)
        {
            foreach (var task in tasks)
            {
                ToastService.ShowWarning($"{task.Title} will due at {task.DueDate?.ToShortTimeString()}");
                //SendEmail(task);
            }
            // Lưu danh sách ID vào biến
            remindedTaskIds = tasks.Select(t => t.Id).ToHashSet();
            // Render lại UI
            InvokeAsync(StateHasChanged);
        }

        private async Task SendEmail(TodoTask task)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(AppConstants.AppDefaultEmail));
                email.To.Add(MailboxAddress.Parse(Contact.PersonalEmail));
                email.Subject = "Task is Dueing";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "test email"
                };

                using var smtp = new SmtpClient();

                // TODO: Configure your actual SMTP settings
                // You'll need to replace these with your actual SMTP server configuration
                await smtp.ConnectAsync(AppConstants.EmailServer, 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(AppConstants.AppDefaultEmail, AppConstants.AppEmailPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Show error toast
                // toastService.ShowError($"Failed to send email: {ex.Message}");
                Console.WriteLine($"Email send error: {ex.Message}");
            }
            finally
            {
                StateHasChanged();
            }
        }

        //Destroy reminder after component is destroyed
        public void Dispose()
        {
            ReminderWatcher.OnReminderTriggered -= HandleReminderTriggered;
        }

        private void ResetNewTask()
        {
            NewTask = new TodoTask
            {
                ContactId = Contact.Id,
                DueDate = DateTime.Now,
            };
        }
        


        private async Task ShowEditTaskModal(TodoTask task)
        {
            NewTask = await TodoTaskService.GetByIdAsync(task.Id);
            //Reset state after getting new task
            StateHasChanged();
            // show modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#taskModal");
            await modalInstance.InvokeVoidAsync("show");
        }
        private async Task ShowAddModal()
        {
            // Reset form
            ResetNewTask();

            //Reset state
            StateHasChanged();

            // Show modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#taskModal");
            await modalInstance.InvokeVoidAsync("show");
        }

        //Delete Task
        public async Task DeleteTask(TodoTask todoTask)
        {
            await TodoTaskService.DeleteAsync(todoTask.Id);
            // Get all task after update
            var allTasks = await TodoTaskService.GetByContactIdAsync(Contact.Id);

            // Rerender taskItems and completedTask
            taskItems = allTasks.Where(t => !t.IsCompleted).ToList();
            completedTasks = allTasks.Where(t => t.IsCompleted).ToList();
        }
        
        private async Task ShowNoteModal()
        {
            ResetNewNote();
            //Reset state
            StateHasChanged();

            // Show modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#noteModal");
            await modalInstance.InvokeVoidAsync("show");
        }

        //Reset Note
        private void ResetNewNote()
        {
            NewNote = new Note
            {
                ContactId = Contact.Id,
                CreatedAt = DateTime.Now,
                CreatedBy = CommonFieldUtil.GetActiveUserId()
            };
        }
        private async Task HandleSubmitNote()
        {
            // Add new task
            if (NewNote.Id == 0)
            {
                await NoteService.AddAsync(NewNote);
            }
            else
            {
                await NoteService.UpdateAsync(NewNote);
            }
            // Get all task after update
            notes = await NoteService.GetByContactIdAsync(Contact.Id);

            // Hide modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#noteModal");
            await modalInstance.InvokeVoidAsync("hide");

            // Reset form
            ResetNewNote();
        }
        private async Task ShowEditNoteModal(Note note)
        {
            NewNote = await NoteService.GetByIdAsync(note.Id);
            //Reset state after getting new task
            StateHasChanged();
            // show modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#noteModal");
            await modalInstance.InvokeVoidAsync("show");
        }

        public async Task DeleteNote(Note note)
        {
            await NoteService.DeleteAsync(note.Id);
            notes = await NoteService.GetByContactIdAsync(Contact.Id);
            StateHasChanged();
        }
    }
}