using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerAPI.Controllers
{

    // <summary>
    // Controller for managing tasks through the Task Manager API.
    // </summary>
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        public static void Main() { }
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // <summary>
        // Retrieves all tasks belonging to the authenticated user.
        // </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Task>> GetAllTasks()
        {
            // Implementation to retrieve all tasks for the authenticated user.
            return Ok(_taskRepository.GetAllTasksForUser(GetUserId()));
        }

        // <summary>
        // Retrieves a single task by its ID if it belongs to the authenticated user.
        // </summary>
        [HttpGet("{taskId}")]
        public ActionResult<Task> GetTask(int taskId)
        {
            var task = _taskRepository.GetTask(taskId);

            if (task == null || task.UserId != GetUserId())
            {
                return NotFound();
            }

            return Ok(task);
        }

        // <summary>
        // Creates a new task for the authenticated user.
        // </summary>
        [HttpPost]
        public ActionResult<Task> CreateTask(Task task)
        {
            task.UserId = GetUserId(); // Assign the user ID to the task.
            _taskRepository.AddTask(task);

            return CreatedAtAction(nameof(GetTask), new { taskId = task.Id }, task);
        }

        // <summary>
        // Updates an existing task if it belongs to the authenticated user.
        // </summary>
        [HttpPut("{taskId}")]
        public IActionResult UpdateTask(int taskId, Task task)
        {
            if (taskId != task.Id || task.UserId != GetUserId())
            {
                return BadRequest();
            }

            _taskRepository.UpdateTask(task);

            return NoContent();
        }

        // <summary>
        // Changes the completion status of a task if it belongs to the authenticated user.
        // </summary>
        [HttpPatch("{taskId}/complete")]
        public IActionResult ChangeTaskCompletion(int taskId, bool isCompleted)
        {
            var task = _taskRepository.GetTask(taskId);

            if (task == null || task.UserId != GetUserId())
            {
                return NotFound();
            }

            task.IsCompleted = isCompleted;
            _taskRepository.UpdateTask(task);

            return NoContent();
        }

        // <summary>
        // Deletes a task if it belongs to the authenticated user.
        // </summary>
        [HttpDelete("{taskId}")]
        public IActionResult DeleteTask(int taskId)
        {
            var task = _taskRepository.GetTask(taskId);

            if (task == null || task.UserId != GetUserId())
            {
                return NotFound();
            }

            _taskRepository.DeleteTask(taskId);

            return NoContent();
        }

        // Helper method to get the user ID of the authenticated user.
        private int GetUserId()
        {
            // Implementation to get the user ID from the authentication token.
            return 123; // Placeholder for demonstration.
        }
    }

    // <summary>
    // Represents a task in the Task Manager API.
    // </summary>
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public string WeatherForecast { get; } // Read-only property for weather forecast.

        public int UserId { get; set; } // User ID associated with the task.
    }

    // <summary>
    // Interface for the Task Repository to manage tasks.
    // </summary>
    public interface ITaskRepository
    {
        IEnumerable<Task> GetAllTasksForUser(int userId);
        Task GetTask(int taskId);
        void AddTask(Task task);
        void UpdateTask(Task task);
        void DeleteTask(int taskId);
    }
}
