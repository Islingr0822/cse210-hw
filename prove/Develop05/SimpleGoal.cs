class SimpleGoal : BaseGoal
{
    
    public SimpleGoal() : base()
    {
        
    }

    public override void CreateGoal()
    {
        SetName();
        SetDescription();
        NumberOfPoints();
    }

    public override void RecordEvent()
    {
        MarkComplete();
    }
}   
