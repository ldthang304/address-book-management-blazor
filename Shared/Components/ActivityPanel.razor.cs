using address_book_backend.Commons.Utils;
using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

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
            }
            // Lưu danh sách ID vào biến
            remindedTaskIds = tasks.Select(t => t.Id).ToHashSet();
            // Render lại UI
            InvokeAsync(StateHasChanged);
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
        private string GetBorderClass(TodoTask task)
        {
            var now = DateTime.Now;

            if (task.DueDate < now)
            {
                return "border-danger"; // Đã quá hạn
            }

            if (remindedTaskIds.Contains(task.Id))
            {
                return "border-warning"; // Được nhắc hẹn (sắp đến hạn)
            }

            return "border-primary"; // Bình thường
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