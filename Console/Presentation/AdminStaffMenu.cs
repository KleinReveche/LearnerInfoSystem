namespace Reveche.SimpleLearnerInfoSystem.Console.Presentation;

public partial class AdminMenu
{
    private void ManageStaff()
    {
        Action[] actions = [AddStaff, UpdateStaff, RemoveStaff];
        MenuUtils.DisplayMenu("Manage Staff", actions);
    }
    
    private void AddStaff()
    {
        
    }
    
    private void UpdateStaff()
    {
    }
    
    private void RemoveStaff()
    {
    }
}