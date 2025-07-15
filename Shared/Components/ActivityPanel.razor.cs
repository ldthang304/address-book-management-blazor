using AddressBookManagement.Models;
using AddressBookManagement.Services;
using AddressBookManagement.Services.Shared;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AddressBookManagement.Shared.Components
{
    public partial class ActivityPanel
    {
        //private fields
        private List<TodoTask> taskItems = new();
        private List<TodoTask> completedTasks = new();
        private List<Master> masterTaskTypes = new();
        private TodoTask NewTask = new();
       
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
        
        //Toast message services
        [Inject]
        private ToastNavigationService ToastNavigationService { get; set; } = null!;

        //Task reminder
        [Inject]
        private ReminderWatcher ReminderWatcher { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            //Initialize new task in modal
            ResetNewTask();
            masterTaskTypes = await MasterService.GetByTypeNameAsync("TaskType");
            var allTasks = await TodoTaskService.GetByContactIdAsync(Contact.Id);

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

        private async Task HandleValidSubmit()
        {
            //Add new tasks to database
            await TodoTaskService.AddAsync(NewTask);

            // ⬇ Refresh the list first (to update UI)
            taskItems = await TodoTaskService.GetByContactIdAsync(Contact.Id);

            //Hide the modal
            var modalInstance = await JS.InvokeAsync<IJSObjectReference>(
                "bootstrap.Modal.getOrCreateInstance", "#taskModal");
            await modalInstance.InvokeVoidAsync("hide");

            //Reset the form
            ResetNewTask();
            StateHasChanged();
        }

        //Handle task reminder 
        private void HandleReminderTriggered(List<TodoTask> tasks)
        {
            foreach (var task in tasks)
            {
                ToastNavigationService.SetMessage($"{task.Title} will due at {task.DueDate?.ToShortTimeString()}", ToastLevel.Error);
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
                ContactId = Contact.Id
            };
        }
        
    }
}