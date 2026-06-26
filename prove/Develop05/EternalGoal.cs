class EternalGoal : BaseGoal
{
    public EternalGoal() : base()
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
        AwardPoints();
    }
}