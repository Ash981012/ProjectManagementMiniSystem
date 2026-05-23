namespace ProjectManagement.Domain.Projects;

public static class ProjectProgressCalculator
{
    public static decimal CalculateProjectProgress(IEnumerable<ProjectTask> tasks)
    {
        var taskList = tasks.ToList();

        if (taskList.Count == 0)
            return 0m;

        var completedTasks = taskList.Count(task => task.Status == TaskWorkflowStatus.Done);
        return Math.Round((decimal)completedTasks / taskList.Count * 100m, 2);
    }

    public static decimal CalculateOverallAverageProgress(IEnumerable<Project> projects)
    {
        var projectList = projects.ToList();

        if (projectList.Count == 0)
            return 0m;

        return Math.Round(projectList.Average(project => project.ProgressPercentage), 2);
    }
}
